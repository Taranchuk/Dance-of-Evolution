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

		public static HediffDef DE_ServantBurrower, DE_ServantSmall, DE_ServantMedium, DE_ServantLarge,
		 DE_ServantGhoul, DE_ServantStrange;

		public static HediffDef DE_FungalTentacle;

		public static ThingDef DE_FungalSlurry;

		public static HediffDef DE_BladedLimb, DE_PiercingLimb;

		public static ThingDef DE_Gun_SpikeThrower, DE_Gun_SporeLauncher;

		public static MutantDef DE_FungalGhoul, DE_FungalGhoulSpecialized;

		public static HediffDef DE_Invisibility;
		public static ThingDef DE_FungalNode;
		public static TerrainDef DE_RottenSoil;
		public static ThingDef DE_Sporemaker, DE_HardenedSporemaker;
		public static ThingDef DE_Cerebrum, DE_HardenedCerebrum;
		public static HediffDef DE_HangingSporesMood, DE_HangingSporesConsciousness, DE_HangingSporesMoving;
		public static JobDef DE_ConsumeSpores;
		public static JobDef DE_HarvestCerebrum;
		public static HeadTypeDef TimelessOne;
		public static HediffDef DE_ClawHand;
		public static BodyPartGroupDef Arms, Shoulders;
		public static ShaderTypeDef CutoutPlant;
		public static ThingDef DE_HardenedFungusWall;
		public static HediffDef DE_PsychicCoordinatorImplant, DE_GrowthStimulatorImplant;
		public static HediffDef DE_UpgradedClawHand;
		public static HediffDef Regeneration, DE_FungalGhoulTalkable;

		public static ThingDef DE_Plant_TreeMycelial;

		public static PawnKindDef DE_MikisMetalonEfialtis;
		static DefsOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(DefsOf));
		}
	}
}