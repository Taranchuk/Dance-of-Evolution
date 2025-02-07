using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(RevenantUtility), "GetClosestTargetInRadius")]
	public static class RevenantUtility_GetClosestTargetInRadius_Patch
	{
		public static void Prefix(Pawn pawn)
		{
			RevenantUtility_ValidTarget_Patch.curPawn = pawn;
		}
		
		public static void Postfix()
		{
			RevenantUtility_ValidTarget_Patch.curPawn = null;
		}
	}

	[HarmonyPatch(typeof(RevenantUtility), "ScanForTarget_NewTemp")]
	public static class RevenantUtility_ScanForTarget_NewTemp_Patch
	{
		public static void Prefix(Pawn pawn)
		{
			RevenantUtility_ValidTarget_Patch.curPawn = pawn;
		}

		public static void Postfix()
		{
			RevenantUtility_ValidTarget_Patch.curPawn = null;
		}
	}
	
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
