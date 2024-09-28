using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(Pawn), "GetGizmos")]
	public static class Pawn_GetGizmos_Patch
	{
		public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn __instance)
		{
			var controllable = __instance.IsControllableServant();
			if (controllable)
			{
				Command_Toggle command_Toggle = new Command_Toggle();
				command_Toggle.hotKey = KeyBindingDefOf.Command_ColonistDraft;
				command_Toggle.isActive = (() => __instance.Drafted);
				command_Toggle.toggleAction = delegate
				{
					if (__instance.drafter is null)
					{
						if (__instance.RaceProps.Animal)
						{
							__instance.equipment = new Pawn_EquipmentTracker(__instance);
						}
						__instance.drafter = new Pawn_DraftController(__instance);
						__instance.drafter.Drafted = true;
					}
					else
					{
						__instance.drafter.Drafted = !__instance.Drafted;
					}
					PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.Drafting, KnowledgeAmount.SpecificInteraction);
					if (__instance.drafter.Drafted)
					{
						LessonAutoActivator.TeachOpportunity(ConceptDefOf.QueueOrders, OpportunityType.GoodToKnow);
					}
				};
				command_Toggle.defaultDesc = "CommandToggleDraftDesc".Translate();
				command_Toggle.icon = TexCommand.Draft;
				command_Toggle.turnOnSound = SoundDefOf.DraftOn;
				command_Toggle.turnOffSound = SoundDefOf.DraftOff;
				command_Toggle.groupKey = 81729172;
				command_Toggle.defaultLabel = (__instance.Drafted ? "CommandUndraftLabel" : "CommandDraftLabel").Translate();
				if (__instance.Downed)
				{
					command_Toggle.Disable("IsIncapped".Translate(__instance.LabelShort, __instance));
				}
				if (!__instance.Drafted)
				{
					command_Toggle.tutorTag = "Draft";
				}
				else
				{
					command_Toggle.tutorTag = "Undraft";
				}
				yield return command_Toggle;
			}

			foreach (var g in __result)
			{
				if (controllable && g is Command_Toggle command && command.defaultDesc == "CommandToggleDraftDesc".Translate())
				{
					continue;
				}
				yield return g;
			}
		}
	}
}