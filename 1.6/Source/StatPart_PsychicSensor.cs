using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    public class StatPart_PsychicSensor : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (TryGetMissingParts(req.Thing, out var missingCount, out var totalCount))
            {
                if (totalCount > 0)
                {
                    if (missingCount == totalCount)
                    {
                        val = 0f;
                    }
                    else
                    {
                        val *= 1f - (missingCount / (float)totalCount);
                    }
                }
            }
        }

        public override string ExplanationPart(StatRequest req)
        {
            return null;
        }

        private bool TryGetMissingParts(Thing t, out int missingCount, out int totalCount)
        {
            missingCount = 0;
            totalCount = 0;
            if (t is Pawn pawn && pawn.kindDef == DefsOf.DE_MikisMetalonEfialtis)
            {
                foreach (var part in pawn.def.race.body.GetPartsWithDef(DefsOf.DE_PsychicSensor))
                {
                    totalCount++;
                    if (pawn.health.hediffSet.PartIsMissing(part))
                    {
                        missingCount++;
                    }
                }

                return totalCount > 0;
            }
            return false;
        }
    }
}
