using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
	[HotSwappable]
	[HarmonyPatch(typeof(Pawn), "IsColonySubhumanPlayerControlled", MethodType.Getter)]
	public static class Pawn_IsColonySubhumanPlayerControlled_Patch
	{
		public static void Postfix(ref bool __result, Pawn __instance)
		{
			if (__result is false && __instance.Spawned && __instance.IsControllableServant())
			{
				__result = true;
			}
			else if (__result is false && ColonistBar_CheckRecacheEntries_Patch.recachingNow
			&& __instance.IsServant(out var hediff) && hediff.ServantType != ServantType.Burrower)
			{
				__result = true;
			}
		}
	}
}
