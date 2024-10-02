using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(PawnRenderer), "CurRotDrawMode", MethodType.Getter)]
	public static class PawnRenderer_CurRotDrawMode_Patch
	{
		public static void Postfix(PawnRenderer __instance, ref RotDrawMode __result)
		{
			if (__result == RotDrawMode.Fresh && __instance.pawn.IsServant(out var hediff) && hediff.ServantType != ServantType.Burrower 
			&& hediff.ServantType != ServantType.Ghoul)
			{
				__result = RotDrawMode.Rotting;
			}
		}
	}
}
