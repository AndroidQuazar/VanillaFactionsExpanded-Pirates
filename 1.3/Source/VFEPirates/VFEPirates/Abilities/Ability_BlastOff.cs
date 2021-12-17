using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using Verse;
using Verse.Sound;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;
using Command_Ability = VFECore.Abilities.Command_Ability;

namespace VFEPirates
{
    public class BlastOffExtension : DefModExtension
    {
        public float powerJumpDetonationRadius;
        public float powerJumpRange;
        public float fuelConsumption;
        public int detonationDamageAmount;
        public SoundDef powerJumpSoundActivated;
    }

    public class Ability_BlastOff : Ability
    {
        public float Range => this.def.GetModExtension<BlastOffExtension>().powerJumpRange;
        public float FuelConsumption => this.def.GetModExtension<BlastOffExtension>().fuelConsumption;
        public float DetonationRadius => this.def.GetModExtension<BlastOffExtension>().powerJumpDetonationRadius;
        public int DetonationDamageAmount => this.def.GetModExtension<BlastOffExtension>().detonationDamageAmount;
        public SoundDef PowerJumpSoundActivated => this.def.GetModExtension<BlastOffExtension>().powerJumpSoundActivated;
        public override Gizmo GetGizmo()
        {
            Command_Ability action = new Command_Ability(this.pawn, this);
            if (this.holder.TryGetComp<CompReloadable>().RemainingCharges < FuelConsumption)
            {
                action.Disable("VFEP.NotEnoughFuel".Translate());
            }
            return action;
        }
        public bool ValidateGlobalTarget(GlobalTargetInfo target)
        {
            return target.IsMapTarget;
        }
        public override void DoAction()
        {
            base.DoAction();
            CameraJumper.TryJump(CameraJumper.GetWorldTarget(this.pawn));
            Find.WorldTargeter.BeginTargeting(delegate (GlobalTargetInfo t)
            {
                if (this.ValidateGlobalTarget(t))
                {
                    Log.Message("Should be clicked on " + t);
                    return true;
                }
                return false;
            }, canTargetTiles: true, this.def.icon, !this.pawn.IsCaravanMember(), null, canSelectTarget: this.ValidateGlobalTarget);
        }
        public override void Cast(LocalTargetInfo target)
        {
            base.Cast(target);
            var map = this.pawn.Map;
            var position = this.pawn.Position;
            var comp = this.holder.TryGetComp<CompReloadable>();
            for (var i = 0; i < FuelConsumption; i++)
            {
                comp.UsedOnce();
            }
            var sound = PowerJumpSoundActivated;
            if (sound != null)
            {
                sound.PlayOneShot(new TargetInfo(position, map));
            }
            AbilityPawnFlyer flyer = (PawnFlyer_PowerJump)PawnFlyer.MakeFlyer(VFEP_DefOf.VFEP_PowerJumpPawn, this.pawn, target.Cell);
            flyer.ability = this;
            flyer.target = target.CenterVector3;
            GenSpawn.Spawn(flyer, target.Cell, map);
            GenExplosion.DoExplosion(position, map, DetonationRadius, DamageDefOf.Bomb, pawn, DetonationDamageAmount, ignoredThings: new List<Thing> { pawn });
        }
    }
}