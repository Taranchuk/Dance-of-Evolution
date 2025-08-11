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
		 DE_ServantGhoul, DE_ServantSpecial;

		public static HediffDef DE_FungalTentacle;

		public static ThingDef DE_FungalSlurry;

		public static HediffDef DE_BladedLimb;

		public static ThingDef DE_Gun_SpikeThrower, DE_Gun_SporeLauncher;

		public static MutantDef DE_FungalGhoul, DE_FungalGhoulSpecialized;
		public static HediffDef DE_Invisibility;
		public static ThingDef DE_FungalNode;
		public static TerrainDef DE_RottenSoil;
		public static ThingDef DE_Sporemaker;
		public static ThingDef DE_Cerebrum;
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
		[MayRequireOdyssey] public static HediffDef DE_VacuumResistanceImplant;

		public static ThingDef DE_Plant_TreeMycelial;

		public static PawnKindDef DE_MikisMetalonEfialtis;

		public static HediffDef DE_ConsciousnessReductionSunlight;
		public static HediffDef DE_Rotting;

		[MayRequireBiotech]
		public static ThoughtDef SunlightSensitivity_Mild;

		public static ThingDef DE_FalseParasol, DE_MyceliumTextile;
		public static DamageArmorCategoryDef Heat;
		public static ThingDef PsychoidLeaves;
		public static GameConditionDef DE_CloudmakerCondition;
		public static IncidentDef DeathPall;
		public static WorkTypeDef Cooking;
		public static WorkTypeDef BasicWorker;
		public static WorkGiverDef DoBillsCremate, DE_FeedCorpseToCerebrum;
		public static RecipeDef DE_FeedCorpse;
		public static ScenarioDef DE_FungalAwakening;
		public static PawnKindDef Noctol;
		public static PawnKindDef DE_FungalNexusKind;
		public static HediffDef DE_ServantUnstable;
		public static JobDef DE_OpenGrowthSpotDialog;
		public static ThingDef RawFungus;
		public static JobDef DE_ApplyCosmeticChange;
		public static GameConditionDef DE_CloudmakerDeathPall;

		public static ThoughtDef AteFungus_Despised, AteFungusAsIngredient_Despised;

		public static PsychicRitualRoleDef LargeServants;
		public static ThingDef DE_LivingDress;
		public static HediffDef FleshmassLung;
		public static JobDef DE_DevourCorpse;
		public static AbilityDef DE_Devour;
		public static ThingDef Filth_Fleshmass;
		public static MentalStateDef Binging_Food;
		public static MentalStateDef Tantrum;
		public static PawnKindDef Toughspike;

		static DefsOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(DefsOf));
		}
	}
}
