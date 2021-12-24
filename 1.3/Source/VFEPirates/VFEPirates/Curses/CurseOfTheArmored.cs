using System;
using System.Text;
using System.Collections.Generic;
using Verse;
using RimWorld;
using HarmonyLib;

namespace VFEPirates
{
    public class CurseOfTheArmored : CurseWorker
    {
        public override void DoPatches(Harmony harmony)
        {
            harmony.Patch(original: AccessTools.Method(typeof(VerbProperties), nameof(VerbProperties.AdjustedArmorPenetration), parameters: new Type[] { typeof(Tool), typeof(Pawn), typeof(Thing), typeof(HediffComp_VerbGiver) }), 
                postfix: new HarmonyMethod(AccessTools.Method(typeof(CurseOfTheArmored), nameof(AdjustedArmorPenetrationThing))));
            harmony.Patch(original: AccessTools.Method(typeof(VerbProperties), nameof(VerbProperties.AdjustedArmorPenetration), parameters: new Type[] { typeof(Tool), typeof(Pawn), typeof(ThingDef), typeof(ThingDef), typeof(HediffComp_VerbGiver) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(CurseOfTheArmored), nameof(AdjustedArmorPenetrationThingDef))));
            harmony.Patch(original: AccessTools.Method(typeof(ExtraDamage), nameof(ExtraDamage.AdjustedArmorPenetration), parameters: new Type[] { }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(CurseOfTheArmored), nameof(GetArmorPenetrationBase))));
            harmony.Patch(original: AccessTools.Method(typeof(ExtraDamage), nameof(ExtraDamage.AdjustedArmorPenetration), parameters: new Type[] { typeof(Verb), typeof(Pawn) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(CurseOfTheArmored), nameof(GetArmorPenetrationVerb))));
            harmony.Patch(original: AccessTools.Method(typeof(ProjectileProperties), nameof(ProjectileProperties.GetArmorPenetration), parameters: new Type[] { typeof(float), typeof(StringBuilder) }), 
                postfix: new HarmonyMethod(AccessTools.Method(typeof(CurseOfTheArmored), nameof(GetArmorPenetrationMultiplier))));
        }

        public static void AdjustedArmorPenetrationThing(Tool tool, Pawn attacker, Thing equipment, HediffComp_VerbGiver hediffCompSource, ref float __result)
        {
            if (IsActive(typeof(CurseOfTheArmored)))
            {
                __result /= 2;
            }
        }

        public static void AdjustedArmorPenetrationThingDef(Tool tool, Pawn attacker, ThingDef equipment, ThingDef equipmentStuff, HediffComp_VerbGiver hediffCompSource, ref float __result)
        {
            if (IsActive(typeof(CurseOfTheArmored)))
            {
                __result /= 2;
            }
        }

        public static void GetArmorPenetrationBase(ref float __result)
        {
            if (IsActive(typeof(CurseOfTheArmored)))
            {
                __result /= 2;
            }
        }

        public static void GetArmorPenetrationVerb(Verb verb, Pawn caster, ref float __result)
        {
            if (IsActive(typeof(CurseOfTheArmored)))
            {
                __result /= 2;
            }
        }

        public static void GetArmorPenetrationMultiplier(float weaponDamageMultiplier, StringBuilder explanation, ref float __result)
        {
            if (IsActive(typeof(CurseOfTheArmored)))
            {
                __result /= 2;
            }
        }
    }
}
