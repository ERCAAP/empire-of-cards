using System;
using System.Collections;
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
        [SerializeField] private BoardGuidePanelUI boardGuidePanel;
        [SerializeField] private RivalIntentPanelUI rivalIntentPanel;

        [Header("Neglect Warning")]
        [SerializeField] private TMP_Text neglectWarningText;
        private float _neglectWarningTimer;

        [Header("Turn Flow")]
        [SerializeField] private TMP_Text turnBriefText;
        [SerializeField] private TMP_Text turnReportText;

        [Header("Income Cascade")]
        [SerializeField] private float cascadeStepDuration = 0.4f;
        [SerializeField] private float cascadeNetHoldDuration = 0.8f;
        [SerializeField] private float cascadeSlideDistance = 200f;
        [SerializeField] private int cascadeFontSize = 36;
        [SerializeField] private int cascadeNetFontSize = 48;
        private Coroutine _activeCascade;

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

        public void SetBoardGuidePanel(BoardGuidePanelUI panel)
        {
            boardGuidePanel = panel;
        }

        public void SetRivalIntentPanel(RivalIntentPanelUI panel)
        {
            rivalIntentPanel = panel;
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
            EventBus.OnIncomeBreakdown += HandleIncomeBreakdown;
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
            EventBus.OnSalaryChoiceRequired += HandleSalaryChoiceRequired;
            EventBus.OnSalaryPaid += HandleSalaryPaid;
            EventBus.OnTaxPeriodProcessed += HandleTaxPeriodProcessed;
            EventBus.OnTaxAuditTriggered += HandleTaxAuditTriggered;
            EventBus.OnInflationOccurred += HandleInflationOccurred;
            EventBus.OnSupplierFailed += HandleSupplierFailed;
            EventBus.OnStockSpoilageOccurred += HandleStockSpoilageOccurred;
            EventBus.OnStockSeasonLossOccurred += HandleStockSeasonLossOccurred;
            EventBus.OnStaffPoachAttempted += HandleStaffPoachAttempted;
            EventBus.OnStaffPoachAccepted += HandleStaffPoachAccepted;
            EventBus.OnStaffPoachCountered += HandleStaffPoachCountered;
            EventBus.OnStaffPoachRejected += HandleStaffPoachRejected;
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
            EventBus.OnIncomeBreakdown -= HandleIncomeBreakdown;
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
            EventBus.OnSalaryChoiceRequired -= HandleSalaryChoiceRequired;
            EventBus.OnSalaryPaid -= HandleSalaryPaid;
            EventBus.OnTaxPeriodProcessed -= HandleTaxPeriodProcessed;
            EventBus.OnTaxAuditTriggered -= HandleTaxAuditTriggered;
            EventBus.OnInflationOccurred -= HandleInflationOccurred;
            EventBus.OnSupplierFailed -= HandleSupplierFailed;
            EventBus.OnStockSpoilageOccurred -= HandleStockSpoilageOccurred;
            EventBus.OnStockSeasonLossOccurred -= HandleStockSeasonLossOccurred;
            EventBus.OnStaffPoachAttempted -= HandleStaffPoachAttempted;
            EventBus.OnStaffPoachAccepted -= HandleStaffPoachAccepted;
            EventBus.OnStaffPoachCountered -= HandleStaffPoachCountered;
            EventBus.OnStaffPoachRejected -= HandleStaffPoachRejected;
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

            boardGuidePanel?.Refresh(GameManager.Instance);
            if (phase == TurnPhase.RivalPhase)
                rivalIntentPanel?.BeginPhase(GameManager.Instance);
            else
                rivalIntentPanel?.SetIdle(GameManager.Instance);
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
            RefreshContextPanels();
        }

        private void HandleTerritoryChanged(int player, int rival)
        {
            // TopBar or a dedicated territory widget can reflect this.
            // Kept intentionally thin -- panels subscribe themselves.
            RefreshAnalytics();
            RefreshContextPanels();
        }

        private void HandleTurnStarted(int turnNumber)
        {
            if (topBar != null)
                topBar.UpdateTurn(turnNumber, Constants.MAX_TURNS);
            RefreshAnalytics();
            RefreshContextPanels();
        }

        private void HandleBusinessNeglected(int businessIndex, int neglectTurns)
        {
            if (neglectWarningText == null) return;

            int penalty = neglectTurns >= 6 ? 40 : 20;
            string key = neglectTurns >= 6 ? "neglect.critical" : "neglect.warning";
            neglectWarningText.text = LocalizationManager.Get(key, businessIndex + 1, penalty);
            _neglectWarningTimer = 3f; // Show for 3 seconds
        }

        private void HandleIncomeBreakdown(IncomeBreakdown breakdown)
        {
            if (breakdown == null || breakdown.steps.Count == 0) return;

            // Cancel any in-progress cascade before starting a new one
            if (_activeCascade != null)
                StopCoroutine(_activeCascade);

            _activeCascade = StartCoroutine(IncomeCascadeRoutine(breakdown));
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
            RefreshContextPanels();
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
            RefreshContextPanels();
        }

        private void HandleRivalActionQueued(RivalQueuedAction action)
        {
            if (rivalPopup != null && action != null)
                rivalPopup.Show($"{action.displayName}  [{action.laneLabel}]", action.shortDescription);
            rivalIntentPanel?.RevealAction(GameManager.Instance, action);
            RefreshAnalytics();
        }

        private void HandleBoardChanged(CardData card, int businessIndex)
        {
            topBar?.UpdateBuildIdentity(GameClarityFormatter.GetBuildIdentity(GameManager.Instance));
            RefreshAnalytics();
            RefreshContextPanels();
        }

        private void HandleBusinessPlaced(CardData card, int slotIndex)
        {
            topBar?.UpdateBuildIdentity(GameClarityFormatter.GetBuildIdentity(GameManager.Instance));
            RefreshAnalytics();
            RefreshContextPanels();
        }

        private void HandleCardPlacedInSlot(CardData card, SlotType slotType)
        {
            topBar?.UpdateBuildIdentity(GameClarityFormatter.GetBuildIdentity(GameManager.Instance));
            RefreshAnalytics();
            RefreshContextPanels();
        }

        private void HandleCardRemovedFromSlot(CardData card, SlotType slotType)
        {
            topBar?.UpdateBuildIdentity(GameClarityFormatter.GetBuildIdentity(GameManager.Instance));
            RefreshAnalytics();
            RefreshContextPanels();
        }

        private void HandleSalaryChoiceRequired(int totalSalaries)
        {
            string detail = $"Payroll due this turn: ${totalSalaries:N0}. Delay raises burnout and poach risk.";
            eventPopup?.ShowMessage("Payroll Due", detail, "Finance Pressure");
            analyticsPanel?.SetAlert("Payroll Due", detail);
            RefreshAnalytics();
        }

        private void HandleSalaryPaid(SalaryChoice choice, int amountPaid)
        {
            string detail = $"Payroll policy: {FormatSalaryChoice(choice)} · Cash moved ${amountPaid:N0}.";
            eventPopup?.ShowMessage("Payroll Resolved", detail, "Staff Stability");
            analyticsPanel?.SetAlert("Payroll Resolved", detail);
            RefreshAnalytics();
        }

        private void HandleTaxPeriodProcessed(int taxOwed, int amountPaid)
        {
            string detail = amountPaid >= taxOwed
                ? $"Tax cleared. Paid ${amountPaid:N0} against ${taxOwed:N0} owed."
                : $"Tax debt rolled. Paid ${amountPaid:N0} against ${taxOwed:N0} owed.";
            eventPopup?.ShowMessage("Tax Period", detail, "Legal Pressure");
            analyticsPanel?.SetAlert("Tax Period", detail);
            RefreshAnalytics();
        }

        private void HandleTaxAuditTriggered(int unpaidDebt)
        {
            string detail = $"Audit opened on unpaid debt of ${unpaidDebt:N0}. Legal pressure will escalate until cleared.";
            eventPopup?.ShowMessage("Tax Audit", detail, "Legal Pressure");
            analyticsPanel?.SetAlert("Tax Audit", detail);
            RefreshAnalytics();
        }

        private void HandleInflationOccurred(int currentTurn, float increase)
        {
            string detail = $"Turn {currentTurn}: supplier and payroll pressure rose by {increase:0.00}.";
            analyticsPanel?.SetAlert("Inflation", detail);
            RefreshAnalytics();
        }

        private void HandleSupplierFailed(VentureType venture, int penaltyCost)
        {
            string detail = $"{FormatVentureName(venture)} supply missed. Immediate penalty ${penaltyCost:N0} and freshness risk increased.";
            eventPopup?.ShowMessage("Supplier Failure", detail, "Supply Pressure");
            analyticsPanel?.SetAlert("Supplier Failure", detail);
            RefreshAnalytics();
        }

        private void HandleStockSpoilageOccurred(VentureType venture, int cost)
        {
            string detail = $"{FormatVentureName(venture)} stock spoiled for ${cost:N0}. Freshness and margin are slipping.";
            eventPopup?.ShowMessage("Stock Loss", detail, "Inventory Pressure");
            analyticsPanel?.SetAlert("Stock Loss", detail);
            RefreshAnalytics();
        }

        private void HandleStockSeasonLossOccurred(VentureType venture, int cost)
        {
            string detail = $"{FormatVentureName(venture)} seasonal mismatch burned ${cost:N0}.";
            eventPopup?.ShowMessage("Season Loss", detail, "Inventory Pressure");
            analyticsPanel?.SetAlert("Season Loss", detail);
            RefreshAnalytics();
        }

        private void HandleStaffPoachAttempted(CardData card, int offer)
        {
            string target = card != null ? card.cardName : "Staff";
            string detail = $"{target} received a rival offer worth ${offer:N0}. Counter now or stability drops.";
            rivalPopup?.Show("STAFF POACH", detail);
            analyticsPanel?.SetAlert("Staff Poach", detail);
            RefreshAnalytics();
        }

        private void HandleStaffPoachAccepted(CardData card)
        {
            string target = card != null ? card.cardName : "Staff";
            string detail = $"{target} left for the rival. Replace capacity before demand slips.";
            eventPopup?.ShowMessage("Staff Lost", detail, "Rival Pressure");
            analyticsPanel?.SetAlert("Staff Lost", detail);
            RefreshAnalytics();
        }

        private void HandleStaffPoachCountered(CardData card, int cost)
        {
            string target = card != null ? card.cardName : "Staff";
            string detail = $"{target} stayed after a counter-offer costing ${cost:N0}.";
            eventPopup?.ShowMessage("Poach Countered", detail, "Staff Stability");
            analyticsPanel?.SetAlert("Poach Countered", detail);
            RefreshAnalytics();
        }

        private void HandleStaffPoachRejected(CardData card, int cost)
        {
            string target = card != null ? card.cardName : "Staff";
            string detail = $"{target} stayed without leaving, but retention spending hit ${cost:N0}.";
            eventPopup?.ShowMessage("Poach Defended", detail, "Staff Stability");
            analyticsPanel?.SetAlert("Poach Defended", detail);
            RefreshAnalytics();
        }

        // ------------------------------------------------------------------
        // Income Cascade Animation (Balatro-style scoring chain)
        // ------------------------------------------------------------------

        /// <summary>
        /// Shows each income component one at a time as floating text near the
        /// top bar, then finishes with the NET total. Temporary TMP_Text objects
        /// are spawned on the HUD canvas, animated, and destroyed.
        /// </summary>
        private IEnumerator IncomeCascadeRoutine(IncomeBreakdown breakdown)
        {
            // Find the HUD canvas (UIManager lives on the canvas GO)
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = GetComponent<Canvas>();
            }
            if (canvas == null)
            {
                Debug.LogWarning("[UIManager] No canvas found for income cascade.");
                yield break;
            }

            Transform parent = canvas.transform;

            // Vertical offset: cascade appears below the top bar area
            float startY = Screen.height * 0.35f;

            // Show each step one at a time
            for (int i = 0; i < breakdown.steps.Count; i++)
            {
                IncomeStep step = breakdown.steps[i];
                float yPos = startY - (i % 6) * 50f; // Wrap after 6 to avoid going off-screen

                yield return StartCoroutine(AnimateCascadeStep(parent, step, yPos));
            }

            // Final NET line -- large, white, holds longer
            yield return StartCoroutine(AnimateCascadeNet(parent, breakdown.netIncome, startY));

            _activeCascade = null;
        }

        private static string FormatSalaryChoice(SalaryChoice choice)
        {
            return choice switch
            {
                SalaryChoice.PayOnTime => "Pay on time",
                SalaryChoice.Delay => "Delay payroll",
                SalaryChoice.PartialPay => "Partial pay",
                SalaryChoice.Advance => "Advance pay",
                _ => choice.ToString()
            };
        }

        private static string FormatVentureName(VentureType venture)
        {
            return venture switch
            {
                VentureType.FastFood => "Fast Food",
                VentureType.Cafe => "Cafe",
                VentureType.GroceryStore => "Grocery Store",
                VentureType.TechApp => "Tech App",
                VentureType.ClothingStore => "Clothing Store",
                _ => venture.ToString()
            };
        }

        /// <summary>
        /// Animates a single income step: creates text, slides it in, holds,
        /// fades out, then destroys the object.
        /// Positive = green, slides from left. Negative = red, slides from right.
        /// Multiplier = gold, slams from center.
        /// </summary>
        private IEnumerator AnimateCascadeStep(Transform parent, IncomeStep step, float yPos)
        {
            // Create temporary text object
            GameObject go = new GameObject("CascadeStep");
            go.transform.SetParent(parent, false);

            TMP_Text tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = cascadeFontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.raycastTarget = false;

            // Add outline for readability
            tmp.outlineWidth = 0.2f;
            tmp.outlineColor = new Color32(0, 0, 0, 180);

            // Format text: "Label  +$50" or "Label  -$110" or "Combo x1.5  +$30"
            string sign = step.amount >= 0 ? "+" : "";
            tmp.text = $"{step.label}  {sign}${step.amount}";

            // Color based on step type
            Color stepColor;
            if (step.isMultiplier)
                stepColor = new Color(1f, 0.84f, 0f);   // Gold
            else if (step.isNegative)
                stepColor = new Color(1f, 0.3f, 0.3f);  // Red
            else
                stepColor = new Color(0.3f, 1f, 0.4f);  // Green

            tmp.color = stepColor;

            // Position via RectTransform
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(500f, 60f);

            // Determine slide direction
            float slideFrom;
            if (step.isMultiplier)
                slideFrom = 0f;         // Center -- slam effect
            else if (step.isNegative)
                slideFrom = cascadeSlideDistance;   // From right
            else
                slideFrom = -cascadeSlideDistance;  // From left

            float centerX = 0f;
            float elapsed = 0f;
            float halfDuration = cascadeStepDuration * 0.5f;

            // Slide in phase (first half)
            while (elapsed < halfDuration)
            {
                float t = elapsed / halfDuration;
                // Ease out quad
                float easeT = 1f - (1f - t) * (1f - t);

                float xPos;
                float scale;

                if (step.isMultiplier)
                {
                    // Slam from large to normal
                    scale = Mathf.Lerp(2f, 1f, easeT);
                    xPos = centerX;
                    rt.localScale = Vector3.one * scale;
                }
                else
                {
                    xPos = Mathf.Lerp(slideFrom, centerX, easeT);
                    rt.localScale = Vector3.one;
                }

                rt.anchoredPosition = new Vector2(xPos, yPos);
                tmp.color = new Color(stepColor.r, stepColor.g, stepColor.b, easeT);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Hold at full visibility
            rt.anchoredPosition = new Vector2(centerX, yPos);
            rt.localScale = Vector3.one;
            tmp.color = stepColor;

            // Fade out phase (second half)
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                float t = elapsed / halfDuration;
                float alpha = 1f - t;
                float drift = t * 20f; // Slight upward drift

                rt.anchoredPosition = new Vector2(centerX, yPos + drift);
                tmp.color = new Color(stepColor.r, stepColor.g, stepColor.b, alpha);

                elapsed += Time.deltaTime;
                yield return null;
            }

            UnityEngine.Object.Destroy(go);
        }

        /// <summary>
        /// Animates the final NET income line: large white text, holds longer,
        /// then fades out.
        /// </summary>
        private IEnumerator AnimateCascadeNet(Transform parent, int netIncome, float yPos)
        {
            GameObject go = new GameObject("CascadeNet");
            go.transform.SetParent(parent, false);

            TMP_Text tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = cascadeNetFontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.raycastTarget = false;

            // Strong outline for final number
            tmp.outlineWidth = 0.3f;
            tmp.outlineColor = new Color32(0, 0, 0, 220);

            string sign = netIncome >= 0 ? "+" : "";
            tmp.text = $"NET  {sign}${netIncome}";

            // White text, or tinted green/red if strongly positive/negative
            Color netColor;
            if (netIncome > 0)
                netColor = new Color(0.9f, 1f, 0.9f); // Slightly green white
            else if (netIncome < 0)
                netColor = new Color(1f, 0.7f, 0.7f); // Slightly red white
            else
                netColor = Color.white;

            tmp.color = netColor;

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(500f, 70f);
            rt.anchoredPosition = new Vector2(0f, yPos);

            // Slam in from scale 2 -> 1
            float slamDuration = 0.15f;
            float elapsed = 0f;

            while (elapsed < slamDuration)
            {
                float t = elapsed / slamDuration;
                float easeT = 1f - (1f - t) * (1f - t);
                float scale = Mathf.Lerp(2f, 1f, easeT);
                rt.localScale = Vector3.one * scale;
                tmp.color = new Color(netColor.r, netColor.g, netColor.b, easeT);

                elapsed += Time.deltaTime;
                yield return null;
            }

            rt.localScale = Vector3.one;
            tmp.color = netColor;

            // Hold
            yield return new WaitForSeconds(cascadeNetHoldDuration);

            // Fade out
            elapsed = 0f;
            float fadeDuration = 0.3f;

            while (elapsed < fadeDuration)
            {
                float t = elapsed / fadeDuration;
                float alpha = 1f - t;
                tmp.color = new Color(netColor.r, netColor.g, netColor.b, alpha);

                elapsed += Time.deltaTime;
                yield return null;
            }

            UnityEngine.Object.Destroy(go);
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
            RefreshContextPanels();
        }

        private void RefreshAnalytics()
        {
            analyticsPanel?.Refresh(GameManager.Instance);
            if (GameManager.Instance != null && GameManager.Instance.EconomyManager != null)
                topBar?.UpdatePressureState(GameManager.Instance.EconomyManager.CurrentPressure);
        }

        private void RefreshContextPanels()
        {
            boardGuidePanel?.Refresh(GameManager.Instance);
            rivalIntentPanel?.RefreshSnapshot(GameManager.Instance);
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
