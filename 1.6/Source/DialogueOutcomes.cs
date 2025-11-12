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
            speaker.Faction.SetRelation(new FactionRelation(Faction.OfPlayer, FactionRelationKind.Hostile));
        }
    }
}
