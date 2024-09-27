using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	public static class DanceOfEvolution
	{
		public static bool IsImmuneTo(this Pawn pawn, Hediff other)
		{
			if (pawn.HasFungalNexus(out Hediff_FungalNexus fungalNexus))
			{
				return fungalNexus.IsImmuneTo(other);
			}
			return false;
		}

		public static bool HasFungalNexus(this Pawn pawn)
		{
			return pawn.HasFungalNexus(out _);
		}

		public static bool IsInfected(this Corpse corpse)
		{
			return GameComponent_ReanimateCorpses.Instance.infectedCorpses.Exists(ic => ic.corpse == corpse);
		}
	
		public static bool HasFungalNexus(this Pawn pawn, out Hediff_FungalNexus fungalNexus)
		{
			fungalNexus = pawn.health.hediffSet.GetFirstHediff<Hediff_FungalNexus>();
			return fungalNexus != null;
		}
	}
}
