using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using System;

namespace DanceOfEvolution
{
	[HotSwappable]
	public class MapComponent_DanceOfEvolution : MapComponent
	{
		public MapComponent_DanceOfEvolution(Map map) : base(map)
		{
		}

		public Dictionary<ThingDef, int> nextPlantSpawn = new Dictionary<ThingDef, int>();
		public Dictionary<IntVec3, float> cellsWithDarknessTime = new Dictionary<IntVec3, float>();
		public const int DarknessCheckInterval = 120;
		public static readonly Dictionary<(ThingDef plantDef, Func<TerrainDef, bool> terrainPredicate), FloatRange> PlantSpawnTimes = new Dictionary<(ThingDef plantDef, Func<TerrainDef, bool> terrainPredicate), FloatRange>
		{
			{ (plantDef: DefsOf.DE_Plant_TreeMycelial, terrainPredicate: (TerrainDef terrainDef) => true), new FloatRange(3f, 20f) },
			{ (plantDef: DefsOf.DE_FalseParasol, terrainPredicate: (TerrainDef terrainDef) => terrainDef != DefsOf.DE_RottenSoil), new FloatRange(1.5f, 10f) },
			{ (plantDef: DefsOf.DE_FalseParasol, terrainPredicate: (TerrainDef terrainDef) => terrainDef == DefsOf.DE_RottenSoil), new FloatRange(0.75f, 5f) }
		};

		public override void MapComponentTick()
		{
			base.MapComponentTick();

			if (Find.TickManager.TicksGame % DarknessCheckInterval == 0)
			{
				var rottenSoil = map.AllCells.Where(x => x.GetTerrain(map) == DefsOf.DE_RottenSoil).ToList();
				foreach (var terrain in rottenSoil)
				{
					if (!cellsWithDarknessTime.ContainsKey(terrain))
					{
						cellsWithDarknessTime[terrain] = 0;
					}
					if (map.glowGrid.GroundGlowAt(terrain) <= 0)
					{
						cellsWithDarknessTime[terrain] += DarknessCheckInterval;
					}
					else
					{
						cellsWithDarknessTime[terrain] = 0;
					}
				}
			}
			foreach (var kvp in PlantSpawnTimes)
			{
				var plantTerrainPredicate = kvp.Key;
				var plantDef = plantTerrainPredicate.plantDef;
				var terrainPredicate = plantTerrainPredicate.terrainPredicate;
				var spawnInterval = kvp.Value;

				if (!nextPlantSpawn.ContainsKey(plantDef))
				{
					SetNextPlantSpawnTick(plantDef, spawnInterval);
				}
				else if (Find.TickManager.TicksGame >= nextPlantSpawn[plantDef])
				{
					var validCells = cellsWithDarknessTime.Where(x => x.Value >= GenDate.TicksPerDay * 2
					&& map.glowGrid.GroundGlowAt(x.Key) <= 0
					&& x.Key.GetFirstBuilding(map) is null
					&& x.Key.GetPlant(map) is null && terrainPredicate(x.Key.GetTerrain(map))).ToList();

					if (validCells.Any())
					{
						var terrain = validCells.RandomElement();
						var plant = GenSpawn.Spawn(plantDef, terrain.Key, map) as Plant;
						plant.Growth = 0.05f;
						SetNextPlantSpawnTick(plantDef, spawnInterval);
					}
					else
					{
						nextPlantSpawn[plantDef] += GenDate.TicksPerHour;
					}
				}
			}
		}

		public void SetNextPlantSpawnTick(ThingDef plantDef, FloatRange spawnInterval)
		{
			nextPlantSpawn[plantDef] = Find.TickManager.TicksGame + (int)(GenDate.TicksPerDay * spawnInterval.RandomInRange);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref nextPlantSpawn, "nextPlantSpawn", LookMode.Def, LookMode.Value);
			Scribe_Collections.Look(ref cellsWithDarknessTime, "cellsWithDarknessTime", LookMode.Value, LookMode.Value);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				nextPlantSpawn ??= new Dictionary<ThingDef, int>();
				cellsWithDarknessTime ??= new Dictionary<IntVec3, float>();
			}
		}
	}
}