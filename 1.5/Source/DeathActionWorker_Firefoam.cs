using RimWorld;
using Verse;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    public class DeathActionWorker_Firefoam : DeathActionWorker
    {
        public override RulePackDef DeathRules => RulePackDefOf.Transition_DiedExplosive;

        public override bool DangerousInMelee => true;

        public override void PawnDied(Corpse corpse, Lord prevLord)
        {
            GenExplosion.DoExplosion(corpse.Position, corpse.Map, 4.9f, DamageDefOf.Extinguish, corpse.InnerPawn);
        }
    }
}