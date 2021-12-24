﻿using System;
using System.Linq;
using System.Reflection.Emit;
using System.Collections.Generic;
using Verse;
using RimWorld;
using HarmonyLib;

namespace VFEPirates
{
    public class CurseOfBlind : CurseWorker
    {
        public override void DoPatches(Harmony harmony)
        {
            harmony.Patch(original: AccessTools.Method(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), parameters: new Type[] { typeof(Letter), typeof(string) }), 
                prefix: new HarmonyMethod(AccessTools.Method(typeof(CurseOfBlind), nameof(MuteLetter))));
        }

        public static bool MuteLetter(Letter let, string debugInfo)
        {
            if (IsActive(typeof(CurseOfBlind)))
			{
                Find.Archive.Add(let);
                let.Received();
                return false;
            }
            return true;
        }
    }
}
