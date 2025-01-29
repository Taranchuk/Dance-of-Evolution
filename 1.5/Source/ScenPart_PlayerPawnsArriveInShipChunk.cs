using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
namespace DanceOfEvolution
{
	public class ScenPart_PlayerPawnsArriveInShipChunk : ScenPart_PlayerPawnsArriveMethod
	{
		public override IEnumerable<Thing> PlayerStartingThings()
		{
			var things = new List<Thing>();
			var master = Find.GameInitData.startingAndOptionalPawns.First();
			var hediff_FungalNexus = master.GetFungalNexus();
			
			things.Add(MakeServant(PawnKindDefOf.Fingerspike, hediff_FungalNexus));
			things.Add(MakeServant(DefsOf.Noctol, hediff_FungalNexus));
			things.Add(MakeServant(PawnKindDefOf.Colonist, hediff_FungalNexus));
			return things;
		}
		
		public Pawn MakeServant(PawnKindDef pawnKindDef, Hediff_FungalNexus hediff_FungalNexus)
		{
			var servant = PawnGenerator.GeneratePawn(pawnKindDef, Faction.OfPlayer);
			servant.MakeServant(hediff_FungalNexus);
			return servant;
		}

		public override void GenerateIntoMap(Map map)
		{
			if (Find.GameInitData == null)
			{
				return;
			}
			var list = new List<Thing>();
			Thing thing = ThingMaker.MakeThing(ThingDefOf.ShipChunk);
			list.Add(thing);
			foreach (var startingAndOptionalPawn in Find.GameInitData.startingAndOptionalPawns)
			{
				list.Add(startingAndOptionalPawn);
			}
			foreach (var allPart in Find.Scenario.AllParts)
			{
				list.AddRange(allPart.PlayerStartingThings());
			}
			var intVec = MapGenerator.PlayerStartSpot;
			SkyfallerMaker.SpawnSkyfaller(ThingDefOf.ShipChunkIncoming, list, intVec, map);
		}
	}
}