using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	public class ThoughtWorker_LivingDressMood : ThoughtWorker
	{
		public override ThoughtState CurrentStateInternal(Pawn p)
		{
			var livingDress = p.apparel?.WornApparel?.FirstOrDefault(a => a.def == DefsOf.DE_LivingDress);
			if (livingDress == null)
			{
				return ThoughtState.Inactive;
			}

			var comp = livingDress.GetComp<CompLivingDress>();
			if (comp?.BondedPawn != p)
			{
				return ThoughtState.Inactive;
			}

			return ThoughtState.ActiveAtStage(0);
		}
	}
}
