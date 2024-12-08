using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(GasUtility), "IsEffectedByExposure")]
    public static class GasUtility_IsEffectedByExposure_Patch
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
