using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    public class CompProperties_SpawnBurrower : CompProperties_AbilityEffect
    {
        public CompProperties_SpawnBurrower()
        {
            compClass = typeof(CompAbilityEffect_SpawnBurrower);
        }
    }

    public class CompAbilityEffect_SpawnBurrower : CompAbilityEffect
    {
        public new CompProperties_SpawnBurrower Props => (CompProperties_SpawnBurrower)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            var pawn = parent.pawn;
            var newBurrower = PawnGenerator.GeneratePawn(DefsOf.DE_Burrower, pawn.Faction);
            GenSpawn.Spawn(newBurrower, pawn.Position, pawn.Map);
            newBurrower.MakeServant(pawn.GetFungalNexus(), DefsOf.DE_ServantBurrower);
        }
    }
}