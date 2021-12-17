using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace VFEPirates
{
    public class Hediff_SiegeMode : HediffWithComps
    {
        public Ability_SiegeMode ability; 

        public bool remove;
        public override bool ShouldRemove => remove;
        public override void Tick()
        {
            base.Tick();
            if (this.pawn.pather.Moving)
            {
                remove = true;
                ability.enabled = false;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref remove, "remove");
            Scribe_References.Look(ref ability, "ability");
        }
    }

    [StaticConstructorOnStartup]
    public class CommandAbilityToggle : Command_Toggle
    {
        public static readonly Texture2D CooldownTex =
            SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.1f));

        public Pawn pawn;
        public Ability ability;

        public CommandAbilityToggle(Pawn pawn, Ability ability) : base()
        {
            this.pawn = pawn;
            this.ability = ability;

            this.defaultLabel = ability.def.LabelCap;
            this.defaultDesc = ability.GetDescriptionForPawn();
            this.icon = ability.def.icon;
            this.disabled = !ability.IsEnabledForPawn(out string reason);
            this.disabledReason = reason.Colorize(Color.red);
            this.order = 10f + (ability.def.requiredHediff?.hediffDef?.index ?? 0) + (ability.def.requiredHediff?.minimumLevel ?? 0);
            //this.shrinkable     = true;
        }
        protected override GizmoResult GizmoOnGUIInt(Rect butRect, GizmoRenderParms parms)
        {
            GizmoResult result = base.GizmoOnGUIInt(butRect, parms);
            if (this.disabled && this.ability.cooldown > Find.TickManager.TicksGame)
                GUI.DrawTexture(butRect.RightPartPixels(butRect.width * ((float)(this.ability.cooldown - Find.TickManager.TicksGame) / this.ability.GetCooldownForPawn())), CooldownTex);
            return result;
        }
    }

    public class SiegeModeExtension : DefModExtension
    {
        public HediffDef siegeModeHediff;
        public SoundDef soundEnabled;
        public SoundDef soundDisabled;
    }
    public class Ability_SiegeMode : Ability
    {
        public bool enabled;
        public override Gizmo GetGizmo()
        {
            if (enabled)
            {
                return new CommandAbilityToggle(pawn, this)
                {
                    defaultLabel = "VFEP.SiegeModeOff".Translate(),
                    toggleAction = delegate
                    {
                        enabled = false;
                        DoAction();
                    },
                    isActive = () => enabled = true,
                    icon = ContentFinder<Texture2D>.Get("UI/Abilities/SiegeModeOff"),
                    turnOnSound = this.def.GetModExtension<SiegeModeExtension>().soundEnabled,
                    turnOffSound = this.def.GetModExtension<SiegeModeExtension>().soundDisabled,
                };
            }
            else
            {
                return new CommandAbilityToggle(pawn, this)
                {
                    defaultLabel = "VFEP.SiegeModeOn".Translate(),
                    toggleAction = delegate
                    {
                        enabled = true;
                        DoAction();
                    },
                    isActive = () => enabled = false,
                    icon = ContentFinder<Texture2D>.Get("UI/Abilities/SiegeModeOn"),
                    turnOnSound = this.def.GetModExtension<SiegeModeExtension>().soundEnabled,
                    turnOffSound = this.def.GetModExtension<SiegeModeExtension>().soundDisabled,
                };
            }
        }
        public override void ApplyHediffs(LocalTargetInfo targetInfo)
        {
            if (targetInfo.Pawn != null)
            {
                var extension = this.def.GetModExtension<SiegeModeExtension>();
                if (enabled)
                {
                    var hediff = HediffMaker.MakeHediff(extension.siegeModeHediff, targetInfo.Pawn) as Hediff_SiegeMode;
                    hediff.ability = this;
                    targetInfo.Pawn.health.AddHediff(hediff);
                }
                else
                {
                    var hediff = targetInfo.Pawn.health.hediffSet.GetFirstHediffOfDef(extension.siegeModeHediff);
                    if (hediff != null)
                    {
                        targetInfo.Pawn.health.RemoveHediff(hediff);
                    }
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref enabled, "enabled");
        }
    }
}