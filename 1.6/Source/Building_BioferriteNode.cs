using System.Linq;
using UnityEngine;
using Verse.Sound;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
namespace DanceOfEvolution
{
	public class Building_BioferriteNode : Building_FungalNode
	{
		protected override void ConvertTerrain(IntVec3 cell, Map map)
		{
			map.terrainGrid.SetTerrain(cell, DefsOf.DE_MyceliumFerrite);
		}

		protected override bool TerrainValidator(TerrainDef terrain)
		{
			return terrain == DefsOf.DE_RottenSoil;
		}
	}
}