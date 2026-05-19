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

            // Bufe (Diner) - food starter
            ventures[0] = CreateVenture(
                VentureType.Bufe,
                "Bufe",
                "Start with a humble diner. Solid food foundation.",
                FindCard(lookup, "B01_Bufe"),
                FindCard(lookup, "C04_Sef"),
                0
            );

            // Tech Startup - tech starter
            ventures[1] = CreateVenture(
                VentureType.TechStartup,
                "Tech Startup",
                "Start with a tech startup. Slow start, big payoff.",
                FindCard(lookup, "B04_TechStartup"),
                FindCard(lookup, "C07_Hacker"),
                0
            );

            // Reklam Ajansi (Ad Agency) - marketing starter
            ventures[2] = CreateVenture(
                VentureType.ReklamAjansi,
                "Reklam Ajansi",
                "Start with an ad agency. Boost all your businesses.",
                FindCard(lookup, "B08_ReklamAjansi"),
                FindCard(lookup, "C05_MarketingGurusu"),
                0
            );

            // Karanlik Pazar (Black Market) - no business, extra money
            ventures[3] = CreateVenture(
                VentureType.KaranlikPazar,
                "Karanlik Pazar",
                "No starting business. Extra cash and a shady contact.",
                null,
                FindCard(lookup, "C09_Dolandirici"),
                Constants.KARANLIK_PAZAR_BONUS_MONEY
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
