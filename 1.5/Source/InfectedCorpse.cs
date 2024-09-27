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
		
		public Faction parentFaction;
		public InfectedCorpse(Corpse corpse, Faction faction)
		{
			this.corpse = corpse;
			InitializeTicksUntilReanimation();
			parentFaction = faction;
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

		public void Tick(out bool reanimated)
		{
			reanimated = false;
			ticksUntilReanimation--;
			if (ticksUntilReanimation <= 0)
			{
				ReanimateCorpse();
				reanimated = true;
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
			if (pawn.Faction != parentFaction)
			{
				pawn.SetFaction(parentFaction);
			}
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
			Scribe_References.Look(ref parentFaction, "parentFaction");
		}
	}
}