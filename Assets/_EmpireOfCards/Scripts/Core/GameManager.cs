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

        [Header("=== Customer Share & Market Blocks ===")]
        [SerializeField] private int playerCustomers;
        [SerializeField] private int rivalCustomers;
        [SerializeField] private int playerMarketBlocks;
        [SerializeField] private int rivalMarketBlocks;
        [SerializeField] private float fbiRisk;

        [Header("=== Balance Data ===")]
        [SerializeField] private GameBalanceData balanceData;

        [Header("=== Manager References ===")]
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private EconomyManager economyManager;
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private TerritoryManager territoryManager;
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
        public TerritoryManager TerritoryManager => territoryManager;
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

        public void SetSlotManager(SlotManager sm)
        {
            this.slotManager = sm;
        }

        public void SetStaffStateSystem(StaffStateSystem sss)
        {
            this.staffStateSystem = sss;
        }

        public void SetChainReactionSystem(ChainReactionSystem crs)
        {
            this.chainReactionSystem = crs;
        }

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

        /// <summary>
        /// Assigns all manager dependencies without reflection.
        /// Called by WiringService during bootstrap.
        /// </summary>
        public void Init(GameBalanceData balance, TurnManager tm,
            EconomyManager em, DeckManager dm, BoardManager bm,
            TerritoryManager ter, RivalAI ai, ShopManager shop,
            UIManager ui, AudioManager audio, VFXManager vfx, SaveManager save)
        {
            this.balanceData = balance;
            this.turnManager = tm;
            this.economyManager = em;
            this.deckManager = dm;
            this.boardManager = bm;
            this.territoryManager = ter;
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

            // Reset extracted resource state
            resources.Reset(balanceData);

            // Reset customer/market/fbi state
            fbiRisk = 0f;
            playerCustomers = 0;
            rivalCustomers = 0;
            playerMarketBlocks = 0;
            rivalMarketBlocks = 0;

            // Initialize subsystems
            VentureType chosenVenture = VentureType.FastFood; // Default
            if (selectedVenture != null)
            {
                chosenVenture = selectedVenture.ventureType;
            }

            if (activeBoardProfile != null && slotManager != null)
            {
                slotManager.Configure(activeBoardProfile);
                resources.SetBusinessSlots(activeBoardProfile.startingOperationSlots);
            }

            if (boardManager != null)
            {
                boardManager.Reset();
                boardManager.ConfigureForVenture(chosenVenture, activeBoardProfile);
                boardManager.SetMaxSlots(resources.BusinessSlots);
            }

            if (activeEconomyProfile != null && economyManager != null)
            {
                economyManager.SetActiveProfile(activeEconomyProfile);
                resources.SetMoney(Mathf.RoundToInt(activeEconomyProfile.startingCash) + (selectedVenture != null ? selectedVenture.bonusMoney : 0));
                economyManager.SyncCashFromResources(resources.Money);
            }

            if (deckManager != null)
            {
                if (activeDeckProfile != null && _cardLookup != null)
                    deckManager.InitializeDeck(activeDeckProfile, _cardLookup);
            }

            if (selectedVenture != null)
            {
                if (selectedVenture.startingBusiness != null && boardManager != null)
                    boardManager.PlaceBusiness(selectedVenture.startingBusiness, 0);
                if (selectedVenture.bonusDeckCard != null && deckManager != null)
                    deckManager.AddCardToDeck(selectedVenture.bonusDeckCard);
            }

            if (rivalAI != null)
            {
                if (selectedVenture != null)
                    rivalAI.Initialize(chosenVenture);
                else
                    rivalAI.Initialize();
            }
            if (shopManager != null)
            {
                if (selectedVenture != null)
                {
                    shopManager.SetVentureBias(chosenVenture);
                    shopManager.FilterPoolByVenture(chosenVenture);
                }
                shopManager.RefreshShop();
            }
            if (staffStateSystem != null)
                staffStateSystem.Reset();
            if (chainReactionSystem != null)
                chainReactionSystem.Reset();
            WinLoseChecker.Reset();

            // Fire initial events so UI updates
            EventBus.MoneyUpdated(resources.Money);
            EventBus.MarketBlocksUpdated(0, 0);
            SetGameState(GameState.Playing);
            _gameStateMachine.Initialize(new InGameState());

            if (autoStartTurn)
                StartNextTurn();
        }

        public void StartNextTurn()
        {
            currentTurn++;
            activeVentureRuntime?.OnTurnStarted(currentTurn);
            EventBus.TurnStarted(currentTurn);
            if (economyManager != null)
                economyManager.GenerateTurnBrief(currentTurn);

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

            SetRunDisplayName(runData.runName);
            SetRunCategory(runData.runCategoryId, runData.runCategoryLabel);
            currentTurn = Mathf.Max(1, runData.currentTurn);

            resources.SetMoney(runData.playerMoney);
            resources.SetActions(runData.playerActions, runData.playerMaxActions);
            resources.SetBusinessSlots(runData.playerBusinessSlots);
            fbiRisk = runData.fbiRisk;
            playerCustomers = Mathf.Max(0, runData.playerCustomers);
            rivalCustomers = Mathf.Max(0, runData.rivalCustomers);
            SetMarketBlocks(runData.playerMarketBlocks, runData.rivalMarketBlocks);
            if (activeBoardProfile != null && slotManager != null)
                slotManager.Configure(activeBoardProfile);

            var opCards = ResolveCards(runData.operationSlotIds);
            var staffCards = ResolveCards(runData.staffSlotIds);
            var marketingCards = ResolveCards(runData.marketingSlotIds);
            var supplierCards = ResolveCards(runData.supplierSlotIds);
            var tempCards = ResolveCards(runData.tempEffectSlotIds);
            slotManager?.RestoreState(opCards, staffCards, marketingCards, supplierCards, tempCards);
            boardManager?.RebuildFromSlots();

            deckManager?.RestoreState(
                activeDeckProfile,
                _cardLookup,
                runData.drawPileIds,
                runData.handIds,
                runData.discardPileIds,
                runData.redrawsRemaining);

            if (economyManager != null && runData.economySnapshot != null)
                economyManager.RestoreSnapshot(runData.economySnapshot);
            else
                economyManager?.SyncCashFromResources(resources.Money);

            activeVentureRuntime?.RestoreState(runData.ventureRuntimeState, runData.openingArcState, runData.eventChainState);
            activeVentureRuntime?.OnTurnStarted(currentTurn);
            rivalAI?.RestoreState(runData.rivalState);

            EventBus.MoneyUpdated(resources.Money);
            EventBus.TurnStarted(currentTurn);
            turnManager?.ResumePlayPhase(currentTurn);
        }

        /// <summary>
        /// Called by TurnManager when all 5 phases are done.
        /// Checks win/lose, max turns, then starts the next turn.
        /// </summary>
        public void EndCurrentTurn()
        {
            EventBus.TurnEnded(currentTurn);

            int winCustomers = Constants.WIN_CUSTOMER_SHARE;
            bool dominationActive = currentTurn >= Constants.DOMINATION_CHECK_START_TURN;

            if (dominationActive && WinLoseChecker.CheckWin(playerCustomers, winCustomers))
            {
                EndRun(true);
                return;
            }

            // 1. Bankruptcy — always active
            if (resources.Money <= 0)
            {
                EndRun(false);
                return;
            }

            // 4. Rival domination — only after domination turn threshold
            if (dominationActive && rivalCustomers >= winCustomers)
            {
                EndRun(false);
                return;
            }

            // GDD 13.3 extended loss conditions: reputation collapse + legal disaster
            if (economyManager != null && economyManager.Snapshot != null)
            {
                if (WinLoseChecker.CheckExtendedLose(
                    economyManager.Snapshot.rating,
                    economyManager.Snapshot.legalRisk))
                {
                    EndRun(false);
                    return;
                }
            }

            if (currentTurn >= MaxTurns)
            {
                EndRun(playerCustomers >= rivalCustomers);
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
            return new RunSaveData
            {
                saveVersion = SaveManager.CurrentRunSaveVersion,
                slotId = currentRunSlotId,
                runName = RunDisplayName,
                runCategoryId = runCategoryId,
                runCategoryLabel = runCategoryLabel,
                ventureType = (int)(selectedVenture != null ? selectedVenture.ventureType : VentureType.FastFood),
                currentTurn = currentTurn,
                playerMoney = resources.Money,
                playerActions = resources.Actions,
                playerMaxActions = resources.MaxActions,
                playerBusinessSlots = resources.BusinessSlots,
                playerCustomers = playerCustomers,
                rivalCustomers = rivalCustomers,
                playerMarketBlocks = playerMarketBlocks,
                rivalMarketBlocks = rivalMarketBlocks,
                fbiRisk = fbiRisk,
                redrawsRemaining = deckManager != null ? deckManager.RedrawsRemaining : 0,
                economySnapshot = economyManager != null ? economyManager.Snapshot : null,
                drawPileIds = deckManager != null ? deckManager.GetDrawPileIds() : new List<string>(),
                handIds = deckManager != null ? deckManager.GetHandIds() : new List<string>(),
                discardPileIds = deckManager != null ? deckManager.GetDiscardPileIds() : new List<string>(),
                operationSlotIds = slotManager != null ? slotManager.GetSlotIds(SlotType.Operation) : new List<string>(),
                staffSlotIds = slotManager != null ? slotManager.GetSlotIds(SlotType.Staff) : new List<string>(),
                marketingSlotIds = slotManager != null ? slotManager.GetSlotIds(SlotType.Marketing) : new List<string>(),
                supplierSlotIds = slotManager != null ? slotManager.GetSlotIds(SlotType.Supplier) : new List<string>(),
                tempEffectSlotIds = slotManager != null ? slotManager.GetSlotIds(SlotType.TempEffect) : new List<string>(),
                ventureRuntimeState = activeVentureRuntime != null ? activeVentureRuntime.CaptureRuntimeState() : null,
                openingArcState = activeVentureRuntime != null ? activeVentureRuntime.CaptureOpeningArcState() : null,
                eventChainState = activeVentureRuntime != null ? activeVentureRuntime.CaptureEventChainState() : null,
                rivalState = rivalAI != null ? rivalAI.CaptureState() : null
            };
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
            var cards = new List<CardData>();
            if (ids == null)
                return cards;

            for (int i = 0; i < ids.Count; i++)
            {
                string id = ids[i];
                if (string.IsNullOrWhiteSpace(id))
                {
                    cards.Add(null);
                    continue;
                }

                cards.Add(_cardLookup != null && _cardLookup.TryGetValue(id, out var card) ? card : null);
            }

            return cards;
        }
    }
}
