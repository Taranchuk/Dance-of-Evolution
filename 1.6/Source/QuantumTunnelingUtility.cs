using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DanceOfEvolution
{
    public static class QuantumTunnelingUtility
    {
        private static readonly List<TraitDef> blacklistedTraits = new List<TraitDef>();
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

        public static void TryApplyMoodlet(Pawn pawn)
        {
            if (pawn.needs?.mood?.thoughts == null || pawn.story?.traits == null || !pawn.RaceProps.Humanlike || pawn.Inhumanized())
            {
                return;
            }

            if (blacklistedTraits.Count == 0)
            {
                blacklistedTraits.Add(TraitDefOf.Ascetic);
                blacklistedTraits.Add(DefsOf.TorturedArtist);
                blacklistedTraits.Add(DefsOf.Occultist);
                blacklistedTraits.Add(DefsOf.Disturbing);
                blacklistedTraits.Add(DefsOf.VoidFascination);
            }

            if (blacklistedTraits.Any(t => pawn.story.traits.HasTrait(t)))
            {
                return;
            }

            var naturalMood = pawn.story.traits.GetTrait(DefsOf.NaturalMood);
            if (naturalMood != null && naturalMood.Degree == -2) // Depressive
            {
                return;
            }

            var nerves = pawn.story.traits.GetTrait(DefsOf.Nerves);
            if (nerves != null && (nerves.Degree == 1 || nerves.Degree == 2)) // Steadfast or Iron-willed
            {
                return;
            }

            pawn.needs.mood.thoughts.memories.TryGainMemory(DefsOf.DE_QuantumTunnelingMood);
        }
    }
}
