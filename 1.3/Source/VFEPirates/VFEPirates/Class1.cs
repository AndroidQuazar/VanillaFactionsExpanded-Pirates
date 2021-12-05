using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VFEPirates
{
    public class VFEPiratesMod : Mod
    {
        public static Harmony harmony;
        public VFEPiratesMod(ModContentPack modContentPack) : base(modContentPack)
        {
            harmony = new Harmony("VFEPirates.Mod");
            harmony.PatchAll();
        }
    }
}
