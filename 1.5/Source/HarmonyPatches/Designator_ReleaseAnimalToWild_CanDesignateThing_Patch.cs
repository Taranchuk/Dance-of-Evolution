using RimWorld;
using Verse;
using HarmonyLib;
namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Designator_ReleaseAnimalToWild), nameof(Designator_ReleaseAnimalToWild.CanDesignateThing))]
    public static class Designator_ReleaseAnimalToWild_CanDesignateThing_Patch
    {
        public static bool Prefix(Thing t)
        {
            if (t is Pawn pawn && pawn.IsServant())
            {
                return false;
            }
            return true;
        }
    }
}