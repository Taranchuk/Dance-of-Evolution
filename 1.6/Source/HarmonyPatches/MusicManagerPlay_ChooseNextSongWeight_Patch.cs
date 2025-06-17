using HarmonyLib;
using RimWorld;
using Verse;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DanceOfEvolution
{
    [HarmonyPatch]
    public static class MusicManagerPlay_ChooseNextSongWeight_Patch
    {
        private static MethodBase cachedTargetMethod = null;
        private static bool targetMethodSearched = false;
        public static MethodBase TargetMethod()
        {
            return cachedTargetMethod;
        }
        public static bool Prepare()
        {
            if (targetMethodSearched)
            {
                return cachedTargetMethod != null;
            }

            var declaringType = typeof(MusicManagerPlay);
            var nestedTypes = declaringType.GetNestedTypes(AccessTools.all);
            var allTypesToSearch = new List<Type> { declaringType }.Concat(nestedTypes);

            foreach (var type in allTypesToSearch)
            {
                cachedTargetMethod = AccessTools.GetDeclaredMethods(type)
                   .FirstOrDefault(m => m.ReturnType == typeof(float) &&
                                        m.GetParameters().Length == 1 &&
                                        m.GetParameters()[0].ParameterType == typeof(SongDef));

                if (cachedTargetMethod != null)
                {
                    Log.Message($"[DanceOfEvolution] Found target method by signature search in type {type.FullName}: {cachedTargetMethod.Name}");
                    break;
                }
            }

            targetMethodSearched = true;

            if (cachedTargetMethod == null)
            {
                Log.Error("[DanceOfEvolution] Cannot find the compiler-generated weight selector method. Song weight patch will not be applied.");
            }

            return cachedTargetMethod != null;
        }
        public static void Postfix(SongDef s, ref float __result)
        {
            if (__result > 0f && s != null && s.HasModExtension<SongExtension_Fungal>())
            {
                Map map = Find.AnyPlayerHomeMap ?? Find.CurrentMap;
                bool nexusPawnExists = map != null && map.mapPawns.AllPawnsSpawned.Any(p => p.health?.hediffSet?.HasHediff(DefsOf.DE_FungalNexus) ?? false);

                if (nexusPawnExists)
                {
                    __result *= 1.5f;
                }
            }
        }
    }
}