﻿using System;
using System.Linq;
using System.Reflection.Emit;
using System.Collections.Generic;
using Verse;
using RimWorld;
using HarmonyLib;

namespace VFEPirates
{
    public class CurseOfAging : CurseWorker
    {
        public override void DoPatches()
        {
            Patch(original: AccessTools.Method(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.AgeTick)), 
                transpiler: AccessTools.Method(typeof(CurseOfAging), nameof(AgeTick)));
        }

        public static long AgeMultiplier(long interval) => 10 * interval;

        public static IEnumerable<CodeInstruction> AgeTick(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();

            for (int i = 0; i < instructionList.Count; i++)
			{
                CodeInstruction instruction = instructionList[i];

                if (instruction.opcode == OpCodes.Conv_I8)
				{
                    yield return instruction;
                    instruction = instructionList[++i];

                    yield return new CodeInstruction(opcode: OpCodes.Call, AccessTools.Method(typeof(CurseOfAging), nameof(AgeMultiplier)));
				}

                yield return instruction;
			}
        }
    }
}
