using System.Collections.Generic;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	public class ThoughtWorker_UncomfortableOutfit : ThoughtWorker
	{
		public override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (p.Inhumanized())
			{
				return ThoughtState.Inactive;
			}
			List<Apparel> wornApparel = p.apparel.WornApparel;
			for (int i = 0; i < wornApparel.Count; i++)
			{
				if (wornApparel[i].Stuff == DefsOf.DE_MyceliumTextile)
				{
					return ThoughtState.ActiveAtStage(0);
				}
			}
			return ThoughtState.Inactive;
		}
	}
}