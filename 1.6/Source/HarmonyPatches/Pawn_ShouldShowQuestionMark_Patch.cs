using HarmonyLib;
using Verse;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Pawn), "ShouldShowQuestionMark")]
    public static class Pawn_ShouldShowQuestionMark_Patch
    {
        public static void Postfix(ref bool __result, Pawn __instance)
        {
            if (__result is false)
            {
                var lord = __instance.GetLord();
                if (lord != null && lord.CurLordToil is LordToil_MycelyssEnvoy waitForPlayer && waitForPlayer.envoy == __instance)
                {
                    __result = true;
                }
            }
        }
    }
}
