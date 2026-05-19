using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap.Data
{
    /// <summary>
    /// Definitions for the 13 Action cards.
    /// </summary>
    public static class ActionCardDefs
    {
        public static CardData[] Create()
        {
            var cards = new CardData[13];

            // A01 - Flyer
            cards[0] = CardHelper.CreateAction("A01_ElIlani", "Flyer", Rarity.Common,
                "Free but weak. Starter card.", 0,
                ActionEffectType.AddCustomersToRandom, 3, 0f, 0,
                new[] { CardTag.Marketing, CardTag.Basic });

            // A02 - Small Investment
            cards[1] = CardHelper.CreateAction("A02_KucukYatirim", "Small Investment", Rarity.Common,
                "Quick cash. Starter card.", 0,
                ActionEffectType.AddMoneyInstant, 150, 0f, 0,
                new[] { CardTag.Finance, CardTag.Basic });

            // A03 - Viral Marketing
            cards[2] = CardHelper.CreateAction("A03_ViralPazarlama", "Viral Marketing", Rarity.Uncommon,
                "Explodes when played at the right turn.", 150,
                ActionEffectType.MultiplyAllCustomers, 0, 2f, 0,
                new[] { CardTag.Marketing, CardTag.Viral });

            // A04 - Hostile Takeover
            cards[3] = CardHelper.CreateAction("A04_DusmancaDevralma", "Hostile Takeover", Rarity.Rare,
                "Expensive but directly weakens rival.", 400,
                ActionEffectType.CloseRivalWeakestBusiness, 0, 0f, 0,
                new[] { CardTag.Aggressive });

            // A05 - Fake Reviews
            cards[4] = CardHelper.CreateAction("A05_SahteYorumlar", "Fake Reviews", Rarity.Uncommon,
                "Cheap customers. But risky.", 80,
                ActionEffectType.AddCustomersWithFBI, 8, 0f, 12,
                new[] { CardTag.Marketing, CardTag.Illegal });

            // A06 - Price Slashing
            cards[5] = CardHelper.CreateAction("A06_FiyatKirma", "Price Slashing", Rarity.Uncommon,
                "Sacrifice income, steal customers.", 0,
                ActionEffectType.StealCustomersHalfIncome, 8, 0f, 0,
                new[] { CardTag.Aggressive, CardTag.Pricing });
            cards[5].actionIncomeSacrifice = 0.5f;

            // A07 - Sabotage
            cards[6] = CardHelper.CreateAction("A07_Sabotaj", "Sabotage", Rarity.Rare,
                "Powerful but very risky.", 250,
                ActionEffectType.DisableRivalOneTurn, 0, 0f, 15,
                new[] { CardTag.Aggressive, CardTag.Illegal });

            // A08 - Investor Pitch
            cards[7] = CardHelper.CreateAction("A08_YatirimciSunumu", "Investor Pitch", Rarity.Uncommon,
                "Big money now. Pay later.", 0,
                ActionEffectType.MoneyNowPayLater, 600, 0f, 0,
                new[] { CardTag.Finance, CardTag.Investor });
            cards[7].actionDebtDuration = 3;
            cards[7].actionDebtPercent = 0.15f;

            // A09 - Emergency Hire
            cards[8] = CardHelper.CreateAction("A09_AcilIseAlim", "Emergency Hire", Rarity.Uncommon,
                "Quick employee. But random.", 100,
                ActionEffectType.DrawAndPlayEmployee, 0, 0f, 0,
                new[] { CardTag.Hiring });

            // A10 - Liquidation Sale
            cards[9] = CardHelper.CreateAction("A10_TasfiyeSatisi", "Liquidation Sale", Rarity.Common,
                "Last resort. Or strategic move.", 0,
                ActionEffectType.SacrificeBusiness, 0, 0f, 0,
                new[] { CardTag.Finance, CardTag.Desperate });

            // A11 - Hostile Merger
            // DECISION: Swap your WEAKEST business with the rival's STRONGEST.
            // Incredible if you have a Pop-Up Shop about to expire or a failing Diner.
            // Terrible if your board is already strong. Timing is everything.
            cards[10] = CardHelper.CreateAction("A11_DusmancaBirlesme", "Hostile Merger", Rarity.Rare,
                "Swap your weakest business with the rival's strongest. Bold move.", 350,
                ActionEffectType.SwapBusinessWithRival, 0, 0f, 0,
                new[] { CardTag.Aggressive, CardTag.Finance });

            // A12 - All In
            // DECISION: 50/50 gamble. Double your current money or lose half.
            // Incredible when broke (double a small amount = low risk).
            // Terrifying when rich (lose half of 2000 = devastating).
            // Creates a "when do I play this?" tension every time it is in hand.
            cards[11] = CardHelper.CreateAction("A12_HepsiYaHic", "All In", Rarity.Rare,
                "Flip a coin. Heads: double your money. Tails: lose half.", 0,
                ActionEffectType.GambleDoubleOrNothing, 0, 2.0f, 0,
                new[] { CardTag.Finance, CardTag.Risky, CardTag.Crypto });

            // A13 - Insurance Claim
            // DECISION: Spend 200 to make one business immune to events and rival attacks
            // for 1 turn. Incredible before an Economic Crisis or when rival is aggressive.
            // Wasted if nothing bad happens. Rewards prediction and scouting.
            cards[12] = CardHelper.CreateAction("A13_SigortaTalebi", "Insurance Claim", Rarity.Uncommon,
                "One business is untouchable this turn. Worth it?", 200,
                ActionEffectType.ProtectBusinessOneTurn, 0, 0f, 0,
                new[] { CardTag.Finance, CardTag.Defensive });

            return cards;
        }
    }
}
