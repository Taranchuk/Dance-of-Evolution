using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
    public class JobDriver_TalkToEnvoy : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return new Toil
            {
                initAction = delegate
                {
                    Pawn targetPawn = (Pawn)job.targetA.Thing;
                    Find.WindowStack.Add(new Dialog_MycelyssEnvoy(targetPawn, pawn));
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}