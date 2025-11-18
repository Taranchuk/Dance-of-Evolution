using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(MapGenerator), "GenerateMap")]
    public static class MapGenerator_GenerateMap_Patch
    {
        public static void Postfix(Map __result)
        {
            if (__result != null)
            {
                GameComponent_CurseManager.Instance.ApplyCurse(__result);
            }
        }
    }
}
