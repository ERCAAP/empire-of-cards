using UnityEngine;
using EmpireOfCards.Core.StateMachine;

namespace EmpireOfCards.Core.TurnPhases
{
    /// <summary>
    /// Turn phase 7 (GDD v4 Section 9.1): End-of-turn market update.
    /// Final market share recalculation, season update if needed, turn summary event.
    /// </summary>
    public class MarketUpdatePhase : IState
    {
        private readonly TurnManager _turnManager;
        private float _timer;

        public MarketUpdatePhase(TurnManager tm) { _turnManager = tm; }

        public void Enter()
        {
            _timer = 0f;

            var gm = GameManager.Instance;
            if (gm == null) return;

            // Final market share recalculation based on full turn results
            int playerCustomers = gm.PlayerCustomers;
            int rivalCustomers = gm.RivalCustomers;
            int totalMarket = Constants.TOTAL_MARKET_CUSTOMERS;

            if (playerCustomers + rivalCustomers > totalMarket)
            {
                float total = Mathf.Max(1f, playerCustomers + rivalCustomers);
                playerCustomers = Mathf.RoundToInt((playerCustomers / total) * totalMarket);
                rivalCustomers = totalMarket - playerCustomers;
                gm.SetPlayerCustomers(playerCustomers);
                gm.SetRivalCustomers(rivalCustomers);
            }

            EventBus.MarketShareUpdated(playerCustomers, rivalCustomers);
            gm.SetMarketBlocks(
                Mathf.RoundToInt(playerCustomers / 10f),
                Mathf.RoundToInt(rivalCustomers / 10f));

            // Season update check
            int currentTurn = gm.CurrentTurn;
            int seasonIndex = (currentTurn - 1) / Constants.TURNS_PER_SEASON;
            seasonIndex = Mathf.Clamp(seasonIndex, 0, 4);
            SeasonType newSeason = (SeasonType)seasonIndex;

            if (currentTurn > 1)
            {
                int prevSeasonIndex = (currentTurn - 2) / Constants.TURNS_PER_SEASON;
                prevSeasonIndex = Mathf.Clamp(prevSeasonIndex, 0, 4);
                if (seasonIndex != prevSeasonIndex)
                {
                    EventBus.SeasonChanged(newSeason, seasonIndex);
                    Debug.Log($"[MarketUpdatePhase] Season changed to {newSeason}");
                }
            }

            Debug.Log($"[MarketUpdatePhase] Turn {currentTurn} summary — Player: {playerCustomers}, Rival: {rivalCustomers}, Season: {newSeason}");
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= _turnManager.MarketUpdateMinDuration)
            {
                _turnManager.CompleteCurrentPhase();
            }
        }

        public void Exit() { }
    }
}
