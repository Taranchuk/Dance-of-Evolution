using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(StorytellerUtility), nameof(StorytellerUtility.DefaultThreatPointsNow))]
    public static class StorytellerUtility_DefaultThreatPointsNow_Patch
    {
        public static float TryOverrideCombatPower(float originalCombatPower, Pawn pawn)
        {
            if (pawn != null && Utils.IsServant(pawn, out _) && pawn.RaceProps.IsAnomalyEntity && pawn.Faction == Faction.OfPlayer)
            {
                return pawn.kindDef.combatPower * 2f;
            }
            return originalCombatPower;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            FieldInfo combatPowerField = AccessTools.Field(typeof(PawnKindDef), nameof(PawnKindDef.combatPower));
            MethodInfo helperMethod = AccessTools.Method(typeof(StorytellerUtility_DefaultThreatPointsNow_Patch), nameof(TryOverrideCombatPower));
            int pawnLocalIndex = 7;
            int patchedCount = 0;

            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (instruction.LoadsField(combatPowerField))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_S, (byte)pawnLocalIndex);
                    yield return new CodeInstruction(OpCodes.Call, helperMethod);

                    patchedCount++;
                }
            }

            if (patchedCount == 0)
            {
                Log.Warning("[DanceOfEvolution] Transpiler for StorytellerUtility.DefaultThreatPointsNow did not find any PawnKindDef.combatPower loads. Combat power adjustment might not work.");
            }
            else if (patchedCount != 3)
            {
                Log.Warning($"[DanceOfEvolution] Transpiler for StorytellerUtility.DefaultThreatPointsNow found {patchedCount} PawnKindDef.combatPower loads (expected 3). Combat power adjustment might be incomplete.");
            }
        }
    }
}