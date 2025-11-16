using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    public class DialogueOutcome_MakeHostile : DialogueOutcome
    {
        public override void DoOutcome(Pawn negotiator, Pawn speaker)
        {
            Lord currentLord = speaker.GetLord();
            if (currentLord != null)
            {
                currentLord.Map.lordManager.RemoveLord(currentLord);
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
