using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using System.Text;
using Verse.Sound;
using Verse.AI;
namespace DanceOfEvolution
{
	public class Building_Cerebrum : Building, IThingHolder, IStoreSettingsParent, IStorageGroupMember,
	IHaulDestination, IHaulSource, ITargetingSource, IThingHolderEvents<Thing>
	{
		public float growth;
		private ThingOwner<Thing> innerContainer;
		private StorageSettings settings;
		private StorageGroup storageGroup;
		private Texture2D activateTex;
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
		bool IStorageGroupMember.DrawConnectionOverlay => base.Spawned;
		Map IStorageGroupMember.Map => base.MapHeld;
		string IStorageGroupMember.StorageGroupTag => def.building.storageGroupTag;
		StorageSettings IStorageGroupMember.StoreSettings => GetStoreSettings();
		StorageSettings IStorageGroupMember.ParentStoreSettings => GetParentStoreSettings();
		StorageSettings IStorageGroupMember.ThingStoreSettings => settings;
		bool IStorageGroupMember.DrawStorageTab => true;
		bool IStorageGroupMember.ShowRenameButton => base.Faction == Faction.OfPlayer;
		public bool CasterIsPawn => true;

		public bool IsMeleeAttack => false;

		public bool Targetable => true;

		public bool MultiSelect => false;

		public bool HidePawnTooltips => false;

		public Thing Caster => this;

		public Pawn CasterPawn => null;

		public Verb GetVerb => null;

		public Texture2D UIIcon
		{
			get
			{
				if (activateTex == null)
				{
					activateTex = ContentFinder<Texture2D>.Get("UI/Designators/Harvest", true);
				}
				return activateTex;
			}
		}

		public TargetingParameters targetParams
		{
			get
			{
				return new TargetingParameters
				{
					canTargetPawns = true,
					canTargetAnimals = false,
					canTargetMechs = false,
					canTargetHumans = true,
					canTargetItems = false
				};
			}
		}

		public ITargetingSource DestinationSelector => null;

		public Building_Cerebrum()
		{
			innerContainer = new ThingOwner<Thing>(this, oneStackOnly: false);
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
			var nutrition = t.GetStatValue(StatDefOf.Nutrition);
			if (nutrition <= 0f)
			{
				return false;
			}
			if (growth >= 1)
			{
				return false;
			}
			var result = GetStoreSettings().AllowedToAccept(t) && innerContainer.CanAcceptAnyOf(t);
			return result;
		}

		public int SpaceRemainingFor(ThingDef _)
		{
			return growth < 1 ? int.MaxValue : 0;
		}

		public void Notify_SettingsChanged()
		{
			if (base.Spawned)
			{
				base.MapHeld.listerHaulables.Notify_HaulSourceChanged(this);
			}
		}
		public const float NutritionToGrowth = 60f;
		public void Notify_ItemAdded(Thing item)
		{
			base.MapHeld.listerHaulables.Notify_AddedThing(item);
			var nutrition = item.GetStatValue(StatDefOf.Nutrition) * item.stackCount;
			if (def == DefsOf.DE_HardenedCerebrum)
			{
				nutrition *= 2;
			}
			growth += nutrition / NutritionToGrowth;
			growth = Mathf.Clamp01(growth);
			item.Destroy();
		}
		public void Notify_ItemRemoved(Thing item)
		{
			base.MapHeld.listerHaulables.Notify_HaulSourceChanged(this);
		}

		public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
		{
			if (storageGroup != null)
			{
				storageGroup?.RemoveMember(this);
				storageGroup = null;
			}
			innerContainer?.TryDropAll(base.Position, base.Map, ThingPlaceMode.Near);
			base.DeSpawn(mode);
		}

		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
		}
		const int DaysToMatureFully = 120;
		public override void Tick()
		{
			base.Tick();
			int totalTicksToMature = GenDate.TicksPerDay * DaysToMatureFully;
			float growthPerTick = 1f / totalTicksToMature;
			growth += growthPerTick;
			if (growth > 1f)
			{
				growth = 1f;
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (var g in base.GetGizmos())
			{
				yield return g;
			}
			if (growth >= 1)
			{
				Command_Action command_Action = new Command_Action
				{
					defaultLabel = "DesignatorHarvest".Translate(),
					defaultDesc = "DesignatorHarvestDesc".Translate(),
					icon = UIIcon,
					action = delegate
					{
						SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
						Find.Targeter.BeginTargeting(this);
					}
				};
				AcceptanceReport acceptanceReport = CanInteract();
				if (!acceptanceReport.Accepted)
				{
					command_Action.Disable(acceptanceReport.Reason.CapitalizeFirst());
				}
				yield return command_Action;
			}
		}

		public virtual AcceptanceReport CanInteract(Pawn activateBy = null)
		{
			if (activateBy != null)
			{
				if (activateBy.HasFungalNexus() is false)
				{
					return "DE_OnlyFungalNexusCanHarvestThis".Translate();
				}
				if (activateBy.Dead)
				{
					return "PawnIsDead".Translate(activateBy);
				}
				if (activateBy.Downed)
				{
					return "MessageRitualPawnDowned".Translate(activateBy);
				}
				if (activateBy.Deathresting)
				{
					return "IsDeathresting".Translate(activateBy.Named("PAWN"));
				}
				if (!activateBy.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
				{
					return "MessageIncapableOfManipulation".Translate(activateBy);
				}
				if (!activateBy.CanReach(this, PathEndMode.ClosestTouch, Danger.Deadly))
				{
					return "CannotReach".Translate();
				}
			}
			return true;
		}

		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
		{
			foreach (var f in base.GetFloatMenuOptions(selPawn))
			{
				yield return f;
			}
			if (growth >= 1)
			{
				AcceptanceReport acceptanceReport = CanInteract(selPawn);
				FloatMenuOption floatMenuOption = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("DesignatorHarvest".Translate(), delegate
				{
					OrderForceTarget(selPawn);
				}), selPawn, this);
				if (!acceptanceReport.Accepted)
				{
					floatMenuOption.Disabled = true;
					floatMenuOption.Label = floatMenuOption.Label + " (" + acceptanceReport.Reason.UncapitalizeFirst() + ")";
				}
				yield return floatMenuOption;
			}
		}

		public override string GetInspectString()
		{
			var sb = new StringBuilder(base.GetInspectString());
			if (growth < 1)
			{
				int totalTicksToMature = GenDate.TicksPerDay * DaysToMatureFully;
				float remainingGrowth = 1f - growth;
				int remainingTicks = Mathf.CeilToInt(totalTicksToMature * remainingGrowth);
				sb.AppendLine("DE_PeriodUntilMaturity".Translate(remainingTicks.ToStringTicksToPeriod()));
			}
			else
			{
				sb.AppendLine("DE_Mature".Translate());
			}
			return sb.ToString().TrimEndNewlines();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
			Scribe_Deep.Look(ref settings, "settings", this);
			Scribe_References.Look(ref storageGroup, "storageGroup");
			Scribe_Values.Look(ref growth, "growth");
		}

		public bool CanHitTarget(LocalTargetInfo target)
		{
			return ValidateTarget(target, showMessages: false);
		}

		public bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
		{
			if (!target.IsValid || target.Pawn == null)
			{
				return false;
			}
			Pawn pawn = target.Pawn;
			AcceptanceReport acceptanceReport = CanInteract(pawn);
			if (!acceptanceReport.Accepted)
			{
				if (showMessages && !acceptanceReport.Reason.NullOrEmpty())
				{
					Messages.Message("CannotGenericWorkCustom".Translate("DesignatorHarvest".Translate()) + ": " + acceptanceReport.Reason.CapitalizeFirst(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return true;
		}

		public void DrawHighlight(LocalTargetInfo target)
		{
			if (target.IsValid)
			{
				GenDraw.DrawTargetHighlight(target);
			}
		}

		public void OrderForceTarget(LocalTargetInfo target)
		{
			if (ValidateTarget(target, showMessages: false))
			{
				Job job = JobMaker.MakeJob(DefsOf.DE_HarvestCerebrum, this);
				target.Pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
			}
		}

		public void OnGUI(LocalTargetInfo target)
		{
			string label = "VoidMonolithChooseActivator".Translate();
			Widgets.MouseAttachedLabel(label);
			if (ValidateTarget(target, showMessages: false) && targetParams.CanTarget(target.Pawn, this))
			{
				GenUI.DrawMouseAttachment(UIIcon);
			}
			else
			{
				GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
			}
		}
	}
}
