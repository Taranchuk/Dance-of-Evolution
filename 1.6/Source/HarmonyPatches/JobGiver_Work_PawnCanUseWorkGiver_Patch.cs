using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HotSwappable]
    [HarmonyPatch(typeof(JobGiver_Work), "PawnCanUseWorkGiver")]
    public static class JobGiver_Work_PawnCanUseWorkGiver_Patch
    {
        public static void Prefix(Pawn pawn, WorkGiver giver, out bool __state)
        {
            __state = giver.def.nonColonistsCanDo;
            if (pawn.IsControllableServant())
            {
                giver.def.nonColonistsCanDo = true;
            }
        }

        public static void Postfix(Pawn pawn, WorkGiver giver, bool __state, bool __result)
        {
            giver.def.nonColonistsCanDo = __state;
        }
    }
}
