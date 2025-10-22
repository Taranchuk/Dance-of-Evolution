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
                    foreach (var gate in gates)
                    {
                        var otherMap = gate.GetOtherMap();
                        if (otherMap != null && !SettlementDefeatUtility.IsDefeated(otherMap, faction))
                        {
                            __result = false;
                            break;
                        }
                    }
                }
            }
        }
    }
}
