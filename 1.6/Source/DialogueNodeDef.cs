using System.Collections.Generic;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    public class DialogueNodeDef : Def
    {
        public string text;
        public List<DialogueOption> options;
    }

    public class DialogueOption
    {
        public string text;
        public DialogueNodeDef nextNode;
        public List<DialogueOutcome> outcomes;
    }

    public abstract class DialogueOutcome
    {
        public abstract void DoOutcome(Pawn negotiator, Pawn speaker);
    }
}
