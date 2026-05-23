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
using EmpireOfCards.Gameplay.Staff;
using EmpireOfCards.Gameplay.Venture;
using System.Collections.Generic;

namespace EmpireOfCards.Core
{
    public partial class GameManager : MonoBehaviour
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

        [Header("=== Customer Share & Market Blocks ===")]
        [SerializeField] private int playerCustomers;
        [SerializeField] private int rivalCustomers;
        [SerializeField] private int playerMarketBlocks;
        [SerializeField] private int rivalMarketBlocks;
        [SerializeField] private float fbiRisk;

        [Header("=== Turn Placement Budget ===")]
        [SerializeField] private int persistentBuildsPlayedThisTurn;

        [Header("=== Balance Data ===")]
        [SerializeField] private GameBalanceData balanceData;

        [Header("=== Manager References ===")]
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private EconomyManager economyManager;
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private QuestionManager questionManager;
        [SerializeField] private DecisionHistoryManager decisionHistoryManager;
        [SerializeField] private CustomerFlowManager customerFlowManager;
        [SerializeField] private MarketShareVisualizer marketShareVisualizer;
        [SerializeField] private RivalAI rivalAI;
        [SerializeField] private ShopManager shopManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private VFXManager vfxManager;
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private SlotManager slotManager;
        [SerializeField] private StaffStateSystem staffStateSystem;
        [SerializeField] private ChainReactionSystem chainReactionSystem;

        [Header("=== First Venture ===")]
        [SerializeField] private VentureData selectedVenture;
        [SerializeField] private VentureBoardProfile activeBoardProfile;
        [SerializeField] private VentureDeckProfile activeDeckProfile;
        [SerializeField] private VentureEconomyProfile activeEconomyProfile;
        [SerializeField] private string runDisplayName;
        [SerializeField] private string runCategoryId;
        [SerializeField] private string runCategoryLabel;
        [SerializeField] private string currentRunSlotId;

        private Dictionary<string, CardData> _cardLookup;
        private IVentureRuntime activeVentureRuntime;

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
        public int PlayerMarketBlocks => playerMarketBlocks;
        public int RivalMarketBlocks => rivalMarketBlocks;
        public int PersistentBuildsPlayedThisTurn => persistentBuildsPlayedThisTurn;
        public bool CanPlacePersistentBuildThisTurn => persistentBuildsPlayedThisTurn < Constants.PERSISTENT_BUILDS_PER_TURN;

        // --- Backward-compat: callers can migrate to Resources.X at their own pace ---
        public int PlayerMoney => resources.Money;
        public int PlayerActions => resources.Actions;
        public int MaxActions => resources.MaxActions;
        public int BusinessSlotCount => resources.BusinessSlots;

        // === Data & Manager Accessors ===
        public GameBalanceData BalanceData => balanceData;
        public TurnManager TurnManager => turnManager;
        public EconomyManager EconomyManager => economyManager;
        public DeckManager DeckManager => deckManager;
        public BoardManager BoardManager => boardManager;
        public QuestionManager QuestionManager => questionManager;
        public DecisionHistoryManager DecisionHistoryManager => decisionHistoryManager;
        public CustomerFlowManager CustomerFlowManager => customerFlowManager;
        public MarketShareVisualizer MarketShareVisualizer => marketShareVisualizer;
        public RivalAI RivalAI => rivalAI;
        public ShopManager ShopManager => shopManager;
        public UIManager UIManager => uiManager;
        public AudioManager AudioManager => audioManager;
        public VFXManager VFXManager => vfxManager;
        public SaveManager SaveManager => saveManager;
        public SlotManager SlotManager => slotManager;
        public StaffStateSystem StaffStateSystem => staffStateSystem;
        public ChainReactionSystem ChainReactionSystem => chainReactionSystem;
        public VentureBoardProfile ActiveBoardProfile => activeBoardProfile;
        public VentureDeckProfile ActiveDeckProfile => activeDeckProfile;
        public VentureEconomyProfile ActiveEconomyProfile => activeEconomyProfile;
        public VentureData SelectedVenture => selectedVenture;
        public IVentureRuntime ActiveVentureRuntime => activeVentureRuntime;
        public IReadOnlyDictionary<string, CardData> CardLookup => _cardLookup;
        public string RunDisplayName => string.IsNullOrWhiteSpace(runDisplayName)
            ? (selectedVenture != null ? selectedVenture.ventureName : "New Venture")
            : runDisplayName;
        public string RunCategoryId => runCategoryId;
        public string RunCategoryLabel => runCategoryLabel;
        public string CurrentRunSlotId => currentRunSlotId;
        public TechCategoryProfile ActiveTechCategoryProfile => TechCategoryCatalog.Find(runCategoryId);

