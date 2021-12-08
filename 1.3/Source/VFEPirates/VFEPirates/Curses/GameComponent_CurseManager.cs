using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace VFEPirates
{
    public class GameComponent_CurseManager : GameComponent
    {
        public static GameComponent_CurseManager Instance;

        private Dictionary<StorytellerDef, List<CurseDef>> activeCurses = new Dictionary<StorytellerDef, List<CurseDef>>();

        public HashSet<Type> activeCurseTypes = new HashSet<Type>();
        public HashSet<CurseDef> activeCurseDefs = new HashSet<CurseDef>();
        public GameComponent_CurseManager(Game game) : base()
        {
            Init();
        }
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
            if (Find.Storyteller?.def != null)
            {
                activeCurses[Find.Storyteller.def] = DefDatabase<CurseDef>.AllDefsListForReading; // for testing only, need to remove after
            }

            UpdateLists();
        }
        public void Add(CurseDef curseDef)
        {
            if (!activeCurses.ContainsKey(Find.Storyteller.def))
            {
                activeCurses[Find.Storyteller.def] = new List<CurseDef> { curseDef };
            }
            else
            {
                activeCurses[Find.Storyteller.def].Add(curseDef);
            }
            UpdateLists();
        }
        public void Remove(CurseDef curseDef)
        {
            if (activeCurses.ContainsKey(Find.Storyteller.def))
            {
                activeCurses[Find.Storyteller.def].Remove(curseDef);
            }
            UpdateLists();
        }

        private void UpdateLists()
        {
            activeCurseTypes = activeCurses[Find.Storyteller.def].Select(x => x.workerClass).ToHashSet();
            activeCurseDefs = activeCurses[Find.Storyteller.def].ToHashSet();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref activeCurses, "activeCurses");
            Init();
        }
    }
}
