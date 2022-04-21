using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace VFEPirates.HarmonyPatches
{
    [HarmonyPatch(typeof(CompReloadable), "CreateVerbTargetCommand")]
    public static class CompReloadable_CreateVerbTargetCommand_Patch
    {
        public static void Postfix(ref Command_Reloadable __result, Thing gear, Verb verb)
        {
            if (verb is Verb_DroneDeployment || verb is Verb_Spidermine)
            {
                __result.defaultIconColor = Color.white;
                __result.overrideColor = Color.white;
            }
        }
    }

    [HarmonyPatch(typeof(Command_VerbTarget), "IconDrawColor", MethodType.Getter)]
    public static class Command_VerbTarget_IconDrawColor_Patch
    {
        public static void Postfix(ref Color __result, Command_VerbTarget __instance)
        {
            if (__instance.verb is Verb_DroneDeployment || __instance.verb is Verb_Spidermine)
            {
                __result = Color.white;
            }
        }
    }
}
