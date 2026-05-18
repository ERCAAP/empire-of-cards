using UnityEngine;
using EmpireOfCards.Core.StateMachine;

namespace EmpireOfCards.Core.TurnPhases
{
    /// <summary>
    /// Turn phase 2: Draw 5 cards, allow 1 redraw (GDD Section 4.1, Step 2).
    /// </summary>
    public class DrawPhase : IState
    {
        private readonly TurnManager _turnManager;
        private float _timer;
        private bool _cardsDrawn;

        public DrawPhase(TurnManager tm) { _turnManager = tm; }

        public void Enter()
        {
            _timer = 0f;
            _cardsDrawn = false;

            var dm = GameManager.Instance.DeckManager;
            if (dm != null)
            {
                dm.DiscardHand();
                dm.ResetRedraws();
                dm.DrawCards(Constants.HAND_SIZE);
                _cardsDrawn = true;
            }
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            // Wait for draw animation minimum duration
            if (_timer >= _turnManager.DrawPhaseMinDuration && _cardsDrawn)
            {
                _turnManager.CompleteCurrentPhase();
            }
        }

        public void Exit() { }
    }
}
