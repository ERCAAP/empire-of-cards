using System;
using UnityEngine;
using EmpireOfCards.Core.StateMachine;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;
using EmpireOfCards.UI;
using EmpireOfCards.Audio;
using EmpireOfCards.VFX;
using EmpireOfCards.Save;

namespace EmpireOfCards.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // === Game State Machine ===
        private StateMachine.StateMachine _gameStateMachine;

        [Header("=== Oyun Durumu ===")]
        [SerializeField] private GameState currentGameState = GameState.MainMenu;
        [SerializeField] private int currentTurn;
        [SerializeField] private bool gameIsRunning;

        [Header("=== Oyuncu Kaynakları ===")]
        [SerializeField] private int playerMoney;
        [SerializeField] private int playerActions;
        [SerializeField] private int maxActions;
        [SerializeField] private int businessSlotCount;
        [SerializeField] private float fbiRisk;

        [Header("=== Bölge & Müşteri ===")]
        [SerializeField] private int playerCustomers;
        [SerializeField] private int rivalCustomers;
        [SerializeField] private int playerTerritories;
        [SerializeField] private int rivalTerritories;

        [Header("=== Balans Verisi ===")]
        [SerializeField] private GameBalanceData balanceData;
        [SerializeField] private DeckPresetData startingDeck;

        [Header("=== Manager Referansları ===")]
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private EconomyManager economyManager;
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private ComboSystem comboSystem;
        [SerializeField] private TerritoryManager territoryManager;
        [SerializeField] private FBISystem fbiSystem;
        [SerializeField] private RivalAI rivalAI;
        [SerializeField] private ShopManager shopManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private VFXManager vfxManager;
        [SerializeField] private SaveManager saveManager;

        // === Properties ===
        public GameState CurrentGameState => currentGameState;
        public int CurrentTurn => currentTurn;
        public int MaxTurns => balanceData != null ? balanceData.maxTurns : Constants.MAX_TURNS;
        public int PlayerMoney => playerMoney;
        public int PlayerActions => playerActions;
        public int MaxActions => maxActions;
        public int BusinessSlotCount => businessSlotCount;
        public float FBIRisk => fbiRisk;
        public int PlayerCustomers => playerCustomers;
        public int RivalCustomers => rivalCustomers;
        public int PlayerTerritories => playerTerritories;
        public int RivalTerritories => rivalTerritories;
        public bool GameIsRunning => gameIsRunning;

        public GameBalanceData BalanceData => balanceData;
        public DeckPresetData StartingDeck => startingDeck;
        public TurnManager TurnManager => turnManager;
        public EconomyManager EconomyManager => economyManager;
        public DeckManager DeckManager => deckManager;
        public BoardManager BoardManager => boardManager;
        public ComboSystem ComboSystem => comboSystem;
        public TerritoryManager TerritoryManager => territoryManager;
        public FBISystem FBISystem => fbiSystem;
        public RivalAI RivalAI => rivalAI;
        public ShopManager ShopManager => shopManager;
        public UIManager UIManager => uiManager;
        public AudioManager AudioManager => audioManager;
        public VFXManager VFXManager => vfxManager;
        public SaveManager SaveManager => saveManager;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _gameStateMachine = new StateMachine.StateMachine();
        }

        private void Start()
        {
            SetGameState(GameState.MainMenu);
        }

        // POLLING: Update calls state machine tick every frame
        private void Update()
        {
            _gameStateMachine?.Tick();
        }

        // === Game Flow ===

        /// <summary>
        /// Initializes a fresh run with balance data (or Constants fallback).
        /// Resets all state, initializes subsystems, fires initial events, then starts turn 1.
        /// </summary>
        public void StartNewRun()
        {
            int startMoney = balanceData != null ? balanceData.startingMoney : Constants.STARTING_MONEY;
            int startActions = balanceData != null ? balanceData.startingActions : Constants.STARTING_ACTIONS;
            int startSlots = balanceData != null ? balanceData.startingBusinessSlots : Constants.STARTING_SLOTS;

            currentTurn = 0;
            playerMoney = startMoney;
            playerActions = startActions;
            maxActions = startActions;
            businessSlotCount = startSlots;
            fbiRisk = 0f;
            playerCustomers = 0;
            rivalCustomers = 0;
            playerTerritories = 0;
            rivalTerritories = 0;
            gameIsRunning = true;

            // Initialize subsystems
            if (deckManager != null && startingDeck != null)
                deckManager.InitializeDeck(startingDeck);
            if (boardManager != null)
                boardManager.SetMaxSlots(startSlots);
            if (rivalAI != null)
                rivalAI.Initialize();
            if (shopManager != null)
                shopManager.RefreshShop();

            // Fire initial events so UI updates
            EventBus.MoneyUpdated(playerMoney);
            EventBus.TerritoryUpdated(0, 0);
            EventBus.FBIRiskUpdated(0f);

            SetGameState(GameState.Playing);
            StartNextTurn();
        }

        /// <summary>
        /// Advances the turn counter and kicks off the TurnManager phase sequence.
        /// </summary>
        public void StartNextTurn()
        {
            currentTurn++;
            EventBus.TurnStarted(currentTurn);

            if (turnManager != null)
                turnManager.BeginTurn(currentTurn);
        }

        /// <summary>
        /// Called by TurnManager when all 5 phases are done.
        /// Checks win/lose, max turns, then starts the next turn.
        /// </summary>
        public void EndCurrentTurn()
        {
            EventBus.TurnEnded(currentTurn);

            // Check win/lose after turn ends
            if (CheckWinCondition()) return;
            if (CheckLoseCondition()) return;

            // GDD Section 10: Turn 20 = final turn
            if (currentTurn >= MaxTurns)
            {
                // Time's up - player needed 6 territories but didn't get them
                EndRun(playerTerritories >= rivalTerritories);
                return;
            }

            StartNextTurn();
        }

        /// <summary>
        /// Ends the run. Fires GameEnded event and transitions to score/game-over screen.
        /// </summary>
        public void EndRun(bool won)
        {
            gameIsRunning = false;
            EventBus.GameEnded(won);
            SetGameState(won ? GameState.ScoreScreen : GameState.GameOver);
        }

        // === Win/Lose Checks (GDD Section 6.3) ===

        /// <summary>
        /// WIN: Player has 6+ territories (instant end, any turn).
        /// </summary>
        public bool CheckWinCondition()
        {
            int winReq = balanceData != null ? balanceData.winTerritories : Constants.WIN_TERRITORIES;
            if (playerTerritories >= winReq)
            {
                EndRun(true);
                return true;
            }
            return false;
        }

        /// <summary>
        /// LOSE: Rival has 7+ territories OR player money &lt;= 0.
        /// (Turn 20 timeout is handled in EndCurrentTurn, not here.)
        /// </summary>
        public bool CheckLoseCondition()
        {
            int loseReq = balanceData != null ? balanceData.loseTerritories : Constants.LOSE_TERRITORIES;

            // Rival dominates
            if (rivalTerritories >= loseReq)
            {
                EndRun(false);
                return true;
            }

            // Bankruptcy (GDD: Para 0'a düşerse = IFLAS)
            if (playerMoney <= 0)
            {
                EndRun(false);
                return true;
            }

            return false;
        }

        // === Resource Management ===

        /// <summary>
        /// Attempts to spend the given amount. Returns true on success.
        /// </summary>
        public bool SpendMoney(int amount)
        {
            if (amount < 0 || playerMoney < amount) return false;
            playerMoney -= amount;
            EventBus.MoneyUpdated(playerMoney);
            EventBus.MoneySpent(amount);
            return true;
        }

        /// <summary>
        /// Adds money to the player's total.
        /// </summary>
        public void GainMoney(int amount)
        {
            if (amount <= 0) return;
            playerMoney += amount;
            EventBus.MoneyUpdated(playerMoney);
            EventBus.IncomeReceived(amount);
        }

        /// <summary>
        /// Net change that can go negative (salary payments, penalties, etc).
        /// </summary>
        public void AdjustMoney(int netAmount)
        {
            playerMoney += netAmount;
            EventBus.MoneyUpdated(playerMoney);
        }

        /// <summary>
        /// Consumes one action point. Returns true if the player had actions remaining.
        /// </summary>
        public bool UseAction()
        {
            if (playerActions <= 0) return false;
            playerActions--;
            return true;
        }

        /// <summary>
        /// Resets action points to maxActions at the start of a turn.
        /// </summary>
        public void ResetActions()
        {
            playerActions = maxActions;
        }

        /// <summary>
        /// Adds extra action capacity (e.g. from Yapay Zeka Asistani upgrade).
        /// Capped at balance max.
        /// </summary>
        public void AddExtraAction(int count = 1)
        {
            int cap = balanceData != null ? balanceData.maxActions : Constants.MAX_ACTIONS;
            maxActions = Mathf.Min(maxActions + count, cap);
            playerActions = Mathf.Min(playerActions + count, maxActions);
        }

        /// <summary>
        /// Adds one business slot, capped at balance max.
        /// </summary>
        public void AddBusinessSlot()
        {
            int max = balanceData != null ? balanceData.maxBusinessSlots : Constants.MAX_SLOTS;
            businessSlotCount = Mathf.Min(businessSlotCount + 1, max);
            if (boardManager != null)
                boardManager.SetMaxSlots(businessSlotCount);
        }

        // === Territory & Customer Updates ===

        public void SetPlayerCustomers(int count)
        {
            playerCustomers = Mathf.Max(0, count);
        }

        public void SetRivalCustomers(int count)
        {
            rivalCustomers = Mathf.Max(0, count);
        }

        /// <summary>
        /// Sets territory counts (clamped to total) and fires EventBus update.
        /// </summary>
        public void SetTerritories(int player, int rival)
        {
            int total = balanceData != null ? balanceData.totalTerritories : Constants.TERRITORY_COUNT;
            playerTerritories = Mathf.Clamp(player, 0, total);
            rivalTerritories = Mathf.Clamp(rival, 0, total);
            EventBus.TerritoryUpdated(playerTerritories, rivalTerritories);
        }

        // === FBI Risk ===

        /// <summary>
        /// Sets FBI risk (0-1) and fires EventBus update.
        /// </summary>
        public void SetFBIRisk(float risk)
        {
            fbiRisk = Mathf.Clamp(risk, 0f, 1f);
            EventBus.FBIRiskUpdated(fbiRisk);
        }

        // === Game State Machine ===

        /// <summary>
        /// Transitions game state and fires EventBus notification.
        /// </summary>
        public void SetGameState(GameState newState)
        {
            if (currentGameState == newState) return;
            currentGameState = newState;
            EventBus.GameStateChanged(newState);
        }

        public StateMachine.StateMachine GetGameStateMachine() => _gameStateMachine;

        private void OnDestroy()
        {
            if (Instance == this)
            {
                EventBus.ClearAll();
                Instance = null;
            }
        }
    }
}
