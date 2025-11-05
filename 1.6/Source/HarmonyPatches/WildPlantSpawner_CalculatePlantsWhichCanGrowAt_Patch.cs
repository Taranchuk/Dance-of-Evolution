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
                       if (def.CanEverPlantAt(c, __instance.map, checkMapTemperature: false))
                       {
                           float outdoorTemp = __instance.map.mapTemperature.OutdoorTemp;
                           if (outdoorTemp > def.plant.minGrowthTemperature && outdoorTemp < def.plant.maxGrowthTemperature)
                           {
                               outPlants.Add(def);
                           }
                        }
                    }
                }
            }
        }
    }
}
