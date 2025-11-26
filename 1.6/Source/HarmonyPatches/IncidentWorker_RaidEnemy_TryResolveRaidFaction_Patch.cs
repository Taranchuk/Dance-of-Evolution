using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryResolveRaidFaction")]
    public static class IncidentWorker_RaidEnemy_TryResolveRaidFaction_Patch
    {
        public static void Postfix(IncidentParms parms, ref bool __result)
        {
            if (__result && ShouldDoEnvoy(parms))
            {
                IncidentParms envoyParms = new IncidentParms
                {
                    target = parms.target,
                    faction = parms.faction
                };
                if (DefsOf.DE_MycelyssEnvoy.Worker.TryExecute(envoyParms))
                {
                    __result = false;
                }
            }
        }

        public static bool ShouldDoEnvoy(this IncidentParms parms)
        {
            return parms.faction != null && parms.faction.def == DefsOf.DE_Mycelyss && !GameComponent_CurseManager.Instance.mycelyssEnvoyEventTriggered && PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction.Any(p => p.health.hediffSet.HasHediff(DefsOf.DE_FungalNexus));
        }
    }
}
