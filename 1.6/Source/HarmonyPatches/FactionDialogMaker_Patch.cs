using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(FactionDialogMaker), nameof(FactionDialogMaker.FactionDialogFor))]
    public static class FactionDialogMaker_FactionDialogFor_Patch
    {
        public static void Prefix(Pawn negotiator, Faction faction)
        {
            if (faction?.def == DefsOf.DE_Mycelyss)
            {
                faction.def.techLevel = TechLevel.Industrial;
            }
        }

        public static void Postfix(Pawn negotiator, Faction faction)
        {
            if (faction?.def == DefsOf.DE_Mycelyss)
            {
                faction.def.techLevel = TechLevel.Animal;
            }
        }
    }
}
