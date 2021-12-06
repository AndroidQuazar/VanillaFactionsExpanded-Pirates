using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements.Experimental;
using Verse;
using Verse.AI;

namespace VFEPirates
{
    [StaticConstructorOnStartup]
    public class Building_WarcasketFoundry : Building
    {
        public Pawn occupant;

        public CompPowerTrader compPower;

        public float weldingProgress;

        public WarcasketProject curWarcasketProject;
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
                        JobDef jobDef = VFEP_DefOf.VFEP_EntombIn;
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

        public IEnumerable<IngredientCount> RequiredIngredients()
        {
            return curWarcasketProject.armorDef.costList.Select(x => x.ToIngredientCount())
             .Concat(curWarcasketProject.helmetDef.costList.Select(x => x.ToIngredientCount()))
             .Concat(curWarcasketProject.shoulderPadsDef.costList.Select(x => x.ToIngredientCount()));
        }
        public bool ReadyForWelding => compPower.PowerOn && occupant != null && curWarcasketProject != null;
        public void OpenCustomizationWindow(Pawn entombedPawn)
        {
            // Legodude, here you need to create and call customization window where you initialize chosenWarCasketProject with all its variables, such as crafter, total cost, workamount etc
            // All armor, helmet and shoulder pad defs can be found in VFEPiratesMod class

            // below is just a test code which you can use for now
            var armor = VFEPiratesMod.allArmorDefs.RandomElement();
            var shoulderPads = VFEPiratesMod.allShoulderPadsDefs.RandomElement();
            var helmet = VFEPiratesMod.allHelmetDefs.RandomElement();
            curWarcasketProject = new WarcasketProject
            {
                armorDef = armor,
                shoulderPadsDef = shoulderPads,
                helmetDef = helmet,
                colorArmor = Color.red,
                colorHelmet = Color.green,
                colorShoulderPads = Color.blue,
                totalWorkAmount = armor.GetStatValueAbstract(StatDefOf.WorkToMake) + shoulderPads.GetStatValueAbstract(StatDefOf.WorkToMake) + helmet.GetStatValueAbstract(StatDefOf.WorkToMake)
            };
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos()) yield return g;
            if (Prefs.DevMode)
            {
                if (occupant != null && curWarcasketProject != null)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "DEV: Finish welding",
                        action = delegate
                        {
                            curWarcasketProject.ApplyOn(occupant);
                            DeregisterOccupant();
                        }
                    };
                }
            }
        }
        public void RegisterOccupant(Pawn pawn)
        {
            occupant = pawn;
        }

        public void DeregisterOccupant()
        {
            occupant.jobs.StopAll();
            occupant = null;
        }

        public static readonly Material Frame = MaterialPool.MatFrom("Things/Building/WarcasketFoundry/WarcasketFrame");

        [TweakValue("00", 0, 10)] public static float yOffset = 4.5f;
        [TweakValue("00", 0, 10)] public static float zOffset = 0.7f;

        [TweakValue("00", 0, 10)] public static float ySize = 2f;
        [TweakValue("00", 0, 10)] public static float zSize = 2f;
        public override void Draw()
        {
            base.Draw();
            if (occupant != null)
            {
                var pos = this.TrueCenter();
                pos.y += yOffset;
                pos.z += zOffset;
                var quat = Quaternion.identity;
                var matrix = default(Matrix4x4);
                matrix.SetTRS(pos, quat, new Vector3(ySize, 1, zSize));
                Graphics.DrawMesh(MeshPool.plane10, matrix, Frame, 0);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref occupant, "occupant");
            Scribe_Deep.Look(ref curWarcasketProject, "curWarcasketProject");
        }
    }
}
