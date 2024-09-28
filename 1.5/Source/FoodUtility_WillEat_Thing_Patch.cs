using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(FoodUtility), "WillEat", new Type[] { typeof(Pawn), typeof(Thing), typeof(Pawn), typeof(bool), typeof(bool) })]
	public static class FoodUtility_WillEat_Thing_Patch
	{
		[HarmonyPriority(int.MinValue)]
		public static void Postfix(ref bool __result, Pawn p, Thing food)
		{
			if (p.IsServant())
			{
				__result = food.def == DefsOf.DE_FungalSlurry;
			}
		}
	}
}
