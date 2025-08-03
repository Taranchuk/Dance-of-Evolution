using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;
namespace DanceOfEvolution
{
	[HotSwappable]
	[HarmonyPatch]
	public static class Pawn_GetDisabledWorkTypes_FillList_Patch
	{
		[HarmonyTargetMethod]
		public static MethodInfo TargetMethod()
		{
			foreach (var method in AccessTools.GetDeclaredMethods(typeof(Pawn)))
				if (method.Name.Contains("GetDisabledWorkTypes") && method.Name.Contains("FillList"))
					return method;
			return null;
		}

		public static void Postfix(Pawn __instance, List<WorkTypeDef> list)
		{
			if (UnityData.IsInMainThread)
			{
				DisableWorkTypes(__instance, list);
			}
			else
			{
				LongEventHandler.ExecuteWhenFinished(delegate
				{
					DisableWorkTypes(__instance, list);
				});
			}
		}

		private static void DisableWorkTypes(Pawn __instance, List<WorkTypeDef> list)
		{
			if (__instance.IsMutant && __instance.mutant.def == DefsOf.DE_FungalGhoulSpecialized)
			{
				var servant = __instance.GetServantTypeHediff() as Hediff_ServantGhoul;
				if (servant.specialized)
				{
					list.Clear();
					foreach (var def in DefDatabase<WorkTypeDef>.AllDefs)
					{
						if (list.Contains(def) is false && (def.relevantSkills is null || def.relevantSkills.Contains(servant.specializedSkill) is false))
						{
							list.Add(def);
						}
					}
				}
			}
		}
	}
}
