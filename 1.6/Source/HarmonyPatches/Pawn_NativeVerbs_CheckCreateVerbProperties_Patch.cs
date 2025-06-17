using RimWorld;
using HarmonyLib;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Pawn_NativeVerbs), "CheckCreateVerbProperties")]
    public static class Pawn_NativeVerbs_CheckCreateVerbProperties_Patch
    {
        public static void Prefix(Pawn_NativeVerbs __instance, out bool __state)
        {
            __state = __instance.pawn.RaceProps.giveNonToolUserBeatFireVerb;
            if (__instance.pawn.IsServant() && __state is false)
            {
                __instance.pawn.RaceProps.giveNonToolUserBeatFireVerb = true;
            }
        }
        public static void Postfix(Pawn_NativeVerbs __instance, bool __state)
        {
            __instance.pawn.RaceProps.giveNonToolUserBeatFireVerb = __state;
        }
    }
}