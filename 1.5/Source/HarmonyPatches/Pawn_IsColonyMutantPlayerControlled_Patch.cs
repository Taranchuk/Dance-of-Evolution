using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(Pawn), "IsColonyMutantPlayerControlled", MethodType.Getter)]
	public static class Pawn_IsColonyMutantPlayerControlled_Patch
	{
		public static void Postfix(ref bool __result, Pawn __instance)
		{
			if (__result is false && __instance.IsControllableServant())
			{
				__result = true;
			}
		}
	}
}