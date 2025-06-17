using Verse;
using HarmonyLib;
using RimWorld;
using System.Reflection;
namespace DanceOfEvolution
{
	[HarmonyPatch]
	public static class JobGiver_Manhunter_FindPawnTarget_Patch
	{
		public static MethodBase TargetMethod()
		{
			var nestedType = typeof(JobGiver_Manhunter).GetNestedType("<>c", BindingFlags.NonPublic);
			return nestedType?.GetMethod("<FindPawnTarget>b__8_0", BindingFlags.NonPublic | BindingFlags.Instance);
		}

		public static void Postfix(Thing x, ref bool __result)
		{
			if (x is Pawn pawn && pawn.IsServant())
			{
				__result = true;
			}
		}
	}
}