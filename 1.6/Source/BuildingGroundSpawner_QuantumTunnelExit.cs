using RimWorld;
using Verse;
using System.Collections.Generic;

namespace DanceOfEvolution
{
    public class BuildingGroundSpawner_QuantumTunnelExit : BuildingGroundSpawner
    {
        private Building_QuantumTunnelGate originalEntranceGate;

        public void SetOriginalEntranceGate(Building_QuantumTunnelGate gate)
        {
            originalEntranceGate = gate;
        }

        public override void Spawn(Map map, IntVec3 pos)
        {
            PocketMapUtility.currentlyGeneratingPortal = originalEntranceGate;
            base.Spawn(map, pos);
            PocketMapUtility.currentlyGeneratingPortal = null;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref originalEntranceGate, "originalEntranceGate");
        }
    }
}