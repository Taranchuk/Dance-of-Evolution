using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(FoodUtility), "WillEat", new Type[] { typeof(Pawn), typeof(ThingDef), typeof(Pawn), typeof(bool), typeof(bool) })]
	public static class FoodUtility_WillEat_ThingDef_Patch
	{
		[HarmonyPriority(int.MinValue)]
		public static void Postfix(ref bool __result, Pawn p, ThingDef food)
		{
			if (p.IsServant())
			{
				__result = food == DefsOf.DE_FungalSlurry;
			}
		}
	}
}
