using System;
using UnityEngine;
using EmpireOfCards.Core.StateMachine;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;

namespace EmpireOfCards.Core.TurnPhases
{
    /// <summary>
    /// Turn phase 4: System calculates everything step by step (GDD Section 4.1, Step 4).
    /// Sub-steps: 4a BusinessProduce -> 4b CustomerFlow -> 4b.5 SeasonCheck
    ///         -> 4b.6 MarketShareCalculation -> 4d IncomeCalculation
    ///         -> 4e StaffTick -> 4f ChainReactionCheck
    /// Each step has a cinematic delay tuned for readability and drama.
    /// </summary>
    public class ResolvePhase : IState
    {
        private readonly TurnManager _turnManager;
        private readonly ResolveStep[] _steps;
        private int _stepIndex;
        private float _stepTimer;
        private float _currentStepDelay;
        private bool _stepExecuted;

        // Track current season for season change detection
        private SeasonType _lastSeason;

        public ResolvePhase(TurnManager tm)
        {
            _turnManager = tm;
            _steps = new[]
            {
                ResolveStep.BusinessProduce,
                ResolveStep.SeasonCheck,
                ResolveStep.IncomeCalculation,
                ResolveStep.CustomerFlow,
                ResolveStep.MarketShareCalculation,
                ResolveStep.StaffTick,
                ResolveStep.ChainReactionCheck
            };
        }

        public void Enter()
        {
            _stepIndex = 0;
            _stepTimer = 0f;
            _stepExecuted = false;
            _currentStepDelay = 0f;
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
                _currentStepDelay = GetStepDelay(_steps[_stepIndex]);
                _stepExecuted = true;
                _stepTimer = 0f;
            }

            _stepTimer += Time.deltaTime;
            if (_stepTimer >= _currentStepDelay)
            {
                _stepIndex++;
                _stepExecuted = false;
            }
        }

        private float GetStepDelay(ResolveStep step)
        {
            return step switch
            {
                ResolveStep.BusinessProduce        => 0.4f,
                ResolveStep.CustomerFlow           => 0.6f,
                ResolveStep.SeasonCheck            => 0.4f,
                ResolveStep.MarketShareCalculation => 0.5f,
                ResolveStep.IncomeCalculation      => 0.8f,
                ResolveStep.StaffTick              => 0.4f,
                ResolveStep.ChainReactionCheck     => 0.4f,
                _                                  => 0.5f
            };
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

                // 4b: Normalize the economy-resolved customer result before presentation.
                case ResolveStep.CustomerFlow:
                    NormalizeCustomerShare(gm);
                    break;

                // 4b.5: Season transition check
                case ResolveStep.SeasonCheck:
                {
                    int currentTurn = gm.CurrentTurn;
                    int seasonIndex = (currentTurn - 1) / Constants.TURNS_PER_SEASON;
                    seasonIndex = Mathf.Clamp(seasonIndex, 0, 4);
                    SeasonType newSeason = (SeasonType)seasonIndex;

                    if (currentTurn > 1 && newSeason != _lastSeason)
                    {
                        EventBus.SeasonChanged(newSeason, seasonIndex);
                        Debug.Log($"[ResolvePhase] Season changed to {newSeason}");
                    }

                    _lastSeason = newSeason;
                    break;
                }

                // 4b.6: Market share calculation
                case ResolveStep.MarketShareCalculation:
                {
                    if (gm.TerritoryManager != null)
                    {
                        gm.TerritoryManager.CalculateMarketBlocks(
                            gm.PlayerCustomers,
                            gm.RivalCustomers,
                            Constants.TOTAL_MARKET_CUSTOMERS);
                    }

                    EventBus.MarketShareUpdated(gm.PlayerCustomers, gm.RivalCustomers);
                    break;
                }

                // 4d: Calculate and apply income, salaries, tax
                case ResolveStep.IncomeCalculation:
                    if (gm.EconomyManager != null)
                    {
                        gm.EconomyManager.ProcessEndOfTurn();
                        if (gm.EconomyManager.Snapshot != null)
                        {
                            int playerShare = Mathf.RoundToInt(gm.EconomyManager.Snapshot.marketShare);
                            gm.SetPlayerCustomers(playerShare);
                            int rivalShare = Mathf.Clamp(Constants.TOTAL_MARKET_CUSTOMERS - playerShare, 0, Constants.TOTAL_MARKET_CUSTOMERS);
                            gm.SetRivalCustomers(rivalShare);
                        }
                    }
                    break;

                // 4e: Staff moral/fatigue/loyalty/XP tick (GDD 6.1, 6.4)
                case ResolveStep.StaffTick:
                    if (gm.StaffStateSystem != null)
                    {
                        gm.StaffStateSystem.TickAll();
                        Debug.Log("[ResolvePhase] Staff states ticked.");
                    }
                    break;

                // 4g: Chain reaction evaluation (GDD 11.1-11.3)
                case ResolveStep.ChainReactionCheck:
                    if (gm.ChainReactionSystem != null)
                    {
                        gm.ChainReactionSystem.EvaluateChains(gm.CurrentTurn);
                        Debug.Log("[ResolvePhase] Chain reactions evaluated.");
                    }
                    break;
            }
        }

        private static void NormalizeCustomerShare(GameManager gm)
        {
            if (gm == null)
                return;

            int totalMarket = Constants.TOTAL_MARKET_CUSTOMERS;
            int playerCustomers = Mathf.Max(0, gm.PlayerCustomers);
            int rivalCustomers = Mathf.Max(0, gm.RivalCustomers);

            if (playerCustomers + rivalCustomers > totalMarket)
            {
                float total = Mathf.Max(1f, playerCustomers + rivalCustomers);
                playerCustomers = Mathf.RoundToInt((playerCustomers / total) * totalMarket);
                rivalCustomers = totalMarket - playerCustomers;
            }

            gm.SetPlayerCustomers(playerCustomers);
            gm.SetRivalCustomers(rivalCustomers);
        }

        public static float GetVentureSeasonMultiplier(VentureType venture, SeasonType season)
        {
            switch (venture)
            {
                case VentureType.FastFood:
                    return season switch
                    {
                        SeasonType.Summer => 1.10f,
                        SeasonType.Winter => 1.05f,
                        SeasonType.RamadanSeason => 1.20f,
                        _ => 1f
                    };

                case VentureType.Cafe:
                    return season switch
                    {
                        SeasonType.Summer => 1.15f,
                        SeasonType.Winter => 0.95f,
                        SeasonType.RamadanSeason => 0.85f,
                        _ => 1f
                    };

                case VentureType.TechApp:
                    return season switch
                    {
                        SeasonType.Summer => 0.95f,
                        SeasonType.Winter => 1.10f,
                        _ => 1f
                    };

                case VentureType.ClothingStore:
                    return season switch
                    {
                        SeasonType.Autumn => 1.20f,
                        SeasonType.Winter => 1.10f,
                        SeasonType.Summer => 0.95f,
                        _ => 1f
                    };

                case VentureType.GroceryStore:
                    return season switch
                    {
                        SeasonType.Winter => 1.05f,
                        SeasonType.RamadanSeason => 1.15f,
                        _ => 1f
                    };

                default:
                    return 1f;
            }
        }

        public void Exit() { }
    }
}
