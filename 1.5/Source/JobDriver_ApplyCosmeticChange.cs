using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DanceOfEvolution
{
	[HotSwappable]
	public class JobDriver_ApplyCosmeticChange : JobDriver
	{
		private const TargetIndex GrowthSpotInd = TargetIndex.A;
		private const TargetIndex IngredientInd = TargetIndex.B;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (job.targetQueueB != null)
			{
				pawn.ReserveAsManyAsPossible(job.GetTargetQueue(TargetIndex.B), job);
				foreach (var target in job.GetTargetQueue(TargetIndex.B))
				{
					pawn.Map.physicalInteractionReservationManager.Reserve(pawn, job, target.Thing);
				}
			}
			return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
		}

		public override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(GrowthSpotInd);
			// Collect each required resource using predefined toils
			foreach (Toil item in CollectIngredientsToils(IngredientInd, GrowthSpotInd, GrowthSpotInd,
				subtractNumTakenFromJobCount: true, failIfStackCountLessThanJobCount: false))
			{
				yield return item;
			}
			// Go to GrowthSpot
			yield return Toils_Goto.GotoThing(GrowthSpotInd, PathEndMode.OnCell);
			yield return Toils_General.Wait(15000).WithProgressBarToilDelay(GrowthSpotInd);
			// Apply cosmetic change
			Toil applyCosmetic = ToilMaker.MakeToil();
			applyCosmetic.initAction = () =>
			{
				Thing growthSpot = job.GetTarget(GrowthSpotInd).Thing;
				Pawn fungalNexus = pawn;
				Hediff_FungalNexus fungalNexusHediff = fungalNexus.GetFungalNexus();
				HediffDef selectedCosmetic = fungalNexusHediff.selectedCosmetic;

				if (selectedCosmetic != null)
				{
					if (job.placedThings != null)
					{
						for (var i = job.placedThings.Count - 1; i >= 0; i--)
						{
							job.placedThings[i].thing?.Destroy();
						}
						pawn.Map.physicalInteractionReservationManager.ReleaseClaimedBy(pawn, job);
						job.placedThings = null;
					}
					
					// Remove existing cosmetics
					var allAttachments = fungalNexus.health.hediffSet.hediffs.FindAll(x => Startup.bodyAttachments.Contains(x.def)); // FindAll returns List, no ToList needed
					allAttachments.ForEach(x => fungalNexus.health.RemoveHediff(x));

					// Apply new cosmetic
					fungalNexus.health.AddHediff(selectedCosmetic);
				}
			};
			applyCosmetic.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return applyCosmetic;
		}

		public IEnumerable<Toil> CollectIngredientsToils(TargetIndex ingredientInd, TargetIndex billGiverInd, TargetIndex ingredientPlaceCellInd, bool subtractNumTakenFromJobCount = false, bool failIfStackCountLessThanJobCount = true)
		{
			Toil extract = Toils_JobTransforms.ExtractNextTargetFromQueue(ingredientInd);
			yield return extract;
			Toil getToHaulTarget = Toils_Goto.GotoThing(ingredientInd, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(ingredientInd).FailOnSomeonePhysicallyInteracting(ingredientInd);
			yield return getToHaulTarget;
			yield return Toils_Haul.StartCarryThing(ingredientInd, putRemainderInQueue: true, subtractNumTakenFromJobCount, failIfStackCountLessThanJobCount);
			yield return JobDriver_DoBill.JumpToCollectNextIntoHandsForBill(getToHaulTarget, TargetIndex.B);
			yield return Toils_Goto.GotoThing(billGiverInd, PathEndMode.OnCell).FailOnDestroyedOrNull(ingredientInd);
			Toil findPlaceTarget = Toils_JobTransforms.SetTargetToIngredientPlaceCell(billGiverInd, ingredientInd, ingredientPlaceCellInd);
			yield return findPlaceTarget;
			yield return PlaceHauledThingInCell(ingredientPlaceCellInd, findPlaceTarget, storageMode: false);
			yield return Toils_Jump.JumpIfHaveTargetInQueue(ingredientInd, extract);
		}
		
		public static Toil PlaceHauledThingInCell(TargetIndex cellInd, Toil nextToilOnPlaceFailOrIncomplete, bool storageMode, bool tryStoreInSameStorageIfSpotCantHoldWholeStack = false)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				IntVec3 cell = curJob.GetTarget(cellInd).Cell;
				if (actor.carryTracker.CarriedThing == null)
				{
					Log.Error(string.Concat(actor, " tried to place hauled thing in cell but is not hauling anything."));
				}
				else
				{
					SlotGroup slotGroup = actor.Map.haulDestinationManager.SlotGroupAt(cell);
					if (slotGroup != null && slotGroup.Settings.AllowedToAccept(actor.carryTracker.CarriedThing))
					{
						actor.Map.designationManager.TryRemoveDesignationOn(actor.carryTracker.CarriedThing, DesignationDefOf.Haul);
					}
					Action<Thing, int> placedAction = delegate (Thing th, int added)
					{
						if (curJob.placedThings == null)
						{
							curJob.placedThings = new List<ThingCountClass>();
						}
						ThingCountClass thingCountClass = curJob.placedThings.Find((ThingCountClass x) => x.thing == th);
						if (thingCountClass != null)
						{
							thingCountClass.Count += added;
						}
						else
						{
							curJob.placedThings.Add(new ThingCountClass(th, added));
						}
					};

					if (!actor.carryTracker.TryDropCarriedThing(cell, ThingPlaceMode.Direct, out var _, placedAction))
					{
						if (storageMode)
						{
							if (nextToilOnPlaceFailOrIncomplete != null && ((tryStoreInSameStorageIfSpotCantHoldWholeStack && StoreUtility.TryFindBestBetterStoreCellForIn(actor.carryTracker.CarriedThing, actor, actor.Map, StoragePriority.Unstored, actor.Faction, cell.GetSlotGroup(actor.Map), out var foundCell)) || StoreUtility.TryFindBestBetterStoreCellFor(actor.carryTracker.CarriedThing, actor, actor.Map, StoragePriority.Unstored, actor.Faction, out foundCell)))
							{
								if (actor.CanReserve(foundCell))
								{
									actor.Reserve(foundCell, actor.CurJob);
								}
								actor.CurJob.SetTarget(cellInd, foundCell);
								actor.jobs.curDriver.JumpToToil(nextToilOnPlaceFailOrIncomplete);
							}
							else
							{
								Job job = HaulAIUtility.HaulAsideJobFor(actor, actor.carryTracker.CarriedThing);
								if (job != null)
								{
									curJob.targetA = job.targetA;
									curJob.targetB = job.targetB;
									curJob.targetC = job.targetC;
									curJob.count = job.count;
									curJob.haulOpportunisticDuplicates = job.haulOpportunisticDuplicates;
									curJob.haulMode = job.haulMode;
									actor.jobs.curDriver.JumpToToil(nextToilOnPlaceFailOrIncomplete);
								}
								else
								{
									Log.Error(string.Concat("Incomplete haul for ", actor, ": Could not find anywhere to put ", actor.carryTracker.CarriedThing, " near ", actor.Position, ". Destroying. This should never happen!"));
									actor.carryTracker.CarriedThing.Destroy();
								}
							}
						}
						else if (nextToilOnPlaceFailOrIncomplete != null)
						{
							actor.jobs.curDriver.JumpToToil(nextToilOnPlaceFailOrIncomplete);
						}
					}
				}
			};
			return toil;
		}
	}
}