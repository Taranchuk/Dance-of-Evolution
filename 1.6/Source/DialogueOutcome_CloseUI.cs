using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    public class DialogueOutcome_CloseUI : DialogueOutcome
    {
        public override void DoOutcome(Pawn negotiator, Pawn speaker)
        {
            Find.WindowStack.TryRemove(typeof(Dialog_MycelyssEnvoy));
        }
    }
}