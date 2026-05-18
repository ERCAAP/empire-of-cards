using UnityEngine;

namespace EmpireOfCards.Core.StateMachine.GameStates
{
    /// <summary>
    /// State responsible for setting up a new game run.
    /// Initializes the deck, starting money, board, and rival before transitioning to InGameState.
    /// </summary>
    public class GameSetupState : IState
    {
        private readonly StateMachine stateMachine;
        private readonly InGameState inGameState;
        private bool setupComplete;

        public GameSetupState(StateMachine stateMachine, InGameState inGameState)
        {
            this.stateMachine = stateMachine;
            this.inGameState = inGameState;
        }

        public void Enter()
        {
            Debug.Log("[GameSetupState] Enter - Initializing new game");
            setupComplete = false;

            // Initialize player deck
            // TODO: DeckManager.Instance.InitializeDeck();

            // Set starting money via GameManager
            GameManager gm = GameManager.Instance;
            if (gm != null)
            {
                gm.StartNewRun();
            }

            // Create the game board
            // TODO: BoardManager.Instance.CreateBoard();

            // Set up the rival
            // TODO: RivalAI.Instance.Initialize();

            setupComplete = true;

            Debug.Log("[GameSetupState] Setup complete");
        }

        public void Execute()
        {
            // Transition to InGameState once setup is finished
            if (setupComplete)
            {
                stateMachine.ChangeState(inGameState);
            }
        }

        public void Exit()
        {
            // Nothing to clean up
        }
    }
}
