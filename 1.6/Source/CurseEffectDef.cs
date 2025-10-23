using System;
using Verse;

namespace DanceOfEvolution
{
    public class CurseEffectDef : Def
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
                    workerInt.def = this;
                }
                return workerInt;
            }
        }
    }
}