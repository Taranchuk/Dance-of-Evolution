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

	public class Hediff_Implant_Stackable : Hediff_Implant
	{
		public override bool TryMergeWith(Hediff other)
		{
			return false;
		}
	}
}