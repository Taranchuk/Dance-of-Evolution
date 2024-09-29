using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(Pawn_NeedsTracker), "ShouldHaveNeed")]
	public static class Pawn_NeedsTracker_ShouldHaveNeed_Patch
	{
		[HarmonyPriority(int.MinValue)]
		public static void Postfix(Pawn ___pawn, NeedDef nd, ref bool __result)
		{
			if (__result)
			{
				if (___pawn.kindDef == DefsOf.DE_Burrower)
				{
					__result = false;
				}
			}

		}
	}
}