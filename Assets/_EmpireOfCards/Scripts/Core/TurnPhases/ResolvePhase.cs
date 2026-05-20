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
    ///         -> 4b.6 MarketShareCalculation -> 4c ComboCheck -> 4c.5 TierCheck
    ///         -> 4d IncomeCalculation -> 4e DeteriorationCheck
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

        // Track whether tier changed this resolve for variable delay
        private bool _tierChangedThisResolve;

        // Track current season for season change detection
        private SeasonType _lastSeason;

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
            _currentStepDelay = 0f;
            _tierChangedThisResolve = false;
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
                ResolveStep.ComboCheck             => 1.0f,
                ResolveStep.TierCheck              => _tierChangedThisResolve ? 1.5f : 0.3f,
                ResolveStep.IncomeCalculation      => 0.8f,
                ResolveStep.DeteriorationCheck     => 0.5f,
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

                // 4b: Calculate total customers and territory distribution
                case ResolveStep.CustomerFlow:
                    if (gm.BoardManager != null)
                        gm.SetPlayerCustomers(gm.BoardManager.CalculatePlayerCustomers());

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
                    int totalMarket = gm.BalanceData != null
                        ? gm.BalanceData.GetMarketPool(gm.CurrentTurn)
                        : Constants.BASE_MARKET_CUSTOMERS;

                    // Apply season multiplier to market pool
                    int seasonIndex = Mathf.Clamp((gm.CurrentTurn - 1) / Constants.TURNS_PER_SEASON, 0, 4);
                    float seasonMultiplier = GetSeasonMultiplier((SeasonType)seasonIndex);

                    int adjustedMarket = Mathf.RoundToInt(totalMarket * seasonMultiplier);

                    // Calculate player share from board state
                    int playerCustomers = gm.PlayerCustomers;
                    int rivalCustomers = gm.RivalCustomers;

                    // Clamp to adjusted market
                    if (adjustedMarket > 0 && (playerCustomers + rivalCustomers) > adjustedMarket)
                    {
                        float total = playerCustomers + rivalCustomers;
                        playerCustomers = Mathf.RoundToInt((playerCustomers / total) * adjustedMarket);
                        rivalCustomers = adjustedMarket - playerCustomers;
                    }

                    gm.SetPlayerCustomers(playerCustomers);
                    gm.SetRivalCustomers(rivalCustomers);
                    EventBus.MarketShareUpdated(playerCustomers, rivalCustomers);

                    Debug.Log($"[ResolvePhase] MarketShare: player={playerCustomers}, rival={rivalCustomers}, market={adjustedMarket} (season x{seasonMultiplier:F2})");
                    break;
                }

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
                    break;

                // 4c.5: Evaluate Company Tier (GDD Section 1.6) - customer-based
                case ResolveStep.TierCheck:
                    if (gm.CompanyTierSystem != null)
                    {
                        CompanyTier tierBefore = gm.CompanyTierSystem.CurrentTier;
                        gm.CompanyTierSystem.EvaluateTier(gm.PlayerCustomers);
                        _tierChangedThisResolve = gm.CompanyTierSystem.CurrentTier != tierBefore;
                    }
                    break;

                // 4d: Calculate and apply income, salaries, tax
                case ResolveStep.IncomeCalculation:
                    if (gm.EconomyManager != null)
                    {
                        gm.EconomyManager.ProcessEndOfTurn();
                    }
                    break;

                // 4e: FBI check, platform rating decay, business closure countdown
                case ResolveStep.DeteriorationCheck:
                    if (gm.FBISystem != null)
                    {
                        gm.FBISystem.AccumulateRiskFromBoard();
                        gm.FBISystem.CheckForRaid();
                    }

                    // Platform rating decay: skip if any business has a Marketing-tagged employee
                    if (gm.EconomyManager != null && gm.BoardManager != null)
                    {
                        bool hasMarketingCoverage = HasMarketingEmployee(gm.BoardManager);
                        if (!hasMarketingCoverage)
                        {
                            Debug.Log("[ResolvePhase] No marketing coverage -- platform rating decays.");
                        }
                        else
                        {
                            Debug.Log("[ResolvePhase] Marketing coverage active -- platform rating decay skipped.");
                        }
                    }
                    break;
            }
        }

        private static float GetSeasonMultiplier(SeasonType season)
        {
            return season switch
            {
                SeasonType.Spring        => Constants.SEASON_MULTIPLIER_SPRING,
                SeasonType.Summer        => Constants.SEASON_MULTIPLIER_SUMMER,
                SeasonType.Autumn        => Constants.SEASON_MULTIPLIER_AUTUMN,
                SeasonType.Winter        => Constants.SEASON_MULTIPLIER_WINTER,
                SeasonType.RamadanSeason => Constants.SEASON_MULTIPLIER_RAMADAN,
                _                        => 1.0f
            };
        }

        private static bool HasMarketingEmployee(BoardManager board)
        {
            var businesses = board.PlayerBusinesses;
            for (int i = 0; i < businesses.Count; i++)
            {
                if (businesses[i].isClosed) continue;
                foreach (var emp in businesses[i].employees)
                {
                    if (emp == null) continue;
                    if (emp.tags == null) continue;
                    foreach (var tag in emp.tags)
                    {
                        if (tag == CardTag.Marketing) return true;
                    }
                }
            }
            return false;
        }

        public void Exit() { }
    }
}
