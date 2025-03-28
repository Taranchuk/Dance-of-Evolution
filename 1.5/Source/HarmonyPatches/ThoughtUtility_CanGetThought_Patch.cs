using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(ThoughtUtility), "CanGetThought")]
    public static class ThoughtUtility_CanGetThought_Patch
    {
        public static void Postfix(Pawn pawn, ThoughtDef def, ref bool __result)
        {
            if (__result is false && def == DefsOf.SunlightSensitivity_Mild && pawn.IsFungalNexus())
            {
                __result = true;
            }
        }
    }
}
