using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.AI.Group;

namespace DanceOfEvolution
{
	[HarmonyPatch]
	public static class Building_FungoidShip_Tick_Patch
	{
		public static bool Prepare()
		{
			return ModsConfig.IsActive("vanillaracesexpanded.fungoid");
		}

		[HarmonyTargetMethod]
		public static MethodBase TargetMethod()
		{
			var buildingType = AccessTools.TypeByName("VanillaRacesExpandedFungoid.Building_FungoidShip");
			if (buildingType == null) return null;
			return AccessTools.Method(buildingType, "Tick");
		}


		public static void Postfix(object __instance)
		{
			if (__instance is Building building && building.Spawned && building.IsHashIntervalTick(300))
			{
				Building_FungalNode.SpreadRottenSoil(building.Map, building.Position);
			}
		}
	}

	[HarmonyPatch]
	public static class VRE_Fungoid_Vomit_Immunity_Patch
	{
		public static bool Prepare()
		{
			return ModsConfig.IsActive("vanillaracesexpanded.fungoid");
		}

		[HarmonyTargetMethod]
		public static MethodBase TargetMethod()
		{
			var patchType = AccessTools.TypeByName("VanillaRacesExpandedFungoid.VanillaRacesExpandedFungoid_InteractionWorker_Interacted_Patch");
			if (patchType == null) return null;
			return AccessTools.Method(patchType, "VomitIfFungoid", new[] { typeof(Pawn), typeof(Pawn) });
		}

		public static bool Prefix(Pawn initiator, Pawn recipient)
		{
			if (initiator.IsFungalNexus() ||
				recipient.IsFungalNexus())
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch]
	public static class Building_FungoidShip_PopUpFungoids_Patch
	{
		public static bool Prepare()
		{
			return ModsConfig.IsActive("vanillaracesexpanded.fungoid");
		}

		[HarmonyTargetMethod]
		public static MethodBase TargetMethod()
		{
			var buildingType = AccessTools.TypeByName("VanillaRacesExpandedFungoid.Building_FungoidShip");
			if (buildingType == null) return null;
			return AccessTools.Method(buildingType, "PopUpFungoids");
		}

		public static void Postfix(Building __instance)
		{
			var map = __instance.Map;
			var internalDefOf = AccessTools.TypeByName("VanillaRacesExpandedFungoid.InternalDefOf");
			var ancientFungoidKind = (PawnKindDef)AccessTools.Field(internalDefOf, "VRE_AncientFungoid").GetValue(null);
			var fungoidXenotype = (XenotypeDef)AccessTools.Field(internalDefOf, "VRE_Fungoid").GetValue(null);

			PawnGenerationRequest request = new PawnGenerationRequest(
				kind: ancientFungoidKind,
				faction: Faction.OfAncientsHostile,
				context: PawnGenerationContext.NonPlayer,
				tile: -1,
				forceGenerateNewPawn: false,
				allowDead: false,
				allowDowned: false,
				canGeneratePawnRelations: false,
				mustBeCapableOfViolence: true,
				colonistRelationChanceFactor: 0f,
				forceAddFreeWarmLayerIfNeeded: false,
				allowGay: true,
				allowPregnant: false,
				allowFood: true,
				allowAddictions: false,
				inhabitant: false,
				certainlyBeenInCryptosleep: false,
				forceRedressWorldPawnIfFormerColonist: false,
				worldPawnFactionDoesntMatter: false,
				biocodeWeaponChance: 0f,
				biocodeApparelChance: 1f,
				extraPawnForExtraRelationChance: null,
				relationWithExtraPawnChanceFactor: 0f,
				validatorPreGear: null,
				validatorPostGear: null,
				forcedTraits: null,
				prohibitedTraits: null,
				minChanceToRedressWorldPawn: null,
				fixedGender: null,
				fixedBiologicalAge: 40,
				fixedChronologicalAge: 1000,
				fixedLastName: null,
				fixedBirthName: null,
				fixedTitle: null,
				fixedIdeo: null,
				forceNoIdeo: false,
				forceNoBackstory: false,
				forbidAnyTitle: false,
				forceDead: false,
				forcedXenogenes: null,
				forcedEndogenes: null,
				forcedXenotype: fungoidXenotype,
				forcedCustomXenotype: null,
				allowedXenotypes: null,
				forceBaselinerChance: 0f,
				developmentalStages: DevelopmentalStage.Adult
			);
			Pawn nexus = PawnGenerator.GeneratePawn(request);
			nexus.health.AddHediff(DefsOf.DE_FungalNexus);
			GenSpawn.Spawn(nexus, CellFinder.RandomClosewalkCellNear(__instance.Position, __instance.Map, 4), __instance.Map);
			var hediff = nexus.health.hediffSet.GetFirstHediffOfDef(DefsOf.DE_FungalNexus) as Hediff_FungalNexus;
			var pawns = new List<Pawn>();
			pawns.Add(nexus);
			for (int i = 0; i < 2; i++)
			{
				var pawn = PawnGenerator.GeneratePawn(DefsOf.Noctol, Faction.OfAncientsHostile);
				pawn.MakeServant(hediff);
				GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(__instance.Position, __instance.Map, 4), __instance.Map);
				pawns.Add(pawn);
			}
			for (int i = 0; i < 2; i++)
			{
				var pawn = PawnGenerator.GeneratePawn(DefsOf.Toughspike, Faction.OfAncientsHostile);
				pawn.MakeServant(hediff);
				GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(__instance.Position, __instance.Map, 4), __instance.Map);
				pawns.Add(pawn);
			}
			var lordJob = new LordJob_AssaultColony(Faction.OfAncientsHostile, canKidnap: false, canTimeoutOrFlee: true, sappers: false, useAvoidGridSmart: false, canSteal: false);
			LordMaker.MakeNewLord(Faction.OfAncientsHostile, lordJob, __instance.Map, pawns);
		}
	}
}
