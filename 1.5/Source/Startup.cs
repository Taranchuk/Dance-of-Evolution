using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
	[StaticConstructorOnStartup]
	public static class Startup
	{
		public static List<HediffDef> bodyAttachments = new List<HediffDef>();
		static Startup()
		{
			PatchTraders();
			PatchThinkTreeDefs();
			ChangeStorageSettings(DefsOf.DE_FungalNode);
			foreach (var item in DefDatabase<HediffDef>.AllDefsListForReading)
			{
				var extension = item.GetModExtension<HediffExtension>();
				if (extension != null && extension.isBodyAttachment)
				{
					bodyAttachments.Add(item);
				}
			}
		}

		private static void ChangeStorageSettings(ThingDef def)
		{
			def.building.fixedStorageSettings.filter.disallowedThingDefs.Add(DefsOf.DE_FungalSlurry);
			foreach (var item in def.building.fixedStorageSettings.filter.AllowedThingDefs.ToList())
			{
				if (item.IsCorpse && item.race.IsMechanoid || item.IsNutritionGivingIngestible is false)
				{
					def.building.fixedStorageSettings.filter.disallowedThingDefs.Add(item);
					def.building.fixedStorageSettings.filter.SetAllow(item, false);
				}
			}
		}

		public static void PatchThinkTreeDefs()
		{
			var thinkTreeDefs = DefDatabase<ThinkTreeDef>.AllDefsListForReading;
			foreach (var thinkTreeDef in thinkTreeDefs)
			{
				if (thinkTreeDef.defName == "Downed") continue;
				var rootNode = thinkTreeDef.thinkRoot;
				if (rootNode == null || rootNode.subNodes == null) continue;
				bool inserted = false;
				int isColonist = rootNode.subNodes.FindIndex(node => node.GetType() == typeof(ThinkNode_ConditionalColonist));
				if (isColonist >= 0)
				{
					rootNode.subNodes.Insert(isColonist + 1, new JobGiver_ConsumeSpores());
					inserted = true;
				}
				else
				{
					int queuedJobNodeIndex = rootNode.subNodes.FindIndex(node => node.GetType() == typeof(ThinkNode_QueuedJob));
					if (queuedJobNodeIndex >= 0)
					{
						InsertNodeAt(rootNode.subNodes, queuedJobNodeIndex);
						inserted = true;
					}
					else
					{
						int subtreeNodeIndex = rootNode.subNodes.FindIndex(node => node.GetType() == typeof(ThinkNode_Subtree) &&
							(node as ThinkNode_Subtree)?.treeDef.defName == "LordDuty");

						if (subtreeNodeIndex >= 0)
						{
							InsertNodeAt(rootNode.subNodes, subtreeNodeIndex);
							inserted = true;
						}
						else
						{
							var revenantIndex = rootNode.subNodes.FindIndex(node => node.GetType() == 
								typeof(ThinkNode_ConditionalRevenantState));
							if (revenantIndex >= 0)
							{
								InsertNodeAt(rootNode.subNodes, revenantIndex);
								inserted = true;
							}
							else
							{
								Log.Message("Could not patch " + thinkTreeDef.defName);
							}
						}
					}
				}

				if (inserted)
					Log.Message($"Patched {thinkTreeDef.defName} - {rootNode.subNodes.ToStringHuman()}");
				else
				{
					Log.Message("2 Could not patch " + thinkTreeDef.defName);
				}
			}
		}
		
		public static string ToStringHuman(this List<ThinkNode> nodes)
		{
			var sb = new StringBuilder();
			foreach (var item in nodes)
			{
				if (item is ThinkNode_Subtree subtree)
				{
					sb.AppendWithComma(subtree + " - " + subtree.treeDef);
				}
				else
				{
					sb.AppendWithComma(item.ToString());
				}
			}
			return sb.ToString().TrimEndNewlines();
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
			if (subNodes.Any(x => x is JobGiver_SeekAllowedArea is false))
			{
				newNode.subNodes.Insert(newNode.subNodes.Count - 1, new JobGiver_SeekAllowedArea());
			}
			else
			{
				Log.Message("SeekAllowedArea already exists");
			}
			subNodes.Insert(index, newNode);
			subNodes.Insert(index + 2, new JobGiver_ConsumeSpores());
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