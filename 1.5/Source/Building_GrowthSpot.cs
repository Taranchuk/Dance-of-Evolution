using RimWorld;
using Verse;
using System.Collections.Generic;
using Verse.AI;

namespace DanceOfEvolution
{
    public class Building_GrowthSpot : Building
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(selPawn))
            {
                yield return floatMenuOption;
            }
            if (!selPawn.HasFungalNexus())
            {
                yield break;
            }
            if (!selPawn.CanReach(this, PathEndMode.OnCell, Danger.Deadly))
            {
                yield return new FloatMenuOption("CannotUseReason".Translate("NoPath".Translate().CapitalizeFirst()), null);
            }
            else
            {
                yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("DE_ApplyCosmetic".Translate().CapitalizeFirst(), delegate
                {
                    selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(DefsOf.DE_OpenGrowthSpotDialog, this), JobTag.Misc);
                }), selPawn, this);
            }
        }
    }
}