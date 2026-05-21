using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay
{
    public static class WinLoseChecker
    {
        static int consecutiveLowRatingTurns;
        static int consecutiveNegativeCashTurns;

        public static void Reset()
        {
            consecutiveLowRatingTurns = 0;
            consecutiveNegativeCashTurns = 0;
        }

        /// <summary>
        /// Checks all win/lose conditions after a turn ends.
        /// Returns (gameOver, won, reason).
        /// </summary>
        public static (bool gameOver, bool won, string reason) Check(
            PlayerResources res, float rivalShare, int currentTurn)
        {
            // ── WIN: market share >= 60 ────────────────────────────
            if (res.GetMarketShare() >= Constants.WIN_MARKET_SHARE)
                return (true, true, "Market Share %60'i gectin!");

            // ── WIN: turn 25 with more share than rival ────────────
            if (currentTurn >= Constants.MAX_TURNS && res.GetMarketShare() > rivalShare)
                return (true, true, "25 tur sonunda rakibinden ondeydin!");

            // ── LOSE: bankruptcy (cash <= 0 for 3 consecutive turns)
            if (res.GetMoney() <= 0)
            {
                consecutiveNegativeCashTurns++;
                if (consecutiveNegativeCashTurns >= Constants.LOSE_BANKRUPT_TURNS)
                    return (true, false, "Iflas! 3 tur boyunca paran sifirin altinda kaldi.");
            }
            else
            {
                consecutiveNegativeCashTurns = 0;
            }

            // ── LOSE: rating collapse (<= 1.5 for 3 consecutive turns)
            if (res.GetRating() <= Constants.LOSE_RATING)
            {
                consecutiveLowRatingTurns++;
                if (consecutiveLowRatingTurns >= Constants.LOSE_RATING_TURNS)
                    return (true, false, "Puan cokusu! 3 tur boyunca rating 1.5 altinda kaldi.");
            }
            else
            {
                consecutiveLowRatingTurns = 0;
            }

            // ── LOSE: legal disaster (>= 90) ──────────────────────
            if (res.GetLegalRisk() >= Constants.LOSE_LEGAL_RISK)
                return (true, false, "Hukuki felaket! Yasal risk %90'i asti, isletme kapatildi.");

            // ── LOSE: rival domination (>= 70) ────────────────────
            if (rivalShare >= Constants.LOSE_RIVAL_SHARE)
                return (true, false, "Rakip domine etti! Rakibin pazar payi %70'i gecti.");

            // ── LOSE: hygiene shutdown (< 1) ──────────────────────
            if (res.GetHygiene() < Constants.LOSE_HYGIENE)
                return (true, false, "Hijyen felaketi! Saglik bakanligi isletmeyi kapatti.");

            // ── LOSE: turn 25 with less share than rival ──────────
            if (currentTurn >= Constants.MAX_TURNS && res.GetMarketShare() <= rivalShare)
                return (true, false, "25 tur sonunda rakibinin gerisinde kaldin.");

            return (false, false, "");
        }
    }
}
