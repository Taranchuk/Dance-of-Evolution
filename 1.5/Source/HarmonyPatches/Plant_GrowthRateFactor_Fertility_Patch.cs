using RimWorld;
using Verse;
using HarmonyLib;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Plant), "GrowthRateFactor_Fertility", MethodType.Getter)]
	public static class Plant_GrowthRateFactor_Fertility_Patch
	{
		public static void Postfix(Plant __instance, ref float __result)
		{
			TerrainDef terrain = __instance.Map.terrainGrid.TerrainAt(__instance.Position);
			if (terrain == DefsOf.DE_RottenSoil && !__instance.def.plant.cavePlant)
			{
				__result = 0f;
			}
		}
	}


}