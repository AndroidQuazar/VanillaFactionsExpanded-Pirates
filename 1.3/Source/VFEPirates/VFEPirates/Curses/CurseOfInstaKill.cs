using System;
using System.Linq;
using System.Reflection.Emit;
using System.Collections.Generic;
using Verse;
using RimWorld;
using HarmonyLib;

namespace VFEPirates
{
    public class CurseOfInstaKill : CurseWorker
    {
        public override void DoPatches()
        {
            Patch(original: AccessTools.Method(typeof(Pawn_HealthTracker), "MakeDowned"), 
                prefix: AccessTools.Method(typeof(CurseOfInstaKill), nameof(InstaKill)));
        }

        public static bool InstaKill(DamageInfo? dinfo, Hediff hediff, Pawn ___pawn)
        {
            if (IsActive(typeof(CurseOfInstaKill)))
			{
                ___pawn.Kill(dinfo, hediff);
                return false;
            }
            return true;
        }
    }
}
