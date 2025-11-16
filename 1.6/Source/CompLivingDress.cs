using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
	public class CompLivingDress : ThingComp
	{
		private Pawn bondedPawn;
		private int lastRegenerationTick;
		private int nextMentalBreakTick = -1;

		public CompProperties_LivingDress Props => (CompProperties_LivingDress)props;

		public Pawn BondedPawn => bondedPawn;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_References.Look(ref bondedPawn, "bondedPawn");
			Scribe_Values.Look(ref lastRegenerationTick, "lastRegenerationTick");
			Scribe_Values.Look(ref nextMentalBreakTick, "nextMentalBreakTick");
		}

		public override void CompTick()
		{
			base.CompTick();
			if (Find.TickManager.TicksGame % 600 == 0)
			{
				TryRegenerate();
			}
			if (parent.Spawned)
			{
				if (nextMentalBreakTick > 0 && Find.TickManager.TicksGame >= nextMentalBreakTick)
				{
					TryTriggerMentalBreak();
				}
			}
		}

		public void OnEquipped(Pawn pawn)
		{
			if (bondedPawn == null)
			{
				bondedPawn = pawn;
				if (nextMentalBreakTick == -1)
				{
					nextMentalBreakTick = Find.TickManager.TicksGame + (5 * 60000);
				}
				if (PawnUtility.ShouldSendNotificationAbout(pawn))
				{
					Messages.Message("DE_LivingDressBonded".Translate(pawn.LabelShort), pawn, MessageTypeDefOf.PositiveEvent);
				}
			}
		}

		private void TryRegenerate()
		{
			if (parent.HitPoints < parent.MaxHitPoints)
			{
				parent.HitPoints = Mathf.Min(parent.HitPoints + 1, parent.MaxHitPoints);
			}
		}

		private void HealBondedPawn(float healingAmount)
		{
			if (bondedPawn == null || healingAmount <= 0)
			{
				return;
			}

			var injuries = new List<Hediff_Injury>();
			bondedPawn.health.hediffSet.GetHediffs(ref injuries, h => h.CanHealNaturally());

			var sortedInjuries = injuries.OrderByDescending(h => h.Severity).ToList();

			foreach (var injury in sortedInjuries)
			{
				if (healingAmount <= 0) break;

				float amountToHeal = Mathf.Min(injury.Severity, healingAmount);
				injury.Heal(amountToHeal);
				healingAmount -= amountToHeal;
			}
		}

		private void TryTriggerMentalBreak()
		{
			if (bondedPawn?.Faction == Faction.OfPlayer && bondedPawn.mindState?.mentalStateHandler != null)
			{
				var mentalStateDef = Rand.Bool ? DefsOf.Binging_Food : DefsOf.Tantrum;
				bondedPawn.mindState.mentalStateHandler.TryStartMentalState(mentalStateDef, null, true);
				nextMentalBreakTick = Find.TickManager.TicksGame + (5 * 60000);
			}
		}

		public void DevourCorpse(Corpse corpse)
		{
			float bodySize = corpse.InnerPawn.RaceProps.baseBodySize;
			int healingGain = 10;
			if (bodySize >= 2.0f)
			{
				healingGain = 100;
			}
			else if (bodySize >= 1.0f || corpse.InnerPawn.RaceProps.Humanlike)
			{
				healingGain = 50;
			}

			HealBondedPawn(healingGain);
			FilthMaker.TryMakeFilth(corpse.Position, corpse.Map, DefsOf.Filth_Fleshmass, 3);
			Messages.Message("DE_LivingDressDevoured".Translate(bondedPawn.LabelShort, corpse.InnerPawn.LabelShort, healingGain),
				bondedPawn, MessageTypeDefOf.PositiveEvent);
			corpse.Destroy();
		}

		public override string CompInspectStringExtra()
		{
			var result = new List<string>();

			if (bondedPawn != null)
			{
				result.Add("DE_BondedTo".Translate(bondedPawn.LabelShort));
			}
			else
			{
				result.Add("DE_NotBonded".Translate());
			}

			return string.Join("\n", result);
		}

	}
}
