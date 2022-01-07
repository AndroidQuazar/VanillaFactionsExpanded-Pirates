using System;
using System.Linq;
using System.Reflection.Emit;
using System.Collections.Generic;
using Verse;
using RimWorld;
using HarmonyLib;

namespace VFEPirates
{
    public class CurseOfGluttony : CurseWorker
    {
        public override void DoPatches()
        {
            Patch(original: AccessTools.PropertyGetter(typeof(Need_Food), nameof(Need_Food.FoodFallPerTick)), 
                postfix: AccessTools.Method(typeof(CurseOfGluttony), nameof(DoubleHunger)));
        }

        public static void DoubleHunger(ref float __result)
        {
            __result *= 2;
        }
    }
}
