using HarmonyLib;
using RimWorld;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(ColonistBar), "CheckRecacheEntries")]
    public static class ColonistBar_CheckRecacheEntries_Patch
    {
        public static bool recachingNow;
        public static void Prefix()
        {
            recachingNow = true;
        }

        public static void Postfix()
        {
            recachingNow = false;
        }
    }
}