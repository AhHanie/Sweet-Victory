using RimWorld;
using Verse;

namespace Sweet_Victory
{
    [DefOf]
    public static class SweetVictoryThoughtDefOf
    {
        public static ThoughtDef SweetVictory_DefeatedRaid;

        [MayRequire("vanillaracesexpanded.fungoid")]
        public static ThoughtDef SweetVictory_DefeatedFungoidShip;

        static SweetVictoryThoughtDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SweetVictoryThoughtDefOf));
        }
    }
}
