﻿using RimWorld;
using UnityEngine;
using Verse;

namespace VFEPirates
{
    public class Apparel_WarcasketArmor : Apparel
    {
        public Apparel bodySuit;
        public Apparel BodySuit
        {
            get
            {
                if (bodySuit is null)
                {
                    bodySuit = ThingMaker.MakeThing(VFEP_DefOf.VFEP_Warcasket_Bodysuit) as Apparel;
                }
                return bodySuit;
            }
        }
        public override void PostMake()
        {
            base.PostMake();
        }

        public static float yOffset = 0.0125f;
        public override void DrawWornExtras()
        {
            base.DrawWornExtras();
            var bodyMesh = MeshPool.humanlikeBodySet.MeshAt(Wearer.Rotation);
            var pos = Wearer.DrawPos;
            pos.y += yOffset;
            var baseMat = BodySuit.Graphic.MatAt(Wearer.Rotation);
            baseMat = Wearer.Drawer.renderer.graphics.flasher.GetDamagedMat(baseMat);
            var quat = Quaternion.AngleAxis(Wearer.Drawer.renderer.BodyAngle(), Vector3.up);
            Graphics.DrawMesh(bodyMesh, pos, quat, baseMat, 0);
        }
    }
}
