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
		public override bool ShouldRemove => false;
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref masterHediff, "master");
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
			if (pawn.skills is null)
			{
				pawn.skills = new Pawn_SkillTracker(pawn);
				pawn.story ??= new Pawn_StoryTracker(pawn);
			}
			foreach (var skill in pawn.skills.skills)
			{
				skill.Level = 10;
			}
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
			pawn.health.AddHediff(DefsOf.DE_SpikeThrower);
			pawn.equipment ??= new Pawn_EquipmentTracker(pawn);
		}
	}

	public class Hediff_ServantLarge : Hediff_ServantType
	{
		public override ServantType ServantType => ServantType.Large;
	}

	public class Hediff_ServantGhoul : Hediff_ServantType
	{
		public override ServantType ServantType => ServantType.Ghoul;
	}

	public class Hediff_ServantStrange : Hediff_ServantType
	{
		public override ServantType ServantType => ServantType.Strange;
	}
}