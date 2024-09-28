using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(Pawn), "IsColonistPlayerControlled", MethodType.Getter)]
    public static class Pawn_IsColonistPlayerControlled_Patch
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(Pawn __instance, ref bool __result)
        {
            if (__instance.IsServant())
            {
                if (__instance.Spawned && __instance.Faction == Faction.OfPlayer && __instance.MentalStateDef == null)
                {
                    __result = true;
                }
            }
        }
    }
}