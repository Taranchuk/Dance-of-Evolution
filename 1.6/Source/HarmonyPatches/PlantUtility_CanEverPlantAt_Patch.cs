using RimWorld;
using Verse;
using HarmonyLib;
using System;
namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(PlantUtility), "CanEverPlantAt",
		new Type[] { typeof(ThingDef), typeof(IntVec3), typeof(Map), typeof(Thing), typeof(bool), typeof(bool), typeof(bool) },
		new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal }
		)]
	public static class CanEverPlantAt_Patch
	{
		public static void Postfix(ref AcceptanceReport __result, ThingDef plantDef, IntVec3 c, Map map)
		{
			if (c.InBounds(map))
			{
				TerrainDef terrain = map.terrainGrid.TerrainAt(c);
				if (terrain.IsFungalTerrain() && plantDef.CanSpawnOnFungalTerrain() is false)
				{
					__result = new AcceptanceReport("DE_CanOnlyPlantCavePlantsOnRottenSoil".Translate());
				}
			}
		}
	}
}
