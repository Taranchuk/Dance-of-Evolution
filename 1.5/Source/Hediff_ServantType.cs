using System.Collections.Generic;
using RimWorld;
using Verse;
namespace DanceOfEvolution
{
	public enum ServantType
	{
		Burrower,
		Small,
		Medium,
		Large,
		Ghoul,
		Strange
	}
	public abstract class Hediff_ServantType : HediffWithComps
	{
		public abstract ServantType ServantType { get; }
		public Hediff_FungalNexus masterHediff;
		private HediffStage stage;
		public override HediffStage CurStage => stage;
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
			var growthStimulator = masterHediff.pawn.health.hediffSet.GetFirstHediffOfDef(DefsOf.DE_GrowthStimulatorImplant) as Hediff_Level;
			if (growthStimulator != null)
			{
				stage.regeneration += 10 * growthStimulator.level;
				stage.showRegenerationStat = true;
			}
		}

		public bool Controllable
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
				if (pawn.MapHeld != masterHediff.pawn?.MapHeld)
				{
					return false;
				}
				if (masterHediff.pawn.Downed || masterHediff.pawn.Dead || masterHediff.pawn.Destroyed)
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
			pawn.natives.cachedVerbProperties = null;
			pawn.needs.AddOrRemoveNeedsAsAppropriate();
			if (pawn.skills is null)
			{
				pawn.skills = new Pawn_SkillTracker(pawn);
				pawn.story ??= new Pawn_StoryTracker(pawn);
			}
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
			if (hediff.def == HediffDefOf.LungRotExposure || hediff.def == HediffDefOf.LungRot || hediff.def == HediffDefOf.BloodLoss)
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
	}

	public class Hediff_ServantLarge : Hediff_ServantType
	{
		public override ServantType ServantType => ServantType.Large;
	}

	public class Hediff_ServantGhoul : Hediff_ServantType
	{
		public override ServantType ServantType => ServantType.Ghoul;

		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			pawn.mutant = new Pawn_MutantTracker(pawn, DefsOf.DE_FungalGhoul, RotStage.Fresh);
			pawn.mutant.Turn(clearLord: true);
		}
	}

	public class Hediff_ServantStrange : Hediff_ServantType
	{
		public override ServantType ServantType => ServantType.Strange;
		public override void PostAdd(DamageInfo? dinfo)
		{
			pawn.abilities ??= new Pawn_AbilityTracker(pawn);
			base.PostAdd(dinfo);
			pawn.health.AddHediff(DefsOf.DE_PiercingLimb);
			pawn.health.AddHediff(DefsOf.DE_PiercingLimb);
			if (pawn.Name is null)
			{
				pawn.Name = new NameSingle(pawn.Label);
			}
		}
	}
}