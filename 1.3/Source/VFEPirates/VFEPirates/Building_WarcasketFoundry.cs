using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements.Experimental;
using Verse;
using Verse.AI;

namespace VFEPirates
{
    public class WarcasketDef : ThingDef
    {

    }

    [StaticConstructorOnStartup]
    public static class StaticStartup
    {
        static StaticStartup()
        {
            Log.Message("1: " + Traverse.Create(typeof(PawnApparelGenerator)).Field<List<ThingStuffPair>>("allApparelPairs").Value.Count);
            var apparels = DefDatabase<ThingDef>.AllDefs.Where(x => x.apparel != null).ToList();
            var allApparelPairs = Traverse.Create(typeof(PawnApparelGenerator)).Field<List<ThingStuffPair>>("allApparelPairs").Value;
            allApparelPairs.RemoveAll(pair => pair.thing is WarcasketDef);
            Log.Message("2: " + Traverse.Create(typeof(PawnApparelGenerator)).Field<List<ThingStuffPair>>("allApparelPairs").Value.Count);
        }
    }
    public class ApparelExtension : DefModExtension
    {
        public bool nonSpawnable;
        public bool hiddenFromDatabases;
        public bool isWarCasketApparel;
    }
    public class Building_WarcasketFoundry : Building
    {
        public Pawn occupant;

        public CompPowerTrader compPower;
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            compPower = this.TryGetComp<CompPowerTrader>();
        }
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (var g in base.GetFloatMenuOptions(selPawn)) yield return g;
            if (this.Faction == Faction.OfPlayer)
            {
                if (occupant is null)
                {
                    var reason = CannotUseNowReason(selPawn);
                    if (reason != null)
                    {
                        yield return new FloatMenuOption("VFEPirates.CannotEntomb".Translate(reason), null);
                    }
                    else
                    {
                        JobDef jobDef = VFEP_DefOf.VFEP_EntompIn;
                        string label = "VFEPirates.EntombInWarcasket".Translate();
                        Action action = delegate
                        {
                            Job job = JobMaker.MakeJob(jobDef, this);
                            selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                            Messages.Message("VFEPirates.EntombingWarning".Translate(), this, MessageTypeDefOf.CautionInput);
                        };
                        yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action), selPawn, this);
                    }
                }
            }
        }

        public string CannotUseNowReason(Pawn selPawn)
        {
            if (!compPower.PowerOn)
            {
                return "NoPower".Translate();
            }
            return null;
        }

        public void RegisterOccupant(Pawn pawn)
        {
            occupant = pawn;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref occupant, "occupant");
        }
    }
}
