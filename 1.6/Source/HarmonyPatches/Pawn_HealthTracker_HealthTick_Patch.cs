using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;
namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(Pawn_HealthTracker), "HealthTick")]
	public static class Pawn_HealthTracker_HealthTick_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var helperMethod = AccessTools.Method(typeof(Pawn_HealthTracker_HealthTick_Patch), nameof(ModifyFlagValue));

			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.opcode == OpCodes.Ldloc_S && ((LocalBuilder)instruction.operand).LocalIndex == 6)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Pawn_HealthTracker), "pawn"));
					yield return new CodeInstruction(OpCodes.Call, helperMethod);
				}
			}
		}

		public static bool ModifyFlagValue(bool originalValue, Pawn pawn)
		{
			if (pawn.IsServant())
			{
				return false;
			}
			return originalValue;
		}

		public const int LungRotExposureTickRate = 60;
		public static void Postfix(Pawn_HealthTracker __instance)
		{
			if (!__instance.pawn.Spawned || !__instance.pawn.IsHashIntervalTick(LungRotExposureTickRate))
			{
				return;
			}

			var pawn = __instance.pawn;
			var terrain = pawn.Position.GetTerrain(pawn.Map);

			float exposureSeverity = GetLungRotExposureSeverity(terrain, pawn);
			if (exposureSeverity > 0f && ShouldApplyLungRotExposure(pawn))
			{
				HealthUtility.AdjustSeverity(pawn, HediffDefOf.LungRotExposure, exposureSeverity);
			}
		}

		private static float GetLungRotExposureSeverity(TerrainDef terrain, Pawn pawn)
		{
			if (terrain == DefsOf.DE_RottenSoil)
			{
				return 0.001f;
			}

			if (terrain == DefsOf.DE_MyceliumFerrite && pawn.HostileTo(Faction.OfPlayer))
			{
				return 0.002f;
			}

			return 0f;
		}

		private static bool ShouldApplyLungRotExposure(Pawn pawn)
		{
			if (pawn.health.hediffSet.HasHediff(DefsOf.FleshmassLung) ||
				pawn.health.hediffSet.HasHediff(HediffDefOf.DetoxifierLung))
			{
				return false;
			}
			if (pawn.genes != null)
			{
				return !pawn.genes.GenesListForReading.Any(x =>
					x.Active &&
					x.def.makeImmuneTo != null &&
					x.def.makeImmuneTo.Contains(HediffDefOf.LungRot));
			}

			return true;
		}
	}
}
