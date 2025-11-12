using RimWorld;
using Verse;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    public class DialogueOutcome_OpenPawnSelectWindow : DialogueOutcome
    {
        public override void DoOutcome(Pawn negotiator, Pawn speaker)
        {
            Find.WindowStack.Add(new Window_SelectPawnsForDemand(speaker));
        }
    }
}
