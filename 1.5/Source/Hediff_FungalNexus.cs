using RimWorld;
using Verse;
using System.Collections.Generic;

namespace DanceOfEvolution
{
	[HotSwappable]
	public class Hediff_FungalNexus : HediffWithComps
	{
		private int timer = 0;
		public List<Pawn> servants = new();
		public int MaxServants => 4;
		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			if (!pawn.Inhumanized())
			{
				pawn.health.AddHediff(HediffDefOf.Inhumanized);
			}
			servants = new();
			timer = 0;
		}

		public override void PostTick()
		{
			base.PostTick();
			if (pawn.Spawned)
			{
				timer++;
				if (timer >= GenDate.TicksPerDay * 2f)
				{
					CheckAndUpdateBurrowers();
					timer = 0;
				}
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (pawn.IsColonistPlayerControlled)
			{
				yield return new FungalNexusGizmo(this);
			}
			
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEBUG: Add Burrower",
					action = () => SpawnBurrower(),
				};
			}
		}

		public override string GetInspectString()
		{
			if (servants.Count < MaxServants)
			{
				return "DE_NextBurrowerSpawnIn".Translate(((int)((GenDate.TicksPerDay * 2f) - timer)).ToStringTicksToPeriod());
			}
			return null;
		}
		
		private void CheckAndUpdateBurrowers()
		{
			servants.RemoveAll(x => x is null || x.Destroyed || x.Dead);
			if (servants.Count < MaxServants)
			{
				SpawnBurrower();
			}
		}

		private void SpawnBurrower()
		{
			var newBurrower = PawnGenerator.GeneratePawn(DefsOf.DE_Burrower, pawn.Faction);
			servants.Add(newBurrower);
			GenSpawn.Spawn(newBurrower, pawn.Position, pawn.Map);
		}

		public bool IsImmuneTo(Hediff other)
		{
			if (HediffDefOf.LungRotExposure == other.def || HediffDefOf.LungRot == other.def)
			{
				return true;
			}
			return false;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref servants, "servants", LookMode.Reference);
			Scribe_Values.Look(ref timer, "timer");
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				servants ??= new();
			}
		}
	}
}
