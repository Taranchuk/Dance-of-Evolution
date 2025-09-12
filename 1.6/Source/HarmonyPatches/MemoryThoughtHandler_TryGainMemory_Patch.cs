using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(MemoryThoughtHandler), nameof(MemoryThoughtHandler.TryGainMemory), new[] { typeof(ThoughtDef), typeof(Pawn), typeof(Precept) })]
    public static class MemoryThoughtHandler_TryGainMemory_Patch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(MemoryThoughtHandler __instance, ThoughtDef def)
        {
            if (ModsConfig.IdeologyActive)
            {
                if ((def == DefsOf.AteFungus_Despised || def == DefsOf.AteFungusAsIngredient_Despised)
                    && __instance.pawn.IsFungalNexus())
                {
                    return false;
                }
            }
            return true;
        }
    }
}
