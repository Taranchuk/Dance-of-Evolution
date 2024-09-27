using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace DanceOfEvolution
{
	public class JobDriver_ReanimateCorpse : JobDriver
	{
		protected Corpse Corpse => (Corpse)job.GetTarget(TargetIndex.A).Thing;
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Corpse, job, 1, -1, null, errorOnFailed);
		}

		public override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
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
			GameComponent_ReanimateCorpses gameComponent = Current.Game.GetComponent<GameComponent_ReanimateCorpses>();
			gameComponent.AddInfectedCorpse(Corpse, pawn.Faction);
			pawn.Destroy();
		}
	}
}