using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	public class Hediff_Rotting : HediffWithComps
	{
		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			base.Notify_PawnDied(dinfo, culprit);
			if (pawn?.Corpse?.Map != null)
			{
				var cell = pawn.Corpse.Position;
				var map = pawn.Corpse.Map;
				var corpse = pawn.Corpse;
				GenExplosion.DoExplosion(radius: 3.9f, center: corpse.Position, map: corpse.Map,
					damType: DamageDefOf.ToxGas, instigator: corpse.InnerPawn, damAmount: -1,
					armorPenetration: -1f, explosionSound: null, weapon: null,
					projectile: null, intendedTarget: null, postExplosionSpawnThingDef: null,
					postExplosionSpawnChance: 0f, postExplosionSpawnThingCount: 1,
					postExplosionGasType: GasType.ToxGas);

				GenExplosion.DoExplosion(radius: 3.9f, center: corpse.Position, map: corpse.Map,
					damType: DamageDefOf.ToxGas, instigator: corpse.InnerPawn, damAmount: -1,
					armorPenetration: -1f, explosionSound: null, weapon: null,
					projectile: null, intendedTarget: null, postExplosionSpawnThingDef: null,
					postExplosionSpawnChance: 0f, postExplosionSpawnThingCount: 1,
					postExplosionGasType: GasType.RotStink);

				if (corpse.GetComp<CompRottable>() is CompRottable comp)
				{
					comp.RotProgress = comp.PropsRot.TicksToRotStart;
				}
			}
		}
	}
}
