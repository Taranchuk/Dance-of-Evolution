using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddUndraftedOrders")]
	public static class FloatMenuMakerMap_AddUndraftedOrders_Patch
	{
		[HarmonyPriority(Priority.First)]
		public static bool Prefix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
		{
			if (pawn.IsControllableServant())
			{
				return false;
			}
			return true;
		}
	}
}