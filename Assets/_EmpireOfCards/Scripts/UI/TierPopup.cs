using UnityEngine;
using TMPro;
using EmpireOfCards.Core;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Celebratory popup when the company tier increases.
    /// Same animation pattern as ComboPopup: punch scale -> hold -> fade out.
    /// Subscribes to EventBus.OnCompanyTierChanged.
    /// </summary>
    public class TierPopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text tierText;
        [SerializeField] private TMP_Text subtitleText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Timing")]
        [SerializeField] private float punchDuration = 0.3f;
        [SerializeField] private float displayDuration = 3f;
        [SerializeField] private float fadeOutDuration = 0.8f;

        [Header("Animation")]
        [SerializeField] private float punchScale = 1.6f;

        private enum State { Idle, Punch, Display, FadeOut }
        private State state = State.Idle;
        private float stateTimer;
        private RectTransform rectTransform;

        // Tier display data
        private static readonly string[] TierNames = { "TRADER", "ENTREPRENEUR", "CORPORATION", "CONGLOMERATE" };
        private static readonly string[] TierSubtitles = {
            "",
            "Your business is growing!",
            "You are a serious company now!",
            "Your empire is rising!"
        };
        private static readonly Color[] TierColors = {
            new Color(0.7f, 0.7f, 0.7f),
            new Color(0.3f, 0.8f, 0.4f),
            new Color(0.3f, 0.6f, 1f),
            new Color(1f, 0.8f, 0.2f)
        };

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (canvasGroup != null)
                canvasGroup.alpha = 0f;
        }

        private void OnEnable()
        {
            EventBus.OnCompanyTierChanged += HandleTierChanged;
        }

        private void OnDisable()
        {
            EventBus.OnCompanyTierChanged -= HandleTierChanged;
        }

        private void Update()
        {
            if (state == State.Idle) return;

            stateTimer += Time.deltaTime;

            switch (state)
            {
                case State.Punch:
                    float pt = Mathf.Clamp01(stateTimer / punchDuration);
                    float scale = Mathf.Lerp(punchScale, 1f, pt * pt);
                    rectTransform.localScale = Vector3.one * scale;
                    if (pt >= 1f)
                    {
                        rectTransform.localScale = Vector3.one;
                        state = State.Display;
                        stateTimer = 0f;
                    }
                    break;

                case State.Display:
                    if (stateTimer >= displayDuration)
                    {
                        state = State.FadeOut;
                        stateTimer = 0f;
                    }
                    break;

                case State.FadeOut:
                    float ft = Mathf.Clamp01(stateTimer / fadeOutDuration);
                    if (canvasGroup != null)
                        canvasGroup.alpha = 1f - ft;
                    if (ft >= 1f)
                    {
                        if (canvasGroup != null)
                            canvasGroup.alpha = 0f;
                        state = State.Idle;
                    }
                    break;
            }
        }

        private void HandleTierChanged(CompanyTier newTier)
        {
            int idx = (int)newTier;

            if (tierText != null)
            {
                tierText.text = TierNames[idx];
                tierText.color = TierColors[idx];
            }

            if (subtitleText != null)
                subtitleText.text = TierSubtitles[idx];

            if (canvasGroup != null)
                canvasGroup.alpha = 1f;

            rectTransform.localScale = Vector3.one * punchScale;
            state = State.Punch;
            stateTimer = 0f;
        }

        public void SetReferences(TMP_Text title, TMP_Text subtitle, CanvasGroup cg)
        {
            tierText = title;
            subtitleText = subtitle;
            canvasGroup = cg;
        }
    }
}
