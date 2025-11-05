using RimWorld;
using Verse;
using HarmonyLib;
using System.Collections.Generic;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(WildPlantSpawner), "CurrentPlantDensityFactor", MethodType.Getter)]
    public static class WildPlantSpawner_CurrentPlantDensityFactor_Patch
    {
        public static void Postfix(WildPlantSpawner __instance, ref float __result)
        {
            Log.Message(__result);
            __result = 0.2f;
        }

    }
    [HotSwappable]
    [HarmonyPatch(typeof(WildPlantSpawner), "CalculatePlantsWhichCanGrowAt")]
    public static class WildPlantSpawner_CalculatePlantsWhichCanGrowAt_Patch
    {
        public static void Postfix(WildPlantSpawner __instance, IntVec3 c, ref List<ThingDef> outPlants)
        {
            Log.Message("map.wildPlantSpawner.CurrentPlantDensityFactor: " + __instance.map.wildPlantSpawner.CurrentPlantDensityFactor);
            if (__instance.map.gameConditionManager.ConditionIsActive(DefsOf.DE_CloudmakerCondition))
            {
                Log.Message("Checking plants");
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
                            else
                            {
                                Log.Message("2 Cannot spawn " + def);
                            }
                        }
                       else
                       {
                            Log.Message("Cannot spawn " + def);
                       }
                    }
                }
            }
            else
            {
                Log.Message("Not active");
            }
        }
    }
}
