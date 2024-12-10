using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HotSwappable]
	public class MycelialTree : Plant
	{
		private const float ConsumingGrowthRateFactor = 1.5f;

		private const int TwistedFleshCountAtMaxGrowth = 30;

		private static readonly FloatRange NutritionForNewTreeRange = new FloatRange(65f, 95f);

		private static readonly CachedTexture CreateCorpseStockpileIcon = new CachedTexture("UI/Icons/CorpseStockpileZone");

		private static readonly SimpleCurve ConsumeRadiusByGrowthCurve = new SimpleCurve
	{
		new CurvePoint(0.15f, 3.9f),
		new CurvePoint(0.85f, 5.9f)
	};

		private float nutrition;

		private Dictionary<ThingWithComps, Mote> roots;

		private List<ThingWithComps> rootTargets;

		private Queue<ThingWithComps> deferredDestroy = new Queue<ThingWithComps>();

		private List<Thing> tmpThings = new List<Thing>();

		private List<IntVec3> tmpRadialCells = new List<IntVec3>();

		public bool consumeFlesh;
		public bool consumeDessicated = true;
		public bool consumeFungalCorpses = true;

		private bool ConsumableNearby => rootTargets.Count > 0;

		public float ConsumeRadius => ConsumeRadiusByGrowthCurve.Evaluate(Growth);

		public override float GrowthRate => base.GrowthRate * GrowthRateFactor_Consuming * GrowthRateFactor_RottenSoil;

		public int? ticksFullGrowth;
		private float GrowthRateFactor_Consuming
		{
			get
			{
				if (!ConsumableNearby)
				{
					if (Growth < 0.1f)
					{
						return 1f;
					}
					return 0;
				}
				return 1f;
			}
		}

		private float GrowthRateFactor_RottenSoil
		{
			get
			{
				var terrain = Position.GetTerrain(Map);
				if (terrain == DefsOf.DE_RottenSoil)
				{
					return 1.5f;
				}
				return 1f;
			}
		}

		private IEnumerable<IntVec3> RadialCells => GenRadial.RadialCellsAround(base.Position, ConsumeRadius, useCenter: true);
		public override string GrowthRateCalcDesc
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(base.GrowthRateCalcDesc);
				if (GrowthRateFactor_Consuming != 1f)
				{
					stringBuilder.AppendInNewLine("StatsReport_MultiplierFor".Translate("FleshConsumption".Translate()) + ": " + GrowthRateFactor_Consuming.ToStringPercent());
				}
				return stringBuilder.ToString();
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref rootTargets, "rootTargets", LookMode.Reference);
			Scribe_Values.Look(ref nutrition, "nutrition", 0f);
			Scribe_Values.Look(ref consumeFlesh, "consumeFlesh");
			Scribe_Values.Look(ref consumeDessicated, "consumeDessicated", true);
			Scribe_Values.Look(ref consumeFungalCorpses, "consumeFungalCorpses", true);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				rootTargets.RemoveAll((ThingWithComps x) => x?.Destroyed ?? true);
			}
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			if (!ModLister.CheckAnomaly("Harbinger tree"))
			{
				Destroy();
				return;
			}
			base.SpawnSetup(map, respawningAfterLoad);
			LongEventHandler.ExecuteWhenFinished(UpdateRoots);
		}

		public override void TickLong()
		{
			base.TickLong();
			UpdateRoots();
			if (Growth >= 1 && ticksFullGrowth.HasValue is false)
			{
				ticksFullGrowth = Find.TickManager.TicksGame;
			}
			if (ticksFullGrowth.HasValue && Find.TickManager.TicksGame >= ticksFullGrowth.Value + (GenDate.TicksPerDay * 2))
			{
				var pos = Position;
				var map = Map;
				int num = this.YieldNow();
				if (num > 0)
				{
					Thing thing = ThingMaker.MakeThing(def.plant.harvestedThingDef);
					thing.stackCount = num;
					GenPlace.TryPlaceThing(thing, pos, map, ThingPlaceMode.Near);
				}
				Destroy(DestroyMode.KillFinalizeLeavingsOnly);
			}
		}

		private void UpdateRoots()
		{
			if (roots == null)
			{
				roots = new Dictionary<ThingWithComps, Mote>();
			}
			if (rootTargets == null)
			{
				rootTargets = new List<ThingWithComps>();
			}
			rootTargets.RemoveAll(x => x.TryGetComp<CompMycelialTreeConsumable>(out var comp) && comp.CanBeConsumedBy(this) is false);
			tmpRadialCells.Clear();
			tmpRadialCells.AddRange(RadialCells.ToList());
			foreach (ThingWithComps rootTarget in rootTargets)
			{
				TryMakeRoot(rootTarget);
			}
			foreach (IntVec3 tmpRadialCell in tmpRadialCells)
			{
				if (!tmpRadialCell.InBounds(base.Map))
				{
					continue;
				}
				tmpThings.Clear();
				tmpThings.AddRange(tmpRadialCell.GetThingList(base.Map));
				foreach (Thing tmpThing in tmpThings)
				{
					if (tmpThing is ThingWithComps thing && thing.TryGetComp<CompMycelialTreeConsumable>(out var comp) && comp.CanBeConsumedBy(this))
					{
						TryMakeRoot(thing);
					}
				}
			}
			foreach (ThingWithComps rootTarget2 in rootTargets)
			{
				if (rootTarget2.Destroyed || !tmpRadialCells.Contains(rootTarget2.PositionHeld)
				 || rootTarget2.TryGetComp<CompMycelialTreeConsumable>(out var comp) && comp.CanBeConsumedBy(this) is false)
				{
					deferredDestroy.Enqueue(rootTarget2);
				}
			}
			while (deferredDestroy.Count > 0)
			{
				ThingWithComps thingWithComps = deferredDestroy.Dequeue();
				if (roots.TryGetValue(thingWithComps, out var value))
				{
					value?.Destroy();
				}
				roots.Remove(thingWithComps);
				rootTargets.Remove(thingWithComps);
			}
		}

		private void TryMakeRoot(ThingWithComps thing)
		{
			if (!roots.ContainsKey(thing) || thing.Position.GetThingList(base.Map)
				.Any(x => x.def == ThingDefOf.Mote_HarbingerTreeRoots) is false)
			{
				float exactRot = 0f;
				if (thing is Corpse corpse)
				{
					exactRot = corpse.InnerPawn.Drawer.renderer.BodyAngle(PawnRenderFlags.None);
				}
				roots[thing] = MoteMaker.MakeStaticMote(thing.Position.ToVector3Shifted(), base.Map, ThingDefOf.Mote_HarbingerTreeRoots, 1f, makeOffscreen: false, exactRot);
			}
			if (!rootTargets.Contains(thing))
			{
				rootTargets.Add(thing);
			}
		}

		public void AddNutrition(float amt)
		{
			if (!(amt <= 0f))
			{
				nutrition += amt;
			}
		}

		private void CreateCorpseStockpile()
		{
			List<MycelialTree> selectedTrees = Find.Selector.SelectedObjects.OfType<MycelialTree>().ToList();
			if (base.Map.zoneManager.ZoneAt(base.Position) != null)
			{
				Zone_Stockpile existing;
				if ((existing = base.Map.zoneManager.ZoneAt(base.Position) as Zone_Stockpile) == null)
				{
					return;
				}
				base.Map.floodFiller.FloodFill(base.Position, (IntVec3 c) => selectedTrees.Any((MycelialTree tree) => tree.RadialCells.Contains(c)) && (base.Map.zoneManager.ZoneAt(c) == null || base.Map.zoneManager.ZoneAt(c) == existing) && (bool)Designator_ZoneAdd.IsZoneableCell(c, base.Map), delegate (IntVec3 c)
				{
					if (!existing.ContainsCell(c))
					{
						existing.AddCell(c);
					}
				});
				return;
			}
			Zone_Stockpile stockpile = new Zone_Stockpile(StorageSettingsPreset.CorpseStockpile, base.Map.zoneManager);
			stockpile.settings.filter.SetAllow(ThingCategoryDefOf.CorpsesMechanoid, allow: false);
			stockpile.settings.filter.SetAllow(SpecialThingFilterDefOf.AllowCorpsesUnnatural, allow: false);
			base.Map.zoneManager.RegisterZone(stockpile);
			Zone_Stockpile existingStockpile = null;
			base.Map.floodFiller.FloodFill(base.Position, delegate (IntVec3 c)
			{
				if (base.Map.zoneManager.ZoneAt(c) is Zone_Stockpile zone_Stockpile)
				{
					existingStockpile = zone_Stockpile;
				}
				return selectedTrees.Any((MycelialTree tree) => tree.RadialCells.Contains(c)) && base.Map.zoneManager.ZoneAt(c) == null && (bool)Designator_ZoneAdd.IsZoneableCell(c, base.Map);
			}, delegate (IntVec3 c)
			{
				stockpile.AddCell(c);
			});
			if (existingStockpile == null)
			{
				return;
			}
			List<IntVec3> list = stockpile.Cells.ToList();
			stockpile.Delete();
			foreach (IntVec3 item in list)
			{
				existingStockpile.AddCell(item);
			}
		}

		public override void DrawExtraSelectionOverlays()
		{
			base.DrawExtraSelectionOverlays();
			GenDraw.DrawRadiusRing(base.Position, ConsumeRadius);
		}

		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.GetInspectString());
			stringBuilder.AppendLineIfNotEmpty();
			if (DebugSettings.godMode)
			{
				stringBuilder.AppendLine("DEV: Nutrition: " + nutrition.ToString("F2"));
			}
			if (ConsumableNearby)
			{
				stringBuilder.AppendLine("HarbingerTreeConsuming".Translate());
			}
			else
			{
				stringBuilder.AppendLine("HarbingerTreeNotConsuming".Translate());
			}
			return stringBuilder.ToString().TrimEndNewlines();
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			if (!(base.Map.zoneManager.ZoneAt(base.Position) is Zone_Stockpile) || base.Map.zoneManager.ZoneAt(base.Position) != null)
			{
				yield return new Command_Action
				{
					defaultLabel = "CreateCorpseStockpile".Translate(),
					defaultDesc = "CreateCorpseStockpileDesc".Translate(),
					icon = CreateCorpseStockpileIcon.Texture,
					action = CreateCorpseStockpile
				};
			}

			yield return new Command_Toggle
			{
				defaultLabel = "DE_ConsumeFleshCorpses".Translate(),
				isActive = () => consumeFlesh,
				toggleAction = delegate
				{
					consumeFlesh = !consumeFlesh;
					UpdateRoots();
				}
			};
			yield return new Command_Toggle
			{
				defaultLabel = "DE_ConsumeDessicatedCorpses".Translate(),
				isActive = () => consumeDessicated,
				toggleAction = delegate
				{
					consumeDessicated = !consumeDessicated;
					UpdateRoots();
				}
			};
			yield return new Command_Toggle
			{
				defaultLabel = "DE_ConsumeFungalCorpses".Translate(),
				isActive = () => consumeFungalCorpses,
				toggleAction = delegate
				{
					consumeFungalCorpses = !consumeFungalCorpses;
					UpdateRoots();
				}
			};

			if (!DebugSettings.ShowDevGizmos)
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "DEV: Add 10 nutrition",
				action = delegate
				{
					AddNutrition(10f);
				}
			};
			yield return new Command_Action
			{
				defaultLabel = "DEV: Update Roots",
				action = UpdateRoots
			};
			yield return new Command_Action
			{
				defaultLabel = "DEV: Blood Spatters (Delay)",
				action = delegate
				{
					List<FloatMenuOption> list = new List<FloatMenuOption>();
					for (int i = 1; i < 10; i++)
					{
						int delay = i * 60;
						list.Add(new FloatMenuOption(delay.TicksToSeconds() + "s", delegate
						{
							DelayedSplatter(delay);
						}));
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
			};
			void DelayedSplatter(int ticks)
			{
				EffecterDef effecterDef = new EffecterDef
				{
					maintainTicks = ticks + 5
				};
				SubEffecterDef item = new SubEffecterDef
				{
					subEffecterClass = typeof(SubEffecter_GroupedChance),
					chancePerTick = 1f,
					children = new List<SubEffecterDef>(EffecterDefOf.HarbingerTreeConsume.children),
					initialDelayTicks = ticks,
					subTriggerOnSpawn = false
				};
				effecterDef.children = new List<SubEffecterDef> { item };
				foreach (ThingWithComps rootTarget in rootTargets)
				{
					effecterDef.SpawnMaintained(rootTarget, rootTarget);
				}
			}
		}
	}
}
