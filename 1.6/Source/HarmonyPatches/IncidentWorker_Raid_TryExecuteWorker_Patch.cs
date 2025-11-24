using HarmonyLib;
using RimWorld;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(IncidentWorker_Raid), "TryExecuteWorker")]
    public static class IncidentWorker_Raid_TryExecuteWorker_Patch
    {
        public static bool Prefix(IncidentWorker_Raid __instance, IncidentParms parms, ref bool __result)
        {
            if (parms.ShouldDoEnvoy()) return true;
            if (__instance is IncidentWorker_RaidFriendly && parms.faction != null && parms.faction.def == DefsOf.DE_Mycelyss)
            {
                __result = DefsOf.DE_MycelyssRaid.Worker.TryExecute(parms);
                return false;
            }
            return true;
        }
    }
}
