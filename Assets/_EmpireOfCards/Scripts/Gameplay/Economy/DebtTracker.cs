namespace EmpireOfCards.Gameplay.Economy
{
    /// <summary>
    /// Tracks investor debt from the "Yatirimci Sunumu" action card.
    /// For N turns, a percentage of income is deducted.
    /// Default: +600 gold now, 3 turns x 15% income deducted.
    /// Status: legacy-leaning. The active run economy now relies on
    /// EconomyManager investor-debt state and CreditSystem flow.
    /// </summary>
    public class DebtTracker
    {
        public int TurnsRemaining { get; private set; }
        public float DebtPercent { get; private set; }

        /// <summary>
        /// Starts investor debt: for <paramref name="duration"/> turns,
        /// <paramref name="percent"/> of income is deducted each turn.
        /// </summary>
        public void StartDebt(int duration, float percent)
        {
            TurnsRemaining = duration;
            DebtPercent = percent;
        }

        /// <summary>
        /// Decrements the remaining debt turns by one. Call at end of each turn.
        /// </summary>
        public void Tick()
        {
            if (TurnsRemaining > 0)
                TurnsRemaining--;
        }

        /// <summary>
        /// Resets all debt state.
        /// </summary>
        public void Reset()
        {
            TurnsRemaining = 0;
            DebtPercent = 0f;
        }
    }
}
