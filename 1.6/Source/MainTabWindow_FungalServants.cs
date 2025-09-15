using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace DanceOfEvolution
{
	public class MainTabWindow_FungalServants : MainTabWindow_PawnTable
	{
		public override PawnTableDef PawnTableDef => DefsOf.DE_FungalServants;

		public override IEnumerable<Pawn> Pawns => Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer)
			.Where(p => p.IsServant());
	}
}
