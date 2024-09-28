using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "CanBleed", MethodType.Getter)]
	public static class Pawn_HealthTracker_CanBleed_Patch
	{
		public static void Postfix(Pawn_HealthTracker __instance, ref bool __result)
		{
			if (__instance.pawn.IsServant())
			{
				__result = false;
			}
		}
	}	
}