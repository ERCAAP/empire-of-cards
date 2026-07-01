using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using EmpireOfCards.Core;

namespace EmpireOfCards.UI
{
    public class GameOverScreen : MonoBehaviour
    {
        // ── UI refs ─────────────────────────────────────────────────
        GameObject _root;
        TextMeshProUGUI _resultTitle;
        TextMeshProUGUI _reasonText;
        TextMeshProUGUI _scoreBreakdown;
        TextMeshProUGUI _gradeText;
        Button _replayButton;
        Button _menuButton;

        // ── Colors ──────────────────────────────────────────────────
        static readonly Color COL_BG = new Color(0.06f, 0.06f, 0.10f, 0.97f);
        static readonly Color COL_WIN = new Color(0.95f, 0.80f, 0.20f);
        static readonly Color COL_LOSE = new Color(0.85f, 0.20f, 0.20f);
        static readonly Color COL_GREEN = new Color(0.20f, 0.65f, 0.25f);
        static readonly Color COL_BLUE = new Color(0.20f, 0.45f, 0.75f);
        static readonly Color COL_GRAY = new Color(0.7f, 0.7f, 0.7f);
        static readonly Color COL_GOLD = new Color(0.95f, 0.80f, 0.20f);

        // ── Grade colors ────────────────────────────────────────────
        static readonly Color COL_GRADE_S = new Color(1f, 0.84f, 0f);
        static readonly Color COL_GRADE_A = new Color(0.30f, 0.85f, 0.30f);
        static readonly Color COL_GRADE_B = new Color(0.20f, 0.60f, 0.85f);
        static readonly Color COL_GRADE_C = new Color(0.85f, 0.85f, 0.15f);
        static readonly Color COL_GRADE_D = new Color(0.85f, 0.50f, 0.15f);
        static readonly Color COL_GRADE_F = new Color(0.85f, 0.20f, 0.20f);

        // ── Build ───────────────────────────────────────────────────

        public void Build(Transform canvasTransform)
        {
            // Full-screen overlay
            _root = new GameObject("GameOverScreen");
            _root.transform.SetParent(canvasTransform, false);

            var rootRect = _root.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.sizeDelta = Vector2.zero;

            var bg = _root.AddComponent<Image>();
            bg.color = COL_BG;

            // Result title (WIN / LOSE)
            _resultTitle = CreateText(_root.transform, "OYUN BITTI", 52f,
                new Vector2(0f, 300f), COL_WIN, FontStyles.Bold,
                new Vector2(800f, 70f));

            // Reason text
            _reasonText = CreateText(_root.transform, "", 20f,
                new Vector2(0f, 230f), COL_GRAY, FontStyles.Italic,
                new Vector2(700f, 40f));

            // Divider line
            CreateDivider(_root.transform, new Vector2(0f, 195f));

            // Grade display (large letter)
            _gradeText = CreateText(_root.transform, "B", 96f,
                new Vector2(0f, 95f), COL_GRADE_B, FontStyles.Bold,
                new Vector2(200f, 120f));

            // Score breakdown panel
            var breakdownPanel = CreatePanel(_root.transform, 500f, 200f,
                new Color(0.10f, 0.10f, 0.16f, 0.9f), new Vector2(0f, -60f));

            _scoreBreakdown = CreateText(breakdownPanel.transform, "", 16f,
                Vector2.zero, Color.white, FontStyles.Normal,
                new Vector2(460f, 180f));
            _scoreBreakdown.alignment = TextAlignmentOptions.TopLeft;

            // Buttons
            _replayButton = CreateMenuButton(_root.transform, "TEKRAR OYNA",
                new Vector2(-120f, -220f), COL_GREEN);
            _replayButton.onClick.AddListener(OnReplayClicked);

            _menuButton = CreateMenuButton(_root.transform, "ANA MENU",
                new Vector2(120f, -220f), COL_BLUE);
            _menuButton.onClick.AddListener(OnMenuClicked);

            _root.SetActive(false);
        }

        // ── EventBus ────────────────────────────────────────────────

        void OnEnable()
        {
            EventBus.OnGameOver += HandleGameOver;
        }

        void OnDisable()
        {
            EventBus.OnGameOver -= HandleGameOver;
        }

        // ── Handlers ────────────────────────────────────────────────

        void HandleGameOver(bool won, string reason)
        {
            int turn = GameManager.Instance != null ? GameManager.Instance.CurrentTurn : 0;
            float marketShare = 0f;
            float rating = 0f;
            int cash = 0;

            if (GameManager.Instance != null)
            {
                var res = GameManager.Instance.Resources;
                if (res != null)
                {
                    marketShare = res.GetMarketShare();
                    rating = res.GetRating();
                    cash = res.GetMoney();
                }
            }

            int score = CalculateScore(won, turn, marketShare, rating, cash);
            ScoreGrade grade = GetGrade(score);

            Show(won, reason, score, grade, turn, marketShare, rating, cash);
        }

        // ── Show / Hide ─────────────────────────────────────────────

