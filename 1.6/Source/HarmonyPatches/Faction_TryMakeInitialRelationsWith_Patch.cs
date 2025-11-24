using HarmonyLib;
using RimWorld;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Faction), nameof(Faction.TryMakeInitialRelationsWith))]
    public static class Faction_TryMakeInitialRelationsWith_Patch
    {
        public static void Prefix()
        {
            DefsOf.DE_Mycelyss.permanentEnemy = true;
        }

        public static void Postfix()
        {
            DefsOf.DE_Mycelyss.permanentEnemy = false;
        }
    }
}
