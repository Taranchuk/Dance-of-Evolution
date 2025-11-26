using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Pawn), "SetFaction")]
    public static class Pawn_SetFaction_Patch
    {
        private static void Postfix(Pawn __instance, Faction newFaction, Pawn recruiter = null)
        {
            if (__instance.IsFungalNexus(out var hediff))
            {
                foreach (var servant in hediff.servants)
                {
                    servant.SetFaction(newFaction, recruiter);
                }
            }
            else if (__instance.IsServant(out var servantHediff) && newFaction != servantHediff.masterHediff?.pawn.Faction)
            {
                servantHediff.masterHediff?.servants.Remove(__instance);
            }
        }
    }
}
