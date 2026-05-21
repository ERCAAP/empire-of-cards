using UnityEngine;
using EmpireOfCards.Core.StateMachine;

namespace EmpireOfCards.Core.TurnPhases
{
    /// <summary>
    /// Turn phase 2 (GDD v4 Section 9.1): Player reviews board state before playing cards.
    /// Shows demand, capacity, quality, rating bottlenecks. Auto-completes after minimum duration.
    /// </summary>
    public class PlanningPhase : IState
    {
        private readonly TurnManager _turnManager;
        private float _timer;

        public PlanningPhase(TurnManager tm) { _turnManager = tm; }

        public void Enter()
        {
            _timer = 0f;

            var gm = GameManager.Instance;
            if (gm != null && gm.EconomyManager != null)
            {
                var snapshot = gm.EconomyManager.Snapshot;
                if (snapshot != null)
                {
                    Debug.Log($"[PlanningPhase] Board state — Demand: {snapshot.totalDemand}, " +
                              $"Capacity: {snapshot.totalCapacity}, Quality: {snapshot.quality:F1}, " +
                              $"Rating: {snapshot.rating:F1}, Cash: {gm.PlayerMoney}");
                }
            }
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= _turnManager.PlanningPhaseMinDuration)
            {
                _turnManager.CompleteCurrentPhase();
            }
        }

        public void Exit() { }
    }
}
