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
		public ServantType servantTypeTarget = ServantType.Large;
		public HediffDef selectedCosmetic;
		public HediffStage cachedStage;
		public override HediffStage CurStage
		{
			get
			{
				if (cachedStage != null)
				{
					return cachedStage;
				}
				var baseStage = base.CurStage;
				if (baseStage == null)
				{
					return null;
				}
				var stage = baseStage.Clone();
				stage.statOffsets = stage.statOffsets.ListFullCopy();

				int mutationCount = 0;
				foreach (var hediff in pawn.health.hediffSet.hediffs)
				{
					if (hediff.def.organicAddedBodypart)
					{
						mutationCount++;
					}
				}

				if (mutationCount > 0)
				{
					float bonus = mutationCount * 0.10f;
					var sensitivityModifier = stage.statOffsets.FirstOrDefault(sm => sm.stat == StatDefOf.PsychicSensitivity);
					if (sensitivityModifier != null)
					{
						bonus += sensitivityModifier.value;
						stage.statOffsets.Remove(sensitivityModifier);
					}
					stage.statOffsets.Add(new StatModifier
					{
						stat = StatDefOf.PsychicSensitivity,
						value = bonus
					});
				}
				cachedStage = stage;
				return stage;
			}
		}

		public int TotalServantsCount
		{
			get
			{
				servants.RemoveAll(x => x is null || x.Destroyed || x.Dead);
				return servants.Sum(x => ServantSlotCount(x))
				+ GameComponent_ReanimateCorpses.Instance.infectedCorpses.Where(x => x.hediff_FungalNexus == this)
				.Sum(x => ServantSlotCount(x.corpse.InnerPawn));
			}
		}

		private static int ServantSlotCount(Pawn x)
		{
			return x.kindDef == DefsOf.DE_MikisMetalonEfialtis ? 3 : 1;
		}

		public override void Notify_Spawned()
		{
			base.Notify_Spawned();
			foreach (var item in servants)
			{
				PawnComponentsUtility.AddAndRemoveDynamicComponents(item);
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
			ApplyAppearanceSettings();
			var torso = pawn.RaceProps.body.corePart;
			pawn.health.AddHediff(HediffMaker.MakeHediff(DefsOf.DE_ClawHand, pawn, torso));
			pawn.health.AddHediff(HediffMaker.MakeHediff(DefsOf.DE_ClawHand, pawn, torso));

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
				yield return new Command_SetServantTypeTarget(this);
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

		private void ApplyAppearanceSettings()
		{
			if (pawn.story == null) return; // Safety check

			if (DanceOfEvolutionMod.settings.useTimelessHead)
			{
				if (pawn.story.headType != DefsOf.TimelessOne)
				{
					pawn.story.headType = DefsOf.TimelessOne;
					pawn.Drawer.renderer.SetAllGraphicsDirty(); // Force redraw
				}
			}

			if (DanceOfEvolutionMod.settings.showMushroomCap)
			{
				if (pawn.story.hairDef != HairDefOf.Bald)
				{
					pawn.story.hairDef = HairDefOf.Bald;
					pawn.Drawer.renderer.SetAllGraphicsDirty();
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref servants, "servants", LookMode.Reference);
			Scribe_Values.Look(ref timer, "timer");
			Scribe_Values.Look(ref servantCountOffset, "servantCountOffset");
			Scribe_Values.Look(ref servantTypeTarget, "servantTypeTarget", ServantType.Large);
			Scribe_Defs.Look(ref selectedCosmetic, "selectedCosmetic");
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				servants ??= new();
			}
		}
	}
}
