using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new[] { typeof(PawnGenerationRequest) })]
	public static class PawnGenerator_GeneratePawn_Patch
	{
		public static void Postfix(PawnGenerationRequest request, ref Pawn __result)
		{
			if (__result?.Faction?.def == DefsOf.DE_Mycelyss && __result.RaceProps.Humanlike && __result.health.hediffSet.GetFirstHediffOfDef(DefsOf.DE_FungalNexus) is null)
			{
				var fungalNexusHediff = HediffMaker.MakeHediff(DefsOf.DE_FungalNexus, __result);
				__result.health.AddHediff(fungalNexusHediff);
			}
		}
	}
}
