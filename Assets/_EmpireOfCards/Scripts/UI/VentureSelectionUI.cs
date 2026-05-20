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

        // Venture-specific accent colors (order matches VentureType enum: FastFood, Cafe, TechApp, ClothingStore, GroceryStore)
        private static readonly Color[] VentureAccents = new Color[]
        {
            new Color(0.9f, 0.4f, 0.1f),   // FastFood - red-orange
            new Color(0.6f, 0.38f, 0.22f), // Cafe - coffee brown
            new Color(0.25f, 0.55f, 0.95f),// TechApp - blue
            new Color(0.85f, 0.25f, 0.55f),// ClothingStore - pink/rose
            new Color(0.3f, 0.7f, 0.35f)   // GroceryStore - green
        };

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

            }

            if (startButton != null)
            {
                startButton.onClick.AddListener(ConfirmSelection);
                startButton.interactable = false;
            }

            RefreshTexts();
        }

        private void OnEnable()
        {
            LocalizationManager.OnLanguageChanged += RefreshTexts;
        }

        private void OnDisable()
        {
            LocalizationManager.OnLanguageChanged -= RefreshTexts;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _selectedIndex = -1;
            RefreshTexts();
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

        private void RefreshTexts()
        {
            if (_ventures == null)
                return;

            for (int i = 0; i < _ventures.Length; i++)
            {
                if (ventureNameTexts != null && i < ventureNameTexts.Length && ventureNameTexts[i] != null)
                    ventureNameTexts[i].text = GetVentureName(_ventures[i]);

                if (ventureDescTexts != null && i < ventureDescTexts.Length && ventureDescTexts[i] != null)
                    ventureDescTexts[i].text = GetVentureDescription(_ventures[i]);
            }
        }

        private static string GetVentureName(VentureData venture)
        {
            if (venture == null)
                return string.Empty;

            string key = $"venture.{GetKeySuffix(venture.ventureType)}.name";
            return LocalizationManager.GetWithFallback(key, venture.ventureName);
        }

        private static string GetVentureDescription(VentureData venture)
        {
            if (venture == null)
                return string.Empty;

            string descKey = $"venture.{GetKeySuffix(venture.ventureType)}.desc";
            string playstyleKey = $"venture.{GetKeySuffix(venture.ventureType)}.playstyle";
            string rule = LocalizationManager.GetWithFallback("venture.same_sector_rule", "Rival starts in the same sector.");

            string desc = LocalizationManager.GetWithFallback(descKey, venture.description);
            string playstyle = LocalizationManager.GetWithFallback(playstyleKey, venture.playstyleSummary);

            if (!string.IsNullOrWhiteSpace(playstyle))
                desc += $"\n{playstyle}";

            if (!string.IsNullOrWhiteSpace(rule))
                desc += $"\n{rule}";

            return desc;
        }

        private static string GetKeySuffix(VentureType ventureType)
        {
            return ventureType switch
            {
                VentureType.FastFood => "fast_food",
                VentureType.Cafe => "cafe",
                VentureType.TechApp => "tech_app",
                VentureType.ClothingStore => "clothing_store",
                VentureType.GroceryStore => "grocery_store",
                _ => "fast_food"
            };
        }
    }
}
