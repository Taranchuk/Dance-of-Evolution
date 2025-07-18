using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "HealthTickInterval")]
    public static class Pawn_HealthTracker_HealthTickInterval_Patch
    {
        public static bool suppressMessage;
        public static void Prefix(Pawn_HealthTracker __instance)
        {
            if (__instance.pawn.IsServant())
            {
                suppressMessage = true;
            }
        }
        public static void Postfix()
        {
            suppressMessage = false;
        }
    }
}
