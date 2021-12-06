using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace VFEPirates
{
    public class JobDriver_EntombIn : JobDriver
    {
        public Building_WarcasketFoundry Foundry => TargetA.Thing as Building_WarcasketFoundry;
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetA, job, 2);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.A, Foundry.Position);
			Toil toil = new Toil();
            toil.initAction = delegate
            {
                pawn.Strip();
                Foundry.RegisterOccupant(pawn);
                Foundry.OpenCustomizationWindow(pawn);
            };
			toil.tickAction = delegate
			{
                base.pawn.rotationTracker.FaceCell(pawn.Position + Rot4.South.FacingCell);
			};
			toil.socialMode = RandomSocialMode.Quiet;
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			toil.handlingFacing = true;
			yield return toil;
		}

        [TweakValue("00", 0, 10)] public static float zOffset = 0.67f;
        public override Vector3 ForcedBodyOffset
        {
            get
            {
                if (pawn.Position == Foundry.Position)
                {
                    return new Vector3(0, 0, zOffset);
                }
                return default;
            }
        }
    }
}
