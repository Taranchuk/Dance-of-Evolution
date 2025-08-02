using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(MentalState), "PostStart")]
    public static class MentalState_PostStart_Patch
    {
        public static void Postfix(MentalState __instance)
        {
            if ((__instance.def == DefsOf.Tantrum || __instance.def == DefsOf.Binging_Food) && __instance.pawn.apparel?.WornApparel?.Any(x => x.def == DefsOf.DE_LivingDress) == true)
            {
                __instance.forceRecoverAfterTicks = (int)(
                ((__instance.def.minTicksBeforeRecovery + __instance.def.maxTicksBeforeRecovery) / 2f) * 0.25f);
            }
        }
    }
}
