using RimWorld;
using Verse;
using HarmonyLib;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(WorkGiver_HunterHunt), "HasHuntingWeapon")]
    public static class WorkGiver_HunterHunt_HasHuntingWeapon_Patch
    {
    	public static void Postfix(ref bool __result, Pawn p)
        {
        	if (p.IsServant(out var hediff) && hediff.ServantType == ServantType.Medium)
            {
                __result = true;
            }
        }
    }
}