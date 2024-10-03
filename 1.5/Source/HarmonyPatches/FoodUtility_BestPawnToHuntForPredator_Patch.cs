using DanceOfEvolution;
using HarmonyLib;
using RimWorld;
using Verse;

[HarmonyPatch(typeof(FoodUtility), "BestPawnToHuntForPredator")]
public static class FoodUtility_BestPawnToHuntForPredator_Patch
{
	public static bool Prefix(Pawn predator, ref Pawn __result)
	{
		if (predator.IsServant())
		{
			__result = null;
			return false;
		}
		return true;
	}
}
