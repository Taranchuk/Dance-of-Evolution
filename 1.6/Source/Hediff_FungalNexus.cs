using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.Sound;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DanceOfEvolution
{

	[HotSwappable]
	public class Hediff_FungalNexus : HediffWithComps
	{
		private float timer = 0;
		public List<Pawn> servants = new();
		private int lastCurseUseTick = -1;
		private const int BaseCooldownTicks = 200 * GenDate.TicksPerDay;
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
				var command = new Command_ActionWithCooldown
				{
					defaultLabel = "DE_Curse".Translate(),
					defaultDesc = "DE_CurseDesc".Translate(),
					icon = ContentFinder<Texture2D>.Get("UI/Abilities/Curse", true),
					cooldownPercentGetter = () => GetCurseCooldownPercent(),
					action = () => StartCurseTargeting()
				};
				if (GetCurseCooldownPercent() < 1f)
				{
					command.Disable("DE_CurseOnCooldown".Translate());
				}
				yield return command;
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
				float spawnInterval = GenDate.TicksPerDay * 2f;
				float remainingTime = spawnInterval - timer;
				remainingTime /= burrowerSpawnSpeed;
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
			if (pawn.story == null) return;

			if (DanceOfEvolutionMod.settings.useTimelessHead)
			{
				var originalDefName = pawn.story.headType.defName;
				if (!originalDefName.Contains("_Timeless"))
				{
					var newDefName = "DE_" + originalDefName + "_Timeless";
					var newHead = DefDatabase<HeadTypeDef>.GetNamed(newDefName, false);
					if (newHead != null)
					{
						pawn.story.headType = newHead;
					}
					else if (pawn.story.headType != DefsOf.DE_TimelessOne)
					{
						pawn.story.headType = DefsOf.DE_TimelessOne;
					}
					pawn.Drawer.renderer.SetAllGraphicsDirty();
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
			Scribe_Values.Look(ref lastCurseUseTick, "lastCurseUseTick", -1);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				servants ??= new();
			}
		}

		private float GetCurseCooldownPercent()
		{
			if (lastCurseUseTick == -1)
			{
				return 1f;
			}

			int currentTick = Find.TickManager.TicksGame;
			int ticksSinceUse = currentTick - lastCurseUseTick;
			int effectiveCooldown = GetEffectiveCooldownTicks();

			return Mathf.Clamp01((float)ticksSinceUse / effectiveCooldown);
		}

		private int GetEffectiveCooldownTicks()
		{
			float psychicSensitivity = pawn.GetStatValue(StatDefOf.PsychicSensitivity);
			float sensitivityFactor = Mathf.Max(psychicSensitivity, 0.1f);
			return (int)(BaseCooldownTicks / sensitivityFactor);
		}

		private void StartCurseTargeting()
		{
			CameraJumper.TryJump(CameraJumper.GetWorldTarget(pawn.Map.Parent));
			Find.WorldTargeter.BeginTargeting(
				delegate (GlobalTargetInfo target)
				{
					if (Valid(target, true))
					{
						if (target.WorldObject is MapParent mapParent)
						{
							GameComponent_CurseManager.Instance.AddCursedSite(mapParent);
							DefsOf.Pawn_Sightstealer_Howl.PlayOneShotOnCamera();
							Find.LetterStack.ReceiveLetter("DE_CurseApplied".Translate(), "DE_CurseSuccess".Translate(mapParent.Label), LetterDefOf.PositiveEvent, mapParent);
							lastCurseUseTick = Find.TickManager.TicksGame;
							CameraJumper.TryJump(CameraJumper.GetWorldTarget(mapParent));
							return true;
						}
					}
					return false;
				},
				true,
				null,
				true,
null
			);
		}

		private bool Valid(GlobalTargetInfo target, bool throwMessages = false)
		{
			if (!(target.WorldObject is MapParent mapParent))
			{
				if (throwMessages)
				{
					Messages.Message("DE_CannotCurseThis".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (mapParent.HasMap)
			{
				if (throwMessages)
				{
					Messages.Message("DE_CannotCurseGeneratedMap".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}

			if (GameComponent_CurseManager.Instance.IsCursed(mapParent))
			{
				if (throwMessages)
				{
					Messages.Message("DE_AlreadyCursed".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return true;
		}
	}
}
