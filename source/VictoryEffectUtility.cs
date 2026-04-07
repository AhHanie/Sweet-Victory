using RimWorld;
using Verse;
using Verse.Sound;

namespace Sweet_Victory
{
    public static class VictoryEffectUtility
    {
        public static bool HasFreeColonists(Map map)
        {
            if (map == null || map.Disposed)
            {
                return false;
            }

            foreach (Pawn pawn in map.mapPawns.FreeColonistsSpawned)
            {
                if (pawn != null)
                {
                    return true;
                }
            }

            return false;
        }

        public static void PlayDefeatSound(Map map)
        {
            if (map == null || map.Disposed)
            {
                return;
            }

            SoundDefOf.MechClusterDefeated.PlayOneShotOnCamera(map);
        }

        public static void RewardRaidVictory(Map map)
        {
            if (!HasFreeColonists(map))
            {
                return;
            }

            foreach (Pawn pawn in map.mapPawns.FreeColonistsSpawned)
            {
                pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(SweetVictoryThoughtDefOf.DefeatedRaid);
            }

            PlayDefeatSound(map);
        }
    }
}
