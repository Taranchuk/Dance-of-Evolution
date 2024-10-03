using RimWorld;
using Verse;
using HarmonyLib;
using System;
namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(PlantUtility), "CanEverPlantAt",
		new Type[] { typeof(ThingDef), typeof(IntVec3), typeof(Map), typeof(Thing), typeof(bool), typeof(bool) },
		new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal }
		)]
	public static class PlantUtility_CanEverPlantAt_Patch
	{
		public static void Postfix(ThingDef plantDef, IntVec3 c, Map map, ref AcceptanceReport __result)
		{
			TerrainDef terrain = map.terrainGrid.TerrainAt(c);
			if (terrain == DefsOf.DE_RottenSoil && !plantDef.plant.cavePlant)
			{
				__result = new AcceptanceReport("DE_CanOnlyPlantCavePlantsOnRottenSoil".Translate());
			}
		}
	}
}