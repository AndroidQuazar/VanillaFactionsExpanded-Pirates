using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VFEPirates
{
    public class GameComponent_CurseManager : GameComponent
    {
        public static GameComponent_CurseManager Instance;
        public HashSet<CurseDef> activeCurseDefs = new();

        private Dictionary<StorytellerDef, List<CurseDef>> activeCurses = new();

        public HashSet<Type> activeCurseTypes = new();

        public GameComponent_CurseManager(Game game)
        {
            Init();
        }

        public int DesiredPopulationMax => activeCurseDefs.Count;
        public int MaxThreatBigIntervalDays => activeCurseDefs.Count;

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            Init();
        }

        public void Init()
        {
            Instance = this;
            activeCurses ??= new Dictionary<StorytellerDef, List<CurseDef>>();
            activeCurseTypes ??= new HashSet<Type>();
            activeCurseDefs ??= new HashSet<CurseDef>();

            UpdateLists();
        }

        public void Add(CurseDef curseDef)
        {
            if (!activeCurses.ContainsKey(Find.Storyteller.def))
                activeCurses[Find.Storyteller.def] = new List<CurseDef> {curseDef};
            else
                activeCurses[Find.Storyteller.def].Add(curseDef);
            UpdateLists();
        }

        public void Remove(CurseDef curseDef)
        {
            if (activeCurses.ContainsKey(Find.Storyteller.def)) activeCurses[Find.Storyteller.def].Remove(curseDef);
            UpdateLists();
        }

        public void Notify_StorytellerChanged()
        {
            if (!activeCurses.ContainsKey(Find.Storyteller.def))
                activeCurses[Find.Storyteller.def] = new List<CurseDef>();
            UpdateLists();
        }

        private void UpdateLists()
        {
            if (Find.Storyteller?.def != null)
            {
                activeCurseTypes = activeCurses[Find.Storyteller.def].Select(x => x.workerClass).ToHashSet();
                activeCurseDefs = activeCurses[Find.Storyteller.def].ToHashSet();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref activeCurses, "activeCurses", LookMode.Def, LookMode.Deep);
            Init();
        }
    }
}