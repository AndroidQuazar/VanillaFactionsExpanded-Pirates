using HarmonyLib;
using RimWorld;

namespace VFEPirates
{
    public class CurseOfMind : CurseWorker
    {
        public override void DoPatches(Harmony harmony)
        {
			harmony.DoPatch(typeof(Thought), "MoodOffset", postfix: AccessTools.Method(typeof(CurseOfMind), nameof(Postfix)));
        }
		public static void Postfix(ref float __result)
		{
            if (IsActive(typeof(CurseOfMind)))
            {
                __result *= 2f;
            }
        }
    }
}
