using RimWorld;
using HarmonyLib;
namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(Pawn_PlayerSettings), "SupportsAllowedAreas", MethodType.Getter)]
	public static class Pawn_PlayerSettings_SupportsAllowedAreas_Patch
	{
		public static bool Prefix(Pawn_PlayerSettings __instance, ref bool __result)
		{
			if (__instance.pawn.IsServant())
			{
				__result = true;
				return false;
			}
			return true;
		}
	}
}