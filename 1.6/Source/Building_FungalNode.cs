using System.Linq;
using UnityEngine;
using Verse.Sound;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
namespace DanceOfEvolution
{
	public class Building_FungalNode : Building_NutrientPasteDispenser, IThingHolder, IStoreSettingsParent, IStorageGroupMember, IHaulDestination, IHaulSource, ISearchableContents
	{
		private const int MaxNutrition = 200;
		private ThingOwner<Thing> innerContainer;
		private StorageSettings settings;
		private StorageGroup storageGroup;
		public IEnumerable<Thing> StoredItems => innerContainer.InnerListForReading;
		public bool StorageTabVisible => true;
		StorageGroup IStorageGroupMember.Group
		{
			get
			{
				return storageGroup;
			}
			set
			{
				storageGroup = value;
			}
		}

		bool IStorageGroupMember.DrawConnectionOverlay => Spawned;
		Map IStorageGroupMember.Map => MapHeld;
		string IStorageGroupMember.StorageGroupTag => def.building.storageGroupTag;
		StorageSettings IStorageGroupMember.StoreSettings => GetStoreSettings();
		StorageSettings IStorageGroupMember.ParentStoreSettings => GetParentStoreSettings();
		StorageSettings IStorageGroupMember.ThingStoreSettings => settings;
		bool IStorageGroupMember.DrawStorageTab => true;
		bool IStorageGroupMember.ShowRenameButton => Faction == Faction.OfPlayer;
		public ThingOwner SearchableContents => innerContainer;
		public bool depositFood = true;
		public Building_FungalNode()
		{
			innerContainer = new ThingOwner<Thing>(this, oneStackOnly: false);
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			powerComp = new CompPowerTrader
			{
				parent = this
			};
			powerComp.powerOnInt = true;
			foreach (var cell in GenAdj.CellsOccupiedBy(this))
			{
				var terrain = cell.GetTerrain(map);
				if (TerrainValidator(terrain))
				{
					ConvertTerrain(cell, map);
				}
			}
		}

		protected virtual void ConvertTerrain(IntVec3 cell, Map map)
		{
			ConvertTerrainStatic(cell, map);
		}

		public override void PostMake()
		{
			base.PostMake();
			if (def.building.defaultStorageSettings.filter.allowedDefs != null)
			{
				def.building.defaultStorageSettings.filter.allowedDefs.Remove(DefsOf.DE_FungalSlurry);
				def.building.defaultStorageSettings.filter.allowedDefs.RemoveWhere(x => x.IsNutritionGivingIngestible is false);
				def.building.defaultStorageSettings.filter.allowedDefs.RemoveWhere(x => x.IsCorpse && x.race.IsMechanoid);
			}
			if (def.building.defaultStorageSettings.filter.thingDefs != null)
			{
				def.building.defaultStorageSettings.filter.thingDefs.Remove(DefsOf.DE_FungalSlurry);
				def.building.defaultStorageSettings.filter.thingDefs.RemoveWhere(x => x.IsNutritionGivingIngestible is false);
				def.building.defaultStorageSettings.filter.thingDefs.RemoveWhere(x => x.IsCorpse && x.race.IsMechanoid);
			}
			settings = new StorageSettings(this);
			if (def.building.defaultStorageSettings != null)
			{
				settings.CopyFrom(def.building.defaultStorageSettings);
			}
		}

		public StorageSettings GetStoreSettings()
		{
			return settings;
		}

		public StorageSettings GetParentStoreSettings()
		{
			return def.building.fixedStorageSettings;
		}

		public ThingOwner GetDirectlyHeldThings()
		{
			return innerContainer;
		}
		public bool Accepts(Thing t)
		{
			var nutrition = t.GetStatValue(StatDefOf.Nutrition) * t.stackCount;
			if (nutrition <= 0f)
			{
				return false;
			}
			float currentNutrition = CurrentNutrition();
			if (currentNutrition + nutrition > MaxNutrition)
			{
				return false;
			}
			var result = depositFood && GetStoreSettings().AllowedToAccept(t) && innerContainer.CanAcceptAnyOf(t);
			return result;
		}

		private float CurrentNutrition()
		{
			return StoredItems.Sum(x => x.GetStatValue(StatDefOf.Nutrition) * x.stackCount);
		}

		public int SpaceRemainingFor(ThingDef _)
		{
			return (int)(MaxNutrition - CurrentNutrition());
		}

		public void Notify_SettingsChanged()
		{
			if (Spawned)
			{
				MapHeld.listerHaulables.Notify_HaulSourceChanged(this);
			}
		}

		public override void Tick()
		{
			base.Tick();
			if (Spawned && this.IsHashIntervalTick(300))
			{
				SpreadTerrain(this.Map, this.Position);
			}
		}

		private static void ExecuteSpreadTerrain(Map map, IntVec3 Position, System.Func<TerrainDef, bool> terrainValidator, System.Action<IntVec3, Map> terrainConverter)
		{
			foreach (var cell in GenRadial.RadialCellsAround(Position, 70f, true))
			{
				if (!cell.InBounds(map))
				{
					continue;
				}
				var terrain = cell.GetTerrain(map);
				if (terrainValidator(terrain))
				{
					terrainConverter(cell, map);
					cell.GetThingList(map).Where(x => x.def.IsPlant && x.def.CanSpawnOnFungalTerrain() is false).ToList().Do(x => x.Destroy());
					break;
				}
			}
		}

		protected virtual void SpreadTerrain(Map map, IntVec3 Position)
		{
			ExecuteSpreadTerrain(map, Position, TerrainValidator, ConvertTerrain);
		}

