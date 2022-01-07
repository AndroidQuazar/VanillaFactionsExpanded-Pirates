using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFEPirates
{
    public class RaidStrategyWorker_Gauntlet : RaidStrategyWorker_ImmediateAttack
    {
        public override bool CanUseWith(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            string fDefName = parms.faction.def.defName;
            return base.CanUseWith(parms, groupKind) && ((Faction.OfPirates != null && fDefName == Faction.OfPirates.def.defName) || fDefName == "VFEP_Junkers" || fDefName == "VFEP_Mercenaries");
        }
    }
}
