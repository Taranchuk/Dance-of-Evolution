using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(EquipmentUtility), "CanEquip",
new Type[] { typeof(Thing), typeof(Pawn), typeof(string), typeof(bool) },
new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal })]
    public static class EquipmentUtility_CanEquip_Patch
    {
        private static void Postfix(ref bool __result, Thing thing, Pawn pawn, ref string cantReason, bool checkBonded = true)
        {
            if (__result && pawn.HasFungalNexus() && PawnApparelGenerator.IsHeadgear(thing.def))
            {
                __result = false;
            }
        }
    }
}