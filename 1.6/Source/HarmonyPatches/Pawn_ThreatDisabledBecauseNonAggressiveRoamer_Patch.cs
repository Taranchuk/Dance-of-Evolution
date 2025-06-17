using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(Pawn), "ThreatDisabledBecauseNonAggressiveRoamer")]
	public static class Pawn_ThreatDisabledBecauseNonAggressiveRoamer_Patch
	{
		public static void Postfix(Pawn __instance, Pawn otherPawn, ref bool __result)
		{
			if (__result && __instance.IsServant())
			{
				__result = false;
			}
			if (AttackTargetFinder_BestAttackTarget_Validator_Patch.CheckHostility(true,
				__instance, otherPawn) is false)
			{
				__result = true;
			}
		}
	}
}
