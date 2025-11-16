using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    public class DialogueOutcome_BackToRoot : DialogueOutcome
    {
        public override void DoOutcome(Pawn negotiator, Pawn speaker)
        {
            Find.WindowStack.TryRemove(typeof(Dialog_MycelyssEnvoy));
            Find.WindowStack.Add(new Dialog_MycelyssEnvoy(speaker, negotiator));
        }
    }
}
