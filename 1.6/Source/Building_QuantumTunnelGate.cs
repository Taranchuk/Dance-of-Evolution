using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace DanceOfEvolution
{
    [HotSwappable]
    public class Building_QuantumTunnelGate : MapPortal
    {
        public PlanetTile originTile;
        private PlanetTile destinationTile;
        private CompQuantumTunnel quantumTunnel;
        private PlanetLayer cachedLayer;
        private PlanetTile cachedOrigin;
        private PlanetTile cachedClosest;
        public override bool AutoDraftOnEnter => false;
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            quantumTunnel = GetComp<CompQuantumTunnel>();
            if (!respawningAfterLoad)
            {
                if (originTile == PlanetTile.Invalid)
                {
                    originTile = map.Tile;
                }
                Find.CameraDriver.shaker.DoShake(0.1f, 120);
                SoundDefOf.PitGateOpen.PlayOneShot(SoundInfo.InMap(this));
            }
        }

        public override Map GetOtherMap()
        {
            if (exit != null && exit.Map != null)
            {
                return exit.Map;
            }
            Log.Warning("Quantum tunnel gate exit is not properly connected, returning current map as fallback");
            return Map;
        }
        
        public override IntVec3 GetDestinationLocation()
        {
            if (exit != null && exit.Position != IntVec3.Invalid)
            {
                return exit.Position;
            }
            return IntVec3.Invalid;
        }

        public override void OnEntered(Pawn pawn)
        {
            QuantumTunnelingUtility.TryApplyMoodlet(pawn);
            Notify_ThingAdded(pawn);
            if (!beenEntered)
            {
                beenEntered = true;
                if (!def.portal.enteredLetterLabel.NullOrEmpty())
                {
                    Find.LetterStack.ReceiveLetter(def.portal.enteredLetterLabel, def.portal.enteredLetterText.Formatted(pawn.Named("PAWN")), def.portal.enteredLetterDef, exit);
                }
            }
            if (Find.CurrentMap == Map)
            {
                def.portal.traverseSound?.PlayOneShot(this);
            }
            else if (exit != null && Find.CurrentMap == exit.Map)
            {
                def.portal.traverseSound?.PlayOneShot(exit);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref originTile, "originTile");
            Scribe_Values.Look(ref destinationTile, "destinationTile");
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (this.exit is null || this.exit.Destroyed)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DE_SelectDestination".Translate(),
                    defaultDesc = "DE_SelectDestinationDesc".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/EnterCave"),
                    action = () =>
                    {
                        StartWorldTargeting();
                    },
                    disabled = Map.resourceCounter.GetCount(ThingDefOf.Bioferrite) <= 0,
                    disabledReason = "DE_NotEnoughBioferriteToSelectDestination".Translate()
                };
            }
            else
            {
                foreach (Gizmo gizmo in base.GetGizmos())
                {
                    yield return gizmo;
                }
                yield return new Command_Action
                {
                    defaultLabel = "DE_ViewExitGate".Translate(),
                    defaultDesc = "DE_ViewExitGateDesc".Translate(),
                    icon = Building_QuantumTunnelGateExit.ViewEntranceTex.Texture,
                    action = delegate
                    {
                        CameraJumper.TryJumpAndSelect(exit);
                    }
                };
            }
        }

        private void StartWorldTargeting()
        {
            CameraJumper.TryJump(CameraJumper.GetWorldTarget(this));
            cachedLayer = null;
            cachedOrigin = PlanetTile.Invalid;
            cachedClosest = PlanetTile.Invalid;
            Find.WorldSelector.ClearSelection();
            Find.WorldTargeter.BeginTargeting(
                (GlobalTargetInfo globalTarget) =>
                {
                    float distance = QuantumTunnelingUtility.GetDistance(Map.Tile, globalTarget.Tile);
                    int totalBioferriteCost = (int)(distance * quantumTunnel.Props.bioferriteCostPerHex);
                    if (Map.resourceCounter.GetCount(ThingDefOf.Bioferrite) < totalBioferriteCost)
                    {
                        Messages.Message("DE_NotEnoughBioferriteForTeleport".Translate(totalBioferriteCost, Map.resourceCounter.GetCount(ThingDefOf.Bioferrite)), MessageTypeDefOf.RejectInput);
                        return false;
                    }
                    if (Find.WorldObjects.MapParentAt(globalTarget.Tile) is MapParent mapParent && mapParent.Map != null)
                    {
                        Current.Game.CurrentMap = mapParent.Map;
                    }
                    StartTeleport(globalTarget.Tile);
                    return true;
                },
                true,
                null,
                true, onUpdate: delegate
                {
                    PlanetTile tile = this.Map.Tile;
                    PlanetTile planetTile;
                    if (cachedLayer != Find.WorldSelector.SelectedLayer || cachedOrigin != tile)
                    {
                        cachedLayer = Find.WorldSelector.SelectedLayer;
                        cachedOrigin = tile;
                        planetTile = cachedClosest = Find.WorldSelector.SelectedLayer.GetClosestTile_NewTemp(tile);
                    }
                    else
                    {
                        planetTile = cachedClosest;
                    }
                    int availableBioferrite = Map.resourceCounter.GetCount(ThingDefOf.Bioferrite);
                    int maxDistance = (int)(availableBioferrite / (float)quantumTunnel.Props.bioferriteCostPerHex);
                    GenDraw.DrawWorldRadiusRing(planetTile, maxDistance);
                },
                null,
                null,
                null
            );
        }

        private void StartTeleport(PlanetTile tile)
        {
            destinationTile = tile;

            if (!ValidateDestination(tile))
            {
                return;
            }

            float distance = QuantumTunnelingUtility.GetDistance(Map.Tile, destinationTile);
            int totalBioferriteCost = (int)(distance * quantumTunnel.Props.bioferriteCostPerHex);

            if (Map.resourceCounter.GetCount(ThingDefOf.Bioferrite) < totalBioferriteCost)
            {
                Messages.Message("DE_NotEnoughBioferriteForTeleport".Translate(totalBioferriteCost, Map.resourceCounter.GetCount(ThingDefOf.Bioferrite)), MessageTypeDefOf.RejectInput);
                return;
            }
            Find.CameraDriver.shaker.DoShake(0.1f, 120);
            SoundDefOf.PitGateOpen.PlayOneShot(SoundInfo.InMap(this));
            ExecuteTeleport();
        }

        private void ConsumeBioferrite(int totalBioferriteCost)
        {
            int toRemove = totalBioferriteCost;
            List<Thing> bioferriteStacks = Map.listerThings.ThingsOfDef(ThingDefOf.Bioferrite);
            for (int i = 0; i < bioferriteStacks.Count; i++)
            {
                int num = Mathf.Min(toRemove, bioferriteStacks[i].stackCount);
                Thing splitOffThing = bioferriteStacks[i].SplitOff(num);
                splitOffThing.Destroy();
                toRemove -= num;
                if (toRemove <= 0) break;
            }
        }

        private bool ValidateDestination(PlanetTile tile)
        {
            MapParent mapParent = Find.WorldObjects.MapParentAt(tile);
            if (mapParent != null && mapParent.HasMap)
            {
                Map destinationMap = mapParent.Map;
                if (!DropCellFinder.TryFindDropSpotNear(destinationMap.Center, destinationMap, out IntVec3 _, allowFogged: false, canRoofPunch: false, maxRadius: 99999, allowIndoors: true, size: null))
                {
                    Messages.Message("DE_NoDropSpotAvailable".Translate(), MessageTypeDefOf.RejectInput);
                    return false;
                }
                if (destinationMap.Parent.Faction != null && destinationMap.Parent.Faction.HostileTo(Faction.OfPlayer))
                {
                    Messages.Message("DE_HostileFactionAtDestination".Translate(), MessageTypeDefOf.RejectInput);
                    return false;
                }
            }

            return true;
        }

        private void ExecuteTeleport()
        {
            float distance = QuantumTunnelingUtility.GetDistance(Map.Tile, destinationTile);
            int totalBioferriteCost = (int)(distance * quantumTunnel.Props.bioferriteCostPerHex);

            if (Map.resourceCounter.GetCount(ThingDefOf.Bioferrite) < totalBioferriteCost)
            {
                Messages.Message("DE_NotEnoughBioferriteForTeleport".Translate(totalBioferriteCost, Map.resourceCounter.GetCount(ThingDefOf.Bioferrite)), MessageTypeDefOf.RejectInput);
                return;
            }

            ConsumeBioferrite(totalBioferriteCost);

            MapParent mapParent = Find.WorldObjects.MapParentAt(destinationTile);
            if (mapParent != null && mapParent.HasMap)
            {
                Current.Game.CurrentMap = mapParent.Map;
                FinalizeTeleport(mapParent.Map);
            }
            else
            {
                LongEventHandler.QueueLongEvent(delegate
                {
                    Map newMap = GetOrGenerateMap(destinationTile);
                    Current.Game.CurrentMap = newMap;
                    FinalizeTeleport(newMap);
                }, "GeneratingMap", false, null);
            }
        }

        private void FinalizeTeleport(Map destinationMap)
        {
            if (!DropCellFinder.TryFindDropSpotNear(destinationMap.Center, destinationMap, out var dropSpot, allowFogged: false, canRoofPunch: false))
            {
                dropSpot = destinationMap.Center;
            }
            var spawner = (BuildingGroundSpawner_QuantumTunnelExit)ThingMaker.MakeThing(DefsOf.DE_QuantumTunnelGateExit_Spawner);
            spawner.SetOriginalEntranceGate(this);
            GenSpawn.Spawn(spawner, dropSpot, destinationMap);
            Find.CameraDriver.JumpToCurrentMapLoc(dropSpot);
            Find.CameraDriver.shaker.DoShake(0.1f, 120);
            SoundDefOf.PitGateOpen.PlayOneShot(SoundInfo.InMap(new TargetInfo(dropSpot, destinationMap)));
            Find.LetterStack.ReceiveLetter("DE_QuantumTeleportSuccessLabel".Translate(), "DE_QuantumTeleportSuccess".Translate(), LetterDefOf.PositiveEvent, spawner);
        }

        private Map GetOrGenerateMap(PlanetTile tile)
        {
            MapParent mapParent = Find.WorldObjects.MapParentAt(tile);
            if (mapParent == null)
            {
                mapParent = GetOrGenerateMapUtility.GetOrGenerateMap(tile, WorldObjectDefOf.Camp.overrideMapSize ?? Find.World.info.initialMapSize, WorldObjectDefOf.Camp).Parent;
                mapParent.SetFaction(Faction.OfPlayer);
                mapParent.GetComponent<TimedDetectionRaids>()?.StartDetectionCountdown(24000, 60000);
                return mapParent.Map;
            }
            if (mapParent.HasMap is false)
            {
                var size = mapParent.def.overrideMapSize ?? Find.World.info.initialMapSize;
                if (mapParent is Site site)
                {
                    size = site.PreferredMapSize;
                }
                return GenerateMap(tile, size, mapParent.def);
            }
            return mapParent.Map;
        }
        
        private Map GenerateMap(PlanetTile tile, IntVec3 size, WorldObjectDef worldObjectDef)
        {
            var orGenerateMap = GetOrGenerateMapUtility.GetOrGenerateMap(tile, size, worldObjectDef);
            if ((orGenerateMap.Parent is Settlement || (orGenerateMap.Parent is Site site2 && site2.parts.All((SitePart part) => part.def.considerEnteringAsAttack))) && orGenerateMap.Parent.Faction != null && orGenerateMap.Parent.Faction != Faction.OfPlayer)
            {
                Find.TickManager.Notify_GeneratedPotentiallyHostileMap();
                TaggedString letterLabel = "LetterLabelCaravanEnteredEnemyBase".Translate();
                TaggedString letterText = "DE_LetterGateLandedInEnemyBase".Translate(orGenerateMap.Parent.Label.ApplyTag(TagType.Settlement, orGenerateMap.Parent.Faction.GetUniqueLoadID())).CapitalizeFirst();
                SettlementUtility.AffectRelationsOnAttacked(orGenerateMap.Parent, ref letterText);
                PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(orGenerateMap.mapPawns.AllPawns, ref letterLabel, ref letterText, "LetterRelatedPawnsSettlement".Translate(Faction.OfPlayer.def.pawnsPlural), informEvenIfSeenBefore: true);
                Find.LetterStack.ReceiveLetter(letterLabel, letterText, LetterDefOf.NeutralEvent);
                Find.GoodwillSituationManager.RecalculateAll(canSendHostilityChangedLetter: true);
            }
            return orGenerateMap;
        }
    }
}
