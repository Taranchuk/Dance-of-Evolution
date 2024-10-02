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
		static DefsOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(DefsOf));
		}
	}
}