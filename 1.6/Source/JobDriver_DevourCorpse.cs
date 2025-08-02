using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
	public class JobDriver_DevourCorpse : JobDriver
	{
		private Corpse Corpse => (Corpse)job.targetA.Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Corpse, job, 1, -1, null, errorOnFailed);
		}

		public override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOnBurningImmobile(TargetIndex.A);

			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

			Toil devourToil = ToilMaker.MakeToil();
			devourToil.initAction = delegate
			{
				var livingDress = pawn.apparel?.WornApparel?.FirstOrDefault(a => a.def == DefsOf.DE_LivingDress);
				var comp = livingDress?.GetComp<CompLivingDress>();
				if (comp != null && comp.BondedPawn == pawn)
				{
					comp.DevourCorpse(Corpse);
				}
			};
			devourToil.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return devourToil;
		}
	}
}
