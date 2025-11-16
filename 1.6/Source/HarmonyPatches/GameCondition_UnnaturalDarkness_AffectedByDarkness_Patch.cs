using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(GameCondition_UnnaturalDarkness), nameof(GameCondition_UnnaturalDarkness.AffectedByDarkness))]
    public static class GameCondition_UnnaturalDarkness_AffectedByDarkness_Patch
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            if (pawn.MapHeld != null && GameComponent_CurseManager.Instance.IsCursed(pawn.MapHeld.Parent))
            {
                if (!__result && pawn.HostileTo(Faction.OfPlayer))
                {
                    __result = true;
                }
                else if (__result && pawn.Faction == Faction.OfPlayer)
                {
                    __result = false;
                }
            }
        }
    }
}
