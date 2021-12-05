using HarmonyLib;
using RimWorld;
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

        public static List<WarcasketDef> allArmorDefs = new List<WarcasketDef>();
        public static List<WarcasketDef> allShoulderPadsDefs = new List<WarcasketDef>();
        public static List<WarcasketDef> allHelmetDefs = new List<WarcasketDef>();
        public VFEPiratesMod(ModContentPack modContentPack) : base(modContentPack)
        {
            harmony = new Harmony("VFEPirates.Mod");
            harmony.PatchAll();
        }
    }
}
