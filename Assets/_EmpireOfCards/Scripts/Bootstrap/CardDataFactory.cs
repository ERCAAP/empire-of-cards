using UnityEngine;
using System.Collections.Generic;
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

            // Cards
            var biz = BusinessCardDefs.Create();
            var emp = EmployeeCardDefs.Create();
            var act = ActionCardDefs.Create();
            var upg = UpgradeCardDefs.Create();
            var evt = EventCardDefs.Create();

            var allCards = Combine(biz, emp, act, upg, evt);

            // Combos, balance, deck, rival, shop
            var combos  = ComboDefs.Create(allCards);
            var balance = BalanceDefs.CreateBalance();
            var deck    = BalanceDefs.CreateDeck(allCards);
            var rival   = BalanceDefs.CreateRival();
            var shop    = BalanceDefs.CreateShopPool(allCards, deck);

            var meta = BalanceDefs.CreateMetaProgression();

            var lookup = CardHelper.EndSession();

            var bundle = new GameDataBundle
            {
                allCards    = allCards,
                combos      = combos,
                balanceData = balance,
                startingDeck = deck,
                rivalData   = rival,
                shopPool    = shop,
                cardLookup  = lookup,
                metaProgressionData = meta
            };

            Debug.Log($"[CardDataFactory] Data created: {bundle.allCards.Length} cards, " +
                      $"{bundle.combos.Length} combos, shop pool: {bundle.shopPool.Length} cards.");

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
    }
}
