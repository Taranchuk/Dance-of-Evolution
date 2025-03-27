using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(IncidentWorker_GoldenCubeArrival), "ValidatePawn")]
    public static class IncidentWorker_GoldenCubeArrival_ValidatePawn_Patch
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            if (pawn != null && pawn.health.hediffSet.HasHediff(DefsOf.DE_FungalNexus))
            {
                __result = false;
                return;
            }
            if (!__result && pawn != null && Utils.IsServant(pawn, out _))
            {
                if (pawn.health.hediffSet.HasHediff(HediffDefOf.CubeInterest) || pawn.health.hediffSet.HasHediff(HediffDefOf.CubeComa))
                {
                    __result = false;
                }
                else
                {
                    __result = true;
                }
            }
        }
    }
}