using HarmonyLib;
using RimWorld;
using Verse;


namespace VFEPirates
{

    [HarmonyPatch(typeof(Pawn_InteractionsTracker))]
    [HarmonyPatch("TryInteractWith_Patch")]
    public static class VFEPirates_Pawn_InteractionsTracker_TryInteractWith_Patch
    {
        [HarmonyPostfix]
        static void AddInteractionThoughts(Pawn __pawn, bool __result)
        {
            if (__result && __pawn?.Ideo?.HasPrecept(VFEP_DefOf.VFEP_Camaraderie_Respected) == true )
            {
                __pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(VFEP_DefOf.VFEP_Camaraderie_Respected_Memory, null, __pawn?.Ideo?.GetPrecept(VFEP_DefOf.VFEP_Camaraderie_Respected));

            }
            if (__result && __pawn?.Ideo?.HasPrecept(VFEP_DefOf.VFEP_Camaraderie_Exalted) == true)
            {
                __pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(VFEP_DefOf.VFEP_Camaraderie_Exalted_Memory, null, __pawn?.Ideo?.GetPrecept(VFEP_DefOf.VFEP_Camaraderie_Exalted));

            }
        }
    }








}
