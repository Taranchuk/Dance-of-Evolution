using RimWorld;
using Verse;
using UnityEngine;
using Verse.Sound;
using System.Collections.Generic;

namespace DanceOfEvolution
{
	[StaticConstructorOnStartup]
	public class Dialog_GrowthSpot : Window
	{
		private Pawn fungalNexus;
		private HediffDef selectedCosmetic;
		private Vector2 cosmeticScrollPosition;
		private float viewRectHeight;
		private static readonly Vector2 ButSize = new Vector2(200f, 40f);
		private const float IconSize = 60f;
		private static readonly Vector3 PortraitOffset = new Vector3(0f, 0f, 0.15f);
		private const float PortraitZoom = 1.1f;
		private bool showHeadgear;
		private bool showClothes;
		private Dictionary<Apparel, Color> apparelColors = new Dictionary<Apparel, Color>();

		public override Vector2 InitialSize => new Vector2(950f, 750f);

		public Dialog_GrowthSpot(Pawn nexus, Thing spot)
		{
			this.fungalNexus = nexus;
			this.growthSpot = spot;
			forcePause = true;
			showClothes = true;
			closeOnAccept = false;
			closeOnCancel = false;
		}

		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Medium;
			Rect rect = new Rect(inRect);
			rect.height = Text.LineHeight * 2f;
			Widgets.Label(rect, "GrowthSpotCosmetics".Translate().CapitalizeFirst() + ": " + Find.ActiveLanguageWorker.WithDefiniteArticle(fungalNexus.Name.ToStringShort, fungalNexus.gender, plural: false, name: true).ApplyTag(TagType.Name));
			Text.Font = GameFont.Small;
			inRect.yMin = rect.yMax + 4f;

			Rect pawnRect = inRect.LeftPart(0.3f).Rounded();
			Rect tabRect = inRect.RightPart(0.7f).Rounded();
			pawnRect.yMax -= ButSize.y + 4f;
			tabRect.yMax -= ButSize.y + 4f;

			DrawPawn(pawnRect);
			DrawCosmeticsTab(tabRect);
			DrawBottomButtons(inRect);
		}

		private void DrawPawn(Rect rect)
		{
			Rect rect2 = rect;
			rect2.yMin = rect.yMax - Text.LineHeight * 2f;
			Widgets.CheckboxLabeled(new Rect(rect2.x, rect2.y, rect2.width, rect2.height / 2f), "ShowHeadgear".Translate(), ref showHeadgear);
			Widgets.CheckboxLabeled(new Rect(rect2.x, rect2.y + rect2.height / 2f, rect2.width, rect2.height / 2f), "ShowApparel".Translate(), ref showClothes);
			rect.yMax = rect2.yMin - 4f;
			Widgets.BeginGroup(rect);
			for (int i = 0; i < 3; i++)
			{
				Rect position = new Rect(0f, rect.height / 3f * (float)i, rect.width, rect.height / 3f).ContractedBy(4f);
				RenderTexture image = PortraitsCache.Get(fungalNexus, new Vector2(position.width, position.height), new Rot4(2 - i), PortraitOffset, PortraitZoom, supersample: true, compensateForUIScale: true, showHeadgear, showClothes, apparelColors, fungalNexus.story.HairColor, stylingStation: true);
				GUI.DrawTexture(position, image);
			}
			Widgets.EndGroup();
		}

		private void DrawCosmeticsTab(Rect rect)
		{
			Rect viewRect = new Rect(rect.x, rect.y, rect.width - 16f, viewRectHeight);
			Widgets.BeginScrollView(rect, ref cosmeticScrollPosition, viewRect);
			int num = Mathf.FloorToInt(viewRect.width / IconSize) - 1;
			float num2 = (viewRect.width - (float)num * IconSize - (float)(num - 1) * 10f) / 2f;
			int numCosmetics = 0;
			int row = 0;
			int col = 0;

			foreach (HediffDef cosmetic in Startup.bodyAttachments)
			{
				if (col >= num)
				{
					col = 0;
					row++;
				}

				Rect iconRect = new Rect(rect.x + num2 + (float)col * IconSize + (float)col * 10f, rect.y + (float)row * IconSize + (float)row * 10f, IconSize, IconSize);
				Widgets.DrawHighlight(iconRect);
				if (Mouse.IsOver(iconRect))
				{
					Widgets.DrawHighlight(iconRect);
					TooltipHandler.TipRegion(iconRect, cosmetic.LabelCap);
				}
				Widgets.DefIcon(iconRect, cosmetic, null, 1f);

				if (selectedCosmetic == cosmetic)
				{
					Widgets.DrawBox(iconRect, 2);
				}

				if (Widgets.ButtonInvisible(iconRect))
				{
					selectedCosmetic = cosmetic;
					SoundDefOf.Tick_High.PlayOneShotOnCamera();
				}
				col++;
				numCosmetics++;
			}

			if (Event.current.type == EventType.Layout)
			{
				viewRectHeight = (float)(row + 1) * IconSize + (float)row * 10f + 10f;
			}
			Widgets.EndScrollView();
		}


		private void DrawBottomButtons(Rect inRect)
		{
			if (Widgets.ButtonText(new Rect(inRect.x, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), "Cancel".Translate()))
			{
				Close();
			}
			if (Widgets.ButtonText(new Rect(inRect.xMin + inRect.width / 2f - ButSize.x / 2f, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), "Reset".Translate()))
			{
				selectedCosmetic = null;
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}
			if (Widgets.ButtonText(new Rect(inRect.xMax - ButSize.x, inRect.yMax - ButSize.y, ButSize.x, ButSize.y), "Accept".Translate()))
			{
				ApplyCosmeticChange();
				Close();
			}
		}

		private void ApplyCosmeticChange()
		{
			if (selectedCosmetic != null)
			{

			}
		}
	}
}