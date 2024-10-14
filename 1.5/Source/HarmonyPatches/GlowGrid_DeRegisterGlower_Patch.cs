using HarmonyLib;
using Verse;
using System.Collections.Generic;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(GlowGrid), "DeRegisterGlower")]
    public static class GlowGrid_DeRegisterGlower_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return GlowGrid_RegisterGlower_Patch.Transpiler(instructions);
        }
    }
}