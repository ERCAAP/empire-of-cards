using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using EmpireOfCards.Core;
using EmpireOfCards.UI.Clarity;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Displays money (animated counter), turn indicator, and FBI risk bar.
    /// Subscribes to EventBus events -- never calls manager methods for data.
    /// All animations use Update() polling, no coroutines.
    ///
    /// Money counter personality:
    ///   - Color flashes green/red on gain/loss
    ///   - Pulses red when low or critical
    ///   - Scale punches on large changes
    /// </summary>
    public class TopBarUI : MonoBehaviour
    {
        [Header("Money")]
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private float moneyLerpSpeed = 8f;

        [Header("Money Personality")]
        [SerializeField] private Color moneyColorNormal = new Color(1f, 0.85f, 0.2f);
        [SerializeField] private Color moneyColorGain = Color.green;
        [SerializeField] private Color moneyColorLoss = Color.red;
        [SerializeField] private float moneyFlashDuration = 0.2f;
        [SerializeField] private int moneyLowThreshold = 100;

        [Header("Turn")]
        [SerializeField] private TMP_Text turnText;
        [SerializeField] private TMP_Text companyTierText;
        [SerializeField] private TMP_Text buildIdentityText;
        [SerializeField] private TMP_Text pressureText;

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

        // Money personality state
        private int _previousMoney;
        private float _moneyFlashTimer;       // Counts down from moneyFlashDuration
        private Color _moneyFlashColor;       // The flash color (green or red)
        private float _moneyPulseTimer;       // Accumulates for low-money pulse

        /// <summary>
        /// Assigns all sub-element references without reflection.
        /// Called by WiringService during bootstrap.
        /// </summary>
        public void Init(TMP_Text money, TMP_Text turn, Image fbiFill, TMP_Text fbiText, TMP_Text tier = null, TMP_Text buildIdentity = null, TMP_Text pressure = null)
        {
            this.moneyText = money;
            this.turnText = turn;
            this.fbiBarFill = fbiFill;
            this.fbiRiskText = fbiText;
            this.companyTierText = tier;
            this.buildIdentityText = buildIdentity;
            this.pressureText = pressure;

            if (companyTierText != null)
                companyTierText.text = CompanyTier.Trader.ToString().ToUpperInvariant();
            if (buildIdentityText != null)
                buildIdentityText.text = "BUILD · Balanced Board";
            if (pressureText != null)
                pressureText.text = "PRESSURE · Board Stable";
        }

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void OnEnable()
        {
            EventBus.OnMoneyChanged += OnMoneyChanged;
            EventBus.OnTurnStarted += OnTurnStarted;
            EventBus.OnFBIRiskChanged += OnFBIRiskChanged;
            EventBus.OnCompanyTierChanged += OnCompanyTierChanged;
        }

        private void OnDisable()
        {
            EventBus.OnMoneyChanged -= OnMoneyChanged;
            EventBus.OnTurnStarted -= OnTurnStarted;
            EventBus.OnFBIRiskChanged -= OnFBIRiskChanged;
            EventBus.OnCompanyTierChanged -= OnCompanyTierChanged;
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

            // --- Money color personality ---
            if (moneyText != null)
                moneyText.color = GetMoneyColor();

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
            int delta = newAmount - _previousMoney;

            // --- Color flash based on direction ---
            if (delta > 0)
            {
                _moneyFlashColor = moneyColorGain;
                _moneyFlashTimer = moneyFlashDuration;
            }
            else if (delta < 0)
            {
                _moneyFlashColor = moneyColorLoss;
                _moneyFlashTimer = moneyFlashDuration;
            }

            // --- Scale punch based on magnitude ---
            int absDelta = Mathf.Abs(delta);
            if (absDelta > 200 && moneyText != null)
            {
                moneyText.transform.DOKill();
                moneyText.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 6, 0.5f)
                    .SetLink(moneyText.gameObject);
            }
            else if (absDelta > 50 && moneyText != null)
            {
                moneyText.transform.DOKill();
                moneyText.transform.DOPunchScale(Vector3.one * 0.05f, 0.15f, 6, 0.5f)
                    .SetLink(moneyText.gameObject);
            }

            _previousMoney = newAmount;
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

        private void OnCompanyTierChanged(CompanyTier tier)
        {
            if (companyTierText != null)
                companyTierText.text = tier.ToString().ToUpperInvariant();
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
        /// Also syncs previous money so no false flash triggers.
        /// </summary>
        public void UpdateMoney(int amount)
        {
            targetMoney = amount;
            displayedMoney = amount;
            _previousMoney = amount;

            if (moneyText != null)
                moneyText.text = $"${amount:N0}";
        }

        /// <summary>
        /// Updates the turn counter text.
        /// Before soft cap: "Turn X" (dynamic game length, no max shown).
        /// After soft cap (turn 25+): "Turn X ⚠️" (penalty warning).
        /// </summary>
        public void UpdateTurn(int current, int max)
        {
            if (turnText == null) return;

            if (current >= Constants.SOFT_CAP_TURN)
                turnText.text = $"Turn {current}/{max}  WARNING";
            else
                turnText.text = $"Turn {current}/{max}";
        }

        /// <summary>
        /// Sets the target FBI risk. The bar lerps toward it in Update().
        /// </summary>
        public void UpdateFBIRisk(float risk)
        {
            targetFBI = Mathf.Clamp01(risk);
        }

        public void UpdateBuildIdentity(string buildIdentity)
        {
            if (buildIdentityText == null)
                return;

            string value = string.IsNullOrWhiteSpace(buildIdentity) ? "Balanced Board" : buildIdentity;
            buildIdentityText.text = $"BUILD · {value}";
        }

        public void UpdatePressureState(BoardPressureType pressure)
        {
            if (pressureText == null)
                return;

            string label = GameClarityFormatter.GetPressureLabel(pressure);
            if (pressure == BoardPressureType.None)
            {
                pressureText.text = "PRESSURE · Board Stable";
                pressureText.color = new Color(0.74f, 0.77f, 0.70f);
                return;
            }

            pressureText.text = $"PRESSURE · {label}";
            pressureText.color = pressure switch
            {
                BoardPressureType.HighLegalRisk => new Color(0.95f, 0.34f, 0.30f),
                BoardPressureType.LowCash => new Color(0.98f, 0.66f, 0.26f),
                BoardPressureType.LowRating => new Color(0.98f, 0.66f, 0.26f),
                BoardPressureType.CapacityShortfall => new Color(0.34f, 0.73f, 1.00f),
                BoardPressureType.WeakQuality => new Color(0.98f, 0.66f, 0.26f),
                BoardPressureType.LowDemand => new Color(0.76f, 0.65f, 0.98f),
                BoardPressureType.StaffInstability => new Color(0.98f, 0.66f, 0.26f),
                _ => Color.white
            };
        }

        // ------------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------------

        /// <summary>
        /// Resolves the money text color each frame:
        ///   1. If a flash is active (gain/loss), lerp flash -> normal over duration
        ///   2. Else if money below 0: fast red pulse (2 Hz)
        ///   3. Else if money below threshold: slow red pulse (1 Hz)
        ///   4. Otherwise: normal gold color
        /// </summary>
        private Color GetMoneyColor()
        {
            // 1. Flash from gain/loss (highest priority)
            if (_moneyFlashTimer > 0f)
            {
                _moneyFlashTimer -= Time.deltaTime;
                float t = Mathf.Clamp01(_moneyFlashTimer / moneyFlashDuration);
                return Color.Lerp(moneyColorNormal, _moneyFlashColor, t);
            }

            // 2. Critical: money at or below zero -- fast pulse (2 Hz)
            int currentMoney = Mathf.RoundToInt(targetMoney);
            if (currentMoney <= 0)
            {
                _moneyPulseTimer += Time.deltaTime;
                float pulse = (Mathf.Sin(_moneyPulseTimer * 2f * Mathf.PI * 2f) + 1f) * 0.5f; // 0..1
                return Color.Lerp(moneyColorNormal, moneyColorLoss, pulse);
            }

            // 3. Low money: slow heartbeat pulse (1 Hz)
            if (currentMoney < moneyLowThreshold)
            {
                _moneyPulseTimer += Time.deltaTime;
                float pulse = (Mathf.Sin(_moneyPulseTimer * 2f * Mathf.PI * 1f) + 1f) * 0.5f; // 0..1
                return Color.Lerp(moneyColorNormal, moneyColorLoss, pulse * 0.6f);
            }

            // 4. Normal
            _moneyPulseTimer = 0f;
            return moneyColorNormal;
        }

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
