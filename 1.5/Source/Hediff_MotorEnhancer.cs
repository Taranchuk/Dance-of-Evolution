using System.Linq;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	public class Recipe_InstallMotoEnhancer : Recipe_InstallImplant
	{
		public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
		{
			if (thing is Pawn pawn && pawn.HasFungalNexus() is false)
			{
				return false;
			}
			return base.AvailableOnNow(thing, part);
		}
	}

	public class Hediff_MotorEnhancer : Hediff_Implant
	{
		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			var clawHands = pawn.health.hediffSet.hediffs.Where(x => x.def == DefsOf.DE_ClawHand).ToList();
			foreach (var item in clawHands)
			{
				pawn.health.RemoveHediff(item);
			}

			var torso = pawn.RaceProps.body.corePart;
			pawn.health.AddHediff(HediffMaker.MakeHediff(DefsOf.DE_UpgradedClawHand, pawn, torso));
			pawn.health.AddHediff(HediffMaker.MakeHediff(DefsOf.DE_UpgradedClawHand, pawn, torso));
		}
	}
}
