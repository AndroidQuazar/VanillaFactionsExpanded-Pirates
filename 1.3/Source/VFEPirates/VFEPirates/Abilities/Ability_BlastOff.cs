using HarmonyLib;
using Ionic.Zlib;
using Mono.Unix.Native;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Verse;
using Verse.AI.Group;
using Verse.Noise;
using Verse.Sound;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;
using Command_Ability = VFECore.Abilities.Command_Ability;

namespace VFEPirates
{
    public class BlastOffExtension : DefModExtension
    {
        public float fuelConsumption;
        public int maxLaunchDistance;
    }
    public class Ability_BlastOff : Ability
    {
        public float FuelConsumption => this.def.GetModExtension<BlastOffExtension>().fuelConsumption;
        public int MaxLaunchDistance => this.def.GetModExtension<BlastOffExtension>().maxLaunchDistance;
        public override Gizmo GetGizmo()
        {
            Command_Ability action = new Command_Ability(this.pawn, this);
            if (this.holder.TryGetComp<CompReloadable>().RemainingCharges < FuelConsumption)
            {
                action.Disable("VFEP.NotEnoughFuel".Translate());
            }
            return action;
        }
        public bool ValidateGlobalTarget(GlobalTargetInfo target)
        {
            return target.IsMapTarget;
        }
        public override void DoAction()
        {
            base.DoAction();
            CameraJumper.TryJump(CameraJumper.GetWorldTarget(this.pawn));
            var texture = TravelingPawn.MakeReadableTextureInstance(PortraitsCache.Get(pawn, new Vector2(50, 50), Rot4.South));
            Find.WorldTargeter.BeginTargeting(ChoseWorldTarget, canTargetTiles: true, texture, closeWorldTabWhenFinished: false, delegate
            {

            }, (GlobalTargetInfo target) => TargetingLabelGetter(target, pawn.Tile, MaxLaunchDistance, Gen.YieldSingle(pawn), TryLaunch, null));
            bool ChoseWorldTarget(GlobalTargetInfo target)
            {
                return ChoseWorldTargetForBlastOff(target, pawn.Tile, Gen.YieldSingle(pawn), MaxLaunchDistance, TryLaunch, null);
            }
        }
        public bool ChoseWorldTargetForBlastOff(GlobalTargetInfo target, int tile, IEnumerable<IThingHolder> pods, int maxLaunchDistance, Action<int, TransportPodsArrivalAction> launchAction, CompLaunchable launchable)
        {
            if (!target.IsValid)
            {
                Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                return false;
            }
            int num = Find.WorldGrid.TraversalDistanceBetween(tile, target.Tile);
            if (maxLaunchDistance > 0 && num > maxLaunchDistance)
            {
                Messages.Message("TransportPodDestinationBeyondMaximumRange".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                return false;
            }
            if (Find.World.Impassable(target.Tile))
            {
                Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                return false;
            }
            launchAction(target.Tile, null);
            return true;
        }

        private ThingWithComps transporter;
        private CompTransporter cachedCompTransporter;
        public CompTransporter Transporter
        {
            get
            {
                if (cachedCompTransporter == null)
                {
                    cachedCompTransporter = transporter.GetComp<CompTransporter>();
                }
                return cachedCompTransporter;
            }
        }
        public void TryLaunch(int destinationTile, TransportPodsArrivalAction arrivalAction)
        {
            var comp = this.holder.TryGetComp<CompReloadable>();
            for (var i = 0; i < FuelConsumption; i++)
            {
                comp.UsedOnce();
            }
            var map = this.pawn.Map;
            transporter = ThingMaker.MakeThing(ThingDefOf.TransportPod) as ThingWithComps;
            GenSpawn.Spawn(transporter, this.pawn.Position, map);
            this.pawn.DeSpawn();
            Transporter.innerContainer.TryAdd(this.pawn);
            TransporterUtility.InitiateLoading(Gen.YieldSingle(Transporter));
            Transporter.TryRemoveLord(map);
            int groupID = Transporter.groupID;
            CompTransporter compTransporter = Transporter;
            ThingOwner directlyHeldThings = compTransporter.GetDirectlyHeldThings();
            ActiveDropPawn activeDropPawn = (ActiveDropPawn)ThingMaker.MakeThing(VFEP_DefOf.VFEP_ActiveDropPawn);
            activeDropPawn.pawn = this.pawn;
            activeDropPawn.Contents = new ActiveDropPodInfo();
            activeDropPawn.Contents.despawnPodBeforeSpawningThing = true;
            activeDropPawn.Contents.openDelay = 0;
            activeDropPawn.Contents.innerContainer.TryAddRangeOrTransfer(directlyHeldThings, canMergeWithExistingStacks: true, destroyLeftover: true);
            PawnLeaving obj = (PawnLeaving)SkyfallerMaker.MakeSkyfaller(VFEP_DefOf.VFEP_PawnLeaving, activeDropPawn);
            obj.pawn = this.pawn;
            obj.groupID = groupID;
            obj.destinationTile = destinationTile;
            obj.worldObjectDef = VFEP_DefOf.VFEP_TravelingPawn;
            compTransporter.CleanUpLoadingVars(map);
            compTransporter.parent.Destroy();
            GenSpawn.Spawn(obj, compTransporter.parent.Position, map);
            CameraJumper.TryHideWorld();
            transporter = null;
            cachedCompTransporter = null;
        }

        public static string TargetingLabelGetter(GlobalTargetInfo target, int tile, int maxLaunchDistance, IEnumerable<IThingHolder> pods, Action<int, TransportPodsArrivalAction> launchAction, CompLaunchable launchable)
        {
            if (!target.IsValid)
            {
                return null;
            }
            int num = Find.WorldGrid.TraversalDistanceBetween(tile, target.Tile);
            if (maxLaunchDistance > 0 && num > maxLaunchDistance)
            {
                GUI.color = ColorLibrary.RedReadable;
                return "TransportPodDestinationBeyondMaximumRange".Translate();
            }
            return string.Empty;
        }
    }

