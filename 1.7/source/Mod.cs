using HarmonyLib;
using Verse;

namespace Sweet_Victory
{
    public class Mod : Verse.Mod
    {
        public Mod(ModContentPack content) : base(content)
        {
            LongEventHandler.QueueLongEvent(Init, "SweetVictory.LoadingLabel", doAsynchronously: true, null);
        }

        public static void Init()
        {
            new Harmony("sk.sweetvictory").PatchAll();
        }
    }
}
