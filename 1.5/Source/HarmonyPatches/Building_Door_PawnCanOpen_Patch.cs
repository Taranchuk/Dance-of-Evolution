using HarmonyLib;
using RimWorld;
using Verse;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Building_Door), "PawnCanOpen")]
	public static class Building_Door_PawnCanOpen_Patch
	{
		public static void Postfix(ref bool __result, Pawn p, Building_Door __instance)
		{
			if (p.IsControllableServant())
			{
				__result = true;
			}
		}
	}
}