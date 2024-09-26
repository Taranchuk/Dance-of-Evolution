using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[DefOf]
	public static class DefsOf
	{
		public static ThingDef DE_NexusBurgeon;
	}
	
	[StaticConstructorOnStartup]
	public static class Startup
	{
		static Startup()
		{
			// Get all FactionDefs
			var allFactionDefs = DefDatabase<FactionDef>.AllDefsListForReading;

			// Filter to get only neolithic factions
			var neolithicFactions = allFactionDefs.Where(f => f.techLevel == TechLevel.Neolithic);

			// Iterate through each neolithic faction
			foreach (var faction in neolithicFactions)
			{
				// Add DE_NexusBurgeon to caravanTraderKinds
				foreach (var traderKind in faction.caravanTraderKinds)
				{
					AddToStockGenerators(traderKind);
				}

				// Add DE_NexusBurgeon to baseTraderKinds
				foreach (var traderKind in faction.baseTraderKinds)
				{
					AddToStockGenerators(traderKind);
				}
			}
		}

		private static void AddToStockGenerators(TraderKindDef traderKind)
		{
			// Ensure the stockGenerators list is not null
			if (traderKind.stockGenerators == null)
			{
				traderKind.stockGenerators = new List<StockGenerator>();
			}

			// Add DE_NexusBurgeon to the stockGenerators
			traderKind.stockGenerators.Add(new StockGenerator_SingleDef
			{
				thingDef = DefsOf.DE_NexusBurgeon
			});
		}
	}
}