using System.Linq;
using System.Text;
using UnityEngine;
using Verse.Sound;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    public class Building_CubeFungi : Building, IThingHolder, IStoreSettingsParent, IStorageGroupMember, IHaulDestination, IHaulSource, ISearchableContents
    {
        private const int MaxNutrition = 60;
        private ThingOwner<Thing> innerContainer;
        private StorageSettings settings;
        private StorageGroup storageGroup;
        public IEnumerable<Thing> StoredItems => innerContainer.InnerListForReading;
        public bool StorageTabVisible => true;
        StorageGroup IStorageGroupMember.Group
        {
            get
            {
                return storageGroup;
            }
            set
            {
                storageGroup = value;
            }
        }

        bool IStorageGroupMember.DrawConnectionOverlay => Spawned;
        Map IStorageGroupMember.Map => MapHeld;
        string IStorageGroupMember.StorageGroupTag => def.building.storageGroupTag;
        StorageSettings IStorageGroupMember.StoreSettings => GetStoreSettings();
        StorageSettings IStorageGroupMember.ParentStoreSettings => GetParentStoreSettings();
        StorageSettings IStorageGroupMember.ThingStoreSettings => settings;
        bool IStorageGroupMember.DrawStorageTab => true;
        bool IStorageGroupMember.ShowRenameButton => Faction == Faction.OfPlayer;
        public ThingOwner SearchableContents => innerContainer;
        public bool depositFood = true;

        public float containedBioferrite;
        public bool unloadingEnabled = true;
        private bool initalized;
        private const int HaulingThreshold = 30;
        private const float MaxBioferriteCapacity = 30f;
        private CompFacility facilityComp;
        private CompCableConnection cableConnection;
        private Sustainer workingSustainer;
        private List<Thing> previousPlatforms;

        public RimWorld.CompFacility FacilityComp => facilityComp ?? (facilityComp = GetComp<RimWorld.CompFacility>());
        public List<Thing> Platforms => FacilityComp.LinkedBuildings;
        public bool ReadyForHauling => Mathf.FloorToInt(containedBioferrite) >= HaulingThreshold;

        private float BioferritePerDay
        {
            get
            {
                if (CurrentNutrition() <= 0)
                {
                    return 0f;
                }
                float num = 0f;
                foreach (Thing platform in Platforms)
                {
                    if (platform is Building_HoldingPlatform { Occupied: not false } building_HoldingPlatform)
                    {
                        num += CompProducesBioferrite.BioferritePerDay(building_HoldingPlatform.HeldPawn);
                    }
                }
                return num * 1.5f;
            }
        }

        public CompCableConnection CableConnection => cableConnection ?? (cableConnection = GetComp<CompCableConnection>());

        public Building_CubeFungi()
        {
            innerContainer = new ThingOwner<Thing>(this, oneStackOnly: false);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                Initialize();
                RemoveCubeAddictionFromAllPawns(map);
            }
        }

        public override void PostMake()
        {
            base.PostMake();
            if (def.building.defaultStorageSettings.filter.allowedDefs != null)
            {
                def.building.defaultStorageSettings.filter.allowedDefs.Remove(DefsOf.DE_FungalSlurry);
                def.building.defaultStorageSettings.filter.allowedDefs.RemoveWhere(x => x.IsNutritionGivingIngestible is false);
                def.building.defaultStorageSettings.filter.allowedDefs.RemoveWhere(x => x.IsCorpse && x.race.IsMechanoid);
            }
            if (def.building.defaultStorageSettings.filter.thingDefs != null)
            {
                def.building.defaultStorageSettings.filter.thingDefs.Remove(DefsOf.DE_FungalSlurry);
                def.building.defaultStorageSettings.filter.thingDefs.RemoveWhere(x => x.IsNutritionGivingIngestible is false);
                def.building.defaultStorageSettings.filter.thingDefs.RemoveWhere(x => x.IsCorpse && x.race.IsMechanoid);
            }
            settings = new StorageSettings(this);
            if (def.building.defaultStorageSettings != null)
            {
                settings.CopyFrom(def.building.defaultStorageSettings);
            }
        }

        private void RemoveCubeAddictionFromAllPawns(Map map)
        {
            List<HediffDef> hediffsToClear = new List<HediffDef>
            {
                HediffDefOf.CubeInterest,
                HediffDefOf.CubeWithdrawal,
                HediffDefOf.CubeComa,
                HediffDefOf.CubeRage
            };
            foreach (Pawn pawn in map.mapPawns.AllPawns)
            {
                if (pawn?.health?.hediffSet != null)
                {
                    foreach (Hediff hediff in pawn.health.hediffSet.hediffs.Where(h => hediffsToClear.Contains(h.def)).ToList())
                    {
                        pawn.health.RemoveHediff(hediff);
                    }
                }
            }
        }

        public StorageSettings GetStoreSettings()
        {
            return settings;
        }

        public StorageSettings GetParentStoreSettings()
        {
            return def.building.fixedStorageSettings;
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return innerContainer;
        }

        public bool Accepts(Thing t)
        {
            var nutrition = t.GetStatValue(StatDefOf.Nutrition) * t.stackCount;
            if (nutrition <= 0f)
            {
                return false;
            }
            float currentNutrition = CurrentNutrition();
            if (currentNutrition + nutrition > MaxNutrition)
            {
                return false;
            }
            var result = depositFood && GetStoreSettings().AllowedToAccept(t) && innerContainer.CanAcceptAnyOf(t);
            return result;
        }

        public float CurrentNutrition()
        {
            return StoredItems.Sum(x => x.GetStatValue(StatDefOf.Nutrition) * x.stackCount);
        }

        public int SpaceRemainingFor(ThingDef _)
        {
            return (int)(MaxNutrition - CurrentNutrition());
        }

        public void Notify_SettingsChanged()
        {
            if (Spawned)
            {
                MapHeld.listerHaulables.Notify_HaulSourceChanged(this);
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (this.IsHashIntervalTick(60))
            {
                CheckForPlatformChanges();
            }
            
            if (this.IsHashIntervalTick(250))
            {
                containedBioferrite = Mathf.Min(containedBioferrite + BioferritePerDay / 60000f * 250f, MaxBioferriteCapacity);
                if (CurrentNutrition() > 0f && Platforms.Any(p => p is Building_HoldingPlatform { Occupied: true }))
                {
                    float nutritionToConsume = 3f / GenDate.TicksPerDay * 250f;
                    float currentNutrition = CurrentNutrition();
                    if (currentNutrition >= nutritionToConsume)
                    {
                        ConsumeNutrition(nutritionToConsume);
                    }
                    else
                    {
                        ConsumeNutrition(currentNutrition);
                    }
                }
            }
            if (IsWorking())
            {
                if (workingSustainer == null)
                {
                    workingSustainer = SoundDefOf.BioferriteHarvester_Ambient.TrySpawnSustainer(SoundInfo.InMap(this));
                }
                workingSustainer.Maintain();
            }
            else
            {
                workingSustainer?.End();
                workingSustainer = null;
            }
        }

        private void ConsumeNutrition(float amount)
        {
            float remaining = amount;
            List<Thing> thingsToConsume = StoredItems.OrderBy(x => x.GetStatValue(StatDefOf.Nutrition) / x.stackCount).ToList();

            foreach (Thing thing in thingsToConsume)
            {
                float nutritionPerStack = thing.GetStatValue(StatDefOf.Nutrition);
                int numToConsume = Mathf.CeilToInt(remaining / nutritionPerStack);
                numToConsume = Mathf.Min(numToConsume, thing.stackCount);

                float consumedNutrition = numToConsume * nutritionPerStack;
                thing.SplitOff(numToConsume);
                remaining -= consumedNutrition;

                if (remaining <= 0f)
                {
                    break;
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
            Scribe_Deep.Look(ref settings, "settings", this);
            Scribe_References.Look(ref storageGroup, "storageGroup");
            Scribe_Values.Look(ref depositFood, "depositFood", true);

            Scribe_Values.Look(ref containedBioferrite, "containedBioferrite", 0f);
            Scribe_Values.Look(ref unloadingEnabled, "unloadingEnabled", defaultValue: false);
            Scribe_Values.Look(ref initalized, "initalized", defaultValue: false);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            yield return new Command_Toggle
            {
                defaultLabel = "DE_DepositFood".Translate(),
                defaultDesc = "DE_DepositFoodDesc".Translate(),
                icon = ThingDefOf.RawPotatoes.uiIcon,
                isActive = () => depositFood,
                toggleAction = () => depositFood = !depositFood
            };

            Command_Toggle command_Toggle = new Command_Toggle();
            command_Toggle.defaultLabel = "BioferriteHarvesterToggleUnloading".Translate();
            command_Toggle.defaultDesc = "BioferriteHarvesterToggleUnloadingDesc".Translate();
            command_Toggle.isActive = () => unloadingEnabled;
            command_Toggle.toggleAction = delegate
            {
                unloadingEnabled = !unloadingEnabled;
            };
            command_Toggle.activateSound = SoundDefOf.Tick_Tiny;
            command_Toggle.icon = ContentFinder<Texture2D>.Get("UI/Commands/BioferriteUnloading");
            yield return command_Toggle;

            if (containedBioferrite >= 1f)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "BioferriteHarvesterEjectContents".Translate();
                command_Action.defaultDesc = "BioferriteHarvesterEjectContentsDesc".Translate(Find.ActiveLanguageWorker.Pluralize(ThingDefOf.Bioferrite.label));
                command_Action.action = delegate
                {
                    EjectContents();
                };
                command_Action.Disabled = containedBioferrite == 0f;
                command_Action.activateSound = SoundDefOf.Tick_Tiny;
                command_Action.icon = ContentFinder<Texture2D>.Get("UI/Commands/EjectBioferrite");
                yield return command_Action;
            }
            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action command_Action2 = new Command_Action();
                command_Action2.defaultLabel = "DEV: Add +1 bioferrite";
                command_Action2.action = delegate
                {
                    containedBioferrite = Mathf.Min(containedBioferrite + 1f, MaxBioferriteCapacity);
                };
                yield return command_Action2;
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
            if (storageGroup != null)
            {
                storageGroup?.RemoveMember(this);
                storageGroup = null;
            }
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            if (mode != DestroyMode.WillReplace)
            {
                innerContainer?.TryDropAll(Position, Map, ThingPlaceMode.Near);
            }
            base.DeSpawn(mode);
            initalized = false;
            workingSustainer?.End();
            workingSustainer = null;
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public bool HaulDestinationEnabled => true;

        public bool HaulSourceEnabled => false;

        private void Initialize()
        {
            if (initalized)
            {
                return;
            }
            initalized = true;
            previousPlatforms = new List<Thing>(Platforms);
            RebuildCables();
        }

        private void CheckForPlatformChanges()
        {
            if (!initalized)
            {
                return;
            }
            previousPlatforms ??= new List<Thing>();
            List<Thing> currentPlatforms = Platforms;
            bool platformsChanged = false;
            foreach (Thing platform in currentPlatforms)
            {
                if (!previousPlatforms.Contains(platform))
                {
                    platformsChanged = true;
                    break;
                }
            }
            if (!platformsChanged)
            {
                foreach (Thing platform in previousPlatforms)
                {
                    if (!currentPlatforms.Contains(platform))
                    {
                        platformsChanged = true;
                        break;
                    }
                }
            }

            if (platformsChanged)
            {
                RebuildCables();
                previousPlatforms = new List<Thing>(currentPlatforms);
            }
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
            if (!initalized)
            {
                Initialize();
            }
        }

        public override bool IsWorking()
        {
            return BioferritePerDay != 0f;
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            if (stringBuilder.Length != 0)
            {
                stringBuilder.AppendLine();
            }
            stringBuilder.Append("BioferriteHarvesterContained".Translate());
            stringBuilder.Append($": {containedBioferrite:F2} / {MaxBioferriteCapacity.ToString()} (+{BioferritePerDay:F2} ");
            stringBuilder.Append(string.Format("{0})", "BioferriteHarvesterPerDay".Translate()));
            stringBuilder.AppendLine();
            stringBuilder.Append("DE_NutritionStored".Translate());
            stringBuilder.Append($": {CurrentNutrition():F2} / {MaxNutrition.ToString()}");
            return stringBuilder.ToString();
        }

        public Thing TakeOutBioferrite()
        {
            int num = Mathf.FloorToInt(containedBioferrite);
            if (num == 0)
            {
                return null;
            }
            containedBioferrite -= num;
            Thing thing = ThingMaker.MakeThing(ThingDefOf.Bioferrite);
            thing.stackCount = num;
            return thing;
        }

        private void EjectContents()
        {
            Thing thing = TakeOutBioferrite();
            if (thing != null)
            {
                GenPlace.TryPlaceThing(thing, Position, Map, ThingPlaceMode.Near);
            }
        }

        public override void Notify_DefsHotReloaded()
        {
            base.Notify_DefsHotReloaded();
            RebuildCables();
        }

        private void RebuildCables()
        {
            CableConnection.RebuildCables(Platforms, (Thing thing) => thing is Building_HoldingPlatform building_HoldingPlatform && building_HoldingPlatform.Occupied);
        }
    }
}
