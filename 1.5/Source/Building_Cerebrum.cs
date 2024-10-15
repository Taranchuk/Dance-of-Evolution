using System.Linq;
using System.Collections.Generic;
using RimWorld;
using Verse;
namespace DanceOfEvolution
{
	public class Building_Cerebrum : Building, IThingHolder, IStoreSettingsParent, IStorageGroupMember, IHaulDestination, IHaulSource, ISearchableContents
	{
		public float growth;
		private ThingOwner<Thing> innerContainer;
		private StorageSettings settings;
		private StorageGroup storageGroup;
		public IEnumerable<Thing> StoredItems => innerContainer.InnerListForReading;
		public bool StorageTabVisible => true;
		StorageGroup IStorageGroupMember.Group
		{
			get
			{
				return storageGroup;
			}
			set
			{
				storageGroup = value;
			}
		}
		bool IStorageGroupMember.DrawConnectionOverlay => base.Spawned;
		Map IStorageGroupMember.Map => base.MapHeld;
		string IStorageGroupMember.StorageGroupTag => def.building.storageGroupTag;
		StorageSettings IStorageGroupMember.StoreSettings => GetStoreSettings();
		StorageSettings IStorageGroupMember.ParentStoreSettings => GetParentStoreSettings();
		StorageSettings IStorageGroupMember.ThingStoreSettings => settings;
		bool IStorageGroupMember.DrawStorageTab => true;
		bool IStorageGroupMember.ShowRenameButton => base.Faction == Faction.OfPlayer;
		public ThingOwner SearchableContents => innerContainer;
		public Building_Cerebrum()
		{
			innerContainer = new ThingOwner<Thing>(this, oneStackOnly: false);
		}

		public override void PostMake()
		{
			base.PostMake();
			if (def.building.defaultStorageSettings.filter.allowedDefs != null)
			{
				def.building.defaultStorageSettings.filter.allowedDefs.Remove(DefsOf.DE_FungalSlurry);
				def.building.defaultStorageSettings.filter.allowedDefs.RemoveWhere(x => x.IsNutritionGivingIngestible is false);
				def.building.defaultStorageSettings.filter.allowedDefs.RemoveWhere(x => x.IsCorpse && x.race.IsMechanoid);
			}
			if (def.building.defaultStorageSettings.filter.thingDefs != null)
			{
				def.building.defaultStorageSettings.filter.thingDefs.Remove(DefsOf.DE_FungalSlurry);
				def.building.defaultStorageSettings.filter.thingDefs.RemoveWhere(x => x.IsNutritionGivingIngestible is false);
				def.building.defaultStorageSettings.filter.thingDefs.RemoveWhere(x => x.IsCorpse && x.race.IsMechanoid);
			}
			settings = new StorageSettings(this);
			if (def.building.defaultStorageSettings != null)
			{
				settings.CopyFrom(def.building.defaultStorageSettings);
			}
		}

		public StorageSettings GetStoreSettings()
		{
			return settings;
		}

		public StorageSettings GetParentStoreSettings()
		{
			return def.building.fixedStorageSettings;
		}

		public ThingOwner GetDirectlyHeldThings()
		{
			return innerContainer;
		}

		public bool Accepts(Thing t)
		{
			var nutrition = t.GetStatValue(StatDefOf.Nutrition);
			if (nutrition <= 0f)
			{
				return false;
			}
			if (growth >= 1)
			{
				return false;
			}
			var result = GetStoreSettings().AllowedToAccept(t) && innerContainer.CanAcceptAnyOf(t);
			return result;
		}

		private float CurrentNutrition()
		{
			return StoredItems.Sum(x => x.GetStatValue(StatDefOf.Nutrition) * x.stackCount);
		}

		public int SpaceRemainingFor(ThingDef _)
		{
			return (int)(100 - CurrentNutrition());
		}

		public void Notify_SettingsChanged()
		{
			if (base.Spawned)
			{
				base.MapHeld.listerHaulables.Notify_HaulSourceChanged(this);
			}
		}

		public void Notify_ItemAdded(Thing item)
		{
			base.MapHeld.listerHaulables.Notify_AddedThing(item);
		}

		public void Notify_ItemRemoved(Thing item)
		{
			base.MapHeld.listerHaulables.Notify_HaulSourceChanged(this);
		}

		public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
		{
			if (storageGroup != null)
			{
				storageGroup?.RemoveMember(this);
				storageGroup = null;
			}
			innerContainer?.TryDropAll(base.Position, base.Map, ThingPlaceMode.Near);
			base.DeSpawn(mode);
		}

		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
			Scribe_Deep.Look(ref settings, "settings", this);
			Scribe_References.Look(ref storageGroup, "storageGroup");
			Scribe_Values.Look(ref growth, "growth");
		}
	}
}