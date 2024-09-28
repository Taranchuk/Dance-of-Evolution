using Verse;

namespace DanceOfEvolution
{
    public class Hediff_BladedLimb : HediffWithComps
	{
        public override bool TryMergeWith(Hediff other)
        {
			return false;
		}
	}
}