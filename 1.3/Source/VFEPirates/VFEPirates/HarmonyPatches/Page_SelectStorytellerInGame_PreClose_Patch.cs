using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEPirates
{
    [HarmonyPatch(typeof(Page_SelectStorytellerInGame))]
    [HarmonyPatch("PreClose")]
    public static class Page_SelectStorytellerInGame_PreClose_Patch
    {
        [HarmonyPostfix]
        public static void OpenCurses(Page_SelectStorytellerInGame __instance)
        {
            if (Current.Game.storyteller.storytellerComps.OfType<StorytellerComp_Cursed>().Any()) Find.WindowStack.Add(new Page_ChooseCurses());
            else GameComponent_CurseManager.Instance.Notify_StorytellerChanged();
        }
    }
}