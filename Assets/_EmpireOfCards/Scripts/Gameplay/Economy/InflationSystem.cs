using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay.Economy
{
    /// <summary>
    /// Status: partial. Inflation rules exist, but the system is not yet wired
    /// into the live end-of-turn economy pipeline.
    /// </summary>
    public class InflationEvent
    {
        public float supplierCostIncrease;
        public float rentIncrease;
        public float salaryExpectationIncrease;
    }

    /// <summary>
    /// Status: partial. This helper owns inflation cadence and modifiers, but
    /// callers still need to integrate it into the active run economy.
    /// </summary>
    public class InflationSystem
    {
        private int lastInflationTurn;
        private float cumulativeInflation;

        public float CumulativeInflation => cumulativeInflation;

        public bool ShouldTriggerInflation(int currentTurn)
        {
            if (currentTurn <= 0) return false;
            int turnsSinceLast = currentTurn - lastInflationTurn;
            return turnsSinceLast >= Constants.INFLATION_INTERVAL;
        }

        public InflationEvent TriggerInflation(int currentTurn)
        {
            float increase = Random.Range(
                Constants.INFLATION_COST_INCREASE_MIN,
                Constants.INFLATION_COST_INCREASE_MAX);

            cumulativeInflation += increase;
            lastInflationTurn = currentTurn;

            var inflationEvent = new InflationEvent
            {
                supplierCostIncrease = increase,
                rentIncrease = increase,
                salaryExpectationIncrease = Constants.INFLATION_SALARY_INCREASE
            };

            EventBus.InflationOccurred(currentTurn, increase);
            Debug.Log($"[InflationSystem] Inflation triggered at turn {currentTurn}: +{increase:P0} costs, cumulative={cumulativeInflation:P0}");

            return inflationEvent;
        }

        public int ApplyInflationToSupplierCost(int baseCost)
        {
            if (cumulativeInflation <= 0f) return baseCost;
            return Mathf.RoundToInt(baseCost * (1f + cumulativeInflation));
        }

        public int ApplyInflationToRent(int baseRent)
        {
            if (cumulativeInflation <= 0f) return baseRent;
            return Mathf.RoundToInt(baseRent * (1f + cumulativeInflation));
        }

        public int ApplyInflationToSalary(int baseSalary)
        {
            if (cumulativeInflation <= 0f) return baseSalary;
            float salaryInflation = cumulativeInflation * (Constants.INFLATION_SALARY_INCREASE / Constants.INFLATION_COST_INCREASE_MIN);
            return Mathf.RoundToInt(baseSalary * (1f + salaryInflation));
        }

        public void Reset()
        {
            lastInflationTurn = 0;
            cumulativeInflation = 0f;
        }
    }
}
