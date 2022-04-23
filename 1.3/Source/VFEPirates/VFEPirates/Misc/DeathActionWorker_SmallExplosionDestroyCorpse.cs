using RimWorld;
using Verse;

namespace VFEPirates
{
    public class DeathActionWorker_SmallExplosionDestroyCorpse : DeathActionWorker
    {
        public override RulePackDef DeathRules => RulePackDefOf.Transition_DiedExplosive;

        public override bool DangerousInMelee => true;

        public override void PawnDied(Corpse corpse)
        {
            GenExplosion.DoExplosion(corpse.Position, corpse.Map, 1.9f, DamageDefOf.Flame, corpse.InnerPawn);
            corpse.Destroy();
        }
    }
}
