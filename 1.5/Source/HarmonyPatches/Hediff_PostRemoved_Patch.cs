using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Hediff), "PostRemoved")]
    public static class Hediff_PostRemoved_Patch
    {
        public static void Postfix(Hediff __instance)
        {
            if (__instance.def.organicAddedBodypart && __instance.pawn.IsFungalNexus(out var nexus))
            {
                nexus.cachedStage = null;
            }
        }
    }
}
