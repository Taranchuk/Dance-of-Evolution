using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(JobGiver_DeliverPawnToPsychicRitualCell), "TryGiveJob")]
    public static class JobGiver_DeliverPawnToPsychicRitualCell_TryGiveJob_Patch
    {
        public static void Postfix(Pawn pawn, ref Job __result, JobGiver_DeliverPawnToPsychicRitualCell __instance)
        {
            if (__result != null)
            {
                return;
            }

            Pawn target = pawn.mindState.duty.focusSecond.Pawn;
            if (target == null || target.kindDef != PawnKindDefOf.FleshmassNucleus)
            {
                return;
            }

            if (!(pawn.GetLord()?.LordJob is LordJob_PsychicRitual psychicRitual) || psychicRitual.def != DefsOf.DE_CoagulatePower)
            {
                return;
            }

            if (__instance.skipIfTargetCanReach && !target.Downed && !target.IsPrisoner)
            {
                LocalTargetInfo destination = pawn.mindState.duty.focusThird;
                if (!destination.IsValid || target.Position == destination.Cell)
                {
                    return;
                }
                if (!pawn.CanReach(target, PathEndMode.OnCell, PawnUtility.ResolveMaxDanger(pawn, __instance.maxDanger)))
                {
                    return;
                }
                Job job = JobMaker.MakeJob(JobDefOf.DeliverToCell, target, destination);
                job.count = 1;
                job.locomotionUrgency = PawnUtility.ResolveLocomotion(pawn, __instance.locomotionUrgency);
                job.expiryInterval = __instance.jobMaxDuration;
                __result = job;
            }
        }
    }
}