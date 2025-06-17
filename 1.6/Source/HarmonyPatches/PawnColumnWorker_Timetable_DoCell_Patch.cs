using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;
namespace DanceOfEvolution
{
    //[HarmonyPatch(typeof(PawnColumnWorker_Timetable), "DoCell")]
    public static class PawnColumnWorker_Timetable_DoCell_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var isMutantInfo = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));
            foreach (var codeInstruction in codeInstructions)
            {
                yield return codeInstruction;
                if (codeInstruction.Calls(isMutantInfo))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(PawnColumnWorker_Timetable_DoCell_Patch),
                        "TryOverrideMutantCheck"));
                }
            }
        }

        public static bool TryOverrideMutantCheck(bool result, Pawn pawn)
        {
            if (pawn.IsWorkMutant())
            {
                return false;
            }
            return result;
        }

        public static bool IsWorkMutant(this Pawn pawn)
        {
            return pawn.mutant?.def == DefsOf.DE_FungalGhoulSpecialized;
        }
    }
}
