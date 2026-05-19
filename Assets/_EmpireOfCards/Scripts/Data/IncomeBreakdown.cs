using System.Collections.Generic;

namespace EmpireOfCards.Data
{
    /// <summary>
    /// Detailed per-step breakdown of turn income, used by the
    /// Income Cascade Animation (Balatro-style scoring chain).
    /// Built by EconomyManager.ProcessEndOfTurn(), consumed by UIManager.
    /// </summary>
    [System.Serializable]
    public class IncomeBreakdown
    {
        public List<IncomeStep> steps = new List<IncomeStep>();
        public int netIncome;
    }

    /// <summary>
    /// A single line in the income cascade.
    /// Positive = business / combo income, negative = salary / tax.
    /// </summary>
    [System.Serializable]
    public class IncomeStep
    {
        public string label;      // "Diner", "Barista Bonus", "Combo x1.5", "Salaries", "Tax"
        public int amount;        // +50, +30, multiplier value, -110, -46
        public bool isMultiplier; // true for combo multipliers
        public bool isNegative;   // true for salaries, tax, debt

        public IncomeStep() { }

        public IncomeStep(string label, int amount, bool isMultiplier = false, bool isNegative = false)
        {
            this.label = label;
            this.amount = amount;
            this.isMultiplier = isMultiplier;
            this.isNegative = isNegative;
        }
    }
}
