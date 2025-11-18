using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    [HotSwappable]
    public class Building_QuantumTunnelGateExitAid : Building_QuantumTunnelGateExit
    {
        public override bool IsMilitaryAid => true;

        public override string EnterString => "";
        
        public override bool AutoDraftOnEnter => false;

        public override string CancelEnterString => "";

        public override Texture2D EnterTex => null;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            PocketMapUtility.currentlyGeneratingPortal = this;
            base.SpawnSetup(map, respawningAfterLoad);
            PocketMapUtility.currentlyGeneratingPortal = null;
        }
        public override string GetInspectString()
        {
            return "";
        }

        public override bool IsEnterable(out string reason)
        {
            reason = "DE_YouCannotUseIt".Translate();
            return false;
        }

        public override Map GetOtherMap()
        {
            return null;
        }

        public override IntVec3 GetDestinationLocation()
        {
            return IntVec3.Invalid;
        }

        public override void OnEntered(Pawn pawn)
        {
        }

        public override void Tick()
        {
            base.Tick();
            if (pawnsToSpawn != null && pawnsToSpawn.Any() && Find.TickManager.TicksGame > spawnedTick + 60)
            {
                if (ticksUntilNextSpawn <= 0)
                {
                    Pawn pawn = pawnsToSpawn.First();
                    IntVec3 spawnPos = CellFinder.StandableCellNear(Position, Map, 7f, (IntVec3 c) => c.GetFirstBuilding(Map) == null);
                    if (spawnPos.Walkable(Map) == false || spawnPos.GetFirstBuilding(Map) != null)
                    {
                        foreach (var radius in new float[] { 3f, 5f, 7f, 9f, 11f })
                        {
                            spawnPos = GenRadial.RadialCellsAround(Position, radius, true).FirstOrDefault(c => c.Walkable(Map) && c.GetFirstBuilding(Map) == null);
                            if (spawnPos.Walkable(Map) && spawnPos.GetFirstBuilding(Map) == null)
                            {
                                break;
                            }
                        }
                    }
                    GenSpawn.Spawn(pawn, spawnPos, Map);
                    Lord existingLord = null;
                    foreach (var lord in Map.lordManager.lords)
                    {
                        if (lord.LordJob is LordJob_AssistColony assistColonyJob && lord.faction == pawn.Faction)
                        {
                            existingLord = lord;
                            break;
                        }
                    }
                    var lord2 = pawn.GetLord();
                    if (lord2 != null)
                    {
                        lord2.RemovePawn(pawn);
                    }
                    if (existingLord != null)
                    {
                        existingLord.AddPawn(pawn);
                    }
                    else
                    {
                        var lordJob = new LordJob_AssistColony(pawn.Faction, spawnPos);
                        var newLord = LordMaker.MakeNewLord(pawn.Faction, lordJob, Map, new List<Pawn> { pawn });
                    }
                    
                    pawnsToSpawn.Remove(pawn);
                    ticksUntilNextSpawn = 20;
                }
                else
                {
                    ticksUntilNextSpawn--;
                }
            }
        
            if (pawnsToSpawn == null || !pawnsToSpawn.Any())
            {
                Collapse();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref pawnsToSpawn, "pawnsToSpawn", LookMode.Reference);
        }
    }
}
