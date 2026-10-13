using HarmonyLib;
using RimWorld;
using Verse;

namespace Sweet_Victory.Patches
{
    [HarmonyPatch(typeof(Hive), nameof(Hive.DeSpawn))]
    public static class Hive_DeSpawn_Patch
    {
        public static void Prefix(Hive __instance, ref Map __state)
        {
            __state = __instance.Map;
        }

        public static void Postfix(Map __state)
        {
            if (HiveUtility.TotalSpawnedHivesCount(__state, filterFogged: true) != 0)
            {
                return;
            }

            VictoryEffectUtility.PlayDefeatSound(__state);
        }
    }
}
