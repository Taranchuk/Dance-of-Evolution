using RimWorld;
using Verse;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    public class DialogueOutcome_LeaveAndBecomeHostile : DialogueOutcome
    {
        public override void DoOutcome(Pawn negotiator, Pawn speaker)
        {
            Lord lord = speaker.GetLord();
            if (lord != null && lord.LordJob is LordJob_MycelyssEnvoy job)
            {
                job.makeHostileOnExit = true;
                lord.ReceiveMemo("Leave");
            }
            var factionRelation = new FactionRelation();
            factionRelation.other = Faction.OfPlayer;
            factionRelation.kind = FactionRelationKind.Hostile;
            factionRelation.baseGoodwill = -100;
            
            speaker.Faction.SetRelation(factionRelation);
            
            Find.LetterStack.ReceiveLetter("DE_MycelyssHostile".Translate(), "DE_MycelyssHostileDesc".Translate(), LetterDefOf.NegativeEvent, speaker);
        }
    }
}
