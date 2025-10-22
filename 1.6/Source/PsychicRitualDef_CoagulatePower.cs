using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    [HotSwappable]
    public class PsychicRitualDef_CoagulatePower : PsychicRitualDef_InvocationCircle
    {
        public override List<PsychicRitualToil> CreateToils(PsychicRitual psychicRitual, PsychicRitualGraph parent)
        {
            List<PsychicRitualToil> list = base.CreateToils(psychicRitual, parent);
            list.Add(new PsychicRitualToil_CoagulatePower(InvokerRole, targetRole));
            return list;
        }

        public override IEnumerable<string> BlockingIssues(PsychicRitualRoleAssignments assignments, Map map)
        {
            foreach (string issue in base.BlockingIssues(assignments, map))
            {
                yield return issue;
            }
            Pawn pawn = assignments.FirstAssignedPawn(targetRole);
            if (pawn != null && pawn.kindDef != PawnKindDefOf.FleshmassNucleus)
            {
                yield return "DE_InvalidFleshmassNucleus".Translate();
            }
        }

        public override PsychicRitualCandidatePool FindCandidatePool()
        {
            var pool = base.FindCandidatePool();
            pool.AllCandidatePawns.AddRange(Find.CurrentMap.mapPawns.AllPawns.Where(x => x.kindDef == PawnKindDefOf.FleshmassNucleus));
            return pool;
        }

        public override Lord MakeNewLord(PsychicRitualRoleAssignments assignments)
        {
            Pawn fleshmassNucleus = assignments.FirstAssignedPawn(targetRole);
            if (fleshmassNucleus != null && fleshmassNucleus.GetLord() != null)
            {
                fleshmassNucleus.GetLord().RemovePawn(fleshmassNucleus);
            }
            return base.MakeNewLord(assignments);
        }
    }

    [HotSwappable]
    public class PsychicRitualToil_CoagulatePower : PsychicRitualToil
    {
        public PsychicRitualRoleDef invokerRole;
        public PsychicRitualRoleDef targetRole;

        protected PsychicRitualToil_CoagulatePower() { }

        public PsychicRitualToil_CoagulatePower(PsychicRitualRoleDef invokerRole, PsychicRitualRoleDef targetRole)
        {
            this.invokerRole = invokerRole;
            this.targetRole = targetRole;
        }

        public override void Start(PsychicRitual psychicRitual, PsychicRitualGraph parent)
        {
            base.Start(psychicRitual, parent);
            Pawn invoker = psychicRitual.assignments.FirstAssignedPawn(invokerRole);
            Pawn fleshmassNucleus = psychicRitual.assignments.FirstAssignedPawn(targetRole);
            bool success = Rand.Chance(psychicRitual.PowerPercent);
            if (success)
            {
                Hediff regenerationHediff = HediffMaker.MakeHediff(DefsOf.Regeneration, invoker);
                regenerationHediff.Severity = 0.5f;
                invoker.health.AddHediff(regenerationHediff);
                Messages.Message("DE_CoagulatePowerSuccess".Translate(invoker.LabelShort, invoker), invoker, MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                ApplyFailureEffects(invoker);
                Messages.Message("DE_CoagulatePowerFailure".Translate(invoker.LabelShort, invoker), invoker, MessageTypeDefOf.NegativeEvent);
            }

            fleshmassNucleus.Kill(new DamageInfo(DamageDefOf.Cut, 99999f, 0f, -1f));
            if (fleshmassNucleus.Corpse != null)
            {
                fleshmassNucleus.Corpse.Destroy();
            }
        }

        private void ApplyFailureEffects(Pawn invoker)
        {
            var allNotMissingParts = invoker.health.hediffSet.GetNotMissingParts().ToList();

            var arms = allNotMissingParts.Where(x => x.groups.Any(y => y == BodyPartGroupDefOf.LeftHand || y == BodyPartGroupDefOf.RightHand));
            if (arms.Any())
            {
                BodyPartRecord selectedArm = arms.RandomElement();
                Hediff injury = HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, invoker);
                injury.Part = selectedArm;
                invoker.health.AddHediff(injury);
            }

            var legs = allNotMissingParts.Where(x => x.groups.Any(y => y == BodyPartGroupDefOf.Legs));
            if (legs.Any())
            {
                BodyPartRecord selectedLeg = legs.RandomElement();
                Hediff injury = HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, invoker);
                injury.Part = selectedLeg;
                invoker.health.AddHediff(injury);
            }

            var lungs = allNotMissingParts.Where(x => x.def == BodyPartDefOf.Lung);
            if (lungs.Any())
            {
                var selectedLung = lungs.RandomElement();
                Hediff injury = HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, invoker);
                injury.Part = selectedLung;
                invoker.health.AddHediff(injury);
            }

            var eyes = allNotMissingParts.Where((BodyPartRecord p) => p.def == BodyPartDefOf.Eye);
            if (eyes.Any())
            {
                BodyPartRecord selectedEye = eyes.RandomElement();
                Hediff injury = HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, invoker);
                injury.Part = selectedEye;
                invoker.health.AddHediff(injury);
            }

            var carcinoma = HediffMaker.MakeHediff(HediffDefOf.Carcinoma, invoker);
            invoker.health.AddHediff(carcinoma);
        }
    
    	public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref invokerRole, "invokerRole");
            Scribe_Defs.Look(ref targetRole, "targetRole");
        }
    }
}
