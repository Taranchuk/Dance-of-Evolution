using Verse;
using HarmonyLib;
using RimWorld;
using Verse.AI;
using System.Collections.Generic;
using System.Reflection.Emit;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(JobDriver_Wait), "CheckForAutoAttack")]
    public static class JobDriver_Wait_CheckForAutoAttack_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            foreach (var codeInstruction in codeInstructions)
            {
                if (codeInstruction.opcode == OpCodes.Stloc_1)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(JobDriver_Wait_CheckForAutoAttack_Patch), "TryOverrideCanDoFirefight"));
                }
                yield return codeInstruction;
            }
        }

        public static bool TryOverrideCanDoFirefight(bool flag, JobDriver_Wait jobDriver)
        {
            if (jobDriver.pawn.IsServant())
            {
                return true;
            }
            return flag;
        }
    }
}