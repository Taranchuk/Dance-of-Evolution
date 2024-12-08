using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(Thing), "TakeDamage")]
	public static class Thing_TakeDamage_Patch
	{
		public static void Postfix(Thing __instance, ref DamageInfo dinfo, DamageWorker.DamageResult __result)
		{
			if (dinfo.Instigator is Pawn attacker)
			{
				var invisiblity = attacker.health.hediffSet.GetFirstHediffOfDef(DefsOf.DE_Invisibility);
				if (invisiblity != null)
				{
					attacker.health.RemoveHediff(invisiblity);
				}
				if (attacker.kindDef == DefsOf.DE_MikisMetalonEfialtis && __instance is Pawn victim)
				{
					victim.health.AddHediff(DefsOf.DE_Rotting);
				}
			}
		}
	}
}