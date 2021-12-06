using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace VFEPirates
{
	public class WorkGiver_DoWelding : WorkGiver_Scanner
	{
		public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(VFEP_DefOf.VFEP_WarcasketFoundry);

		public override Danger MaxPathDanger(Pawn pawn) => Danger.Some;
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
			if (t is Building_WarcasketFoundry foundry)
            {
				return foundry.ReadyForWelding;
			}
            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
		{
			return null;
		}
	}
}
