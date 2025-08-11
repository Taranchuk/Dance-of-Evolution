using HarmonyLib;
using Verse;
using UnityEngine;
namespace DanceOfEvolution
{
    //[HarmonyPatch(typeof(GlowGrid), "CombineColors")]
    public static class GlowGrid_CombineColors_Patch
    {
        public static bool ignoreCavePlants = false;
        public static bool Prefix(Color32 existingSum, Color32 toAdd, CompGlower toAddGlower, ref Color32 __result)
        {
            if (ignoreCavePlants)
            {
                if (toAddGlower.parent.def == DefsOf.DE_Sporemaker)
                {
                    __result = existingSum;
                    return false;
                }
            }
            return true;
        }
    }
}
