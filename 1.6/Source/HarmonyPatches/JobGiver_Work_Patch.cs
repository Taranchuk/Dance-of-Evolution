using HarmonyLib;
using RimWorld;
using Verse;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(JobGiver_Work), "GetPriority")]
	public static class JobGiver_Work_Patch
	{
		public static bool Prefix(JobGiver_Work __instance, ref float __result, Pawn pawn)
		{
			if (__instance is not JobGiver_DoWork && pawn.IsServant())
			{
				__result = 0f;
				return false;
			}
			return true;
		}
	}
}