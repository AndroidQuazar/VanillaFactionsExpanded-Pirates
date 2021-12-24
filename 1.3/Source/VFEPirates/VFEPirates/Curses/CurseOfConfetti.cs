using System;
using System.Collections.Generic;
using Verse;
using HarmonyLib;

namespace VFEPirates
{
    public class CurseOfConfetti : CurseWorker
    {
        public override void DoPatches(Harmony harmony)
        {
            harmony.Patch(original: AccessTools.Method(typeof(Pawn), nameof(Pawn.Kill)), postfix: new HarmonyMethod(AccessTools.Method(typeof(CurseOfConfetti), nameof(Postfix))));
        }

        public static void Postfix(DamageInfo? dinfo)
        {
            if (IsActive(typeof(CurseOfConfetti)))
            {
                //Spawn Mote
                //Emit Sound
            }
        }
    }
}
