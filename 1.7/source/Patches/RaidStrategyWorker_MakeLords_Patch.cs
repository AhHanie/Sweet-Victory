using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace Sweet_Victory.Patches
{
    [HarmonyPatch(typeof(RaidStrategyWorker), nameof(RaidStrategyWorker.MakeLords))]
    public static class RaidStrategyWorker_MakeLords_Patch
    {
        public static void Prefix(IncidentParms parms, ref RaidTrackingState __state)
        {
            __state = RaidTrackingPatchUtility.BeginTracking(parms);
        }

        public static void Postfix(IncidentParms parms, RaidTrackingState __state)
        {
            RaidTrackingPatchUtility.FinishTracking(__state);
        }
    }

    [HarmonyPatch(typeof(RaidStrategyWorker_PsychicRitualSiege), nameof(RaidStrategyWorker_PsychicRitualSiege.MakeLords))]
    public static class RaidStrategyWorker_PsychicRitualSiege_MakeLords_Patch
    {
        public static void Prefix(IncidentParms parms, ref RaidTrackingState __state)
        {
            __state = RaidTrackingPatchUtility.BeginTracking(parms);
        }

        public static void Postfix(IncidentParms parms, RaidTrackingState __state)
        {
            RaidTrackingPatchUtility.FinishTracking(__state);
        }
    }

    public sealed class RaidTrackingState
    {
        public Map map;
        public HashSet<int> existingLordLoadIds;
        public bool shouldTrack;
    }

    public static class RaidTrackingPatchUtility
    {
        public static RaidTrackingState BeginTracking(IncidentParms parms)
        {
            RaidTrackingState state = new RaidTrackingState();
            if (!(parms?.target is Map map) || parms.faction == null || !parms.faction.HostileTo(Faction.OfPlayer))
            {
                return state;
            }

            state.map = map;
            state.shouldTrack = true;
            state.existingLordLoadIds = new HashSet<int>();

            for (int i = 0; i < map.lordManager.lords.Count; i++)
            {
                state.existingLordLoadIds.Add(map.lordManager.lords[i].loadID);
            }

            return state;
        }

        public static void FinishTracking(RaidTrackingState state)
        {
            if (state == null || !state.shouldTrack || state.map == null)
            {
                return;
            }

            List<Lord> newLords = new List<Lord>();
            for (int i = 0; i < state.map.lordManager.lords.Count; i++)
            {
                Lord lord = state.map.lordManager.lords[i];
                if (state.existingLordLoadIds.Contains(lord.loadID))
                {
                    continue;
                }

                newLords.Add(lord);
            }

            state.map.GetComponent<RaidVictoryTrackerMapComponent>().RegisterRaid(newLords);
        }
    }
}
