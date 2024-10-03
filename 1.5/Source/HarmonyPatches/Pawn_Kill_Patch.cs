using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(Pawn), "Kill")]
	public static class Pawn_Kill_Patch
	{
		private static void Postfix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
		{
			if (__instance.Dead)
			{
				if (__instance.kindDef == DefsOf.DE_Burrower)
				{
					if (__instance.Corpse != null && !__instance.Corpse.Destroyed)
					{
						__instance.Corpse.Destroy();
					}
				}
				if (__instance.IsFungalNexus(out var hediff))
				{
					foreach (var servant in hediff.servants)
					{
						servant.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter);	
					}
				}
				if (__instance.IsServant(out var hediff2))
				{
					hediff2.masterHediff?.servants.Remove(__instance);
				}
			}
		}
	}
}