using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
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
                var compActivity = nociosphere.GetComp<CompActivity>();
                compActivity.EnterActiveState();
                var compNociosphere = nociosphere.GetComp<CompNociosphere>();
                compNociosphere.sentOnslaught = true;
                compNociosphere.sentFromLocation = loc;
            }
        }
    }

    public class CurseWorker_RevenantHypnosis : CurseWorker
    {
        public override void Apply(Map map)
        {
            if (TryFindRandomSpawnCell(map, out var loc))
            {
                var revenant = GenSpawn.Spawn(PawnGenerator.GeneratePawn(PawnKindDefOf.Revenant, Faction.OfPlayer), loc, map) as Pawn;
                var comp = revenant.GetComp<CompRevenant>();
                var affectedEnemies = new List<Pawn>();
                foreach (var enemy in GetAllHostilePawns(map).Where(x => x != revenant && RevenantUtility.ValidTarget(x)).ToList())
                {
                    if (Rand.Chance(0.15f))
                    {
                        comp.Hypnotize(enemy);
                        if (enemy.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RevenantHypnosis) != null)
                        {
                            affectedEnemies.Add(enemy);
                        }
                    }
                }
                if (affectedEnemies.TryRandomElement(out var result))
                {
                    revenant.DeSpawn();
                    GenSpawn.Spawn(revenant, result.Position, map);
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
                    FleshbeastUtility.SpawnFleshbeastsFromPitBurrowEmergence(loc, map, StorytellerUtility.DefaultThreatPointsNow(Find.World), new IntRange(600, 600), new IntRange(60, 180));
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
                var hediff = HediffMaker.MakeHediff(HediffDefOf.CubeInterest, enemy) as Hediff_CubeInterest;
                hediff.Severity = 1.0f;
                enemy.health.AddHediff(hediff);
                hediff.StartWithdrawal();
                hediff.WithdrawalHediff.Severity = Rand.Range(0f, 1f);
            }
        }
    }

    public class CurseWorker_SunBlock : CurseWorker
    {
        public override void Apply(Map map)
        {
            var cond = GameConditionMaker.MakeCondition(GameConditionDefOf.UnnaturalDarkness);
            cond.Permanent = true;
            map.gameConditionManager.RegisterCondition(cond);
            map.gameConditionManager.mapBrightnessTracker.targetBrightness = 0;
            map.gameConditionManager.mapBrightnessTracker.brightness = 0;
            cond.startTick = Find.TickManager.TicksGame - cond.TransitionTicks;
            map.mapDrawer.WholeMapChanged(MapMeshFlagDefOf.GroundGlow);
            var noctolGroup = GetNoctolsForPoints(StorytellerUtility.DefaultSiteThreatPointsNow(), map);
            foreach (var noctol in noctolGroup)
            {
                noctol.SetFaction(Faction.OfPlayer);
                IntVec3 spawnLoc;
                if (CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => c.Standable(map) && !c.Fogged(map), map, CellFinder.EdgeRoadChance_Animal, out spawnLoc))
                {
                    GenSpawn.Spawn(noctol, spawnLoc, map, Rot4.Random);
                }
            }
        }
        
        private static readonly FloatRange NoctolPointsFactorRange = new FloatRange(0.8f, 1f);

        private List<Pawn> GetNoctolsForPoints(float points, Map map)
        {
            PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms
            {
                groupKind = PawnGroupKindDefOf.Noctols,
                tile = map.Tile,
                faction = Faction.OfEntities
            };
            pawnGroupMakerParms.points = ((points > 0f) ? points : StorytellerUtility.DefaultThreatPointsNow(map)) * NoctolPointsFactorRange.RandomInRange;
            pawnGroupMakerParms.points = Mathf.Max(pawnGroupMakerParms.points, pawnGroupMakerParms.faction.def.MinPointsToGeneratePawnGroup(pawnGroupMakerParms.groupKind) * 1.05f);
            return PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms)?.ToList();
        }
    }

    public class CurseWorker_MetalHorror : CurseWorker
    {
        public override void Apply(Map map)
        {
            foreach (var enemy in GetAllHostilePawns(map))
            {
                MetalhorrorUtility.Infect(enemy);
                var hediff = enemy.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.MetalhorrorImplant);
                if (hediff != null)
                {
                    hediff.Severity = Rand.Range(0, 1f);
                }
            }
        }
    }
}