        public void Show(bool won, string reason, int totalScore, ScoreGrade grade,
            int turn, float marketShare, float rating, int cash)
        {
            // Title
            if (_resultTitle != null)
            {
                _resultTitle.text = won ? "ZAFER!" : "MAGLUBIYET";
                _resultTitle.color = won ? COL_WIN : COL_LOSE;
            }

            // Reason
            if (_reasonText != null)
                _reasonText.text = reason;

            // Grade
            if (_gradeText != null)
            {
                _gradeText.text = grade.ToString();
                _gradeText.color = GetGradeColor(grade);
            }

            // Score breakdown
            if (_scoreBreakdown != null)
            {
                int shareScore = (int)(marketShare * Constants.SCORE_MARKET_SHARE);
                int cashScore = cash * Constants.SCORE_CASH / 100;
                int ratingScore = (int)(rating * Constants.SCORE_RATING);
                int eraScore = GetEraScore(turn);
                int fastBonus = turn <= Constants.FAST_FINISH_TURN && won
                    ? Constants.SCORE_FAST_FINISH : 0;

                _scoreBreakdown.text =
                    $"<color=#F2CC33>SKOR DETAYI</color>\n\n" +
                    $"  Pazar Payi ({marketShare:F0}%)     <color=#4DDB4D>+{shareScore}</color>\n" +
                    $"  Kasa (${cash})            <color=#4DDB4D>+{cashScore}</color>\n" +
                    $"  Rating ({rating:F1})          <color=#4DDB4D>+{ratingScore}</color>\n" +
                    $"  Era Bonusu              <color=#4DDB4D>+{eraScore}</color>\n" +
                    (fastBonus > 0
                        ? $"  Hizli Bitis Bonusu      <color=#FFD700>+{fastBonus}</color>\n"
                        : "") +
                    $"\n  <color=#F2CC33>TOPLAM: {totalScore}</color>";
            }

            if (_root != null) _root.SetActive(true);
        }

        public void Hide()
        {
            if (_root != null) _root.SetActive(false);
        }

        // ── Score Calculation ───────────────────────────────────────

        int CalculateScore(bool won, int turn, float marketShare, float rating, int cash)
        {
            int score = 0;
            score += (int)(marketShare * Constants.SCORE_MARKET_SHARE);
            score += cash * Constants.SCORE_CASH / 100;
            score += (int)(rating * Constants.SCORE_RATING);
            score += GetEraScore(turn);

            if (turn <= Constants.FAST_FINISH_TURN && won)
                score += Constants.SCORE_FAST_FINISH;

            return score;
        }

        int GetEraScore(int turn)
        {
            if (turn > Constants.ERA_3_END) return Constants.SCORE_ERA_REACHED * 4;
            if (turn > Constants.ERA_2_END) return Constants.SCORE_ERA_REACHED * 3;
            if (turn > Constants.ERA_1_END) return Constants.SCORE_ERA_REACHED * 2;
            return Constants.SCORE_ERA_REACHED;
        }

        ScoreGrade GetGrade(int score)
        {
            if (score >= Constants.GRADE_S_THRESHOLD) return ScoreGrade.S;
            if (score >= Constants.GRADE_A_THRESHOLD) return ScoreGrade.A;
            if (score >= Constants.GRADE_B_THRESHOLD) return ScoreGrade.B;
            if (score >= Constants.GRADE_C_THRESHOLD) return ScoreGrade.C;
            if (score >= Constants.GRADE_D_THRESHOLD) return ScoreGrade.D;
            return ScoreGrade.F;
        }

        Color GetGradeColor(ScoreGrade grade)
        {
            switch (grade)
            {
                case ScoreGrade.S: return COL_GRADE_S;
                case ScoreGrade.A: return COL_GRADE_A;
                case ScoreGrade.B: return COL_GRADE_B;
                case ScoreGrade.C: return COL_GRADE_C;
                case ScoreGrade.D: return COL_GRADE_D;
                default:           return COL_GRADE_F;
            }
        }

        // ── Button Callbacks ────────────────────────────────────────

        void OnReplayClicked()
        {
            EventBus.ClearAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        void OnMenuClicked()
        {
            EventBus.ClearAll();
            SceneManager.LoadScene("MainMenu");
        }

        // ── UI Helpers ──────────────────────────────────────────────

        GameObject CreatePanel(Transform parent, float w, float h, Color color,
            Vector2 pos)
        {
            var go = new GameObject("Panel");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(w, h);
            var img = go.AddComponent<Image>();
            img.color = color;
            return go;
        }

        void CreateDivider(Transform parent, Vector2 pos)
        {
            var go = new GameObject("Divider");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(500f, 2f);
            var img = go.AddComponent<Image>();
            img.color = new Color(0.4f, 0.4f, 0.4f, 0.6f);
        }

        Button CreateMenuButton(Transform parent, string label, Vector2 pos, Color bgColor)
        {
            var go = new GameObject($"Btn_{label}");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(200f, 50f);

            var img = go.AddComponent<Image>();
            img.color = bgColor;

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 20f;
            tmp.color = Color.white;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;

            return btn;
        }

        TextMeshProUGUI CreateText(Transform parent, string text, float size,
            Vector2 pos, Color color, FontStyles style = FontStyles.Normal,
            Vector2? customSize = null)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = customSize ?? new Vector2(600f, 50f);
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
