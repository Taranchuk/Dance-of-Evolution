using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	public class PlaceWorker_MustBuildOnRottenSoil : PlaceWorker
	{
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc,
		Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			foreach (var cell in GenAdj.CellsOccupiedBy(loc, rot, checkingDef.Size))
			{
				TerrainDef terrain = map.terrainGrid.TerrainAt(cell);
				if (terrain.IsFungalTerrain() is false)
				{
					return new AcceptanceReport("DE_MustBuildOnRottenSoil".Translate());
				}
			}
			return true;
		}
	}
}
