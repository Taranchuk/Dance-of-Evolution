using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(FleeUtility), "FleeJob")]
    public static class FleeUtility_FleeJob_Patch
    {
        public static bool Prefix(ref Job __result, Pawn pawn)
        {
            if (pawn.IsServant())
            {
                __result = null;
                return false;
            }
            return true;
        }
    }
}