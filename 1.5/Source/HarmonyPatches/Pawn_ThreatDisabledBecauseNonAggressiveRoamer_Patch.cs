using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Pawn), "ThreatDisabledBecauseNonAggressiveRoamer")]
    public static class Pawn_ThreatDisabledBecauseNonAggressiveRoamer_Patch
    {
        public static void Postfix(Pawn __instance, ref bool __result)
        {
            if (__result && __instance.IsServant())
            {
                __result = false;
            }
        }
    }
}
