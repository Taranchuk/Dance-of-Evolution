using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch]
	public static class Dialog_FormCaravan_CheckForErrors_Patch
	{
		public static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(Dialog_FormCaravan), nameof(Dialog_FormCaravan.CheckForErrors));
			yield return AccessTools.Method(typeof(Dialog_SplitCaravan), nameof(Dialog_SplitCaravan.CheckForErrors));
		}

		public static void Postfix(ref bool __result, List<Pawn> pawns)
		{
			if (__result && pawns.Any(x => x.IsServant(out var servantType) && pawns.Contains(servantType.masterHediff.pawn) is false))
			{
				__result = false;
				Messages.Message("DE_ServantsMustHaveNexus".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			}
		}
	}
}
