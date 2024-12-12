using RimWorld;
using Verse;
using System.Collections.Generic;
using UnityEngine;

namespace DanceOfEvolution
{
	public class Building_Cloudmaker : Building
	{
		private CompRefuelable refuelableComp;
		private Dictionary<Map, GameCondition> causedConditions = new Dictionary<Map, GameCondition>();
		private static List<Map> mapsToRemoveConditionFrom = new List<Map>();
		private const int DeathPallCooldownTicks = 1200000; // 20 days
		private const float WorldRange = 10f; // Range in tiles for AoE effect
		private int lastDeathPallTick = -1;
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			refuelableComp = GetComp<CompRefuelable>();
			if (!respawningAfterLoad)
			{
				UpdateMapEffects();
			}
		}

		public override void Tick()
		{
			base.Tick();

			if (this.IsHashIntervalTick(60)) // Check every game second
			{
				UpdateMapEffects();
			}

			// Clean up expired conditions
			mapsToRemoveConditionFrom.Clear();
			foreach (var mapCondition in causedConditions)
			{
				if (mapCondition.Value.Expired || !mapCondition.Key.GameConditionManager.ConditionIsActive(mapCondition.Value.def))
				{
					mapsToRemoveConditionFrom.Add(mapCondition.Key);
				}
			}
			foreach (var map in mapsToRemoveConditionFrom)
			{
				causedConditions.Remove(map);
			}
		}

		private void UpdateMapEffects()
		{
			bool hasFuel = refuelableComp.HasFuel;
			if (hasFuel)
			{
				foreach (Map map in Find.Maps)
				{
					if (InAoE(map.Tile))
					{
						ApplyConditionToMap(map);
					}
				}
			}
			else
			{
				foreach (Map map in Find.Maps)
				{
					if (InAoE(map.Tile))
					{
						RemoveConditionFromMap(map);
					}
				}
			}
		}

		private void ApplyConditionToMap(Map map)
		{
			if (!causedConditions.ContainsKey(map))
			{
				GameCondition gameCondition = GameConditionMaker.MakeCondition(DefDatabase<GameConditionDef>.GetNamed("DE_CloudmakerCondition"));
				gameCondition.Duration = gameCondition.TransitionTicks;
				gameCondition.conditionCauser = this;
				map.gameConditionManager.RegisterCondition(gameCondition);
				causedConditions.Add(map, gameCondition);
				SetupCondition(gameCondition, map);
			}
		}

		private void RemoveConditionFromMap(Map map)
		{
			if (causedConditions.ContainsKey(map))
			{
				GameCondition gameCondition = causedConditions[map];
				gameCondition.End();
				causedConditions.Remove(map);
			}
		}

		private void SetupCondition(GameCondition condition, Map map)
		{
			condition.suppressEndMessage = true;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref lastDeathPallTick, "lastDeathPallTick", -1);
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				causedConditions.RemoveAll((KeyValuePair<Map, GameCondition> x) => !Find.Maps.Contains(x.Key));
			}
			Scribe_Collections.Look(ref causedConditions, "causedConditions", LookMode.Reference, LookMode.Reference);
			if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
			{
				causedConditions.RemoveAll((KeyValuePair<Map, GameCondition> x) => x.Value == null);
				foreach (KeyValuePair<Map, GameCondition> causedCondition in causedConditions)
				{
					causedCondition.Value.conditionCauser = this;
				}
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo g in base.GetGizmos())
			{
				yield return g;
			}

			if (Faction == Faction.OfPlayer)
			{
				if (refuelableComp.HasFuel)
				{
					Command_Action deathPall = new Command_Action
					{
						defaultLabel = "DE_TriggerDeathPall".Translate(),
						defaultDesc = "DE_TriggerDeathPallDesc".Translate(),
						action = delegate ()
						{
							TriggerDeathPall();
						}
					};

					if (lastDeathPallTick > 0)
					{
						int ticksRemaining = lastDeathPallTick + DeathPallCooldownTicks - Find.TickManager.TicksGame;
						if (ticksRemaining > 0)
						{
							deathPall.Disable("DE_DeathPallCooldown".Translate(ticksRemaining.ToStringTicksToPeriod()));
						}
					}

					yield return deathPall;
				}
			}
		}

		private void TriggerDeathPall()
		{
			lastDeathPallTick = Find.TickManager.TicksGame;
		}

		public bool InAoE(int tile)
		{
			if (this.Tile == -1 || tile == -1 || !refuelableComp.HasFuel)
			{
				return false;
			}
			return Find.WorldGrid.ApproxDistanceInTiles(tile, this.Tile) < WorldRange;
		}
	}
}
