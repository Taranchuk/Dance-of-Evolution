using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace DanceOfEvolution
{
    [StaticConstructorOnStartup]
    public class FungalNexusGizmo : Gizmo
    {
        private const int InRectPadding = 6;
        private const int CellPadding = 2;
        private const float Width = 136f;
        private const int HeaderHeight = 20;
        private static readonly Color EmptyBlockColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        private static readonly Color FilledBlockColor = new ColorInt(214, 90, 24).ToColor;
        private static readonly Color ExcessBlockColor = ColorLibrary.Red;
        private Hediff_FungalNexus nexus;
        public FungalNexusGizmo(Hediff_FungalNexus nexus)
        {
            this.nexus = nexus;
            Order = -90f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect rect2 = rect.ContractedBy(InRectPadding);
            Widgets.DrawWindowBackground(rect);

            int maxServants = nexus.MaxServants;
            int currentServants = nexus.servants.Count;
            string text = currentServants.ToString("F0") + " / " + maxServants.ToString("F0");
            TaggedString taggedString = "DE_Servants".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + text;

            if (currentServants > 0)
            {
                IEnumerable<string> entries = from p in nexus.servants
                                              group p by p.kindDef into p
                                              select (string)(p.Key.LabelCap + " x") + p.Count();
                taggedString += "\n\n" + entries.ToLineList(" - ");
            }

            TooltipHandler.TipRegion(rect, taggedString);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect3 = new Rect(rect2.x, rect2.y, rect2.width, HeaderHeight);
            Widgets.Label(rect3, "DE_Servants".Translate());
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperRight;
            Widgets.Label(rect3, text);
            Text.Anchor = TextAnchor.UpperLeft;

            int num = Mathf.Max(currentServants, maxServants);
            Rect rect4 = new Rect(rect2.x, rect3.yMax + 6f, rect2.width, rect2.height - rect3.height - 6f);
            int num2 = 2;
            int num3 = Mathf.FloorToInt(rect4.height / (float)num2);
            int num4 = Mathf.FloorToInt(rect4.width / (float)num3);
            int num5 = 0;
            while (num2 * num4 < num)
            {
                num2++;
                num3 = Mathf.FloorToInt(rect4.height / (float)num2);
                num4 = Mathf.FloorToInt(rect4.width / (float)num3);
                num5++;
                if (num5 >= 1000)
                {
                    Log.Error("Failed to fit servant cells into gizmo rect.");
                    return new GizmoResult(GizmoState.Clear);
                }
            }

            int num6 = Mathf.FloorToInt(rect4.width / (float)num3);
            int num7 = num2;
            float num8 = (rect4.width - (float)(num6 * num3)) / 2f;
            int num9 = 0;
            for (int i = 0; i < num7; i++)
            {
                for (int j = 0; j < num6; j++)
                {
                    num9++;
                    Rect rect5 = new Rect(rect4.x + (float)(j * num3) + num8, rect4.y + (float)(i * num3), num3, num3).ContractedBy(CellPadding);
                    if (num9 <= num)
                    {
                        if (num9 <= currentServants)
                        {
                            Widgets.DrawRectFast(rect5, (num9 <= maxServants) ? FilledBlockColor : ExcessBlockColor);
                        }
                        else
                        {
                            Widgets.DrawRectFast(rect5, EmptyBlockColor);
                        }
                    }
                }
            }

            return new GizmoResult(GizmoState.Clear);
        }

        public override float GetWidth(float maxWidth)
        {
            return Width;
        }
    }
}