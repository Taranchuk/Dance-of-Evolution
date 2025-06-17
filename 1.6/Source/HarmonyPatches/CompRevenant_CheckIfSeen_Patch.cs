using HarmonyLib;
using Verse;
using RimWorld;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(CompRevenant), "CheckIfSeen")]
    public static class CompRevenant_CheckIfSeen_Patch
    {
        public static bool Prefix(CompRevenant __instance)
        {
            if (__instance.parent is Pawn pawn && pawn.IsServant())
            {
                return false;
            }
            return true;
        }
    }
}