using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    public class CompAbilityEffect_ServantInvisibility : CompAbilityEffect
    {
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn targetPawn = target.Pawn;
            Pawn casterPawn = parent.pawn;
            if (targetPawn != null && casterPawn != null)
            {
                CompRevenant revenantComp = casterPawn.TryGetComp<CompRevenant>();
                if (revenantComp != null)
                {
                    revenantComp.Invisibility.BecomeInvisible();
                    revenantComp.becomeInvisibleTick = int.MaxValue;
                }
            }
        }
    }
}