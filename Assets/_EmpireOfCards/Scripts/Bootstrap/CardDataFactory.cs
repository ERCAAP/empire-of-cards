using UnityEngine;
using System.Collections.Generic;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Bootstrap.Data;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Thin orchestrator that delegates card/combo/balance creation
    /// to focused definition classes in Bootstrap.Data.
    /// </summary>
    public static class CardDataFactory
    {
        public static GameDataBundle CreateAllData()
        {
            CardHelper.BeginSession();

            var allCards = V4ContentFactory.CreateCards();
            var boardProfiles = V4ContentFactory.CreateBoardProfiles();
            var deckProfiles = V4ContentFactory.CreateDeckProfiles();
            var economyProfiles = V4ContentFactory.CreateEconomyProfiles();

            // Combos, balance, deck, rival, shop
            var combos  = new ComboData[0];
            var balance = BalanceDefs.CreateBalance();
            var deck    = BalanceDefs.CreateDeck(allCards);
            var rival   = BalanceDefs.CreateRival();
            var shop = new List<CardData>();
            foreach (var card in allCards)
            {
                if (card == null) continue;
                if (card.cardFamily == CardFamily.Crisis) continue;
                shop.Add(card);
            }

            var meta = BalanceDefs.CreateMetaProgression();

            var ventures = V4ContentFactory.CreateVentures(allCards, boardProfiles, deckProfiles, economyProfiles);

            var lookup = CardHelper.EndSession();

            var bundle = new GameDataBundle
            {
                allCards    = allCards,
                combos      = combos,
                balanceData = balance,
                startingDeck = deck,
                rivalData   = rival,
                shopPool    = shop.ToArray(),
                cardLookup  = lookup,
                metaProgressionData = meta,
                ventures    = ventures,
                boardProfiles = boardProfiles,
                deckProfiles = deckProfiles,
                economyProfiles = economyProfiles
            };

            Debug.Log($"[CardDataFactory] Data created: {bundle.allCards.Length} cards, " +
                      $"{bundle.combos.Length} combos, shop pool: {bundle.shopPool.Length} cards, " +
                      $"{bundle.ventures.Length} ventures.");

            return bundle;
        }

        private static CardData[] Combine(params CardData[][] groups)
        {
            int total = 0;
            for (int i = 0; i < groups.Length; i++) total += groups[i].Length;

            var result = new CardData[total];
            int idx = 0;
            for (int i = 0; i < groups.Length; i++)
            {
                var g = groups[i];
                for (int j = 0; j < g.Length; j++)
                    result[idx++] = g[j];
            }
            return result;
        }
    }

    /// <summary>
    /// Holds all game data created by CardDataFactory.
    /// </summary>
    public class GameDataBundle
    {
        public CardData[] allCards;
        public CardData[] shopPool;
        public GameBalanceData balanceData;
        public DeckPresetData startingDeck;
        public RivalData rivalData;
        public ComboData[] combos;
        public Dictionary<string, CardData> cardLookup;
        public MetaProgressionData metaProgressionData;
        public VentureData[] ventures;
        public VentureBoardProfile[] boardProfiles;
        public VentureDeckProfile[] deckProfiles;
        public VentureEconomyProfile[] economyProfiles;
    }
}
