using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    public class IncidentWorker_MycelyssEnvoy : IncidentWorker_MycelyssBase
    {
        public override bool CanFireNowSub(IncidentParms parms)
        {
            return !GameComponent_CurseManager.Instance.mycelyssEnvoyEventTriggered && PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction.Any(p => p.IsFungalNexus());
        }

        public override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (!RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 spawnCell, map, CellFinder.EdgeRoadChance_Hostile))
            {
                return false;
            }

            Faction mycelyssFaction = GetMycelyssFaction();
            if (mycelyssFaction == null)
            {
                return false;
            }

            if (mycelyssFaction.HostileTo(Faction.OfPlayer))
            {
                var factionRelation = new FactionRelation();
                factionRelation.other = Faction.OfPlayer;
                factionRelation.kind = FactionRelationKind.Neutral;
                factionRelation.baseGoodwill = 0;
                
                mycelyssFaction.SetRelation(factionRelation);
                
                Find.LetterStack.ReceiveLetter("DE_MycelyssNeutral".Translate(), "DE_MycelyssNeutralDesc".Translate(), LetterDefOf.NeutralEvent, mycelyssFaction?.leader);
            }
            Pawn leader = PawnGenerator.GeneratePawn(DefsOf.DE_Mycelyss_Envoy, mycelyssFaction);
            GenSpawn.Spawn(leader, spawnCell, map);
            List<Pawn> pawns = new List<Pawn> { leader };
            List<Pawn> convoy = GenerateAndSpawnPawns(parms, map, spawnCell, mycelyssFaction);
            pawns.AddRange(convoy);

            IntVec3 destinationCell = FindDestinationCell(map, leader, spawnCell);

            var lordJob = new LordJob_MycelyssEnvoy(leader, destinationCell);
            LordMaker.MakeNewLord(mycelyssFaction, lordJob, map, pawns);

            Find.LetterStack.ReceiveLetter("DE_MycelyssEnvoyTitle".Translate(), "DE_MycelyssEnvoyDesc".Translate(), LetterDefOf.NeutralEvent, leader);
            GameComponent_CurseManager.Instance.mycelyssEnvoyEventTriggered = true;

            return true;
        }
    }
}
