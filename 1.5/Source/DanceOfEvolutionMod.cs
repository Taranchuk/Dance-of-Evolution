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
			new Harmony("DanceOfEvolutionMod").PatchAll();
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