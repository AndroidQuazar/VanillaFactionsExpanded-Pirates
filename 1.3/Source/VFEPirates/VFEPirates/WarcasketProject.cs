using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace VFEPirates
{
    public class WarcasketProject : IExposable
    {
        public ThingDef armorDef;
        public ThingDef shoulderPadsDef;
        public ThingDef helmetDef;

        public Color colorArmor;
        public Color colorShoulderPads;
        public Color colorHelmet;

        public float totalWorkAmount; // we could just retrieve it from all apparels, but might be too much perf impact if done every frame and tick
        public void ApplyOn(Pawn pawn)
        {
            var armor = ThingMaker.MakeThing(armorDef) as Apparel;
            armor.SetColor(colorArmor);
            pawn.apparel.Wear(armor, false, true);

            var helmet = ThingMaker.MakeThing(helmetDef) as Apparel;
            helmet.SetColor(colorHelmet);
            pawn.apparel.Wear(helmet, false, true);

            var shoulderPads = ThingMaker.MakeThing(shoulderPadsDef) as Apparel;
            shoulderPads.SetColor(colorShoulderPads);
            pawn.apparel.Wear(shoulderPads, false, true);
        }
        public void ExposeData()
        {
            Scribe_Defs.Look(ref armorDef, "armorDef");
            Scribe_Defs.Look(ref shoulderPadsDef, "shoulderPadsDef");
            Scribe_Defs.Look(ref helmetDef, "helmetDef");
            Scribe_Values.Look(ref colorArmor, "colorArmor");
            Scribe_Values.Look(ref colorShoulderPads, "colorShoulderPads");
            Scribe_Values.Look(ref colorHelmet, "colorHelmet");
            Scribe_Values.Look(ref totalWorkAmount, "totalWorkAmount");
        }
    }
}
