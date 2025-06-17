using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch]
	public static class SocialInteractionUtility_CanReceiveInteraction_Patch
	{
		private static bool ModifyIsMutantResult(bool originalResult, Pawn pawn)
		{
			if (pawn.IsServant(out var hediff) && hediff is Hediff_ServantGhoul ghoul && ghoul.specializedSkill == SkillDefOf.Social)
			{
				return false;
			}
			return originalResult;
		}

		public static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(SocialInteractionUtility), nameof(SocialInteractionUtility.CanReceiveInteraction));
			yield return AccessTools.Method(typeof(SocialInteractionUtility), nameof(SocialInteractionUtility.CanInitiateInteraction));
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var code = new List<CodeInstruction>(instructions);
			var isMutantMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));
			var modifyIsMutantResultMethod = AccessTools.Method(typeof(SocialInteractionUtility_CanReceiveInteraction_Patch), nameof(ModifyIsMutantResult));

			for (int i = 0; i < code.Count; i++)
			{
				var instruction = code[i];
				yield return instruction;

				// Look for the IsMutant method call
				if (instruction.opcode == OpCodes.Callvirt && instruction.Calls(isMutantMethod))
				{
					// Insert new instructions after IsMutant
					yield return new CodeInstruction(OpCodes.Ldarg_0); // Load Pawn (arg 0) onto the stack
					yield return new CodeInstruction(OpCodes.Call, modifyIsMutantResultMethod); // Call ModifyIsMutantResult
				}
			}
		}
	}
}
