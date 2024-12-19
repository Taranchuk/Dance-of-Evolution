using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace DanceOfEvolution
{

	[HarmonyPatch(typeof(FloatMenuMakerMap), "AddMutantOrders")]
	public static class FloatMenuMakerMap_AddMutantOrders_Patch
	{
		public static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
		{
			if (pawn.IsMutant && pawn.mutant.def == DefsOf.DE_FungalGhoul)
			{
				IntVec3 clickCell = IntVec3.FromVector3(clickPos);
				foreach (Thing thing in pawn.Map.thingGrid.ThingsAt(clickCell))
				{
					var comp = thing.TryGetComp<CompUsableNeuralEnhancer>();
					if (comp != null)
					{
						foreach (FloatMenuOption floatMenuOption4 in thing.GetFloatMenuOptions(pawn))
						{
							opts.Add(floatMenuOption4);
						}
					}
				}
			}
		}
	}
}