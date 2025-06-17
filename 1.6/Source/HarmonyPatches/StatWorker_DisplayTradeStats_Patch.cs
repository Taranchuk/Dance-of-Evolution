using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(StatWorker), nameof(StatWorker.DisplayTradeStats))]
    public static class StatWorker_DisplayTradeStats_Patch
    {
        public static void Postfix(StatRequest req, ref bool __result)
        {
            if (!__result && req.HasThing && req.Thing is Pawn pawn)
            {
                if (pawn.IsServant())
                {
                    __result = true;
                }
            }
        }
    }
}