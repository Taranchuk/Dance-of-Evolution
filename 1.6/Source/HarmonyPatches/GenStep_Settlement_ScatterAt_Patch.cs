using HarmonyLib;
using RimWorld;
using Verse;
using System;
using System.Linq;
using RimWorld.BaseGen;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(GenStep_Settlement), "ScatterAt")]
    public static class GenStep_Settlement_ScatterAt_Patch
    {
        public static bool Prefix(IntVec3 c, Map map, GenStepParams parms)
        {
            if (map.ParentFaction != null && map.ParentFaction.def == DefsOf.DE_Mycelyss)
            {
                foreach (IntVec3 cell in GenRadial.RadialPatternInRadius(50))
                {
                    IntVec3 intVec = cell + c;
                    if (intVec.InBounds(map))
                    {
                        foreach (Thing thing in map.thingGrid.ThingsListAt(intVec).ToList())
                        {
                            thing.Destroy();
                        }
                        if (intVec.Roofed(map))
                        {
                            map.roofGrid.SetRoof(intVec, null);
                        }
                        map.terrainGrid.SetTerrain(intVec, DefsOf.DE_RottenSoil);
                    }
                }
                PrefabDef prefab = DefsOf.DE_MycelyssBase;
                CellRect rect = CellRect.CenteredOn(c, prefab.size.x, prefab.size.z);
                MapGenerator.SetVar("SpawnRect", rect);
                PrefabUtility.SpawnPrefab(prefab, map, c, Rot4.North, map.ParentFaction);
                Lord singlePawnLord = LordMaker.MakeNewLord(map.ParentFaction, new LordJob_DefendBase(map.ParentFaction, rect.CenterCell, 25000), map);
                TraverseParms traverseParms = TraverseParms.For(TraverseMode.PassDoors);
                ResolveParams resolveParams = new ResolveParams();
                resolveParams.rect = rect;
                resolveParams.faction = map.ParentFaction;
                resolveParams.singlePawnLord = singlePawnLord;
                resolveParams.pawnGroupKindDef = PawnGroupKindDefOf.Settlement;
                resolveParams.singlePawnSpawnCellExtraPredicate = (IntVec3 x) => map.reachability.CanReachMapEdge(x, traverseParms);
                resolveParams.pawnGroupMakerParams = new PawnGroupMakerParms();
                resolveParams.pawnGroupMakerParams.tile = map.Tile;
                resolveParams.pawnGroupMakerParams.faction = map.ParentFaction;
                if (parms.sitePart != null && parms.sitePart.parms != null)
                {
                    resolveParams.pawnGroupMakerParams.points = parms.sitePart.parms.points;
                }
                else
                {
                    resolveParams.pawnGroupMakerParams.points = SymbolResolver_Settlement.DefaultPawnsPoints.RandomInRange;
                }
                resolveParams.pawnGroupMakerParams.inhabitants = true;
                BaseGen.globalSettings.map = map;
                BaseGen.symbolStack.Push("pawnGroup", resolveParams);
                BaseGen.Generate();
                return false;
            }
            return true;
        }
    }
}
