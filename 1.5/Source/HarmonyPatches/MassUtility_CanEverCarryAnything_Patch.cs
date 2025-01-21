using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(MassUtility), "CanEverCarryAnything")]
    public static class MassUtility_CanEverCarryAnything_Patch
    {
        public static void Postfix(Pawn p, ref bool __result)
        {
            if (__result is false && p.IsServant())
            {
                __result = true;
            }
        }
    }
}