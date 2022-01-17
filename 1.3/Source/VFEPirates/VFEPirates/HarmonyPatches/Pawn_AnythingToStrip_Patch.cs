using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System;
using Verse;
using System.Linq;

namespace VFEPirates
{
    [HarmonyPatch(typeof(Pawn), "AnythingToStrip")]
    public static class Pawn_AnythingToStrip_Patch
    {
        public static bool Prefix(Pawn __instance)
        {
            if (__instance.IsWearingWarcasket())
            {
                return false;
            }
            return true;
        }
    }

    //[HarmonyPatch(typeof(PawnGroupMakerUtility), "ChoosePawnGenOptionsByPoints")]
    //public static class PawnGroupMakerUtility_ChoosePawnGenOptionsByPoints_Patch
    //{
    //    public static bool Prefix(ref IEnumerable<PawnGenOption> __result, float pointsTotal, List<PawnGenOption> options, PawnGroupMakerParms groupParms)
	//	{
	//		__result = ChoosePawnGenOptionsByPoints(pointsTotal, options, groupParms).ToList();
	//		return false;
	//	}
	//
	//	public static IEnumerable<PawnGenOption> ChoosePawnGenOptionsByPoints(float pointsTotal, List<PawnGenOption> options, PawnGroupMakerParms groupParms)
	//	{
	//		Log.Message("STARTING ANALYZING");
	//		if (groupParms.seed.HasValue)
	//		{
	//			Rand.PushState(groupParms.seed.Value);
	//		}
	//		float num = PawnGroupMakerUtility.MaxPawnCost(groupParms.faction, pointsTotal, groupParms.raidStrategy, groupParms.groupKind);
	//		List<PawnGenOption> list = new List<PawnGenOption>();
	//		List<PawnGenOption> list2 = new List<PawnGenOption>();
	//		float num2 = pointsTotal;
	//		bool flag = false;
	//		float highestCost = -1f;
	//		while (true)
	//		{
	//			list.Clear();
	//			Log.Message("Options: " + options.Count);
	//			for (int i = 0; i < options.Count; i++)
	//			{
	//				PawnGenOption pawnGenOption = options[i];
	//				if (!(pawnGenOption.Cost > num2) && !(pawnGenOption.Cost > num) 
	//					&& PawnGroupMakerUtility.PawnGenOptionValid(pawnGenOption, groupParms) 
	//					&& (groupParms.raidStrategy == null || groupParms.raidStrategy.Worker.CanUsePawnGenOption(pointsTotal, pawnGenOption, list2, groupParms.faction)) 
	//					&& (!flag || !pawnGenOption.kind.factionLeader))
	//				{
	//					if (pawnGenOption.Cost > highestCost)
	//					{
	//						highestCost = pawnGenOption.Cost;
	//					}
	//					Log.Message("Added with: " + pawnGenOption.kind + " - " + pawnGenOption.Cost);
	//					list.Add(pawnGenOption);
	//				}
	//				else
    //                {
	//					Log.Message($"Failed to add pawngenoption: pawnGenOption.kind: {pawnGenOption.kind}, pawnGenOption.Cost: {pawnGenOption.Cost}, num2: {num2}, num: {num}," +
	//						$" PawnGenOptionValid: { PawnGroupMakerUtility.PawnGenOptionValid(pawnGenOption, groupParms) }" +
	//						$", groupParms.raidStrategy: { groupParms.raidStrategy}, CanUsePawnGenOption:" +
	//						$"{groupParms.raidStrategy?.Worker?.CanUsePawnGenOption(pointsTotal, pawnGenOption, list2, groupParms.faction)}" +
	//						$", pawnGenOption.kind.factionLeader: {pawnGenOption.kind.factionLeader}");
    //                }
	//			}
	//			if (list.Count == 0)
	//			{
	//				Log.Message("list.Count == 0");
	//				break;
	//			}
	//			Func<PawnGenOption, float> weightSelector = (PawnGenOption gr) => gr.selectionWeight 
	//			* PawnWeightFactorByMostExpensivePawnCostFractionCurve.Evaluate(gr.Cost / highestCost);
	//			PawnGenOption pawnGenOption2 = list.RandomElementByWeight(weightSelector);
	//			list2.Add(pawnGenOption2);
	//			num2 -= pawnGenOption2.Cost;
	//			if (pawnGenOption2.kind.factionLeader)
	//			{
	//				flag = true;
	//			}
	//		}
	//		if (list2.Count == 1 && num2 > pointsTotal / 2f)
	//		{
	//			Log.Warning("Used only " + (pointsTotal - num2) + " / " + pointsTotal + " points generating for " + groupParms.faction);
	//		}
	//		if (groupParms.seed.HasValue)
	//		{
	//			Rand.PopState();
	//		}
	//		return list2;
	//	}
	//
	//	private static readonly SimpleCurve PawnWeightFactorByMostExpensivePawnCostFractionCurve = new SimpleCurve
	//	{
	//		new CurvePoint(0.2f, 0.01f),
	//		new CurvePoint(0.3f, 0.3f),
	//		new CurvePoint(0.5f, 1f)
	//	};
	//}
}
