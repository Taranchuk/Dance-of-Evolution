using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using System.Reflection.Emit;
namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(FoodUtility), "BestFoodSourceOnMap")]
	public static class FoodUtility_BestFoodSourceOnMap_Transpiler
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var helperMethod = AccessTools.Method(typeof(FoodUtility_BestFoodSourceOnMap_Transpiler), nameof(InterceptPathEndMode));
			CodeInstruction previousInstruction = null;

			foreach (var code in instructions)
			{
				yield return code;
				if (previousInstruction != null && previousInstruction.opcode == OpCodes.Ldloc_1 && code.opcode == OpCodes.Ldc_I4_1)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_1); // Load the 'eater' argument
					yield return new CodeInstruction(OpCodes.Call, helperMethod); // Call the helper method
				}
				previousInstruction = code;
			}
		}

		private static PathEndMode InterceptPathEndMode(PathEndMode endMode, Pawn eater)
		{
			if (eater.IsServant())
			{
				return PathEndMode.ClosestTouch;
			}
			return endMode;
		}
	}
}
