using HarmonyLib;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;
using Verse.Noise;

namespace VFEPirates
{
    public class CurseOfDarkness : CurseWorker
    {
        public override void DoPatches(Harmony harmony)
        {
            harmony.DoPatch(typeof(DarklightUtility), "IsDarklightAt", prefix: AccessTools.Method(typeof(CurseOfDarkness), nameof(Prefix)));
            harmony.DoPatch(typeof(GlowGrid), "RecalculateAllGlow", postfix: AccessTools.Method(typeof(CurseOfDarkness), nameof(Postfix)));
        }

        public static bool Prefix(ref bool __result, IntVec3 position, Map map)
        {
            if (IsActive(typeof(CurseOfDarkness)))
            {
                if (map.glowGrid.GameGlowAt(position) <= 0)
                {
                    __result = true;
                    return false;
                }
            }
            return true;
        }

        public static void Postfix(GlowGrid __instance, Map ___map)
        {
            if (IsActive(typeof(CurseOfDarkness)))
            {
                if (Current.ProgramState != ProgramState.Playing)
                {
                    return;
                }
                int numGridCells = ___map.cellIndices.NumGridCells;
                for (int i = 0; i < numGridCells; i++)
                {
                    var glow = ___map.glowGrid.GameGlowAt(___map.cellIndices.IndexToCell(i));
                    if (glow <= 0)
                    {
                        __instance.glowGrid[i] = Color.black;
                        __instance.glowGridNoCavePlants[i] = Color.black;
                    }
                }
            }
        }
    }
}
