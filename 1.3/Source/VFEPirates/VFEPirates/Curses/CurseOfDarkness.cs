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
        public override void DoPatches()
        {
            Patch(typeof(DarklightUtility), "IsDarklightAt", prefix: AccessTools.Method(typeof(CurseOfDarkness), nameof(IsDarklightAtPrefix)));
            Patch(typeof(GlowGrid), "RecalculateAllGlow", postfix: AccessTools.Method(typeof(CurseOfDarkness), nameof(RecalculateAllGlowPostfix)));
            Patch(typeof(Building), nameof(Building.SpawnSetup), postfix: AccessTools.Method(typeof(CurseOfDarkness), nameof(MarkGlowDirty)));
            Patch(typeof(Building), nameof(Building.DeSpawn), prefix: AccessTools.Method(typeof(CurseOfDarkness), nameof(MarkGlowDirty)));
            Patch(typeof(GenCelestial), nameof(GenCelestial.CurCelestialSunGlow), postfix: AccessTools.Method(typeof(CurseOfDarkness), nameof(RegisterCelestialGlow)));
            Patch(typeof(SkyManager), "CurrentSkyTarget", postfix: AccessTools.Method(typeof(CurseOfDarkness), nameof(CurrentSkyTargetPostfix)));
        }

        public static bool IsDarklightAtPrefix(ref bool __result, IntVec3 position, Map map)
        {
            if (map.glowGrid.GameGlowAt(position) <= 0)
            {
                __result = true;
                return false;
            }
            return true;
        }

        public static void RecalculateAllGlowPostfix(GlowGrid __instance, Map ___map)
        {
            int numGridCells = ___map.cellIndices.NumGridCells;
            for (int i = 0; i < numGridCells; i++)
            {
                var position = ___map.cellIndices.IndexToCell(i);
                var glow = ___map.glowGrid.GameGlowAt(position);
                if (glow <= 0.6f)
                {
                    var color = Color.black;
                    color.a = 1f * (1f - glow);
                    __instance.glowGrid[i] = color;
                    __instance.glowGridNoCavePlants[i] = color;
                }
            }
        }

        public static void MarkGlowDirty(Building __instance)
        {
            __instance.Map.glowGrid.MarkGlowGridDirty(__instance.Position);
        }
        public static void CurrentSkyTargetPostfix(ref SkyTarget __result)
        {
            Log.Message("__result.glow: " + __result.glow);
            __result.colors = new SkyColorSet(Color.Lerp(__result.colors.sky, Color.black, 1f - __result.glow), __result.colors.shadow, __result.colors.overlay, __result.colors.saturation);
        }
        public static void RegisterCelestialGlow(Map map, float __result)
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

        public override void OnActivate()
        {
            base.OnActivate();
            RefreshEvererything();
        }

        public override void OnDisactivate()
        {
            base.OnDisactivate();
            RefreshEvererything();
        }

        public void RefreshEvererything()
        {
            foreach (var map in Find.Maps)
            {
                foreach (var cell in map.AllCells)
                {
                    map.glowGrid.MarkGlowGridDirty(cell);
                }
            }
        }
    }
}
