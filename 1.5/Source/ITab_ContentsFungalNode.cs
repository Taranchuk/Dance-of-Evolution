using System.Collections.Generic;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    public class ITab_ContentsFungalNode : ITab_ContentsBase
	{
		private List<Thing> listInt = new List<Thing>();

		public override IList<Thing> container
		{
			get
			{
				var building = base.SelThing as Building_FungalNode;
				listInt.Clear();
				if (building != null)
				{
					listInt.AddRange(building.StoredItems);
				}
				return listInt;
			}
		}

		public ITab_ContentsFungalNode()
		{
			labelKey = "TabCasketContents";
			containedItemsKey = "ContainedItems";
			canRemoveThings = false;
		}
	}
}