    public class PawnIncoming : DropPodIncoming
    {
        public Pawn pawn;
        public override Graphic Graphic => pawn.Graphic;
        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            float num = 0f;
            if (def.skyfaller.rotateGraphicTowardsDirection)
            {
                num = angle;
            }
            if (def.skyfaller.angleCurve != null)
            {
                angle = def.skyfaller.angleCurve.Evaluate(TimeInAnimation);
            }
            if (def.skyfaller.rotationCurve != null)
            {
                num += def.skyfaller.rotationCurve.Evaluate(TimeInAnimation);
            }
            if (def.skyfaller.xPositionCurve != null)
            {
                drawLoc.x += def.skyfaller.xPositionCurve.Evaluate(TimeInAnimation);
            }
            if (def.skyfaller.zPositionCurve != null)
            {
                drawLoc.z += def.skyfaller.zPositionCurve.Evaluate(TimeInAnimation);
            }
            pawn.Drawer.renderer.RenderPawnAt(drawLoc, pawn.Rotation);
            DrawDropSpotShadow();
        }

        private Effecter flightEffecter;
        public override void Tick()
        {
            if (flightEffecter == null && def.pawnFlyer.flightEffecterDef != null)
            {
                flightEffecter = def.pawnFlyer.flightEffecterDef.Spawn();
                flightEffecter.Trigger(this, TargetInfo.Invalid);
            }
            else
            {
                flightEffecter?.EffectTick(this, TargetInfo.Invalid);
            }
            base.Tick();
        }

