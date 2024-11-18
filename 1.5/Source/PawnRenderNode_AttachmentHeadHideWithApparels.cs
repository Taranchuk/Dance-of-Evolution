using System.Linq;
using RimWorld;
using Verse;
namespace DanceOfEvolution
{
    public class PawnRenderNode_AttachmentHeadHideWithApparels : PawnRenderNode_AttachmentHead
    {
        public PawnRenderNode_AttachmentHeadHideWithApparels(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            if (pawn.apparel.WornApparel.Any(x => PawnApparelGenerator.IsHeadgear(x.def)))
            {
                return null;
            }
            return base.GraphicFor(pawn);
        }
    }
}
