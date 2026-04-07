using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace Sweet_Victory
{
    public class RaidVictoryTrackerMapComponent : MapComponent
    {
        private List<RaidVictoryRecord> activeRaids = new List<RaidVictoryRecord>();

        public RaidVictoryTrackerMapComponent(Map map)
            : base(map)
        {
        }

        public void RegisterRaid(List<Lord> lords)
        {
            if (lords.Count == 0)
            {
                return;
            }

            RaidVictoryRecord record = new RaidVictoryRecord();
            for (int i = 0; i < lords.Count; i++)
            {
                Lord lord = lords[i];
                if (record.activeLordLoadIds.Contains(lord.loadID))
                {
                    continue;
                }

                record.activeLordLoadIds.Add(lord.loadID);
            }

            if (record.activeLordLoadIds.Count > 0)
            {
                activeRaids.Add(record);
            }
        }

        public void NotifyLordRemoved(Lord lord)
        {
            if (lord == null || lord.ownedPawns.Count > 0)
            {
                return;
            }

            for (int i = activeRaids.Count - 1; i >= 0; i--)
            {
                if (!activeRaids[i].RemoveLord(lord.loadID))
                {
                    continue;
                }

                if (activeRaids[i].activeLordLoadIds.Count == 0)
                {
                    activeRaids.RemoveAt(i);
                    VictoryEffectUtility.RewardRaidVictory(map);
                }

                return;
            }
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref activeRaids, "activeRaids", LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.PostLoadInit && activeRaids == null)
            {
                activeRaids = new List<RaidVictoryRecord>();
            }
        }

        public override void FinalizeInit()
        {
            HashSet<int> currentLordIds = new HashSet<int>();
            for (int i = 0; i < map.lordManager.lords.Count; i++)
            {
                currentLordIds.Add(map.lordManager.lords[i].loadID);
            }

            for (int i = activeRaids.Count - 1; i >= 0; i--)
            {
                activeRaids[i].RetainExistingLords(currentLordIds);
                if (activeRaids[i].activeLordLoadIds.Count == 0)
                {
                    activeRaids.RemoveAt(i);
                }
            }
        }
    }

    public class RaidVictoryRecord : IExposable
    {
        public List<int> activeLordLoadIds = new List<int>();

        public bool RemoveLord(int lordLoadId)
        {
            return activeLordLoadIds.Remove(lordLoadId);
        }

        public void RetainExistingLords(HashSet<int> currentLordIds)
        {
            activeLordLoadIds.RemoveAll((int lordLoadId) => !currentLordIds.Contains(lordLoadId));
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref activeLordLoadIds, "activeLordLoadIds", LookMode.Value);

            if (Scribe.mode == LoadSaveMode.PostLoadInit && activeLordLoadIds == null)
            {
                activeLordLoadIds = new List<int>();
            }
        }
    }
}
