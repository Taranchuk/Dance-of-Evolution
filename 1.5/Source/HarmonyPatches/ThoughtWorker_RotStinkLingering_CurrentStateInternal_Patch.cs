using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(ThoughtWorker_RotStinkLingering), nameof(ThoughtWorker_RotStinkLingering.CurrentStateInternal))]
    public static class ThoughtWorker_RotStinkLingering_CurrentStateInternal_Patch
    {
        public static void Postfix(ref ThoughtState __result, Pawn p)
        {
            if (p.IsServant() || p.HasFungalNexus())
            {
                __result = ThoughtState.Inactive;
            }
        }
    }
}
