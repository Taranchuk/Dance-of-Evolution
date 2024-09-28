using Verse;

namespace DanceOfEvolution
{
    public class Hediff_Stackable : HediffWithComps
	{
        public override bool TryMergeWith(Hediff other)
        {
			return false;
		}
	}
}