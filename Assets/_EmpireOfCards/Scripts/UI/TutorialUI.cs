using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Runtime-built tutorial overlay with two display modes:
    ///   FullScreen -- dark overlay with large centered text (for story beats)
    ///   FloatingTip -- smaller anchored panel for in-game guidance
    ///
    /// Features:
    ///   - Typewriter text effect (characters revealed one by one)
    ///   - Pulsing action button ("Begin" / "Got it")
    ///   - 2D arrow indicator that can point to any screen position
    ///   - Skip button to bail out of the whole tutorial
    /// </summary>
    public class TutorialUI : MonoBehaviour
    {
        // === Display mode ===
        public enum DisplayMode { FullScreen, FloatingTip }

        // === Full-screen elements ===
        private RectTransform _rootPanel;
        private Image _overlayImage;
        private RectTransform _fullScreenPanel;
        private Image _fullScreenPanelBg;
        private TMP_Text _fullScreenText;
        private Button _fullScreenButton;
        private TMP_Text _fullScreenButtonLabel;
        private Image _fullScreenButtonImage;

        // === Floating tip elements ===
        private RectTransform _tipPanel;
        private Image _tipPanelBg;
        private TMP_Text _tipText;
        private Button _tipButton;
        private TMP_Text _tipButtonLabel;
        private Image _tipButtonImage;

        // === 2D Arrow indicator ===
        private RectTransform _arrow2DRoot;
        private TMP_Text _arrow2DText;

        // === Skip button ===
        private Button _skipButton;

        // === Typewriter state ===
        private string _targetText = "";
        private int _charIndex;
        private float _typeTimer;
        private const float TYPE_SPEED = 0.02f;
        private bool _typing;
        private TMP_Text _activeText;

        // === Button pulse state ===
        private Image _activeButtonImage;
        private Color _buttonBaseColor;
        private float _pulseTimer;
        private const float PULSE_SPEED = 2f;

        // === Events ===
        /// <summary>Fired when the player clicks the advance button.</summary>
        public event Action OnNextClicked;

        /// <summary>Fired when the player clicks the Skip Tutorial button.</summary>
        public event Action OnSkipClicked;

        // ------------------------------------------------------------------
        //  Factory
        // ------------------------------------------------------------------

        /// <summary>
        /// Builds the complete tutorial UI hierarchy under the given canvas
        /// parent. Returns the TutorialUI component ready for use.
        /// </summary>
        public static TutorialUI Create(Transform canvasParent)
        {
            // === ROOT (full-screen overlay) ===
            var rootGo = new GameObject("TutorialOverlay");
            rootGo.transform.SetParent(canvasParent, false);

            var rootRT = rootGo.AddComponent<RectTransform>();
            StretchFill(rootRT);

            var overlayImg = rootGo.AddComponent<Image>();
            overlayImg.color = new Color(0f, 0f, 0f, 0.75f);
            overlayImg.raycastTarget = true;

            // ============================================================
            //  FULL-SCREEN STORY PANEL (centered, large)
            // ============================================================
            var fsPanel = CreatePanel("FullScreenPanel", rootGo.transform,
                new Vector2(800, 360), new Color(0.06f, 0.06f, 0.1f, 0.97f));
            var fsPanelRT = fsPanel.GetComponent<RectTransform>();
            CenterAnchor(fsPanelRT);

            // Story text
            var fsTextGo = CreateTMPText("FullScreenText", fsPanel.transform,
                "", 24, TextAlignmentOptions.Center, Color.white);
            var fsTextRT = fsTextGo.GetComponent<RectTransform>();
            fsTextRT.anchorMin = new Vector2(0f, 0.2f);
            fsTextRT.anchorMax = new Vector2(1f, 1f);
            fsTextRT.offsetMin = new Vector2(40, 10);
            fsTextRT.offsetMax = new Vector2(-40, -20);
            var fsTextTMP = fsTextGo.GetComponent<TMP_Text>();
            fsTextTMP.enableWordWrapping = true;
            fsTextTMP.overflowMode = TextOverflowModes.Ellipsis;

            // Story button ("Begin" / "Got it")
            var fsBtnGo = CreateButton("FullScreenButton", fsPanel.transform,
                "Got it", new Color(0.18f, 0.55f, 0.32f));
            var fsBtnRT = fsBtnGo.GetComponent<RectTransform>();
            fsBtnRT.anchorMin = new Vector2(0.5f, 0f);
            fsBtnRT.anchorMax = new Vector2(0.5f, 0f);
            fsBtnRT.pivot = new Vector2(0.5f, 0f);
            fsBtnRT.anchoredPosition = new Vector2(0, 20);
            fsBtnRT.sizeDelta = new Vector2(180, 50);

            // ============================================================
            //  FLOATING TIP PANEL (bottom-center, smaller)
            // ============================================================
            var tipPanel = CreatePanel("TipPanel", rootGo.transform,
                new Vector2(720, 180), new Color(0.08f, 0.08f, 0.12f, 0.95f));
            var tipPanelRT = tipPanel.GetComponent<RectTransform>();
            tipPanelRT.anchorMin = new Vector2(0.5f, 0f);
            tipPanelRT.anchorMax = new Vector2(0.5f, 0f);
            tipPanelRT.pivot = new Vector2(0.5f, 0f);
            tipPanelRT.anchoredPosition = new Vector2(0, 30);

            // Tip text
            var tipTextGo = CreateTMPText("TipText", tipPanel.transform,
                "", 20, TextAlignmentOptions.Center, Color.white);
            var tipTextRT = tipTextGo.GetComponent<RectTransform>();
            tipTextRT.anchorMin = new Vector2(0f, 0.25f);
            tipTextRT.anchorMax = new Vector2(1f, 1f);
            tipTextRT.offsetMin = new Vector2(25, 5);
            tipTextRT.offsetMax = new Vector2(-25, -10);
            var tipTextTMP = tipTextGo.GetComponent<TMP_Text>();
            tipTextTMP.enableWordWrapping = true;

            // Tip button
            var tipBtnGo = CreateButton("TipButton", tipPanel.transform,
                "Got it", new Color(0.18f, 0.55f, 0.32f));
            var tipBtnRT = tipBtnGo.GetComponent<RectTransform>();
            tipBtnRT.anchorMin = new Vector2(0.5f, 0f);
            tipBtnRT.anchorMax = new Vector2(0.5f, 0f);
            tipBtnRT.pivot = new Vector2(0.5f, 0f);
            tipBtnRT.anchoredPosition = new Vector2(0, 8);
            tipBtnRT.sizeDelta = new Vector2(140, 40);

            // ============================================================
            //  2D ARROW INDICATOR (movable, rotatable)
            // ============================================================
            var arrowGo = new GameObject("Arrow2D");
            arrowGo.transform.SetParent(rootGo.transform, false);
            var arrowRT = arrowGo.AddComponent<RectTransform>();
            arrowRT.sizeDelta = new Vector2(60, 60);
            arrowRT.anchorMin = new Vector2(0.5f, 0.5f);
            arrowRT.anchorMax = new Vector2(0.5f, 0.5f);

            var arrowTMP = arrowGo.AddComponent<TextMeshProUGUI>();
            arrowTMP.text = "-->"; // Right-pointing triangle by default
            arrowTMP.fontSize = 42;
            arrowTMP.alignment = TextAlignmentOptions.Center;
            arrowTMP.color = new Color(1f, 0.85f, 0.2f); // Gold
            arrowTMP.raycastTarget = false;
            arrowGo.SetActive(false);

            // ============================================================
            //  SKIP TUTORIAL BUTTON (top-right corner)
            // ============================================================
            var skipGo = CreateButton("SkipTutorial", rootGo.transform,
                "Skip Tutorial", new Color(0.4f, 0.15f, 0.15f));
            var skipRT = skipGo.GetComponent<RectTransform>();
            skipRT.anchorMin = new Vector2(1f, 1f);
            skipRT.anchorMax = new Vector2(1f, 1f);
            skipRT.pivot = new Vector2(1f, 1f);
            skipRT.anchoredPosition = new Vector2(-20, -20);
            skipRT.sizeDelta = new Vector2(170, 40);

            // ============================================================
            //  ASSEMBLE COMPONENT
            // ============================================================
            var tutUI = rootGo.AddComponent<TutorialUI>();

            tutUI._rootPanel = rootRT;
            tutUI._overlayImage = overlayImg;

            // Full screen
            tutUI._fullScreenPanel = fsPanelRT;
            tutUI._fullScreenPanelBg = fsPanel.GetComponent<Image>();
            tutUI._fullScreenText = fsTextTMP;
            tutUI._fullScreenButton = fsBtnGo.GetComponent<Button>();
            tutUI._fullScreenButtonLabel = fsBtnGo.GetComponentInChildren<TMP_Text>();
            tutUI._fullScreenButtonImage = fsBtnGo.GetComponent<Image>();

            // Floating tip
            tutUI._tipPanel = tipPanelRT;
            tutUI._tipPanelBg = tipPanel.GetComponent<Image>();
            tutUI._tipText = tipTextTMP;
            tutUI._tipButton = tipBtnGo.GetComponent<Button>();
            tutUI._tipButtonLabel = tipBtnGo.GetComponentInChildren<TMP_Text>();
            tutUI._tipButtonImage = tipBtnGo.GetComponent<Image>();

            // 2D arrow
            tutUI._arrow2DRoot = arrowRT;
            tutUI._arrow2DText = arrowTMP;

            // Skip button
            tutUI._skipButton = skipGo.GetComponent<Button>();

            // Wire button clicks
            tutUI._fullScreenButton.onClick.AddListener(() => tutUI.HandleNextClick());
            tutUI._tipButton.onClick.AddListener(() => tutUI.HandleNextClick());
            tutUI._skipButton.onClick.AddListener(() => tutUI.OnSkipClicked?.Invoke());

            // Start hidden
            rootGo.SetActive(false);

            Debug.Log("[TutorialUI] Story-driven tutorial overlay created.");
            return tutUI;
        }

        // ------------------------------------------------------------------
        //  Public API
        // ------------------------------------------------------------------

        /// <summary>
        /// Shows the full-screen story panel with typewriter effect.
        /// Used for dramatic story moments (Steps 1, 13, 14).
        /// </summary>
        public void ShowFullScreen(string message, string buttonLabel = "Got it")
        {
            gameObject.SetActive(true);
            _overlayImage.color = new Color(0f, 0f, 0f, 0.75f);

            _fullScreenPanel.gameObject.SetActive(true);
            _tipPanel.gameObject.SetActive(false);

            _fullScreenButtonLabel.text = buttonLabel;
            _activeButtonImage = _fullScreenButtonImage;
            _buttonBaseColor = _fullScreenButtonImage.color;

            StartTypewriter(_fullScreenText, message);
        }

        /// <summary>
        /// Shows the floating tip panel at the bottom of the screen.
        /// Used for in-game guidance (most steps).
        /// </summary>
        public void ShowTip(string message, string buttonLabel = "Got it")
        {
            gameObject.SetActive(true);
            _overlayImage.color = new Color(0f, 0f, 0f, 0.4f); // Lighter overlay for tips

            _fullScreenPanel.gameObject.SetActive(false);
            _tipPanel.gameObject.SetActive(true);

            _tipButtonLabel.text = buttonLabel;
            _activeButtonImage = _tipButtonImage;
            _buttonBaseColor = _tipButtonImage.color;

            StartTypewriter(_tipText, message);
        }

        /// <summary>
        /// Shows a 2D arrow pointing toward a screen position.
        /// The arrow sits at screenPos and rotates to face the direction.
        /// </summary>
        public void ShowArrow2D(Vector2 screenPos, Vector2 direction)
        {
            if (_arrow2DRoot == null) return;

            _arrow2DRoot.gameObject.SetActive(true);
            _arrow2DRoot.anchoredPosition = screenPos;

            // Calculate rotation from default right-pointing to desired direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _arrow2DRoot.localRotation = Quaternion.Euler(0, 0, angle);

            // Choose arrow glyph based on dominant axis
            if (_arrow2DText != null)
                _arrow2DText.text = "-->"; // Always use right-pointing, rotation handles direction
        }

        /// <summary>
        /// Hides the 2D arrow indicator.
        /// </summary>
        public void HideArrow2D()
        {
            if (_arrow2DRoot != null)
                _arrow2DRoot.gameObject.SetActive(false);
        }

        /// <summary>
        /// Hides the entire tutorial overlay (both panels, arrow, everything).
        /// </summary>
        public void Hide()
        {
            _typing = false;
            HideArrow2D();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Hides the advance button (for auto-advance steps).
        /// </summary>
        public void HideButton()
        {
            if (_fullScreenPanel.gameObject.activeSelf)
                _fullScreenButton.gameObject.SetActive(false);
            else
                _tipButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Shows the advance button (after hiding it).
        /// </summary>
        public void ShowButton()
        {
            if (_fullScreenPanel.gameObject.activeSelf)
                _fullScreenButton.gameObject.SetActive(true);
            else
                _tipButton.gameObject.SetActive(true);
        }

        // ------------------------------------------------------------------
        //  Update -- typewriter + button pulse
        // ------------------------------------------------------------------

        private void Update()
        {
            // Typewriter effect
            if (_typing && _activeText != null)
            {
                _typeTimer += Time.unscaledDeltaTime; // unscaled so it works when paused
                while (_typeTimer >= TYPE_SPEED && _charIndex < _targetText.Length)
                {
                    _typeTimer -= TYPE_SPEED;
                    _charIndex++;
                    _activeText.text = _targetText.Substring(0, _charIndex);
                }

                if (_charIndex >= _targetText.Length)
                    _typing = false;
            }

            // Button pulse (gentle brightness oscillation)
            if (_activeButtonImage != null)
            {
                _pulseTimer += Time.unscaledDeltaTime * PULSE_SPEED;
                float t = (Mathf.Sin(_pulseTimer) + 1f) * 0.5f;
                Color pulsed = Color.Lerp(_buttonBaseColor, _buttonBaseColor * 1.4f, t);
                pulsed.a = _buttonBaseColor.a;
                _activeButtonImage.color = pulsed;
            }
        }

        // ------------------------------------------------------------------
        //  Internal
        // ------------------------------------------------------------------

        private void HandleNextClick()
        {
            // If still typing, finish the text instantly first
            if (_typing && _activeText != null)
            {
                _typing = false;
                _activeText.text = _targetText;
                return;
            }

            OnNextClicked?.Invoke();
        }

        private void StartTypewriter(TMP_Text textComponent, string message)
        {
            _activeText = textComponent;
            _targetText = message;
            _charIndex = 0;
            _typeTimer = 0f;
            _typing = true;
            _pulseTimer = 0f;
            textComponent.text = "";
        }

        // ------------------------------------------------------------------
        //  UI Construction Helpers
        // ------------------------------------------------------------------

        private static GameObject CreatePanel(string name, Transform parent, Vector2 size, Color bgColor)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta = size;
            var img = go.AddComponent<Image>();
            img.color = bgColor;
            return go;
        }

        private static GameObject CreateTMPText(string name, Transform parent,
            string content, int fontSize, TextAlignmentOptions alignment, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.fontSize = fontSize;
            tmp.alignment = alignment;
            tmp.color = color;
            return go;
        }

        private static GameObject CreateButton(string name, Transform parent,
            string label, Color bgColor)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            var img = go.AddComponent<Image>();
            img.color = bgColor;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            var lblGo = new GameObject("Label");
            lblGo.transform.SetParent(go.transform, false);
            var lblRT = lblGo.AddComponent<RectTransform>();
            StretchFill(lblRT);
            var lblTMP = lblGo.AddComponent<TextMeshProUGUI>();
            lblTMP.text = label;
            lblTMP.fontSize = 20;
            lblTMP.alignment = TextAlignmentOptions.Center;
            lblTMP.color = Color.white;

            return go;
        }

        private static void StretchFill(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static void CenterAnchor(RectTransform rt)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
        }
    }
}
