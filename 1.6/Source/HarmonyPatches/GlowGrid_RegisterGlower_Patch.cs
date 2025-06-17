using HarmonyLib;
using Verse;
using System.Collections.Generic;
using System.Reflection.Emit;
namespace DanceOfEvolution
{
    //[HarmonyPatch(typeof(GlowGrid), "RegisterGlower")]
    public static class GlowGrid_RegisterGlower_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool flagSet = false;
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (!flagSet && instruction.opcode == OpCodes.Stloc_0)
                {
                    flagSet = true;
                    yield return new CodeInstruction(OpCodes.Ldloc_0); // Load the flag value
                    yield return new CodeInstruction(OpCodes.Ldarg_1); // Load ldarg1 value
                    yield return new CodeInstruction(OpCodes.Call, typeof(GlowGrid_RegisterGlower_Patch)
                    .GetMethod("HelperMethod")); // Call helper method
                    yield return new CodeInstruction(OpCodes.Stloc_0);  // Store updated flag value
                }
            }
        }

        public static bool HelperMethod(bool flag, CompGlower glower)
        {
            if (glower.parent.def == DefsOf.DE_Sporemaker || glower.parent.def == DefsOf.DE_HardenedSporemaker)
            {
                return true;
            }
            return flag;
        }
    }
}
