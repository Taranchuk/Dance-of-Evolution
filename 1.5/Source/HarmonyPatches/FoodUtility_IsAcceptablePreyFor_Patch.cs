using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(FoodUtility), "IsAcceptablePreyFor")]
    public static class FoodUtility_IsAcceptablePreyFor_Patch
    {
        public static void Postfix(Pawn prey, ref bool __result)
        {
            if (prey.IsEntity && prey.IsServant())
            {
                __result = false;
            }
        }
    }
}