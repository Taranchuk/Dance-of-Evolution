using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using System.Text;
using Verse.Sound;
using Verse.AI;
namespace DanceOfEvolution
{
	public class Building_Cerebrum : Building_WorkTable, ITargetingSource
	{
		public int corpseCount;
		
		public const int MAX_CORPSE_TO_HARVEST = 10;
		private Texture2D activateTex;

		public bool CasterIsPawn => true;

		public bool IsMeleeAttack => false;

		public bool Targetable => true;

		public bool MultiSelect => false;

		public bool HidePawnTooltips => false;

		public Thing Caster => this;

		public Pawn CasterPawn => null;

		public Verb GetVerb => null;

		public Texture2D UIIcon
		{
			get
			{
				if (activateTex == null)
				{
					activateTex = ContentFinder<Texture2D>.Get("UI/Designators/Harvest", true);
				}
				return activateTex;
			}
		}

		public TargetingParameters targetParams
		{
			get
			{
				return new TargetingParameters
				{
					canTargetPawns = true,
					canTargetAnimals = false,
					canTargetMechs = false,
					canTargetHumans = true,
					canTargetItems = false
				};
			}
		}

		public ITargetingSource DestinationSelector => null;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (var g in base.GetGizmos())
			{
				yield return g;
			}
			if (corpseCount >= MAX_CORPSE_TO_HARVEST)
			{
				Command_Action command_Action = new Command_Action
				{
					defaultLabel = "DesignatorHarvest".Translate(),
					defaultDesc = "DesignatorHarvestDesc".Translate(),
					icon = UIIcon,
					action = delegate
					{
						SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
						Find.Targeter.BeginTargeting(this);
					}
				};
				AcceptanceReport acceptanceReport = CanInteract();
				if (!acceptanceReport.Accepted)
				{
					command_Action.Disable(acceptanceReport.Reason.CapitalizeFirst());
				}
				yield return command_Action;
			}
		}

		public virtual AcceptanceReport CanInteract(Pawn activateBy = null)
		{
			if (activateBy != null)
			{
				if (activateBy.HasFungalNexus() is false)
				{
					return "DE_OnlyFungalNexusCanHarvestThis".Translate();
				}
				if (activateBy.Dead)
				{
					return "PawnIsDead".Translate(activateBy);
				}
				if (activateBy.Downed)
				{
					return "MessageRitualPawnDowned".Translate(activateBy);
				}
				if (activateBy.Deathresting)
				{
					return "IsDeathresting".Translate(activateBy.Named("PAWN"));
				}
				if (!activateBy.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
				{
					return "MessageIncapableOfManipulation".Translate(activateBy);
				}
				if (!activateBy.CanReach(this, PathEndMode.ClosestTouch, Danger.Deadly))
				{
					return "CannotReach".Translate();
				}
			}
			return true;
		}

		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
		{
			foreach (var f in base.GetFloatMenuOptions(selPawn))
			{
				yield return f;
			}
			if (corpseCount >= MAX_CORPSE_TO_HARVEST)
			{
				AcceptanceReport acceptanceReport = CanInteract(selPawn);
				FloatMenuOption floatMenuOption = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("DesignatorHarvest".Translate(), delegate
				{
					OrderForceTarget(selPawn);
				}), selPawn, this);
				if (!acceptanceReport.Accepted)
				{
					floatMenuOption.Disabled = true;
					floatMenuOption.Label = floatMenuOption.Label + " (" + acceptanceReport.Reason.UncapitalizeFirst() + ")";
				}
				yield return floatMenuOption;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref corpseCount, "corpseCount");
		}

		public bool CanHitTarget(LocalTargetInfo target)
		{
			return ValidateTarget(target, showMessages: false);
		}

		public bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
		{
			if (!target.IsValid || target.Pawn == null)
			{
				return false;
			}
			Pawn pawn = target.Pawn;
			AcceptanceReport acceptanceReport = CanInteract(pawn);
			if (!acceptanceReport.Accepted)
			{
				if (showMessages && !acceptanceReport.Reason.NullOrEmpty())
				{
					Messages.Message("CannotGenericWorkCustom".Translate("DesignatorHarvest".Translate()) + ": " + acceptanceReport.Reason.CapitalizeFirst(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return true;
		}

		public void DrawHighlight(LocalTargetInfo target)
		{
			if (target.IsValid)
			{
				GenDraw.DrawTargetHighlight(target);
			}
		}

		public void OrderForceTarget(LocalTargetInfo target)
		{
			if (ValidateTarget(target, showMessages: false))
			{
				Job job = JobMaker.MakeJob(DefsOf.DE_HarvestCerebrum, this);
				target.Pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
			}
		}

		public void OnGUI(LocalTargetInfo target)
		{
			string label = "VoidMonolithChooseActivator".Translate();
			Widgets.MouseAttachedLabel(label);
			if (ValidateTarget(target, showMessages: false) && targetParams.CanTarget(target.Pawn, this))
			{
				GenUI.DrawMouseAttachment(UIIcon);
			}
			else
			{
				GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
			}
		}
	}
}
