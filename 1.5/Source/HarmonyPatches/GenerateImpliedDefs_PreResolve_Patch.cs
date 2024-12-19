using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(DefGenerator), nameof(DefGenerator.GenerateImpliedDefs_PreResolve))]
	public static class GenerateImpliedDefs_PreResolve_Patch
	{
		public static void Postfix()
		{
			var preWanderTags = new Dictionary<ThinkTreeDef, string>();
			foreach (var thinkTreeDef in DefDatabase<ThinkTreeDef>.AllDefsListForReading)
			{
				if (thinkTreeDef.thinkRoot == null || thinkTreeDef.defName.StartsWith("DE_")) continue;
				FindPreWanderTags(thinkTreeDef.thinkRoot, preWanderTags, thinkTreeDef);
			}

			var servantWorkTypes = new Dictionary<HediffDef, (List<WorkTypeDef> workTypeDefs, List<WorkGiverDef> workgivers)>
			{
				{ DefsOf.DE_ServantSmall, (new List<WorkTypeDef> { WorkTypeDefOf.Hauling, WorkTypeDefOf.Cleaning, WorkTypeDefOf.Firefighter, DefsOf.BasicWorker }, new List<WorkGiverDef> { }) },
				{ DefsOf.DE_ServantMedium, (new List<WorkTypeDef> { WorkTypeDefOf.Mining, WorkTypeDefOf.Hunting, DefsOf.Cooking, WorkTypeDefOf.Construction }, new List<WorkGiverDef> { DefsOf.DoBillsCremate, DefsOf.DE_FeedCorpseToCerebrum }) },
				{ DefsOf.DE_ServantLarge, (new List<WorkTypeDef> { WorkTypeDefOf.Growing, WorkTypeDefOf.PlantCutting }, new List<WorkGiverDef> { }) },
				{ DefsOf.DE_ServantSpecial, (new List<WorkTypeDef> { WorkTypeDefOf.Growing, WorkTypeDefOf.PlantCutting }, new List<WorkGiverDef> { }) }
			};

			foreach (var kvp in preWanderTags)
			{
				var prefix = kvp.Value.Split('_')[0]; // Get the prefix (e.g., "Ghoul" from "Ghoul_PreWander")
				var servantDef = new ThinkTreeDef
				{
					defName = $"DE_{prefix}ServantWork",
					insertTag = kvp.Value,
					insertPriority = 100,
					thinkRoot = new ThinkNode_Priority
					{
						subNodes = new List<ThinkNode>
						{
							MakeServantNode(servantWorkTypes)
						}
					}
				};

				if (DefDatabase<ThinkTreeDef>.GetNamedSilentFail(servantDef.defName) is null)
				{
					DefGenerator.AddImpliedDef(servantDef);
				}
			}

			var nonPatchedThinkTreeDefs = DefDatabase<ThingDef>.AllDefsListForReading.
			Where(x => x.race?.thinkTreeMain != null && x.race.Humanlike is false).Select(x => x.race.thinkTreeMain)
			.Distinct().Where(t => preWanderTags.ContainsKey(t) is false).ToList();
			foreach (var thinkTreeDef in nonPatchedThinkTreeDefs)
			{
				var wander = thinkTreeDef.thinkRoot.subNodes.Where(x => x is JobGiver_Wander wander).LastOrDefault();
				if (wander != null)
				{
					thinkTreeDef.thinkRoot.subNodes.Insert(thinkTreeDef.thinkRoot.subNodes
						.IndexOf(wander), MakeServantNode(servantWorkTypes));
				}
			}
		}

		private static ThinkNode_IsControllableServant MakeServantNode(Dictionary<HediffDef,
		(List<WorkTypeDef> workTypes, List<WorkGiverDef> workgivers)> servantWorkTypes)
		{
			return new ThinkNode_IsControllableServant
			{
				subNodes = servantWorkTypes.Select(workKvp => new ThinkNode_ConditionalHasHediff
				{
					hediff = workKvp.Key,
					subNodes = new List<ThinkNode>
									{
										new JobGiver_DoWork
										{
											workTypes = workKvp.Value.workTypes,
											workgivers = workKvp.Value.workgivers
										}
									}
				}).ToList<ThinkNode>()
			};
		}

		private static void FindPreWanderTags(ThinkNode node, Dictionary<ThinkTreeDef, string> tags, ThinkTreeDef def)
		{
			if (node is ThinkNode_SubtreesByTag subtreeNode && subtreeNode.insertTag?.EndsWith("_PreWander") == true)
			{
				tags[def] = subtreeNode.insertTag;
			}

			if (node.subNodes != null)
			{
				foreach (var subNode in node.subNodes)
				{
					FindPreWanderTags(subNode, tags, def);
				}
			}
		}
	}
}