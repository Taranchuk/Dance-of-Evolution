using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using System.Collections.Generic;
using RimWorld;

namespace DanceOfEvolution
{
	[HotSwappable]
	public class PsychicRitualDef_CoagulatePsychicWeight : PsychicRitualDef_InvocationCircle
	{
		public PsychicRitualRoleDef extraDefenderRole;

		public override List<PsychicRitualRoleDef> Roles
		{
			get
			{
				var list = base.Roles;
				list.Add(extraDefenderRole);
				return list;
			}
		}
		public override List<PsychicRitualToil> CreateToils(PsychicRitual psychicRitual, PsychicRitualGraph parent)
		{
			List<PsychicRitualToil> list = base.CreateToils(psychicRitual, parent);
			var invokation = list.OfType<PsychicRitualToil_InvokeHorax>().First();
			invokation.defenderPositions.Clear();
			int num2 = 0;
			var assignments = psychicRitual.assignments;
			int num4 = assignments.RoleAssignedCount(DefenderRole);
			int num5 = assignments.RoleAssignedCount(extraDefenderRole);
			bool playerRitual = assignments.AllAssignedPawns.Any(x => x.Faction == Faction.OfPlayer);
			foreach (Pawn item4 in assignments.AssignedPawns(DefenderRole))
			{
				_ = item4;
				IntVec3 cell3 = assignments.Target.Cell;
				cell3 += IntVec3.FromPolar(360f * (float)num2++ / (float)num4, 1.5f);
				//cell3 = GetBestStandableRolePosition(playerRitual, cell3, assignments.Target.Cell, assignments.Target.Map, 1.5f);
				invokation.defenderPositions.Add(cell3);
			}
			num2 = 0;
			foreach (Pawn item5 in assignments.AssignedPawns(extraDefenderRole))
			{
				_ = item5;
				IntVec3 cell3 = assignments.Target.Cell;
				cell3 += IntVec3.FromPolar(360f * (float)num2++ / (float)num5, 1.5f);
				invokation.defenderPositions.Add(cell3);
			}
			list.Add(new PsychicRitualToil_CoagulatePsychicWeight(InvokerRole, DefenderRole, extraDefenderRole));
			return list;
		}

		public override PsychicRitualCandidatePool FindCandidatePool()
		{
			var pool = base.FindCandidatePool();
			pool.AllCandidatePawns.AddRange(Find.CurrentMap.mapPawns.AllPawns.Where(x =>
			x.IsServant(out var servantType) && (servantType.ServantType == ServantType.Large
			|| x.kindDef == PawnKindDefOf.Sightstealer)));
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
				|| fungalNexus.servants.Count(x => x.IsServant(out var servant)
				&& servant.ServantType == ServantType.Large) < 3
				|| fungalNexus.servants.Count(x => x.IsServant(out var servant)
				&& servant.pawn.kindDef == PawnKindDefOf.Sightstealer) < 1)
			{
				yield return "DE_OnlyFungalNexusWithThreeLargeServantsAndSightstealerServantCanStartThis".Translate();
			}
			var defenders = assignments.AssignedPawns(DefenderRole);
			if (defenders.Count(x => x.IsServant(out var servant) && servant.masterHediff.pawn == pawn) < 3)
			{
				yield return "DE_ThreeLargeServantsRequired".Translate();
			}
			var extraDefenders = assignments.AssignedPawns(extraDefenderRole);
			if (extraDefenders.Count(x => x.IsServant(out var servant) && servant.masterHediff.pawn == pawn) < 1)
			{
				yield return "DE_SightstealerRequired".Translate();
			}
		}
	}

	[HotSwappable]
	public class PsychicRitualToil_CoagulatePsychicWeight : PsychicRitualToil
	{
		public PsychicRitualRoleDef invokerRole;
		public PsychicRitualRoleDef defenderRole;
		public PsychicRitualRoleDef extraDefenderRole;

		protected PsychicRitualToil_CoagulatePsychicWeight()
		{
		}

		public PsychicRitualToil_CoagulatePsychicWeight(PsychicRitualRoleDef invokerRole, PsychicRitualRoleDef defenderRole, PsychicRitualRoleDef extraDefenderRole)
		{
			this.invokerRole = invokerRole;
			this.defenderRole = defenderRole;
			this.extraDefenderRole = extraDefenderRole;
		}

		public override void Start(PsychicRitual psychicRitual, PsychicRitualGraph parent)
		{
			base.Start(psychicRitual, parent);
			var invoker = psychicRitual.assignments.FirstAssignedPawn(invokerRole);
			var fungalNexus = invoker.GetFungalNexus();
			psychicRitual.ReleaseAllPawnsAndBuildings();

			bool success = Rand.Chance(psychicRitual.PowerPercent);

			var faction = success ? invoker.Faction : Faction.OfEntities;
			Pawn sightstealer = psychicRitual.assignments.FirstAssignedPawn(extraDefenderRole);

			var defenders = psychicRitual.assignments.AssignedPawns(defenderRole).ToList();
			foreach (var defender in defenders)
			{
				defender.Kill(new DamageInfo(DamageDefOf.Psychic, 99999f, 0f, -1f));
				if (defender.Corpse != null)
				{
					defender.Corpse.Destroy();
				}
			}
			sightstealer.Kill(new DamageInfo(DamageDefOf.Psychic, 99999f, 0f, -1f));
			if (sightstealer.Corpse != null)
			{
				sightstealer.Corpse.Destroy();
			}

			if (success)
			{
				Pawn unstableServant = PawnGenerator.GeneratePawn(PawnKindDefOf.Revenant, faction);
				unstableServant.MakeServant(fungalNexus, DefsOf.DE_ServantUnstable);
				GenSpawn.Spawn(unstableServant, sightstealer.Position, invoker.Map);
				var compRevenant = unstableServant.TryGetComp<CompRevenant>();
				compRevenant.Invisibility.BecomeVisible();
				compRevenant.becomeInvisibleTick = int.MaxValue;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref invokerRole, "invokerRole");
			Scribe_Defs.Look(ref defenderRole, "defenderRole");
			Scribe_Defs.Look(ref extraDefenderRole, "extraDefenderRole");
		}
	}
}
