using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(Pawn_ApparelTracker), "Wear")]
	public static class Pawn_ApparelTracker_Wear_Patch
	{
		public static void Postfix(Pawn_ApparelTracker __instance, Apparel newApparel)
		{
			if (newApparel.def == DefsOf.DE_LivingDress)
			{
				var comp = newApparel.GetComp<CompLivingDress>();
				comp?.OnEquipped(__instance.pawn);
			}
		}
	}
}
