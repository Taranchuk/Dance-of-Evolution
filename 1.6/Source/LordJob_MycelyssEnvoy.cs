using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    public class LordJob_MycelyssEnvoy : LordJob
    {
        private Pawn envoy;
        private IntVec3 destinationCell;
        public bool makeHostileOnExit = false;

        public LordJob_MycelyssEnvoy() { }

        public LordJob_MycelyssEnvoy(Pawn envoy, IntVec3 destinationCell)
        {
            this.envoy = envoy;
            this.destinationCell = destinationCell;
        }

        public override StateGraph CreateGraph()
        {
            var stateGraph = new StateGraph();
            var lordToil_TravelAndWait = new LordToil_MycelyssEnvoy(envoy, destinationCell);
            stateGraph.StartingToil = lordToil_TravelAndWait;

            var lordToil_Exit = new LordToil_ExitMapFollowEnvoy(envoy);
            stateGraph.AddToil(lordToil_Exit);

            var transitionToExit = new Transition(lordToil_TravelAndWait, lordToil_Exit);
            transitionToExit.AddTrigger(new Trigger_Memo("Leave"));
            stateGraph.AddTransition(transitionToExit);


            return stateGraph;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            if (makeHostileOnExit)
            {
                Faction faction = Find.FactionManager.FirstFactionOfDef(DefsOf.DE_Mycelyss);
                if (faction != null && !faction.HostileTo(Faction.OfPlayer))
                {
                    faction.SetRelation(new FactionRelation(Faction.OfPlayer, FactionRelationKind.Hostile));
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref envoy, "envoy");
            Scribe_Values.Look(ref destinationCell, "destinationCell");
            Scribe_Values.Look(ref makeHostileOnExit, "makeHostileOnExit", false);
        }
    }
}
