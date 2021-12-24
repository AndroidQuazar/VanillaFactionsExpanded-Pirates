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
        public override void DoPatches(Harmony harmony)
        {
            harmony.Patch(original: AccessTools.PropertyGetter(typeof(Need_Food), nameof(Need_Food.FoodFallPerTick)), 
                postfix: new HarmonyMethod(AccessTools.Method(typeof(CurseOfGluttony), nameof(DoubleHunger))));
        }

        public static void DoubleHunger(ref float __result)
        {
            if (IsActive(typeof(CurseOfGluttony)))
			{
                __result *= 2;
            }
        }
    }
}
