using HarmonyLib;
using RimWorld;
using Verse;
using System;
using System.Reflection;

namespace DanceOfEvolution
{
    [HarmonyPatch]
    public static class CompUntameable_CheckFaction_Patch
    {
        private static Type targetType;

        public static bool Prepare()
        {
            targetType = AccessTools.TypeByName("VEF.AnimalBehaviours.CompUntameable");
            return targetType != null;
        }

        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method(targetType, "CheckFaction");
        }

        public static bool Prefix(ThingComp __instance)
        {
            Pawn pawn = __instance.parent as Pawn;
            if (pawn.IsServant())
            {
                return false;
            }
            return true;
        }
    }
}
