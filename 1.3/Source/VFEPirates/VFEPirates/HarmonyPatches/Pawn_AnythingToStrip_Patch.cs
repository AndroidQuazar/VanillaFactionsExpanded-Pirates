using HarmonyLib;
using Verse;

namespace VFEPirates
{
    [HarmonyPatch(typeof(Pawn), "AnythingToStrip")]
    public static class Pawn_AnythingToStrip_Patch
    {
        public static bool Prefix(Pawn __instance)
        {
            if (__instance.IsWearingWarcasket())
            {
                return false;
            }
            return true;
        }
    }
}
