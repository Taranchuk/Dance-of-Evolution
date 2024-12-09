using System.Linq;
using RimWorld;
using Verse;
namespace DanceOfEvolution
{
	public class PawnRenderNodeWorker_FungalNexus : PawnRenderNodeWorker_FlipWhenCrawling
	{
		public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
		{
			if (!parms.Portrait && parms.bed != null && !parms.bed.def.building.bed_showSleeperBody)
			{
				return base.CanDrawNow(node, parms);
			}
			if (parms.pawn.apparel.WornApparel.Any(x => PawnApparelGenerator.IsHeadgear(x.def)))
			{
				return false;
			}
			return base.CanDrawNow(node, parms);
		}
	}
}
