using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;
using RimWorld;
using HarmonyLib;

namespace VFEPirates
{
	public class CurseOfGreed : CurseWorker
	{
        public override void DoPatches()
        {
            Patch(typeof(Mineable), "TrySpawnYield", transpiler: AccessTools.Method(typeof(CurseOfGreed), nameof(Transpiler)));
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();

            for (int i = 0; i < instructionList.Count; i++)
			{
                CodeInstruction instruction = instructionList[i];

                if (instruction.LoadsField(AccessTools.Field(typeof(BuildingProperties), nameof(BuildingProperties.mineableThing))))
				{
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(typeof(CurseOfGreed), nameof(SwapForGold)));
                    instruction = instructionList[++i];
				}

                yield return instruction;
			}
        }

        public static ThingDef SwapForGold(ThingDef originalThingDef) => ThingDefOf.Gold;
    }
}
