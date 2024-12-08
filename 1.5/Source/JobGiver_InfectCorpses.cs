using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
	public class JobGiver_InfectCorpses : ThinkNode_JobGiver
	{
		public override Job TryGiveJob(Pawn pawn)
		{
			var fungalNexus = pawn.GetServantTypeHediff()?.masterHediff?.pawn?.GetFungalNexus();
			if (fungalNexus == null) return null;

			var corpse = FindCorpse(pawn, fungalNexus, checkForTarget: true);
			if (corpse != null)
			{
				return JobMaker.MakeJob(DefsOf.DE_InfectingCorpse, corpse);
			}
			corpse = FindCorpse(pawn, fungalNexus, checkForTarget: false);
			if (corpse != null)
			{
				return JobMaker.MakeJob(DefsOf.DE_InfectingCorpse, corpse);
			}
			return null;
		}

		private Corpse FindCorpse(Pawn pawn, Hediff_FungalNexus fungalNexus, bool checkForTarget)
		{
			return (Corpse)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map,
			ThingRequest.ForGroup(ThingRequestGroup.Corpse), PathEndMode.Touch,
			TraverseParms.For(pawn), 9999, c => CorpseValidator(c, fungalNexus, checkForTarget) 
			&& pawn.CanReserve(c));
		}
		
		private static bool CorpseValidator(Thing c, Hediff_FungalNexus fungalNexus, bool checkForTarget)
		{
			if (c is Corpse corpse && !corpse.IsInfected())
			{
				if (corpse.GetRotStage() == RotStage.Rotting)
				{
					var pawn = corpse.InnerPawn;
					if (pawn.IsServant())
					{
						return false;
					}
					if (checkForTarget)
					{
						var type = InfectedCorpse.TryGetServantTypeAndHediff(pawn);
						if (type != null && type.Value.servantType == fungalNexus.servantTypeTarget)
						{
							return true;
						}
					}
					else
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}