using UnityEngine;

namespace EmpireOfCards.Core.StateMachine.TurnStates
{
    /// <summary>
    /// Turn phase where the player draws cards up to hand size
    /// and may perform one redraw before the hand is locked.
    /// </summary>
    public class DrawPhaseState : IState
    {
        private readonly StateMachine stateMachine;
        private readonly IState nextState;

        private int redrawsRemaining;
        private bool drawComplete;
        private bool handLocked;

        public DrawPhaseState(StateMachine stateMachine, IState nextState)
        {
            this.stateMachine = stateMachine;
            this.nextState = nextState;
        }

        public void Enter()
        {
            Debug.Log("[DrawPhaseState] Enter - Drawing cards");

            drawComplete = false;
            handLocked = false;
            redrawsRemaining = Constants.REDRAWS_PER_TURN;

            // Draw cards up to HAND_SIZE
            // TODO: DeckManager.Instance.DrawCards(Constants.HAND_SIZE);

            // Animate cards being drawn into the hand
            // TODO: UIManager.Instance.AnimateCardDraw(Constants.HAND_SIZE, OnDrawAnimationComplete);

            // For now, mark draw as complete immediately
            drawComplete = true;

            Debug.Log($"[DrawPhaseState] Drew {Constants.HAND_SIZE} cards, {redrawsRemaining} redraw(s) available");
        }

        public void Execute()
        {
            if (!drawComplete)
                return;

            // Allow 1 redraw - the player can tap a card to swap it
            // Once redrawsRemaining hits 0 or the player confirms, lock the hand
            // TODO: Listen for redraw input via UIManager

            // For now, auto-lock the hand after drawing
            if (!handLocked)
            {
                LockHand();
            }

            if (handLocked)
            {
                stateMachine.ChangeState(nextState);
            }
        }

        public void Exit()
        {
            // Hand is locked; no more changes allowed until the play phase
            Debug.Log("[DrawPhaseState] Exit - Hand locked");
        }

        /// <summary>
        /// Called when the player chooses to redraw a specific card.
        /// </summary>
        public void RequestRedraw(int cardIndex)
        {
            if (redrawsRemaining <= 0)
            {
                Debug.Log("[DrawPhaseState] No redraws remaining");
                return;
            }

            redrawsRemaining--;

            // TODO: DeckManager.Instance.RedrawCard(cardIndex);
            // TODO: UIManager.Instance.AnimateRedraw(cardIndex);

            Debug.Log($"[DrawPhaseState] Redrew card at index {cardIndex}, redraws remaining: {redrawsRemaining}");
        }

        /// <summary>
        /// Called when the player confirms their hand or after the redraw is used.
        /// </summary>
        public void ConfirmHand()
        {
            LockHand();
        }

        private void LockHand()
        {
            handLocked = true;
            Debug.Log("[DrawPhaseState] Hand confirmed and locked");
        }

        private void OnDrawAnimationComplete()
        {
            drawComplete = true;
        }
    }
}
