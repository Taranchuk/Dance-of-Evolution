using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(JumpUtility), "OrderJump")]
    public static class JumpUtility_OrderJump_Patch
    {
        public static bool Prefix(Pawn pawn, LocalTargetInfo target, Verb verb, float range)
        {
            if (pawn.IsServant() && verb != null
            && verb is Verb_CastAbilityJump triggeringAbility && triggeringAbility.ability.CompOfType<CompAbilityEffect_ConsumeLeap>() != null)
            {
                Job job = JobMaker.MakeJob(JobDefOf.CastJump, target);
                job.verbToUse = verb;
                pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                return false;
            }
            return true;
        }
    }
}