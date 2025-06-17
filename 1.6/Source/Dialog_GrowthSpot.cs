using RimWorld;
using Verse;
using UnityEngine;
using Verse.Sound;
using System.Collections.Generic;
using System.Linq;
using System;
using Verse.AI;

namespace DanceOfEvolution
{
	[HotSwappable]
	[StaticConstructorOnStartup]
	public class Dialog_GrowthSpot : Window
	{
		private Pawn pawn;
		private Thing growthSpot;
		private HediffDef selectedCosmetic;
		private Vector2 cosmeticScrollPosition;
		private float viewRectHeight;
		private static readonly Vector2 ButSize = new Vector2(200f, 40f);
		private float IconSize => 120f;
		private static readonly Vector3 PortraitOffset = new Vector3(0f, 0f, 0.15f);
		private const float PortraitZoom = 1.1f;
		public override Vector2 InitialSize => new Vector2(950f, 750f);
		public Dialog_GrowthSpot(Pawn nexus, Thing spot)
		{
			this.pawn = nexus;
			this.growthSpot = spot;
			forcePause = true;
			closeOnAccept = false;
			closeOnCancel = false;
		}

		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Medium;
			Rect rect = new Rect(inRect);
			rect.height = Text.LineHeight * 2f;
			Widgets.Label(rect, "DE_GrowthSpotCosmetics".Translate() + ": " + Find.ActiveLanguageWorker.WithDefiniteArticle(pawn.Name.ToStringShort, pawn.gender, plural: false, name: true).ApplyTag(TagType.Name));
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
			rect.yMax = rect2.yMin - 4f;
			Widgets.BeginGroup(rect);
			var allAttachments = pawn.health.hediffSet.hediffs.Where(x => Startup.bodyAttachments.Contains(x.def)).ToList();
			allAttachments.ForEach(x => pawn.health.RemoveHediff(x));
			if (selectedCosmetic != null)
			{
				pawn.health.AddHediff(selectedCosmetic);
			}
			for (int i = 0; i < 3; i++)
			{
				Rect position = new Rect(0f, rect.height / 3f * (float)i, rect.width, rect.height / 3f).ContractedBy(4f);
				RenderTexture image = PortraitsCache.Get(pawn, new Vector2(position.width, position.height), new Rot4(2 - i), PortraitOffset, PortraitZoom, supersample: true, compensateForUIScale: true, true, true, stylingStation: true);
				GUI.DrawTexture(position, image);
			}
			var allAttachments2 = pawn.health.hediffSet.hediffs.Where(x => Startup.bodyAttachments.Contains(x.def)).ToList();
			allAttachments2.ForEach(x => pawn.health.RemoveHediff(x));
			allAttachments.ForEach(x => pawn.health.AddHediff(x));
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

				Rect iconRect = new Rect(rect.x + num2 + (float)col * IconSize + (float)col * 10f,
				rect.y + (float)row * IconSize + (float)row * 10f, IconSize, IconSize);
				Widgets.DrawMenuSection(iconRect);
				var pawnRenderNodeProperties = cosmetic.RenderNodeProperties.First();
				var node = (PawnRenderNode)Activator.CreateInstance(pawnRenderNodeProperties.nodeClass, pawn,
					pawnRenderNodeProperties, pawn.Drawer.renderer.renderTree);
				var graphic = node.GraphicFor(pawn);
				GUI.DrawTexture(iconRect, graphic.MatSouth.mainTexture);

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
				ThingDef rawFungus = DefsOf.RawFungus;
				int requiredAmount = 60;
				IngredientCount ingredientCount = new IngredientCount();
				ingredientCount.SetBaseCount(requiredAmount);
				ingredientCount.filter.SetAllow(rawFungus, allow: true);
				var rawFungusList = new List<ThingCount>();
				if (WorkGiver_DoBill.TryFindBestFixedIngredients(new List<IngredientCount> { ingredientCount },
				 pawn, growthSpot, rawFungusList))
				{
					Job job = JobMaker.MakeJob(DefsOf.DE_ApplyCosmeticChange, growthSpot);
					if (!rawFungusList.NullOrEmpty())
					{
						job.targetQueueB = new List<LocalTargetInfo>(rawFungusList.Count);
						job.countQueue = new List<int>(rawFungusList.Count);
						foreach (ThingCount extraIngredient in rawFungusList)
						{
							job.targetQueueB.Add(extraIngredient.Thing);
							job.countQueue.Add(extraIngredient.Count);
						}
					}
					job.haulMode = HaulMode.ToCellNonStorage;
					pawn.jobs.TryTakeOrderedJob(job);

					var fungalNexusHediff = pawn.GetFungalNexus();
					fungalNexusHediff.selectedCosmetic = selectedCosmetic;
				}
				else
				{
					Messages.Message("DE_NeedRawFungusForCosmeticChange".Translate(requiredAmount), MessageTypeDefOf.RejectInput, historical: false);
				}
			}
		}
	}
}