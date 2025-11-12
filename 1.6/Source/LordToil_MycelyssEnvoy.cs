using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    [HotSwappable]
    public class LordToil_MycelyssEnvoy : LordToil
    {
        public Pawn envoy;
        private IntVec3 spot;
        private bool isDemand;
        private bool arrived;
        protected virtual float AllArrivedCheckRadius => 10f;
        public override IntVec3 FlagLoc => spot;
        public override bool AllowSatisfyLongNeeds => false;
        public LordToil_MycelyssEnvoy(Pawn envoy, IntVec3 spot, bool isDemand = false)
        {
            this.envoy = envoy;
            this.spot = spot;
            this.isDemand = isDemand;
        }

        public override void UpdateAllDuties()
        {
            if (arrived)
            {
                foreach (Pawn pawn in lord.ownedPawns)
                {
                    if (pawn == envoy)
                    {
                        pawn.mindState.duty = new PawnDuty(DutyDefOf.Defend, spot);
                        pawn.mindState.duty.radius = 5f;
                    }
                    else
                    {
                        pawn.mindState.duty = new PawnDuty(DutyDefOf.Follow, envoy, 6);
                    }
                }
            }
            else
            {
                for (int i = 0; i < lord.ownedPawns.Count; i++)
                {
                    var pawn = lord.ownedPawns[i];
                    var pawnDuty = pawn == envoy ? new PawnDuty(DutyDefOf.TravelOrLeave, spot) : new PawnDuty(DutyDefOf.Follow, envoy, 6);
                    pawnDuty.maxDanger = Danger.Deadly;
                    if (pawn.CurJobDef == JobDefOf.Wait_Wander)
                    {
                        pawn.jobs.StopAll();
                    }
                    pawn.mindState.duty = pawnDuty;
                }
            }
        }

        public override void Init()
        {
            base.Init();
            arrived = CheckAllArrived();
            UpdateAllDuties();
        }

        public override void LordToilTick()
        {
            base.LordToilTick();
            if (Find.TickManager.TicksGame % 205 == 0)
            {
                UpdateAllDuties();
            }
            if (!arrived && CheckAllArrived())
            {
                arrived = true;
            }
        }

        private bool CheckAllArrived()
        {
            for (int i = 0; i < lord.ownedPawns.Count; i++)
            {
                Pawn pawn = lord.ownedPawns[i];
                if (!pawn.Position.InHorDistOf(spot, AllArrivedCheckRadius) || !pawn.CanReach(spot, PathEndMode.ClosestTouch, Danger.Deadly))
                {
                    return false;
                }
            }
            return true;
        }

        public override IEnumerable<FloatMenuOption> ExtraFloatMenuOptions(Pawn forPawn, Pawn target)
        {
            if (forPawn == envoy)
            {
                if (isDemand)
                {
                    if (!PawnDemandUtility.GetValidDemandPawns().Any())
                    {
                        yield return new FloatMenuOption("DE_OfferPawns".Translate() + " (" + "DE_NoValidPawns".Translate() + ")", null);
                    }
                    else
                    {
                        yield return new FloatMenuOption("DE_OfferPawns".Translate(), delegate
                        {
                            Job job = JobMaker.MakeJob(DefsOf.DE_OfferPawns, envoy);
                            target.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                        });
                    }
                }
                else
                {
                    yield return new FloatMenuOption("DE_TalkToEnvoy".Translate(envoy.LabelShort), delegate
                        {
                            Job job = JobMaker.MakeJob(DefsOf.DE_TalkToEnvoy, envoy);
                            target.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                        });
                }
            }
        }
    }
}
