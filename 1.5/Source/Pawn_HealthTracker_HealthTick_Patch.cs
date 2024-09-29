using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
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
	}
}