using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(HediffSet), "AddDirect")]
    public static class HediffSet_AddDirect_Patch
    {
        [HarmonyPriority(int.MaxValue)]
        private static bool Prefix(HediffSet __instance, Pawn ___pawn, Hediff hediff)
        {
            if (___pawn.IsImmuneTo(hediff))
            {
                return false;
            }
            return true;
        }
    }
}
