using System;
using System.Collections.Generic;
using Verse;
using HarmonyLib;

namespace VFEPirates
{
    public class CurseOfSteel : CurseWorker
    {
        public override void DoPatches(Harmony harmony)
        {
            harmony.Patch(original: AccessTools.PropertyGetter(typeof(DamageInfo), nameof(DamageInfo.Amount)), postfix: new HarmonyMethod(AccessTools.Method(typeof(CurseOfSteel), nameof(Postfix))));
        }

        public static void Postfix(ref float __result)
        {
            if (IsActive(typeof(CurseOfSteel)))
            {
                __result *= 2;
            }
        }
    }
}
