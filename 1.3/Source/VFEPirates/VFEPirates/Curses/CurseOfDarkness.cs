using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;
using Verse.Noise;

namespace VFEPirates
{
    public class CurseOfDarkness : CurseWorker
    {
        public static HashSet<Map> darkenedMaps = new HashSet<Map>();
        public override void DoPatches(Harmony harmony)
        {
            harmony.DoPatch(typeof(DarklightUtility), "IsDarklightAt", prefix: AccessTools.Method(typeof(CurseOfDarkness), nameof(IsDarklightAtPrefix)));
            harmony.DoPatch(typeof(GlowGrid), "RecalculateAllGlow", postfix: AccessTools.Method(typeof(CurseOfDarkness), nameof(RecalculateAllGlowPostfix)));
            harmony.DoPatch(typeof(Building), nameof(Building.SpawnSetup), postfix: AccessTools.Method(typeof(CurseOfDarkness), nameof(MarkGlowDirty)));
            harmony.DoPatch(typeof(Building), nameof(Building.DeSpawn), prefix: AccessTools.Method(typeof(CurseOfDarkness), nameof(MarkGlowDirty)));
            harmony.DoPatch(typeof(GenCelestial), nameof(GenCelestial.CurCelestialSunGlow), postfix: AccessTools.Method(typeof(CurseOfDarkness), nameof(RegisterCelestialGlow)));
        }

        public static bool IsDarklightAtPrefix(ref bool __result, IntVec3 position, Map map)
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

        public static void RecalculateAllGlowPostfix(GlowGrid __instance, Map ___map)
        {
            if (IsActive(typeof(CurseOfDarkness)))
            {
                int numGridCells = ___map.cellIndices.NumGridCells;
                for (int i = 0; i < numGridCells; i++)
                {
                    var position = ___map.cellIndices.IndexToCell(i);
                    var glow = ___map.glowGrid.GameGlowAt(position);
                    if (glow <= 0)
                    {
                        __instance.glowGrid[i] = Color.black;
                        __instance.glowGridNoCavePlants[i] = Color.black;
                    }
                }
            }
        }

        public static void MarkGlowDirty(Building __instance)
        {
            if (IsActive(typeof(CurseOfDarkness)))
                __instance.Map.glowGrid.MarkGlowGridDirty(__instance.Position);
        }

        public static void RegisterCelestialGlow(Map map, float __result)
        {
            if (IsActive(typeof(CurseOfDarkness)))
            {
                if (__result <= 0)
                {
                    if (!darkenedMaps.Contains(map))
                    {
                        foreach (var cell in map.AllCells)
                        {
                            map.glowGrid.MarkGlowGridDirty(cell);
                        }
                        darkenedMaps.Add(map);
                        Log.Message("Adding darkness");
                    }
                }
                else if (darkenedMaps.Contains(map))
                {
                    foreach (var cell in map.AllCells)
                    {
                        map.glowGrid.MarkGlowGridDirty(cell);
                    }
                    Log.Message("Should remove darkness");
                    darkenedMaps.Remove(map);
                }
            }
        }
    }
}
