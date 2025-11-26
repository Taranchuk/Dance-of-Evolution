using HarmonyLib;
using RimWorld;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(IncidentWorker_RaidFriendly), "TryResolveRaidFaction")]
    public static class IncidentWorker_RaidFriendly_TryResolveRaidFaction_Patch
    {
        public static void Postfix(IncidentParms parms, ref bool __result)
        {
            if (__result && parms.faction != null && parms.faction.def == DefsOf.DE_Mycelyss && DefsOf.DE_MycelyssRaid.Worker.TryExecute(parms))
            {
                __result = false;
            }
        }
    }
}
