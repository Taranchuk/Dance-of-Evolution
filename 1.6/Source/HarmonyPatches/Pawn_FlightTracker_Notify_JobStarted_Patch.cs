using HarmonyLib;
using Verse.AI;
using Verse;
using RimWorld;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Pawn_FlightTracker), "Notify_JobStarted")]
    public static class Pawn_FlightTracker_Notify_JobStarted_Patch
    {
        public static bool Prefix(Pawn_FlightTracker __instance, Job job)
        {
            if (__instance.pawn.IsServant())
            {
                if (__instance.pawn.mindState.IsIdle || job.def.isIdle || job.def == JobDefOf.GotoWander)
                {
                    job.flying = false;
                    if (__instance.Flying)
                    {
                        __instance.ForceLand();
                    }
                    return false;
                }
                if (__instance.CanEverFly is false)
                {
                    return false;
                }
                __instance.StartFlyingInternal();
                job.flying = true;
                return false;
            }
            return true;
        }
    }
}
