using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
namespace DanceOfEvolution
{
	[HarmonyPatch]
	public static class CaravanUIUtility_AddPawnsSections_Patch
	{
		public static MethodBase TargetMethod()
		{
			foreach (var nestedType in typeof(CaravanUIUtility).GetNestedTypes(AccessTools.all))
			{
				var methods = nestedType.GetMethods(AccessTools.all);
				var method = methods.FirstOrDefault(x => x.Name.Contains("<AddPawnsSections>b__8_7"));
				if (method != null)
				{
					return method;
				}
			}
			return null;
		}
		
		public static void Postfix(TransferableOneWay x, ref bool __result)
		{
			if (!__result && x.AnyThing is Pawn pawn && pawn.IsServantEntity())
			{
				__result = true;
			}
		}
	}
}