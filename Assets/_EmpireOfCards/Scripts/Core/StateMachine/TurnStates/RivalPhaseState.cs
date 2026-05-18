using UnityEngine;

namespace EmpireOfCards.Core.StateMachine.TurnStates
{
    /// <summary>
    /// Turn phase where the rival AI takes its turn.
    /// Shows a popup summarizing the rival's actions and updates territory.
    /// </summary>
    public class RivalPhaseState : IState
    {
        private readonly StateMachine stateMachine;
        private readonly IState nextState;

        private bool rivalActionsComplete;
        private bool popupClosed;

        public RivalPhaseState(StateMachine stateMachine, IState nextState)
        {
            this.stateMachine = stateMachine;
            this.nextState = nextState;
        }

        public void Enter()
        {
            Debug.Log("[RivalPhaseState] Enter - Rival's turn");

            rivalActionsComplete = false;
            popupClosed = false;

            // Execute the rival AI's turn logic
            // TODO: RivalAI.Instance.ExecuteTurn(OnRivalActionsComplete);

            // For now, simulate immediate completion
            rivalActionsComplete = true;

            if (rivalActionsComplete)
            {
                // Show a popup summarizing what the rival did
                // TODO: UIManager.Instance.ShowRivalActionsPopup(
                //     RivalAI.Instance.GetLastTurnSummary(),
                //     OnPopupClosed
                // );

                // For now, auto-close
                popupClosed = true;
            }
        }

        public void Execute()
        {
            if (rivalActionsComplete && popupClosed)
            {
                stateMachine.ChangeState(nextState);
            }
        }

        public void Exit()
        {
            // Update the territory display to reflect rival changes
            GameManager gm = GameManager.Instance;
            if (gm != null)
            {
                // TODO: UIManager.Instance.UpdateTerritoryDisplay(
                //     gm.PlayerTerritories, gm.RivalTerritories
                // );

                Debug.Log($"[RivalPhaseState] Exit - Territories: Player {gm.PlayerTerritories} | Rival {gm.RivalTerritories}");
            }
        }

        /// <summary>
        /// Callback invoked when the rival AI finishes its actions.
        /// </summary>
        private void OnRivalActionsComplete()
        {
            rivalActionsComplete = true;

            // Show the summary popup after rival finishes
            // TODO: UIManager.Instance.ShowRivalActionsPopup(
            //     RivalAI.Instance.GetLastTurnSummary(),
            //     OnPopupClosed
            // );
        }

        /// <summary>
        /// Callback invoked when the player closes the rival actions popup.
        /// </summary>
        public void OnPopupClosed()
        {
            popupClosed = true;
        }
    }
}
