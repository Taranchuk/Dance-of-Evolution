using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Pawn_TrainingTracker), "TrainingTrackerTickRare")]
    public static class Pawn_TrainingTracker_TrainingTrackerTickRare_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        {
            var shouldSkip = AccessTools.Method(typeof(Utils), nameof(Utils.IsServant), new Type[] { typeof(Pawn) });
            var pawnField = AccessTools.Field(typeof(Pawn_TrainingTracker), "pawn");
            var animalTypeField = AccessTools.Field(typeof(RaceProperties), "animalType");

            var codes = instructions.ToList();
            var label = ilg.DefineLabel();

            bool patched = false;
            for (int i = 0; i < codes.Count; i++)
            {
                var instr = codes[i];
                if (!patched && i > 5 && codes[i - 4].LoadsField(animalTypeField) && codes[i - 2].opcode == OpCodes.Bne_Un_S && codes[i - 1].opcode == OpCodes.Ret && codes[i].opcode == OpCodes.Ldarg_0)
                {
                    patched = true;
                    yield return new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(instr);
                    instr.labels.Add(label);
                    yield return new CodeInstruction(OpCodes.Ldfld, pawnField);
                    yield return new CodeInstruction(OpCodes.Call, shouldSkip);
                    yield return new CodeInstruction(OpCodes.Brfalse_S, label);
                    yield return new CodeInstruction(OpCodes.Ret);
                }
                yield return instr;
            }

            if (!patched)
            {
                Log.Error("[DanceOfEvolution] Pawn_TrainingTracker:TrainingTrackerTickRare Transpiler failed");
            }
        }
    }
}
