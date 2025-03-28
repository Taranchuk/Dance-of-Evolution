using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
	[HotSwappable]
	public class JobDriver_ConsumeSpores : JobDriver
	{
		protected Building_Sporemaker Sporemaker => (Building_Sporemaker)job.GetTarget(TargetIndex.A).Thing;
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}
		public override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(TargetIndex.A);
			this.FailOn(() => (pawn.IsServant() is false && pawn.IsFungalNexus() is false) || Sporemaker.Active is false);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
			yield return Toils_General.Wait(180, TargetIndex.A).WithProgressBarToilDelay(TargetIndex.A);
			yield return new Toil
			{
				initAction = () =>
				{
					var hediff = pawn.health.AddHediff(Sporemaker.sporeHediff);
					hediff.TryGetComp<HediffComp_Disappears>().ticksToDisappear = GenDate.TicksPerDay;
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}