using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(RevenantUtility), "GetClosestTargetInRadius")]
    public static class RevenantUtility_GetClosestTargetInRadius_Patch
    {
        public static void Prefix(Pawn pawn)
        {
            RevenantUtility_ValidTarget_Patch.curPawn = pawn;
        }

        public static void Postfix()
        {
            RevenantUtility_ValidTarget_Patch.curPawn = null;
        }
    }
}
