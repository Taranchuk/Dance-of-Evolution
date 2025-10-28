using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(CompNociosphere), "OnPassive")]

    public static class CompNociosphere_OnPassive_Patch
    {
        public static void Postfix(CompNociosphere __instance)
        {
            Pawn pawn = __instance.Pawn;
            if (pawn == null || pawn.DestroyedOrNull() || pawn.Spawned is false || pawn.IsAlly() is false) return;

            pawn.DeSpawn();
            pawn.GetLord()?.Notify_PawnLost(pawn, PawnLostCondition.ExitedMap);
            if (!pawn.DestroyedOrNull())
            {
                Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
            }
        }
    }
}
