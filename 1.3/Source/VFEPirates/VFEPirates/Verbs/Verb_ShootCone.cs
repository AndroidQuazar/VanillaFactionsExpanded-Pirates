using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace VFEPirates
{
    public class VerbProps_ShootCone : VerbProperties
    {
        public int coneAngle;
    }

    public class Verb_ShootCone : Verb_Shoot
    {
        public VerbProps_ShootCone VerbProps => this.verbProps as VerbProps_ShootCone;

        public override void DrawHighlight(LocalTargetInfo target)
        {
            if(VerbProps.range <= GenRadial.MaxRadialPatternRadius)
            {
                DrawConeRounded(VerbProps.coneAngle / 2f);
            }
            else
            {
                DrawLines();
            }

            if (target.IsValid)
            {
                GenDraw.DrawTargetHighlight(target);
                DrawHighlightFieldRadiusAroundTarget(target);
            }
        }
        

        private void DrawLines()
        {
            var startPos = this.Caster.Position.ToVector3Shifted();
            var quatLeft = Quaternion.Euler(0f, -VerbProps.coneAngle / 2f, 0f);
            var quatRight = Quaternion.Euler(0f, VerbProps.coneAngle / 2f, 0f);
            var targetLeft = startPos + (this.Caster.Rotation.AsQuat * quatLeft * new Vector3(0f, 0f, verbProps.range));
            var targetRight = startPos + (this.Caster.Rotation.AsQuat * quatRight * new Vector3(0f, 0f, verbProps.range));
            float layer = 100f;
            GenDraw.DrawLineBetween(startPos, targetLeft);
            GenDraw.DrawLineBetween(startPos, targetRight);

        }
        private void DrawConeRounded(float angle)
        {
            IntVec3 pos = this.Caster.Position;
            Rot4 rotation = this.caster.Rotation;

            Func<IntVec3, bool> predicate = ((IntVec3 c) => InCone(c, pos, rotation, angle));
            GenDraw.DrawRadiusRing(pos, this.verbProps.range, Color.white, predicate);
        }
        public override bool CanHitTarget(LocalTargetInfo targ)
        {
            return base.CanHitTarget(targ) && InCone(targ.Cell, this.caster.Position, this.caster.Rotation, VerbProps.coneAngle);
        }


        private bool InCone(IntVec3 evaluatedCell, IntVec3 from, Rot4 rotation, float degrees)
        {
            Vector3 dif = evaluatedCell.ToVector3() - from.ToVector3();
            Vector3 lookRotation = Quaternion.LookRotation(dif, Vector3.up).eulerAngles;
            if (GenGeo.AngleDifferenceBetween(lookRotation.y, rotation.AsAngle) <= degrees)
            {
                return true;
            }
            return false;
        }

    }
}