        protected override void Impact()
        {
            base.Impact();
            GenExplosion.DoExplosion(this.pawn.PositionHeld, this.pawn.MapHeld, 15, DamageDefOf.Bomb, pawn, 15, ignoredThings: Gen.YieldSingle<Thing>(pawn).ToList());
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref pawn, "pawn");
        }
    }
    public class PawnLeaving : FlyShipLeaving
    {
        public Pawn pawn;
        public override Graphic Graphic => pawn.Graphic;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            ticksToDiscard = 500;
        }
        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            float num = 0f;
            if (def.skyfaller.rotateGraphicTowardsDirection)
            {
                num = angle;
            }
            if (def.skyfaller.angleCurve != null)
            {
                angle = def.skyfaller.angleCurve.Evaluate(TimeInAnimation);
            }
            if (def.skyfaller.rotationCurve != null)
            {
                num += def.skyfaller.rotationCurve.Evaluate(TimeInAnimation);
            }
            if (def.skyfaller.xPositionCurve != null)
            {
                drawLoc.x += def.skyfaller.xPositionCurve.Evaluate(TimeInAnimation);
            }
            if (def.skyfaller.zPositionCurve != null)
            {
                drawLoc.z += def.skyfaller.zPositionCurve.Evaluate(TimeInAnimation);
            }
            pawn.Drawer.renderer.RenderPawnAt(drawLoc, pawn.Rotation);
            DrawDropSpotShadow();
        }

        private Effecter flightEffecter;
        public override void Tick()
        {
            if (flightEffecter == null && def.pawnFlyer.flightEffecterDef != null)
            {
                flightEffecter = def.pawnFlyer.flightEffecterDef.Spawn();
                flightEffecter.Trigger(this, TargetInfo.Invalid);
            }
            else
            {
                flightEffecter?.EffectTick(this, TargetInfo.Invalid);
            }
            base.Tick();
        }

        private static List<Thing> tmpActiveDropPods = new List<Thing>();
        protected override void LeaveMap()
        {
            var alreadyLeftField = Traverse.Create(this).Field<bool>("alreadyLeft");
            if (alreadyLeftField.Value || !createWorldObject)
            {
                if (Contents != null)
                {
                    foreach (Thing item in (IEnumerable<Thing>)Contents.innerContainer)
                    {
                        Pawn pawn;
                        if ((pawn = item as Pawn) != null)
                        {
                            pawn.ExitMap(allowedToJoinOrCreateCaravan: false, Rot4.Invalid);
                        }
                    }
                    Contents.innerContainer.ClearAndDestroyContentsOrPassToWorld(DestroyMode.QuestLogic);
                }
                base.LeaveMap();
                return;
            }
            if (groupID < 0)
            {
                Log.Error("Drop pod left the map, but its group ID is " + groupID);
                Destroy();
                return;
            }
            if (destinationTile < 0)
            {
                Log.Error("Drop pod left the map, but its destination tile is " + destinationTile);
                Destroy();
                return;
            }
            Lord lord = TransporterUtility.FindLord(groupID, base.Map);
            if (lord != null)
            {
                base.Map.lordManager.RemoveLord(lord);
            }
            TravelingPawn travelingPawn = (TravelingPawn)WorldObjectMaker.MakeWorldObject(worldObjectDef);
            travelingPawn.pawn = this.pawn;
            travelingPawn.Tile = base.Map.Tile;
            travelingPawn.SetFaction(Faction.OfPlayer);
            travelingPawn.destinationTile = destinationTile;
            travelingPawn.arrivalAction = arrivalAction;
            Find.WorldObjects.Add(travelingPawn);
            tmpActiveDropPods.Clear();
            tmpActiveDropPods.AddRange(base.Map.listerThings.ThingsInGroup(ThingRequestGroup.ActiveDropPod));
            for (int i = 0; i < tmpActiveDropPods.Count; i++)
            {
                FlyShipLeaving flyShipLeaving = tmpActiveDropPods[i] as FlyShipLeaving;
                if (flyShipLeaving != null && flyShipLeaving.groupID == groupID)
                {
                    Traverse.Create(this).Field("alreadyLeft").SetValue(true);
                    travelingPawn.AddPod(flyShipLeaving.Contents, justLeftTheMap: true);
                    flyShipLeaving.Contents = null;
                    flyShipLeaving.Destroy();
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref pawn, "pawn");
        }
    }


    public class ActiveDropPawn : ActiveDropPod
    {
        public Pawn pawn;
        public override Graphic Graphic => pawn.Graphic;
        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            pawn.Drawer.renderer.RenderPawnAt(DrawPos, Rot4.South);
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref pawn, "pawn");
        }
    }
    public class TravelingPawn : TravelingTransportPods
    {
        public Pawn pawn;
        public Texture2D PawnTexture => MakeReadableTextureInstance(PortraitsCache.Get(pawn, new Vector2(50, 50), Rot4.South));
        public override Material Material => MaterialPool.MatFrom(PawnTexture);

        [TweakValue("00", 0, 1)] public static float drawTest = 0.083f;
        public override string Label => pawn.Label;
        public override string LabelShort => pawn.LabelShort;
        public override string LabelShortCap => pawn.LabelShortCap;
        public override void Draw()
        {
            float averageTileSize = Find.WorldGrid.averageTileSize;
            var pos = DrawPos;
            var size = 0.7f * averageTileSize;
            var altOffset = drawTest;
            var material = Material;
            Vector3 normalized = pos.normalized;
            Vector3 vector = normalized;
            Quaternion q = Quaternion.LookRotation(Vector3.up, vector);
            Vector3 s = new Vector3(size, 1f, size);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(pos + normalized * altOffset, q, s);
            int layer = WorldCameraManager.WorldLayer;
            Graphics.DrawMesh(MeshPool.plane10, matrix, material, layer);
        }
        public static Texture2D MakeReadableTextureInstance(RenderTexture source)
        {
            RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            temporary.name = "MakeReadableTexture_Temp";
            Graphics.Blit(source, temporary);
            RenderTexture active = RenderTexture.active;
            RenderTexture.active = temporary;
            Texture2D texture2D = new Texture2D(source.width, source.height);
            texture2D.ReadPixels(new Rect(0f, 0f, temporary.width, temporary.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = active;
            RenderTexture.ReleaseTemporary(temporary);
            return texture2D;
        }

        [HarmonyPatch(typeof(TravelingTransportPods), "Arrived")]
        public static class TravelingTransportPods_Arrived
        {
            public static bool Prefix(TravelingTransportPods __instance)
            {
                if (__instance is TravelingPawn tp)
                {
                    tp.ArrivedOverride();
                    return false;
                }
                return true;
            }
        }
        public void ArrivedOverride()
        {
            var arrivedField = Traverse.Create(this).Field("arrived");
            var podsField = Traverse.Create(this).Field("pods");
            if (arrivedField.GetValue<bool>())
            {
                return;
            }
            arrivedField.SetValue(true);
            if (arrivalAction == null || !arrivalAction.StillValid(podsField.GetValue<List<ActiveDropPodInfo>>().Cast<IThingHolder>(), destinationTile))
            {
                arrivalAction = null;
                List<Map> maps = Find.Maps;
                for (int i = 0; i < maps.Count; i++)
                {
                    if (maps[i].Tile == destinationTile)
                    {
                        arrivalAction = new PawnArrivalAction_LandInSpecificCell(maps[i].Parent, pawn);
                        break;
                    }
                }
                if (arrivalAction == null)
                {
                    if (TransportPodsArrivalAction_FormCaravan.CanFormCaravanAt(podsField.GetValue<List<ActiveDropPodInfo>>().Cast<IThingHolder>(), destinationTile))
                    {
                        arrivalAction = new TransportPodsArrivalAction_FormCaravan();
                    }
                    else
                    {
                        List<Caravan> caravans = Find.WorldObjects.Caravans;
                        for (int j = 0; j < caravans.Count; j++)
                        {
                            if (caravans[j].Tile == destinationTile && (bool)TransportPodsArrivalAction_GiveToCaravan.CanGiveTo(podsField.GetValue<List<ActiveDropPodInfo>>().Cast<IThingHolder>(), caravans[j]))
                            {
                                arrivalAction = new TransportPodsArrivalAction_GiveToCaravan(caravans[j]);
                                break;
                            }
                        }
                    }
                }
            }
            if (arrivalAction != null && arrivalAction.ShouldUseLongEvent(podsField.GetValue<List<ActiveDropPodInfo>>(), destinationTile))
            {
                LongEventHandler.QueueLongEvent(delegate
                {
                    AccessTools.Method(typeof(TravelingTransportPods), "DoArrivalAction").Invoke(this, null);
                }, "GeneratingMapForNewEncounter", doAsynchronously: false, null);
            }
            else
            {
                AccessTools.Method(typeof(TravelingTransportPods), "DoArrivalAction").Invoke(this, null);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref pawn, "pawn");
        }
    }

    public class PawnArrivalAction_LandInSpecificCell : TransportPodsArrivalAction_LandInSpecificCell
    {
        private MapParent mapParent;

        private Pawn pawn;
        public PawnArrivalAction_LandInSpecificCell()
        {
        }

        public PawnArrivalAction_LandInSpecificCell(MapParent mapParent, Pawn pawn)
        {
            this.mapParent = mapParent;
            this.pawn = pawn;
        }
        public override void Arrived(List<ActiveDropPodInfo> pods, int tile)
        {
            Thing lookTarget = TransportPodsArrivalActionUtility.GetLookTarget(pods);
            DropTravelingPawns(pods, mapParent.Map.Center, mapParent.Map);
            Messages.Message("VFEP.MessagePawnArrived".Translate(lookTarget.Named("PAWN")), lookTarget, MessageTypeDefOf.TaskCompletion);
        }

        public void DropTravelingPawns(List<ActiveDropPodInfo> dropPods, IntVec3 near, Map map)
        {
            TransportPodsArrivalActionUtility.RemovePawnsFromWorldPawns(dropPods);
            for (int i = 0; i < dropPods.Count; i++)
            {
                DropCellFinder.TryFindDropSpotNear(near, map, out var result, allowFogged: false, canRoofPunch: true);
                MakePawnDropAt(result, map, dropPods[i]);
            }
        }

        public void MakePawnDropAt(IntVec3 c, Map map, ActiveDropPodInfo info)
        {
            ActiveDropPawn activeDropPod = (ActiveDropPawn)ThingMaker.MakeThing(VFEP_DefOf.VFEP_ActiveDropPawn);
            activeDropPod.pawn = this.pawn;
            activeDropPod.Contents = info;
            var pawnIncoming = SkyfallerMaker.SpawnSkyfaller(VFEP_DefOf.VFEP_PawnIncoming, activeDropPod, c, map) as PawnIncoming;
            pawnIncoming.pawn = activeDropPod.Contents.innerContainer.First() as Pawn;
            foreach (Thing item in (IEnumerable<Thing>)activeDropPod.Contents.innerContainer)
            {
                Pawn pawn;
                if ((pawn = item as Pawn) != null && pawn.IsWorldPawn())
                {
                    Find.WorldPawns.RemovePawn(pawn);
                    pawn.psychicEntropy?.SetInitialPsyfocusLevel();
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref pawn, "pawn");
        }
    }
}