using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
	public class JobGiver_ServantWander : JobGiver_Wander
	{
		private static float ChanceToWander = 0.2f;

		public JobGiver_ServantWander()
		{
			locomotionUrgency = LocomotionUrgency.Amble;
			ticksBetweenWandersRange = new IntRange(480, 900);
		}

		public override Job TryGiveJob(Pawn pawn)
		{
			pawn.mindState.nextMoveOrderIsWait = !Rand.Chance(ChanceToWander);
			return base.TryGiveJob(pawn);
		}

		public override IntVec3 GetWanderRoot(Pawn pawn)
		{
			return pawn.Position;
		}
	}
	
	[HarmonyPatch(typeof(JobGiver_Wander), "TryGiveJob")]
	public static class JobGiver_Wander_TryGiveJob_Patch
	{
		public static bool Prefix(JobGiver_Wander __instance, Pawn pawn, ref Job __result)
		{
			if (pawn.IsServant() && __instance is not JobGiver_ServantWander)
			{
				var jobgiver = new JobGiver_ServantWander
				{
					wanderRadius = 12
				};
				var job = jobgiver.TryGiveJob(pawn);
				if (job != null)
				{
					__result = job;
					return false;
				}
			}
			return true;
		}
	}
}