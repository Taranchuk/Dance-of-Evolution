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
        }
    }
}
