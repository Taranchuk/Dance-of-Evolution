using HarmonyLib;
using Verse;

namespace DanceOfEvolution
{
    public class DanceOfEvolutionMod : Mod
	{
		public DanceOfEvolutionMod(ModContentPack pack) : base(pack)
		{
			new Harmony("DanceOfEvolutionMod").PatchAll();
		}
	}
}