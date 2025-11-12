using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace DanceOfEvolution
{
    public class JobDriver_OfferPawns : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil toil = new Toil();
            toil.initAction = delegate ()
            {
                Pawn actor = toil.actor;
                Pawn envoy = (Pawn)actor.CurJob.targetA.Thing;
                Find.WindowStack.Add(new Window_SelectPawnsForDemand(envoy));
            };
            yield return toil;
        }
    }
}
