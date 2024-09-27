using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace DanceOfEvolution
{
	public class JobDriver_ReanimateCorpse : JobDriver
	{
		private const int ReanimateDuration = 4000; // Duration of the reanimation process
		private Sustainer riseSustainer;
		private Effecter riseEffecter;
		protected Corpse Corpse => (Corpse)job.GetTarget(TargetIndex.A).Thing;
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Corpse, job, 1, -1, null, errorOnFailed);
		}

		public override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);

			// Move to the corpse
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

			// Reanimate the corpse
			Toil reanimate = new Toil
			{
				tickAction = () =>
				{
					if (riseSustainer == null || riseSustainer.Ended)
					{
						SoundInfo info = SoundInfo.InMap(Corpse, MaintenanceType.PerTick);
						riseSustainer = SoundDefOf.Pawn_Shambler_Rise.TrySpawnSustainer(info);
					}
					if (riseEffecter == null)
					{
						riseEffecter = EffecterDefOf.ShamblerRaise.Spawn(Corpse, Corpse.Map);
					}
					riseSustainer.Maintain();
					riseEffecter.EffectTick(Corpse, TargetInfo.Invalid);
				},
				defaultCompleteMode = ToilCompleteMode.Delay,
				defaultDuration = ReanimateDuration
			};
			reanimate.WithProgressBarToilDelay(TargetIndex.A);
			yield return reanimate;

			// Finalize reanimation
			yield return new Toil
			{
				initAction = () =>
				{
					InfestCorpse();
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}

		private void InfestCorpse()
		{
			if (Corpse != null && Corpse.InnerPawn != null)
			{

			}
		}
	}
}