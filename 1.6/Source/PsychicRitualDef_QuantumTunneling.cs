using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    [HotSwappable]
    public class PsychicRitualDef_QuantumTunneling : PsychicRitualDef_InvocationCircle
    {
        public override List<PsychicRitualToil> CreateToils(PsychicRitual psychicRitual, PsychicRitualGraph parent)
        {
            List<PsychicRitualToil> list = base.CreateToils(psychicRitual, parent);
            var invocation = list.OfType<PsychicRitualToil_InvokeHorax>().First();
            invocation.defenderPositions.Clear();
            list.Add(new PsychicRitualToil_CreateQuantumTunnel());
            
            return list;
        }
    }

    [HotSwappable]
    public class PsychicRitualToil_CreateQuantumTunnel : PsychicRitualToil
    {
        public override void End(PsychicRitual psychicRitual, PsychicRitualGraph parent, bool success)
        {
            base.End(psychicRitual, parent, success);
            
            var ritualTarget = psychicRitual.assignments.Target;
            var map = ritualTarget.Map;
            
            if (success)
            {
                var spawnParms = new LargeBuildingSpawnParms
                {
                    ignoreTerrainAffordance = true
                };
                if (!LargeBuildingCellFinder.TryFindCell(out var spawnCell, map, spawnParms.ForThing(DefsOf.DE_QuantumTunnelGate)))
                {
                    Log.Error("Could not find a valid location to spawn the quantum tunnel gate. Spawning at ritual spot as fallback.");
                    spawnCell = ritualTarget.Cell;
                }
                var tunnelGateSpawner = ThingMaker.MakeThing(DefsOf.DE_QuantumTunnelGate_Spawner);
                GenSpawn.Spawn(tunnelGateSpawner, spawnCell, map);
                Messages.Message("DE_QuantumTunnelingSuccess".Translate(), tunnelGateSpawner, MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                Messages.Message("DE_QuantumTunnelingFailure".Translate(), MessageTypeDefOf.NegativeEvent);
            }
            
            psychicRitual.ReleaseAllPawnsAndBuildings();
        }
    }
}
