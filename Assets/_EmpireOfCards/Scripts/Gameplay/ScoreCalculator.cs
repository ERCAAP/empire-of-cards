using System.Collections.Generic;
using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay
{
    public static class ScoreCalculator
    {
        /// <summary>
        /// Returns (totalScore, grade, breakdown dictionary).
        /// </summary>
        public static (int total, ScoreGrade grade, Dictionary<string, int> breakdown) Calculate(
            float marketShare,
            int cash,
            float rating,
            int crisesSolved,
            Era eraReached,
            float legalRisk,
            int finishTurn)
        {
            var breakdown = new Dictionary<string, int>();

            int sharePoints = (int)(marketShare * Constants.SCORE_MARKET_SHARE);
            breakdown["MarketShare"] = sharePoints;

            int cashPoints = cash > 0 ? (cash / 100) * Constants.SCORE_CASH : 0;
            breakdown["Cash"] = cashPoints;

            int ratingPoints = (int)(rating * Constants.SCORE_RATING);
            breakdown["Rating"] = ratingPoints;

            int crisisPoints = crisesSolved * Constants.SCORE_CRISIS_SOLVED;
            breakdown["CrisesSolved"] = crisisPoints;

            int eraPoints = (int)eraReached * Constants.SCORE_ERA_REACHED;
            breakdown["EraReached"] = eraPoints;

            bool cleanPlay = legalRisk < 10f;
            int cleanPoints = cleanPlay ? Constants.SCORE_CLEAN_PLAY : 0;
            breakdown["CleanPlay"] = cleanPoints;

            bool fastFinish = finishTurn <= Constants.FAST_FINISH_TURN;
            int fastPoints = fastFinish ? Constants.SCORE_FAST_FINISH : 0;
            breakdown["FastFinish"] = fastPoints;

            int total = sharePoints + cashPoints + ratingPoints
                      + crisisPoints + eraPoints + cleanPoints + fastPoints;

            ScoreGrade grade = GradeFromTotal(total);

            return (total, grade, breakdown);
        }

        static ScoreGrade GradeFromTotal(int total)
        {
            if (total >= Constants.GRADE_S_THRESHOLD) return ScoreGrade.S;
            if (total >= Constants.GRADE_A_THRESHOLD) return ScoreGrade.A;
            if (total >= Constants.GRADE_B_THRESHOLD) return ScoreGrade.B;
            if (total >= Constants.GRADE_C_THRESHOLD) return ScoreGrade.C;
            if (total >= Constants.GRADE_D_THRESHOLD) return ScoreGrade.D;
            return ScoreGrade.F;
        }
    }
}
