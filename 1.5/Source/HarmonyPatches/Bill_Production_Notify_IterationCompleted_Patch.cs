using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(Bill_Production), "Notify_IterationCompleted")]
	public static class Bill_Production_Notify_IterationCompleted_Patch
	{
		public static void Postfix(Bill_Production __instance)
		{
			if (__instance.recipe == DefsOf.DE_FeedCorpse &&__instance.billStack.billGiver is Building_Cerebrum buildingCerebrum)
			{
				buildingCerebrum.corpseCount++;
				Log.Message("Feed corpse: " + buildingCerebrum.corpseCount);
			}
		}
	}
}