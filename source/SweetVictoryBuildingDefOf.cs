using Verse;
using RimWorld;

namespace Sweet_Victory
{
    [DefOf]
    public static class SweetVictoryBuildingDefOf
    {
        [MayRequire("oskarpotocki.vfe.insectoid2")]
        public static ThingDef VFEI2_InfestedShipPart;
        [MayRequire("oskarpotocki.vfe.insectoid2")]
        public static ThingDef VFEI2_InfestedShipChunk;

        [MayRequire("vanillaracesexpanded.fungoid")]
        public static ThingDef VRE_FungoidShipPart;
        static SweetVictoryBuildingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SweetVictoryBuildingDefOf));
        }
    }
}
