using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(GameCondition_UnnaturalDarkness), "InUnnaturalDarkness")]
	public static class GameCondition_UnnaturalDarkness_InUnnaturalDarkness_Patch
	{
		public static bool Prefix(Pawn p)
		{
			if (p.Spawned && p.Map.gameConditionManager.ConditionIsActive(DefsOf.DE_CloudmakerCondition))
			{
				return false;
			}
			return true;
		}
	}
}
