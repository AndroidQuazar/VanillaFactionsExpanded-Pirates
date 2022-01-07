using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFEPirates
{
    [StaticConstructorOnStartup]
    public static class PawnApparelGenerator_GenerateStartingApparelFor_Patch
    {
        static PawnApparelGenerator_GenerateStartingApparelFor_Patch()
        {
            VFEPiratesMod.harmony.Patch(AccessTools.Method(typeof(PawnApparelGenerator), "GenerateStartingApparelFor"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(PawnApparelGenerator_GenerateStartingApparelFor_Patch), nameof(Postfix))));
        }
        public static void Postfix(Pawn pawn, PawnGenerationRequest request)
        {
            if (pawn.IsWearingWarcasket())
            {
                CheckApparels(pawn, VFEPiratesMod.allArmorDefs);
                CheckApparels(pawn, VFEPiratesMod.allShoulderPadsDefs);
                CheckApparels(pawn, VFEPiratesMod.allHelmetDefs);
            }
            void CheckApparels(Pawn pawn, List<WarcasketDef> apparels)
            {
                if (!pawn.apparel.WornApparel.Any(x => apparels.Contains(x.def)))
                {
                    var armorDef = apparels.RandomElement();
                    var armor = ThingMaker.MakeThing(armorDef, GenStuff.RandomStuffFor(armorDef)) as Apparel;
                    pawn.apparel.Wear(armor, false, true);
                }
            }
        }
    }
}
