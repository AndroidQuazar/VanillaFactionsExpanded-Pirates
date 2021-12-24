using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFEPirates
{
    [StaticConstructorOnStartup]
    public static class StaticStartup
    {
        static StaticStartup()
        {
            FillWarcasketDefLists();
            InitializeCurses();
        }

        private static void InitializeCurses()
        {
            foreach (var def in DefDatabase<CurseDef>.AllDefs)
            {
				try
				{
                    def.Worker.DoPatches(VFEPiratesMod.harmony);
                }
                catch (Exception ex)
				{
                    Log.Error($"Exception thrown while initializing curses. Worker = \"{def.Worker}\" Exception = {ex.Message}");
				}
            }
        }
        private static void FillWarcasketDefLists()
        {
            var apparels = DefDatabase<ThingDef>.AllDefs.OfType<WarcasketDef>().ToList();
            VFEPiratesMod.allArmorDefs.AddRange(apparels.Where(x => x.isArmor));
            VFEPiratesMod.allShoulderPadsDefs.AddRange(apparels.Where(x => x.isShoulderPads));
            VFEPiratesMod.allHelmetDefs.AddRange(apparels.Where(x => x.isHelmet));
        }
    }
}
