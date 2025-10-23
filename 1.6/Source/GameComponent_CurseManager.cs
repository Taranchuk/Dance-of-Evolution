using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using Verse;
using UnityEngine;

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

        public void AddCursedSite(WorldObject worldObject)
        {
            if (!cursedSites.Contains(worldObject))
            {
                cursedSites.Add(worldObject);
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
                var curseDef = DefDatabase<CurseEffectDef>.AllDefs.RandomElement();
                curseDef.Worker.Apply(map);
                cursedSites.Remove(worldObject);
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
