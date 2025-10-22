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
    public class PsychicRitualDef_ShardDuplication : PsychicRitualDef_InvocationCircle
    {

        public override List<PsychicRitualToil> CreateToils(PsychicRitual psychicRitual, PsychicRitualGraph parent)
        {
            var list = base.CreateToils(psychicRitual, parent);
            list.Add(new PsychicRitualToil_ShardDuplication(InvokerRole, targetRole));
            return list;
        }

        public override IEnumerable<string> BlockingIssues(PsychicRitualRoleAssignments assignments, Map map)
        {
            foreach (var issue in base.BlockingIssues(assignments, map))
            {
                yield return issue;
            }
            var targetPawn = assignments.FirstAssignedPawn(targetRole);
            if (targetPawn != null && targetPawn.GetStatValue(StatDefOf.PsychicSensitivity) <= 0f)
            {
                yield return "DE_RitualTargetPsychicallyDeaf".Translate(targetPawn.Named("PAWN"));
            }
        }
    }

    [HotSwappable]
    public class PsychicRitualToil_ShardDuplication : PsychicRitualToil
    {
        public PsychicRitualRoleDef invokerRole;
        public PsychicRitualRoleDef targetRole;

        private const float QualityThresholdForSuccess = 0.2f;

        public PsychicRitualToil_ShardDuplication() { }

        public PsychicRitualToil_ShardDuplication(PsychicRitualRoleDef invokerRole, PsychicRitualRoleDef targetRole)
        {
            this.invokerRole = invokerRole;
            this.targetRole = targetRole;
        }

        public override void Start(PsychicRitual psychicRitual, PsychicRitualGraph parent)
        {
            base.Start(psychicRitual, parent);

            var targetPawn = psychicRitual.assignments.FirstAssignedPawn(targetRole);
            if (targetPawn == null) return;

            var map = psychicRitual.assignments.Target.Map;
            var cell = targetPawn.Position;
            var filthDef = DefDatabase<ThingDef>.GetNamed("Filth_Fleshmass");
            var shardDef = ThingDefOf.Shard;
            bool success = Rand.Chance(psychicRitual.PowerPercent);

            if (success && psychicRitual.PowerPercent >= QualityThresholdForSuccess)
            {
                int baseYield = 1;
                int sensitivityBonus = (targetPawn.GetStatValue(StatDefOf.PsychicSensitivity) >= 1.2f) ? 1 : 0;
                int qualityBonus = Mathf.FloorToInt(psychicRitual.PowerPercent / 0.2f);
                int totalYield = baseYield + sensitivityBonus + qualityBonus;

                targetPawn.Kill(null, null);
                targetPawn.Corpse?.Destroy();

                FilthMaker.TryMakeFilth(cell, map, filthDef, targetPawn.LabelIndefinite(), 3);

                if (totalYield > 0)
                {
                    var shard = ThingMaker.MakeThing(shardDef);
                    shard.stackCount = totalYield;
                    GenSpawn.Spawn(shard, cell, map, WipeMode.Vanish);
                }
            }
            else
            {
                targetPawn.Kill(null, null);
                targetPawn.Corpse?.Destroy();
                FilthMaker.TryMakeFilth(cell, map, filthDef, targetPawn.LabelIndefinite(), 1);
            }

            psychicRitual.ReleaseAllPawnsAndBuildings();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref invokerRole, "invokerRole");
            Scribe_Defs.Look(ref targetRole, "targetRole");
        }
    }
}
