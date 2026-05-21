using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay
{
    public static class ScoreCalculator
    {
        /// <summary>
        /// Calculates final score based on player performance.
        /// Returns (total, grade, breakdown).
        /// </summary>
        public static (int total, ScoreGrade grade, Dictionary<string, int> breakdown) Calculate(
            PlayerResources res, int currentTurn, int crisesSolved, bool usedRiskCards)
        {
            var b = new Dictionary<string, int>();

            b["Market Share"] = Mathf.RoundToInt(res.GetMarketShare()) * Constants.SCORE_MARKET_SHARE;
            b["Cash"] = (res.GetMoney() / 100) * Constants.SCORE_CASH;
            b["Rating"] = Mathf.RoundToInt(res.GetRating()) * Constants.SCORE_RATING;
            b["Krizler"] = crisesSolved * Constants.SCORE_CRISIS_SOLVED;
            b["Cag"] = (int)GameManager.GetEra(currentTurn) * Constants.SCORE_ERA_REACHED;

            if (!usedRiskCards)
                b["Temiz Oyun"] = Constants.SCORE_CLEAN_PLAY;

            if (currentTurn < 20)
                b["Hizli Bitis"] = Constants.SCORE_FAST_FINISH;

            int total = 0;
            foreach (var v in b.Values)
                total += v;

            ScoreGrade grade = total >= 3000 ? ScoreGrade.S :
                               total >= 2000 ? ScoreGrade.A :
                               total >= 1500 ? ScoreGrade.B :
                               total >= 1000 ? ScoreGrade.C :
                               total >= 500  ? ScoreGrade.D : ScoreGrade.F;

            return (total, grade, b);
        }
    }
}