		public static void SpreadTerrainStatic(Map map, IntVec3 Position)
		{
			ExecuteSpreadTerrain(map, Position, TerrainValidatorStatic, ConvertTerrainStatic);
		}

		protected static void ConvertTerrainStatic(IntVec3 cell, Map map)
		{
			map.terrainGrid.SetTerrain(cell, DefsOf.DE_RottenSoil);
		}

		protected static bool TerrainValidatorStatic(TerrainDef terrain)
		{
			return terrain.IsFungalTerrain() is false && terrain != TerrainDefOf.Space && terrain.IsWater is false;
		}

		protected virtual bool TerrainValidator(TerrainDef terrain)
		{
			return TerrainValidatorStatic(terrain);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
			Scribe_Deep.Look(ref settings, "settings", this);
			Scribe_References.Look(ref storageGroup, "storageGroup");
			Scribe_Values.Look(ref depositFood, "depositFood", true);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			Designator_Build designator_Build = BuildCopyCommandUtility.FindAllowedDesignator(ThingDefOf.Hopper);
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				if (gizmo is Designator_Build build && designator_Build.defaultLabel != build.defaultLabel)
				{
					yield return gizmo;
				}
			}

			yield return new Command_Toggle
			{
				defaultLabel = "DE_DepositFood".Translate(),
				defaultDesc = "DE_DepositFoodDesc".Translate(),
				icon = ThingDefOf.RawPotatoes.uiIcon,
				isActive = () => depositFood,
				toggleAction = () => depositFood = !depositFood
			};

			var printMealCommand = new Command_Action
			{
				defaultLabel = "DE_PrintMeal".Translate(),
				defaultDesc = "DE_PrintMealDesc".Translate(),
				icon = DefsOf.DE_FungalSlurry.uiIcon,
				action = delegate
				{
					if (CanDispenseNow)
					{
						var meal = TryDispenseFood();
						if (meal != null)
						{
							GenPlace.TryPlaceThing(meal, this.InteractionCell, Map, ThingPlaceMode.Near);
						}
					}
				}
			};

			if (!CanDispenseNow)
			{
				printMealCommand.Disable("DE_NotEnoughFoodToPrintMeal".Translate());
			}

			yield return printMealCommand;
		}

		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			base.Destroy(mode);
			if (storageGroup != null)
			{
				storageGroup?.RemoveMember(this);
				storageGroup = null;
			}
		}

		public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
		{
			if (mode != DestroyMode.WillReplace)
			{
				innerContainer?.TryDropAll(Position, Map, ThingPlaceMode.Near);
			}
			base.DeSpawn(mode);
		}

		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
		}

		public override ThingDef DispensableDef => DefsOf.DE_FungalSlurry;

		public bool HaulDestinationEnabled => true;

		public bool HaulSourceEnabled => false;

		[HarmonyPatch(typeof(Alert_PasteDispenserNeedsHopper), "BadDispensers", MethodType.Getter)]
		public static class Alert_PasteDispenserNeedsHopper_BadDispensers_Patch
		{
			public static void Postfix(ref List<Thing> ___badDispensersResult)
			{
				___badDispensersResult.RemoveAll(d => d is Building_FungalNode);
			}
		}

		[HarmonyPatch(typeof(ThingListGroupHelper), "Includes")]
		public static class ThingListGroupHelper_Includes
		{
			public static void Postfix(ThingDef def, ThingRequestGroup group, ref bool __result)
			{
				if (__result is false && (group == ThingRequestGroup.FoodSourceNotPlantOrTree || group == ThingRequestGroup.FoodSource) && typeof(Building_FungalNode).IsAssignableFrom(def.thingClass))
				{
					__result = true;
				}
			}
		}

		public override Thing TryDispenseFood()
		{
			if (!CanDispenseNow)
			{
				return null;
			}

			float remainingNutrition = def.building.nutritionCostPerDispense - 0.0001f;
			List<ThingDef> ingredientsList = new List<ThingDef>();

			do
			{
				Thing feedItem = FindFeedInAnyHopper();
				if (feedItem == null)
				{
					Log.Error("Did not find enough food in the inner container while trying to dispense.");
					return null;
				}

				int numToSplit = Mathf.Min(feedItem.stackCount, Mathf.CeilToInt(remainingNutrition / feedItem.GetStatValue(StatDefOf.Nutrition)));
				remainingNutrition -= numToSplit * feedItem.GetStatValue(StatDefOf.Nutrition);
				ingredientsList.Add(feedItem.def);
				feedItem.SplitOff(numToSplit);
			}
			while (remainingNutrition > 0f);

			def.building.soundDispense.PlayOneShot(new TargetInfo(Position, Map));
			Thing meal = ThingMaker.MakeThing(DispensableDef);
			CompIngredients compIngredients = meal.TryGetComp<CompIngredients>();
			for (int i = 0; i < ingredientsList.Count; i++)
			{
				compIngredients.RegisterIngredient(ingredientsList[i]);
			}
			return meal;
		}

		public override Thing FindFeedInAnyHopper()
		{
			foreach (Thing item in innerContainer)
			{
				if (IsAcceptableFeedstock(item.def))
				{
					return item;
				}
			}
			return null;
		}

		public override bool HasEnoughFeedstockInHoppers()
		{
			float totalNutrition = 0f;
			foreach (Thing item in innerContainer)
			{
				if (IsAcceptableFeedstock(item.def))
				{
					totalNutrition += item.stackCount * item.GetStatValue(StatDefOf.Nutrition);
				}
				if (totalNutrition >= def.building.nutritionCostPerDispense)
				{
					return true;
				}
			}
			return false;
		}
	}
}
