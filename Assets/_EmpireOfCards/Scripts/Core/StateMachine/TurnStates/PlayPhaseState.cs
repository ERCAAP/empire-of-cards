using UnityEngine;

namespace EmpireOfCards.Core.StateMachine.TurnStates
{
    /// <summary>
    /// Turn phase where the player plays cards from hand onto the board.
    /// The player has a limited number of actions and can end the phase early.
    /// </summary>
    public class PlayPhaseState : IState
    {
        private readonly StateMachine stateMachine;
        private readonly IState nextState;

        private bool phaseEnded;

        public PlayPhaseState(StateMachine stateMachine, IState nextState)
        {
            this.stateMachine = stateMachine;
            this.nextState = nextState;
        }

        public void Enter()
        {
            Debug.Log("[PlayPhaseState] Enter - Player's turn to act");

            phaseEnded = false;

            // Enable card drag-and-drop from hand to board
            // TODO: UIManager.Instance.EnableCardDrag(true);

            // Enable the action bar (play, sell, use ability buttons)
            // TODO: UIManager.Instance.EnableActionBar(true);

            // Highlight valid placement slots on the board
            // TODO: BoardManager.Instance.ShowValidSlots();

            GameManager gm = GameManager.Instance;
            if (gm != null)
            {
                Debug.Log($"[PlayPhaseState] Actions available: {gm.PlayerActions}/{gm.MaxActions}");
            }
        }

        public void Execute()
        {
            if (phaseEnded)
            {
                stateMachine.ChangeState(nextState);
                return;
            }

            // Check if all actions have been used
            GameManager gm = GameManager.Instance;
            if (gm != null && gm.PlayerActions <= 0)
            {
                Debug.Log("[PlayPhaseState] All actions used, ending play phase");
                EndPhase();
            }
        }

        public void Exit()
        {
            // Disable card drag-and-drop
            // TODO: UIManager.Instance.EnableCardDrag(false);

            // Disable the action bar
            // TODO: UIManager.Instance.EnableActionBar(false);

            // Hide valid slot highlights
            // TODO: BoardManager.Instance.HideValidSlots();

            Debug.Log("[PlayPhaseState] Exit - Play phase complete");
        }

        /// <summary>
        /// Called when the player presses the End Turn button during the play phase.
        /// Also called automatically when all actions are spent.
        /// </summary>
        public void EndPhase()
        {
            if (phaseEnded)
                return;

            phaseEnded = true;

            // Notify the TurnManager that the player ended their play phase
            GameManager gm = GameManager.Instance;
            if (gm != null && gm.TurnManager != null)
            {
                gm.TurnManager.EndPlayPhase();
            }

            Debug.Log("[PlayPhaseState] Player ended play phase");
        }
    }
}
