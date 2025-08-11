using System.Linq;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
	public class JobGiver_ConsumeSpores : ThinkNode_JobGiver
	{
		public override Job TryGiveJob(Pawn pawn)
		{
			if (pawn.IsControllableServantNoTileAndDownedCheck() || pawn.IsFungalNexus())
			{
				var sporeMakers = pawn.Map.listerThings.ThingsOfDef(DefsOf.DE_Sporemaker).Cast<Building_Sporemaker>();
				foreach (var sporeMaker in sporeMakers.OrderBy(x => x.Position.DistanceTo(pawn.Position)))
				{
					if (sporeMaker.Active && pawn.health.hediffSet.GetFirstHediffOfDef(sporeMaker.sporeHediff) is null)
					{
						return JobMaker.MakeJob(DefsOf.DE_ConsumeSpores, sporeMaker);
					}
				}
			}
			return null;
		}
	}
}
