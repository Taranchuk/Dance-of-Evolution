using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace DanceOfEvolution
{
    public class StockGenerator_SingleDefWithChance : StockGenerator_SingleDef
    {
        public float chance;
        public override IEnumerable<Thing> GenerateThings(PlanetTile forTile, Faction forFaction = null)
        {
            if (Rand.Chance(chance))
            {
                yield break;
            }

            foreach (Thing thing in base.GenerateThings(forTile, forFaction))
            {
                yield return thing;
            }
        }
    }
}
