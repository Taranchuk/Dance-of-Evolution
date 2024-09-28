using System.Collections.Generic;
using RimWorld;
using Verse;
using System.Linq;
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
		public override bool ShouldRemove => masterHediff is null || masterHediff.pawn is null
			|| masterHediff.pawn.health.hediffSet.hediffs.Contains(masterHediff) is false;
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref masterHediff, "master");
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