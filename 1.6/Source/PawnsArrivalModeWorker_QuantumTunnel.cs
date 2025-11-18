using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DanceOfEvolution
{
    public class PawnsArrivalModeWorker_QuantumTunnel : PawnsArrivalModeWorker
    {
        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            Map map = (Map)parms.target;
            var spawner = (BuildingGroundSpawner_QuantumTunnelAid)ThingMaker.MakeThing(DefsOf.DE_QuantumTunnelGateAid_Spawner);
            spawner.pawnsToSpawn = pawns;
            GenSpawn.Spawn(spawner, parms.spawnCenter, map);
        }

        public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (parms.spawnCenter.IsValid)
            {
                return true;
            }

            var spawnParms = new LargeBuildingSpawnParms
            {
                thingDef = DefsOf.DE_QuantumTunnelGateExit,
                canSpawnOnImpassable = false,
                attemptSpawnLocationType = SpawnLocationType.Outdoors,
                maxDistanceToColonyBuilding = -1,
                minDistanceToColonyBuilding = 0,
                minDistToEdge = 5
            };

            var hostiles = map.attackTargetsCache.TargetsHostileToColony.Where(t => t.Thing is Pawn).Select(t => t.Thing.Position).ToList();
            if (hostiles.Any())
            {
                foreach (var hostile in hostiles.InRandomOrder())
                {
                    for (int radius = 10; radius <= 40; radius += 10)
                    {
                        if (LargeBuildingCellFinder.TryFindCellNear(hostile, map, radius, spawnParms, out parms.spawnCenter, true))
                        {
                            return true;
                        }
                    }
                }
            }

            if (LargeBuildingCellFinder.TryFindCell(out parms.spawnCenter, map, spawnParms, null, null, true))
            {
                return true;
            }

            return RCellFinder.TryFindRandomPawnEntryCell(out parms.spawnCenter, map, CellFinder.EdgeRoadChance_Hostile);
        }
    }
}
