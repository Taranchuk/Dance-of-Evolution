using HarmonyLib;
using RimWorld;
using Verse;
namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(PawnComponentsUtility), "AddAndRemoveDynamicComponents")]
	public static class PawnComponentsUtility_AddAndRemoveDynamicComponents_Patch
	{
		public static void Postfix(Pawn pawn)
		{
			if (pawn.IsControllableServant(out var hediff))
			{
				hediff.AssignComponents();
			}
		}
	}
}