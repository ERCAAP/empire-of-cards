using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Core;

namespace EmpireOfCards.UI
{
    public class TopBarUI : MonoBehaviour
    {
        // ── Stat labels ─────────────────────────────────────────────
        TextMeshProUGUI _cashText;
        TextMeshProUGUI _demandText;
        TextMeshProUGUI _capacityText;
        TextMeshProUGUI _qualityText;
        TextMeshProUGUI _ratingText;
        TextMeshProUGUI _stabilityText;
        TextMeshProUGUI _legalText;
        TextMeshProUGUI _shareText;

        // ── Game state labels ───────────────────────────────────────
        TextMeshProUGUI _actionsText;
        TextMeshProUGUI _turnText;
        TextMeshProUGUI _seasonText;
        TextMeshProUGUI _eraText;

        // ── Colors ──────────────────────────────────────────────────
        static readonly Color COL_GOOD    = new Color(0.30f, 0.85f, 0.30f);
        static readonly Color COL_BAD     = new Color(0.90f, 0.20f, 0.20f);
        static readonly Color COL_WARNING = new Color(0.95f, 0.85f, 0.15f);
        static readonly Color COL_NEUTRAL = Color.white;

        // ── Build (called by HUDBuilder) ────────────────────────────

        public void Build(Transform canvasTransform)
        {
            // Top bar panel
            var panelGo = new GameObject("TopBarPanel");
            panelGo.transform.SetParent(canvasTransform, false);

            var panelRect = panelGo.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0f, 1f);
            panelRect.anchorMax = new Vector2(1f, 1f);
            panelRect.pivot = new Vector2(0.5f, 1f);
            panelRect.sizeDelta = new Vector2(0f, 50f);

            var panelImage = panelGo.AddComponent<Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.15f, 0.85f);

            var layout = panelGo.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 12f;
            layout.padding = new RectOffset(10, 10, 5, 5);
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = false;
            layout.childControlHeight = true;

            // Stats row
            _cashText      = CreateStatLabel(panelGo.transform, "Cash", "$300");
            _demandText    = CreateStatLabel(panelGo.transform, "Demand", "2.0");
            _capacityText  = CreateStatLabel(panelGo.transform, "Capacity", "3.0");
            _qualityText   = CreateStatLabel(panelGo.transform, "Quality", "3.0");
            _ratingText    = CreateStatLabel(panelGo.transform, "Rating", "3.0");
            _stabilityText = CreateStatLabel(panelGo.transform, "Stability", "5.0");
            _legalText     = CreateStatLabel(panelGo.transform, "Legal", "0");
            _shareText     = CreateStatLabel(panelGo.transform, "Share", "15%");

            // Separator
            CreateSeparator(panelGo.transform);

            // Game state
            _actionsText = CreateStatLabel(panelGo.transform, "Actions", "2");
            _turnText    = CreateStatLabel(panelGo.transform, "Turn", "1");
            _seasonText  = CreateStatLabel(panelGo.transform, "Season", "Spring");
            _eraText     = CreateStatLabel(panelGo.transform, "Era", "Garage");
        }

        // ── EventBus subscriptions ──────────────────────────────────

        void OnEnable()
        {
            EventBus.OnMoneyChanged += HandleMoney;
            EventBus.OnDemandChanged += HandleDemand;
            EventBus.OnCapacityChanged += HandleCapacity;
            EventBus.OnQualityChanged += HandleQuality;
            EventBus.OnRatingChanged += HandleRating;
            EventBus.OnStaffStabilityChanged += HandleStability;
            EventBus.OnLegalRiskChanged += HandleLegal;
            EventBus.OnMarketShareChanged += HandleShare;
            EventBus.OnActionsChanged += HandleActions;
            EventBus.OnTurnStarted += HandleTurn;
            EventBus.OnSeasonChanged += HandleSeason;
            EventBus.OnEraChanged += HandleEra;
        }

        void OnDisable()
        {
            EventBus.OnMoneyChanged -= HandleMoney;
            EventBus.OnDemandChanged -= HandleDemand;
            EventBus.OnCapacityChanged -= HandleCapacity;
            EventBus.OnQualityChanged -= HandleQuality;
            EventBus.OnRatingChanged -= HandleRating;
            EventBus.OnStaffStabilityChanged -= HandleStability;
            EventBus.OnLegalRiskChanged -= HandleLegal;
            EventBus.OnMarketShareChanged -= HandleShare;
            EventBus.OnActionsChanged -= HandleActions;
            EventBus.OnTurnStarted -= HandleTurn;
            EventBus.OnSeasonChanged -= HandleSeason;
            EventBus.OnEraChanged -= HandleEra;
        }

        // ── Handlers ────────────────────────────────────────────────

        void HandleMoney(int val)
        {
            SetText(_cashText, $"${val}", val >= 100 ? COL_GOOD : val >= 30 ? COL_WARNING : COL_BAD);
        }

        void HandleDemand(float val)
        {
            SetText(_demandText, $"D:{val:F1}", val >= 5f ? COL_GOOD : val >= 3f ? COL_WARNING : COL_BAD);
        }

        void HandleCapacity(float val)
        {
            SetText(_capacityText, $"C:{val:F1}", val >= 5f ? COL_GOOD : val >= 3f ? COL_WARNING : COL_BAD);
        }

        void HandleQuality(float val)
        {
            SetText(_qualityText, $"Q:{val:F1}", val >= 5f ? COL_GOOD : val >= 3f ? COL_WARNING : COL_BAD);
        }

        void HandleRating(float val)
        {
            SetText(_ratingText, $"R:{val:F1}", val >= 4f ? COL_GOOD : val >= 2.5f ? COL_WARNING : COL_BAD);
        }

        void HandleStability(float val)
        {
            SetText(_stabilityText, $"S:{val:F1}", val >= 5f ? COL_GOOD : val >= 3f ? COL_WARNING : COL_BAD);
        }

        void HandleLegal(float val)
        {
            SetText(_legalText, $"L:{val:F0}", val <= 20f ? COL_GOOD : val <= 50f ? COL_WARNING : COL_BAD);
        }

        void HandleShare(float player, float rival)
        {
            SetText(_shareText, $"{player:F0}%", player >= 40f ? COL_GOOD : player >= 20f ? COL_WARNING : COL_BAD);
        }

        void HandleActions(int val)
        {
            SetText(_actionsText, $"Act:{val}", val > 0 ? COL_GOOD : COL_BAD);
        }

        void HandleTurn(int turn)
        {
            SetText(_turnText, $"T:{turn}/{Constants.MAX_TURNS}", COL_NEUTRAL);
        }

        void HandleSeason(SeasonType season)
        {
            SetText(_seasonText, season.ToString(), COL_NEUTRAL);
        }

        void HandleEra(Era era)
        {
            SetText(_eraText, era.ToString(), COL_NEUTRAL);
        }

        // ── Helpers ─────────────────────────────────────────────────

        void SetText(TextMeshProUGUI label, string text, Color color)
        {
            if (label == null) return;
            label.text = text;
            label.color = color;
        }

        TextMeshProUGUI CreateStatLabel(Transform parent, string name, string defaultText)
        {
            var go = new GameObject($"Stat_{name}");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(70f, 40f);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = defaultText;
            tmp.fontSize = 14f;
            tmp.color = COL_NEUTRAL;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = false;

            return tmp;
        }

        void CreateSeparator(Transform parent)
        {
            var go = new GameObject("Separator");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(2f, 30f);

            var img = go.AddComponent<Image>();
            img.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }
}
