using HarmonyLib;
using RimWorld;
using Verse;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(JobGiver_Mate), "TryGiveJob")]
    public static class JobGiver_Mate_TryGiveJob_Patch
    {
        public static bool Prefix(Pawn pawn)
        {
            if (pawn.IsServant())
            {
                return false;
            }
            return true;
        }
    }
}