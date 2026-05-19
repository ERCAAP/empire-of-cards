using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Full-screen venture selection panel shown at game start.
    /// Player picks their first business venture before the run begins.
    /// </summary>
    public class VentureSelectionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform[] ventureCards;
        [SerializeField] private Image[] ventureCardImages;
        [SerializeField] private TMP_Text[] ventureNameTexts;
        [SerializeField] private TMP_Text[] ventureDescTexts;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Button startButton;
        [SerializeField] private CanvasGroup canvasGroup;

        private VentureData[] _ventures;
        private int _selectedIndex = -1;

        // Fires when player confirms their venture choice
        public event Action<VentureData> OnVentureSelected;

        // Colors
        private static readonly Color NormalColor = new Color(0.15f, 0.15f, 0.2f, 0.95f);
        private static readonly Color SelectedColor = new Color(0.2f, 0.4f, 0.6f, 1f);
        private static readonly Color HoverColor = new Color(0.2f, 0.25f, 0.35f, 0.95f);

        // Venture-specific accent colors
        private static readonly Color[] VentureAccents = new Color[]
        {
            new Color(0.9f, 0.5f, 0.2f),   // Bufe - orange
            new Color(0.3f, 0.6f, 0.9f),   // Tech - blue
            new Color(0.9f, 0.3f, 0.5f),   // Reklam - pink
            new Color(0.4f, 0.4f, 0.4f)    // Karanlik - dark gray
        };

        private static readonly string[] VentureEmojis = { "🍔", "💻", "📢", "🕶️" };

        /// <summary>
        /// Assigns UI element references at runtime (since SerializeField can't
        /// be set on components added via AddComponent).
        /// Called by GameSceneBootstrap after HUDBuilder creates the panel.
        /// </summary>
        public void SetUIReferences(RectTransform[] cards, Image[] cardImages,
            TMP_Text[] nameTexts, TMP_Text[] descTexts, Button startBtn)
        {
            ventureCards = cards;
            ventureCardImages = cardImages;
            ventureNameTexts = nameTexts;
            ventureDescTexts = descTexts;
            startButton = startBtn;

            if (startButton != null)
                startButton.interactable = false;
        }

        /// <summary>
        /// Assigns venture data and sets up button callbacks.
        /// Called by bootstrap after HUD is built.
        /// </summary>
        public void Init(VentureData[] ventures)
        {
            _ventures = ventures;

            for (int i = 0; i < ventureCards.Length && i < ventures.Length; i++)
            {
                int idx = i; // Capture for lambda
                var btn = ventureCards[i].GetComponent<Button>();
                if (btn != null)
                    btn.onClick.AddListener(() => SelectVenture(idx));

                if (ventureNameTexts != null && i < ventureNameTexts.Length)
                    ventureNameTexts[i].text = ventures[i].ventureName;

                if (ventureDescTexts != null && i < ventureDescTexts.Length)
                    ventureDescTexts[i].text = ventures[i].description;
            }

            if (startButton != null)
            {
                startButton.onClick.AddListener(ConfirmSelection);
                startButton.interactable = false;
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _selectedIndex = -1;
            UpdateVisuals();

            if (startButton != null)
                startButton.interactable = false;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void SelectVenture(int index)
        {
            _selectedIndex = index;
            UpdateVisuals();

            if (startButton != null)
                startButton.interactable = true;
        }

        private void ConfirmSelection()
        {
            if (_selectedIndex < 0 || _ventures == null || _selectedIndex >= _ventures.Length)
                return;

            OnVentureSelected?.Invoke(_ventures[_selectedIndex]);
            Hide();
        }

        private void UpdateVisuals()
        {
            for (int i = 0; i < ventureCardImages.Length; i++)
            {
                if (ventureCardImages[i] == null) continue;

                if (i == _selectedIndex)
                {
                    ventureCardImages[i].color = SelectedColor;
                    ventureCards[i].localScale = Vector3.one * 1.05f;
                }
                else
                {
                    ventureCardImages[i].color = NormalColor;
                    ventureCards[i].localScale = Vector3.one;
                }
            }
        }
    }
}
