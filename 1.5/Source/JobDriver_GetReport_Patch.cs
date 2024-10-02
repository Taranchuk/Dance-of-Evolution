using System.Collections.Generic;
using System.Reflection;
using DanceOfEvolution;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

[HarmonyPatch]
public static class JobDriver_GetReport_Patch
{
	static IEnumerable<MethodBase> TargetMethods()
	{
		yield return AccessTools.Method(typeof(JobDriver_Ingest), nameof(JobDriver_Ingest.GetReport));
		yield return AccessTools.Method(typeof(JobDriver_FoodFeedPatient), nameof(JobDriver_FoodFeedPatient.GetReport));
		yield return AccessTools.Method(typeof(JobDriver_FoodDeliver), nameof(JobDriver_FoodDeliver.GetReport));
	}

	static void Prefix(JobDriver __instance, out ThingDef __state)
	{
		__state = ThingDefOf.MealNutrientPaste;
		var thing = __instance.job.GetTarget(TargetIndex.A).Thing;
		if (thing is Building_NutrientPasteDispenser buildingNutrientPasteDispenser)
		{
			ThingDefOf.MealNutrientPaste = buildingNutrientPasteDispenser.DispensableDef;
		}
		else if (thing.def == DefsOf.DE_FungalSlurry)
		{
			ThingDefOf.MealNutrientPaste = DefsOf.DE_FungalSlurry;
		}
	}

	static void Postfix(ThingDef __state)
	{
		ThingDefOf.MealNutrientPaste = __state;
	}
}
