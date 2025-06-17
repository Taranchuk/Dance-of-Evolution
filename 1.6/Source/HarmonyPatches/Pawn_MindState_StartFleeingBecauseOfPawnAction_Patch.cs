using RimWorld;
using HarmonyLib;
using Verse.AI;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Pawn_MindState), "StartFleeingBecauseOfPawnAction")]
    public static class Pawn_MindState_StartFleeingBecauseOfPawnAction_Patch
    {
        public static bool Prefix(Pawn_MindState __instance)
        {
            if (__instance.pawn.IsServant())
            {
                return false;
            }
            return true;
        }
    }
}