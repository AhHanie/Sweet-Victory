using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace Sweet_Victory.Patches
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.ExitMap))]
    public static class Pawn_ExitMap_Patch
    {
        public static void Prefix(Pawn __instance)
        {
            if (__instance.Faction == null || !__instance.Faction.HostileTo(Faction.OfPlayer))
            {
                return;
            }

            Map map = __instance.Map;
            if (map == null)
            {
                return;
            }

            if (!(__instance.carryTracker?.CarriedThing is Pawn carriedPawn) || !carriedPawn.IsColonist)
            {
                return;
            }

            Lord lord = __instance.GetLord();
            map.GetComponent<RaidVictoryTrackerMapComponent>().NotifyRaidKidnappedColonist(lord);
        }
    }
}
