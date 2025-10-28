using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Unity.Collections;
using Verse;

namespace DanceOfEvolution
{
    [HotSwappable]
    public abstract class CurseWorker
    {
        public CurseEffectDef def;

        public abstract void Apply(Map map);

        protected IEnumerable<Pawn> GetAllHostilePawns(Map map)
        {
            var allPawns = map.mapPawns.AllPawns;
            return allPawns.Where(p => p.HostileTo(Faction.OfPlayer));
        }

        protected bool TryFindRandomSpawnCell(Map map, out IntVec3 result)
        {
            return CellFinder.TryFindRandomCell(map, c => c.Standable(map) && !c.Fogged(map) && c.DistanceToEdge(map) > 5, out result);
        }
    }
}
