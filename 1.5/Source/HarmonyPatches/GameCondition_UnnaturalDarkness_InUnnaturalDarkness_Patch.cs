using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(GameCondition_UnnaturalDarkness), "InUnnaturalDarkness")]
    public static class GameCondition_UnnaturalDarkness_InUnnaturalDarkness_Patch
    {
        public static bool Prefix(Pawn p)
        {
            if (p.IsServant(out var hediff) && hediff.ServantType != ServantType.Ghoul)
            {
                return false;
            }
            return true;
        }
    }
}
