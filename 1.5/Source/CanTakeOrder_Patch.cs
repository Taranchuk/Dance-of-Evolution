using RimWorld;
using Verse;
using HarmonyLib;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "CanTakeOrder")]
    public static class FloatMenuMakerMap_CanTakeOrder_Patch
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            if (pawn.IsServant())
            {
                __result = true;
            }
        }
    }
}