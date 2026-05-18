using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Core;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Displays money (animated counter), turn indicator, and FBI risk bar.
    /// Subscribes to EventBus events -- never calls manager methods for data.
    /// All animations use Update() polling, no coroutines.
    /// </summary>
    public class TopBarUI : MonoBehaviour
    {
        [Header("Money")]
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private float moneyLerpSpeed = 8f;

        [Header("Turn")]
        [SerializeField] private TMP_Text turnText;

        [Header("FBI Risk")]
        [SerializeField] private Image fbiBarFill;
        [SerializeField] private TMP_Text fbiRiskText;
        [SerializeField] private Color fbiColorLow = Color.green;
        [SerializeField] private Color fbiColorMid = Color.yellow;
        [SerializeField] private Color fbiColorHigh = Color.red;
        [SerializeField] private float fbiLerpSpeed = 5f;

        // Runtime
        private float displayedMoney;
        private float targetMoney;
        private float displayedFBI;
        private float targetFBI;

        /// <summary>
        /// Assigns all sub-element references without reflection.
        /// Called by WiringService instead of RuntimeWiring.SetField().
        /// </summary>
        public void Init(TMP_Text money, TMP_Text turn, Image fbiFill, TMP_Text fbiText)
        {
            this.moneyText = money;
            this.turnText = turn;
            this.fbiBarFill = fbiFill;
            this.fbiRiskText = fbiText;
        }

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void OnEnable()
        {
            EventBus.OnMoneyChanged += OnMoneyChanged;
            EventBus.OnTurnStarted += OnTurnStarted;
            EventBus.OnFBIRiskChanged += OnFBIRiskChanged;
        }

        private void OnDisable()
        {
            EventBus.OnMoneyChanged -= OnMoneyChanged;
            EventBus.OnTurnStarted -= OnTurnStarted;
            EventBus.OnFBIRiskChanged -= OnFBIRiskChanged;
        }

        private void Update()
        {
            // Lerp money counter toward target
            if (!Mathf.Approximately(displayedMoney, targetMoney))
            {
                displayedMoney = Mathf.MoveTowards(
                    displayedMoney,
                    targetMoney,
                    moneyLerpSpeed * Mathf.Max(1f, Mathf.Abs(targetMoney - displayedMoney)) * Time.deltaTime);

                if (moneyText != null)
                    moneyText.text = $"${Mathf.RoundToInt(displayedMoney):N0}";
            }

            // Lerp FBI bar fill and color
            if (!Mathf.Approximately(displayedFBI, targetFBI))
            {
                displayedFBI = Mathf.MoveTowards(displayedFBI, targetFBI, fbiLerpSpeed * Time.deltaTime);

                if (fbiBarFill != null)
                {
                    fbiBarFill.fillAmount = displayedFBI;
                    fbiBarFill.color = GetFBIColor(displayedFBI);
                }

                if (fbiRiskText != null)
                    fbiRiskText.text = $"FBI Risk: {Mathf.RoundToInt(displayedFBI * 100f)}%";
            }
        }

        // ------------------------------------------------------------------
        // EventBus callbacks
        // ------------------------------------------------------------------

        private void OnMoneyChanged(int newAmount)
        {
            SetTargetMoney(newAmount);
        }

        private void OnTurnStarted(int turnNumber)
        {
            UpdateTurn(turnNumber, Constants.MAX_TURNS);
        }

        private void OnFBIRiskChanged(float risk)
        {
            UpdateFBIRisk(risk);
        }

        // ------------------------------------------------------------------
        // Public setters (also called by UIManager.UpdateAllUI)
        // ------------------------------------------------------------------

        /// <summary>
        /// Sets the target money value. The counter lerps toward it in Update().
        /// </summary>
        public void SetTargetMoney(int amount)
        {
            targetMoney = amount;
        }

        /// <summary>
        /// Immediately sets the money display without animation.
        /// </summary>
        public void UpdateMoney(int amount)
        {
            targetMoney = amount;
            displayedMoney = amount;

            if (moneyText != null)
                moneyText.text = $"${amount:N0}";
        }

        /// <summary>
        /// Updates the turn counter text.
        /// </summary>
        public void UpdateTurn(int current, int max)
        {
            if (turnText != null)
                turnText.text = $"Turn {current}/{max}";
        }

        /// <summary>
        /// Sets the target FBI risk. The bar lerps toward it in Update().
        /// </summary>
        public void UpdateFBIRisk(float risk)
        {
            targetFBI = Mathf.Clamp01(risk);
        }

        // ------------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------------

        private Color GetFBIColor(float t)
        {
            // 0..0.5 -> green..yellow, 0.5..1 -> yellow..red
            if (t < 0.5f)
                return Color.Lerp(fbiColorLow, fbiColorMid, t * 2f);
            else
                return Color.Lerp(fbiColorMid, fbiColorHigh, (t - 0.5f) * 2f);
        }
    }
}
