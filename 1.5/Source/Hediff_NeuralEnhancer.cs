using Verse;

namespace DanceOfEvolution
{
	public class Hediff_NeuralEnhancer : Hediff_Implant
	{
		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			var servant = pawn.GetServantTypeHediff() as Hediff_ServantGhoul;
			servant.Specialize();
		}
	}
}
