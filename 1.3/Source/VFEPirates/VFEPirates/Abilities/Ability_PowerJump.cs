using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;
using Command_Ability = VFECore.Abilities.Command_Ability;

namespace VFEPirates
{
    public class Ability_PowerJump : Ability
    {
        public override bool CanHitTarget(LocalTargetInfo target)
        {
            return target.Cell.WalkableBy(pawn.Map, pawn) && target.Cell.DistanceTo(pawn.Position) <= 15f;
        }
        public override Command_Action GetGizmo()
        {
            Command_Ability action = new Command_Ability(this.pawn, this);
            if (this.holder.TryGetComp<CompReloadable>().RemainingCharges < 20)
            {
                action.Disable("VFEP.NotEnoughFuel".Translate());
            }
            return action;
        }
        public override void Cast(LocalTargetInfo target)
        {
            base.Cast(target);
            var map = this.pawn.Map;
            var comp = this.holder.TryGetComp<CompReloadable>();
            for (var i = 0; i < 20; i++)
            {
                comp.UsedOnce();
            }

            AbilityPawnFlyer flyer = (PawnFlyer_PowerJump)PawnFlyer.MakeFlyer(VFEP_DefOf.VFEP_PowerJumpPawn, this.pawn, target.Cell);
            flyer.ability = this;
            flyer.target = target.CenterVector3;
            GenSpawn.Spawn(flyer, target.Cell, map);
            GenExplosion.DoExplosion(this.pawn.Position, map, 3f, DamageDefOf.Bomb, pawn, 15, ignoredThings: new List<Thing> { pawn });
        }
    }

    public class PawnFlyer_PowerJump : AbilityPawnFlyer
    {
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
            GenExplosion.DoExplosion(Position, Map, 3f, DamageDefOf.Bomb, pawn, 15, ignoredThings: new List<Thing> { pawn });
        }
    }
}