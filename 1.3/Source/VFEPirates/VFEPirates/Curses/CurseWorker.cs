using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static Verse.AsymmetricLinkData;

namespace VFEPirates
{
    public class CurseWorker
    {
        public CurseWorker()
        {

        }
        public static bool IsActive(Type type) => GameComponent_CurseManager.Instance.activeCurseTypes.Contains(type);
        public static bool IsActive(CurseDef def) => GameComponent_CurseManager.Instance.activeCurseDefs.Contains(def);
        public virtual void DoPatches(Harmony harmony)
        {

        }
    }
}
