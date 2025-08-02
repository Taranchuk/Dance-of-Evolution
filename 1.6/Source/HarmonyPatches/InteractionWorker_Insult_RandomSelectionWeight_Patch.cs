using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(InteractionWorker_Insult), "RandomSelectionWeight")]
	public static class InteractionWorker_Insult_RandomSelectionWeight_Patch
	{
		public static void Postfix(Pawn initiator, Pawn recipient, ref float __result)
		{
			if (initiator?.apparel?.WornApparel?.Any(a => a.def == DefsOf.DE_LivingDress) == true ||
				recipient?.apparel?.WornApparel?.Any(a => a.def == DefsOf.DE_LivingDress) == true)
			{
				__result *= 10f;
			}
		}
	}
}
