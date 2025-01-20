using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace DanceOfEvolution
{
	[HarmonyPatch]
	public static class AttackTargetFinder_BestAttackTarget_Validator_Patch
	{
		public static MethodBase TargetMethod()
		{
			foreach (var nestedType in typeof(AttackTargetFinder).GetNestedTypes(AccessTools.all))
			{
				var methods = nestedType.GetMethods(AccessTools.all);
				var method = methods.FirstOrDefault(x => x.Name.Contains("<BestAttackTarget>")
				&& x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(IAttackTarget));
				if (method != null)
				{
					return method;
				}
			}
			return null;
		}

		public static void Postfix(ref bool __result, IAttackTarget t, Thing ___searcherThing)
		{
			if (__result)
			{
				var pawn1 = t.Thing as Pawn;
				var pawn2 = ___searcherThing as Pawn;
				__result = CheckHostility(__result, pawn1, pawn2);
			}
		}

		public static bool CheckHostility(bool __result, Pawn pawn1, Pawn pawn2)
		{
			if (__result)
			{
				if (pawn1 != null && pawn2 != null)
				{
					if (PreventHostility(pawn2, pawn1))
					{
						__result = false;
					}
					else if (PreventHostility(pawn1, pawn2))
					{
						__result = false;
					}
				}
			}
			return __result;
		}

		public static bool PreventHostility(Pawn pawn1, Pawn pawn2)
		{
			try
			{
				if (pawn1.IsServant() && pawn2.IsForbidden(pawn1))
				{
					return true;
				}
			}
			catch
			{
				
			}
			return false;
		}
	}
}