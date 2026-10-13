using HarmonyLib;
using RimWorld;
using Verse;

namespace Sweet_Victory.Patches
{
    [HarmonyPatch(typeof(Hive), nameof(Hive.DeSpawn))]
    public static class Hive_DeSpawn_Patch
    {
        [HarmonyInfix(typeof(HiveUtility), nameof(HiveUtility.TotalSpawnedHivesCount))]
        public static int Postfix(int __result, Map map)
        {
            if (__result == 0)
            {
                VictoryEffectUtility.PlayDefeatSound(map);
            }

            return __result;
        }
    }
}
