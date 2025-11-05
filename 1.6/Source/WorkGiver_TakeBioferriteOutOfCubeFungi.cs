using Verse;
using Verse.AI;
using RimWorld;

namespace DanceOfEvolution
{
    public class WorkGiver_TakeBioferriteOutOfCubeFungi : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(DefsOf.DE_CubeFungi);

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return false;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Building_CubeFungi { unloadingEnabled: not false, ReadyForHauling: not false }))
            {
                return false;
            }
            if (t.IsBurning())
            {
                return false;
            }
            if (!pawn.CanReserve(t, 1, -1, null, forced))
            {
                return false;
            }
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(DefsOf.DE_TakeBioferriteOutOfCubeFungi, t);
        }
    }
}