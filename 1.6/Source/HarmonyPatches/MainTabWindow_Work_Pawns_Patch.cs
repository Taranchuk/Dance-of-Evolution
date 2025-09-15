using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(MainTabWindow_Work), "Pawns", MethodType.Getter)]
	public static class MainTabWindow_Work_Pawns_Patch
	{
		public static void Postfix(ref IEnumerable<Pawn> __result)
		{
			__result = __result.Where(p => !p.IsServant());
		}
	}
}
