using RimWorld;
using UnityEngine;
using Verse;

namespace DanceOfEvolution
{
	[HotSwappable]
	public class FalseParasol : Plant
	{
		public override void Kill(DamageInfo? dinfo = null, Hediff exactCulprit = null)
		{
			if (dinfo?.Def?.armorCategory == DefsOf.Heat)
			{
				GenExplosion.DoExplosion(Position, Map, 4.9f, DamageDefOf.Extinguish, null);
			}
			base.Kill(dinfo, exactCulprit);
		}

		public override int YieldNow()
		{
			var result = base.YieldNow();
			if (result > 0)
			{
				Thing psychoid = ThingMaker.MakeThing(DefsOf.PsychoidLeaves);
				var harvestYield = 7f;
				float num = Mathf.InverseLerp(def.plant.harvestMinGrowth, 1f, growthInt);
				num = 0.5f + num * 0.5f;
				harvestYield *= num;
				harvestYield *= Mathf.Lerp(0.5f, 1f, (float)HitPoints / (float)base.MaxHitPoints);
				if (def.plant.harvestYieldAffectedByDifficulty)
				{
					harvestYield *= Find.Storyteller.difficulty.cropYieldFactor;
				}
				psychoid.stackCount = GenMath.RoundRandom(harvestYield);
				GenPlace.TryPlaceThing(psychoid, this.Position, this.Map, ThingPlaceMode.Near);
			}
			return result;
		}
	}
}
