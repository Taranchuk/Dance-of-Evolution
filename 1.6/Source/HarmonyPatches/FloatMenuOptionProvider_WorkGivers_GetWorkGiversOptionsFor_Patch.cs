using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(FloatMenuOptionProvider_WorkGivers), "GetWorkGiversOptionsFor")]
	public static class FloatMenuOptionProvider_WorkGivers_GetWorkGiversOptionsFor_Patch
	{
		public static IEnumerable<FloatMenuOption> Postfix(IEnumerable<FloatMenuOption> __result, Pawn pawn)
		{
			if (pawn.IsControllableServant())
			{
				yield break;
			}
			else
			{
				foreach (var option in __result)
				{
					yield return option;
				}
			}
		}
	}
}
