using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Sound;
namespace VFEPirates
{
    public class CompRumSuit : ThingComp
    {
        private int ticksUntilSpawn;

        public CompProperties_RumSuit PropsSpawner => (CompProperties_RumSuit)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!respawningAfterLoad)
            {
                ResetCountdown();
            }
        }

        public override void CompTick()
        {
            TickInterval(1);
            Log.Message("TEST");
        }

        public override void CompTickRare()
        {
            TickInterval(250);
            Log.Message("TEST");
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            base.Notify_Unequipped(pawn);
            ResetCountdown();
        }

        private void TickInterval(int interval)
        {
            if (!parent.Spawned)
            {
                return;
            }

            Apparel apparel = parent as Apparel;

            if (apparel.Wearer == null)
            {
                return;
            }

            else if (parent.Position.Fogged(parent.Map))
            {
                return;
            }
            
                ticksUntilSpawn -= interval;
                CheckShouldSpawn();
            
        }

        private void CheckShouldSpawn()
        {
            if (ticksUntilSpawn <= 0)
            {
                ResetCountdown();
                TryDoSpawn();
            }
        }

        public bool TryDoSpawn()
        {
            if (!parent.Spawned)
            {
                return false;
            }
            if (PropsSpawner.spawnMaxAdjacent >= 0)
            {
                int num = 0;
                for (int i = 0; i < 9; i++)
                {
                    IntVec3 c = parent.Position + GenAdj.AdjacentCellsAndInside[i];
                    if (!c.InBounds(parent.Map))
                    {
                        continue;
                    }
                    List<Thing> thingList = c.GetThingList(parent.Map);
                    for (int j = 0; j < thingList.Count; j++)
                    {
                        if (thingList[j].def == PropsSpawner.thingToSpawn)
                        {
                            num += thingList[j].stackCount;
                            if (num >= PropsSpawner.spawnMaxAdjacent)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            if (TryFindSpawnCell(parent, PropsSpawner.thingToSpawn, PropsSpawner.spawnCount, out var result))
            {
                Thing thing = ThingMaker.MakeThing(PropsSpawner.thingToSpawn);
                thing.stackCount = PropsSpawner.spawnCount;
                if (thing == null)
                {
                    Log.Error("Could not spawn anything for " + parent);
                }
                if (PropsSpawner.inheritFaction && thing.Faction != parent.Faction)
                {
                    thing.SetFaction(parent.Faction);
                }
                GenPlace.TryPlaceThing(thing, result, parent.Map, ThingPlaceMode.Direct, out var lastResultingThing);
                if (PropsSpawner.spawnForbidden)
                {
                    lastResultingThing.SetForbidden(value: true);
                }
                if (PropsSpawner.showMessageIfOwned && parent.Faction == Faction.OfPlayer)
                {
                    Messages.Message("MessageCompSpawnerSpawnedItem".Translate(PropsSpawner.thingToSpawn.LabelCap), thing, MessageTypeDefOf.PositiveEvent);
                }

                VFEP_DefOf.VFEP_RumFinished.PlayOneShot(new TargetInfo(parent.Position, parent.Map, false));
                return true;


            }
            return false;
        }

        public static bool TryFindSpawnCell(Thing parent, ThingDef thingToSpawn, int spawnCount, out IntVec3 result)
        {
            foreach (IntVec3 item in GenAdj.CellsAdjacent8Way(parent).InRandomOrder())
            {
                if (!item.Walkable(parent.Map))
                {
                    continue;
                }
                Building edifice = item.GetEdifice(parent.Map);
                if (edifice != null && thingToSpawn.IsEdifice())
                {
                    continue;
                }
                Building_Door building_Door = edifice as Building_Door;
                if ((building_Door != null && !building_Door.FreePassage) || (parent.def.passability != Traversability.Impassable && !GenSight.LineOfSight(parent.Position, item, parent.Map)))
                {
                    continue;
                }
                bool flag = false;
                List<Thing> thingList = item.GetThingList(parent.Map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    Thing thing = thingList[i];
                    if (thing.def.category == ThingCategory.Item && (thing.def != thingToSpawn || thing.stackCount > thingToSpawn.stackLimit - spawnCount))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    result = item;
                    return true;
                }
            }
            result = IntVec3.Invalid;
            return false;
        }

        private void ResetCountdown()
        {
            ticksUntilSpawn = PropsSpawner.spawnIntervalRange.RandomInRange;
        }

        public override void PostExposeData()
        {
            string text = (PropsSpawner.saveKeysPrefix.NullOrEmpty() ? null : (PropsSpawner.saveKeysPrefix + "_"));
            Scribe_Values.Look(ref ticksUntilSpawn, text + "ticksUntilSpawn", 0);
        }

      

        public override string CompInspectStringExtra()
        {
            Apparel apparel = parent as Apparel;

            if (apparel.Wearer == null)
            {
                return null;
            }
            if (PropsSpawner.writeTimeLeftToSpawn)
            {
                return "VFEP_NextSpawnedRumIn".Translate().Resolve() + ": " + ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
            }
            return null;
        }
    }
}
