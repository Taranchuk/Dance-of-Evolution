using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(GasUtility), "IsAffectedByExposure")]
    public static class GasUtility_IsAffectedByExposure_Patch
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
