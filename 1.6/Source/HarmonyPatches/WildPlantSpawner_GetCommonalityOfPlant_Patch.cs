using RimWorld;
using Verse;
using HarmonyLib;
using System.Collections.Generic;
namespace DanceOfEvolution
{

    [HotSwappable]
    [HarmonyPatch(typeof(WildPlantSpawner), "CalculatePlantsWhichCanGrowAt")]
    public static class WildPlantSpawner_CalculatePlantsWhichCanGrowAt_Patch
    {
        public static void Postfix(WildPlantSpawner __instance, IntVec3 c, ref List<ThingDef> outPlants)
        {
            if (__instance.map.gameConditionManager.ConditionIsActive(DefsOf.DE_CloudmakerCondition))
            {
                foreach (var plant in WildPlantSpawner_GetCommonalityOfPlant_Patch.commonalities)
                {
                    var def = DefDatabase<ThingDef>.GetNamedSilentFail(plant.Key);
                    if (def != null && !outPlants.Contains(def))
                    {
                        outPlants.Add(def);
                    }
                }
            }
        }
    }

    [HotSwappable]
    [HarmonyPatch(typeof(WildPlantSpawner), "GetCommonalityOfPlant")]
    public static class WildPlantSpawner_GetCommonalityOfPlant_Patch
    {
        public static Dictionary<string, float> commonalities = new Dictionary<string, float>
        {
            {"Plant_Timbershroom", 0.1f},
            {"Plant_Nightgrass", 12f},
            {"Boomshroom", 0.01f},
            {"Plant_Willowgill", 0.2f},
            {"Glowstool", 0.01f},
            {"Agarilux", 0.01f},
            {"Bryolux", 0.05f},
            {"Plant_Psilocap", 0.01f},
            {"Plant_NightRafflesia", 0.01f},
        };
        public static void Postfix(WildPlantSpawner __instance, ref float __result, ThingDef plant)
        {
            if (__instance.map.gameConditionManager.ConditionIsActive(DefsOf.DE_CloudmakerCondition))
            {
                if (commonalities.TryGetValue(plant.defName, out var value))
                {
                    __result = value;
                }
                Log.Message(plant + " " + __result);
            }
        }
    }
}
