using HarmonyLib;
using RimWorld;
using Verse;
using System;
using System.Linq;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(QuestUtility), nameof(QuestUtility.TryGetIdealColonist))]
    public static class QuestUtility_TryGetIdealColonist_Patch
    {
        public static void Postfix(ref bool __result, ref Pawn pawn, Map idealMap, Func<Pawn, bool> validator)
        {
            if (!__result && idealMap != null && idealMap.mapPawns != null)
            {
                var potentialServants = idealMap.mapPawns.AllPawnsSpawned;
                foreach (var p in potentialServants)
                {
                    if (Utils.IsServant(p, out _) && (validator == null || validator(p)))
                    {
                        pawn = p;
                        __result = true;
                        return;
                    }
                }
            }
        }
    }
}