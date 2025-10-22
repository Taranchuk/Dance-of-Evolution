using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(PsychicRitualToil_GatherForInvocation), "FallbackToil")]
    public static class PsychicRitualToil_GatherForInvocation_FallbackToil_Patch
    {
        public static void Postfix(ref PsychicRitualToil_Goto __result, PsychicRitual psychicRitual, PsychicRitualDef_InvocationCircle def, IReadOnlyDictionary<PsychicRitualRoleDef, List<IntVec3>> rolePositions)
        {
            if (psychicRitual.def == DefsOf.DE_CoagulatePower)
            {
                __result = new PsychicRitualToil_Goto(rolePositions.Slice(rolePositions.Keys.Except(def.InvokerRole).Except(def.TargetRole)));
            }
        }
    }
}