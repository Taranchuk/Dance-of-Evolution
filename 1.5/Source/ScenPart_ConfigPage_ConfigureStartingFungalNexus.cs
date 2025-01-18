using RimWorld;
using Verse;
namespace DanceOfEvolution
{
	public class ScenPart_ConfigPage_ConfigureStartingFungalNexus : ScenPart_ConfigPage_ConfigureStartingPawnsBase
	{
		[MustTranslate]
		public string customSummary;
		public override string Summary(Scenario scenario) => customSummary; 
		public override int TotalPawnCount => pawnCount;
		public int pawnCount = 1;
		public override void GenerateStartingPawns()
		{
			int num = 0;
			do
			{
				StartingPawnUtility.ClearAllStartingPawns();
				for (int i = 0; i < pawnCount; i++)
				{
					StartingPawnUtility.AddNewPawn();
				}
				num++;
			}
			while (num <= 20 && !StartingPawnUtility.WorkTypeRequirementsSatisfied());
		}
	}
}