using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	public class GameCondition_DeathPallCloudmaker : GameCondition_ForceWeather
	{
		private static readonly IntRange ResurrectIntervalRange = new IntRange(300, 900);
		private int nextResurrectTick;
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextResurrectTick, "nextResurrectTick", 0);
		}

		public override void Init()
		{
			if (!ModLister.CheckAnomaly("Death pall"))
			{
				End();
				return;
			}
			base.Init();
			nextResurrectTick = Find.TickManager.TicksGame + ResurrectIntervalRange.RandomInRange;
		}

		public override void GameConditionTick()
		{
			if (Find.TickManager.TicksGame < nextResurrectTick || Find.TickManager.TicksGame % 60 != 0)
			{
				return;
			}
			foreach (Map affectedMap in base.AffectedMaps)
			{
				foreach (Thing item in affectedMap.listerThings.ThingsInGroup(ThingRequestGroup.Corpse))
				{
					if (item is Corpse corpse && MutantUtility.CanResurrectAsShambler(corpse) && corpse.Age >= 15000)
					{
						Pawn pawn = ResurrectPawn(corpse);
						if (!pawn.Position.Fogged(affectedMap))
						{
							Messages.Message("DeathPallResurrectedMessage".Translate(pawn), pawn, MessageTypeDefOf.NegativeEvent, historical: false);
						}
						nextResurrectTick = Find.TickManager.TicksGame + ResurrectIntervalRange.RandomInRange;
						return;
					}
				}
			}
		}

		private Pawn ResurrectPawn(Corpse corpse)
		{
			Pawn innerPawn = corpse.InnerPawn;
			MutantUtility.ResurrectAsShambler(innerPawn, 60000, Faction.OfPlayer);
			return innerPawn;
		}

		public override void End()
		{
			Find.LetterStack.ReceiveLetter("LetterLabelDeathPallEnded".Translate(), "LetterDeathPallEnded".Translate(), LetterDefOf.NeutralEvent);
			base.End();
			base.SingleMap.weatherDecider.StartNextWeather();
		}
	}
}
