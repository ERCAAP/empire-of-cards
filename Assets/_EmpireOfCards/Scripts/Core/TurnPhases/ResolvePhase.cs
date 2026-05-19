using System;
using UnityEngine;
using EmpireOfCards.Core.StateMachine;

namespace EmpireOfCards.Core.TurnPhases
{
    /// <summary>
    /// Turn phase 4: System calculates everything step by step (GDD Section 4.1, Step 4).
    /// Sub-steps: 4a BusinessProduce -> 4b CustomerFlow -> 4c ComboCheck
    ///         -> 4d IncomeCalculation -> 4e DeteriorationCheck
    /// </summary>
    public class ResolvePhase : IState
    {
        private readonly TurnManager _turnManager;
        private readonly ResolveStep[] _steps;
        private int _stepIndex;
        private float _stepTimer;
        private bool _stepExecuted;

        public ResolvePhase(TurnManager tm)
        {
            _turnManager = tm;
            _steps = (ResolveStep[])Enum.GetValues(typeof(ResolveStep));
        }

        public void Enter()
        {
            _stepIndex = 0;
            _stepTimer = 0f;
            _stepExecuted = false;
        }

        public void Tick()
        {
            if (_stepIndex >= _steps.Length)
            {
                _turnManager.CompleteCurrentPhase();
                return;
            }

            if (!_stepExecuted)
            {
                ExecuteStep(_steps[_stepIndex]);
                _stepExecuted = true;
                _stepTimer = 0f;
            }

            _stepTimer += Time.deltaTime;
            if (_stepTimer >= _turnManager.ResolveStepDelay)
            {
                _stepIndex++;
                _stepExecuted = false;
            }
        }

        private void ExecuteStep(ResolveStep step)
        {
            var gm = GameManager.Instance;

            switch (step)
            {
                // 4a: Businesses produce products/customers
                case ResolveStep.BusinessProduce:
                    if (gm.BoardManager != null)
                    {
                        // Check if rival sabotage disabled production this turn
                        if (gm.BoardManager.ConsumeProductionDisabled())
                        {
                            EventBus.RivalActed("Your businesses couldn't produce this turn due to rival sabotage!");
                        }
                        else
                        {
                            gm.BoardManager.TickBusinesses();
                        }
                    }
                    break;

                // 4b: Calculate total customers and territory distribution
                case ResolveStep.CustomerFlow:
                    // Update player customer count from board state
                    if (gm.BoardManager != null)
                        gm.SetPlayerCustomers(gm.BoardManager.CalculatePlayerCustomers());

                    // Update rival customer count
                    if (gm.RivalAI != null)
                        gm.SetRivalCustomers(gm.RivalAI.RivalCustomers);

                    if (gm.TerritoryManager != null)
                    {
                        int marketPool = gm.BalanceData != null
                            ? gm.BalanceData.GetMarketPool(gm.CurrentTurn)
                            : Constants.BASE_MARKET_CUSTOMERS;
                        gm.TerritoryManager.CalculateTerritories(
                            gm.PlayerCustomers, gm.RivalCustomers, marketPool);
                    }
                    break;

                // 4c: Check and trigger combos
                case ResolveStep.ComboCheck:
                    if (gm.ComboSystem != null && gm.BoardManager != null)
                    {
                        int activeBizCount = gm.BoardManager.GetActiveBusinessCount();
                        int totalMarket = gm.BalanceData != null
                            ? gm.BalanceData.GetMarketPool(gm.CurrentTurn)
                            : Constants.BASE_MARKET_CUSTOMERS;
                        float marketShare = totalMarket > 0
                            ? ((float)gm.PlayerCustomers / totalMarket) * 100f
                            : 0f;

                        gm.ComboSystem.CheckCombos(
                            gm.PlayerMoney,
                            gm.PlayerTerritories,
                            activeBizCount,
                            marketShare);
                    }

                    // Evaluate Company Tier (GDD Section 1.6) - after combo check
                    if (gm.CompanyTierSystem != null)
                        gm.CompanyTierSystem.EvaluateTier(gm.PlayerTerritories);
                    break;

                // 4d: Calculate and apply income, salaries, tax
                case ResolveStep.IncomeCalculation:
                    if (gm.EconomyManager != null)
                    {
                        gm.EconomyManager.ProcessEndOfTurn();
                    }
                    break;

                // 4e: FBI check, business closure countdown, employee leaving
                case ResolveStep.DeteriorationCheck:
                    if (gm.FBISystem != null)
                    {
                        gm.FBISystem.AccumulateRiskFromBoard();
                        gm.FBISystem.CheckForRaid();
                    }
                    break;
            }
        }

        public void Exit() { }
    }
}
