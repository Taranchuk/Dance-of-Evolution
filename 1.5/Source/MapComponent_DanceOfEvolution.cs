using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HotSwappable]
    public class MapComponent_DanceOfEvolution : MapComponent
    {
        public MapComponent_DanceOfEvolution(Map map) : base(map)
        {

        }

        public int nextTreeSpawn;
        public Dictionary<IntVec3, float> cellsWithDarknessTime = new Dictionary<IntVec3, float>();
        public const int DarknessCheckInterval = 120;
        public const int MinDarknessTime = GenDate.TicksPerDay * 2;
        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (nextTreeSpawn == 0)
            {
                SetNextTreeSpawnTick();
            }
            else if (Find.TickManager.TicksGame % DarknessCheckInterval == 0)
            {
                var rottenSoil = map.AllCells.Where(x => x.GetTerrain(map) == DefsOf.DE_RottenSoil).ToList();
                foreach (var terrain in rottenSoil)
                {
                    if (cellsWithDarknessTime.ContainsKey(terrain) is false)
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
            else if (Find.TickManager.TicksGame >= nextTreeSpawn)
            {
                var rottenSoil = cellsWithDarknessTime.Where(x => x.Value >= MinDarknessTime
                && map.glowGrid.GroundGlowAt(x.Key) <= 0 && x.Key.GetFirstBuilding(map) is null
                && x.Key.GetFirstThing<MycelialTree>(map) is null).ToList();
                if (rottenSoil.Any())
                {
                    var terrain = rottenSoil.RandomElement();
                    var plant = GenSpawn.Spawn(DefsOf.DE_Plant_TreeMycelial, terrain.Key, map) as Plant;
                    plant.Growth = 0.05f;
                    SetNextTreeSpawnTick();
                }
                else
                {
                    nextTreeSpawn += GenDate.TicksPerHour;
                }
            }
        }

        public void SetNextTreeSpawnTick()
        {
            nextTreeSpawn = Find.TickManager.TicksGame + (int)(new FloatRange(3f, 20f).RandomInRange * GenDate.TicksPerDay);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref nextTreeSpawn, "nextTreeSpawn");
            Scribe_Collections.Look(ref cellsWithDarknessTime, "cellsWithDarknessTime", LookMode.Value, LookMode.Value);
        }
    }
}