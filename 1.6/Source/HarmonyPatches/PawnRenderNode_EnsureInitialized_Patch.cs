using System.Linq;
using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(PawnRenderNode), "EnsureInitialized")]
    public static class PawnRenderNode_EnsureInitialized_Patch
    {
        public static void Postfix(PawnRenderNode __instance)
        {
            if (__instance.tree?.pawn is null || !__instance.tree.pawn.IsServant(out var _)) return;
            if (__instance.graphics != null)
            {
                for (var i = __instance.graphics.Count - 1; i >= 0; i--)
                {
                    var graphic = __instance.graphics[i];
                    if (graphic == null) continue;
                    graphic = graphic.GetColoredVersion(graphic.Shader, PawnRenderUtility.GetRottenColor(graphic.color), PawnRenderUtility.GetRottenColor(graphic.colorTwo));
                    __instance.graphics[i] = graphic;
                }
            }
            if (__instance.graphicStateLookup != null)
            {
                var keys = __instance.graphicStateLookup.Keys.ToList();
                for (var i = keys.Count - 1; i >= 0; i--)
                {
                    var key = keys[i];
                    var graphic = __instance.graphicStateLookup[key];
                    if (graphic == null) continue;
                    graphic = graphic.GetColoredVersion(graphic.Shader, PawnRenderUtility.GetRottenColor(graphic.color), PawnRenderUtility.GetRottenColor(graphic.colorTwo));
                    __instance.graphicStateLookup[key] = graphic;
                }
            }
        }
    }
}
