using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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

        public override string SettingsCategory()
        {
            return "SweetVictory.SettingsTitle".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            ModSettingsWindow.Draw(inRect);
            base.DoSettingsWindowContents(inRect);
        }
    }
}
