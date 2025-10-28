using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace DanceOfEvolution
{
    [HotSwappable]
    [HarmonyPatch(typeof(SettlementDefeatUtility), "IsDefeated")]
    public static class SettlementDefeatUtility_IsDefeated_Patch
    {
        public static void Postfix(Map map, Faction faction, ref bool __result)
        {
            if (__result)
            {
                if (map.listerThings.AnyThingWithDef(DefsOf.DE_QuantumTunnelGateExit_Spawner))
                {
                    __result = false;
                }
                else
                {
                    var gates = map.listerThings.ThingsOfDef(DefsOf.DE_QuantumTunnelGateExit).OfType<Building_QuantumTunnelGateExit>();
                    if (gates.Any() && map.mapPawns.FreeColonists.Any() is false)
                    {
                        __result = false;
                    }
                }
            }
        }
    }
}
