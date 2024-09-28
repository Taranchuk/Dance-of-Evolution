using RimWorld;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
	public class JobGiver_InfectCorpses : ThinkNode_JobGiver
	{
		public override Job TryGiveJob(Pawn pawn)
		{
			var corpse = FindCorpse(pawn);
			if (corpse != null)
			{
				return JobMaker.MakeJob(DefsOf.DE_InfectingCorpse, corpse);
			}
			return null;
		}

		private Corpse FindCorpse(Pawn pawn)
		{
			return (Corpse)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map,
			ThingRequest.ForGroup(ThingRequestGroup.Corpse), PathEndMode.Touch,
			TraverseParms.For(pawn), 9999, c => CorpseValidator(c) && pawn.CanReserve(c));
		}
		
		private static bool CorpseValidator(Thing c)
		{
			if (c is Corpse corpse && corpse.IsInfected() is false)
			{
				if (corpse.GetRotStage() == RotStage.Rotting)
				{
					var pawn = corpse.InnerPawn;
					if (pawn.IsServant())
					{
						return false;
					}
					return true;
				}
			}
			return false;
		}
	}
}