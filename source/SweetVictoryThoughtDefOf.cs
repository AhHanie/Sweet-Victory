using RimWorld;
using Verse;

namespace Sweet_Victory
{
    [DefOf]
    public static class SweetVictoryThoughtDefOf
    {
        public static ThoughtDef SweetVictory_DefeatedRaid;

        public static ThoughtDef SweetVictory_DefeatedAncientDanger;

        [MayRequire("vanillaracesexpanded.fungoid")]
        public static ThoughtDef SweetVictory_DefeatedFungoidShip;

        [MayRequire("oskarpotocki.vfe.insectoid2")]
        public static ThoughtDef SweetVictory_DefeatedInfestedShip;

        static SweetVictoryThoughtDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SweetVictoryThoughtDefOf));
        }
    }
}
