using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(ThoughtWorker_Dark), nameof(ThoughtWorker_Dark.CurrentStateInternal))]
    public static class ThoughtWorker_Dark_CurrentStateInternal_Patch
    {
        public static void Postfix(ref ThoughtState __result, Pawn p)
        {
            if (p.HasFungalNexus())
            {
                __result = ThoughtState.Inactive;
            }
        }
    }
}
