using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    public class IncidentWorker_MycelyssDemand : IncidentWorker_MycelyssBase
    {
        public override bool CanFireNowSub(IncidentParms parms)
        {
            Faction mycelyssFaction = GetMycelyssFaction();
            if (mycelyssFaction == null || mycelyssFaction.HostileTo(Faction.OfPlayer))
            {
                GameComponent_CurseManager.Instance.mycelyssDemandActive = false;
                return false;
            }
            return GameComponent_CurseManager.Instance.mycelyssDemandActive
                   && GameComponent_CurseManager.Instance.mycelyssDemandTick > 0
                   && Find.TickManager.TicksGame >= GameComponent_CurseManager.Instance.mycelyssDemandTick;
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
            Pawn leader = PawnGenerator.GeneratePawn(DefsOf.DE_Mycelyss_Envoy, mycelyssFaction);
            GenSpawn.Spawn(leader, spawnCell, map);
            List<Pawn> pawns = new List<Pawn> { leader };

            List<Pawn> convoy = GenerateAndSpawnPawns(parms, map, spawnCell, mycelyssFaction);
            pawns.AddRange(convoy);

            IntVec3 destinationCell = FindDestinationCell(map, leader, spawnCell);

            var lordJob = new LordJob_DemandPawns(leader, destinationCell);
            LordMaker.MakeNewLord(mycelyssFaction, lordJob, map, pawns);

            string letterText = "DE_MycelyssDemandArrived".Translate(GameComponent_CurseManager.Instance.requiredPawnCount);
            Find.LetterStack.ReceiveLetter("DE_MycelyssDemandTitle".Translate(), letterText, LetterDefOf.NegativeEvent, leader);

            return true;
        }
    }
}
