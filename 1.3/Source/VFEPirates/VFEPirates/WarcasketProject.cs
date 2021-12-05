using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace VFEPirates
{
    public class WarcasketProject : IExposable
    {
        public ThingDef bodySuit;
        public ThingDef armor;
        public Color colorArmor;
        public ThingDef shoulderPads;
        public Color colorShoulderPads;
        public ThingDef helmet;
        public Color colorHelmet;
        public void ApplyOn(Pawn pawn)
        {

        }
        public void ExposeData()
        {
            throw new System.NotImplementedException();
        }
    }
}
