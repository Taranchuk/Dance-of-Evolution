using Verse.AI.Group;
using System.Collections.Generic;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;
using System.Linq;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(PsychicRitualDef_InvocationCircle), "GenerateRolePositions")]
    public static class PsychicRitualDef_InvocationCircle_GenerateRolePositions_Patch
    {
        private const float DesiredServantRadius = 3.0f;

        public static void Postfix(PsychicRitualDef_InvocationCircle __instance, PsychicRitualRoleAssignments assignments, ref IReadOnlyDictionary<PsychicRitualRoleDef, List<IntVec3>> __result)
        {
            Dictionary<PsychicRitualRoleDef, List<IntVec3>> fixedPositions = new Dictionary<PsychicRitualRoleDef, List<IntVec3>>(__result);
            var map = assignments.Target.Map;
            var centerCell = assignments.Target.Cell;
            Room centerRoom = centerCell.GetRoom(map);
            PsychicRitualRoleDef currentInvokerRole = __instance.InvokerRole;
            List<IntVec3> invokerPositionsList = (currentInvokerRole != null && fixedPositions.TryGetValue(currentInvokerRole, out var invokerPos)) ? invokerPos : new List<IntVec3>();
            if (__instance is PsychicRitualDef_CoagulatePsychicWeight coagulateRitual && coagulateRitual.extraDefenderRole != null)
            {
                PsychicRitualRoleDef extraDefenderRole = coagulateRitual.extraDefenderRole;
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
                        positions.Add(centerCell);
                    }
                    for (int i = 0; i < positions.Count; i++)
                    {
                        positions[i] = centerCell;
                    }
                }
            }
            if (DefsOf.LargeServants != null && fixedPositions.TryGetValue(DefsOf.LargeServants, out var largeServantPositions) && largeServantPositions.Any())
            {
                Vector3 averageInvokerDirection = Vector3.right;
                if (invokerPositionsList.Any())
                {
                    Vector3 sumDirection = Vector3.zero;
                    foreach (IntVec3 pos in invokerPositionsList)
                    {
                        sumDirection += (pos - centerCell).ToVector3();
                    }
                    if (sumDirection != Vector3.zero)
                    {
                        averageInvokerDirection = sumDirection.normalized;
                    }
                }
                Vector3 oppositeDirection = -averageInvokerDirection;
                if (oppositeDirection == Vector3.zero) oppositeDirection = Vector3.left;
                Vector3 perpDirection1 = new Vector3(-oppositeDirection.z, 0, oppositeDirection.x).normalized;
                Vector3 perpDirection2 = new Vector3(oppositeDirection.z, 0, -oppositeDirection.x).normalized;
                if (perpDirection1 == Vector3.zero) perpDirection1 = Vector3.forward;
                if (perpDirection2 == Vector3.zero) perpDirection2 = Vector3.back;
                List<IntVec3> targetPositions = new List<IntVec3>();
                if (largeServantPositions.Count >= 1)
                    targetPositions.Add(centerCell + (oppositeDirection * DesiredServantRadius).ToIntVec3());
                if (largeServantPositions.Count >= 2)
                    targetPositions.Add(centerCell + (perpDirection1 * DesiredServantRadius).ToIntVec3());
                if (largeServantPositions.Count >= 3)
                    targetPositions.Add(centerCell + (perpDirection2 * DesiredServantRadius).ToIntVec3());
                HashSet<IntVec3> occupiedCells = new HashSet<IntVec3>(invokerPositionsList);

                for (int i = 0; i < largeServantPositions.Count; i++)
                {
                    if (largeServantPositions[i] == centerCell)
                    {
                        occupiedCells.Add(centerCell);
                        continue;
                    }

                    IntVec3 targetPos = (i < targetPositions.Count) ? targetPositions[i] : centerCell + (Quaternion.Euler(0, i * 15, 0) * oppositeDirection * DesiredServantRadius).ToIntVec3();
                    System.Predicate<IntVec3> validator = (IntVec3 cell) =>
                        !occupiedCells.Contains(cell) &&
                        invokerPositionsList.All(invokerPos => cell.DistanceToSquared(invokerPos) >= 4) &&
                        cell.GetRoom(map) == centerRoom;

                    IntVec3 standablePos = CellFinder.StandableCellNear(targetPos, map, DesiredServantRadius + 2f, validator);
                    if (!standablePos.IsValid)
                    {
                        standablePos = CellFinder.StandableCellNear(targetPos, map, DesiredServantRadius + 2f, (IntVec3 cell) => !occupiedCells.Contains(cell) && cell.GetRoom(map) == centerRoom);
                    }
                    if (!standablePos.IsValid)
                    {
                        standablePos = CellFinder.StandableCellNear(targetPos, map, DesiredServantRadius + 2f);
                    }
                    if (!standablePos.IsValid)
                    {
                        standablePos = largeServantPositions[i];
                    }

                    largeServantPositions[i] = standablePos;
                    occupiedCells.Add(standablePos);
                }
            }
            __result = fixedPositions;
        }
    }
}