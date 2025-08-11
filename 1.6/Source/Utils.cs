using System.Collections.Generic;
using RimWorld;
using Verse;
using System.Linq;
using System.Reflection;
using HarmonyLib;

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
		public static bool IsFungalNexus(this Pawn pawn)
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
			return pawn.IsServant(out var hediff) && hediff.Controllable;
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
			return GetBaseServantType(pawn);
		}

		public static (ServantType servantType, HediffDef servantHediffDef)? GetBaseServantType(this Pawn pawn)
		{
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
		
		public static bool CanSpawnOnRottenSoil(this ThingDef def)
		{
			return def.plant != null && (def.plant.cavePlant || WildPlantSpawner_GetCommonalityOfPlant_Patch.commonalities.ContainsKey(def.defName));
		}
		
		public static T Clone<T>(this T obj)
		{
			var inst = obj.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
			return (T)inst?.Invoke(obj, null);
		}
	}


	[HarmonyPatch(typeof(Thing), nameof(Thing.Destroy))]
	public static class Thing_Destroy_Patch
	{
		public static void Postfix(Thing __instance)
		{
			if (__instance.def.saveCompressible || __instance is Filth)
				return;
			Log.Message("Destroying thing: " + __instance);
		}
	}

	//[HarmonyPatch(typeof(Thing), nameof(Thing.Position), MethodType.Setter)]
	//public static class Thing_Position_Patch
	//{
	//    public static void Prefix(Thing __instance, out IntVec3 __state)
	//    {
	//        __state = __instance.Position;
	//    }
	//    public static void Postfix(Thing __instance, IntVec3 __state)
	//    {
	//        if (__instance.Position != __state)
	//        {
	//            Log.Message(__instance + " changing position to " + __instance.Position);
	//        }
	//    }
	//}

	[HarmonyPatch(typeof(Pawn), nameof(Pawn.Kill))]
	public static class Pawn_Kill_Patch2
	{
		public static void Postfix(Pawn __instance)
		{
			Log.Message("Killing pawn: " + __instance);
		}
	}
}
