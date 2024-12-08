using HarmonyLib;
using RimWorld;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(Pawn_GeneTracker), nameof(Pawn_GeneTracker.AffectedByDarkness), MethodType.Getter)]
	public static class Pawn_GeneTracker_AffectedByDarkness_Patch
	{
		public static bool Prefix(Pawn_GeneTracker __instance)
		{
			if (__instance.pawn.HasFungalNexus())
			{
				return false;
			}
			return true;
		}
	}
}
