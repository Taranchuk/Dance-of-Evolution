using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace DanceOfEvolution
{

	[HotSwappable]
	public class Hediff_FungalNexus : HediffWithComps
	{
		private float timer = 0;
		public List<Pawn> servants = new();
		public int MaxServants => 4 + servantCountOffset;
		public int servantCountOffset;
		public int TotalServantsCount
		{
			get
			{
				servants.RemoveAll(x => x is null || x.Destroyed || x.Dead);
				return servants.Count + GameComponent_ReanimateCorpses.Instance.infectedCorpses.Where(x => x.hediff_FungalNexus == this).Count();
			}
		}
		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			if (!pawn.Inhumanized())
			{
				pawn.health.AddHediff(HediffDefOf.Inhumanized);
			}
			servants = new();
			timer = 0;
			pawn.story.headType = DefsOf.TimelessOne;
			pawn.story.hairDef = HairDefOf.Bald;

			var arms = pawn.health.hediffSet.GetNotMissingParts().Where(x => x.IsInGroup(DefsOf.Arms) && x.parent.IsInGroup(DefsOf.Shoulders)).ToList();
			foreach (var arm in arms)
			{
				pawn.health.AddHediff(HediffMaker.MakeHediff(DefsOf.DE_ClawHand, pawn, arm));
			}
		}

		public override void PostTick()
		{
			base.PostTick();
			if (pawn.Spawned)
			{
				var burrowerSpawnSpeed = 1f;
				var coordinator = pawn.health.hediffSet.GetFirstHediffOfDef(DefsOf.DE_PsychicCoordinatorImplant) as Hediff_Level;
				if (coordinator != null)
				{
					burrowerSpawnSpeed *= 1 + (coordinator.level * 0.5f);
				}
				timer += burrowerSpawnSpeed;
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
			if (TotalServantsCount < MaxServants)
			{
				var burrowerSpawnSpeed = 1f;
				var coordinator = pawn.health.hediffSet.GetFirstHediffOfDef(DefsOf.DE_PsychicCoordinatorImplant) as Hediff_Level;
				if (coordinator != null)
				{
					burrowerSpawnSpeed *= 1 + (coordinator.level * 0.5f);
				}

				// Use the same formula as in PostTick for total spawn time
				float spawnInterval = GenDate.TicksPerDay * 2f;
				float remainingTime = spawnInterval - timer;

				// Adjust remaining time by spawn speed
				remainingTime /= burrowerSpawnSpeed;

				// Ensure non-negative value
				if (remainingTime < 0)
				{
					remainingTime = 0;
				}
				return "DE_NextBurrowerSpawnIn".Translate(((int)remainingTime).ToStringTicksToPeriod());
			}

			return null;
		}


		private void CheckAndUpdateBurrowers()
		{
			if (TotalServantsCount < MaxServants)
			{
				SpawnBurrower();
			}
		}

		private void SpawnBurrower()
		{
			var newBurrower = PawnGenerator.GeneratePawn(DefsOf.DE_Burrower, pawn.Faction);
			GenSpawn.Spawn(newBurrower, pawn.Position, pawn.Map);
			newBurrower.MakeServant(this, DefsOf.DE_ServantBurrower);
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
			Scribe_Values.Look(ref servantCountOffset, "servantCountOffset");
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				servants ??= new();
			}
		}
	}
}
