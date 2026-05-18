using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Handles the card shop where players purchase new cards each turn.
    /// GDD rules:
    /// - 3 random cards each turn (no duplicates)
    /// - Player buys with money
    /// - Buying does NOT cost an action (shop is free action per GDD)
    /// - Sell rate: 40% of buy cost
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        // --- Data ---
        [Header("Shop Configuration")]
        [SerializeField] private CardData[] shopPool;           // All purchasable cards
        [SerializeField] private int shopSize = 3;              // GDD: 3 cards per turn

        // --- Manager References ---
        [Header("References")]
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private EconomyManager economyManager;
        [SerializeField] private ComboSystem comboSystem;

        // --- Runtime State ---
        [Header("Current Shop (Read Only)")]
        [SerializeField] private List<CardData> currentShopCards = new List<CardData>();
        [SerializeField] private float activeDiscount;

        // --- Events ---
        public event Action<List<CardData>> OnShopRefreshed;
        public event Action<CardData> OnCardPurchased;
        public event Action<CardData, int> OnCardSold;          // card, sellPrice

        // --- Properties ---
        public IReadOnlyList<CardData> CurrentShopCards => currentShopCards;
        public float ActiveDiscount => activeDiscount;

        // ----------------------------------------------------------------
        // Initialization
        // ----------------------------------------------------------------

        /// <summary>
        /// Initializes the shop pool. Call at the start of a run.
        /// The pool can be expanded as the player unlocks tiers (MetaProgression).
        /// </summary>
        public void Initialize(CardData[] availableCards)
        {
            if (availableCards != null && availableCards.Length > 0)
            {
                shopPool = availableCards;
            }

            currentShopCards.Clear();
            activeDiscount = 0f;
        }

        /// <summary>
        /// Adds cards to the shop pool (for tier unlocks).
        /// </summary>
        public void AddToPool(CardData[] newCards)
        {
            if (newCards == null || newCards.Length == 0) return;

            List<CardData> expanded = new List<CardData>(shopPool ?? new CardData[0]);
            expanded.AddRange(newCards);
            shopPool = expanded.ToArray();
        }

        // ----------------------------------------------------------------
        // Refresh (each turn)
        // ----------------------------------------------------------------

        /// <summary>
        /// Picks 3 random cards from the shop pool for this turn's offering.
        /// No duplicates within the same shop offering.
        /// Applies combo discounts (Kriz Avcisi).
        /// Call at the start of each turn.
        /// </summary>
        public void RefreshShop()
        {
            currentShopCards.Clear();

            // Apply combo discount if any
            if (comboSystem != null)
            {
                activeDiscount = comboSystem.GetShopDiscount();
            }
            else
            {
                activeDiscount = 0f;
            }

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
                available.RemoveAt(randomIndex); // Prevent duplicates
            }

            OnShopRefreshed?.Invoke(currentShopCards);
        }

        // ----------------------------------------------------------------
        // Buy Card
        // ----------------------------------------------------------------

        /// <summary>
        /// Attempts to purchase the card at the given shop index.
        /// Deducts money from the player and adds the card to their deck.
        ///
        /// IMPORTANT: Buying does NOT cost an action (GDD says shop is free action).
        /// Only costs money.
        ///
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
                Debug.LogWarning("[ShopManager] Shop slot is empty (already purchased).");
                return false;
            }

            int finalCost = GetDiscountedPrice(card);

            GameManager gm = GameManager.Instance;
            if (gm == null) return false;

            // Check if player can afford it
            if (!gm.SpendMoney(finalCost))
            {
                Debug.Log("[ShopManager] Not enough money to buy this card.");
                return false;
            }

            // NOTE: No UseAction() call! GDD says shop buying is a free action.

            // Add to deck (goes to discard pile, available next shuffle)
            if (deckManager != null)
            {
                deckManager.AddCardToDeck(card);
            }

            // Remove from shop display
            currentShopCards[shopIndex] = null;

            EventBus.CardPlayed(card); // For tracking
            EventBus.MoneySpent(finalCost);
            OnCardPurchased?.Invoke(card);

            Debug.Log($"[ShopManager] Purchased '{card.cardName}' for {finalCost} (original: {card.buyCost}).");
            return true;
        }

        // ----------------------------------------------------------------
        // Sell Card
        // ----------------------------------------------------------------

        /// <summary>
        /// Sells a card from the player's hand or board for 40% of its buy cost.
        /// </summary>
        public bool SellCard(CardData card)
        {
            if (card == null) return false;

            int sellPrice = GetSellPrice(card);

            GameManager gm = GameManager.Instance;
            if (gm == null) return false;

            gm.GainMoney(sellPrice);
            OnCardSold?.Invoke(card, sellPrice);

            Debug.Log($"[ShopManager] Sold '{card.cardName}' for {sellPrice}.");
            return true;
        }

        // ----------------------------------------------------------------
        // Pricing
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns the price of a card after applying the active discount.
        /// Discount comes from Kriz Avcisi combo (50%).
        /// </summary>
        public int GetDiscountedPrice(CardData card)
        {
            if (card == null) return 0;

            float discounted = card.buyCost * (1f - activeDiscount);
            return Mathf.Max(1, Mathf.RoundToInt(discounted));
        }

        /// <summary>
        /// Returns the sell price of a card: buyCost x 40% (GDD).
        /// Uses EconomyManager if available, otherwise calculates directly.
        /// </summary>
        public int GetSellPrice(CardData card)
        {
            if (card == null) return 0;

            if (economyManager != null)
                return economyManager.GetSellPrice(card);

            return Mathf.Max(1, Mathf.RoundToInt(card.buyCost * Constants.SELL_RATE));
        }

        // ----------------------------------------------------------------
        // Discount Management
        // ----------------------------------------------------------------

        /// <summary>
        /// Applies a discount rate to all shop prices this turn.
        /// Used by the Kriz Avcisi combo (50% discount).
        /// </summary>
        public void ApplyDiscount(float discount)
        {
            activeDiscount = Mathf.Clamp01(discount);
        }

        /// <summary>
        /// Clears any active discount.
        /// </summary>
        public void ClearDiscount()
        {
            activeDiscount = 0f;
        }

        // ----------------------------------------------------------------
        // Queries
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns the current shop offering (copy).
        /// </summary>
        public List<CardData> GetShopCards()
        {
            return new List<CardData>(currentShopCards);
        }

        /// <summary>
        /// Returns how many cards are still available to buy this turn.
        /// </summary>
        public int GetAvailableCardCount()
        {
            int count = 0;
            foreach (var card in currentShopCards)
            {
                if (card != null) count++;
            }
            return count;
        }

        /// <summary>
        /// Resets shop state for a new run.
        /// </summary>
        public void Reset()
        {
            currentShopCards.Clear();
            activeDiscount = 0f;
        }
    }
}
