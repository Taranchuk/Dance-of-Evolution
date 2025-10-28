using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(PawnGroupKindWorker_Normal), "GeneratePawns")]
	public static class PawnGroupKindWorker_Normal_GeneratePawns_Patch
	{
		public static bool Prefix(PawnGroupMakerParms parms, PawnGroupMaker groupMaker, List<Pawn> outPawns, bool errorOnZeroResults = true)
		{
			if (parms.faction?.def != DefsOf.DE_Mycelyss)
			{
				return true;
			}
			if (groupMaker.kindDef == PawnGroupKindDefOf.Combat)
			{
				GenerateAnimals(parms, outPawns, parms.points);
			}
			else if (groupMaker.kindDef == PawnGroupKindDefOf.Settlement)
			{
				GenerateSettlementGroup(parms, outPawns);
			}
			return false;
		}

		private static void GenerateSettlementGroup(PawnGroupMakerParms parms, List<Pawn> outPawns)
		{
			if (parms.points >= DefsOf.DE_MycelyssFungalNexus.combatPower)
			{
				var request = new PawnGenerationRequest(
					DefsOf.DE_MycelyssFungalNexus,
					parms.faction,
					PawnGenerationContext.NonPlayer,
					parms.tile,
					forceGenerateNewPawn: false,
					allowDead: false,
					allowDowned: true,
					canGeneratePawnRelations: true,
					mustBeCapableOfViolence: true,
					colonistRelationChanceFactor: 1f,
					forceAddFreeWarmLayerIfNeeded: false,
					allowGay: true,
					allowPregnant: true,
					allowFood: true,
					allowAddictions: true,
					inhabitant: parms.inhabitants,
					certainlyBeenInCryptosleep: false,
					forceRedressWorldPawnIfFormerColonist: false,
					worldPawnFactionDoesntMatter: false,
					biocodeWeaponChance: 0f,
					biocodeApparelChance: 0f
				);

				var nexusPawn = PawnGenerator.GeneratePawn(request);
				var fungalNexusHediff = HediffMaker.MakeHediff(DefsOf.DE_FungalNexus, nexusPawn) as Hediff_FungalNexus;
				nexusPawn.health.AddHediff(fungalNexusHediff);
				outPawns.Add(nexusPawn);
				GenerateAnimals(parms, outPawns, parms.points - DefsOf.DE_MycelyssFungalNexus.combatPower, masterHediff: fungalNexusHediff);
			}
			else
			{
				GenerateAnimals(parms, outPawns, parms.points, masterHediff: null);
			}
		}

		private static void GenerateAnimals(PawnGroupMakerParms parms, List<Pawn> outPawns, float startingPoints, Hediff_FungalNexus masterHediff = null)
		{
			var animalOptions = DefDatabase<PawnKindDef>.AllDefs
				.Where(pk => pk.race?.race?.Animal == true && pk.combatPower > 0)
				.ToList();

			float remainingPoints = startingPoints;

			while (remainingPoints > 0)
			{
				var availableAnimals = animalOptions
					.Where(pk => pk.combatPower <= remainingPoints)
					.ToList();

				if (!availableAnimals.Any())
				{
					break;
				}
				var selectedKind = availableAnimals.RandomElement();
				var request = new PawnGenerationRequest(
					selectedKind,
					parms.faction,
					PawnGenerationContext.NonPlayer,
					parms.tile
				);

				var pawn = PawnGenerator.GeneratePawn(request);
				var result = Utils.TryGetServantTypeAndHediff(pawn);
				if (result.HasValue)
				{
					var hediff = HediffMaker.MakeHediff(result.Value.servantHediffDef, pawn) as Hediff_ServantType;
					pawn.health.AddHediff(hediff);
					if (masterHediff != null)
					{
						hediff.masterHediff = null;
					}
				}
				outPawns.Add(pawn);
				remainingPoints -= selectedKind.combatPower;
			}
		}
	}
}
