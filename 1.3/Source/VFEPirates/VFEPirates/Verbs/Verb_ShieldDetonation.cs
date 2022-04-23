using RimWorld;
using System.Collections.Generic;
using Verse;
using VFECore;

namespace VFEPirates
{
    public class Verb_ShieldDetonation : Verb
    {
        protected override bool TryCastShot()
        {
            var comp = EquipmentSource.GetComp<CompShieldBubble>();
            GenExplosion.DoExplosion(Caster.Position, Caster.Map, comp.Energy / 50f, DamageDefOf.Bomb, Caster, 
                (int)(comp.Energy / 20f), weapon: EquipmentSource.def, ignoredThings: new List<Thing> { Caster});
            comp.Energy = 0;
            return true;
        }
    }
}
