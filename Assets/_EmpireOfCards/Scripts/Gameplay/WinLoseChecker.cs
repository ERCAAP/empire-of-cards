using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay
{
    public static class WinLoseChecker
    {
        static int _lowRatingStreak;
        static int _bankruptStreak;

        public static void Reset()
        {
            _lowRatingStreak = 0;
            _bankruptStreak = 0;
        }

        /// <summary>
        /// Call at end of each turn. Returns (gameOver, won, reason).
        /// </summary>
        public static (bool gameOver, bool won, string reason) Check(
            int currentTurn,
            int money,
            float rating,
            float legalRisk,
            float playerShare,
            float rivalShare,
            float hygiene)
        {
            // -- Track streaks --------------------------------------------------

            if (rating <= Constants.LOSE_RATING)
                _lowRatingStreak++;
            else
                _lowRatingStreak = 0;

            if (money <= 0)
                _bankruptStreak++;
            else
                _bankruptStreak = 0;

            // -- Lose conditions ------------------------------------------------

            // 1. Bankruptcy: money <= 0 for N consecutive turns
            if (_bankruptStreak >= Constants.LOSE_BANKRUPT_TURNS)
                return (true, false, "Iflas! Paraniz bitti.");

            // 2. Rating collapse: rating <= threshold for N consecutive turns
            if (_lowRatingStreak >= Constants.LOSE_RATING_TURNS)
                return (true, false, "Rating coktu! Musteriler gelmez oldu.");

            // 3. Legal shutdown: legal risk >= threshold
            if (legalRisk >= Constants.LOSE_LEGAL_RISK)
                return (true, false, "Yasal risk cok yuksek! Isletme kapatildi.");

            // 4. Rival dominance: rival market share >= threshold
            if (rivalShare >= Constants.LOSE_RIVAL_SHARE)
                return (true, false, "Rakip pazari ele gecirdi!");

            // 5. Hygiene shutdown: hygiene < threshold
            if (hygiene < Constants.LOSE_HYGIENE)
                return (true, false, "Hijyen felaketi! Saglik mudurlugunce kapatildi.");

            // -- Win conditions -------------------------------------------------

            // Market share dominance
            if (playerShare >= Constants.WIN_MARKET_SHARE)
                return (true, true, "Pazar hakimiyeti! Tebrikler!");

            // Turn limit: ahead at final turn = win, otherwise lose
            if (currentTurn >= Constants.MAX_TURNS)
            {
                bool ahead = playerShare > rivalShare;
                string reason = ahead
                    ? "Sezon sonu: Onde bitirdiniz!"
                    : "Sezon sonu: Rakip onde.";
                return (true, ahead, reason);
            }

            // -- Game continues -------------------------------------------------

            return (false, false, null);
        }
    }
}
