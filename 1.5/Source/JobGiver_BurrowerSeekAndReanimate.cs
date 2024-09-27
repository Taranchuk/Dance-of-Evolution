using RimWorld;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
    public class JobGiver_BurrowerSeekAndReanimate : ThinkNode_JobGiver
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
			TraverseParms.For(pawn), 9999, c => CorpseValidator(c));
		}
		private static bool CorpseValidator(Thing c)
		{
			if (c is Corpse corpse && corpse.InnerPawn.GetRotStage() == RotStage.Rotting)
			{
				var pawn = corpse.InnerPawn;
				if (pawn.kindDef == DefsOf.DE_Burrower)
				{
					return false;
				}
				return true;
			}
			return false;
		}
	}
}