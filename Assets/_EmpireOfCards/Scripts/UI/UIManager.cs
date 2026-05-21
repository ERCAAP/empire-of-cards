using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.World;
using EmpireOfCards.UI.Clarity;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Central UI controller. Subscribes to EventBus events and routes
    /// updates to the appropriate panel components. No panel calls manager
    /// methods for data -- everything flows through events.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Panel References")]
        [SerializeField] private TopBarUI topBar;
        [SerializeField] private ActionBarUI actionBar;
        [SerializeField] private ShopPanel shopPanel;
        // Hand is managed by Hand3D in 3D mode
        [SerializeField] private EventPopup eventPopup;
        [SerializeField] private RivalPopup rivalPopup;
        [SerializeField] private ScoreScreen scoreScreen;
        [SerializeField] private GameOverScreen gameOverScreen;
        [SerializeField] private ClarityPanelUI clarityPanel;
        [SerializeField] private AnalyticsPanelUI analyticsPanel;

        [Header("Neglect Warning")]
        [SerializeField] private TMP_Text neglectWarningText;
        private float _neglectWarningTimer;

        [Header("Turn Flow")]
        [SerializeField] private TMP_Text turnBriefText;
        [SerializeField] private TMP_Text turnReportText;

        // --- Events for external listeners (e.g. TurnManager) ---
        public event Action OnEndTurnClicked;
        public event Action<bool> OnShopToggled;

        // --- Properties ---
        public TopBarUI TopBar => topBar;
        public ActionBarUI ActionBar => actionBar;
        public ShopPanel Shop => shopPanel;

        /// <summary>
        /// Assigns all dependencies without reflection.
        /// Called by WiringService during bootstrap.
        /// </summary>
        public void Init(TopBarUI top, ActionBarUI action, ShopPanel shop,
            EventPopup eventP, RivalPopup rival,
            ScoreScreen score, GameOverScreen gameOver)
        {
            this.topBar = top;
            this.actionBar = action;
            this.shopPanel = shop;
            this.eventPopup = eventP;
            this.rivalPopup = rival;
            this.scoreScreen = score;
            this.gameOverScreen = gameOver;
        }

        public void SetClarityPanel(ClarityPanelUI panel)
        {
            clarityPanel = panel;
        }

        public void SetAnalyticsPanel(AnalyticsPanelUI panel)
        {
            analyticsPanel = panel;
        }

        /// <summary>
        /// Sets the neglect warning TMP_Text reference.
        /// Called by WiringService after Init().
        /// </summary>
        public void SetNeglectWarningText(TMP_Text text) { neglectWarningText = text; }
        public void SetFlowTexts(TMP_Text brief, TMP_Text report)
        {
            turnBriefText = brief;
            turnReportText = report;
        }

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void OnEnable()
        {
            EventBus.OnPhaseStarted += HandlePhaseStarted;
            EventBus.OnEventActivated += HandleEventActivated;
            EventBus.OnRivalAction += HandleRivalAction;
            EventBus.OnRivalTaunt += HandleRivalTaunt;
            EventBus.OnGameOver += HandleGameOver;
            EventBus.OnMoneyChanged += HandleMoneyChanged;
            EventBus.OnMarketBlocksChanged += HandleTerritoryChanged;
            EventBus.OnTurnStarted += HandleTurnStarted;
            EventBus.OnBusinessNeglected += HandleBusinessNeglected;
            EventBus.OnRivalMoodChanged += HandleRivalMoodChanged;
            EventBus.OnRivalStrategyComment += HandleRivalStrategyComment;
            EventBus.OnTurnBriefGenerated += HandleTurnBriefGenerated;
            EventBus.OnTurnReportGenerated += HandleTurnReportGenerated;
            EventBus.OnRivalActionQueued += HandleRivalActionQueued;
            EventBus.OnBusinessPlaced += HandleBusinessPlaced;
            EventBus.OnEmployeePlaced += HandleBoardChanged;
            EventBus.OnUpgradePlaced += HandleBoardChanged;
            EventBus.OnCardPlacedInSlot += HandleCardPlacedInSlot;
            EventBus.OnCardRemovedFromSlot += HandleCardRemovedFromSlot;
        }

        private void OnDisable()
        {
            EventBus.OnPhaseStarted -= HandlePhaseStarted;
            EventBus.OnEventActivated -= HandleEventActivated;
            EventBus.OnRivalAction -= HandleRivalAction;
            EventBus.OnRivalTaunt -= HandleRivalTaunt;
            EventBus.OnGameOver -= HandleGameOver;
            EventBus.OnMoneyChanged -= HandleMoneyChanged;
            EventBus.OnMarketBlocksChanged -= HandleTerritoryChanged;
            EventBus.OnTurnStarted -= HandleTurnStarted;
            EventBus.OnBusinessNeglected -= HandleBusinessNeglected;
            EventBus.OnRivalMoodChanged -= HandleRivalMoodChanged;
            EventBus.OnRivalStrategyComment -= HandleRivalStrategyComment;
            EventBus.OnTurnBriefGenerated -= HandleTurnBriefGenerated;
            EventBus.OnTurnReportGenerated -= HandleTurnReportGenerated;
            EventBus.OnRivalActionQueued -= HandleRivalActionQueued;
            EventBus.OnBusinessPlaced -= HandleBusinessPlaced;
            EventBus.OnEmployeePlaced -= HandleBoardChanged;
            EventBus.OnUpgradePlaced -= HandleBoardChanged;
            EventBus.OnCardPlacedInSlot -= HandleCardPlacedInSlot;
            EventBus.OnCardRemovedFromSlot -= HandleCardRemovedFromSlot;
        }

        // ------------------------------------------------------------------
        // EventBus handlers
        // ------------------------------------------------------------------

        private void HandlePhaseStarted(TurnPhase phase)
        {
            // During PlayPhase the player can interact; hide/show as needed
            bool isPlayPhase = phase == TurnPhase.PlayPhase;

            // Hand interactability now driven by Hand3D via EventBus

            if (actionBar != null)
                actionBar.SetVisible(isPlayPhase);
        }

        private void HandleEventActivated(CardData eventCard)
        {
            if (eventPopup != null)
                eventPopup.Show(eventCard);
        }

        private void HandleRivalAction(string action)
        {
            if (string.IsNullOrWhiteSpace(action) || action.Contains("->"))
                return;
            if (rivalPopup != null)
                rivalPopup.Show(action, string.Empty);
        }

        private void HandleRivalTaunt(string taunt)
        {
            if (rivalPopup != null)
                rivalPopup.Show(string.Empty, taunt);
        }

        private void HandleRivalMoodChanged(string moodIcon)
        {
            if (rivalPopup != null)
                rivalPopup.ShowMoodIcon(moodIcon);
        }

        private void HandleRivalStrategyComment(string comment)
        {
            if (rivalPopup != null)
                rivalPopup.ShowStrategyComment(comment);
        }

        private void HandleGameOver(bool won)
        {
            if (won)
            {
                if (scoreScreen != null)
                {
                    scoreScreen.gameObject.SetActive(true);
                }
            }
            else
            {
                if (gameOverScreen != null)
                {
                    gameOverScreen.gameObject.SetActive(true);
                    gameOverScreen.Show(false);
                }
            }
        }

        private void HandleMoneyChanged(int newAmount)
        {
            if (topBar != null)
                topBar.SetTargetMoney(newAmount);
            RefreshAnalytics();
        }

        private void HandleTerritoryChanged(int player, int rival)
        {
            // TopBar or a dedicated territory widget can reflect this.
            // Kept intentionally thin -- panels subscribe themselves.
            RefreshAnalytics();
        }

        private void HandleTurnStarted(int turnNumber)
        {
            if (topBar != null)
                topBar.UpdateTurn(turnNumber, Constants.MAX_TURNS);
            RefreshAnalytics();
        }

        private void HandleBusinessNeglected(int businessIndex, int neglectTurns)
        {
            if (neglectWarningText == null) return;

            int penalty = neglectTurns >= 6 ? 40 : 20;
            string key = neglectTurns >= 6 ? "neglect.critical" : "neglect.warning";
            neglectWarningText.text = LocalizationManager.Get(key, businessIndex + 1, penalty);
            _neglectWarningTimer = 3f; // Show for 3 seconds
        }

        private void HandleTurnBriefGenerated(TurnBriefData brief)
        {
            if (turnBriefText == null || brief == null) return;
            string move = string.IsNullOrWhiteSpace(brief.recommendedMove) ? brief.detail : brief.recommendedMove;
            turnBriefText.text = $"PROBLEM  {brief.headline}";
            if (turnReportText != null)
                turnReportText.text = $"NEXT MOVE  {move}";
            topBar?.UpdateBuildIdentity(brief.buildIdentity);
            topBar?.UpdatePressureState(brief.pressure);
            RefreshAnalytics();
        }

        private void HandleTurnReportGenerated(TurnReportData report)
        {
            if (turnReportText == null || report == null) return;
            string reasons = report.primaryReason;
            if (string.IsNullOrWhiteSpace(reasons))
            {
                reasons = report.reasons != null && report.reasons.Count > 0
                    ? report.reasons[0]
                    : report.summary;
            }
            if (turnBriefText != null)
                turnBriefText.text = $"RESULT  {report.headline}";
            turnReportText.text = $"WHY  {reasons}";
            if (!string.IsNullOrWhiteSpace(report.buildIdentity))
                topBar?.UpdateBuildIdentity(report.buildIdentity);
            if (GameManager.Instance != null && GameManager.Instance.EconomyManager != null)
                topBar?.UpdatePressureState(GameManager.Instance.EconomyManager.CurrentPressure);
            RefreshAnalytics();
        }

        private void HandleRivalActionQueued(RivalQueuedAction action)
        {
            if (rivalPopup != null && action != null)
                rivalPopup.Show($"{action.displayName}  [{action.laneLabel}]", action.shortDescription);
            RefreshAnalytics();
        }

        private void HandleBoardChanged(CardData card, int businessIndex)
        {
            topBar?.UpdateBuildIdentity(GameClarityFormatter.GetBuildIdentity(GameManager.Instance));
            RefreshAnalytics();
        }

        private void HandleBusinessPlaced(CardData card, int slotIndex)
        {
            topBar?.UpdateBuildIdentity(GameClarityFormatter.GetBuildIdentity(GameManager.Instance));
            RefreshAnalytics();
        }

        private void HandleCardPlacedInSlot(CardData card, SlotType slotType)
        {
            topBar?.UpdateBuildIdentity(GameClarityFormatter.GetBuildIdentity(GameManager.Instance));
            RefreshAnalytics();
        }

        private void HandleCardRemovedFromSlot(CardData card, SlotType slotType)
        {
            topBar?.UpdateBuildIdentity(GameClarityFormatter.GetBuildIdentity(GameManager.Instance));
            RefreshAnalytics();
        }

        private void Update()
        {
            if (_neglectWarningTimer > 0)
            {
                _neglectWarningTimer -= Time.deltaTime;
                if (_neglectWarningTimer <= 0 && neglectWarningText != null)
                    neglectWarningText.text = "";
            }
        }

        // ------------------------------------------------------------------
        // Public helpers (called by buttons in the scene)
        // ------------------------------------------------------------------

        /// <summary>
        /// Opens the shop panel and fires the toggle event.
        /// </summary>
        public void ShowShop()
        {
            if (shopPanel != null)
                shopPanel.gameObject.SetActive(true);

            OnShopToggled?.Invoke(true);
        }

        public void ShowCardClarity(CardData card)
        {
            if (clarityPanel == null || card == null)
                return;

            string buildIdentity = GameClarityFormatter.GetBuildIdentity(GameManager.Instance);
            clarityPanel.ShowCard(card, buildIdentity);
        }

        public void ShowSlotClarity(CardData card, SlotZone3D slot, bool valid)
        {
            if (clarityPanel == null || card == null || slot == null)
                return;

            string buildIdentity = GameClarityFormatter.GetBuildIdentity(GameManager.Instance);
            clarityPanel.ShowSlotPreview(card, slot, valid, buildIdentity);
        }

        public void HideClarity()
        {
            clarityPanel?.Hide();
        }

        /// <summary>
        /// Closes the shop panel and fires the toggle event.
        /// </summary>
        public void HideShop()
        {
            if (shopPanel != null)
                shopPanel.Close();

            OnShopToggled?.Invoke(false);
        }

        /// <summary>
        /// Refreshes every visible UI element by pulling current GameManager state.
        /// Called once on game start or when re-entering the playing state.
        /// </summary>
        public void UpdateAllUI()
        {
            var gm = GameManager.Instance;
            if (gm == null)
                return;

            if (topBar != null)
            {
                topBar.SetTargetMoney(gm.PlayerMoney);
                topBar.UpdateTurn(gm.CurrentTurn, gm.MaxTurns);
                topBar.UpdateBuildIdentity(GameClarityFormatter.GetBuildIdentity(gm));
                if (gm.EconomyManager != null)
                    topBar.UpdatePressureState(gm.EconomyManager.CurrentPressure);
            }

            if (actionBar != null)
                actionBar.UpdateActions(gm.PlayerActions, gm.MaxActions);

            RefreshAnalytics();
        }

        private void RefreshAnalytics()
        {
            analyticsPanel?.Refresh(GameManager.Instance);
            if (GameManager.Instance != null && GameManager.Instance.EconomyManager != null)
                topBar?.UpdatePressureState(GameManager.Instance.EconomyManager.CurrentPressure);
        }

        /// <summary>
        /// Called by the End Turn button in the scene.
        /// </summary>
        public void OnEndTurnButtonClicked()
        {
            OnEndTurnClicked?.Invoke();
        }

        // ------------------------------------------------------------------
        // GameOver menu helpers
        // ------------------------------------------------------------------

        public void ShowGameOverScreen()
        {
            if (gameOverScreen != null)
            {
                gameOverScreen.gameObject.SetActive(true);
                gameOverScreen.Show(false);
            }
        }

        public void HideGameOverScreen()
        {
            if (gameOverScreen != null)
                gameOverScreen.gameObject.SetActive(false);
        }
    }
}
