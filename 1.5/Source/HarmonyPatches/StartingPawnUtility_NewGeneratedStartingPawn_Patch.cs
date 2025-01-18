using HarmonyLib;
using Verse;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(StartingPawnUtility), "NewGeneratedStartingPawn")]
    public static class StartingPawnUtility_NewGeneratedStartingPawn_Patch
    {
        public static void Postfix(ref Pawn __result)
        {
            if (Find.Scenario.name == DefsOf.DE_FungalAwakening.label)
            {
                __result.health.AddHediff(DefsOf.DE_FungalNexus);
            }
        }
    }
}
