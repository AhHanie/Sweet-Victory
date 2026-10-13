using HarmonyLib;
using Verse.AI.Group;

namespace Sweet_Victory.Patches
{
    [HarmonyPatch(typeof(LordManager), nameof(LordManager.RemoveLord))]
    public static class LordManager_RemoveLord_Patch
    {
        public static void Postfix(LordManager __instance, Lord oldLord)
        {
            __instance.map.GetComponent<RaidVictoryTrackerMapComponent>().NotifyLordRemoved(oldLord);
        }
    }
}
