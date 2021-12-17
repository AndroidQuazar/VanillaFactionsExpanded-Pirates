using RimWorld;
using Verse;
using System.Collections.Generic;

namespace VFEPirates
{
    public class ThoughtWorker_RumSuit : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
			bool flag = false;
			List<Apparel> wornApparel = p.apparel.WornApparel;
			for (int i = 0; i < wornApparel.Count; i++)
			{
				if (wornApparel[i].def==VFEP_DefOf.VFEP_Apparel_Rumsuit)
				{
					flag = true;
				}
			}

			if (flag)
			{

				return ThoughtState.ActiveAtStage(0);
			}
			else return ThoughtState.Inactive;
		}
    }
}