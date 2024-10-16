using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[DefOf]
	public static class DefsOf
	{
		public static ThingDef DE_NexusBurgeon;
		public static HediffDef DE_FungalNexus;
		public static JobDef DE_InfectingCorpse;
		public static PawnKindDef DE_Burrower;
		
		public static HediffDef DE_ServantBurrower, DE_ServantSmall, DE_ServantMedium, DE_ServantLarge, DE_ServantGhoul, DE_ServantStrange;

		public static HediffDef DE_FungalTentacle;
		
		public static ThingDef DE_FungalSlurry;

		public static HediffDef DE_BladedLimb, DE_PiercingLimb;

		public static ThingDef DE_Gun_SpikeThrower;

		public static MutantDef DE_FungalGhoul;

		public static HediffDef DE_WraithInvisibility;
		public static ThingDef DE_FungalNode;
		public static TerrainDef DE_RottenSoil;
		public static ThingDef DE_Sporemaker, DE_HardenedSporemaker;
		public static ThingDef DE_Cerebrum, DE_HardenedCerebrum;
		public static HediffDef DE_HangingSporesMood, DE_HangingSporesConsciousness, DE_HangingSporesMoving;
		public static JobDef DE_ConsumeSpores;
		public static JobDef DE_HarvestCerebrum;
        static DefsOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(DefsOf));
		}
	}
}