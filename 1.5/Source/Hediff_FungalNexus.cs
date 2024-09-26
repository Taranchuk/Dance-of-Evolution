using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	public class Hediff_FungalNexus : HediffWithComps
	{
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
