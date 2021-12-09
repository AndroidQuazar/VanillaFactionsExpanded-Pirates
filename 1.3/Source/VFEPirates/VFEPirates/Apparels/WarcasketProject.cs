﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VFEPirates
{
    public class WarcasketProject : IExposable
    {
        public WarcasketDef armorDef;

        public Color colorArmor;
        public Color colorHelmet;
        public Color colorShoulderPads;
        public float currentWorkAmountDone;
        public WarcasketDef helmetDef;
        public WarcasketDef shoulderPadsDef;

        public float totalWorkAmount; // we could just retrieve it from all apparels, but might be too much perf impact if done every frame and tick

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

        public IEnumerable<IngredientCount> RequiredIngredients()
        {
            var thingCounts = new Dictionary<ThingDef, int>();
            foreach (var thingCount in armorDef.costList
                .Concat(helmetDef.costList)
                .Concat(shoulderPadsDef.costList))
                if (thingCounts.ContainsKey(thingCount.thingDef))
                    thingCounts[thingCount.thingDef] += thingCount.count;
                else
                    thingCounts[thingCount.thingDef] = thingCount.count;
            ;

            var ingredientCountList = new List<IngredientCount>();
            foreach (var data in thingCounts) ingredientCountList.Add(new ThingDefCountClass(data.Key, data.Value).ToIngredientCount());
            return ingredientCountList;
        }

        public void ApplyOn(Pawn pawn)
        {
            var armor = ThingMaker.MakeThing(armorDef) as Apparel_Warcasket;
            armor.color = colorArmor;
            pawn.apparel.Wear(armor, false, true);

            var helmet = ThingMaker.MakeThing(helmetDef) as Apparel_Warcasket;
            helmet.color = colorHelmet;
            pawn.apparel.Wear(helmet, false, true);

            var shoulderPads = ThingMaker.MakeThing(shoulderPadsDef) as Apparel_Warcasket;
            shoulderPads.color = colorShoulderPads;
            pawn.apparel.Wear(shoulderPads, false, true);

            Log.Message(colorArmor + " - " + colorHelmet + " - " + colorShoulderPads);
            Log.Message(armor.DrawColor + " - " + helmet.DrawColor + " - " + shoulderPads.DrawColor);
        }

        public void DoWork(float workAmount, out bool workDone)
        {
            currentWorkAmountDone += workAmount;
            if (currentWorkAmountDone >= totalWorkAmount)
                workDone = true;
            else
                workDone = false;
        }
    }
}