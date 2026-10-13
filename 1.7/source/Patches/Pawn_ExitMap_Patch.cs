using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace Sweet_Victory.Patches
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.ExitMap))]
    public static class Pawn_ExitMap_Patch
    {
        [HarmonyInfix(typeof(Lord), nameof(Lord.Notify_PawnLost))]
        public static void Prefix(Lord __instance, Pawn pawn)
        {
            if (pawn.Faction == null || !pawn.Faction.HostileTo(Faction.OfPlayer))
            {
                return;
            }

            Map map = pawn.Map;
            if (map == null)
            {
                return;
            }

            if (!(pawn.carryTracker?.CarriedThing is Pawn carriedPawn) || !carriedPawn.IsColonist)
            {
                return;
            }

            map.GetComponent<RaidVictoryTrackerMapComponent>().NotifyRaidKidnappedColonist(__instance);
        }
    }
}
