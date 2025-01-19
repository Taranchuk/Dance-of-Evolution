using HarmonyLib;
using Verse;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_PawnSpawned")]
    public static class Pawn_EquipmentTracker_DropAllEquipment
    {
        public static bool Prefix(Pawn_EquipmentTracker __instance)
        {
            if (__instance.pawn.IsServant(out var servantType) && servantType.ServantType == ServantType.Medium)
            {
                return false;
            }
            return true;
        }
    }
}