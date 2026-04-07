using RimWorld;
using Verse;

namespace Sweet_Victory
{
    [DefOf]
    public static class SweetVictoryThoughtDefOf
    {
        public static ThoughtDef DefeatedRaid;

        static SweetVictoryThoughtDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SweetVictoryThoughtDefOf));
        }
    }
}
