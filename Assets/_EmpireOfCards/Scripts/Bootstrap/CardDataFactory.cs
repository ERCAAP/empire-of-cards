using UnityEngine;
using System.Collections.Generic;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Bootstrap.Data;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Thin orchestrator that delegates card/balance creation
    /// to focused definition classes in Bootstrap.Data.
    /// </summary>
    public static class CardDataFactory
    {
        private static readonly VentureType[] LaunchSurfaceVentures =
        {
            VentureType.FastFood,
            VentureType.Cafe,
            VentureType.GroceryStore
        };

        public static GameDataBundle CreateAllData()
        {
            CardHelper.BeginSession();

            var allCards = V4ContentFactory.CreateCards();
            var boardProfiles = V4ContentFactory.CreateBoardProfiles();
            var deckProfiles = V4ContentFactory.CreateDeckProfiles();
            var economyProfiles = V4ContentFactory.CreateEconomyProfiles();

            // Balance, rival, shop
            var balance = BalanceDefs.CreateBalance();
            var rival   = BalanceDefs.CreateRival();
            var shop = new List<CardData>();
            foreach (var card in allCards)
            {
                if (card == null) continue;
                if (card.cardFamily == CardFamily.Crisis) continue;
                shop.Add(card);
            }

            var ventures = V4ContentFactory.CreateVentures(allCards, boardProfiles, deckProfiles, economyProfiles);

            var lookup = CardHelper.EndSession();

            var bundle = new GameDataBundle
            {
                allCards    = allCards,
                balanceData = balance,
                rivalData   = rival,
                shopPool    = shop.ToArray(),
                cardLookup  = lookup,
                ventures    = ventures,
                boardProfiles = boardProfiles,
                deckProfiles = deckProfiles,
                economyProfiles = economyProfiles
            };

            Debug.Log($"[CardDataFactory] Data created: {bundle.allCards.Length} cards, " +
                      $"shop pool: {bundle.shopPool.Length} cards, " +
                      $"{bundle.ventures.Length} ventures.");

            return bundle;
        }

        public static VentureData[] CreateLaunchSurfaceVentures(VentureData[] allVentures)
        {
            if (allVentures == null || allVentures.Length == 0)
                return System.Array.Empty<VentureData>();

            var filtered = new List<VentureData>(LaunchSurfaceVentures.Length);
            for (int i = 0; i < LaunchSurfaceVentures.Length; i++)
            {
                VentureType type = LaunchSurfaceVentures[i];
                for (int j = 0; j < allVentures.Length; j++)
                {
                    VentureData venture = allVentures[j];
                    if (venture != null && venture.ventureType == type)
                    {
                        filtered.Add(venture);
                        break;
                    }
                }
            }

            return filtered.ToArray();
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
        public RivalData rivalData;
        public Dictionary<string, CardData> cardLookup;
        public VentureData[] ventures;
        public VentureBoardProfile[] boardProfiles;
        public VentureDeckProfile[] deckProfiles;
        public VentureEconomyProfile[] economyProfiles;
    }
}
