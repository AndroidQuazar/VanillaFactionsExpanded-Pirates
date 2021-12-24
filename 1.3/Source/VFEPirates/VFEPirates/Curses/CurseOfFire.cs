using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using HarmonyLib;

namespace VFEPirates
{
    public class CurseOfFire : CurseWorker
    {
        public override void DoPatches(Harmony harmony)
        {
            harmony.Patch(original: AccessTools.PropertyGetter(typeof(FireWatcher), nameof(FireWatcher.LargeFireDangerPresent)), 
                postfix: new HarmonyMethod(AccessTools.Method(typeof(CurseOfFire), nameof(Postfix))));
        }

        public static void Postfix(ref bool __result)
        {
            if (IsActive(typeof(CurseOfFire)))
            {
                __result = false;
            }
        }
    }
}
