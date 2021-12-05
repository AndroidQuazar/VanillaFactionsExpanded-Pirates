using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace VFEPirates
{
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
