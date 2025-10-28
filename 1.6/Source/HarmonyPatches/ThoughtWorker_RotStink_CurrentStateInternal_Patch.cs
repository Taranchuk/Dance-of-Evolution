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
			if (p.IsServant() || p.IsFungalNexus() ||
				p.health?.hediffSet?.HasHediff(DefsOf.FleshmassLung) == true ||
				p.health?.hediffSet?.HasHediff(HediffDefOf.DetoxifierLung) == true)
			{
				__result = ThoughtState.Inactive;
			}
			else if (p.Spawned && p.Position.GetTerrain(p.Map) == DefsOf.DE_RottenSoil)
			{
				__result = ThoughtState.ActiveAtStage(1);
			}
			else if (p.Spawned && p.Position.GetTerrain(p.Map) == DefsOf.DE_MyceliumFerrite && p.HostileTo(Faction.OfPlayer))
			{
				__result = ThoughtState.ActiveAtStage(1);
			}
		}
	}
}
