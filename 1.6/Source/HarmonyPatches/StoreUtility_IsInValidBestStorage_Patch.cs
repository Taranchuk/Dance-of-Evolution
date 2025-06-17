using HarmonyLib;
using RimWorld;
using Verse;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(StoreUtility), "IsInValidBestStorage")]
    public static class StoreUtility_IsInValidBestStorage_Patch
    {
        public static bool Prefix(Thing t, ref bool __result)
        {
            if (t.StoringThing() is Building_FungalNode)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}