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
	public class PsychicRitualDef_CoagulateConciousness : PsychicRitualDef_InvocationCircle
	{
		public override List<PsychicRitualToil> CreateToils(PsychicRitual psychicRitual, PsychicRitualGraph parent)
		{
			List<PsychicRitualToil> list = base.CreateToils(psychicRitual, parent);
			var invokation = list.OfType<PsychicRitualToil_InvokeHorax>().First();
			invokation.defenderPositions.Clear();
			var num2 = 0;
			var assignments = psychicRitual.assignments;
			int num4 = assignments.RoleAssignedCount(DefenderRole);
			bool playerRitual = assignments.AllAssignedPawns.Any((Pawn x) => x.Faction == Faction.OfPlayer);
			foreach (Pawn item4 in assignments.AssignedPawns(DefenderRole))
			{
				_ = item4;
				IntVec3 cell3 = assignments.Target.Cell;
				cell3 += IntVec3.FromPolar(360f * (float)num2++ / (float)num4, 1.5f);
				//cell3 = GetBestStandableRolePosition(playerRitual, cell3, assignments.Target.Cell, assignments.Target.Map, 1.5f);
				invokation.defenderPositions.Add(cell3);
			}
			list.Add(new PsychicRitualToil_CreateSpecialServant(InvokerRole, TargetRole, DefenderRole));
			return list;
		}

		public override PsychicRitualCandidatePool FindCandidatePool()
		{
			var pool = base.FindCandidatePool();
			pool.AllCandidatePawns.AddRange(Find.CurrentMap.mapPawns.SpawnedColonyAnimals.Where(x => x.IsServant(out var servantType) && servantType.ServantType == ServantType.Large));
			return pool;
		}
		
		public override IEnumerable<string> BlockingIssues(PsychicRitualRoleAssignments assignments, Map map)
		{
			using (new ProfilerBlock("PsychicRitualDef.BlockingIssues"))
			{
				var pawn = assignments.FirstAssignedPawn(InvokerRole);
				if (pawn is null || pawn.IsFungalNexus(out var fungalNexus) is false 
				|| fungalNexus.servants.Count(x => x.IsServant(out var servant) && servant.ServantType == ServantType.Large) < 3)
				{
					yield return "DE_OnlyFungalNexusWithThreeLargeServantsCanStartThis".Translate();
				}
				var defenders = assignments.AssignedPawns(DefenderRole);
				if (defenders.Count(x => x.IsServant(out var servant) && servant.masterHediff.pawn == pawn) < 3)
				{
					yield return "DE_ThreeLargeServantsRequired".Translate();
				}
			}
		}
	}

	[HotSwappable]
	public class PsychicRitualToil_CreateSpecialServant : PsychicRitualToil
	{
		public PsychicRitualRoleDef invokerRole, targetRole;
		public PsychicRitualRoleDef defenderRole;
		protected PsychicRitualToil_CreateSpecialServant()
		{
		}

		public PsychicRitualToil_CreateSpecialServant(PsychicRitualRoleDef invokerRole, 
		PsychicRitualRoleDef targetRole, PsychicRitualRoleDef defenderRole)
		{
			this.invokerRole = invokerRole;
			this.targetRole = targetRole;
			this.defenderRole = defenderRole;
		}
		public override void Start(PsychicRitual psychicRitual, PsychicRitualGraph parent)
		{
			base.Start(psychicRitual, parent);
			Pawn target = psychicRitual.assignments.FirstAssignedPawn(targetRole);
			var invoker = psychicRitual.assignments.FirstAssignedPawn(invokerRole);
			var fungalNexus = invoker.GetFungalNexus();
			psychicRitual.ReleaseAllPawnsAndBuildings();
			
			var spider = PawnGenerator.GeneratePawn(DefsOf.DE_MikisMetalonEfialtis, invoker.Faction);
			GenSpawn.Spawn(spider, target.Position, target.Map);
			spider.MakeServant(fungalNexus, DefsOf.DE_ServantSpecial);
			spider.equipment.AddEquipment(ThingMaker.MakeThing(DefsOf.DE_Gun_SporeLauncher) as ThingWithComps);

			var victims = new List<Pawn> { target };
			victims.AddRange(psychicRitual.assignments.AssignedPawns(defenderRole));
			foreach (var victim in victims)
			{
				victim.Kill(new DamageInfo(DamageDefOf.Psychic, 99999f, 0f, -1f));
				if (victim.Corpse != null)
				{
					victim.Corpse.Destroy();
				}
			}
		}


		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref invokerRole, "invokerRole");
			Scribe_Defs.Look(ref targetRole, "targetRole");
			Scribe_Defs.Look(ref defenderRole, "defenderRole");
		}
	}
}
