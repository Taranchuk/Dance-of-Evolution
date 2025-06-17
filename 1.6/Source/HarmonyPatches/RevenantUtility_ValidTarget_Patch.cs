using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(RevenantUtility), "ValidTarget")]
	public static class RevenantUtility_ValidTarget_Patch
	{
		public static Pawn curPawn;
		public static void Postfix(Pawn pawn, ref bool __result)
		{
			if (__result && curPawn != null && curPawn.IsServant() && curPawn.HostileTo(pawn) is false)
			{
				__result = false;
			}
		}
	}
}
