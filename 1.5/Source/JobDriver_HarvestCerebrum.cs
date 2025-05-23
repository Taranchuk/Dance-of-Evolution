using System.Collections.Generic;
using Verse.AI;

namespace DanceOfEvolution
{

	[HotSwappable]
	public class JobDriver_HarvestCerebrum : JobDriver
	{
		protected Building_Cerebrum Cerebrum => (Building_Cerebrum)job.GetTarget(TargetIndex.A).Thing;
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}
		public override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(TargetIndex.A);
			this.FailOn(() => (pawn.IsFungalNexus() is false) || Cerebrum.corpseCount < Building_Cerebrum.MAX_CORPSE_TO_HARVEST);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
			yield return Toils_General.Wait(180, TargetIndex.A).WithProgressBarToilDelay(TargetIndex.A);
			yield return new Toil
			{
				initAction = () =>
				{
					Cerebrum.Destroy();
					var fungalNexus = pawn.GetFungalNexus();
					fungalNexus.servantCountOffset++;
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}