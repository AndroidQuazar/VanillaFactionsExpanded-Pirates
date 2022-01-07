using HarmonyLib;
using RimWorld;
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
    public class PowerJumpExtension : DefModExtension
    {
        public float fuelConsumption;
        public int detonationDamageAmount;
        public SoundDef powerJumpSoundActivated;
    }
    public class Ability_PowerJump : Ability
    {
        public float Range => this.pawn.GetStatValue(VFEP_DefOf.VFEP_PowerJumpRange);
        public float FuelConsumption => this.def.GetModExtension<PowerJumpExtension>().fuelConsumption;
        public float DetonationRadius => this.pawn.GetStatValue(VFEP_DefOf.VFEP_PowerJumpDetonationRadius);
        public int DetonationDamageAmount => this.def.GetModExtension<PowerJumpExtension>().detonationDamageAmount;
        public SoundDef PowerJumpSoundActivated => this.def.GetModExtension<PowerJumpExtension>().powerJumpSoundActivated;
        public override bool CanHitTarget(LocalTargetInfo target)
        {
            return target.Cell.WalkableBy(pawn.Map, pawn) && target.Cell.DistanceTo(pawn.Position) <= Range;
        }
        public override float GetRangeForPawn()
        {
            return Range;
        }
        public override Gizmo GetGizmo()
        {
            Command_Ability action = new Command_Ability(this.pawn, this);
            if (this.holder.TryGetComp<CompReloadable>().RemainingCharges < FuelConsumption)
            {
                action.Disable("VFEP.NotEnoughFuel".Translate());
            }
            return action;
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

    public class PawnFlyer_PowerJump : AbilityPawnFlyer
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                float a = Mathf.Max(Traverse.Create(this).Field<float>("flightDistance").Value, 1f) / this.FlyingPawn.GetStatValue(VFEP_DefOf.VFEP_FlightSpeed);
                a = Mathf.Max(a, def.pawnFlyer.flightDurationMin);
                ticksFlightTime = a.SecondsToTicks();
                ticksFlying = 0;
            }
        }
        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            var vector = this.position;
            vector.y += 10;
            this.FlyingPawn.Drawer.renderer.RenderPawnAt(vector, this.direction);
        }

        private Effecter flightEffecter;
        public override Vector3 DrawPos
        {
            get
            {
                return this.position;
            }
        }
        public override void Tick()
        {
            if (flightEffecter == null && def.pawnFlyer.flightEffecterDef != null)
            {
                flightEffecter = def.pawnFlyer.flightEffecterDef.Spawn();
                flightEffecter.Trigger(this, TargetInfo.Invalid);
            }
            else
            {
                flightEffecter?.EffectTick(this, TargetInfo.Invalid);
            }
            base.Tick();
        }
        protected override void RespawnPawn()
        {
            Pawn pawn = this.FlyingPawn;
            base.RespawnPawn();
            var powerJumpAbility = this.ability as Ability_PowerJump;
            GenExplosion.DoExplosion(Position, Map, powerJumpAbility.DetonationRadius, DamageDefOf.Bomb, pawn, powerJumpAbility.DetonationDamageAmount, ignoredThings: new List<Thing> { pawn });
        }
    }
}