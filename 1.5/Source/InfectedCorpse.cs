using RimWorld;
using Verse;
using Verse.Sound;

namespace DanceOfEvolution
{
	public class InfectedCorpse : IExposable
	{
		public Corpse corpse;
		public Sustainer riseSustainer;
		public Effecter riseEffecter;
		private int ticksUntilReanimation;
		public Hediff_FungalNexus hediff_FungalNexus;
		
		public InfectedCorpse()
		{
			
		}
		public InfectedCorpse(Corpse corpse, Hediff_ServantType hediff)
		{
			this.corpse = corpse;
			this.hediff_FungalNexus = hediff.masterHediff;
			InitializeTicksUntilReanimation();
		}

		private void InitializeTicksUntilReanimation()
		{
			float bodySize = corpse.InnerPawn.BodySize;
			if (bodySize <= 0.99f)
			{
				ticksUntilReanimation = GenDate.TicksPerDay; // 1 day in ticks
			}
			else if (bodySize >= 1f && bodySize <= 1.9f)
			{
				ticksUntilReanimation = 2 * GenDate.TicksPerDay; // 2 days in ticks
			}
			else
			{
				ticksUntilReanimation = 3 * GenDate.TicksPerDay; // 3 days in ticks
			}
		}
		public void Tick(out bool remove)
		{
			remove = false;
			if (hediff_FungalNexus == null || hediff_FungalNexus.pawn.DestroyedOrNull())
			{
				remove = true;
				corpse.Destroy();
				return;
			}
			ticksUntilReanimation -= 100;
			if (ticksUntilReanimation <= 0)
			{
				ReanimateCorpse();
				remove = true;
			}
			else
			{
				MaintainEffects();
			}
		}
		private void ReanimateCorpse()
		{
			var pawn = corpse.InnerPawn;
			ResurrectionUtility.TryResurrect(pawn);
			pawn.MakeServant(hediff_FungalNexus, TryGetServantTypeAndHediff(pawn).Value.servantHediffDef);
		}

		public static (ServantType servantType, HediffDef servantHediffDef)? TryGetServantTypeAndHediff(Pawn pawn)
		{
			if (pawn.RaceProps.Humanlike)
			{
				return (ServantType.Ghoul, DefsOf.DE_ServantGhoul);
			}

			float bodySize = pawn.BodySize;
			if (bodySize <= 0.99f)
			{
				return (ServantType.Small, DefsOf.DE_ServantSmall);
			}
			else if (bodySize >= 1f && bodySize <= 1.9f)
			{
				return (ServantType.Medium, DefsOf.DE_ServantMedium);
			}
			else if (bodySize > 1.9f)
			{
				return (ServantType.Large, DefsOf.DE_ServantLarge);
			}
			return null;
		}

		private void MaintainEffects()
		{
			if (riseSustainer == null || riseSustainer.Ended)
			{
				SoundInfo info = SoundInfo.InMap(corpse, MaintenanceType.PerTick);
				riseSustainer = SoundDefOf.Pawn_Shambler_Rise.TrySpawnSustainer(info);
			}
			if (riseEffecter == null)
			{
				riseEffecter = EffecterDefOf.ShamblerRaise.Spawn(corpse, corpse.Map);
			}
			riseSustainer.Maintain();
			riseEffecter.EffectTick(corpse, TargetInfo.Invalid);
		}

		public void ExposeData()
		{
			Scribe_References.Look(ref corpse, "corpse");
			Scribe_Values.Look(ref ticksUntilReanimation, "ticksUntilReanimation");
			Scribe_References.Look(ref hediff_FungalNexus, "hediff_FungalNexus");
		}
	}
}