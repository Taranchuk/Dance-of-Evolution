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
					if (thingDef.ingestible?.sourceDef != null && thingDef.ingestible.sourceDef.race.IsFlesh)
					{
						thingDef.comps.Add(new CompProperties
						{
							compClass = typeof(CompMycelialTreeConsumable)
						});
					}
					yield return thingDef;
				}
			}
		}

		public static bool SkipDef(ThingDef thingDef)
		{
			if (thingDef.ingestible?.sourceDef != null)
			{
				var race = thingDef.ingestible.sourceDef;
				if (race == DefsOf.DE_Burrower.race || race == DefsOf.DE_MikisMetalonEfialtis.race)
				{
					return true;
				}
			}
			return false;
		}
	}
}