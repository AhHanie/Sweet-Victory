using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace Sweet_Victory
{
    public static class VictoryEffectUtility
    {
        public static void PlayDefeatSound(Map map)
        {
            SoundDefOf.MechClusterDefeated.PlayOneShotOnCamera(map);
        }

        public static void RewardRaidVictory(Map map)
        {
            RewardMapPawns(map, SweetVictoryThoughtDefOf.SweetVictory_DefeatedRaid);
        }

        public static void RewardMapPawns(Map map, ThoughtDef thoughtDef)
        {
            if (map == null || thoughtDef == null)
            {
                return;
            }

            foreach (Pawn pawn in map.mapPawns.FreeColonistsSpawned)
            {
                pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(thoughtDef);
            }

            PlayDefeatSound(map);
        }

        public static List<Pawn> GetMoodMemoryRecipients(Map map)
        {
            if (map == null || map.Disposed)
            {
                return null;
            }

            List<Pawn> recipients = new List<Pawn>();
            List<Pawn> pawns = map.mapPawns.FreeColonists.ToList();
            for (int i = 0; i < pawns.Count; i++)
            {
                Pawn pawn = pawns[i];
                if (pawn?.needs?.mood?.thoughts?.memories == null)
                {
                    continue;
                }

                recipients.Add(pawn);
            }

            return recipients;
        }

        public static void RewardThoughtRecipients(Map map, List<Pawn> recipients, ThoughtDef thoughtDef)
        {
            for (int i = 0; i < recipients.Count; i++)
            {
                recipients[i]?.needs?.mood?.thoughts?.memories?.TryGainMemory(thoughtDef);
            }

            PlayDefeatSound(map);
        }
    }
}
