using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

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
        // HandUI removed -- Hand3D replaces it in 3D mode
        [SerializeField] private ComboPopup comboPopup;
        [SerializeField] private EventPopup eventPopup;
        [SerializeField] private RivalPopup rivalPopup;
        [SerializeField] private ScoreScreen scoreScreen;
        [SerializeField] private GameOverScreen gameOverScreen;

        [Header("Neglect Warning")]
        [SerializeField] private TMP_Text neglectWarningText;
        private float _neglectWarningTimer;

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
            ComboPopup combo, EventPopup eventP, RivalPopup rival,
            ScoreScreen score, GameOverScreen gameOver)
        {
            this.topBar = top;
            this.actionBar = action;
            this.shopPanel = shop;
            this.comboPopup = combo;
            this.eventPopup = eventP;
            this.rivalPopup = rival;
            this.scoreScreen = score;
            this.gameOverScreen = gameOver;
        }

        /// <summary>
        /// Sets the neglect warning TMP_Text reference.
        /// Called by WiringService after Init().
        /// </summary>
        public void SetNeglectWarningText(TMP_Text text) { neglectWarningText = text; }

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void OnEnable()
        {
            EventBus.OnPhaseStarted += HandlePhaseStarted;
            EventBus.OnComboTriggered += HandleComboTriggered;
            EventBus.OnEventActivated += HandleEventActivated;
            EventBus.OnRivalAction += HandleRivalAction;
            EventBus.OnRivalTaunt += HandleRivalTaunt;
            EventBus.OnGameOver += HandleGameOver;
            EventBus.OnMoneyChanged += HandleMoneyChanged;
            EventBus.OnMarketBlocksChanged += HandleTerritoryChanged;
            EventBus.OnFBIRiskChanged += HandleFBIRiskChanged;
            EventBus.OnTurnStarted += HandleTurnStarted;
            EventBus.OnBusinessNeglected += HandleBusinessNeglected;
            EventBus.OnIncomeBreakdown += HandleIncomeBreakdown;
            EventBus.OnRivalMoodChanged += HandleRivalMoodChanged;
            EventBus.OnRivalStrategyComment += HandleRivalStrategyComment;
        }

        private void OnDisable()
        {
            EventBus.OnPhaseStarted -= HandlePhaseStarted;
            EventBus.OnComboTriggered -= HandleComboTriggered;
            EventBus.OnEventActivated -= HandleEventActivated;
            EventBus.OnRivalAction -= HandleRivalAction;
            EventBus.OnRivalTaunt -= HandleRivalTaunt;
            EventBus.OnGameOver -= HandleGameOver;
            EventBus.OnMoneyChanged -= HandleMoneyChanged;
            EventBus.OnMarketBlocksChanged -= HandleTerritoryChanged;
            EventBus.OnFBIRiskChanged -= HandleFBIRiskChanged;
            EventBus.OnTurnStarted -= HandleTurnStarted;
            EventBus.OnBusinessNeglected -= HandleBusinessNeglected;
            EventBus.OnIncomeBreakdown -= HandleIncomeBreakdown;
            EventBus.OnRivalMoodChanged -= HandleRivalMoodChanged;
            EventBus.OnRivalStrategyComment -= HandleRivalStrategyComment;
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

        private void HandleComboTriggered(ComboData combo)
        {
            if (comboPopup != null)
                comboPopup.Show(combo.displayText, combo.glowColor);
        }

        private void HandleEventActivated(CardData eventCard)
        {
            if (eventPopup != null)
                eventPopup.Show(eventCard);
        }

        private void HandleRivalAction(string action)
        {
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
        }

        private void HandleTerritoryChanged(int player, int rival)
        {
            // TopBar or a dedicated territory widget can reflect this.
            // Kept intentionally thin -- panels subscribe themselves.
        }

        private void HandleFBIRiskChanged(float risk)
        {
            if (topBar != null)
                topBar.UpdateFBIRisk(risk);
        }

        private void HandleTurnStarted(int turnNumber)
        {
            if (topBar != null)
                topBar.UpdateTurn(turnNumber, Constants.MAX_TURNS);
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
                topBar.UpdateFBIRisk(gm.FBIRisk);
            }

            if (actionBar != null)
                actionBar.UpdateActions(gm.PlayerActions, gm.MaxActions);
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
