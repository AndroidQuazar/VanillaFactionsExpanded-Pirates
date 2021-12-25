using System;
using System.Linq;
using System.Reflection.Emit;
using System.Collections.Generic;
using Verse;
using RimWorld;
using HarmonyLib;

namespace VFEPirates
{
    public class CurseOfTheEyePatch : CurseWorker
    {
        public override void DoPatches(Harmony harmony)
        {
            harmony.Patch(original: AccessTools.Method(typeof(ShotReport), nameof(ShotReport.HitFactorFromShooter), parameters: new Type[] { typeof(float), typeof(float) }), 
                prefix: new HarmonyMethod(AccessTools.Method(typeof(CurseOfTheEyePatch), nameof(HalfAccuracy))));
        }

        public static void HalfAccuracy(ref float accRating, float distance)
        {
            if (IsActive(typeof(CurseOfTheEyePatch)))
			{
                accRating /= 2;
            }
        }
    }
}
