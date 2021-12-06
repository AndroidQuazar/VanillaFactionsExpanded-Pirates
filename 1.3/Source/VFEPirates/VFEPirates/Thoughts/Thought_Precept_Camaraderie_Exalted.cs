using System;
using Verse;
using RimWorld;

namespace VFEPirates
{
	public class Thought_Precept_Camaraderie_Exalted : ThoughtWorker_Precept_Social
	{
		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{

            if (StaticCollectionsClass.crewMembersLost == 0)
            {
				return false;
			}

			else return ThoughtState.ActiveAtStage(0);
			


		}

		public override float MoodMultiplier(Pawn p)
		{
			if (StaticCollectionsClass.crewMembersLost < 20)
			{
				return StaticCollectionsClass.crewMembersLost;

			}
			else return 20;
		}


	}
}