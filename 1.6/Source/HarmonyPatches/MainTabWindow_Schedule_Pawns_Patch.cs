using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(MainTabWindow_Schedule), "Pawns", MethodType.Getter)]
	public static class MainTabWindow_Schedule_Pawns_Patch
	{
		public static void Postfix(ref IEnumerable<Pawn> __result)
		{
			__result = __result.Where(p => !p.IsServant());
		}
	}
}
