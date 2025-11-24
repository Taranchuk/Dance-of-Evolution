using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecuteWorker")]
    public static class IncidentWorker_RaidEnemy_TryExecuteWorker_Patch
    {
        public static bool Prefix(IncidentParms parms)
        {
            if (ShouldDoEnvoy(parms))
            {
                IncidentParms envoyParms = new IncidentParms
                {
                    target = parms.target,
                    faction = parms.faction
                };
                if (DefsOf.DE_MycelyssEnvoy.Worker.TryExecute(envoyParms))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool ShouldDoEnvoy(this IncidentParms parms)
        {
            return parms.faction != null && parms.faction.def == DefsOf.DE_Mycelyss && !GameComponent_CurseManager.Instance.mycelyssEnvoyEventTriggered && PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction.Any(p => p.health.hediffSet.HasHediff(DefsOf.DE_FungalNexus));
        }
    }
}
