using System.Collections.Generic;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
	public class GameComponent_ReanimateCorpses : GameComponent
	{
		public List<InfectedCorpse> infectedCorpses = new List<InfectedCorpse>();
		public static GameComponent_ReanimateCorpses Instance;
		public GameComponent_ReanimateCorpses()
		{
			Instance = this;
		}
		public GameComponent_ReanimateCorpses(Game game)
		{
			Instance = this;
		}

		public override void GameComponentTick()
		{
			base.GameComponentTick();
			for (int i = infectedCorpses.Count - 1; i >= 0; i--)
			{
				InfectedCorpse infectedCorpse = infectedCorpses[i];
				Corpse corpse = infectedCorpse.corpse;
				if (corpse.Destroyed || !corpse.Spawned)
				{
					infectedCorpses.RemoveAt(i);
				}
				else
				{
					infectedCorpse.Tick(out bool reanimated);
					if (reanimated)
					{
						infectedCorpses.RemoveAt(i);
					}
				}
			}
		}

		public void AddInfectedCorpse(Corpse corpse, Faction faction)
        {
            if (corpse.IsInfected() is false)
            {
                infectedCorpses.Add(new InfectedCorpse(corpse, faction));
            }
        }

        public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref infectedCorpses, "infectedCorpses", LookMode.Deep);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				infectedCorpses ??= new List<InfectedCorpse>();
			}
		}
	}
}