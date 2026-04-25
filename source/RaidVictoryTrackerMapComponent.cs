using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace Sweet_Victory
{
    public class RaidVictoryTrackerMapComponent : MapComponent
    {
        private const int CheckIntervalTicks = 250;
        private const int VictoryRecordExpiryTicks = 3 * GenDate.TicksPerDay;
        private const int AncientDangerHostileRadius = 50;
        private const int AncientDangerHostileRadiusSquared = AncientDangerHostileRadius * AncientDangerHostileRadius;

        private List<RaidVictoryRecord> activeRaids = new List<RaidVictoryRecord>();
        private bool ancientDangerVictoryRewarded;
        private bool ancientDangerTrackingActive;
        private int ancientDangerTrackingStartTick;
        private List<Pawn> ancientDangerHostiles = new List<Pawn>();

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
            record.registeredTick = Find.TickManager.TicksGame;

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

        public void NotifyAncientCryptosleepCasketOpened(IntVec3 casketPosition)
        {
            if (ancientDangerVictoryRewarded || ancientDangerTrackingActive || AnyAncientCryptosleepCasketHasContents())
            {
                return;
            }

            ancientDangerTrackingStartTick = Find.TickManager.TicksGame;
            ancientDangerHostiles = GetActiveHostilePawnsNear(casketPosition);

            if (ancientDangerHostiles.Count == 0)
            {
                RewardAncientDangerVictory();
                return;
            }

            ancientDangerTrackingActive = true;
        }

        public override void MapComponentTick()
        {
            if (Find.TickManager.TicksGame % CheckIntervalTicks != 0)
            {
                return;
            }

            RemoveExpiredRaidRecords();
            CheckAncientDangerVictory();
        }

        private void CheckAncientDangerVictory()
        {
            if (!ancientDangerTrackingActive)
            {
                return;
            }

            if (Find.TickManager.TicksGame - ancientDangerTrackingStartTick >= VictoryRecordExpiryTicks)
            {
                ancientDangerTrackingActive = false;
                ancientDangerHostiles.Clear();
                return;
            }

            for (int i = 0; i < ancientDangerHostiles.Count; i++)
            {
                if (IsActiveHostilePawn(ancientDangerHostiles[i]))
                {
                    return;
                }
            }

            ancientDangerTrackingActive = false;
            ancientDangerHostiles.Clear();
            RewardAncientDangerVictory();
        }

        private void RewardAncientDangerVictory()
        {
            if (ancientDangerVictoryRewarded)
            {
                return;
            }

            ancientDangerVictoryRewarded = true;
            VictoryEffectUtility.RewardMapPawns(map, SweetVictoryThoughtDefOf.SweetVictory_DefeatedAncientDanger);
        }

        private List<Pawn> GetActiveHostilePawnsNear(IntVec3 casketPosition)
        {
            List<Pawn> hostiles = new List<Pawn>();
            if (!casketPosition.IsValid)
            {
                return hostiles;
            }

            IReadOnlyList<Pawn> pawns = map.mapPawns.AllPawnsSpawned;
            for (int i = 0; i < pawns.Count; i++)
            {
                Pawn pawn = pawns[i];
                if (IsActiveHostilePawn(pawn)
                    && pawn.PositionHeld.IsValid
                    && pawn.PositionHeld.DistanceToSquared(casketPosition) <= AncientDangerHostileRadiusSquared)
                {
                    hostiles.Add(pawn);
                }
            }

            return hostiles;
        }

        private bool AnyAncientCryptosleepCasketHasContents()
        {
            return AnyCasketOfDefHasContents(ThingDefOf.AncientCryptosleepCasket)
                || AnyCasketOfDefHasContents(ThingDefOf.AncientCryptosleepPod);
        }

        private bool AnyCasketOfDefHasContents(ThingDef casketDef)
        {
            List<Thing> caskets = map.listerThings.ThingsOfDef(casketDef);
            for (int i = 0; i < caskets.Count; i++)
            {
                if (caskets[i] is Building_Casket casket && !casket.Destroyed && casket.HasAnyContents)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsActiveHostilePawn(Pawn pawn)
        {
            return pawn != null
                && !pawn.Destroyed
                && !pawn.Dead
                && !pawn.Downed
                && pawn.Faction != null
                && pawn.Faction.HostileTo(Faction.OfPlayer);
        }

        private static bool RaidRecordExpired(RaidVictoryRecord record)
        {
            return Find.TickManager.TicksGame - record.registeredTick >= VictoryRecordExpiryTicks;
        }

        private void RemoveExpiredRaidRecords()
        {
            for (int i = activeRaids.Count - 1; i >= 0; i--)
            {
                if (RaidRecordExpired(activeRaids[i]))
                {
                    activeRaids.RemoveAt(i);
                }
            }
        }

        public void NotifyLordRemoved(Lord lord)
        {
            if (lord == null || lord.ownedPawns.Count > 0)
            {
                return;
            }

            RemoveExpiredRaidRecords();

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
            Scribe_Values.Look(ref ancientDangerVictoryRewarded, "ancientDangerVictoryRewarded", defaultValue: false);
            Scribe_Values.Look(ref ancientDangerTrackingActive, "ancientDangerTrackingActive", defaultValue: false);
            Scribe_Values.Look(ref ancientDangerTrackingStartTick, "ancientDangerTrackingStartTick", 0);
            Scribe_Collections.Look(ref ancientDangerHostiles, "ancientDangerHostiles", LookMode.Reference);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (activeRaids == null)
                {
                    activeRaids = new List<RaidVictoryRecord>();
                }

                if (ancientDangerHostiles == null)
                {
                    ancientDangerHostiles = new List<Pawn>();
                }

                activeRaids.RemoveAll((RaidVictoryRecord record) => record.thoughtDef == SweetVictoryThoughtDefOf.SweetVictory_DefeatedAncientDanger);
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
        public ThoughtDef thoughtDef;
        public int registeredTick;

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
            Scribe_Defs.Look(ref thoughtDef, "thoughtDef");
            Scribe_Values.Look(ref registeredTick, "registeredTick", 0);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (activeLordLoadIds == null)
                {
                    activeLordLoadIds = new List<int>();
                }

                if (registeredTick <= 0)
                {
                    registeredTick = Find.TickManager.TicksGame;
                }
            }
        }
    }
}
