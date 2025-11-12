using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    public class LordToil_ExitMapFollowEnvoy : LordToil
    {
        public Pawn envoy;

        public override bool AllowSatisfyLongNeeds => false;

        public override bool AllowSelfTend => false;

        public LordToil_ExitMapFollowEnvoy(Pawn envoy)
        {
            this.envoy = envoy;
        }

        public override void UpdateAllDuties()
        {
            for (int i = 0; i < lord.ownedPawns.Count; i++)
            {
                Pawn pawn = lord.ownedPawns[i];
                if (pawn == envoy)
                {
                    pawn.mindState.duty = new PawnDuty(DutyDefOf.ExitMapBest);
                }
                else
                {
                    if (envoy != null && !envoy.Destroyed && envoy.Spawned)
                    {
                        pawn.mindState.duty = new PawnDuty(DutyDefOf.Follow, envoy, 5f);
                    }
                    else
                    {
                        pawn.mindState.duty = new PawnDuty(DutyDefOf.ExitMapBest);
                    }
                }
            }
        }

        public override void LordToilTick()
        {
            base.LordToilTick();
            if (Find.TickManager.TicksGame % 205 == 0)
            {
                bool envoyFound = false;
                for (int i = 0; i < lord.ownedPawns.Count; i++)
                {
                    Pawn pawn = lord.ownedPawns[i];
                    if (pawn == envoy)
                    {
                        envoyFound = true;
                        if (pawn.Destroyed || !pawn.Spawned)
                        {
                            UpdateAllDuties();
                            return;
                        }
                    }
                }
                if (!envoyFound)
                {
                    UpdateAllDuties();
                }
            }
        }
    }
}