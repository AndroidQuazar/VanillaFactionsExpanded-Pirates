using System;
using Verse;

namespace VFEPirates
{
    public class CurseDef : Def
    {
        public Type workerClass = typeof(CurseWorker);
        [Unsaved(false)]
        private CurseWorker workerInt;
        public CurseWorker Worker
        {
            get
            {
                if (workerInt == null)
                {
                    workerInt = (CurseWorker)Activator.CreateInstance(workerClass);
                }
                return workerInt;
            }
        }

        public string iconPath;
    }
}
