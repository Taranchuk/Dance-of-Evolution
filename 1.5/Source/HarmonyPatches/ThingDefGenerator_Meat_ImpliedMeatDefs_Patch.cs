using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(ThingDefGenerator_Meat), nameof(ThingDefGenerator_Meat.ImpliedMeatDefs))]
    public static class ThingDefGenerator_Meat_ImpliedMeatDefs_Patch
    {
        public static IEnumerable<ThingDef> Postfix(IEnumerable<ThingDef> __result)
        {
            foreach (var thingDef in __result)
            {
                if (ModsConfig.AnomalyActive)
                {
                    CompProperties compProperties = new CompProperties();
                    compProperties.compClass = typeof(CompMycelialTreeConsumable);
                    thingDef.comps.Add(compProperties);
                }
                yield return thingDef;
            }
        }
    }
}