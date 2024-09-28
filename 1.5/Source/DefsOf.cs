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
		static DefsOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(DefsOf));
		}
	}
}