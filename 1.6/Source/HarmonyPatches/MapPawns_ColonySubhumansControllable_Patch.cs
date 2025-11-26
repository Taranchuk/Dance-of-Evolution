using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(MapPawns), "ColonySubhumansControllable", MethodType.Getter)]
    public static class MapPawns_ColonySubhumansControllable_Patch
    {
        public static void Postfix(MapPawns __instance, ref List<Pawn> __result)
        {
            if (ColonistBar_CheckRecacheEntries_Patch.recachingNow)
            {
                return;
            }
            __result = __result.Concat(__instance.PawnsInFaction(Faction.OfPlayer).Where(x => x.IsServant())).Distinct().ToList();
        }
    }
}
