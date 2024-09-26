using System.Linq;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [StaticConstructorOnStartup]
	public static class Startup
	{
		static Startup()
		{
			// Get all FactionDefs
			var allFactionDefs = DefDatabase<FactionDef>.AllDefsListForReading;

			// Filter to get only neolithic factions
			var neolithicFactions = allFactionDefs.Where(f => f.humanlikeFaction
			 && f.techLevel == TechLevel.Neolithic);

			foreach (var faction in neolithicFactions)
			{
				foreach (var traderKind in faction.caravanTraderKinds)
				{
					AddToStockGenerators(traderKind);
				}

				foreach (var traderKind in faction.baseTraderKinds)
				{
					AddToStockGenerators(traderKind);
				}
			}
		}

		private static void AddToStockGenerators(TraderKindDef traderKind)
		{
			if (traderKind.stockGenerators != null)
			{
				traderKind.stockGenerators.Add(new StockGenerator_SingleDef
				{
					thingDef = DefsOf.DE_NexusBurgeon,
					countRange = new IntRange(1, 10),
				});
			}
		}
	}
}