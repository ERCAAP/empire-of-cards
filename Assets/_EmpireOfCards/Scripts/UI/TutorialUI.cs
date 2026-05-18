using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Screen-space overlay panel that displays tutorial messages.
    /// Built entirely at runtime (no prefab needed).
    ///
    /// Layout:
    ///   - Full-screen semi-transparent dark overlay
    ///   - Centered message panel with white text
    ///   - Optional arrow indicator (text-based)
    ///   - "Got it" button to advance
    /// </summary>
    public class TutorialUI : MonoBehaviour
    {
        // === UI elements (created in Create()) ===
        private RectTransform _rootPanel;
        private Image _overlayImage;
        private RectTransform _messagePanel;
        private Image _messagePanelBg;
        private TMP_Text _messageText;
        private TMP_Text _arrowText;
        private Button _nextButton;
        private TMP_Text _nextButtonLabel;

        /// <summary>
        /// Fired when the player clicks the "Got it" / "Next" button.
        /// </summary>
        public event Action OnNextClicked;

        // ------------------------------------------------------------------
        //  Factory
        // ------------------------------------------------------------------

        /// <summary>
        /// Creates the full tutorial UI hierarchy under the given canvas parent.
        /// Returns the TutorialUI component ready to use.
        /// </summary>
        public static TutorialUI Create(Transform canvasParent)
        {
            // --- Root (full-screen overlay) ---
            var rootGo = new GameObject("TutorialOverlay");
            rootGo.transform.SetParent(canvasParent, false);

            var rootRT = rootGo.AddComponent<RectTransform>();
            rootRT.anchorMin = Vector2.zero;
            rootRT.anchorMax = Vector2.one;
            rootRT.sizeDelta = Vector2.zero;
            rootRT.offsetMin = Vector2.zero;
            rootRT.offsetMax = Vector2.zero;

            var overlayImg = rootGo.AddComponent<Image>();
            overlayImg.color = new Color(0f, 0f, 0f, 0.6f);
            overlayImg.raycastTarget = true; // Block clicks behind

            // --- Message Panel (centered box) ---
            var panelGo = new GameObject("MessagePanel");
            panelGo.transform.SetParent(rootGo.transform, false);

            var panelRT = panelGo.AddComponent<RectTransform>();
            panelRT.anchorMin = new Vector2(0.5f, 0.5f);
            panelRT.anchorMax = new Vector2(0.5f, 0.5f);
            panelRT.pivot = new Vector2(0.5f, 0.5f);
            panelRT.sizeDelta = new Vector2(700, 260);

            var panelImg = panelGo.AddComponent<Image>();
            panelImg.color = new Color(0.08f, 0.08f, 0.12f, 0.95f);

            // --- Arrow Text (above the message panel) ---
            var arrowGo = new GameObject("Arrow");
            arrowGo.transform.SetParent(panelGo.transform, false);

            var arrowRT = arrowGo.AddComponent<RectTransform>();
            arrowRT.anchorMin = new Vector2(0.5f, 1f);
            arrowRT.anchorMax = new Vector2(0.5f, 1f);
            arrowRT.pivot = new Vector2(0.5f, 0f);
            arrowRT.anchoredPosition = new Vector2(0, 10);
            arrowRT.sizeDelta = new Vector2(60, 40);

            var arrowTMP = arrowGo.AddComponent<TextMeshProUGUI>();
            arrowTMP.text = "";
            arrowTMP.fontSize = 36;
            arrowTMP.alignment = TextAlignmentOptions.Center;
            arrowTMP.color = new Color(1f, 0.85f, 0.3f); // Gold arrow

            // --- Message Text ---
            var textGo = new GameObject("MessageText");
            textGo.transform.SetParent(panelGo.transform, false);

            var textRT = textGo.AddComponent<RectTransform>();
            textRT.anchorMin = new Vector2(0f, 0.2f);
            textRT.anchorMax = new Vector2(1f, 1f);
            textRT.sizeDelta = Vector2.zero;
            textRT.offsetMin = new Vector2(30, 10);
            textRT.offsetMax = new Vector2(-30, -10);

            var msgTMP = textGo.AddComponent<TextMeshProUGUI>();
            msgTMP.text = "";
            msgTMP.fontSize = 22;
            msgTMP.alignment = TextAlignmentOptions.Center;
            msgTMP.color = Color.white;
            msgTMP.enableWordWrapping = true;
            msgTMP.overflowMode = TextOverflowModes.Ellipsis;

            // --- "Got it" Button ---
            var btnGo = new GameObject("NextButton");
            btnGo.transform.SetParent(panelGo.transform, false);

            var btnRT = btnGo.AddComponent<RectTransform>();
            btnRT.anchorMin = new Vector2(0.5f, 0f);
            btnRT.anchorMax = new Vector2(0.5f, 0f);
            btnRT.pivot = new Vector2(0.5f, 0f);
            btnRT.anchoredPosition = new Vector2(0, 15);
            btnRT.sizeDelta = new Vector2(160, 45);

            var btnImg = btnGo.AddComponent<Image>();
            btnImg.color = new Color(0.2f, 0.55f, 0.35f);

            var btn = btnGo.AddComponent<Button>();
            btn.targetGraphic = btnImg;

            // Button label
            var lblGo = new GameObject("Label");
            lblGo.transform.SetParent(btnGo.transform, false);

            var lblRT = lblGo.AddComponent<RectTransform>();
            lblRT.anchorMin = Vector2.zero;
            lblRT.anchorMax = Vector2.one;
            lblRT.sizeDelta = Vector2.zero;
            lblRT.offsetMin = Vector2.zero;
            lblRT.offsetMax = Vector2.zero;

            var lblTMP = lblGo.AddComponent<TextMeshProUGUI>();
            lblTMP.text = "Got it";
            lblTMP.fontSize = 22;
            lblTMP.alignment = TextAlignmentOptions.Center;
            lblTMP.color = Color.white;

            // --- Assemble TutorialUI component ---
            var tutUI = rootGo.AddComponent<TutorialUI>();
            tutUI._rootPanel = rootRT;
            tutUI._overlayImage = overlayImg;
            tutUI._messagePanel = panelRT;
            tutUI._messagePanelBg = panelImg;
            tutUI._messageText = msgTMP;
            tutUI._arrowText = arrowTMP;
            tutUI._nextButton = btn;
            tutUI._nextButtonLabel = lblTMP;

            // Wire button
            btn.onClick.AddListener(() => tutUI.OnNextClicked?.Invoke());

            // Start hidden
            rootGo.SetActive(false);

            Debug.Log("[TutorialUI] Tutorial overlay created.");
            return tutUI;
        }

        // ------------------------------------------------------------------
        //  Public API
        // ------------------------------------------------------------------

        /// <summary>
        /// Shows the tutorial panel with the given message and optional arrow.
        /// </summary>
        /// <param name="message">The tutorial text to display.</param>
        /// <param name="arrowDir">
        /// Direction hint: "up", "down", "left", "right", or null/empty for none.
        /// </param>
        public void Show(string message, string arrowDir = null)
        {
            if (_messageText != null)
                _messageText.text = message;

            if (_arrowText != null)
            {
                _arrowText.text = DirectionToArrow(arrowDir);
                PositionArrow(arrowDir);
            }

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides the tutorial panel.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        // ------------------------------------------------------------------
        //  Internal Helpers
        // ------------------------------------------------------------------

        /// <summary>
        /// Converts a direction string to a Unicode arrow character.
        /// </summary>
        private static string DirectionToArrow(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                return "";

            return dir.ToLowerInvariant() switch
            {
                "up"    => "\u25B2",   // Black up-pointing triangle
                "down"  => "\u25BC",   // Black down-pointing triangle
                "left"  => "\u25C0",   // Black left-pointing triangle
                "right" => "\u25B6",   // Black right-pointing triangle
                _       => ""
            };
        }

        /// <summary>
        /// Repositions the arrow text element relative to the message panel
        /// based on the direction it should point toward.
        /// </summary>
        private void PositionArrow(string dir)
        {
            if (_arrowText == null) return;

            var rt = _arrowText.rectTransform;

            switch (dir?.ToLowerInvariant())
            {
                case "up":
                    rt.anchorMin = new Vector2(0.5f, 1f);
                    rt.anchorMax = new Vector2(0.5f, 1f);
                    rt.pivot = new Vector2(0.5f, 0f);
                    rt.anchoredPosition = new Vector2(0, 10);
                    break;

                case "down":
                    rt.anchorMin = new Vector2(0.5f, 0f);
                    rt.anchorMax = new Vector2(0.5f, 0f);
                    rt.pivot = new Vector2(0.5f, 1f);
                    rt.anchoredPosition = new Vector2(0, -10);
                    break;

                case "left":
                    rt.anchorMin = new Vector2(0f, 0.5f);
                    rt.anchorMax = new Vector2(0f, 0.5f);
                    rt.pivot = new Vector2(1f, 0.5f);
                    rt.anchoredPosition = new Vector2(-10, 0);
                    break;

                case "right":
                    rt.anchorMin = new Vector2(1f, 0.5f);
                    rt.anchorMax = new Vector2(1f, 0.5f);
                    rt.pivot = new Vector2(0f, 0.5f);
                    rt.anchoredPosition = new Vector2(10, 0);
                    break;

                default:
                    // Hide arrow when no direction
                    rt.anchoredPosition = Vector2.zero;
                    break;
            }
        }
    }
}
