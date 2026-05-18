using System;
using UnityEngine;

namespace EmpireOfCards.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.MainMenu;
        [SerializeField] private int currentTurn;
        [SerializeField] private int maxTurns = 20;

        [Header("Player Resources")]
        [SerializeField] private int playerMoney = 500;
        [SerializeField] private int playerActions = 3;
        [SerializeField] private int maxActions = 3;
        [SerializeField] private int businessSlots = 3;
        [SerializeField] private int maxBusinessSlots = 5;
        [SerializeField] private float fbiRisk;

        [Header("Territory & Customers")]
        [SerializeField] private int playerCustomers;
        [SerializeField] private int rivalCustomers;
        [SerializeField] private int playerTerritories;
        [SerializeField] private int rivalTerritories;

        [Header("Manager References")]
        [SerializeField] private TurnManager turnManager;
        // Future manager references - assigned via Inspector once created
        // [SerializeField] private EconomyManager economyManager;
        // [SerializeField] private DeckManager deckManager;
        // [SerializeField] private ComboSystem comboSystem;
        // [SerializeField] private RivalAI rivalAI;
        // [SerializeField] private ShopManager shopManager;
        // [SerializeField] private UIManager uiManager;
        // [SerializeField] private AudioManager audioManager;
        // [SerializeField] private VFXManager vfxManager;
        // [SerializeField] private SaveManager saveManager;

        // --- Events ---
        public event Action<GameState> OnGameStateChanged;
        public event Action<int> OnTurnChanged;
        public event Action<int> OnMoneyChanged;
        public event Action<int> OnActionsChanged;
        public event Action<int, int> OnTerritoryChanged;
        public event Action<float> OnFBIRiskChanged;

        // --- Properties ---
        public GameState CurrentState => currentState;
        public int CurrentTurn => currentTurn;
        public int MaxTurns => maxTurns;
        public int PlayerMoney => playerMoney;
        public int PlayerActions => playerActions;
        public int MaxActions => maxActions;
        public int BusinessSlots => businessSlots;
        public int MaxBusinessSlots => maxBusinessSlots;
        public float FBIRisk => fbiRisk;
        public int PlayerCustomers => playerCustomers;
        public int RivalCustomers => rivalCustomers;
        public int PlayerTerritories => playerTerritories;
        public int RivalTerritories => rivalTerritories;
        public TurnManager TurnManager => turnManager;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Initializes a fresh run with default values.
        /// </summary>
        public void StartNewRun()
        {
            currentTurn = 1;
            playerMoney = Constants.STARTING_MONEY;
            playerActions = Constants.STARTING_ACTIONS;
            maxActions = Constants.STARTING_ACTIONS;
            businessSlots = Constants.STARTING_SLOTS;
            fbiRisk = 0f;
            playerCustomers = 0;
            rivalCustomers = 0;
            playerTerritories = 0;
            rivalTerritories = 0;

            SetGameState(GameState.InGame);

            OnMoneyChanged?.Invoke(playerMoney);
            OnActionsChanged?.Invoke(playerActions);
            OnTurnChanged?.Invoke(currentTurn);
            OnTerritoryChanged?.Invoke(playerTerritories, rivalTerritories);
            OnFBIRiskChanged?.Invoke(fbiRisk);

            if (turnManager != null)
            {
                turnManager.StartTurn();
            }
        }

        /// <summary>
        /// Ends the current run.
        /// </summary>
        public void EndRun(bool won)
        {
            SetGameState(won ? GameState.ScoreScreen : GameState.GameOver);
        }

        /// <summary>
        /// Returns true if the player has won.
        /// </summary>
        public bool CheckWinCondition()
        {
            if (playerTerritories >= Constants.WIN_TERRITORIES)
            {
                EndRun(true);
                return true;
            }

            if (currentTurn >= maxTurns && playerTerritories > rivalTerritories)
            {
                EndRun(true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the player has lost.
        /// </summary>
        public bool CheckLoseCondition()
        {
            if (rivalTerritories >= Constants.LOSE_TERRITORIES)
            {
                EndRun(false);
                return true;
            }

            if (playerMoney <= 0 && playerActions <= 0)
            {
                EndRun(false);
                return true;
            }

            if (currentTurn >= maxTurns && rivalTerritories >= playerTerritories)
            {
                EndRun(false);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to spend the given amount. Returns true on success.
        /// </summary>
        public bool SpendMoney(int amount)
        {
            if (amount < 0 || playerMoney < amount)
                return false;

            playerMoney -= amount;
            OnMoneyChanged?.Invoke(playerMoney);
            return true;
        }

        /// <summary>
        /// Adds money to the player's total.
        /// </summary>
        public void GainMoney(int amount)
        {
            if (amount <= 0)
                return;

            playerMoney += amount;
            OnMoneyChanged?.Invoke(playerMoney);
        }

        /// <summary>
        /// Consumes one action point. Returns true if the player had actions remaining.
        /// </summary>
        public bool UseAction()
        {
            if (playerActions <= 0)
                return false;

            playerActions--;
            OnActionsChanged?.Invoke(playerActions);
            return true;
        }

        /// <summary>
        /// Resets action points to the current max at the start of a turn.
        /// </summary>
        public void ResetActions()
        {
            playerActions = maxActions;
            OnActionsChanged?.Invoke(playerActions);
        }

        /// <summary>
        /// Advances the turn counter by one.
        /// </summary>
        public void AdvanceTurn()
        {
            currentTurn++;
            OnTurnChanged?.Invoke(currentTurn);
        }

        /// <summary>
        /// Updates territory counts and fires the event.
        /// </summary>
        public void SetTerritories(int player, int rival)
        {
            playerTerritories = Mathf.Clamp(player, 0, Constants.TERRITORY_COUNT);
            rivalTerritories = Mathf.Clamp(rival, 0, Constants.TERRITORY_COUNT);
            OnTerritoryChanged?.Invoke(playerTerritories, rivalTerritories);
        }

        /// <summary>
        /// Adjusts FBI risk and fires the event.
        /// </summary>
        public void SetFBIRisk(float risk)
        {
            fbiRisk = Mathf.Clamp01(risk);
            OnFBIRiskChanged?.Invoke(fbiRisk);
        }

        private void SetGameState(GameState newState)
        {
            if (currentState == newState)
                return;

            currentState = newState;
            OnGameStateChanged?.Invoke(currentState);
        }
    }
}
