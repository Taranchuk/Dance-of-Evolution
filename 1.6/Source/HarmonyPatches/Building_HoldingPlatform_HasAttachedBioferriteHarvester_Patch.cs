using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Building_HoldingPlatform), nameof(Building_HoldingPlatform.HasAttachedBioferriteHarvester), MethodType.Getter)]
    public class Building_HoldingPlatform_HasAttachedBioferriteHarvester_Patch
    {
        public static void Postfix(Building_HoldingPlatform __instance, ref bool __result)
        {
            if (__result)
            {
                return;
            }
            foreach (Thing item in __instance.FacilitiesComp.LinkedFacilitiesListForReading)
            {
                if (item is Building_CubeFungi cubeFungi && cubeFungi.CurrentNutrition() > 0)
                {
                    __result = true;
                    return;
                }
            }
        }
    }
}