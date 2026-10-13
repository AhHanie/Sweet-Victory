using HarmonyLib;
using RimWorld;
using Verse;

namespace Sweet_Victory.Patches
{
    [HarmonyPatch(typeof(Building_AncientCryptosleepCasket), nameof(Building_AncientCryptosleepCasket.EjectContents))]
    public static class Building_AncientCryptosleepCasket_EjectContents_Patch
    {
        public static void Postfix(Building_AncientCryptosleepCasket __instance)
        {
            Map map = __instance.Map;
            map?.GetComponent<RaidVictoryTrackerMapComponent>()?.NotifyAncientCryptosleepCasketOpened(__instance.Position);
        }
    }
}
