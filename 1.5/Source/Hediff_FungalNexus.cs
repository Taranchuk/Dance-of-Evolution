using RimWorld;
using Verse;
using System.Collections.Generic;

namespace DanceOfEvolution
{
	[HotSwappable]
	public class Hediff_FungalNexus : HediffWithComps
	{
		private int timer = 0;
		private List<Pawn> burrowers = new();
		private int MaxServants => 4;
		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo); 
			if (!pawn.Inhumanized())
			{
				pawn.health.AddHediff(HediffDefOf.Inhumanized);
			}
			burrowers = new();
			timer = 0;
		}

		public override void PostTick()
		{
			base.PostTick();
			if (pawn.Spawned)
			{
				timer++;
				if (timer >= GenDate.TicksPerDay * 2f)
				{
					CheckAndUpdateBurrowers();
					timer = 0;
				}
			}
		}

		private void CheckAndUpdateBurrowers()
		{
			burrowers.RemoveAll(x => x is null || x.Destroyed || x.Dead);
			if (burrowers.Count < MaxServants)
			{
				SpawnBurrower();
			}
		}

		private void SpawnBurrower()
		{
			var newBurrower = PawnGenerator.GeneratePawn(DefsOf.DE_Burrower, pawn.Faction);
			burrowers.Add(newBurrower);
			GenSpawn.Spawn(newBurrower, pawn.Position, pawn.Map);
		}

		public bool IsImmuneTo(Hediff other)
		{
			if (HediffDefOf.LungRotExposure == other.def || HediffDefOf.LungRot == other.def)
			{
				return true;
			}
			return false;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref burrowers, "burrowers", LookMode.Reference);
			Scribe_Values.Look(ref timer, "timer");
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				burrowers ??= new();
			}
		}
	}
}
