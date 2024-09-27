using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Pawn), "Kill")]
	public static class Pawn_Kill_Patch
	{
		private static void Postfix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
		{
			if (__instance.Dead && __instance.kindDef == DefsOf.DE_Burrower)
			{
				if (__instance.Corpse != null && !__instance.Corpse.Destroyed)
				{
					__instance.Corpse.Destroy();
				}
			}
		}
	}
}