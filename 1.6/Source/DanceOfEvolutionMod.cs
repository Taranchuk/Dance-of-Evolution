using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace DanceOfEvolution
{
	public class DanceOfEvolutionMod : Mod
	{
		public static DanceOfEvolutionSettings settings;

		public DanceOfEvolutionMod(ModContentPack content) : base(content)
		{
			settings = GetSettings<DanceOfEvolutionSettings>();
			var assembly = Assembly.GetExecutingAssembly();
			var harmony = new Harmony("DanceOfEvolutionMod");
			AccessTools.GetTypesFromAssembly(assembly).Do(delegate (Type type)
			{
				try
				{
					harmony.CreateClassProcessor(type).Patch();
				}
				catch (Exception ex)
				{
					Log.Error("Failed to patch " + type + ": " + ex.Message);
				}
			});
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			Listing_Standard listingStandard = new Listing_Standard();
			listingStandard.Begin(inRect);
			listingStandard.CheckboxLabeled("DE_ShowMushroomCap".Translate(), ref settings.showMushroomCap, "DE_ShowMushroomCapTooltip".Translate());
			listingStandard.CheckboxLabeled("DE_UseTimelessHead".Translate(), ref settings.useTimelessHead, "DE_UseTimelessHeadTooltip".Translate());
			listingStandard.End();
			base.DoSettingsWindowContents(inRect);
		}

		public override string SettingsCategory()
		{
			return Content.Name;
		}
	}
}
