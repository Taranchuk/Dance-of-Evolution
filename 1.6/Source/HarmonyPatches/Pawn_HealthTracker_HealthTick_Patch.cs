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
			if (__instance.pawn.Spawned && __instance.pawn.IsHashIntervalTick(LungRotExposureTickRate))
			{
				var terrain = __instance.pawn.Position.GetTerrain(__instance.pawn.Map);
				if (terrain == DefsOf.DE_RottenSoil &&
					!__instance.pawn.health.hediffSet.HasHediff(DefsOf.FleshmassLung) &&
					!__instance.pawn.health.hediffSet.HasHediff(HediffDefOf.DetoxifierLung))
				{
					if (__instance.pawn.genes is not null)
					{
						var genes = __instance.pawn.genes.GenesListForReading.Where(x => x.Active && x.def.makeImmuneTo is not null && x.def.makeImmuneTo.Contains(HediffDefOf.LungRot));
						if (genes.Any())
						{
							return;
						}
					}
					HealthUtility.AdjustSeverity(__instance.pawn, HediffDefOf.LungRotExposure, 0.001f);
				}
			}
		}
	}
}
