using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;
using EmpireOfCards.UI.Cards;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Shop overlay panel. Shows 3 cards with buy buttons and prices.
    /// Subscribes to ShopManager events and EventBus.OnMoneyChanged.
    /// Grays out cards the player cannot afford.
    /// </summary>
    public class ShopPanel : MonoBehaviour
    {
        [Header("Card Slots")]
        [SerializeField] private CardUI[] shopCards;
        [SerializeField] private TMP_Text[] priceTexts;
        [SerializeField] private Button[] buyButtons;

        [Header("References")]
        [SerializeField] private ShopManager shopManager;
        [SerializeField] private Button closeButton;

        [Header("Styling")]
        [SerializeField] private Color affordableColor = Color.white;
        [SerializeField] private Color unaffordableColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);

        // Cached stock for affordability re-checks
        private List<CardData> currentStock;

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void OnEnable()
        {
            if (shopManager != null)
            {
                shopManager.OnShopRefreshed += HandleShopRefreshed;
                shopManager.OnCardPurchased += HandleCardPurchased;
            }

            // Money changes affect affordability
            EventBus.OnMoneyChanged += HandleMoneyChanged;

            if (closeButton != null)
                closeButton.onClick.AddListener(Close);

            // Wire up buy buttons once
            for (int i = 0; i < buyButtons.Length; i++)
            {
                int index = i; // capture for closure
                if (buyButtons[i] != null)
                    buyButtons[i].onClick.AddListener(() => OnBuyClicked(index));
            }

            RefreshDisplay();
        }

        private void OnDisable()
        {
            if (shopManager != null)
            {
                shopManager.OnShopRefreshed -= HandleShopRefreshed;
                shopManager.OnCardPurchased -= HandleCardPurchased;
            }

            EventBus.OnMoneyChanged -= HandleMoneyChanged;

            if (closeButton != null)
                closeButton.onClick.RemoveListener(Close);

            for (int i = 0; i < buyButtons.Length; i++)
            {
                if (buyButtons[i] != null)
                    buyButtons[i].onClick.RemoveAllListeners();
            }
        }

        // ------------------------------------------------------------------
        // Event handlers
        // ------------------------------------------------------------------

        private void HandleShopRefreshed(List<CardData> cards)
        {
            RefreshDisplay();
        }

        private void HandleCardPurchased(CardData card)
        {
            RefreshDisplay();
        }

        private void HandleMoneyChanged(int newAmount)
        {
            UpdateAffordability(newAmount);
        }

        // ------------------------------------------------------------------
        // Public
        // ------------------------------------------------------------------

        /// <summary>
        /// Closes the shop panel.
        /// </summary>
        public void Close()
        {
            gameObject.SetActive(false);
        }

        // ------------------------------------------------------------------
        // Internal
        // ------------------------------------------------------------------

        private void RefreshDisplay()
        {
            if (shopManager == null)
                return;

            currentStock = new List<CardData>(shopManager.CurrentShopCards);

            for (int i = 0; i < shopCards.Length; i++)
            {
                if (i < currentStock.Count && currentStock[i] != null)
                {
                    if (shopCards[i] != null)
                    {
                        shopCards[i].gameObject.SetActive(true);
                        shopCards[i].SetupCard(currentStock[i]);
                    }

                    if (i < priceTexts.Length && priceTexts[i] != null)
                        priceTexts[i].text = $"${shopManager.GetDiscountedPrice(currentStock[i])}";

                    if (i < buyButtons.Length && buyButtons[i] != null)
                        buyButtons[i].interactable = true;
                }
                else
                {
                    if (i < shopCards.Length && shopCards[i] != null)
                        shopCards[i].gameObject.SetActive(false);

                    if (i < buyButtons.Length && buyButtons[i] != null)
                        buyButtons[i].interactable = false;
                }
            }

            int playerMoney = GameManager.Instance != null ? GameManager.Instance.PlayerMoney : 0;
            UpdateAffordability(playerMoney);
        }

        private void UpdateAffordability(int playerMoney)
        {
            if (shopManager == null || currentStock == null)
                return;

            for (int i = 0; i < shopCards.Length; i++)
            {
                if (i >= currentStock.Count || currentStock[i] == null)
                    continue;

                int price = shopManager.GetDiscountedPrice(currentStock[i]);
                bool canAfford = playerMoney >= price;

                if (i < buyButtons.Length && buyButtons[i] != null)
                    buyButtons[i].interactable = canAfford;

                if (i < priceTexts.Length && priceTexts[i] != null)
                    priceTexts[i].color = canAfford ? affordableColor : unaffordableColor;

                if (shopCards[i] != null)
                {
                    CanvasGroup cg = shopCards[i].GetComponent<CanvasGroup>();
                    if (cg != null)
                        cg.alpha = canAfford ? 1f : 0.5f;
                }
            }
        }

        private void OnBuyClicked(int shopIndex)
        {
            if (shopManager == null)
                return;

            shopManager.BuyCard(shopIndex);
        }
    }
}
