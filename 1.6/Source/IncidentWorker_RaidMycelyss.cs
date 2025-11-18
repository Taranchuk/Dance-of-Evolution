using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace DanceOfEvolution
{
    public class IncidentWorker_RaidMycelyss : IncidentWorker_Raid
    {
        public override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            return ((Map)parms.target).attackTargetsCache.TargetsHostileToColony.Where((IAttackTarget p) => GenHostility.IsActiveThreatToPlayer(p)).Sum((IAttackTarget p) => (p is Pawn pawn) ? pawn.kindDef.combatPower : 0f) > 120f;
        }
        public override bool TryResolveRaidFaction(IncidentParms parms)
        {
            if (parms.faction != null)
            {
                return true;
            }
            if (!CandidateFactions(parms).Any<Faction>())
            {
                return false;
            }
            parms.faction = CandidateFactions(parms).RandomElementByWeight((Faction fac) => (float)fac.PlayerGoodwill + 120.00001f);
            return true;
        }
        public override bool FactionCanBeGroupSource(Faction f, IncidentParms parms, bool desperate = false)
        {
            if (f.def == DefsOf.DE_Mycelyss && f.PlayerRelationKind == FactionRelationKind.Ally)
            {
                return base.FactionCanBeGroupSource(f, parms, desperate);
            }
            return false;
        }

        public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            parms.raidStrategy = RaidStrategyDefOf.ImmediateAttackFriendly;
            parms.raidArrivalMode = DefsOf.DE_QuantumTunnelArrival;
        }

        public override void ResolveRaidPoints(IncidentParms parms)
        {
            if (parms.points <= 0f)
            {
                parms.points = StorytellerUtility.DefaultThreatPointsNow(parms.target);
            }
        }

        public override string GetLetterLabel(IncidentParms parms)
        {
            return parms.raidStrategy.letterLabelFriendly + ": " + parms.faction.Name;
        }

        public override string GetLetterText(IncidentParms parms, List<Pawn> pawns)
        {
            string text = string.Format(parms.raidArrivalMode.textFriendly, parms.faction.def.pawnsPlural, parms.faction.Name.ApplyTag(parms.faction));
            text += "\n\n";
            text += parms.raidStrategy.arrivalTextFriendly;
            Pawn pawn = pawns.Find((Pawn x) => x.Faction.leader == x);
            if (pawn != null)
            {
                text += "\n\n";
                text += "FriendlyRaidLeaderPresent".Translate(pawn.Faction.def.pawnsPlural, pawn.LabelShort, pawn.Named("LEADER"));
            }
            return text;
        }

        public override LetterDef GetLetterDef()
        {
            return LetterDefOf.PositiveEvent;
        }

        public override string GetRelatedPawnsInfoLetterText(IncidentParms parms)
        {
            return "LetterRelatedPawnsRaidFriendly".Translate(Faction.OfPlayer.def.pawnsPlural, parms.faction.def.pawnsPlural);
        }
    }
}