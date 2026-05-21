using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Core;

namespace EmpireOfCards.UI
{
    public class DashboardPanel : MonoBehaviour
    {
        // ── UI refs ─────────────────────────────────────────────────
        GameObject _root;
        TextMeshProUGUI _titleText;
        TextMeshProUGUI _capacityWarning;
        TextMeshProUGUI _hygieneWarning;
        TextMeshProUGUI _ratingWarning;
        TextMeshProUGUI _cashWarning;
        TextMeshProUGUI _priorityText;

        // ── Tracked state ───────────────────────────────────────────
        float _demand;
        float _capacity;
        float _hygiene;
        float _rating;
        int _cash;

        // ── Colors ──────────────────────────────────────────────────
        static readonly Color COL_BG = new Color(0.10f, 0.10f, 0.15f, 0.92f);
        static readonly Color COL_GOLD = new Color(0.95f, 0.80f, 0.20f);
        static readonly Color COL_RED = new Color(0.90f, 0.20f, 0.20f);
        static readonly Color COL_YELLOW = new Color(0.95f, 0.85f, 0.15f);
        static readonly Color COL_ORANGE = new Color(0.95f, 0.55f, 0.15f);
        static readonly Color COL_GREEN = new Color(0.30f, 0.85f, 0.30f);

        // ── Build ───────────────────────────────────────────────────

        public void Build(Transform canvasTransform)
        {
            _root = new GameObject("DashboardPanel");
            _root.transform.SetParent(canvasTransform, false);

            var rootRect = _root.AddComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 0.5f);
            rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.sizeDelta = new Vector2(420f, 300f);

            var bg = _root.AddComponent<Image>();
            bg.color = COL_BG;

            // Title
            _titleText = CreateText(_root.transform, "DURUM RAPORU - Tur 1", 22f,
                new Vector2(0f, 115f), COL_GOLD, FontStyles.Bold);

            // Warning indicators
            _capacityWarning = CreateWarningLine(_root.transform, new Vector2(0f, 60f));
            _hygieneWarning = CreateWarningLine(_root.transform, new Vector2(0f, 25f));
            _ratingWarning = CreateWarningLine(_root.transform, new Vector2(0f, -10f));
            _cashWarning = CreateWarningLine(_root.transform, new Vector2(0f, -45f));

            // Priority text
            _priorityText = CreateText(_root.transform, "", 14f,
                new Vector2(0f, -100f), Color.white, FontStyles.Italic,
                new Vector2(380f, 40f));

            _root.SetActive(false);
        }

        // ── EventBus ────────────────────────────────────────────────

        void OnEnable()
        {
            EventBus.OnPhaseStarted += HandlePhaseStarted;
            EventBus.OnPhaseEnded += HandlePhaseEnded;
            EventBus.OnDemandChanged += d => _demand = d;
            EventBus.OnCapacityChanged += c => _capacity = c;
            EventBus.OnHygieneChanged += h => _hygiene = h;
            EventBus.OnRatingChanged += r => _rating = r;
            EventBus.OnMoneyChanged += m => _cash = m;
        }

        void OnDisable()
        {
            EventBus.OnPhaseStarted -= HandlePhaseStarted;
            EventBus.OnPhaseEnded -= HandlePhaseEnded;
        }

        // ── Handlers ────────────────────────────────────────────────

        void HandlePhaseStarted(TurnPhase phase)
        {
            if (phase != TurnPhase.PlanningPhase) return;

            int turn = GameManager.Instance != null ? GameManager.Instance.CurrentTurn : 0;
            UpdateDashboard(turn);
            Show();
        }

        void HandlePhaseEnded(TurnPhase phase)
        {
            if (phase == TurnPhase.PlanningPhase)
                Hide();
        }

        // ── Update Dashboard ────────────────────────────────────────

        void UpdateDashboard(int turn)
        {
            if (_titleText != null)
                _titleText.text = $"DURUM RAPORU - Tur {turn}";

            string priority = "";
            int priorityWeight = 0;

            // Capacity check
            bool capacityLow = _demand > _capacity;
            SetWarning(_capacityWarning, capacityLow,
                "! Kapasite yetersiz (talep > kapasite)", COL_RED);
            if (capacityLow && priorityWeight < 4)
            {
                priority = "Bu tur onceligin: Kapasite artirmak";
                priorityWeight = 4;
            }

            // Hygiene check
            bool hygieneLow = _hygiene < Constants.HYGIENE_DIRTY_THRESHOLD;
            SetWarning(_hygieneWarning, hygieneLow,
                "! Hijyen dusuk (< 5)", COL_YELLOW);
            if (hygieneLow && priorityWeight < 3)
            {
                priority = "Bu tur onceligin: Hijyen iyilestirmek";
                priorityWeight = 3;
            }

            // Rating check
            bool ratingLow = _rating < Constants.ORGANIC_DEMAND_THRESHOLD;
            SetWarning(_ratingWarning, ratingLow,
                "! Rating dusuk (< 3.5)", COL_ORANGE);
            if (ratingLow && priorityWeight < 2)
            {
                priority = "Bu tur onceligin: Rating yukseltmek";
                priorityWeight = 2;
            }

            // Cash check
            bool cashLow = _cash < 100;
            SetWarning(_cashWarning, cashLow,
                "! Para azaliyor (< $100)", COL_RED);
            if (cashLow && priorityWeight < 1)
            {
                priority = "Bu tur onceligin: Geliri artirmak";
            }

            if (string.IsNullOrEmpty(priority))
                priority = "Durumun iyi. Buyumeye odaklan.";

            if (_priorityText != null)
                _priorityText.text = priority;
        }

        // ── Show / Hide ─────────────────────────────────────────────

        public void Show()
        {
            if (_root != null) _root.SetActive(true);
        }

        public void Hide()
        {
            if (_root != null) _root.SetActive(false);
        }

        // ── Helpers ─────────────────────────────────────────────────

        void SetWarning(TextMeshProUGUI label, bool active, string text, Color color)
        {
            if (label == null) return;
            label.gameObject.SetActive(active);
            label.text = text;
            label.color = active ? color : COL_GREEN;
        }

        TextMeshProUGUI CreateWarningLine(Transform parent, Vector2 pos)
        {
            return CreateText(parent, "", 15f, pos, COL_GREEN,
                FontStyles.Normal, new Vector2(380f, 28f));
        }

        TextMeshProUGUI CreateText(Transform parent, string text, float size,
            Vector2 pos, Color color, FontStyles style = FontStyles.Normal,
            Vector2? customSize = null)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = customSize ?? new Vector2(400f, 40f);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.fontStyle = style;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = true;
            return tmp;
        }
    }
}
