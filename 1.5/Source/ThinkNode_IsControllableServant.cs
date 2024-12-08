using Verse;
using Verse.AI;
namespace DanceOfEvolution
{
    public class ThinkNode_IsControllableServant : ThinkNode_Conditional
    {
        public override bool Satisfied(Pawn pawn)
        {
            return pawn.IsServant(out var hediff) && hediff.ControllableNoTileCheck;
        }
    }
}