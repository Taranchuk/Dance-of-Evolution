using System;
using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DanceOfEvolution
{
	[HarmonyPatch]
	internal static class FoodUtility_BestFoodSourceOnMap_Transpiler2
	{
		static MethodBase TargetMethod()
		{
			var nestedTypes = typeof(FoodUtility).GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (var nestedType in nestedTypes)
			{
				if (nestedType.Name.Contains("c__DisplayClass") && nestedType.GetMethod("<BestFoodSourceOnMap>b__0", BindingFlags.NonPublic | BindingFlags.Instance) != null)
				{
					return nestedType.GetMethod("<BestFoodSourceOnMap>b__0", BindingFlags.NonPublic | BindingFlags.Instance);
				}
			}
			throw new Exception("Failed to find the target method in FoodUtility's compiler-generated class.");
		}

		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
		{
			var thingDefOfMealNutrientPasteField = AccessTools.Field(typeof(ThingDefOf), nameof(ThingDefOf.MealNutrientPaste));
			var getterCanManipulateField = AccessTools.Field(originalMethod.DeclaringType, "getterCanManipulate");

			var interceptThingDefMethod = AccessTools.Method(typeof(FoodUtility_BestFoodSourceOnMap_Transpiler2), nameof(GetInterceptedThingDef));
			var interceptGetterCanManipulateMethod = AccessTools.Method(typeof(FoodUtility_BestFoodSourceOnMap_Transpiler2), nameof(GetInterceptedGetterCanManipulate));

			foreach (var instruction in instructions)
			{
				// Always yield the original instruction first
				yield return instruction;

				// Intercept ThingDefOf.MealNutrientPaste access
				if (instruction.LoadsField(thingDefOfMealNutrientPasteField))
				{
					yield return new CodeInstruction(OpCodes.Ldloc_1); // Load the Building_NutrientPasteDispenser from loc.1
					yield return new CodeInstruction(OpCodes.Call, interceptThingDefMethod);
				}

				// Intercept getterCanManipulate field access
				if (instruction.LoadsField(getterCanManipulateField))
				{
					yield return new CodeInstruction(OpCodes.Ldloc_1); // Load the Building_NutrientPasteDispenser from loc.1
					yield return new CodeInstruction(OpCodes.Call, interceptGetterCanManipulateMethod);
				}
			}
		}

		private static ThingDef GetInterceptedThingDef(ThingDef originalThingDef, Building_NutrientPasteDispenser buildingNutrientPasteDispenser)
		{
			if (buildingNutrientPasteDispenser != null)
			{
				return buildingNutrientPasteDispenser.DispensableDef;
			}
			return originalThingDef;
		}

		private static bool GetInterceptedGetterCanManipulate(bool originalGetterCanManipulate, Building_NutrientPasteDispenser buildingNutrientPasteDispenser)
		{
			if (buildingNutrientPasteDispenser is Building_FungalNode)
			{
				return true;
			}
			return originalGetterCanManipulate;
		}
	}
}
