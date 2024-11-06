using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(ApparelProperties), "PawnCanWear", new Type[] { typeof(Pawn), typeof(bool) })]
    public static class ApparelProperties_PawnCanWear_Patch
    {
        public static void Postfix(Apparel __instance, Pawn pawn, ref bool __result)
        {
            if (pawn.HasFungalNexus() && PawnApparelGenerator.IsHeadgear(__instance.def))
            {
                __result = false;
            }
        }
    }
}