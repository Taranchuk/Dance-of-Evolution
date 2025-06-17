using HarmonyLib;
using RimWorld;
using Verse;
using System.Linq;
using System;
namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(GlowGrid), "GetAccumulatedGlowAt", new Type[] { typeof(int), typeof(bool) })]
	public static class GlowGrid_GetAccumulatedGlowAt_Patch
	{
		public static void Prefix(bool ignoreCavePlants)
		{
			GlowGrid_CombineColors_Patch.ignoreCavePlants = ignoreCavePlants;
		}

		public static void Postfix()
		{
			GlowGrid_CombineColors_Patch.ignoreCavePlants = false;
		}
	}
}