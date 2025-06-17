using RimWorld;
using Verse;
using HarmonyLib;
namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(WorkGiver_HunterHunt), "HasShieldAndRangedWeapon")]
	public static class WorkGiver_HunterHunt_HasShieldAndRangedWeapon_Patch
	{
		public static bool Prefix(ref bool __result, Pawn p)
		{
			if (p.IsServant(out var hediff) && hediff.ServantType == ServantType.Medium)
			{
				__result = false;
				return false;
			}
			return true;
		}
	}
}