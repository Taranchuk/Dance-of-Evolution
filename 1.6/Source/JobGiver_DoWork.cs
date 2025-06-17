using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
namespace DanceOfEvolution
{
    public class JobGiver_DoWork : JobGiver_Work
	{
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_DoWork obj = (JobGiver_DoWork)base.DeepCopy(resolve);
			obj.workTypes = workTypes;
			obj.workgivers = workgivers;
			return obj;
		}

		public List<WorkTypeDef> workTypes;
		public List<WorkGiverDef> workgivers;

		public override float GetPriority(Pawn pawn)
		{
			return 9f;
		}

		private bool PawnCanUseWorkGiverOverride(Pawn pawn, WorkGiver giver)
		{
			if (pawn.WorkTagIsDisabled(giver.def.workTags))
			{
				return false;
			}
			if (giver.def.workType != null && pawn.WorkTypeIsDisabled(giver.def.workType))
			{
				return false;
			}
			if (giver.ShouldSkip(pawn))
			{
				return false;
			}
			if (giver.MissingRequiredCapacity(pawn) != null)
			{
				return false;
			}
			if (pawn.RaceProps.IsMechanoid && !giver.def.canBeDoneByMechs)
			{
				return false;
			}
			return true;
		}

		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
		{
			if (pawn.workSettings is null)
			{
				pawn.workSettings = new Pawn_WorkSettings(pawn);
				pawn.workSettings.EnableAndInitialize();
			}
			List<WorkGiver> list = new List<WorkGiver>();
			if (workgivers != null)
			{
				list.AddRange(workgivers.Select(x => x.Worker));
			}
			if (workTypes != null)
			{
				list.AddRange(workTypes.SelectMany(x => x.workGiversByPriority).Select(x => x.Worker));
			}
			int num = -999;
			TargetInfo bestTargetOfLastPriority = TargetInfo.Invalid;
			WorkGiver_Scanner scannerWhoProvidedTarget = null;
			for (int j = 0; j < list.Count; j++)
			{
				WorkGiver workGiver = list[j];
				if (workGiver.def.priorityInType != num && bestTargetOfLastPriority.IsValid)
				{
					break;
				}
				if (!PawnCanUseWorkGiverOverride(pawn, workGiver))
				{
					continue;
				}
				try
				{
					Job job2 = workGiver.NonScanJob(pawn);
					if (job2 != null)
					{
						if (pawn.jobs.debugLog)
						{
							pawn.jobs.DebugLogEvent($"JobGiver_Work produced non-scan Job {job2.ToStringSafe()} from {workGiver}");
						}
						return new ThinkResult(job2, this, list[j].def.tagToGive);
					}
					WorkGiver_Scanner scanner;
					IntVec3 pawnPosition;
					float closestDistSquared;
					float bestPriority;
					bool prioritized;
					bool allowUnreachable;
					Danger maxPathDanger;
					if ((scanner = workGiver as WorkGiver_Scanner) != null)
					{
						if (scanner.def.scanThings)
						{
							IEnumerable<Thing> enumerable = scanner.PotentialWorkThingsGlobal(pawn);
							bool flag = pawn.carryTracker?.CarriedThing != null && scanner.PotentialWorkThingRequest.Accepts(pawn.carryTracker.CarriedThing) && Validator(pawn.carryTracker.CarriedThing);
							Thing thing;
							if (scanner.Prioritized)
							{
								IEnumerable<Thing> searchSet = enumerable ?? pawn.Map.listerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
								thing = ((!scanner.AllowUnreachable) ? GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, searchSet, scanner.PathEndMode, TraverseParms.For(pawn, scanner.MaxPathDanger(pawn)), 9999f, Validator, (Thing x) => scanner.GetPriority(pawn, x)) : GenClosest.ClosestThing_Global(pawn.Position, searchSet, 99999f, Validator, (Thing x) => scanner.GetPriority(pawn, x)));
								if (flag)
								{
									if (thing != null)
									{
										float num2 = scanner.GetPriority(pawn, pawn.carryTracker.CarriedThing);
										float num3 = scanner.GetPriority(pawn, thing);
										if (num2 >= num3)
										{
											thing = pawn.carryTracker.CarriedThing;
										}
									}
									else
									{
										thing = pawn.carryTracker.CarriedThing;
									}
								}
							}
							else if (flag)
							{
								thing = pawn.carryTracker.CarriedThing;
							}
							else if (scanner.AllowUnreachable)
							{
								IEnumerable<Thing> searchSet2 = enumerable ?? pawn.Map.listerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
								thing = GenClosest.ClosestThing_Global(pawn.Position, searchSet2, 99999f, Validator);
							}
							else
							{
								thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, scanner.PotentialWorkThingRequest, scanner.PathEndMode, TraverseParms.For(pawn, scanner.MaxPathDanger(pawn)), 9999f, Validator, enumerable, 0, scanner.MaxRegionsToScanBeforeGlobalSearch, enumerable != null);
							}
							if (thing != null)
							{
								bestTargetOfLastPriority = thing;
								scannerWhoProvidedTarget = scanner;
							}
						}
						if (scanner.def.scanCells)
						{
							pawnPosition = pawn.Position;
							closestDistSquared = 99999f;
							bestPriority = float.MinValue;
							prioritized = scanner.Prioritized;
							allowUnreachable = scanner.AllowUnreachable;
							maxPathDanger = scanner.MaxPathDanger(pawn);
							IEnumerable<IntVec3> enumerable2 = scanner.PotentialWorkCellsGlobal(pawn);
							if (enumerable2 is IList<IntVec3> list2)
							{
								for (int k = 0; k < list2.Count; k++)
								{
									ProcessCell(list2[k]);
								}
							}
							else
							{
								foreach (IntVec3 item in enumerable2)
								{
									ProcessCell(item);
								}
							}
						}
					}
					void ProcessCell(IntVec3 c)
					{
						bool flag2 = false;
						float num4 = (c - pawnPosition).LengthHorizontalSquared;
						float num5 = 0f;
						if (prioritized)
						{
							if (!c.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, c))
							{
								if (!allowUnreachable && !pawn.CanReach(c, scanner.PathEndMode, maxPathDanger))
								{
									return;
								}
								num5 = scanner.GetPriority(pawn, c);
								if (num5 > bestPriority || (num5 == bestPriority && num4 < closestDistSquared))
								{
									flag2 = true;
								}
							}
						}
						else if (num4 < closestDistSquared && !c.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, c))
						{
							if (!allowUnreachable && !pawn.CanReach(c, scanner.PathEndMode, maxPathDanger))
							{
								return;
							}
							flag2 = true;
						}
						if (flag2)
						{
							bestTargetOfLastPriority = new TargetInfo(c, pawn.Map);
							scannerWhoProvidedTarget = scanner;
							closestDistSquared = num4;
							bestPriority = num5;
						}
					}
					bool Validator(Thing t)
					{
						if (!t.IsForbidden(pawn))
						{
							return scanner.HasJobOnThing(pawn, t);
						}
						return false;
					}
				}
				catch (Exception ex)
				{
					Log.Error(string.Concat(pawn, " threw exception in WorkGiver ", workGiver.def.defName, ": ", ex.ToString()));
				}
				finally
				{
				}

				if (bestTargetOfLastPriority.IsValid)
				{
					Job job3 = ((!bestTargetOfLastPriority.HasThing) ? scannerWhoProvidedTarget.JobOnCell(pawn, bestTargetOfLastPriority.Cell) : scannerWhoProvidedTarget.JobOnThing(pawn, bestTargetOfLastPriority.Thing));
					if (job3 != null)
					{
						job3.workGiverDef = scannerWhoProvidedTarget.def;
						if (pawn.jobs.debugLog)
						{
							pawn.jobs.DebugLogEvent($"JobGiver_Work produced scan Job {job3.ToStringSafe()} from {scannerWhoProvidedTarget}");
						}
						return new ThinkResult(job3, this, list[j].def.tagToGive);
					}
					Log.ErrorOnce(string.Concat(scannerWhoProvidedTarget, " provided target ", bestTargetOfLastPriority, " but yielded no actual job for pawn ", pawn, ". The CanGiveJob and JobOnX methods may not be synchronized."), 6112651);
				}
				num = workGiver.def.priorityInType;
			}
			return ThinkResult.NoJob;
		}
	}
}
