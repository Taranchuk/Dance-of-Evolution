using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	public class Recipe_InstallNeuralEnhancer : Recipe_InstallImplant
	{
		public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
		{
			if (thing is Pawn pawn && (pawn.IsServant(out var hediff) is false || hediff is not Hediff_ServantGhoul servantGhoul || servantGhoul.specialized))
			{
				return false;
			}
			return base.AvailableOnNow(thing, part);
		}
	}

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
