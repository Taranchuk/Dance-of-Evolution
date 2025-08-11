using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(FloatMenuOptionProvider_Ingest), "GetSingleOptionFor")]
    public static class FloatMenuOptionProvider_Ingest_GetSingleOptionFor_Patch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(ref FloatMenuOption __result, Thing clickedThing, FloatMenuContext context)
        {
            if (context.FirstSelectedPawn.IsServant() && clickedThing.def != DefsOf.DE_FungalSlurry)
            {
                __result = null;
                return false;
            }
            return true;
        }
    }
}
