using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    public abstract class IncidentWorker_MycelyssBase : IncidentWorker
    {
        protected List<Pawn> GenerateAndSpawnPawns(IncidentParms parms, Map map, IntVec3 spawnCell, Faction faction)
        {
            var pawns = new List<Pawn>();
            var raidParms = StorytellerUtility.DefaultParmsNow(IncidentDefOf.RaidEnemy.category, map);
            raidParms.faction = faction;
            raidParms.points = StorytellerUtility.DefaultThreatPointsNow(parms.target);
            var pawnParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, parms);

            var minPoints = faction.def.MinPointsToGeneratePawnGroup(PawnGroupKindDefOf.Combat, pawnParms);
            if (pawnParms.points < minPoints)
            {
                pawnParms.points = minPoints;
            }
            var convoy = PawnGroupMakerUtility.GeneratePawns(pawnParms).ToList();
            pawns.AddRange(convoy);

            foreach (Pawn pawn in pawns)
            {
                IntVec3 loc = CellFinder.RandomSpawnCellForPawnNear(spawnCell, map);
                GenSpawn.Spawn(pawn, loc, map, Rot4.Random);
            }

            return pawns;
        }

        protected IntVec3 FindDestinationCell(Map map, Pawn leader, IntVec3 spawnCell)
        {
            if (RCellFinder.TryFindRandomSpotJustOutsideColony(leader.Position, map, leader, out var destinationCell, (IntVec3 c) => !c.Roofed(map)))
            {
                return destinationCell;
            }
            return spawnCell;
        }

        protected Faction GetMycelyssFaction()
        {
            return Find.FactionManager.FirstFactionOfDef(DefsOf.DE_Mycelyss);
        }
    }
}
