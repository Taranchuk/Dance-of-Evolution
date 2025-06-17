using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    public class ThoughtWorker_Hediff : ThoughtWorker
    {
        public override ThoughtState CurrentStateInternal(Pawn p)
        {
            Hediff firstHediffOfDef = p.health.hediffSet.GetFirstHediffOfDef(def.hediff);
            if (firstHediffOfDef == null)
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveDefault;
        }
    }
}