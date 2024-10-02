using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace DanceOfEvolution
{
	public class FungalBuilding : DefModExtension
	{
	}

	[HarmonyPatch(typeof(GenConstruct), "CanConstruct", new Type[] { typeof(Thing), typeof(Pawn), typeof(bool), typeof(bool), typeof(JobDef) })]
	public static class GenConstruct_CanConstruct_Patch
	{
		public static void Postfix(ref bool __result, Thing t, Pawn p)
		{
			if (__result)
			{
				p.CheckCanWorkOnIt(t, ref __result);
			}
		}

		public static void CheckCanWorkOnIt(this Pawn p, Thing t, ref bool __result)
		{
			var extension = (t.def.entityDefToBuild ?? t.def).GetModExtension<FungalBuilding>();
			if (extension != null)
			{
				if (p.IsServant() is false && p.HasFungalNexus() is false)
				{
					__result = false;
				}
			}
		}
	}
}
