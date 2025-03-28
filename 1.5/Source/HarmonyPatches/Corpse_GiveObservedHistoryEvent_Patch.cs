using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
	[HarmonyPatch(typeof(Corpse), nameof(Corpse.GiveObservedHistoryEvent))]
	public static class Corpse_GiveObservedHistoryEvent_Patch
	{
		public static bool Prefix(Pawn observer)
		{
			if (observer.IsFungalNexus())
			{
				return false;
			}
			return true;
		}
	}
}
