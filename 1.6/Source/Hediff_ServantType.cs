using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
namespace DanceOfEvolution
{
	public enum ServantType
	{
		Burrower,
		Small,
		Medium,
		Large,
		Ghoul,
		Special,
		Unstable,
	}

	[HotSwappable]
	public abstract class Hediff_ServantType : HediffWithComps
	{
		public abstract ServantType ServantType { get; }
		public Hediff_FungalNexus masterHediff;
		private HediffStage stage;
		public override HediffStage CurStage
		{
			get
			{
				if (stage is null)
				{
					SetupStage();
				}
				return stage;
			}
		}
		public override bool ShouldRemove => masterHediff is null || masterHediff.pawn is null;
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref masterHediff, "master");
			if (Scribe.mode == LoadSaveMode.PostLoadInit && masterHediff != null)
			{
				SetupStage();
			}
		}


		public void SetupStage()
		{
			if (masterHediff is null)
			{
				return;
			}
			stage = def.stages[CurStageIndex].Clone();
			var coordinator = masterHediff.pawn.health.hediffSet.GetFirstHediffOfDef(DefsOf.DE_PsychicCoordinatorImplant) as Hediff_Level;
			if (coordinator != null)
			{
				stage.statOffsets ??= new List<StatModifier>();
				var modifier = stage.statOffsets.FirstOrDefault(sm => sm.stat == StatDefOf.WorkSpeedGlobal);
				if (modifier is null)
				{
					modifier = new StatModifier
					{
						stat = StatDefOf.WorkSpeedGlobal,
					};
					stage.statOffsets.Add(modifier);
				}
				modifier.value = coordinator.level * 0.02f;
			}
			if (this is not Hediff_UnstableServant)
			{
				var growthStimulator = masterHediff.pawn.health.hediffSet.GetFirstHediffOfDef(DefsOf.DE_GrowthStimulatorImplant) as Hediff_Level;
				if (growthStimulator != null)
				{
					stage.regeneration += 10 * growthStimulator.level * masterHediff.pawn.GetStatValue(StatDefOf.PsychicSensitivity);
					stage.showRegenerationStat = true;
				}
			}
			if (ModsConfig.OdysseyActive)
			{
				var vacuumResistanceImplantHediff = masterHediff.pawn.health.hediffSet.GetFirstHediffOfDef(DefsOf.DE_VacuumResistanceImplant);
				if (vacuumResistanceImplantHediff != null)
				{
					stage.statOffsets ??= new List<StatModifier>();
					var modifier = stage.statOffsets.FirstOrDefault(sm => sm.stat == StatDefOf.VacuumResistance);
					if (modifier is null)
					{
						modifier = new StatModifier
						{
							stat = StatDefOf.VacuumResistance,
						};
						stage.statOffsets.Add(modifier);
					}
					modifier.value = 1f;
				}
			}
			pawn.health.hediffSet.CacheNeeds();
			pawn.needs?.AddOrRemoveNeedsAsAppropriate();
		}

		private float curMasterPsychicSensitivity;
		public override void Tick()
		{
			base.Tick();
			if (masterHediff != null && pawn.IsHashIntervalTick(60))
			{
				var masterPsychicSensitivity = masterHediff.pawn.GetStatValue(StatDefOf.PsychicSensitivity);
				if (masterPsychicSensitivity != curMasterPsychicSensitivity)
				{
					curMasterPsychicSensitivity = masterPsychicSensitivity;
					SetupStage();
				}
			}
		}

		public bool Controllable
		{
			get
			{
				if (masterHediff?.pawn is null || pawn.Tile != masterHediff.pawn.Tile)
				{
					return false;
				}
				if (masterHediff.pawn.Downed)
				{
					return false;
				}
				return ControllableNoTileAndDownedCheck;
			}
		}

		public bool ControllableNoTileAndDownedCheck
		{
			get
			{
				if (pawn.kindDef == DefsOf.DE_Burrower)
				{
					return false;
				}
				if (masterHediff?.pawn is null)
				{
					return false;
				}
				if (masterHediff.pawn.Dead || masterHediff.pawn.Destroyed)
				{
					return false;
				}
				if (masterHediff.pawn.MentalState != null)
				{
					return false;
				}
				return pawn.Faction == Faction.OfPlayer && pawn.MentalStateDef == null;
			}
		}


		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			RemoveHediffsImmuneTo();
			AssignComponents();
			foreach (var skill in pawn.skills.skills)
			{
				skill.Level = 10;
			}
		}

		public void AssignComponents()
		{
			if (pawn.natives != null)
			{
				pawn.natives.cachedVerbProperties = null;
			}
			pawn.needs.AddOrRemoveNeedsAsAppropriate();
			if (pawn.skills is null)
			{
				pawn.skills = new Pawn_SkillTracker(pawn);
				pawn.story ??= new Pawn_StoryTracker(pawn);
			}
			pawn.playerSettings ??= new Pawn_PlayerSettings(pawn);
			pawn.drafter ??= new Pawn_DraftController(pawn);
			pawn.equipment ??= new Pawn_EquipmentTracker(pawn);
			pawn.abilities ??= new Pawn_AbilityTracker(pawn);
		}

		private void RemoveHediffsImmuneTo()
		{
			List<Hediff> hediffsToRemove = new List<Hediff>();

			foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
			{
				if (IsImmuneTo(hediff))
				{
					hediffsToRemove.Add(hediff);
				}
			}

			foreach (Hediff hediff in hediffsToRemove)
			{
				pawn.health.RemoveHediff(hediff);
			}
		}
		public bool IsImmuneTo(Hediff hediff)
		{
			if (hediff.def == HediffDefOf.LungRotExposure || hediff.def == HediffDefOf.LungRot
			|| hediff.def == HediffDefOf.BloodLoss || hediff.def == HediffDefOf.ToxGasExposure)
			{
				return true;
			}
			var extension = hediff.def.GetModExtension<ServantSettingsExtension>();
			if (extension != null && extension.isDisease)
			{
				return true;
			}
			return false;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (var gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}

			if (ControllableNoTileAndDownedCheck && pawn.needs.food != null)
			{
				yield return new Command_Action
				{
					defaultLabel = "DE_SeekFood".Translate(),
					defaultDesc = "DE_SeekFoodDesc".Translate(),
					icon = ContentFinder<Texture2D>.Get("UI/Abilities/Slaughter"),
					action = delegate
					{
						var jobGiver = new JobGiver_GetFood();
						jobGiver.forceScanWholeMap = true;
						jobGiver.minCategory = HungerCategory.Fed;
						jobGiver.maxLevelPercentage = 999f;
						var job = jobGiver.TryGiveJob(pawn);
						if (job != null)
						{
							pawn.jobs.TryTakeOrderedJob(job);
						}
					}
				};
			}
		}
	}

	public class Hediff_ServantBurrower : Hediff_ServantType
	{
		public override ServantType ServantType => ServantType.Burrower;
	}

	public class Hediff_ServantSmall : Hediff_ServantType
	{
		public override ServantType ServantType => ServantType.Small;
		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			pawn.health.AddHediff(DefsOf.DE_FungalTentacle);
		}
	}

	public class Hediff_ServantMedium : Hediff_ServantType
	{
		public override ServantType ServantType => ServantType.Medium;
		public override void PostAdd(DamageInfo? dinfo)
		{
			pawn.abilities ??= new Pawn_AbilityTracker(pawn);
			base.PostAdd(dinfo);
			pawn.health.AddHediff(DefsOf.DE_BladedLimb);
			pawn.health.AddHediff(DefsOf.DE_BladedLimb);
			pawn.equipment.AddEquipment(ThingMaker.MakeThing(DefsOf.DE_Gun_SpikeThrower) as ThingWithComps);
		}

		public override void Tick()
		{
			base.Tick();
			if (Find.TickManager.TicksGame % 60 == 0)
			{
				if (pawn.equipment.Primary == null)
				{
					pawn.equipment.AddEquipment(ThingMaker.MakeThing(DefsOf.DE_Gun_SpikeThrower) as ThingWithComps);
				}
			}
		}
	}

	public class Hediff_ServantLarge : Hediff_ServantType
	{
		public override ServantType ServantType => ServantType.Large;
	}

	public class Hediff_ServantGhoul : Hediff_ServantType
	{
		public override ServantType ServantType => ServantType.Ghoul;
		public bool specialized;
		public SkillDef specializedSkill;
		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			pawn.mutant = new Pawn_MutantTracker(pawn, DefsOf.DE_FungalGhoul, RotStage.Fresh);
			pawn.mutant.Turn(clearLord: true);
			var attachment = Startup.bodyAttachments.RandomElement();
			pawn.health.AddHediff(attachment);
		}

		public void Specialize()
		{
			pawn.mutant = new Pawn_MutantTracker(pawn, DefsOf.DE_FungalGhoulSpecialized, RotStage.Fresh);
			pawn.mutant.Turn(clearLord: true);
			var regeneration = pawn.health.hediffSet.GetFirstHediffOfDef(DefsOf.Regeneration);
			if (regeneration != null)
			{
				pawn.health.RemoveHediff(regeneration);
			}
			var skills = new List<SkillDef>
			{
				SkillDefOf.Animals, SkillDefOf.Plants, SkillDefOf.Crafting, SkillDefOf.Medicine, SkillDefOf.Social, SkillDefOf.Intellectual
			};
			Find.WindowStack.Add(new Window_SkillsToSpecialize(skills, delegate (SkillDef x)
			{
				Specialize(x);
			}));
		}

		public void Specialize(SkillDef skill)
		{
			specialized = true;
			specializedSkill = skill;
			pawn.Notify_DisabledWorkTypesChanged();
			pawn.skills = new Pawn_SkillTracker(pawn);
			pawn.skills.GetSkill(skill).Level = 10;
			pawn.workSettings = new Pawn_WorkSettings(pawn);
			pawn.workSettings.EnableAndInitialize();
			if (skill == SkillDefOf.Social)
			{
				var ghoul = pawn.health.hediffSet.GetFirstHediffOfDef(DefsOf.DE_FungalGhoul.hediff);
				if (ghoul != null)
				{
					pawn.health.RemoveHediff(ghoul);
				}
				pawn.health.AddHediff(DefsOf.DE_FungalGhoulTalkable);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref specialized, "specialized");
			Scribe_Defs.Look(ref specializedSkill, "specializedSkill");
		}
	}

	public class Hediff_ServantSpecial : Hediff_ServantType
	{
		public override ServantType ServantType => ServantType.Special;
	}

	[HotSwappable]
	public class Hediff_UnstableServant : Hediff_ServantType
	{
		public override ServantType ServantType => ServantType.Unstable;

		public override void PostAdd(DamageInfo? dinfo)
		{
			pawn.abilities ??= new Pawn_AbilityTracker(pawn);
			base.PostAdd(dinfo);
		}
		public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
			base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
			pawn.GetComp<CompRevenant>().Invisibility.BecomeVisible();
			pawn.GetComp<CompRevenant>().becomeInvisibleTick = int.MaxValue;
		}
	}
}
