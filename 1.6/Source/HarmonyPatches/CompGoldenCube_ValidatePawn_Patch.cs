using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(CompGoldenCube), "ValidatePawn")]
    public static class CompGoldenCube_ValidatePawn_Patch
    {
        public static bool Prefix(Pawn pawn, ref bool __result)
        {
            if (pawn != null && pawn.health.hediffSet.HasHediff(DefsOf.DE_FungalNexus))
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}