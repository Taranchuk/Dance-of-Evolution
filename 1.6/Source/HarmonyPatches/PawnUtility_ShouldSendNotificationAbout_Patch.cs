using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(PawnUtility), "ShouldSendNotificationAbout")]
    public static class PawnUtility_ShouldSendNotificationAbout_Patch
    {
        public static bool Prefix(Pawn p)
        {
            if (Pawn_HealthTracker_HealthTickInterval_Patch.suppressMessage)
            {
                return false;
            }
            return true;
        }
    }
}
