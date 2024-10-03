using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(HediffComp_GiveHediffLungRot), "CompPostTick")]
	public static class HediffComp_GiveHediffLungRot_CompPostTick_Transpiler
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var anyGasMethod = AccessTools.Method(typeof(GasUtility), "AnyGas", new[] { typeof(IntVec3), typeof(Map), typeof(GasType) });
			var interceptAnyGasMethod = AccessTools.Method(typeof(HediffComp_GiveHediffLungRot_CompPostTick_Transpiler), "InterceptAnyGas");

			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.Calls(anyGasMethod))
				{
					yield return new CodeInstruction(OpCodes.Ldloc_0);
					yield return new CodeInstruction(OpCodes.Call, interceptAnyGasMethod);
				}
			}
		}
		
		public static bool InterceptAnyGas(bool result, Pawn pawn)
		{
			if (!result && pawn.Spawned && pawn.Position.GetTerrain(pawn.Map) == DefsOf.DE_RottenSoil)
			{
				return true;
			}
			return result;
		}
	}

}
