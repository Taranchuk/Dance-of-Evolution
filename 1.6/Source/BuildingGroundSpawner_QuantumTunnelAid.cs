using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace DanceOfEvolution
{
    [HotSwappable]
    public class BuildingGroundSpawner_QuantumTunnelAid : BuildingGroundSpawner
    {
        public List<Pawn> pawnsToSpawn;
        public override void Spawn(Map map, IntVec3 pos)
        {
            base.Spawn(map, pos);
            var tunnelExit = (Building_QuantumTunnelGateExitAid)thingToSpawn;
            tunnelExit.pawnsToSpawn = pawnsToSpawn;
            pawnsToSpawn = null;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref pawnsToSpawn, "pawnsToSpawn", LookMode.Deep);
        }
    }
}
