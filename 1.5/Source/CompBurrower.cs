using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	public class CompBurrower : ThingComp
	{
		private int lifeSpan = 60000; // 1 day in ticks
		public override void CompTick()
		{
			base.CompTick();
			lifeSpan--;
			if (lifeSpan <= 0)
			{
				this.parent.Destroy();
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref lifeSpan, "lifeSpan");
		}
	}
}