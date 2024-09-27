using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	public class Hediff_FungalNexus : HediffWithComps
	{
		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo); 
			if (!pawn.Inhumanized())
			{
				pawn.health.AddHediff(HediffDefOf.Inhumanized);
			}
		}
		public bool IsImmuneTo(Hediff other)
		{
			if (HediffDefOf.LungRotExposure == other.def 
				|| HediffDefOf.LungRot == other.def)
			{
				return true;
			}
			return false;
		}
	}
}
