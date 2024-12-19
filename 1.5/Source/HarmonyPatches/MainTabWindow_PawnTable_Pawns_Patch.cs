using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(MainTabWindow_PawnTable), "Pawns", MethodType.Getter)]
    public static class MainTabWindow_PawnTable_Pawns_Patch
    {
        public static IEnumerable<Pawn> Postfix(IEnumerable<Pawn> __result, MainTabWindow_PawnTable __instance)
        {
            if (__instance is not MainTabWindow_Schedule)
            {
                foreach (var p in __result.Concat(Find.CurrentMap.mapPawns.SpawnedColonyMutantsPlayerControlled
                    .Where(x => x.IsWorkMutant()).Distinct()))
                {
                    yield return p;
                }
            }
            else
            {
                foreach (var p in __result)
                {
                    yield return p;
                }
            }
        }
    }
}