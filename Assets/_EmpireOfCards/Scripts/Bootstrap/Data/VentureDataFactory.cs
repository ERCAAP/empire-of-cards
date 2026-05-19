using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap.Data
{
    /// <summary>
    /// Creates the 4 First Venture options at runtime (GDD Section 1.5).
    /// Same pattern as other Bootstrap.Data factories.
    /// </summary>
    public static class VentureDataFactory
    {
        public static VentureData[] CreateAllVentures(CardData[] allCards)
        {
            var lookup = BuildLookup(allCards);

            var ventures = new VentureData[4];

            // Diner - food starter
            ventures[0] = CreateVenture(
                VentureType.Diner,
                "Diner",
                "Start with a humble diner. Solid food foundation.",
                FindCard(lookup, "B01_Diner"),
                FindCard(lookup, "C04_Chef"),
                0
            );

            // Tech Startup - tech starter
            ventures[1] = CreateVenture(
                VentureType.TechStartup,
                "Tech Startup",
                "Start with a tech startup. Slow start, big payoff.",
                FindCard(lookup, "B04_TechStartup"),
                FindCard(lookup, "C07_Hacker"),  // already English
                0
            );

            // Ad Agency - marketing starter
            ventures[2] = CreateVenture(
                VentureType.AdAgency,
                "Ad Agency",
                "Start with an ad agency. Boost all your businesses.",
                FindCard(lookup, "B08_AdAgency"),
                FindCard(lookup, "C05_MarketingGuru"),
                0
            );

            // Black Market - no business, extra money
            ventures[3] = CreateVenture(
                VentureType.BlackMarket,
                "Black Market",
                "No starting business. Extra cash and a shady contact.",
                null,
                FindCard(lookup, "C09_Fraudster"),
                Constants.BLACK_MARKET_BONUS_MONEY
            );

            return ventures;
        }

        private static VentureData CreateVenture(VentureType type, string name,
            string desc, CardData startingBusiness, CardData bonusCard, int bonusMoney)
        {
            var v = ScriptableObject.CreateInstance<VentureData>();
            v.ventureType = type;
            v.ventureName = name;
            v.description = desc;
            v.startingBusiness = startingBusiness;
            v.bonusDeckCard = bonusCard;
            v.bonusMoney = bonusMoney;
            v.name = $"Venture_{type}";
            return v;
        }

        private static Dictionary<string, CardData> BuildLookup(CardData[] allCards)
        {
            var lookup = new Dictionary<string, CardData>();
            foreach (var card in allCards)
            {
                if (card != null && !string.IsNullOrEmpty(card.cardId))
                    lookup[card.cardId] = card;
            }
            return lookup;
        }

        private static CardData FindCard(Dictionary<string, CardData> lookup, string id)
        {
            if (lookup.TryGetValue(id, out var card)) return card;
            Debug.LogWarning($"[VentureDataFactory] Card not found: {id}");
            return null;
        }
    }
}