        public void SetSelectedVenture(VentureData venture)
        {
            this.selectedVenture = venture;
            activeBoardProfile = venture != null ? venture.boardProfile : null;
            activeDeckProfile = venture != null ? venture.deckProfile : null;
            activeEconomyProfile = venture != null ? venture.economyProfile : null;
            runDisplayName = venture != null ? venture.ventureName : "New Venture";
            runCategoryId = null;
            runCategoryLabel = null;
            RefreshActiveVentureRuntime();
        }

        public void SetRunDisplayName(string displayName)
        {
            runDisplayName = string.IsNullOrWhiteSpace(displayName)
                ? (selectedVenture != null ? selectedVenture.ventureName : "New Venture")
                : displayName.Trim();
        }

        public void SetRunCategory(string categoryId, string categoryLabel)
        {
            runCategoryId = string.IsNullOrWhiteSpace(categoryId) ? null : categoryId.Trim();
            runCategoryLabel = string.IsNullOrWhiteSpace(categoryLabel) ? null : categoryLabel.Trim();
        }

        public void SetCurrentRunSlotId(string slotId)
        {
            currentRunSlotId = string.IsNullOrWhiteSpace(slotId) ? null : slotId.Trim();
        }

        public void SetCardLookup(Dictionary<string, CardData> lookup)
        {
            _cardLookup = lookup;
            RefreshActiveVentureRuntime();
        }

