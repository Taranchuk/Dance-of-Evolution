using Verse;
using HarmonyLib;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(MapPawns), "AnyPawnBlockingMapRemoval", MethodType.Getter)]
    public static class MapPawns_AnyPawnBlockingMapRemoval_Patch
    {
        public static void Postfix(ref bool __result, Map ___map)
        {
            if (!__result)
            {
                __result = ___map.listerThings.AnyThingWithDef(DefsOf.DE_QuantumTunnelGateExit) ||
                           ___map.listerThings.AnyThingWithDef(DefsOf.DE_QuantumTunnelGateExit_Spawner);
            }
        }
    }
}
