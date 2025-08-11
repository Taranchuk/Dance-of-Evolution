using HarmonyLib;
using RimWorld;
using Verse;
using System;
using System.Reflection;

namespace DanceOfEvolution
{
    [HarmonyPatch]
    public static class CompDecayAfterDelay_CompTick_Patch
    {
        private static Type targetType;

        public static bool Prepare()
        {
            targetType = AccessTools.TypeByName("ChickenCorpses.CompDecayAfterDelay");
            return targetType != null;
        }

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method(targetType, "Destroy");
        }

        public static bool Prefix(ThingComp __instance)
        {
            if (__instance.parent is Corpse corpse && corpse.IsInfected())
            {
                return false;
            }
            return true;
        }
    }
}
