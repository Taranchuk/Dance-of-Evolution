using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	public class Building_FungalNode : Building_NutrientPasteDispenser, IThingHolder, IStoreSettingsParent, IStorageGroupMember, IHaulDestination, IHaulSource, ISearchableContents
	{
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

		bool IStorageGroupMember.DrawConnectionOverlay => base.Spawned;
		Map IStorageGroupMember.Map => base.MapHeld;
		string IStorageGroupMember.StorageGroupTag => def.building.storageGroupTag;
		StorageSettings IStorageGroupMember.StoreSettings => GetStoreSettings();
		StorageSettings IStorageGroupMember.ParentStoreSettings => GetParentStoreSettings();
		StorageSettings IStorageGroupMember.ThingStoreSettings => settings;
		bool IStorageGroupMember.DrawStorageTab => true;
		bool IStorageGroupMember.ShowRenameButton => base.Faction == Faction.OfPlayer;
		public ThingOwner SearchableContents => innerContainer;
		public bool depositFood;
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
			return depositFood && GetStoreSettings().AllowedToAccept(t) && innerContainer.CanAcceptAnyOf(t);
		}
		public int SpaceRemainingFor(ThingDef _)
		{
			return StoredItems.Count() - def.building.maxItemsInCell * def.Size.Area;
		}

		public void Notify_SettingsChanged()
		{
			if (base.Spawned)
			{
				base.MapHeld.listerHaulables.Notify_HaulSourceChanged(this);
			}
		}

		public void Notify_ItemAdded(Thing item)
		{
			base.MapHeld.listerHaulables.Notify_AddedThing(item);
		}

		public void Notify_ItemRemoved(Thing item)
		{
			base.MapHeld.listerHaulables.Notify_HaulSourceChanged(this);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
			Scribe_Deep.Look(ref settings, "settings", this);
			Scribe_References.Look(ref storageGroup, "storageGroup");
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
		}

		public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
		{
			if (storageGroup != null)
			{
				storageGroup?.RemoveMember(this);
				storageGroup = null;
			}
			innerContainer?.TryDropAll(base.Position, base.Map, ThingPlaceMode.Near);
			base.DeSpawn(mode);
		}

		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
		}
		
		public override ThingDef DispensableDef => DefsOf.DE_FungalSlurry;

		[HarmonyPatch(typeof(Building_NutrientPasteDispenser), "CanDispenseNow", MethodType.Getter)]
		public static class Building_NutrientPasteDispenser_CanDispenseNow_Patch
		{
			public static bool Prefix(ref bool __result, Building_NutrientPasteDispenser __instance)
			{
				if (__instance is Building_FungalNode fungalNode)
				{
					__result = true;
					return false;
				}
				return true;
			}
		}

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
				if (__result is false && (group == ThingRequestGroup.FoodSourceNotPlantOrTree || group == ThingRequestGroup.FoodSource)
					&& typeof(Building_FungalNode) == def.thingClass)
				{
					__result = true;
				}
			}
		}
	}
}