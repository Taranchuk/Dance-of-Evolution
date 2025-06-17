using RimWorld;
using Verse;
namespace DanceOfEvolution
{
	public class Building_FungusWall : Building
	{
		public int ticksToHarden;
		public bool Hardened => def == DefsOf.DE_HardenedFungusWall;
		public override void PostMake()
		{
			base.PostMake();
			if (Hardened is false)
			{
				ticksToHarden = (int)(Rand.Range(4f, 9f) * GenDate.TicksPerDay);
			}
		}

		public override void TickLong()
		{
			base.TickLong();
			if (Spawned && Hardened is false && Position.GetTerrain(Map) == DefsOf.DE_RottenSoil)
			{
				ticksToHarden -= 2000;
				if (ticksToHarden <= 0)
				{
					var pos = Position;
					var map = Map;
					Destroy();
					var hardenedWall = GenSpawn.Spawn(DefsOf.DE_HardenedFungusWall, pos, map);
					hardenedWall.SetFactionDirect(this.Faction);
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksToHarden, "ticksToHarden");
		}
	}
}