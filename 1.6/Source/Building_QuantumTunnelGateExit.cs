using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace DanceOfEvolution
{
    [HotSwappable]
    public class Building_QuantumTunnelGateExit : PocketMapExit
    {
        private static new readonly CachedTexture ExitMapTex = new CachedTexture("UI/Commands/ExitCave");
        public static new readonly CachedTexture ViewEntranceTex = new CachedTexture("UI/Commands/ViewCave");
        private bool isCollapsing;
        private int collapseTick;
        private Sustainer collapseSustainer;
        private Effecter collapseEffecter1;
        private Effecter collapseEffecter2;

        public override string EnterString => "DE_ExitQuantumTunnel".Translate();

        public override string CancelEnterString => "DE_CancelExitQuantumTunnel".Translate();

        public override string GetInspectString()
        {
            string baseInspectString = base.GetInspectString();
            string collapsingIn = "DE_QuantumTunnelCollapsingIn".Translate(TicksUntilCollapse.ToStringTicksToPeriodVerbose());
            if (string.IsNullOrEmpty(baseInspectString))
            {
                return collapsingIn;
            }
            return baseInspectString + "\n" + collapsingIn;
        }

        public override Texture2D EnterTex => ExitMapTex.Texture;

        public override bool AutoDraftOnEnter => false;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                if (PocketMapUtility.currentlyGeneratingPortal == null)
                {
                    Log.Error("Quantum tunnel exit could not find map portal to connect to");
                    return;
                }
                this.entrance = PocketMapUtility.currentlyGeneratingPortal;
                if (this.entrance != null)
                {
                    this.entrance.exit = this;
                }
                collapseTick = Find.TickManager.TicksGame + GenDate.TicksPerHour * 10;
            }
        }

        public override Map GetOtherMap()
        {
            if (entrance != null && entrance.Map != null)
            {
                return entrance.Map;
            }
            Log.Warning("Quantum tunnel gate exit entrance is not properly connected, returning current map as fallback");
            return Map;
        }

        public override IntVec3 GetDestinationLocation()
        {
            if (entrance != null && entrance.Position != IntVec3.Invalid)
            {
                return entrance.Position;
            }
            return IntVec3.Invalid;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref isCollapsing, "isCollapsing", defaultValue: false);
            Scribe_Values.Look(ref collapseTick, "collapseTick", 0);
        }

        public bool IsCollapsing => isCollapsing;

        public int CollapseStage
        {
            get
            {
                if (collapseTick - Find.TickManager.TicksGame >= 3600)
                {
                    return 1;
                }
                return 2;
            }
        }

        public int TicksUntilCollapse => collapseTick - Find.TickManager.TicksGame;

        public override void Tick()
        {
            base.Tick();
            if (IsCollapsing)
            {
                if (CollapseStage == 1)
                {
                    if (collapseEffecter1 == null)
                    {
                        collapseEffecter1 = EffecterDefOf.PitGateAboveGroundCollapseStage1.Spawn(this, base.Map);
                    }
                }
                else if (CollapseStage == 2)
                {
                    if (collapseSustainer == null)
                    {
                        collapseSustainer = SoundDefOf.PitGateCollapsing.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
                    }
                    collapseSustainer.Maintain();
                    if (collapseEffecter2 == null)
                    {
                        collapseEffecter2 = EffecterDefOf.PitGateAboveGroundCollapseStage2.Spawn(this, base.Map);
                    }
                    if (Find.CurrentMap == base.Map && Rand.MTBEventOccurs(2f, 60f, 1f))
                    {
                        Find.CameraDriver.shaker.DoShake(0.2f);
                    }
                }
                collapseEffecter1?.EffectTick(this, this);
                collapseEffecter2?.EffectTick(this, this);
                if (Find.TickManager.TicksGame >= collapseTick)
                {
                    Collapse();
                }
            }
        }

        private void BeginCollapsing()
        {
            isCollapsing = true;
        }

        private void Collapse()
        {
            collapseSustainer?.End();
            collapseEffecter2?.Cleanup();
            collapseEffecter2 = null;
            collapseEffecter1?.Cleanup();
            collapseEffecter1 = null;
            EffecterDefOf.PitGateAboveGroundCollapsed.Spawn(base.Position, base.Map);
            SoundDefOf.PitGateCollapsing_End.PlayOneShot(new TargetInfo(base.Position, base.Map));
            if (entrance != null)
            {
                entrance.exit = null;
            }
            allowDestroyNonDestroyable = true;
            Destroy(DestroyMode.Deconstruct);
            allowDestroyNonDestroyable = false;
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
                    Find.LetterStack.ReceiveLetter(def.portal.enteredLetterLabel, def.portal.enteredLetterText.Formatted(pawn.Named("PAWN")), def.portal.enteredLetterDef, entrance);
                }
            }
            if (Find.CurrentMap == Map)
            {
                def.portal.traverseSound?.PlayOneShot(this);
            }
            else if (entrance != null && Find.CurrentMap == entrance.Map)
            {
                def.portal.traverseSound?.PlayOneShot(entrance);
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                if (gizmo is Command_Action commandAction && commandAction.defaultLabel == "CommandViewSurface".Translate())
                {
                    continue;
                }
                yield return gizmo;
            }
            yield return new Command_Action
            {
                defaultLabel = "DE_ViewOriginGate".Translate(),
                defaultDesc = "DE_ViewOriginGateDesc".Translate(),
                icon = ViewEntranceTex.Texture,
                action = delegate
                {
                    CameraJumper.TryJumpAndSelect(entrance);
                }
            };

            yield return new Command_Action
            {
                defaultLabel = "DE_CloseGate".Translate(),
                defaultDesc = "DE_CloseGateDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/ExitCave"),
                action = delegate
                {
                    entrance.exit = null;
                    allowDestroyNonDestroyable = true;
                    Destroy();
                    allowDestroyNonDestroyable = false;
                }
            };
        }
    }
}
