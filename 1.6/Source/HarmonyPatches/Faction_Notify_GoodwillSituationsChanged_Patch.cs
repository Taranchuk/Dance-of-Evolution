using HarmonyLib;
using RimWorld;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Faction), "Notify_GoodwillSituationsChanged")]
    public static class Faction_Notify_GoodwillSituationsChanged_Patch
    {
        public static bool Prefix(Faction other)
        {
            if (other?.def == DefsOf.DE_Mycelyss)
            {
                return false;
            }
            return true;
        }
    }
}
