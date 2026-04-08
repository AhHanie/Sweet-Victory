using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Sweet_Victory.Patches
{
    [HarmonyPatch]
    public static class Building_FungoidShip_Destroy_Patch
    {
        private const string FungoidPackageId = "vanillaracesexpanded.fungoid";
        private const string FungoidShipTypeName = "VanillaRacesExpandedFungoid.Building_FungoidShip";

        public static bool Prepare()
        {
            return ModsConfig.IsActive(FungoidPackageId) && GetTargetMethod() != null;
        }

        public static MethodBase TargetMethod()
        {
            return GetTargetMethod();
        }

        public static void Prefix(Thing __instance, ref FungoidShipDestroyState __state)
        {
            Map map = __instance.MapHeld;
            __state = new FungoidShipDestroyState
            {
                map = map,
                recipients = VictoryEffectUtility.GetMoodMemoryRecipients(map)
            };
        }

        public static void Postfix(FungoidShipDestroyState __state)
        {
            if (__state == null)
            {
                return;
            }

            VictoryEffectUtility.RewardThoughtRecipients(
                __state.map,
                __state.recipients,
                SweetVictoryThoughtDefOf.SweetVictory_DefeatedFungoidShip);
        }

        private static MethodInfo GetTargetMethod()
        {
            Type shipType = AccessTools.TypeByName(FungoidShipTypeName);
            if (shipType == null)
            {
                return null;
            }

            return AccessTools.Method(shipType, nameof(Thing.Destroy), new[] { typeof(DestroyMode) });
        }
    }

    public sealed class FungoidShipDestroyState
    {
        public Map map;
        public List<Pawn> recipients;
    }
}
