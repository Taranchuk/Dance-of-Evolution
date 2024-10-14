using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
	[StaticConstructorOnStartup]
	public static class Startup
	{
		static Startup()
		{
			PatchTraders();
			PatchThinkTreeDefs();
			DefsOf.DE_FungalNode.building.fixedStorageSettings.filter.disallowedThingDefs.Add(DefsOf.DE_FungalSlurry);
			foreach (var item in DefsOf.DE_FungalNode.building.fixedStorageSettings.filter.AllowedThingDefs.ToList())
			{
				if (item.IsCorpse && item.race.IsMechanoid || item.IsNutritionGivingIngestible is false)
				{
					DefsOf.DE_FungalNode.building.fixedStorageSettings.filter.disallowedThingDefs.Add(item);
					DefsOf.DE_FungalNode.building.fixedStorageSettings.filter.SetAllow(item, false);
				}
			}
		}

		public static void PatchThinkTreeDefs()
		{
			var thinkTreeDefs = DefDatabase<ThinkTreeDef>.AllDefsListForReading;
			foreach (var thinkTreeDef in thinkTreeDefs)
			{
				var rootNode = thinkTreeDef.thinkRoot;
				if (rootNode == null || rootNode.subNodes == null) continue;
				//bool inserted = false;
				int queuedJobNodeIndex = rootNode.subNodes.FindIndex(node => node.GetType() == typeof(ThinkNode_QueuedJob));
				if (queuedJobNodeIndex >= 0)
				{
					InsertNodeAt(rootNode.subNodes, queuedJobNodeIndex);
					//inserted = true;
				}
				else
				{
					int subtreeNodeIndex = rootNode.subNodes.FindIndex(node => node.GetType() == typeof(ThinkNode_Subtree) &&
						(node as ThinkNode_Subtree)?.treeDef.defName == "LordDuty");

					if (subtreeNodeIndex >= 0)
					{
						InsertNodeAt(rootNode.subNodes, subtreeNodeIndex);
						//inserted = true;
					}
				}

				//if (inserted)
				//	Log.Message($"Patched {thinkTreeDef.defName} - {rootNode.subNodes.ToStringSafeEnumerable()}");

			}
		}

		private static void InsertNodeAt(List<ThinkNode> subNodes, int index)
		{
			var newNode = new ThinkNode_IsControllableServant
			{
				subNodes = new List<ThinkNode>
				{
					new ThinkNode_Tagger
					{
						tagToGive = JobTag.DraftedOrder,
						subNodes = new List<ThinkNode>
						{
							new JobGiver_MoveToStandable(),
							new JobGiver_Orders()
						}
					}
				}
			};

			if (subNodes.Any(x => x is ThinkNode_QueuedJob is false))
			{
				newNode.subNodes.Insert(0, new ThinkNode_QueuedJob());
			}

			if (subNodes.Any(x => x is JobGiver_ReactToCloseMeleeThreat is false))
			{
				newNode.subNodes.Insert(0, new JobGiver_ReactToCloseMeleeThreat());
			}
			subNodes.Insert(index, newNode);
			subNodes.Insert(index + 1, new JobGiver_ConsumeSpores());
		}

		private static void PatchTraders()
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