using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Helpers;
using DG.Tweening;

namespace EmpireOfCards.World
{
    /// <summary>
    /// 5-star rating display in 3D world space.
    /// Stars are small cubes that fill/empty based on rating (1.0 - 5.0).
    /// Gold = filled, dark gray = empty. Pulse animation on rating change.
    /// </summary>
    public class RatingStars3D : MonoBehaviour
    {
        // ── Star objects ────────────────────────────────────────────
        Transform[] _stars = new Transform[5];
        Renderer[] _starRenderers = new Renderer[5];
        Material[] _starMaterials = new Material[5];

        // ── Config ──────────────────────────────────────────────────
        const float STAR_SIZE = 0.25f;
        const float STAR_SPACING = 0.4f;
        static readonly Color COL_FILLED = new Color(1f, 0.85f, 0f);
        static readonly Color COL_EMPTY  = new Color(0.25f, 0.25f, 0.3f);
        static readonly Color COL_HALF   = new Color(0.7f, 0.55f, 0f);

        // ── State ───────────────────────────────────────────────────
        float _currentRating;

        // ── Build ───────────────────────────────────────────────────

        public void Build()
        {
            // Title label
            var titleGo = new GameObject("RatingTitle");
            titleGo.transform.SetParent(transform);
            titleGo.transform.localPosition = new Vector3(0f, 0.35f, 0f);
            titleGo.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            var titleTmp = titleGo.AddComponent<TMPro.TextMeshPro>();
            titleTmp.text = "RATING";
            titleTmp.fontSize = 2f;
            titleTmp.color = new Color(0.8f, 0.8f, 0.8f);
            titleTmp.alignment = TMPro.TextAlignmentOptions.Center;
            titleTmp.fontStyle = TMPro.FontStyles.Bold;

            // Create 5 star cubes
            float startX = -(4 * STAR_SPACING) * 0.5f;

            for (int i = 0; i < 5; i++)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = $"Star_{i + 1}";
                go.transform.SetParent(transform);
                go.transform.localPosition = new Vector3(startX + i * STAR_SPACING, 0f, 0f);
                go.transform.localScale = new Vector3(STAR_SIZE, STAR_SIZE, STAR_SIZE);

                // Rotate 45 degrees on Y to look diamond-shaped from above
                go.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);

                var mat = MaterialHelper.CreateEmissive(COL_EMPTY, Color.black);

                var rend = go.GetComponent<Renderer>();
                rend.material = mat;

                _stars[i] = go.transform;
                _starRenderers[i] = rend;
                _starMaterials[i] = mat;
            }

            // Rating value label
            var valueLabelGo = new GameObject("RatingValue");
            valueLabelGo.transform.SetParent(transform);
            valueLabelGo.transform.localPosition = new Vector3(0f, -0.25f, 0f);
            valueLabelGo.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            var valueTmp = valueLabelGo.AddComponent<TMPro.TextMeshPro>();
            valueTmp.text = "3.0";
            valueTmp.fontSize = 2.5f;
            valueTmp.color = COL_FILLED;
            valueTmp.alignment = TMPro.TextAlignmentOptions.Center;
            valueTmp.fontStyle = TMPro.FontStyles.Bold;

            // Set initial state
            SetRating(Constants.STARTING_RATING);
        }

        // ── EventBus ────────────────────────────────────────────────

        void OnEnable()
        {
            EventBus.OnRatingChanged += HandleRatingChanged;
        }

        void OnDisable()
        {
            EventBus.OnRatingChanged -= HandleRatingChanged;
        }

        void HandleRatingChanged(float newRating)
        {
            float oldRating = _currentRating;
            SetRating(newRating);

            // Determine direction and pulse the changed stars
            bool isUp = newRating > oldRating;
            int oldFull = Mathf.FloorToInt(oldRating);
            int newFull = Mathf.FloorToInt(newRating);

            int changedStar = Mathf.Clamp(isUp ? newFull : oldFull, 0, 4);
            if (_stars[changedStar] != null)
                DOTweenAnimations.RatingStarPulse(_stars[changedStar], isUp);
        }

        // ── Set rating ──────────────────────────────────────────────

        void SetRating(float rating)
        {
            _currentRating = Mathf.Clamp(rating, Constants.RATING_MIN, Constants.RATING_MAX);

            for (int i = 0; i < 5; i++)
            {
                float starThreshold = i + 1f;
                Color targetColor;
                Color emissionColor;

                if (_currentRating >= starThreshold)
                {
                    // Full star
                    targetColor = COL_FILLED;
                    emissionColor = COL_FILLED * 0.5f;
                }
                else if (_currentRating >= starThreshold - 0.5f)
                {
                    // Half star
                    targetColor = COL_HALF;
                    emissionColor = COL_HALF * 0.3f;
                }
                else
                {
                    // Empty star
                    targetColor = COL_EMPTY;
                    emissionColor = Color.black;
                }

                if (_starMaterials[i] != null)
                {
                    _starMaterials[i].DOColor(targetColor, 0.3f);
                    _starMaterials[i].DOColor(emissionColor, "_EmissionColor", 0.3f);
                }
            }

            // Update value label
            var valueLabel = transform.Find("RatingValue");
            if (valueLabel != null)
            {
                var tmp = valueLabel.GetComponent<TMPro.TextMeshPro>();
                if (tmp != null)
                    tmp.text = $"{_currentRating:F1}";
            }
        }
    }
}
