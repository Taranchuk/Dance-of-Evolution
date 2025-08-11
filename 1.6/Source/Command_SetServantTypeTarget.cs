using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace DanceOfEvolution
{
	public class Command_SetServantTypeTarget : Command_Action
	{
		private readonly Hediff_FungalNexus fungalNexus;

		public Command_SetServantTypeTarget(Hediff_FungalNexus fungalNexus)
		{
			this.fungalNexus = fungalNexus;
			defaultLabel = "DE_SetServantTypeTarget".Translate($"DE_ServantType_{fungalNexus.servantTypeTarget}".Translate());
			defaultDesc = "DE_SetServantTypeTargetDesc".Translate();
			icon = GetIcon(fungalNexus.servantTypeTarget);
			action = ShowFloatMenu;
			Order = -90f;
		}

		private void ShowFloatMenu()
		{
			var options = new List<FloatMenuOption>();
			var servantTypes = new[] { ServantType.Small, ServantType.Medium, ServantType.Large,
			ServantType.Ghoul};

			foreach (var type in servantTypes)
            {
                Texture2D icon = GetIcon(type);
                options.Add(new FloatMenuOption(
                    $"DE_ServantType_{type}".Translate(),
                    () => fungalNexus.servantTypeTarget = type,
                    icon,
                    Color.white)
                );
            }

            Find.WindowStack.Add(new FloatMenu(options));
		}

        private static Texture2D GetIcon(ServantType type)
        {
            string iconPath = "UI/Icons/servant_size_";
            switch (type)
            {
                case ServantType.Small:
                    iconPath += "small";
                    break;
                case ServantType.Medium:
                    iconPath += "medium";
                    break;
                case ServantType.Large:
                    iconPath += "large";
                    break;
                case ServantType.Ghoul:
                    iconPath += "human";
                    break;
            }
            var icon = ContentFinder<Texture2D>.Get(iconPath);
            return icon;
        }
    }
}
