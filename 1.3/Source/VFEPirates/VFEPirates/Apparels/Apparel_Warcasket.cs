using RimWorld;
using UnityEngine;
using Verse;

namespace VFEPirates
{
    public class Apparel_Warcasket : Apparel
    {
        public Color color;
        public override Color DrawColor => color;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref color, "color");
        }
    }
}
