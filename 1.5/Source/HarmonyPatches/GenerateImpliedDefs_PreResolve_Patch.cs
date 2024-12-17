using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(DefGenerator), "GenerateImpliedDefs_PreResolve")]
	public static class GenerateImpliedDefs_PreResolve_Patch
	{
		public static void Postfix()
		{

			// Find all unique PreWander tags from ThinkNode_SubtreesByTag nodes
			var preWanderTags = new HashSet<string>();
			foreach (var thinkTreeDef in DefDatabase<ThinkTreeDef>.AllDefsListForReading)
			{
				if (thinkTreeDef.thinkRoot == null) continue;
				FindPreWanderTags(thinkTreeDef.thinkRoot, preWanderTags);
			}

			// Create servant work types dictionary
			var servantWorkTypes = new Dictionary<HediffDef, List<WorkTypeDef>>
			{
				{ DefsOf.DE_ServantSmall, new List<WorkTypeDef> { WorkTypeDefOf.Hauling, WorkTypeDefOf.Cleaning, WorkTypeDefOf.Firefighter, DefsOf.BasicWorker } },
				{ DefsOf.DE_ServantMedium, new List<WorkTypeDef> { WorkTypeDefOf.Mining, WorkTypeDefOf.Hunting, DefsOf.Cooking, WorkTypeDefOf.Construction } },
				{ DefsOf.DE_ServantLarge, new List<WorkTypeDef> { WorkTypeDefOf.Growing, WorkTypeDefOf.PlantCutting } },
				{ DefsOf.DE_ServantSpecial, new List<WorkTypeDef> { WorkTypeDefOf.Growing, WorkTypeDefOf.PlantCutting } }
			};

			// For each PreWander tag, create a new ThinkTreeDef
			foreach (var tag in preWanderTags)
			{
				var prefix = tag.Split('_')[0]; // Get the prefix (e.g., "Ghoul" from "Ghoul_PreWander")
				Log.Message("Added " + prefix + " servant work");
				var servantDef = new ThinkTreeDef
				{
					defName = $"DE_{prefix}ServantWork",
					insertTag = tag,
					insertPriority = 100,
					thinkRoot = new ThinkNode_Priority
					{
						subNodes = new List<ThinkNode>
						{
							new ThinkNode_IsControllableServant
							{
								subNodes = servantWorkTypes.Select(kvp => new ThinkNode_ConditionalHasHediff
								{
									hediff = kvp.Key,
									subNodes = new List<ThinkNode>
									{
										new JobGiver_DoWork
										{
											workTypes = kvp.Value
										}
									}
								}).ToList<ThinkNode>()
							}
						}
					}
				};

				DefGenerator.AddImpliedDef(servantDef);
			}

		}

		private static void FindPreWanderTags(ThinkNode node, HashSet<string> tags)
		{
			if (node is ThinkNode_SubtreesByTag subtreeNode && subtreeNode.insertTag?.EndsWith("_PreWander") == true)
			{
				tags.Add(subtreeNode.insertTag);
			}

			if (node.subNodes != null)
			{
				foreach (var subNode in node.subNodes)
				{
					FindPreWanderTags(subNode, tags);
				}
			}
		}

	}
}