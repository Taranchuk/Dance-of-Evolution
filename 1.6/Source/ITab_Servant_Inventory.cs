using RimWorld;
using UnityEngine;
using Verse;
using System.Collections.Generic;

namespace DanceOfEvolution
{
    [HotSwappable]
    public class ITab_Servant_Inventory : ITab
    {
        private Vector2 scrollPosition;
        private static List<Thing> workingInvList = new List<Thing>();

        public ITab_Servant_Inventory()
        {
            size = new Vector2(460f, 450f);
            labelKey = "DE_TabInventory";
        }

        public override bool IsVisible
        {
            get
            {
                Pawn selPawn = SelPawn;
                return selPawn != null && selPawn.IsServant();
            }
        }
        private float scrollViewHeight;

        public override void FillTab()
        {
            Rect rect = new Rect(0f, 0f, size.x, size.y);
            Rect rect2 = rect.ContractedBy(10f);
            Rect rect3 = new Rect(rect2.x, rect2.y, rect2.width, rect2.height);
            Widgets.BeginGroup(rect3);
            Rect outRect = new Rect(0f, 0f, rect3.width, rect3.height);
            Rect viewRect = new Rect(0f, 0f, rect3.width - 16f, scrollViewHeight);
            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
            Pawn selPawn = SelPawn;
            if (selPawn != null && selPawn.inventory != null)
            {
                float curY = 0f;
                Widgets.ListSeparator(ref curY, viewRect.width, "Inventory".Translate());
                workingInvList.Clear();
                workingInvList.AddRange(selPawn.inventory.innerContainer);
                for (int i = 0; i < workingInvList.Count; i++)
                {
                    DrawThingRow(ref curY, viewRect.width, workingInvList[i]);
                }
                workingInvList.Clear();
                scrollViewHeight = curY;
            }

            Widgets.EndScrollView();
            Widgets.EndGroup();
        }

        private void DrawThingRow(ref float y, float width, Thing thing)
        {
            Rect rect = new Rect(0f, y, width, 28f);
            Widgets.InfoCardButton(rect.width - 24f, y, thing);
            rect.width -= 24f;

            Rect rect2 = new Rect(rect.width - 24f, y, 24f, 24f);
            if (Widgets.ButtonImage(rect2, TexButton.Drop))
            {
                SelPawn.inventory.innerContainer.TryDrop(thing, SelPawn.Position, SelPawn.Map, ThingPlaceMode.Near, out var _);
            }
            rect.width -= 24f;

            if (Mouse.IsOver(rect))
            {
                GUI.color = ITab_Pawn_Gear.HighlightColor;
                GUI.DrawTexture(rect, TexUI.HighlightTex);
            }

            if (thing.def.DrawMatSingle != null && thing.def.DrawMatSingle.mainTexture != null)
            {
                Widgets.ThingIcon(new Rect(4f, y, 28f, 28f), thing);
            }

            Text.Anchor = TextAnchor.MiddleLeft;
            GUI.color = ITab_Pawn_Gear.ThingLabelColor;
            Rect rect5 = new Rect(36f, y, rect.width - 36f, rect.height);
            Text.WordWrap = false;
            Widgets.Label(rect5, thing.LabelCap.Truncate(rect5.width));
            Text.WordWrap = true;

            if (Mouse.IsOver(rect))
            {
                TooltipHandler.TipRegion(rect, thing.GetTooltip());
            }

            y += 28f;
        }
    }
}
