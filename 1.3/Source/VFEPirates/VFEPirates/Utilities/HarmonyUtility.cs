using HarmonyLib;
using System;
using System.Reflection;

namespace VFEPirates
{
    public static class HarmonyUtility
    {
		public static void DoPatch(this Harmony harmony, Type type, string methodName, MethodInfo prefix = null, MethodInfo postfix = null, MethodInfo transpiler = null)
        {
			harmony.Patch(AccessTools.Method(type, methodName), prefix != null ? new HarmonyMethod(prefix) : null,
				postfix != null ? new HarmonyMethod(postfix) : null, transpiler != null ? new HarmonyMethod(transpiler) : null);
		}
	}
}
