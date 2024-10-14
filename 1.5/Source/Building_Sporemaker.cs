using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
namespace DanceOfEvolution
{
	[StaticConstructorOnStartup]
	public class Building_Sporemaker : Building, IThingGlower
	{
		public CompRefuelable refuelableComp;
		public HediffDef sporeHediff;
		public int ticksSwitching;
		public bool Active => refuelableComp.HasFuel && ticksSwitching <= 0;
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			refuelableComp = GetComp<CompRefuelable>();
			base.SpawnSetup(map, respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				sporeHediff = DefsOf.DE_HangingSporesMood;
			}
		}
		
		public bool ShouldBeLitNow()
		{
			return refuelableComp.HasFuel;
		}
		
		public override void Tick()
		{
			base.Tick();
			if (ticksSwitching > 0)
			{
				ticksSwitching -= 100;
				if (ticksSwitching <= 0)
				{
					BroadcastCompSignal("CrateContentsChanged");
				}
			}
			else
			{
				refuelableComp.Notify_UsedThisTick();
			}
		}
		
		public static Texture2D Icon = ContentFinder<Texture2D>.Get("UI/Icons/Sporemaker");
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (var g in base.GetGizmos())
			{
				yield return g;
			}
			yield return new Command_ActionWithCooldown
			{
				cooldownPercentGetter = () => Mathf.InverseLerp(GenDate.TicksPerDay, 0f, ticksSwitching),
				defaultLabel = "DE_SporeMode".Translate(sporeHediff.label),
				defaultDesc = "DE_SporeModeDesc".Translate(sporeHediff.label, sporeHediff.description),
				icon = Icon,
				action = delegate
				{
					var floatList = new List<FloatMenuOption>();
					AddSporeMode(floatList, DefsOf.DE_HangingSporesMood);
					AddSporeMode(floatList, DefsOf.DE_HangingSporesConsciousness);
					AddSporeMode(floatList, DefsOf.DE_HangingSporesMoving);
					Find.WindowStack.Add(new FloatMenu(floatList));
				},
				Disabled = ticksSwitching > 0
			};
		}

		private void AddSporeMode(List<FloatMenuOption> floatList, HediffDef sporeHediff)
		{
			floatList.Add(new FloatMenuOption(sporeHediff.label, delegate
			{
				SetSpore(sporeHediff);
			}));
		}

		private void SetSpore(HediffDef sporeHediff)
		{
			this.sporeHediff = sporeHediff;
			this.ticksSwitching = GenDate.TicksPerDay;
			BroadcastCompSignal("CrateContentsChanged");
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref sporeHediff, "sporeHediff");
			Scribe_Values.Look(ref ticksSwitching, "ticksSwitching");
		}
	}
}