using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace DanceOfEvolution
{
    public static class QuantumTunnelingUtility
    {
        private static PlanetTile cachedOrigin;
        private static PlanetTile cachedDest;
        private static int cachedDistance;
        private static PlanetLayer cachedOriginLayer;
        private static PlanetLayer cachedDestLayer;
        private static readonly List<PlanetLayerConnection> connections = new List<PlanetLayerConnection>();

        public static int GetDistance(PlanetTile from, PlanetTile to)
        {
            if (cachedOrigin == from && cachedDest == to)
            {
                return cachedDistance;
            }
            cachedOrigin = from;
            cachedDest = to;
            cachedDistance = 0;
            if (from.Layer != to.Layer)
            {
                if (cachedOriginLayer == from.Layer && cachedDestLayer == to.Layer)
                {
                }
                else
                {
                    if (!from.Layer.TryGetPath(to.Layer, connections, out var cost))
                    {
                        connections.Clear();
                        return 0;
                    }
                    cachedOriginLayer = to.Layer;
                    cachedDestLayer = from.Layer;
                    connections.Clear();
                }
                from = to.Layer.GetClosestTile_NewTemp(from);
            }
            cachedDistance = (int)(Find.WorldGrid.TraversalDistanceBetween((int)from, (int)to) * to.LayerDef.rangeDistanceFactor);
            return cachedDistance;
        }
    }
}