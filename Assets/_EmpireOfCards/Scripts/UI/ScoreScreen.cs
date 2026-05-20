using System;
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
        public int customerShareScore;
        public int moneyScore;
        public int comboScore;
        public int businessScore;
        public int tierScoreBonus;
        public int earlyFinishBonus;
        public int fbiEvasionBonus;
        public int winBonus;
        public int totalScore;
        public string grade;
    }

    /// <summary>
    /// End-of-run score screen. Animated count-up for each score line,
    /// grade reveal at the end. All animation via Update() polling.
    /// Grade: S (5000+), A (3000+), B (2000+), C (1000+), D (500+), F (<500).
    /// </summary>
    public class ScoreScreen : MonoBehaviour
    {
        [Header("Score Texts")]
        [SerializeField] private TMP_Text customerShareScoreText;
        [SerializeField] private TMP_Text moneyScoreText;
        [SerializeField] private TMP_Text comboScoreText;
        [SerializeField] private TMP_Text businessScoreText;
        [SerializeField] private TMP_Text tierBonusText;
        [SerializeField] private TMP_Text earlyFinishText;
        [SerializeField] private TMP_Text fbiEvasionText;
        [SerializeField] private TMP_Text winBonusText;
        [SerializeField] private TMP_Text totalScoreText;
        [SerializeField] private TMP_Text gradeText;

        [Header("Buttons")]
        [SerializeField] private Button playAgainButton;
        [SerializeField] private Button menuButton;

        [Header("Animation")]
        [SerializeField] private float countDuration = 1.0f;
        [SerializeField] private float lineDelay = 0.25f;
        [SerializeField] private float gradePunchScale = 1.5f;
        [SerializeField] private float gradePunchDuration = 0.3f;

        // Events
        public event Action OnPlayAgainClicked;
        public event Action OnMenuClicked;

        // Animation state
        private ScoreData scoreData;
        private ScoreLine[] lines;
        private int currentLineIndex;
        private float lineTimer;
        private float delayTimer;
        private bool isAnimating;

        // Grade punch
        private bool gradeRevealed;
        private float gradeTimer;

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void Awake()
        {
            if (playAgainButton != null)
                playAgainButton.onClick.AddListener(() => OnPlayAgainClicked?.Invoke());

            if (menuButton != null)
                menuButton.onClick.AddListener(() => OnMenuClicked?.Invoke());
        }

        private void Update()
        {
            if (!isAnimating)
            {
                // Grade punch scale animation after all lines are done
                if (gradeRevealed && gradeText != null)
                {
                    gradeTimer += Time.deltaTime;
                    float t = Mathf.Clamp01(gradeTimer / gradePunchDuration);
                    float scale = Mathf.Lerp(gradePunchScale, 1f, t);
                    gradeText.transform.localScale = Vector3.one * scale;

                    if (t >= 1f)
                    {
                        gradeText.transform.localScale = Vector3.one;
                        gradeRevealed = false; // done animating
                    }
                }
                return;
            }

            if (lines == null || currentLineIndex >= lines.Length)
            {
                // All lines counted -- reveal grade
                isAnimating = false;
                RevealGrade();
                return;
            }

            // Inter-line delay
            if (delayTimer > 0f)
            {
                delayTimer -= Time.deltaTime;
                return;
            }

            // Count-up current line
            lineTimer += Time.deltaTime;
            float lineT = Mathf.Clamp01(lineTimer / countDuration);
            int displayValue = Mathf.RoundToInt(Mathf.Lerp(0f, lines[currentLineIndex].targetValue, lineT));

            if (lines[currentLineIndex].textField != null)
                lines[currentLineIndex].textField.text = displayValue.ToString("N0");

            if (lineT >= 1f)
            {
                // Finalize this line
                if (lines[currentLineIndex].textField != null)
                    lines[currentLineIndex].textField.text = lines[currentLineIndex].targetValue.ToString("N0");

                currentLineIndex++;
                lineTimer = 0f;
                delayTimer = lineDelay;
            }
        }

        // ------------------------------------------------------------------
        // Public
        // ------------------------------------------------------------------

        /// <summary>
        /// Populates and starts the animated score count-up.
        /// </summary>
        public void Show(ScoreData data)
        {
            if (data == null)
                return;

            scoreData = data;
            scoreData.grade = CalculateGrade(scoreData.totalScore);

            // Build the ordered line array
            lines = new ScoreLine[]
            {
                new ScoreLine { textField = customerShareScoreText, targetValue = data.customerShareScore },
                new ScoreLine { textField = moneyScoreText,     targetValue = data.moneyScore },
                new ScoreLine { textField = comboScoreText,     targetValue = data.comboScore },
                new ScoreLine { textField = businessScoreText,  targetValue = data.businessScore },
                new ScoreLine { textField = tierBonusText,      targetValue = data.tierScoreBonus },
                new ScoreLine { textField = earlyFinishText,    targetValue = data.earlyFinishBonus },
                new ScoreLine { textField = fbiEvasionText,     targetValue = data.fbiEvasionBonus },
                new ScoreLine { textField = winBonusText,       targetValue = data.winBonus },
                new ScoreLine { textField = totalScoreText,     targetValue = data.totalScore },
            };

            // Zero out all fields
            foreach (var line in lines)
            {
                if (line.textField != null)
                    line.textField.text = "0";
            }

            if (gradeText != null)
                gradeText.text = "?";

            currentLineIndex = 0;
            lineTimer = 0f;
            delayTimer = 0f;
            isAnimating = true;
            gradeRevealed = false;
        }

        // ------------------------------------------------------------------
        // Internal
        // ------------------------------------------------------------------

        /// <summary>
        /// Determines the letter grade based on total score.
        /// </summary>
        public static string CalculateGrade(int score)
        {
            if (score >= 5000) return "S";
            if (score >= 3000) return "A";
            if (score >= 2000) return "B";
            if (score >= 1000) return "C";
            if (score >= 500)  return "D";
            return "F";
        }

        private void RevealGrade()
        {
            if (gradeText == null || scoreData == null)
                return;

            gradeText.text = scoreData.grade;
            gradeText.transform.localScale = Vector3.one * gradePunchScale;
            gradeRevealed = true;
            gradeTimer = 0f;
        }

        private void OnDestroy()
        {
            if (playAgainButton != null)
                playAgainButton.onClick.RemoveAllListeners();

            if (menuButton != null)
                menuButton.onClick.RemoveAllListeners();
        }

        // Helper struct
        private struct ScoreLine
        {
            public TMP_Text textField;
            public int targetValue;
        }
    }
}
