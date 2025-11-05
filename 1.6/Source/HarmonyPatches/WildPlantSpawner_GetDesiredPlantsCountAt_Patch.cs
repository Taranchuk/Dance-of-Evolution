using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(WildPlantSpawner), nameof(WildPlantSpawner.GetDesiredPlantsCountAt))]
    public static class WildPlantSpawner_GetDesiredPlantsCountAt_Patch
    {
        public static void Postfix(WildPlantSpawner __instance, ref float __result, IntVec3 forCell, float plantDensityFactor)
        {
            if (__instance.map.gameConditionManager.ConditionIsActive(DefsOf.DE_CloudmakerCondition) && forCell.GetTerrain(__instance.map) == DefsOf.DE_RottenSoil)
            {
                float fertility = __instance.map.fertilityGrid.FertilityAt(forCell);
                if (fertility <= 0f && __instance.HaveAnyPlantsWhichIgnoreFertility)
                {
                    fertility = 1f;
                }
                float newResult = Mathf.Min(__instance.GetBaseDesiredPlantsCountAt(forCell) * 1f * plantDensityFactor * fertility, 1f);
                __result = newResult;
            }
        }
    }
}
