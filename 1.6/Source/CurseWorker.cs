using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    public abstract class CurseWorker
    {
        public CurseEffectDef def;

        public abstract void Apply(Map map);

        protected IEnumerable<Pawn> GetAllHostilePawns(Map map)
        {
            return map.mapPawns.AllPawns.Where(p => p.HostileTo(Faction.OfPlayer));
        }

        protected bool TryFindRandomSpawnCell(Map map, out IntVec3 result)
        {
            return CellFinder.TryFindRandomCell(map, c => c.Standable(map) && !c.Fogged(map), out result);
        }
    }
}