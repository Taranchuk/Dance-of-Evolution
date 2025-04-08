using Verse.AI.Group;

namespace DanceOfEvolution
{
    using System.Collections.Generic;
    using HarmonyLib;
    using Verse;
    using RimWorld;

    [HarmonyPatch(typeof(PsychicRitualDef_InvocationCircle), "GenerateRolePositions")]
    public static class PsychicRitualDef_InvocationCircle_GenerateRolePositions_Patch
    {
        public static void Postfix(PsychicRitualDef_InvocationCircle __instance, PsychicRitualRoleAssignments assignments, ref IReadOnlyDictionary<PsychicRitualRoleDef, List<IntVec3>> __result)
        {
            if (!(__instance is PsychicRitualDef_CoagulatePsychicWeight coagulateRitual))
                return;

            PsychicRitualRoleDef extraDefenderRole = coagulateRitual.extraDefenderRole;
            if (extraDefenderRole == null)
                return;

            Dictionary<PsychicRitualRoleDef, List<IntVec3>> fixedPositions = new Dictionary<PsychicRitualRoleDef, List<IntVec3>>(__result);

            int assignedCount = assignments.RoleAssignedCount(extraDefenderRole);
            if (assignedCount > 0)
            {
                if (!fixedPositions.TryGetValue(extraDefenderRole, out var positions))
                {
                    positions = new List<IntVec3>();
                    fixedPositions[extraDefenderRole] = positions;
                }
                while (positions.Count < assignedCount)
                {
                    positions.Add(assignments.Target.Cell);
                }
            }

            __result = fixedPositions;
        }
    }
}