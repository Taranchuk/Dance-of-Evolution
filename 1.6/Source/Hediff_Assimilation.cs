using Verse;

namespace DanceOfEvolution
{
    public class Hediff_Assimilation : HediffWithComps
	{
		public override void PostTick()
		{
			base.PostTick();
			if (this.Severity >= 0.99f)
			{
				this.pawn.health.AddHediff(DefsOf.DE_FungalNexus);
				this.pawn.health.RemoveHediff(this);
			}
		}
	}
}