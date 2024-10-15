using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(ThoughtWorker_RotStink), nameof(ThoughtWorker_RotStink.CurrentStateInternal))]
	public static class ThoughtWorker_RotStink_CurrentStateInternal_Patch
	{
		public static void Postfix(ref ThoughtState __result, Pawn p)
		{
			if (p.IsServant() || p.HasFungalNexus())
			{
				__result = ThoughtState.Inactive;
			}
			else if (p.Spawned && p.Position.GetTerrain(p.Map) == DefsOf.DE_RottenSoil)
			{
				__result = ThoughtState.ActiveAtStage(1);
			}
		}
	}
}
