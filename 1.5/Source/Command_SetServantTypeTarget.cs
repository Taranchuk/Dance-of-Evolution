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
				options.Add(new FloatMenuOption(
					$"DE_ServantType_{type}".Translate(),
					() => fungalNexus.servantTypeTarget = type)
				);
			}

			Find.WindowStack.Add(new FloatMenu(options));
		}
	}
}
