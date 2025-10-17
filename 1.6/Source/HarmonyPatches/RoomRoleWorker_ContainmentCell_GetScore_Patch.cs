using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(RoomRoleWorker_ContainmentCell), "GetScore")]
    public static class RoomRoleWorker_ContainmentCell_GetScore_Patch
    {
        public static void Postfix(Room room, ref float __result)
        {
            var containedAndAdjacentThings = room.ContainedAndAdjacentThings;
            for (int i = 0; i < containedAndAdjacentThings.Count; i++)
            {
                Thing thing = containedAndAdjacentThings[i];
                if (thing.def == DefsOf.DE_CubeFungi)
                {
                    __result += 50f;
                    break;
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(RoomRoleWorker_ContainmentCell), "GetScoreDeltaIfBuildingPlaced")]
    public static class RoomRoleWorker_ContainmentCell_GetScoreDeltaIfBuildingPlaced_Patch
    {
        public static void Postfix(ThingDef buildingDef, ref float __result)
        {
            if (buildingDef == DefsOf.DE_CubeFungi)
            {
                __result = 50f;
            }
        }
    }
}