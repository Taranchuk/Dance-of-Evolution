using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(RimWorld.WorkGiver_GrowerSow), "JobOnCell")]
	public static class WorkGiver_GrowerSow_JobOnCell_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var roofedMethod = typeof(Verse.GridsUtility).GetMethod("Roofed", new[] { typeof(Verse.IntVec3), typeof(Verse.Map) });
			bool added = false;
			var codes = instructions.ToList();
			for (int i = 0; i < codes.Count; i++)
			{
				yield return codes[i];
				if (added is false && codes[i].Calls(roofedMethod))
				{
					yield return new CodeInstruction(OpCodes.Ldloc_0);
					yield return new CodeInstruction(OpCodes.Call,
					AccessTools.Method(typeof(WorkGiver_GrowerSow_JobOnCell_Patch),
					nameof(HasCloudmakerCondition)));
					added = true;
				}
			}
		}

		private static bool HasCloudmakerCondition(bool roofed, Map map)
		{
			if (map.gameConditionManager.ConditionIsActive(DefsOf.DE_CloudmakerCondition))
			{
				return true;
			}
			return roofed;
		}
	}
}