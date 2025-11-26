using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace DanceOfEvolution
{
    [HotSwappable]
    public class FungalServantGroupGizmo : Gizmo
    {
        private ServantType servantType;
        private List<Pawn> servantsInGroup;

        public FungalServantGroupGizmo(Hediff_FungalNexus nexus, ServantType servantType)
        {
            this.servantType = servantType;
            this.servantsInGroup = nexus.servants.Where(p => p.IsServant(out Hediff_ServantType hediff) && hediff.ServantType == servantType).ToList();
            this.Order = -89f;
        }

        public override float GetWidth(float maxWidth)
        {
            return 130f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);

            bool flag = Mouse.IsOver(rect2);
            Color white = Color.white;
            GUI.color = parms.lowLight ? Command.LowLightBgColor : white;
            GenUI.DrawTextureWithMaterial(rect, parms.shrunk ? Command.BGTexShrunk : Command.BGTex, null);
            GUI.color = Color.white;

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect3 = rect2;
            TaggedString str = servantType.ToString().CapitalizeFirst() + " " + "DE_Servants".Translate().ToString().UncapitalizeFirst();
            Vector2 vector = Text.CalcSize(str);
            rect3.width = vector.x;
            rect3.height = vector.y;
            Widgets.Label(rect3, str);

            if (Mouse.IsOver(rect3))
            {
                Widgets.DrawHighlight(rect3);
                if (Widgets.ButtonInvisible(rect3))
                {
                    Find.Selector.ClearSelection();
                    for (int j = 0; j < servantsInGroup.Count; j++)
                    {
                        Find.Selector.Select(servantsInGroup[j]);
                    }
                }
            }

            Rect rect6 = new Rect(rect2.x, rect2.y + 26f + 4f, rect2.width, rect2.height - 26f - 4f);
            float num = rect6.height;
            int num2 = 0;
            int num3 = 0;
            for (float num4 = num; num4 >= 0f; num4 -= 1f)
            {
                num2 = Mathf.FloorToInt(rect6.width / num4);
                num3 = Mathf.FloorToInt(rect6.height / num4);
                if (num2 * num3 >= servantsInGroup.Count)
                {
                    num = num4;
                    break;
                }
            }

            float num5 = (rect6.width - (float)num2 * num) / 2f;
            float num6 = (rect6.height - (float)num3 * num) / 2f;
            int num7 = 0;
            for (int k = 0; k < num2; k++)
            {
                for (int l = 0; l < num2; l++)
                {
                    if (num7 >= servantsInGroup.Count)
                    {
                        break;
                    }
                    Rect rect7 = new Rect(rect6.x + (float)l * num + num5, rect6.y + (float)k * num + num6, num, num);
                    Pawn pawn = servantsInGroup[num7];
                    Vector2 size = rect7.size;
                    Rot4 east = Rot4.East;
                    float portraitZoom = pawn.kindDef.controlGroupPortraitZoom;
                    RenderTexture image = PortraitsCache.Get(pawn, size, east, default(Vector3), portraitZoom);
                    GUI.DrawTexture(rect7, image);
                    if (Mouse.IsOver(rect7))
                    {
                        Widgets.DrawHighlight(rect7);
                        MouseoverSounds.DoRegion(rect7, SoundDefOf.Mouseover_Command);
                        if (Event.current.type == EventType.MouseDown)
                        {
                            if (Event.current.shift)
                            {
                                Find.Selector.Select(servantsInGroup[num7]);
                            }
                            else
                            {
                                CameraJumper.TryJumpAndSelect(servantsInGroup[num7]);
                            }
                        }
                        TargetHighlighter.Highlight(servantsInGroup[num7], arrow: true, colonistBar: false);
                    }
                    if (Find.Selector.IsSelected(servantsInGroup[num7]))
                    {
                        SelectionDrawerUtility.DrawSelectionOverlayOnGUI(servantsInGroup[num7], rect7, 0.8f / (float)num2, 20f);
                    }
                    num7++;
                }
                if (num7 >= servantsInGroup.Count)
                {
                    break;
                }
            }

            return new GizmoResult(flag ? GizmoState.Mouseover : GizmoState.Clear);
        }
    }
}
