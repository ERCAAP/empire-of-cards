using System;
using UnityEngine;
using EmpireOfCards.Core.StateMachine;
using EmpireOfCards.Core.GameStates;
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

        [Header("=== Game State ===")]
        [SerializeField] private GameState currentGameState = GameState.MainMenu;
        [SerializeField] private int currentTurn;
        [SerializeField] private bool gameIsRunning;

        [Header("=== Player Resources (extracted) ===")]
        [SerializeField] private PlayerResources resources = new PlayerResources();

        [Header("=== Territory & Customers ===")]
        [SerializeField] private int playerCustomers;
        [SerializeField] private int rivalCustomers;
        [SerializeField] private int playerTerritories;
        [SerializeField] private int rivalTerritories;
        [SerializeField] private float fbiRisk;

        [Header("=== Balance Data ===")]
        [SerializeField] private GameBalanceData balanceData;
        [SerializeField] private DeckPresetData startingDeck;

        [Header("=== Manager References ===")]
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
        [SerializeField] private MetaProgressionSystem metaProgressionSystem;
        [SerializeField] private CompanyTierSystem companyTierSystem;

        [Header("=== First Venture ===")]
        [SerializeField] private VentureData selectedVenture;

        // === Extracted Sub-Objects (prefer these over backward-compat properties) ===
        public PlayerResources Resources => resources;

        // === Core Properties ===
        public GameState CurrentGameState => currentGameState;
        public int CurrentTurn => currentTurn;
        public int MaxTurns => balanceData != null ? balanceData.maxTurns : Constants.MAX_TURNS;
        public bool GameIsRunning => gameIsRunning;
        public float FBIRisk => fbiRisk;
        public int PlayerCustomers => playerCustomers;
        public int RivalCustomers => rivalCustomers;
        public int PlayerTerritories => playerTerritories;
        public int RivalTerritories => rivalTerritories;

        // --- Backward-compat: callers can migrate to Resources.X at their own pace ---
        public int PlayerMoney => resources.Money;
        public int PlayerActions => resources.Actions;
        public int MaxActions => resources.MaxActions;
        public int BusinessSlotCount => resources.BusinessSlots;

        // === Data & Manager Accessors ===
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
        public MetaProgressionSystem MetaProgressionSystem => metaProgressionSystem;
        public CompanyTierSystem CompanyTierSystem => companyTierSystem;

        /// <summary>
        /// Assigns the MetaProgressionSystem. Called by WiringService after bootstrap.
        /// </summary>
        public void SetMetaProgressionSystem(MetaProgressionSystem mps)
        {
            this.metaProgressionSystem = mps;
        }

        public void SetCompanyTierSystem(CompanyTierSystem cts)
        {
            this.companyTierSystem = cts;
        }

        public void SetSelectedVenture(VentureData venture)
        {
            this.selectedVenture = venture;
        }

        /// <summary>
        /// Assigns all manager dependencies without reflection.
        /// Called by WiringService during bootstrap.
        /// </summary>
        public void Init(GameBalanceData balance, DeckPresetData deck, TurnManager tm,
            EconomyManager em, DeckManager dm, BoardManager bm, ComboSystem cs,
            TerritoryManager ter, FBISystem fbi, RivalAI ai, ShopManager shop,
            UIManager ui, AudioManager audio, VFXManager vfx, SaveManager save)
        {
            this.balanceData = balance;
            this.startingDeck = deck;
            this.turnManager = tm;
            this.economyManager = em;
            this.deckManager = dm;
            this.boardManager = bm;
            this.comboSystem = cs;
            this.territoryManager = ter;
            this.fbiSystem = fbi;
            this.rivalAI = ai;
            this.shopManager = shop;
            this.uiManager = ui;
            this.audioManager = audio;
            this.vfxManager = vfx;
            this.saveManager = save;
        }

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

        private void Update()
        {
            _gameStateMachine?.Tick();
        }

        // === Game Flow ===

        public void StartNewRun()
        {
            currentTurn = 0;
            gameIsRunning = true;

            // Apply meta-progression ascension modifiers before resource reset
            if (metaProgressionSystem != null && saveManager != null)
            {
                var saveData = saveManager.Load();
                metaProgressionSystem.ApplyAscension(saveData.currentAscension, balanceData);
            }

            // Reset extracted resource state
            resources.Reset(balanceData);

            // Reset territory/customer/fbi state
            fbiRisk = 0f;
            playerCustomers = 0;
            rivalCustomers = 0;
            playerTerritories = 0;
            rivalTerritories = 0;

            // Initialize subsystems
            if (deckManager != null && startingDeck != null)
                deckManager.InitializeDeck(startingDeck);

            // Apply First Venture (GDD Section 1.5)
            if (selectedVenture != null)
            {
                // Add bonus money
                if (selectedVenture.bonusMoney > 0)
                    resources.GainMoney(selectedVenture.bonusMoney);

                // Place starting business on board
                if (selectedVenture.startingBusiness != null && boardManager != null)
                    boardManager.PlaceBusiness(selectedVenture.startingBusiness, 0);

                // Add bonus card to deck
                if (selectedVenture.bonusDeckCard != null && deckManager != null)
                    deckManager.AddCardToDeck(selectedVenture.bonusDeckCard);
            }

            if (boardManager != null)
                boardManager.SetMaxSlots(resources.BusinessSlots);
            if (rivalAI != null)
            {
                if (selectedVenture != null)
                    rivalAI.Initialize(selectedVenture.ventureType);
                else
                    rivalAI.Initialize();
            }
            if (shopManager != null)
            {
                if (selectedVenture != null)
                    shopManager.SetVentureBias(selectedVenture.ventureType);
                shopManager.RefreshShop();
            }
            if (companyTierSystem != null)
                companyTierSystem.Reset();

            // Fire initial events so UI updates
            EventBus.MoneyUpdated(resources.Money);
            EventBus.TerritoryUpdated(0, 0);
            EventBus.FBIRiskUpdated(0f);

            SetGameState(GameState.Playing);
            _gameStateMachine.Initialize(new InGameState());

            StartNextTurn();
        }

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

            int winReq = balanceData != null ? balanceData.winTerritories : Constants.WIN_TERRITORIES;
            int loseReq = balanceData != null ? balanceData.loseTerritories : Constants.LOSE_TERRITORIES;

            if (WinLoseChecker.CheckWin(playerTerritories, winReq))
            {
                EndRun(true);
                return;
            }
            if (WinLoseChecker.CheckLose(rivalTerritories, loseReq, resources.Money))
            {
                EndRun(false);
                return;
            }

            // GDD Section 10: Turn 20 = final turn
            if (currentTurn >= MaxTurns)
            {
                EndRun(playerTerritories >= rivalTerritories);
                return;
            }

            StartNextTurn();
        }

        public void EndRun(bool won)
        {
            gameIsRunning = false;
            EventBus.GameEnded(won);
            _gameStateMachine?.ChangeState(new GameOverState());
            SetGameState(won ? GameState.ScoreScreen : GameState.GameOver);
        }

        // === Delegating Resource Methods (backward-compat) ---
        // Callers can migrate to GameManager.Instance.Resources.X directly.

        public bool SpendMoney(int amount) => resources.SpendMoney(amount);
        public void GainMoney(int amount) => resources.GainMoney(amount);
        public void AdjustMoney(int netAmount) => resources.AdjustMoney(netAmount);
        public bool UseAction() => resources.UseAction();
        public void ResetActions() => resources.ResetActions();
        public void AddExtraAction(int count = 1) => resources.AddExtraAction(count, balanceData);

        public void AddBusinessSlot()
        {
            resources.AddBusinessSlot(balanceData);
            if (boardManager != null)
                boardManager.SetMaxSlots(resources.BusinessSlots);
            EventBus.BusinessSlotsChanged(resources.BusinessSlots);
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

        public void SetTerritories(int player, int rival)
        {
            int total = balanceData != null ? balanceData.totalTerritories : Constants.TERRITORY_COUNT;
            playerTerritories = Mathf.Clamp(player, 0, total);
            rivalTerritories = Mathf.Clamp(rival, 0, total);
            EventBus.TerritoryUpdated(playerTerritories, rivalTerritories);
        }

        // === FBI Risk ===

        public void SetFBIRisk(float risk)
        {
            fbiRisk = Mathf.Clamp(risk, 0f, 1f);
            EventBus.FBIRiskUpdated(fbiRisk);
        }

        // === Game State Machine ===

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
