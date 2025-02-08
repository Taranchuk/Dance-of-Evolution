using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Pawn), "HealthScale", MethodType.Getter)]
    public static class Pawn_HealthScale_Patch
    {
        public static void Postfix(ref float __result, Pawn __instance)
        {
            if (__instance.kindDef == PawnKindDefOf.Revenant && __instance.IsServant())
            {
                __result *= 0.5f;
            }
        }
    }
}