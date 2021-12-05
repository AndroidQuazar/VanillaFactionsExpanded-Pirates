﻿using HarmonyLib;
using RimWorld;
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
            var apparels = DefDatabase<ThingDef>.AllDefs.OfType<WarcasketDef>().ToList();
            VFEPiratesMod.allArmorDefs.AddRange(apparels.Where(x => x.isArmor));
            VFEPiratesMod.allShoulderPadsDefs.AddRange(apparels.Where(x => x.isShoulderPads));
            VFEPiratesMod.allHelmetDefs.AddRange(apparels.Where(x => x.isHelmet));

            var allApparelPairs = Traverse.Create(typeof(PawnApparelGenerator)).Field<List<ThingStuffPair>>("allApparelPairs").Value;
            allApparelPairs.RemoveAll(pair => pair.thing is WarcasketDef);

        }
    }
}
