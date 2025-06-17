using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HotSwappable]
	[HarmonyPatch(typeof(Pawn), "IsColonistPlayerControlled", MethodType.Getter)]
	public static class Pawn_IsColonistPlayerControlled_Patch
	{
		[HarmonyPriority(Priority.Last)]
		public static void Postfix(Pawn __instance, ref bool __result)
		{
			if (__instance.Spawned && __instance.IsControllableServant() && __instance.Dead is false)
			{
				__result = true;
			}
		}
	}
}