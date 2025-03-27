using HarmonyLib;
using RimWorld;
using Verse;

namespace DanceOfEvolution
{
    [HarmonyPatch(typeof(StatExtension), "GetStatValue")]
    public static class StatExtension_GetStatValue_Patch
    {
        public static void Postfix(Thing thing, StatDef stat, bool applyPostProcess, ref float __result)
        {
            if (stat == StatDefOf.MarketValue && thing is Pawn pawn)
            {
                if (Utils.IsServant(pawn) && pawn.RaceProps.IsAnomalyEntity)
                {
                    var servantType = pawn.GetBaseServantType()?.servantType;
                    if (servantType.HasValue)
                    {
                        switch (servantType.Value)
                        {
                            case ServantType.Small:
                                __result = 150f;
                                break;
                            case ServantType.Medium:
                                __result = 700f;
                                break;
                            case ServantType.Large:
                                __result = 1500f;
                                break;
                        }
                    }
                }
            }
        }
    }
}