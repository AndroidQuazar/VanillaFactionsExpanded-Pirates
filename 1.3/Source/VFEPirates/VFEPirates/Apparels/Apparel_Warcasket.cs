using RimWorld;
using UnityEngine;
using Verse;

namespace VFEPirates
{
    public class Apparel_Warcasket : Apparel
    {
        public Color? colorApparel;
        public override Color DrawColor => colorApparel ??= this.def.colorGenerator.NewRandomizedColor();
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref colorApparel, "colorApparel");
        }
    }
}
