using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Thing), "Ingested")]
	public static class Thing_Ingested_Patch
	{
		public static void Postfix(Thing __instance, Pawn ingester)
		{
			if (__instance.def == DefsOf.DE_FungalSlurry && ingester.IsServant() is false && ingester.HasFungalNexus() is false)
			{
				FoodUtility.AddFoodPoisoningHediff(ingester, __instance, FoodPoisonCause.Rotten);
			}
		}
	}
}
