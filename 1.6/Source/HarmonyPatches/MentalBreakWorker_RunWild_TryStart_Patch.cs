using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(MentalBreakWorker_RunWild), "TryStart")]
    public static class MentalBreakWorker_RunWild_TryStart_Patch
    {
        public static void Postfix(Pawn pawn, string reason, bool causedByMood)
        {
            if (pawn.IsFungalNexus(out var hediff))
            {
                foreach (var servant in hediff.servants)
                {
                    servant.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter);
                }
            }
        }
    }
}
