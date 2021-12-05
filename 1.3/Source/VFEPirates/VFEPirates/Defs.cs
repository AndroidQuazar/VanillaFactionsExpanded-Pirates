﻿using Verse;

namespace VFEPirates
{
    public class WarcasketDef : ThingDef
    {
        public string shortDescription;
        public bool isArmor;
        public bool isShoulderPads;
        public bool isHelmet;
    }
    public class ApparelExtension : DefModExtension
    {
        public bool nonSpawnable;
        public bool hiddenFromDatabases;
        public bool isWarCasketApparel;
    }
}
