﻿using HarmonyLib;
using Mono.Unix.Native;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.Noise;

namespace VFEPirates
{
    public class WorkGiver_DoWelding : WorkGiver_Scanner
	{
		public override PathEndMode PathEndMode => PathEndMode.InteractionCell;
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(VFEP_DefOf.VFEP_WarcasketFoundry);
		public override Danger MaxPathDanger(Pawn pawn) => Danger.Some;
        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
		{
			if (thing is Building_WarcasketFoundry foundry)
			{
				if (!foundry.ReadyForWelding(pawn, out string failReason))
				{
					JobFailReason.Is(failReason);
				}
				else
				{
					List<ThingCount> chosen = new List<ThingCount>();
					var requiredIngredients = foundry.RequiredIngredients().ToList();
					if (!WeldingUtility.TryFindBestFixedIngredients(requiredIngredients, pawn, foundry, chosen))
                    {
						JobFailReason.Is("VFEM.MissingMaterials".Translate(string.Join(", ", requiredIngredients.Select(x => x.ToString()))));
					}
					else
					{
						Job job = JobMaker.MakeJob(VFEP_DefOf.VFEP_DoWelding);
						job.targetA = foundry;
						job.targetB = foundry.occupant;
						job.targetQueueB = new List<LocalTargetInfo>(chosen.Count);
						job.countQueue = new List<int>(chosen.Count);
						for (int i = 0; i < chosen.Count; i++)
						{
							job.targetQueueB.Add(chosen[i].Thing);
							job.countQueue.Add(chosen[i].Count);
						}
						return job;
					}
				}
			}
			return null;
		}
	}
}