        public void Init(GameBalanceData balance, TurnManager tm,
            EconomyManager em, DeckManager dm, BoardManager bm,
            QuestionManager qm, DecisionHistoryManager dhm, CustomerFlowManager cfm,
            MarketShareVisualizer marketVisualizer, RivalAI ai, ShopManager shop,
            UIManager ui, AudioManager audio, VFXManager vfx, SaveManager save,
            SlotManager sm, StaffStateSystem sss, ChainReactionSystem crs)
        {
            this.balanceData = balance;
            this.turnManager = tm;
            this.economyManager = em;
            this.deckManager = dm;
            this.boardManager = bm;
            this.questionManager = qm;
            this.decisionHistoryManager = dhm;
            this.customerFlowManager = cfm;
            this.marketShareVisualizer = marketVisualizer;
            this.rivalAI = ai;
            this.shopManager = shop;
            this.uiManager = ui;
            this.audioManager = audio;
            this.vfxManager = vfx;
            this.saveManager = save;
            this.slotManager = sm;
            this.staffStateSystem = sss;
            this.chainReactionSystem = crs;
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

        private void OnEnable()
        {
            EventBus.OnCardPlacedInSlot += HandleCardPlacedInSlot;
        }

        private void OnDisable()
        {
            EventBus.OnCardPlacedInSlot -= HandleCardPlacedInSlot;
        }

        private void Update()
        {
            _gameStateMachine?.Tick();
        }

        // === Game Flow ===

        public void StartNewRun(bool autoStartTurn = true)
        {
            if (string.IsNullOrWhiteSpace(currentRunSlotId) && saveManager != null)
                currentRunSlotId = saveManager.CreateRunSlotId();

            currentTurn = 0;
            gameIsRunning = true;
            ResetRunState();
            PrepareNewRunRuntime(autoStartTurn);
        }

        public void StartNextTurn()
        {
            currentTurn++;
            persistentBuildsPlayedThisTurn = 0;
            activeVentureRuntime?.OnTurnStarted(currentTurn);
            EventBus.TurnStarted(currentTurn);
            if (economyManager != null)
                economyManager.GenerateTurnBrief(currentTurn);
            if (questionManager != null)
            {
                BoardPressureType pressure = economyManager != null ? economyManager.CurrentPressure : BoardPressureType.None;
                questionManager.BeginTurnQuestions(currentTurn, selectedVenture != null ? selectedVenture.ventureType : VentureType.FastFood, pressure, economyManager != null ? economyManager.CurrentBrief : null);
            }

            if (turnManager != null)
                turnManager.BeginTurn(currentTurn);
        }

        public void SaveCheckpoint()
        {
            if (saveManager == null || selectedVenture == null || deckManager == null || slotManager == null)
                return;

            saveManager.SaveRun(currentRunSlotId, BuildRunSave());
        }

        public void RestoreRunCheckpoint(RunSaveData runData)
        {
            if (runData == null || !runData.HasData() || selectedVenture == null)
                return;

            currentRunSlotId = runData.slotId;
            StartNewRun(false);
            RestoreRunState(runData);
        }

        /// <summary>
        /// Called by TurnManager when all 5 phases are done.
        /// Checks win/lose, max turns, then starts the next turn.
        /// </summary>
        public void EndCurrentTurn()
        {
            EventBus.TurnEnded(currentTurn);
            if (!TryContinueRunAfterTurn())
                return;

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

        public bool SpendMoney(int amount)
        {
            bool success = resources.SpendMoney(amount);
            if (success)
                economyManager?.SyncCashFromResources(resources.Money);
            return success;
        }

        public void GainMoney(int amount)
        {
            resources.GainMoney(amount);
            economyManager?.SyncCashFromResources(resources.Money);
        }

        public void AdjustMoney(int netAmount)
        {
            resources.AdjustMoney(netAmount);
            economyManager?.SyncCashFromResources(resources.Money);
        }
        public bool UseAction() => resources.UseAction();
        public void ResetActions() => resources.ResetActions();
        public void AddExtraAction(int count = 1) => resources.AddExtraAction(count, balanceData);

        public void RegisterPersistentBuildPlayed()
        {
            persistentBuildsPlayedThisTurn = Mathf.Clamp(
                persistentBuildsPlayedThisTurn + 1,
                0,
                Constants.PERSISTENT_BUILDS_PER_TURN);
        }

        public void AddBusinessSlot()
        {
            resources.AddBusinessSlot(balanceData);
            if (boardManager != null)
                boardManager.SetMaxSlots(resources.BusinessSlots);
            slotManager?.TryExpandSlot(SlotType.Operation);
            EventBus.BusinessSlotsChanged(resources.BusinessSlots);
        }

        // === Customer Share & Market Block Updates ===

        public void SetPlayerCustomers(int count)
        {
            playerCustomers = Mathf.Max(0, count);
        }

        public void SetRivalCustomers(int count)
        {
            rivalCustomers = Mathf.Max(0, count);
        }

        public void SetMarketBlocks(int player, int rival)
        {
            int total = Constants.MARKET_VISUAL_BLOCKS;
            playerMarketBlocks = Mathf.Clamp(player, 0, total);
            rivalMarketBlocks = Mathf.Clamp(rival, 0, total);
            EventBus.MarketBlocksUpdated(playerMarketBlocks, rivalMarketBlocks);
        }

        // === FBI Risk ===

        public void SetFBIRisk(float risk)
        {
            fbiRisk = Mathf.Clamp(risk, 0f, 1f);
            EventBus.LegalRiskUpdated(Mathf.RoundToInt(fbiRisk * 100f));
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

        private RunSaveData BuildRunSave()
        {
            return BuildRunSaveData();
        }

        private void HandleCardPlacedInSlot(CardData card, SlotType slotType)
        {
            activeVentureRuntime?.RegisterCardPlayed(card, slotType);
        }

        private void RefreshActiveVentureRuntime()
        {
            activeVentureRuntime = VentureRuntimeFactory.Create(selectedVenture, _cardLookup);
        }

        private List<CardData> ResolveCards(IList<string> ids)
        {
            return ResolveCardsFromIds(ids);
        }
    }
}
