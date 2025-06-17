using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(PawnRenderNodeWorker), "AppendDrawRequests")]
    public static class PawnRenderNodeWorker_AppendDrawRequests_Patch
    {
        public static bool Prefix(PawnRenderNode node, PawnDrawParms parms, List<PawnGraphicDrawRequest> requests)
        {
            if (node.Worker is PawnRenderNodeWorker_Eye && node.hediff?.def == HediffDefOf.VoidTouched)
            {
                if (parms.pawn.IsFungalNexus(out _))
                {
                    requests.Add(new PawnGraphicDrawRequest(node));
                    return false;
                }
            }
            return true;
        }
    }
}
