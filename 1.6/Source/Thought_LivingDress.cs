using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    public class Thought_LivingDress : Thought_Situational
    {
        public override float MoodOffset()
        {
            return this.CurStage.baseMoodEffect * this.pawn.GetStatValue(StatDefOf.PsychicSensitivity);
        }
    }
}
