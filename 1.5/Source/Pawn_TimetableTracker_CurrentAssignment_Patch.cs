using HarmonyLib;
using RimWorld;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Pawn_TimetableTracker), "CurrentAssignment", MethodType.Getter)]
    public static class Pawn_TimetableTracker_CurrentAssignment_Patch
    {
        public static void Postfix(ref TimeAssignmentDef __result, Pawn_TimetableTracker __instance)
        {
            if (__instance.pawn.IsWorkMutant())
            {
                __result = __instance.times[GenLocalDate.HourOfDay(__instance.pawn)];
            }
        }
    }
}