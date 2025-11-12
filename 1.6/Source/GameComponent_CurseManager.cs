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
        public bool mycelyssEnvoyEventTriggered = false;
        public int mycelyssDemandTick = -1;
        public bool mycelyssDemandActive = false;
        public int requiredPawnCount = 2;
        public static GameComponent_CurseManager Instance;
        private static Material CursedMat = MaterialPool.MatFrom("UI/Icons/CursedSite");
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
                Find.LetterStack.ReceiveLetter("DE_CurseApplied".Translate(), "DE_CurseSuccess".Translate(worldObject.Label), LetterDefOf.PositiveEvent, worldObject, playSound: false);
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
                    cursedSites.Remove(worldObject);
                });
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref cursedSites, "cursedSites", LookMode.Reference);
            Scribe_Values.Look(ref mycelyssEnvoyEventTriggered, "mycelyssEnvoyEventTriggered", false);
            Scribe_Values.Look(ref mycelyssDemandTick, "mycelyssDemandTick", -1);
            Scribe_Values.Look(ref mycelyssDemandActive, "mycelyssDemandActive", false);
            Scribe_Values.Look(ref requiredPawnCount, "requiredPawnCount", 2);
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();
            if (Find.TickManager.TicksGame % 60 == 0)
            {
                cursedSites.RemoveAll(worldObject => worldObject == null || worldObject.Destroyed || !Find.World.worldObjects.Contains(worldObject));
            }
            var faction = Find.FactionManager.FirstFactionOfDef(DefsOf.DE_Mycelyss);
            if (faction != null && faction.HostileTo(Faction.OfPlayer) is false && mycelyssDemandActive && mycelyssDemandTick > 0 && Find.TickManager.TicksGame >= mycelyssDemandTick)
            {
                Map map = Find.AnyPlayerHomeMap;
                if (map != null)
                {
                    foreach (var otherMap in Find.Maps)
                    {
                        if (otherMap.lordManager.lords.Any(l => l.LordJob is LordJob_DemandPawns))
                        {
                            return;
                        }
                    }
                    IncidentParms parms = new IncidentParms
                    {
                        target = map,
                        faction = faction
                    };

                    if (!DefsOf.DE_MycelyssDemand.Worker.TryExecute(parms))
                    {
                        mycelyssDemandTick = Find.TickManager.TicksGame + GenDate.TicksPerDay;
                    }
                }
            }
        }

        public override void GameComponentOnGUI()
        {
            base.GameComponentOnGUI();
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
                if (!cursedSite.HiddenBehindTerrainNow())
                {
                    Material material = CursedMat;
                    Rect rect = ExpandableWorldObjectsUtility.ExpandedIconScreenRect(cursedSite).ExpandedBy(5);
                    Widgets.DrawTextureRotated(rect, CursedMat.mainTexture, 0f, material);
                }
            }
        }
    }
}
