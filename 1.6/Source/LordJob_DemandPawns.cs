using RimWorld;
using System;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    public class LordJob_DemandPawns : LordJob
    {
        private Pawn envoy;
        private IntVec3 entranceSpot;
        private int expirationTick;

        public LordJob_DemandPawns()
        {
        }

        public LordJob_DemandPawns(Pawn envoy, IntVec3 entranceSpot)
        {
            this.envoy = envoy;
            this.entranceSpot = entranceSpot;
            this.expirationTick = Find.TickManager.TicksGame + GenDate.TicksPerDay;
        }

        public override StateGraph CreateGraph()
        {
            var stateGraph = new StateGraph();
            var lordToil_WaitForPlayer = new LordToil_MycelyssEnvoy(envoy, entranceSpot, true);
            stateGraph.StartingToil = lordToil_WaitForPlayer;

            var lordToil_Exit = new LordToil_ExitMapFollowEnvoy(envoy);
            stateGraph.AddToil(lordToil_Exit);

            var transitionToExitOnSuccess = new Transition(lordToil_WaitForPlayer, lordToil_Exit);
            transitionToExitOnSuccess.AddTrigger(new Trigger_Memo("PawnsDelivered"));
            transitionToExitOnSuccess.AddPreAction(new TransitionAction_Custom(() => expirationTick = int.MaxValue));
            stateGraph.AddTransition(transitionToExitOnSuccess);

            var transitionToExitOnFail = new Transition(lordToil_WaitForPlayer, lordToil_Exit);
            transitionToExitOnFail.AddTrigger(new Trigger_TicksPassed(expirationTick));
            transitionToExitOnFail.AddPreAction(new TransitionAction_Custom((Action)delegate
            {
                Faction mycelyssFaction = Find.FactionManager.FirstFactionOfDef(DefsOf.DE_Mycelyss);
                if (mycelyssFaction != null && !mycelyssFaction.HostileTo(Faction.OfPlayer))
                {
                    mycelyssFaction.SetRelation(new FactionRelation(Faction.OfPlayer, FactionRelationKind.Hostile));
                    Messages.Message("DE_MycelyssDemandFailedHostile".Translate(), MessageTypeDefOf.NegativeEvent);
                }
                GameComponent_CurseManager.Instance.mycelyssDemandActive = false;
            }));
            stateGraph.AddTransition(transitionToExitOnFail);

            return stateGraph;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref envoy, "envoy");
            Scribe_Values.Look(ref entranceSpot, "entranceSpot");
            Scribe_Values.Look(ref expirationTick, "expirationTick", 0);
        }
    }
}
