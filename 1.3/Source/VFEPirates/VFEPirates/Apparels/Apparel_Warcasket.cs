using RimWorld;
using UnityEngine;
using Verse;

namespace VFEPirates
{
    public class Apparel_Warcasket : Apparel
    {
        public Color color;

        public override void PostMake()
        {
            base.PostMake();
            color = this.def.colorGenerator.NewRandomizedColor();
        }
        public override Color DrawColor => color;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref color, "color");
        }
    }
}
