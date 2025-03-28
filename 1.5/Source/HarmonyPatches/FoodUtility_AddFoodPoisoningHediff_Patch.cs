using DanceOfEvolution;
using HarmonyLib;
using RimWorld;
using Verse;

[HarmonyPatch(typeof(FoodUtility), "AddFoodPoisoningHediff")]
public static class FoodUtility_AddFoodPoisoningHediff_Patch
{
    public static bool Prefix(Pawn pawn, Thing ingestible)
    {
        if (ingestible?.def == DefsOf.DE_FungalSlurry)
        {
            if (pawn.IsServant() || pawn.IsFungalNexus())
            {
                return false;
            }
        }
        return true;
    }
}
