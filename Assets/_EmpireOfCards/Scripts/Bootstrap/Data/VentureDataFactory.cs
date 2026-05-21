using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap.Data
{
    /// <summary>
    /// Creates the 5 First Venture options at runtime (GDD Section 1.5).
    /// Same pattern as other Bootstrap.Data factories.
    /// </summary>
    public static class VentureDataFactory
    {
        public static VentureData[] CreateAllVentures(CardData[] allCards)
        {
            var lookup = BuildLookup(allCards);

            var ventures = new VentureData[5];

            // FastFood - restaurant starter
            ventures[0] = CreateVenture(
                VentureType.FastFood,
                "Restaurant",
                "Start with a neighborhood restaurant. Quick traffic, review pressure.",
                FindCard(lookup, "FFC01_Grill"),
                FindCard(lookup, "FFC04_LineCook"),
                0
            );

            // Cafe - cafe starter
            ventures[1] = CreateVenture(
                VentureType.Cafe,
                "Cafe",
                "Start with a cozy cafe. Quality over quantity.",
                FindCard(lookup, "CAF01_EspressoBar"),
                FindCard(lookup, "CAF04_Barista"),
                0
            );

            // TechApp - tech app starter
            ventures[2] = CreateVenture(
                VentureType.TechApp,
                "Tech App",
                "Start with a tech app. Slow start, massive scale.",
                FindCard(lookup, "TEC01_AppLaunch"),
                FindCard(lookup, "TEC04_Developer"),
                0
            );

            // ClothingStore - clothing store starter
            ventures[3] = CreateVenture(
                VentureType.ClothingStore,
                "Clothing Store",
                "Start with a clothing store. Style meets strategy.",
                FindCard(lookup, "CLO01_Storefront"),
                FindCard(lookup, "CLO04_Tailor"),
                0
            );

            // GroceryStore - market starter
            ventures[4] = CreateVenture(
                VentureType.GroceryStore,
                "Market",
                "Start with a neighborhood market. Steady traffic, freshness pressure.",
                FindCard(lookup, "GRO01_FreshProduce"),
                FindCard(lookup, "GRO04_Stocker"),
                0
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
