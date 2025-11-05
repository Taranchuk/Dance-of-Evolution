using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(GenStep_Power), nameof(GenStep_Power.Generate))]
    public static class GenStep_Power_Generate_Patch
    {
        public static bool Prefix(Map map, GenStepParams parms)
        {
            if (map.ParentFaction != null && map.ParentFaction.def == DefsOf.DE_Mycelyss)
            {
                return false;
            }
            return true;
        }
    }
}
