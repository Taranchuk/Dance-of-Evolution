using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(ThingDefGenerator_Corpses), nameof(ThingDefGenerator_Corpses.ImpliedCorpseDefs))]
	public static class ThingDefGenerator_Corpses_ImpliedCorpseDefs_Patch
	{
		public static IEnumerable<ThingDef> Postfix(IEnumerable<ThingDef> __result)
		{
			foreach (var thingDef in __result)
			{
				if (!SkipDef(thingDef))
				{
					yield return thingDef;
				}
			}
		}

		public static bool SkipDef(ThingDef thingDef)
		{
			if (thingDef.ingestible?.sourceDef != null)
			{
				if (thingDef.ingestible.sourceDef == DefsOf.DE_Burrower.race)
				{
					return true;
				}
			}
			return false;
		}
	}
}