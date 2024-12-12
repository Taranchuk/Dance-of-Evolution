using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(GameCondition_DeathPall), "ResurrectPawn")]
	public static class GameCondition_DeathPall_Patch
	{
		public static void Postfix(GameCondition_DeathPall __instance, ref Pawn __result)
		{
			if (__instance.conditionCauser is Building_Cloudmaker building_Cloudmaker)
			{
				__result.SetFaction(building_Cloudmaker.Faction);
			}
		}
	}
}