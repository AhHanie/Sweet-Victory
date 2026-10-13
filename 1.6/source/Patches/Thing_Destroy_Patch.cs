using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace Sweet_Victory.Patches
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.Destroy))]
    public static class Thing_Destroy_Patch
    {
        private const string FungoidPackageId = "vanillaracesexpanded.fungoid";
        private const string InsectoidPackageId = "oskarpotocki.vfe.insectoid2";

        public static bool Prepare()
        {
            return ModsConfig.IsActive(FungoidPackageId) || ModsConfig.IsActive(InsectoidPackageId);
        }

        public static void Prefix(Thing __instance, ref ShipBuildingDestroyState __state)
        {
            Map map = __instance.MapHeld;

            ShipBuildingDestroyKind destroyKind = GetDestroyKind(__instance.def);
            if (destroyKind == ShipBuildingDestroyKind.None)
            {
                return;
            }

            __state = new ShipBuildingDestroyState
            {
                map = map,
                destroyKind = destroyKind,
                recipients = VictoryEffectUtility.GetMoodMemoryRecipients(map)
            };
        }

        public static void Postfix(ShipBuildingDestroyState __state)
        {
            if (__state == null)
            {
                return;
            }

            if (__state.destroyKind == ShipBuildingDestroyKind.FungoidShip)
            {
                VictoryEffectUtility.RewardThoughtRecipients(
                    __state.map,
                    __state.recipients,
                    SweetVictoryThoughtDefOf.SweetVictory_DefeatedFungoidShip);
                return;
            }

            if (__state.destroyKind == ShipBuildingDestroyKind.InfestedShip && !AnyInfestedShipBuildingsRemain(__state.map))
            {
                VictoryEffectUtility.RewardThoughtRecipients(
                    __state.map,
                    __state.recipients,
                    SweetVictoryThoughtDefOf.SweetVictory_DefeatedInfestedShip);
            }
        }

        private static ShipBuildingDestroyKind GetDestroyKind(ThingDef def)
        {
            if (ModsConfig.IsActive(FungoidPackageId) && def == SweetVictoryBuildingDefOf.VRE_FungoidShipPart)
            {
                return ShipBuildingDestroyKind.FungoidShip;
            }

            if (ModsConfig.IsActive(InsectoidPackageId)
                && (def == SweetVictoryBuildingDefOf.VFEI2_InfestedShipPart
                    || def == SweetVictoryBuildingDefOf.VFEI2_InfestedShipChunk))
            {
                return ShipBuildingDestroyKind.InfestedShip;
            }

            return ShipBuildingDestroyKind.None;
        }

        private static bool AnyInfestedShipBuildingsRemain(Map map)
        {
            return AnySpawnedThingOfDef(map, SweetVictoryBuildingDefOf.VFEI2_InfestedShipPart)
                || AnySpawnedThingOfDef(map, SweetVictoryBuildingDefOf.VFEI2_InfestedShipChunk);
        }

        private static bool AnySpawnedThingOfDef(Map map, ThingDef def)
        {
            List<Thing> things = map.listerThings.ThingsOfDef(def);
            for (int i = 0; i < things.Count; i++)
            {
                Thing thing = things[i];
                if (thing.Spawned && !thing.Destroyed)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public sealed class ShipBuildingDestroyState
    {
        public Map map;
        public ShipBuildingDestroyKind destroyKind;
        public List<Pawn> recipients;
    }

    public enum ShipBuildingDestroyKind
    {
        None,
        FungoidShip,
        InfestedShip
    }
}
