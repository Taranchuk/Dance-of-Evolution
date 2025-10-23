using System.Linq;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    public class CurseWorker_Nociosphere : CurseWorker
    {
        public override void Apply(Map map)
        {
            if (TryFindRandomSpawnCell(map, out var loc))
            {
                Pawn nociosphere = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Nociosphere, Faction.OfEntities, PawnGenerationContext.NonPlayer, map.Tile));
                GenSpawn.Spawn(nociosphere, loc, map);
                NociosphereUtility.SkipTo(nociosphere, loc);
            }
        }
    }

    public class CurseWorker_RevenantHypnosis : CurseWorker
    {
        public override void Apply(Map map)
        {
            if (TryFindRandomSpawnCell(map, out var loc))
            {
                var revenant = GenSpawn.Spawn(PawnGenerator.GeneratePawn(PawnKindDefOf.Revenant, Faction.OfEntities), loc, map) as Pawn;
                foreach (var enemy in GetAllHostilePawns(map))
                {
                    enemy.health.AddHediff(HediffDefOf.RevenantHypnosis);
                }
            }
        }
    }

    public class CurseWorker_Fleshbeasts : CurseWorker
    {
        public override void Apply(Map map)
        {
            for (int i = 0; i < 3; i++)
            {
                if (TryFindRandomSpawnCell(map, out var loc))
                {
                    FleshbeastUtility.SpawnFleshbeastsFromPitBurrowEmergence(loc, map, StorytellerUtility.DefaultThreatPointsNow(map), new IntRange(600, 600), new IntRange(60, 180));
                }
            }
        }
    }

    public class CurseWorker_GoldCubeWithdrawal : CurseWorker
    {
        public override void Apply(Map map)
        {
            foreach (var enemy in GetAllHostilePawns(map))
            {
                var hediff = HediffMaker.MakeHediff(HediffDefOf.CubeWithdrawal, enemy);
                hediff.Severity = Rand.Range(0.1f, 1.0f);
                enemy.health.AddHediff(hediff);
            }
        }
    }

    public class CurseWorker_SunBlock : CurseWorker
    {
        public override void Apply(Map map)
        {
            map.gameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(GameConditionDefOf.UnnaturalDarkness));
            var pawnGroupParms = new PawnGroupMakerParms
            {
                faction = Faction.OfEntities,
                groupKind = PawnGroupKindDefOf.Noctols,
                tile = map.Tile,
                points = StorytellerUtility.DefaultThreatPointsNow(map)
            };
            var noctolGroup = PawnGroupMakerUtility.GeneratePawns(pawnGroupParms).ToList();
            foreach (var noctol in noctolGroup)
            {
                IntVec3 spawnLoc;
                if (CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => c.Standable(map) && !c.Fogged(map), map, CellFinder.EdgeRoadChance_Animal, out spawnLoc))
                {
                    GenSpawn.Spawn(noctol, spawnLoc, map, Rot4.Random);
                }
            }
        }
    }

    public class CurseWorker_MetalHorror : CurseWorker
    {
        public override void Apply(Map map)
        {
            foreach (var enemy in GetAllHostilePawns(map))
            {
                var hediff = HediffMaker.MakeHediff(HediffDefOf.MetalhorrorImplant, enemy);
                hediff.Severity = Rand.Range(0.1f, 1.0f);
                enemy.health.AddHediff(hediff);
            }
        }
    }
}
