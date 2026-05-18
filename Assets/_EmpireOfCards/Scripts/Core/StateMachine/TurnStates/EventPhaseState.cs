using UnityEngine;

namespace EmpireOfCards.Core.StateMachine.TurnStates
{
    /// <summary>
    /// Turn phase that checks for and resolves random event cards.
    /// Events trigger every 3 turns and apply temporary modifiers.
    /// </summary>
    public class EventPhaseState : IState
    {
        private readonly StateMachine stateMachine;
        private readonly IState nextState;

        private bool eventTriggered;
        private bool popupClosed;

        public EventPhaseState(StateMachine stateMachine, IState nextState)
        {
            this.stateMachine = stateMachine;
            this.nextState = nextState;
        }

        public void Enter()
        {
            Debug.Log("[EventPhaseState] Enter");

            eventTriggered = false;
            popupClosed = false;

            GameManager gm = GameManager.Instance;
            if (gm == null)
            {
                popupClosed = true;
                return;
            }

            // Events trigger every 3 turns
            if (gm.CurrentTurn > 0 && gm.CurrentTurn % 3 == 0)
            {
                eventTriggered = true;

                // Draw an event card from the event deck
                // TODO: CardData eventCard = DeckManager.Instance.DrawEventCard();

                // Show event popup to the player
                // TODO: UIManager.Instance.ShowEventPopup(eventCard, OnPopupClosed);

                Debug.Log($"[EventPhaseState] Event triggered on turn {gm.CurrentTurn}");
            }
            else
            {
                // No event this turn, skip ahead
                popupClosed = true;
                Debug.Log("[EventPhaseState] No event this turn");
            }
        }

        public void Execute()
        {
            // Wait for the event popup to be closed by the player
            if (popupClosed)
            {
                stateMachine.ChangeState(nextState);
            }
        }

        public void Exit()
        {
            if (eventTriggered)
            {
                // Apply event effects to the game state
                // TODO: EventManager.Instance.ApplyCurrentEventEffects();

                Debug.Log("[EventPhaseState] Event effects applied");
            }
        }

        /// <summary>
        /// Callback invoked when the player closes the event popup.
        /// </summary>
        public void OnPopupClosed()
        {
            popupClosed = true;
        }
    }
}
