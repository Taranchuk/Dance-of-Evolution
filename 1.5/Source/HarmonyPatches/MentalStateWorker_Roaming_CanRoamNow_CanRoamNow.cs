using HarmonyLib;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(MentalStateWorker_Roaming), "CanRoamNow")]
    public static class MentalStateWorker_Roaming_CanRoamNow_CanRoamNow
    {
        public static void Postfix(ref bool __result, Pawn pawn)
        {
            if (pawn.IsServant())
            {
                __result = false;
            }
        }
    }
}
