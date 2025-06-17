using RimWorld;
using Verse;
using HarmonyLib;
namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(Pawn), "CanTakeOrder", MethodType.Getter)]
	public static class Pawn_CanTakeOrder_Patch
	{
		public static void Postfix(Pawn __instance, ref bool __result)
		{
			if (__result is false && __instance.IsControllableServant())
			{
				__result = true;
			}
		}
	}
}
