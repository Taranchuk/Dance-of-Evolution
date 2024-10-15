using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(AnimalPenUtility), "NeedsToBeManagedByRope")]
	public static class AnimalPenUtility_NeedsToBeManagedByRope_Patch
	{
		public static void Postfix(Pawn pawn, ref bool __result)
		{
			if (__result && pawn.IsServant())
			{
				__result = false;
			}
		}
	}
}
