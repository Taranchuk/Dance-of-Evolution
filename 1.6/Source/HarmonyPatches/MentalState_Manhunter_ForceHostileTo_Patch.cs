using Verse;
using HarmonyLib;
using Verse.AI;
using System;
namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(MentalState_Manhunter), "ForceHostileTo", new Type[] { typeof(Thing) })]
	public static class MentalState_Manhunter_ForceHostileTo_Patch
	{
		public static void Postfix(Thing t, ref bool __result)
		{
			if (__result is false && t is Pawn p && p.IsServant())
			{
				__result = true;
			}
		}
	}
}