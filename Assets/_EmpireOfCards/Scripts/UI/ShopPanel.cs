using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Data;
using EmpireOfCards.UI.Cards;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Displays available cards for purchase each turn.
    /// </summary>
    public class ShopPanel : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject panel;

        [Header("Card Slots")]
        [SerializeField] private CardUI[] shopCards;
        [SerializeField] private TMP_Text[] priceTexts;
        [SerializeField] private Button[] buyButtons;
        [SerializeField] private Button closeButton;

        // --- Events ---
        public event Action<int> OnBuyRequested;

        private List<CardData> currentStock;

        private void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Close);
            }

            // Wire up buy buttons
            for (int i = 0; i < buyButtons.Length; i++)
            {
                int index = i; // capture for closure
                if (buyButtons[i] != null)
                {
                    buyButtons[i].onClick.AddListener(() => OnBuyCard(index));
                }
            }
        }

        /// <summary>
        /// Populates the shop with the given cards and shows the panel.
        /// </summary>
        public void Open(List<CardData> cards)
        {
            currentStock = cards;

            for (int i = 0; i < shopCards.Length; i++)
            {
                if (i < cards.Count && cards[i] != null)
                {
                    shopCards[i].gameObject.SetActive(true);
                    shopCards[i].SetupCard(cards[i]);

                    if (i < priceTexts.Length && priceTexts[i] != null)
                    {
                        priceTexts[i].text = $"${cards[i].buyCost}";
                    }

                    if (i < buyButtons.Length && buyButtons[i] != null)
                    {
                        buyButtons[i].interactable = true;
                    }
                }
                else
                {
                    shopCards[i].gameObject.SetActive(false);

                    if (i < buyButtons.Length && buyButtons[i] != null)
                    {
                        buyButtons[i].interactable = false;
                    }
                }
            }

            if (panel != null)
            {
                panel.SetActive(true);
            }
        }

        /// <summary>
        /// Closes the shop panel.
        /// </summary>
        public void Close()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        /// <summary>
        /// Called when a buy button is clicked.
        /// </summary>
        public void OnBuyCard(int index)
        {
            if (currentStock == null || index < 0 || index >= currentStock.Count)
                return;

            OnBuyRequested?.Invoke(index);

            // Disable the buy button after purchase
            if (index < buyButtons.Length && buyButtons[index] != null)
            {
                buyButtons[index].interactable = false;
            }
        }

        /// <summary>
        /// Applies a discount to all displayed prices.
        /// </summary>
        public void UpdatePrices(float discount)
        {
            if (currentStock == null)
                return;

            float multiplier = Mathf.Clamp01(1f - discount);

            for (int i = 0; i < currentStock.Count && i < priceTexts.Length; i++)
            {
                if (priceTexts[i] != null && currentStock[i] != null)
                {
                    int discountedPrice = Mathf.RoundToInt(currentStock[i].buyCost * multiplier);
                    priceTexts[i].text = $"${discountedPrice}";
                }
            }
        }

        private void OnDestroy()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
            }

            foreach (Button btn in buyButtons)
            {
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                }
            }
        }
    }
}
