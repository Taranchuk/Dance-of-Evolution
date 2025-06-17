using HarmonyLib;
using RimWorld;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Need_Food), "FoodFallPerTickAssumingCategory")]
	public static class Need_Food_FoodFallPerTickAssumingCategory_Patch
	{
		public static void Postfix(ref float __result, Need_Food __instance)
		{
			if (__instance.pawn.IsServant())
			{
				__result /= 5f;
			}
		}
	}
}
