using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace VFEPirates
{
    public class JobDriver_EntombIn : JobDriver
    {
        [TweakValue("00", 0, 10)] public static float zOffset = 0.67f;
        public Building_WarcasketFoundry Foundry => TargetA.Thing as Building_WarcasketFoundry;

        public override Vector3 ForcedBodyOffset
        {
            get
            {
                if (pawn.Position == Foundry.Position) return new Vector3(0, 0, zOffset);
                return default;
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed) => pawn.Reserve(TargetA, job);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.A, Foundry.Position);
            yield return Toils_Reserve.Release(TargetIndex.A);
            var toil = new Toil();
            toil.initAction = delegate
            {
                pawn.Strip();
                Foundry.RegisterOccupant(pawn);
                Foundry.OpenCustomizationWindow(pawn, () =>
                {
                    Foundry.DeregisterOccupant();
                    EndJobWith(JobCondition.Incompletable);
                });
            };
            toil.tickAction = delegate { pawn.rotationTracker.FaceCell(pawn.Position + Rot4.South.FacingCell); };
            toil.socialMode = RandomSocialMode.Quiet;
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.handlingFacing = true;
            yield return toil;
        }
    }
}