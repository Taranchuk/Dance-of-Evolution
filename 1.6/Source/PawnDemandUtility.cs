using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    public static class PawnDemandUtility
    {
        public static bool IsValidDemandPawn(Pawn pawn)
        {
            if (pawn == null || pawn.Dead || pawn.Destroyed || pawn.Downed)
                return false;
            if (!pawn.RaceProps.Humanlike)
                return false;
            if (pawn.Faction != Faction.OfPlayer)
                return false;
            if (pawn.health.hediffSet.HasHediff(DefsOf.DE_ServantGhoul))
                return false;
            if (pawn.health.hediffSet.HasHediff(DefsOf.DE_FungalNexus))
                return false;

            return true;
        }
        public static List<Pawn> GetValidDemandPawns()
        {
            return PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction
                .Where(p => IsValidDemandPawn(p))
                .ToList();
        }
        public static bool CanMeetDemand(int requiredCount)
        {
            return GetValidDemandPawns().Count >= requiredCount;
        }
        public static void DeliverPawns(List<Pawn> deliveredPawns, Lord lord)
        {
            foreach (Pawn pawn in deliveredPawns)
            {
                pawn.SetFaction(lord.faction);
                pawn.jobs.StopAll();
                lord.AddPawn(pawn);
            }
            lord.ReceiveMemo("PawnsDelivered");
        }
    }
}
