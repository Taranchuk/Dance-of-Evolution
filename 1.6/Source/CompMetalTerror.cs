using Verse;
using System;
using RimWorld;

namespace DanceOfEvolution
{
	public class CompMetalTerror : ThingComp
	{
		private const float daysUntilShedding = 10f;
		private int ticksUntilShedding;
		public override void PostPostMake()
		{
			base.PostPostMake();
			ResetSheddingTicks();
		}

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
			var pawn = parent as Pawn;
			if (!pawn.health.hediffSet.HasHediff(DefsOf.DE_ServantSpecial))
			{
				pawn.health.AddHediff(DefsOf.DE_ServantSpecial);
			}
			if (pawn.equipment.Primary == null || pawn.equipment.Primary.def != DefsOf.DE_Gun_SporeLauncher)
			{
				var weapon = ThingMaker.MakeThing(DefsOf.DE_Gun_SporeLauncher);
				pawn.equipment.AddEquipment(weapon as ThingWithComps);
			}
		}
		private void ResetSheddingTicks()
		{
			ticksUntilShedding = (int)(daysUntilShedding * 60000);
		}

		public override void CompTick()
		{
			base.CompTick();
			ticksUntilShedding--;

			if (ticksUntilShedding <= 0)
			{
				Shed();
			}
			if (parent.Spawned)
			{
				CheckSunlight(parent.Position.InSunlight(parent.Map));
			}
		}

		public void CheckSunlight(bool isInSunlight)
		{
			var pawn = parent as Pawn;
			var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(DefsOf.DE_ConsciousnessReductionSunlight);
			if (isInSunlight)
			{
				if (hediff is null)
				{
					pawn.health.AddHediff(DefsOf.DE_ConsciousnessReductionSunlight);
				}
			}
			else
			{
				if (hediff != null)
				{
					pawn.health.RemoveHediff(hediff);
				}
			}
		}

		private void Shed()
		{
			Thing bioferrite = ThingMaker.MakeThing(ThingDefOf.Bioferrite);
			bioferrite.stackCount = 75;
			GenPlace.TryPlaceThing(bioferrite, parent.Position, parent.Map, ThingPlaceMode.Near);
			ResetSheddingTicks();
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref ticksUntilShedding, "ticksUntilShedding");
		}

		public override string CompInspectStringExtra()
		{
			if (ticksUntilShedding > 0)
			{
				return "DE_ProducesBioferriteInDays".Translate(ticksUntilShedding.ToStringTicksToPeriod());
			}
			return base.CompInspectStringExtra();
		}
	}
}
