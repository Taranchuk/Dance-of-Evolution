using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	public class Building_FungalNode : Building_NutrientPasteDispenser
	{
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			powerComp = new CompPowerTrader
			{
				parent = this
			};
			powerComp.powerOnInt = true;
		}
		public override ThingDef DispensableDef => DefsOf.DE_FungalSlurry;
		
		[HarmonyPatch(typeof(Building_NutrientPasteDispenser), "CanDispenseNow", MethodType.Getter)]
		public static class Building_NutrientPasteDispenser_CanDispenseNow_Patch
		{
			public static bool Prefix(ref bool __result, Building_NutrientPasteDispenser __instance)
			{
				if (__instance is Building_FungalNode fungalNode)
				{
					__result = true;
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(Alert_PasteDispenserNeedsHopper), "BadDispensers", MethodType.Getter)]
		public static class Alert_PasteDispenserNeedsHopper_BadDispensers_Patch
		{
			public static void Postfix(ref List<Thing> ___badDispensersResult)
			{
				___badDispensersResult.RemoveAll(d => d is Building_FungalNode);
			}
		}

		[HarmonyPatch(typeof(ThingListGroupHelper), "Includes")]
		public static class ThingListGroupHelper_Includes
		{
			public static void Postfix(ThingDef def, ThingRequestGroup group, ref bool __result)
			{
				if (__result is false && (group == ThingRequestGroup.FoodSourceNotPlantOrTree || group == ThingRequestGroup.FoodSource)
					&& typeof(Building_FungalNode) == def.thingClass)
				{
					__result = true;
				}
			}
		}
	}
}