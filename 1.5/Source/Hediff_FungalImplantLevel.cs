using Verse;

namespace DanceOfEvolution
{
	public class Hediff_FungalImplantLevel : Hediff_Level
	{
		public override void ChangeLevel(int levelOffset)
		{
			base.ChangeLevel(levelOffset);
			var nexus = pawn.GetFungalNexus();
			foreach (var servant in nexus.servants)
			{
				var servantHediff = servant.GetServantTypeHediff();
				servantHediff.SetupStage();
			}
		}
	}
}
