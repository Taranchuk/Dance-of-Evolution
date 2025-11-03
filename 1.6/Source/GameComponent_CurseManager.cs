using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using Verse;
using UnityEngine;
using RimWorld;
using Verse.Sound;
using LudeonTK;

namespace DanceOfEvolution
{
    [HotSwappable]
    [StaticConstructorOnStartup]
    public class GameComponent_CurseManager : GameComponent
    {
        private List<WorldObject> cursedSites = new List<WorldObject>();
        public static GameComponent_CurseManager Instance;
        private static Material CursedMat = MaterialPool.MatFrom("UI/Overlays/Arrow");
        public GameComponent_CurseManager(Game game)
        {
            Instance = this;
        }

        [DebugAction("General", "Add curse...", false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static List<DebugActionNode> AddCurse()
        {
            List<DebugActionNode> list = new List<DebugActionNode>();
            foreach (var allDef in DefDatabase<CurseEffectDef>.AllDefs)
            {
                var localDef = allDef;
                list.Add(new DebugActionNode(localDef.defName, DebugActionType.Action, delegate
                {
                    localDef.Worker.Apply(Find.CurrentMap);
                }));
            }
            return list;
        }

        public void AddCursedSite(WorldObject worldObject)
        {
            if (!cursedSites.Contains(worldObject))
            {
                cursedSites.Add(worldObject);
                SoundDefOf.Sightstealer_DistantHowl.PlayOneShotOnCamera();
                Find.LetterStack.ReceiveLetter("DE_CurseApplied".Translate(), "DE_CurseSuccess".Translate(worldObject.Label), LetterDefOf.PositiveEvent, worldObject);
                CameraJumper.TryJump(CameraJumper.GetWorldTarget(worldObject));
                if (worldObject.Faction != null && worldObject.Faction.HasGoodwill)
                {
                    worldObject.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -50, reason: HistoryEventDefOf.UsedHarmfulAbility);
                }
            }
        }

        public bool IsCursed(WorldObject worldObject)
        {
            return cursedSites.Contains(worldObject);
        }

        public void ApplyCurse(Map map)
        {
            var worldObject = map.Parent;
            if (IsCursed(worldObject))
            {
                LongEventHandler.toExecuteWhenFinished.Add(delegate
                {
                    //foreach (var curseDef in DefDatabase<CurseEffectDef>.AllDefs)
                    //{
                    //    curseDef.Worker.Apply(map);
                    //}
                    var curseDef = DefDatabase<CurseEffectDef>.AllDefs.RandomElement();
                    curseDef.Worker.Apply(map);
                    Log.Message("Applied curse " + curseDef);
                    cursedSites.Remove(worldObject);
                });
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref cursedSites, "cursedSites", LookMode.Reference);
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();
            if (Find.TickManager.TicksGame % 60 == 0)
            {
                cursedSites.RemoveAll(worldObject => worldObject == null || worldObject.Destroyed || !Find.World.worldObjects.Contains(worldObject));
            }
        }

        public override void GameComponentUpdate()
        {
            base.GameComponentUpdate();
            if (Current.ProgramState != ProgramState.Playing || WorldRendererUtility.WorldSelected is false)
            {
                return;
            }

            foreach (var cursedSite in cursedSites)
            {
                if (PlanetLayer.Selected != cursedSite.Tile.Layer)
                {
                    continue;
                }
                WorldGrid worldGrid = Find.WorldGrid;
                Vector2 vector = GenWorldUI.WorldToUIPosition(worldGrid.GetTileCenter(cursedSite.Tile));
                Rect rect = new Rect(0f, 0f, UI.screenWidth, UI.screenHeight);
                rect = rect.ContractedBy(0.1f * rect.width, 0.1f * rect.height);
                bool num = rect.Contains(vector);
                if (num)
                {
                    Vector3 tileCenter = worldGrid.GetTileCenter(cursedSite.Tile);
                    float averageTileSize = cursedSite.Tile.Layer.AverageTileSize;
                    WorldRendererUtility.DrawQuadTangentialToPlanet(pos: tileCenter, size: 2f * averageTileSize, altOffset: 0.05f, material: CursedMat);
                }
            }
        }
    }
}
