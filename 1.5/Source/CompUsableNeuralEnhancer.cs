using RimWorld;
using Verse;

namespace DanceOfEvolution
{

	public class CompUsableNeuralEnhancer : CompUsableImplant
	{
		public override AcceptanceReport CanBeUsedBy(Pawn p, bool forced = false, bool ignoreReserveAndReachable = false)
		{
			if (p.IsServant(out var hediff) is false || hediff is not Hediff_ServantGhoul servantGhoul || servantGhoul.specialized)
			{
				return "DE_OnlyNonSpecializedFungalGhoulsCanUseIt".Translate();
			}
			return base.CanBeUsedBy(p, forced, ignoreReserveAndReachable);
		}
	}
}