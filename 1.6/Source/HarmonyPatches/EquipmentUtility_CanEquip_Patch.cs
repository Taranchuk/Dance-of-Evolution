using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(EquipmentUtility), "CanEquip", [typeof(Thing), typeof(Pawn), typeof(string), typeof(bool)],
    [ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal])]
    public static class EquipmentUtility_CanEquip_Patch
    {
        private static void Postfix(ref bool __result, Thing thing, Pawn pawn, ref string cantReason, bool checkBonded = true)
        {
            if (pawn.apparel != null && thing?.def == DefsOf.DE_LivingDress)
            {
                var comp = thing.TryGetComp<CompLivingDress>();
                if (comp?.BondedPawn != null)
                {
                    __result = false;
                    cantReason = "DE_LivingDressAlreadyBonded".Translate();
                }
            }
        }
    }
}
