using System.Collections;
using UnityEngine;
using TMPro;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.UI
{
    public class IncomeCascadeAnimator : MonoBehaviour
    {
        [Header("Income Cascade")]
        [SerializeField] private float cascadeStepDuration = 0.4f;
        [SerializeField] private float cascadeNetHoldDuration = 0.8f;
        [SerializeField] private float cascadeSlideDistance = 200f;
        [SerializeField] private int cascadeFontSize = 36;
        [SerializeField] private int cascadeNetFontSize = 48;
        private Coroutine _activeCascade;

        private void OnEnable()
        {
            EventBus.OnIncomeBreakdown += HandleIncomeBreakdown;
        }

        private void OnDisable()
        {
            EventBus.OnIncomeBreakdown -= HandleIncomeBreakdown;
        }

        private void HandleIncomeBreakdown(IncomeBreakdown breakdown)
        {
            if (breakdown == null || breakdown.steps.Count == 0) return;
            Play(breakdown);
        }

        public void Play(IncomeBreakdown breakdown)
        {
            if (breakdown == null || breakdown.steps.Count == 0) return;

            if (_activeCascade != null)
                StopCoroutine(_activeCascade);

            _activeCascade = StartCoroutine(IncomeCascadeRoutine(breakdown));
        }

        /// <summary>
        /// Shows each income component one at a time as floating text near the
        /// top bar, then finishes with the NET total. Temporary TMP_Text objects
        /// are spawned on the HUD canvas, animated, and destroyed.
        /// </summary>
        private IEnumerator IncomeCascadeRoutine(IncomeBreakdown breakdown)
        {
            // Find the HUD canvas (IncomeCascadeAnimator lives on the canvas GO)
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = GetComponent<Canvas>();
            }
            if (canvas == null)
            {
                Debug.LogWarning("[IncomeCascadeAnimator] No canvas found for income cascade.");
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

            Object.Destroy(go);
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

            Object.Destroy(go);
        }
    }
}
