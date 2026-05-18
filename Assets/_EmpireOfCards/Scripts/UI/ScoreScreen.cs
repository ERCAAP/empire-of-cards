using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Data container for end-of-run scoring.
    /// </summary>
    [Serializable]
    public class ScoreData
    {
        public int territoryScore;
        public int moneyScore;
        public int comboScore;
        public int businessScore;
        public int earlyFinishBonus;
        public int fbiEvasionBonus;
        public int winBonus;
        public int totalScore;
        public string grade;
    }

    /// <summary>
    /// End-of-run score screen that tallies up the player's performance
    /// with an animated counter.
    /// </summary>
    public class ScoreScreen : MonoBehaviour
    {
        [Header("Score Texts")]
        [SerializeField] private TMP_Text totalScoreText;
        [SerializeField] private TMP_Text territoryScoreText;
        [SerializeField] private TMP_Text moneyScoreText;
        [SerializeField] private TMP_Text comboScoreText;
        [SerializeField] private TMP_Text businessScoreText;
        [SerializeField] private TMP_Text earlyFinishText;
        [SerializeField] private TMP_Text gradeText;

        [Header("Buttons")]
        [SerializeField] private Button playAgainButton;

        [Header("Animation")]
        [SerializeField] private float countDuration = 1.5f;
        [SerializeField] private float lineDelay = 0.3f;

        /// <summary>
        /// Called by the Play Again button in the scene.
        /// </summary>
        public event Action OnPlayAgainClicked;

        private void Awake()
        {
            if (playAgainButton != null)
            {
                playAgainButton.onClick.AddListener(() => OnPlayAgainClicked?.Invoke());
            }
        }

        /// <summary>
        /// Populates and animates the score screen with the given data.
        /// </summary>
        public void Show(ScoreData data)
        {
            if (data == null)
                return;

            data.grade = CalculateGrade(data.totalScore);

            // Set final values immediately as fallback
            SetScoreTexts(data);

            // Start animated count-up
            StartCoroutine(AnimateScoreCount(data));
        }

        /// <summary>
        /// Determines the letter grade based on total score.
        /// </summary>
        public string CalculateGrade(int score)
        {
            if (score >= 5000) return "S";
            if (score >= 4000) return "A";
            if (score >= 3000) return "B";
            if (score >= 2000) return "C";
            if (score >= 1000) return "D";
            return "F";
        }

        /// <summary>
        /// Animates each score line counting up from zero one by one.
        /// </summary>
        private IEnumerator AnimateScoreCount(ScoreData data)
        {
            // Reset all to zero
            SetText(territoryScoreText, 0);
            SetText(moneyScoreText, 0);
            SetText(comboScoreText, 0);
            SetText(businessScoreText, 0);
            SetText(earlyFinishText, 0);
            SetText(totalScoreText, 0);
            if (gradeText != null) gradeText.text = "?";

            yield return StartCoroutine(CountUpLine(territoryScoreText, data.territoryScore));
            yield return new WaitForSeconds(lineDelay);

            yield return StartCoroutine(CountUpLine(moneyScoreText, data.moneyScore));
            yield return new WaitForSeconds(lineDelay);

            yield return StartCoroutine(CountUpLine(comboScoreText, data.comboScore));
            yield return new WaitForSeconds(lineDelay);

            yield return StartCoroutine(CountUpLine(businessScoreText, data.businessScore));
            yield return new WaitForSeconds(lineDelay);

            yield return StartCoroutine(CountUpLine(earlyFinishText, data.earlyFinishBonus));
            yield return new WaitForSeconds(lineDelay);

            yield return StartCoroutine(CountUpLine(totalScoreText, data.totalScore));
            yield return new WaitForSeconds(lineDelay);

            // Reveal grade
            if (gradeText != null)
            {
                gradeText.text = data.grade;
                gradeText.transform.localScale = Vector3.one * 1.5f;
                // Quick punch scale
                float elapsed = 0f;
                while (elapsed < 0.3f)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / 0.3f;
                    gradeText.transform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, t);
                    yield return null;
                }
                gradeText.transform.localScale = Vector3.one;
            }
        }

        private IEnumerator CountUpLine(TMP_Text textField, int targetValue)
        {
            if (textField == null)
                yield break;

            float elapsed = 0f;
            while (elapsed < countDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / countDuration);
                int current = Mathf.RoundToInt(Mathf.Lerp(0, targetValue, t));
                textField.text = current.ToString("N0");
                yield return null;
            }

            textField.text = targetValue.ToString("N0");
        }

        private void SetScoreTexts(ScoreData data)
        {
            SetText(territoryScoreText, data.territoryScore);
            SetText(moneyScoreText, data.moneyScore);
            SetText(comboScoreText, data.comboScore);
            SetText(businessScoreText, data.businessScore);
            SetText(earlyFinishText, data.earlyFinishBonus);
            SetText(totalScoreText, data.totalScore);
            if (gradeText != null) gradeText.text = data.grade;
        }

        private void SetText(TMP_Text field, int value)
        {
            if (field != null)
                field.text = value.ToString("N0");
        }

        private void OnDestroy()
        {
            if (playAgainButton != null)
            {
                playAgainButton.onClick.RemoveAllListeners();
            }
        }
    }
}
