using HarmonyLib;
using RimWorld;
using Verse;
namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(ITab_Pawn_Character), "IsVisible", MethodType.Getter)]
	public static class ITab_Pawn_Character_IsVisible_Patch
	{
		public static void Postfix(ITab_Pawn_Character __instance, ref bool __result)
		{
			if (__result)
			{
				Pawn pawn = __instance.PawnToShowInfoAbout;
				if (pawn.IsServant() && pawn.RaceProps.Humanlike is false)
					__result = false;
			}
		}
	}
}