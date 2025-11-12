using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HotSwappable]
    public class Dialog_MycelyssEnvoy : Dialog_NodeTree
    {
        public Dialog_MycelyssEnvoy(Pawn envoy, Pawn playerNegotiator) : base(BuildNodeTree(envoy, playerNegotiator), true, false, null)
        {
        }

        private static DiaNode BuildNodeTree(Pawn envoy, Pawn playerNegotiator)
        {
            return GenerateNode(DefsOf.DE_MycelyssEnvoy_Root, envoy, playerNegotiator);
        }

        private static DiaNode GenerateNode(DialogueNodeDef nodeDef, Pawn envoy, Pawn playerNegotiator)
        {
            DiaNode node = new DiaNode(nodeDef.text);
            if (!nodeDef.options.NullOrEmpty())
            {
                foreach (DialogueOption optionData in nodeDef.options)
                {
                    DiaOption option = new DiaOption(optionData.text);
                    option.action = () =>
                    {
                        if (!optionData.outcomes.NullOrEmpty())
                        {
                            foreach (DialogueOutcome outcome in optionData.outcomes)
                            {
                                outcome.DoOutcome(playerNegotiator, envoy);
                            }
                        }
                    };
                    if (optionData.nextNode != null)
                    {
                        option.link = GenerateNode(optionData.nextNode, envoy, playerNegotiator);
                    }
                    node.options.Add(option);
                }
            }
            return node;
        }
    }
}
