using HarmonyLib;
using System.Diagnostics;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(TargetInfo), MethodType.Constructor)]
    [HarmonyPatch(new[] { typeof(IntVec3), typeof(Map), typeof(bool) })]
    public static class TargetInfo_Constructor_Patch
    {
        public static void Postfix(IntVec3 cell, Map map, bool allowNullMap, ref TargetInfo __instance)
        {
            if (!allowNullMap && cell.IsValid && map == null)
            {
                string stackTrace = new StackTrace(true).ToString();
                Log.Warning($"Constructed TargetInfo with cell={cell} and a null map.\nStackTrace: {stackTrace}");
            }
        }
    }
}