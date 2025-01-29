using System.Collections.Generic;
using RimWorld;
using Verse;
using System.Linq;
using System.Reflection;

namespace DanceOfEvolution
{
	public static class Utils
	{
		public static bool IsImmuneTo(this Pawn pawn, Hediff other)
		{
			if (pawn.IsFungalNexus(out Hediff_FungalNexus fungalNexus))
			{
				return fungalNexus.IsImmuneTo(other);
			}
			else if (pawn.IsServant(out var servantType))
			{
				return servantType.IsImmuneTo(other);
			}
			return false;
		}
		public static Hediff_FungalNexus GetFungalNexus(this Pawn pawn)
		{
			return pawn.health.hediffSet.GetFirstHediff<Hediff_FungalNexus>();
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
			fungalNexus = pawn.GetFungalNexus();
			return fungalNexus != null;
		}

		public static bool IsServant(this Pawn pawn)
		{
			return pawn.IsServant(out _);
		}

		public static bool IsServantEntity(this Pawn pawn)
		{
			return pawn.IsServant() && pawn.RaceProps.IsAnomalyEntity;
		}
		public static bool IsControllableServant(this Pawn pawn)
		{
			return pawn.IsControllableServant(out _);
		}

		public static bool IsControllableServantNoTileAndDownedCheck(this Pawn pawn)
		{
			return pawn.IsServant(out var hediff) && hediff.ControllableNoTileAndDownedCheck;
		}
		public static bool IsControllableServant(this Pawn pawn, out Hediff_ServantType hediff)
		{
			return pawn.IsServant(out hediff) && hediff.Controllable;
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

		public static void MakeServant(this Pawn pawn, Hediff_FungalNexus masterHediff)
		{
			MakeServant(pawn, masterHediff, TryGetServantTypeAndHediff(pawn).Value.servantHediffDef);
		}
		public static void MakeServant(this Pawn pawn, Hediff_FungalNexus masterHediff, HediffDef servantHediff)
		{
			if (pawn.Faction != masterHediff.pawn.Faction)
			{
				pawn.SetFaction(masterHediff.pawn.Faction);
			}
			var hediff = HediffMaker.MakeHediff(servantHediff, pawn) as Hediff_ServantType;
			hediff.masterHediff = masterHediff;
			pawn.health.AddHediff(hediff);
			masterHediff.servants.Add(pawn);
		}

		public static (ServantType servantType, HediffDef servantHediffDef)? TryGetServantTypeAndHediff(this Pawn pawn)
		{
			if (pawn.RaceProps.Humanlike)
			{
				return (ServantType.Ghoul, DefsOf.DE_ServantGhoul);
			}

			float bodySize = pawn.BodySize;
			if (bodySize <= 0.99f)
			{
				return (ServantType.Small, DefsOf.DE_ServantSmall);
			}
			else if (bodySize >= 1f && bodySize < 2.11f)
			{
				return (ServantType.Medium, DefsOf.DE_ServantMedium);
			}
			else if (bodySize >= 2.11f)
			{
				return (ServantType.Large, DefsOf.DE_ServantLarge);
			}
			return null;
		}

		public static bool TryGiveMutation(this Pawn pawn, HediffDef mutationDef)
		{
			if (mutationDef.defaultInstallPart == null)
			{
				return false;
			}
			List<BodyPartRecord> list = (from part in pawn.RaceProps.body.GetPartsWithDef(mutationDef.defaultInstallPart)
										 where pawn.health.hediffSet.HasMissingPartFor(part)
										 select part).ToList();
			List<BodyPartRecord> list2 = (from part in pawn.RaceProps.body.GetPartsWithDef(mutationDef.defaultInstallPart)
										  where !pawn.health.hediffSet.HasDirectlyAddedPartFor(part)
										  select part).ToList();
			BodyPartRecord bodyPartRecord = null;
			if (list.Any())
			{
				bodyPartRecord = list.RandomElement();
			}
			else if (list2.Any())
			{
				bodyPartRecord = list2.RandomElement();
			}
			if (bodyPartRecord == null)
			{
				return false;
			}
			MedicalRecipesUtility.SpawnThingsFromHediffs(pawn, bodyPartRecord, pawn.PositionHeld, pawn.MapHeld);
			pawn.health.AddHediff(mutationDef, bodyPartRecord);
			return true;
		}

		public static T Clone<T>(this T obj)
		{
			var inst = obj.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
			return (T)inst?.Invoke(obj, null);
		}
	}
}
