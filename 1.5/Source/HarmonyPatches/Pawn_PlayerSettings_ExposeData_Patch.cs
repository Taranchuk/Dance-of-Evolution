using RimWorld;
using HarmonyLib;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Pawn_PlayerSettings), "ExposeData")]
    public static class Pawn_PlayerSettings_ExposeData_Patch
    {
        public static void Prefix(Pawn_PlayerSettings __instance, out float? __state)
        {
            __state = __instance.pawn.def.race.roamMtbDays;
            if (__instance.pawn.IsServant())
            {
                __instance.pawn.def.race.roamMtbDays = null;
            }
        }

        public static void Postfix(Pawn_PlayerSettings __instance, float? __state)
        {
            __instance.pawn.def.race.roamMtbDays = __state;
        }
    }
}