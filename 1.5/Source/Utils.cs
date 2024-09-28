using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	public static class DanceOfEvolution
	{
		public static bool IsImmuneTo(this Pawn pawn, Hediff other)
		{
			if (pawn.IsFungalNexus(out Hediff_FungalNexus fungalNexus))
			{
				return fungalNexus.IsImmuneTo(other);
			}
			return false;
		}

		public static bool HasFungalNexus(this Pawn pawn)
		{
			return pawn.IsFungalNexus(out _);
		}

		public static bool IsInfected(this Corpse corpse)
		{
			return GameComponent_ReanimateCorpses.Instance.infectedCorpses.Exists(ic => ic.corpse == corpse);
		}
	
		public static bool IsFungalNexus(this Pawn pawn, out Hediff_FungalNexus fungalNexus)
		{
			fungalNexus = pawn.health.hediffSet.GetFirstHediff<Hediff_FungalNexus>();
			return fungalNexus != null;
		}

		public static bool IsServant(this Pawn pawn)
		{
			return pawn.IsServant(out _);
		}

		public static bool IsServant(this Pawn pawn, out Hediff_ServantType hediff)
		{
			hediff = pawn.GetServantTypeHediff();
			return hediff != null;
		}
		
		public static Hediff_ServantType GetServantTypeHediff(this Pawn pawn)
		{
			return pawn.health.hediffSet.GetFirstHediff<Hediff_ServantType>();
		}
		
		public static void MakeServant(this Pawn pawn, Hediff_FungalNexus masterHediff, HediffDef servantHediff)
		{
			var hediff = pawn.health.AddHediff(servantHediff) as Hediff_ServantType;
			hediff.masterHediff = masterHediff;
			masterHediff.servants.Add(pawn);
		}
	}
}
