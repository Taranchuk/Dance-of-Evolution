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
	public class PsychicRitualDef_CoagulateMuscleMass : PsychicRitualDef_InvocationCircle
	{
		public override List<PsychicRitualToil> CreateToils(PsychicRitual psychicRitual, PsychicRitualGraph parent)
		{
			List<PsychicRitualToil> list = base.CreateToils(psychicRitual, parent);
			var invokation = list.OfType<PsychicRitualToil_InvokeHorax>().First(); // Assuming InvokeHorax is still relevant or can be replaced
			invokation.defenderPositions.Clear();
			int num2 = 0;
			var assignments = psychicRitual.assignments;
			int num4 = assignments.RoleAssignedCount(DefenderRole);
			bool playerRitual = assignments.AllAssignedPawns.Any(x => x.Faction == Faction.OfPlayer);
			foreach (Pawn item4 in assignments.AssignedPawns(DefenderRole))
			{
				_ = item4;
				IntVec3 cell3 = assignments.Target.Cell; // Target is not used in the original ritual, might need to adjust
				cell3 += IntVec3.FromPolar(360f * (float)num2++ / (float)num4, 1.5f);
				//cell3 = GetBestStandableRolePosition(playerRitual, cell3, assignments.Target.Cell, assignments.Target.Map, 1.5f);
				invokation.defenderPositions.Add(cell3);
			}
			list.Add(new PsychicRitualToil_CoagulateMuscleMass(InvokerRole, DefenderRole)); // Removed TargetRole
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
			foreach (string issue in base.BlockingIssues(assignments, map))
			{
				yield return issue;
			}

			var pawn = assignments.FirstAssignedPawn(InvokerRole);
			if (pawn is null || !pawn.IsFungalNexus(out var fungalNexus)
				|| fungalNexus.servants.Count(x => x.IsServant(out var servant) && servant.ServantType == ServantType.Large) < 3)
			{
				yield return "DE_OnlyFungalNexusWithThreeLargeServantsCanStartThis".Translate(); // Reusing the same key, consider a new one
			}
			var defenders = assignments.AssignedPawns(DefenderRole);
			if (defenders.Count(x => x.IsServant(out var servant) && servant.masterHediff.pawn == pawn) < 3)
			{
				yield return "DE_ThreeLargeServantsRequired".Translate();
			}
		}
	}

	[HotSwappable]
	public class PsychicRitualToil_CoagulateMuscleMass : PsychicRitualToil
	{
		public PsychicRitualRoleDef invokerRole;
		public PsychicRitualRoleDef defenderRole;

		protected PsychicRitualToil_CoagulateMuscleMass()
		{
		}

		public PsychicRitualToil_CoagulateMuscleMass(PsychicRitualRoleDef invokerRole, PsychicRitualRoleDef defenderRole) // Removed targetRole
		{
			this.invokerRole = invokerRole;
			this.defenderRole = defenderRole;
		}

		public override void End(PsychicRitual psychicRitual, PsychicRitualGraph parent, bool success)
		{
			base.End(psychicRitual, parent, success);
			var invoker = psychicRitual.assignments.FirstAssignedPawn(invokerRole);
			var fungalNexus = invoker.GetFungalNexus();
			psychicRitual.ReleaseAllPawnsAndBuildings();

			// Generate Chimera
			var faction = success ? invoker.Faction : Faction.OfEntities; // Faction of the chimera
			Pawn chimera = PawnGenerator.GeneratePawn(PawnKindDefOf.Chimera, faction);
			if (success)
			{
				chimera.MakeServant(fungalNexus);
			}
			else
			{
				LordMaker.MakeNewLord(Faction.OfEntities, new LordJob_ChimeraAssault(), invoker.Map,
					new List<Pawn> { chimera });
			}

			GenSpawn.Spawn(chimera, invoker.Position, invoker.Map); // Spawn at invoker's position

			// Kill defenders (large servants)
			var defenders = psychicRitual.assignments.AssignedPawns(defenderRole).ToList(); // ToList to avoid modification during iteration
			foreach (var defender in defenders)
			{
				defender.Kill(new DamageInfo(DamageDefOf.Psychic, 99999f, 0f, -1f));
				if (defender.Corpse != null)
				{
					defender.Corpse.Destroy();
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref invokerRole, "invokerRole");
			Scribe_Defs.Look(ref defenderRole, "defenderRole");
		}
	}
}