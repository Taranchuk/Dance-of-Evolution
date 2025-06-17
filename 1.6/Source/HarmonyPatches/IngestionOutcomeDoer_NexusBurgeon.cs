using System.Collections.Generic;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    public class IngestionOutcomeDoer_NexusBurgeon : IngestionOutcomeDoer
	{
		public HediffDef hediffDef;
		public override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
		{
			if (pawn.GetStatValue(StatDefOf.PsychicSensitivity) > 0)
			{
				Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
				pawn.health.AddHediff(hediff);
			}
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
		{
			foreach (StatDrawEntry item in hediffDef.SpecialDisplayStats(StatRequest.ForEmpty()))
			{
				yield return item;
			}
		}
	}
}