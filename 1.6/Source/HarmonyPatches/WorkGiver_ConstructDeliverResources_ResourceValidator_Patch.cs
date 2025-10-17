using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HotSwappable]
    [HarmonyPatch(typeof(WorkGiver_ConstructDeliverResources), "ResourceValidator")]
    public static class WorkGiver_ConstructDeliverResources_ResourceValidator_Patch
    {
        public static bool Prefix(Pawn pawn, ThingDefCountClass need, Thing th, ref bool __result)
        {
            if (need.thingDef == ThingDefOf.GoldenCube && th.def == ThingDefOf.GoldenCube)
            {
                var studyUnlocks = th.TryGetComp<CompStudyUnlocks>();
                if (studyUnlocks != null && !studyUnlocks.Completed)
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
    }
}
