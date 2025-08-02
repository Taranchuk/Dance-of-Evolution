using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    public class CompAbilityEffect_Devour : CompAbilityEffect
    {
        public new CompProperties_AbilityDevour Props => (CompProperties_AbilityDevour)props;

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            var corpse = target.Thing as Corpse;
            if (corpse is null)
            {
                return false;
            }
            if (corpse.GetRotStage() != RotStage.Fresh)
            {
                if (throwMessages)
                {
                    Messages.Message("DE_DevourAbilityInvalidTarget".Translate(), MessageTypeDefOf.RejectInput);
                }
                return false;
            }
            return base.Valid(target, throwMessages);
        }
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            var corpse = target.Thing as Corpse;
            if (corpse != null)
            {
                var job = JobMaker.MakeJob(DefsOf.DE_DevourCorpse, corpse);
                parent.pawn.jobs.TryTakeOrderedJob(job);
            }
        }
    }
}
