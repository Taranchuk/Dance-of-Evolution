using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Ability), "StartCooldown")]
    public static class Ability_StartCooldown_Patch
    {
        public static void Prefix(Ability __instance, ref int ticks)
        {
            if (__instance.def == DefsOf.DE_SpawnBurrower)
            {
                var pawn = __instance.pawn;
                var burrowerSpawnSpeed = 1f;
                var coordinator = pawn.health.hediffSet.GetFirstHediffOfDef(DefsOf.DE_PsychicCoordinatorImplant) as Hediff_Level;
                if (coordinator != null)
                {
                    burrowerSpawnSpeed *= 1 + (coordinator.level * 0.5f);
                }
                ticks = (int)(ticks / burrowerSpawnSpeed);
            }
            else if (__instance.def == DefsOf.DE_DeadlifeSporeCloud)
            {
                var pawn = __instance.pawn;
                var psychicSensitivity = pawn.GetStatValue(StatDefOf.PsychicSensitivity);
                if (psychicSensitivity > 0)
                {
                    ticks = (int)(ticks / psychicSensitivity);
                }
            }
            else if (__instance.def == DefsOf.DE_Invisibility_Ability)
            {
                var pawn = __instance.pawn;
                var psychicSensitivity = pawn.GetStatValue(StatDefOf.PsychicSensitivity);
                if (psychicSensitivity > 0)
                {
                    ticks = (int)(ticks / psychicSensitivity);
                }
            }
        }
    }
}