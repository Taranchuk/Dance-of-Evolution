using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(Pawn), "Destroy")]
	public static class Pawn_Destroy_Patch
	{
		private static void Prefix(Pawn __instance)
		{
			if (__instance.IsServant(out var hediff))
			{
				hediff.masterHediff?.servants.Remove(__instance);
			}
		}
	}
}