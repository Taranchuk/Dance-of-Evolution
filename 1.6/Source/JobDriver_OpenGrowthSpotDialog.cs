using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;

namespace DanceOfEvolution
{
    public class JobDriver_OpenGrowthSpotDialog : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell).FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_General.Do(delegate
            {
                Find.WindowStack.Add(new Dialog_GrowthSpot(pawn, job.targetA.Thing));
            });
        }
    }
}