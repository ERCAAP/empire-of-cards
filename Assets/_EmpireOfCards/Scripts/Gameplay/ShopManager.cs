using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Handles the card shop where players purchase new cards each turn.
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        // --- Data ---
        [Header("Shop Configuration")]
        [SerializeField] private CardData[] shopPool; // All non-starter cards available for purchase
        [SerializeField] private int shopSize = 3;

        // --- Runtime State ---
        [Header("Current Shop (Read Only)")]
        [SerializeField] private List<CardData> currentShopCards = new List<CardData>();

        // --- Discount ---
        private float activeDiscount;

        // --- Manager References ---
        [Header("References")]
        [SerializeField] private DeckManager deckManager;

        // --- Events ---
        public event Action<List<CardData>> OnShopRefreshed;
        public event Action<CardData> OnCardPurchased;

        // --- Properties ---
        public IReadOnlyList<CardData> CurrentShopCards => currentShopCards;
        public float ActiveDiscount => activeDiscount;

        /// <summary>
        /// Picks random cards from the shop pool for this turn's offering.
        /// Call at the start of each turn.
        /// </summary>
        public void RefreshShop()
        {
            currentShopCards.Clear();
            activeDiscount = 0f;

            if (shopPool == null || shopPool.Length == 0)
            {
                Debug.LogWarning("[ShopManager] Shop pool is empty. No cards to offer.");
                OnShopRefreshed?.Invoke(currentShopCards);
                return;
            }

            // Build a temporary list to avoid duplicates
            List<CardData> available = new List<CardData>(shopPool);

            int cardsToPick = Mathf.Min(shopSize, available.Count);

            for (int i = 0; i < cardsToPick; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, available.Count);
                currentShopCards.Add(available[randomIndex]);
                available.RemoveAt(randomIndex);
            }

            OnShopRefreshed?.Invoke(currentShopCards);
        }

        /// <summary>
        /// Attempts to purchase the card at the given shop index.
        /// Deducts money from the player and adds the card to their deck.
        /// Returns true if the purchase succeeded.
        /// </summary>
        public bool BuyCard(int shopIndex)
        {
            if (shopIndex < 0 || shopIndex >= currentShopCards.Count)
            {
                Debug.LogWarning("[ShopManager] Invalid shop index.");
                return false;
            }

            CardData card = currentShopCards[shopIndex];

            if (card == null)
            {
                Debug.LogWarning("[ShopManager] Shop slot is empty.");
                return false;
            }

            int finalCost = GetDiscountedPrice(card);

            if (!GameManager.Instance.SpendMoney(finalCost))
            {
                Debug.Log("[ShopManager] Not enough money to buy this card.");
                return false;
            }

            if (!GameManager.Instance.UseAction())
            {
                // Refund the money since we can't complete the action
                GameManager.Instance.GainMoney(finalCost);
                Debug.Log("[ShopManager] No actions remaining.");
                return false;
            }

            // Add to deck and remove from shop
            if (deckManager != null)
            {
                deckManager.AddCardToDeck(card);
            }

            currentShopCards[shopIndex] = null;
            OnCardPurchased?.Invoke(card);
            return true;
        }

        /// <summary>
        /// Returns the current shop offering.
        /// </summary>
        public List<CardData> GetShopCards()
        {
            return new List<CardData>(currentShopCards);
        }

        /// <summary>
        /// Applies a discount rate to all shop prices this turn.
        /// Used by combos such as "Kriz Avcisi".
        /// </summary>
        public void ApplyDiscount(float discount)
        {
            activeDiscount = Mathf.Clamp01(discount);
        }

        /// <summary>
        /// Returns the price of a card after applying the active discount.
        /// </summary>
        public int GetDiscountedPrice(CardData card)
        {
            if (card == null)
                return 0;

            float discounted = card.buyCost * (1f - activeDiscount);
            return Mathf.Max(1, Mathf.RoundToInt(discounted));
        }
    }
}
