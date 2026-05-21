using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap.Data
{
    public class GameDataBundle
    {
        public CardData[] allCards;
        public CardData[] shopPool;
        public SectorProfile[] sectorProfiles;
        public Dictionary<string, CardData> cardLookup;
    }

    public static class ContentFactory
    {
        public static GameDataBundle CreateAllData()
        {
            var allCards = CreateAllCards();
            var lookup = new Dictionary<string, CardData>(allCards.Length);
            var shop = new List<CardData>();

            foreach (var card in allCards)
            {
                lookup[card.cardId] = card;
                // Risk and Crisis cards are not in the shop -- they trigger via events
                if (card.cardFamily != CardFamily.Risk && card.cardFamily != CardFamily.Crisis)
                    shop.Add(card);
            }

            var profiles = CreateSectorProfiles();

            var bundle = new GameDataBundle
            {
                allCards = allCards,
                shopPool = shop.ToArray(),
                sectorProfiles = profiles,
                cardLookup = lookup
            };

            Debug.Log($"[ContentFactory] Created {allCards.Length} cards, {shop.Count} in shop, {profiles.Length} sector profiles.");
            return bundle;
        }

        // ── Card Creation ───────────────────────────────────────────

        static CardData[] CreateAllCards()
        {
            var cards = new List<CardData>();
            cards.AddRange(CreateKitchenCards());
            cards.AddRange(CreateStaffCards());
            cards.AddRange(CreateMarketingCards());
            cards.AddRange(CreateSupplierCards());
            cards.AddRange(CreateRiskCards());
            cards.AddRange(CreateReactionCards());
            cards.AddRange(CreateCrisisCards());
            return cards.ToArray();
        }

        // ── Kitchen / Operation ─────────────────────────────────────

        static CardData[] CreateKitchenCards()
        {
            return new[]
            {
                Make("kitchen_small_shop", "Kucuk Dukkan", CardType.Operation, CardFamily.Setup,
                    Rarity.Common, SlotType.Kitchen,
                    buyCost: 80, capacityDelta: 2f, qualityDelta: 1f),

                Make("kitchen_extra_table", "Ekstra Masa", CardType.Operation, CardFamily.Setup,
                    Rarity.Common, SlotType.Kitchen,
                    buyCost: 40, capacityDelta: 1.5f),

                Make("kitchen_grill", "Izgara Istasyonu", CardType.Operation, CardFamily.Growth,
                    Rarity.Uncommon, SlotType.Kitchen,
                    buyCost: 60, qualityDelta: 1.2f, capacityDelta: 1f),
            };
        }

        // ── Staff ───────────────────────────────────────────────────

        static CardData[] CreateStaffCards()
        {
            return new[]
            {
                Make("staff_rookie_cook", "Cylak Asci", CardType.Staff, CardFamily.Setup,
                    Rarity.Common, SlotType.Salon,
                    buyCost: 0, upkeep: 8, qualityDelta: 0.5f,
                    staffTier: StaffTier.Intern, canPromote: true),

                Make("staff_cheap_waiter", "Ucuz Garson", CardType.Staff, CardFamily.Setup,
                    Rarity.Common, SlotType.Salon,
                    buyCost: 0, upkeep: 5, capacityDelta: 0.5f,
                    staffTier: StaffTier.Intern, canPromote: true),

                Make("staff_cleaner", "Temizlikci", CardType.Staff, CardFamily.Setup,
                    Rarity.Common, SlotType.Salon,
                    buyCost: 0, upkeep: 6, hygieneDelta: 1.5f,
                    staffTier: StaffTier.Junior),
            };
        }

        // ── Marketing ───────────────────────────────────────────────

        static CardData[] CreateMarketingCards()
        {
            return new[]
            {
                Make("mkt_brosur", "Brosur Dagitimi", CardType.Marketing, CardFamily.Setup,
                    Rarity.Common, SlotType.Marketing,
                    buyCost: 15, demandDelta: 1f),

                Make("mkt_google_ad", "Google Reklam", CardType.Marketing, CardFamily.Growth,
                    Rarity.Uncommon, SlotType.Marketing,
                    buyCost: 25, upkeep: 25, demandDelta: 2.5f),
            };
        }

        // ── Supplier ────────────────────────────────────────────────

        static CardData[] CreateSupplierCards()
        {
            return new[]
            {
                Make("sup_butcher", "Kasap (Hal'den Taze Et)", CardType.Supplier, CardFamily.Setup,
                    Rarity.Common, SlotType.Storage,
                    buyCost: 0, upkeep: 20, qualityDelta: 1.5f, hygieneDelta: 0.5f),

                Make("sup_frozen", "Ucuz Donmus Et", CardType.Supplier, CardFamily.Setup,
                    Rarity.Common, SlotType.Storage,
                    buyCost: 0, upkeep: 5, qualityDelta: 0.3f, hygieneDelta: -0.3f),
            };
        }

        // ── Risk ────────────────────────────────────────────────────

        static CardData[] CreateRiskCards()
        {
            return new[]
            {
                Make("risk_fake_review", "Sahte Yorum", CardType.Risk, CardFamily.Risk,
                    Rarity.Common, SlotType.TempEffect,
                    buyCost: 15, ratingDelta: 0.8f,
                    shortTermBenefit: 0.8f, longTermRisk: 25f,
                    triggersCrisis: CrisisType.ReviewBomb),

                Make("risk_uninsured", "Sigortasiz Calisan", CardType.Risk, CardFamily.Risk,
                    Rarity.Common, SlotType.TempEffect,
                    buyCost: 0, legalRiskDelta: 5f,
                    shortTermBenefit: 0.4f, longTermRisk: 15f,
                    triggersCrisis: CrisisType.HygieneInspection),
            };
        }

        // ── Reaction ────────────────────────────────────────────────

        static CardData[] CreateReactionCards()
        {
            return new[]
            {
                Make("react_emergency_clean", "Acil Temizlik", CardType.Reaction, CardFamily.Reaction,
                    Rarity.Common, SlotType.TempEffect,
                    buyCost: 15, hygieneDelta: 3f,
                    duration: 1, tags: new[] { "hygiene_crisis" }),

                Make("react_apology", "Ozur Kampanyasi", CardType.Reaction, CardFamily.Reaction,
                    Rarity.Common, SlotType.TempEffect,
                    buyCost: 25, ratingDelta: 0.5f,
                    duration: 1, tags: new[] { "rating_crisis" }),
            };
        }

        // ── Crisis ──────────────────────────────────────────────────

        static CardData[] CreateCrisisCards()
        {
            return new[]
            {
                Make("crisis_review_bomb", "Kotu Yorum Patlamasi", CardType.Crisis, CardFamily.Crisis,
                    Rarity.Uncommon, SlotType.TempEffect,
                    ratingDelta: -1f, demandDelta: -2f,
                    crisisType: CrisisType.ReviewBomb, crisisDuration: 2,
                    solutionTags: new[] { "rating_crisis" }),

                Make("crisis_hygiene_inspect", "Hijyen Denetimi", CardType.Crisis, CardFamily.Crisis,
                    Rarity.Uncommon, SlotType.TempEffect,
                    legalRiskDelta: 15f,
                    crisisType: CrisisType.HygieneInspection, crisisDuration: 2,
                    solutionTags: new[] { "hygiene_crisis", "legal_crisis" }),
            };
        }

        // ── Sector Profiles ─────────────────────────────────────────

        static SectorProfile[] CreateSectorProfiles()
        {
            var restaurant = ScriptableObject.CreateInstance<SectorProfile>();
            restaurant.sectorType = SectorType.Restaurant;
            restaurant.displayName = "Restaurant";
            restaurant.description = "Yemek sektoru. Mutfak, salon, depo yonet.";

            restaurant.startingCash       = Constants.STARTING_CASH;
            restaurant.startingDemand     = Constants.STARTING_DEMAND;
            restaurant.startingCapacity   = Constants.STARTING_CAPACITY;
            restaurant.startingQuality    = Constants.STARTING_QUALITY;
            restaurant.startingRating     = Constants.STARTING_RATING;
            restaurant.startingStability  = Constants.STARTING_STABILITY;
            restaurant.startingLegalRisk  = Constants.STARTING_LEGAL_RISK;
            restaurant.startingMarketShare = Constants.STARTING_MARKET_SHARE;
            restaurant.startingHygiene    = Constants.STARTING_HYGIENE;

            restaurant.kitchenSubSlots   = new[] { "Ana Ocak", "Izgara", "Hazirlik Tezgahi", "Firinda", "Tatli Istasyonu" };
            restaurant.salonSubSlots     = new[] { "Asci", "Garson", "Kasiyer", "Temizlikci", "Kurye", "Sube Muduru", "Komi" };
            restaurant.storageSubSlots   = new[] { "Kasap", "Manav", "Firinci", "Icecek", "Toptan Gida" };
            restaurant.marketingSubSlots = new[] { "Brosur", "Google", "Instagram", "Yemek App", "Yerel Etkinlik" };

            // GDD v5 Section 4.2: Slot counts per era
            restaurant.eraSlotLayouts = new EraSlotLayout[]
            {
                new EraSlotLayout { era = Era.Garage,    kitchenSlots = 2, salonSlots = 3, storageSlots = 1, marketingSlots = 1, tempEffectSlots = 2, actionsPerTurn = Constants.ACTIONS_ERA_1 },
                new EraSlotLayout { era = Era.Growth,    kitchenSlots = 3, salonSlots = 4, storageSlots = 2, marketingSlots = 2, tempEffectSlots = 3, actionsPerTurn = Constants.ACTIONS_ERA_2 },
                new EraSlotLayout { era = Era.Scale,     kitchenSlots = 4, salonSlots = 6, storageSlots = 2, marketingSlots = 3, tempEffectSlots = 3, actionsPerTurn = Constants.ACTIONS_ERA_3 },
                new EraSlotLayout { era = Era.Dominance, kitchenSlots = 5, salonSlots = 7, storageSlots = 3, marketingSlots = 3, tempEffectSlots = 3, actionsPerTurn = Constants.ACTIONS_ERA_4 },
            };

            // GDD v5 Section 7.1: Season multipliers for restaurant
            restaurant.seasonMultipliers = new[]
            {
                Constants.SEASON_SPRING,
                Constants.SEASON_SUMMER,
                Constants.SEASON_AUTUMN,
                Constants.SEASON_WINTER,
                Constants.SEASON_RAMADAN
            };

            restaurant.derivedMetricNames = new[]
            {
                "Malzeme Kalitesi", "Servis Hizi", "Hijyen", "Google Puani"
            };

            return new[] { restaurant };
        }

        // ── Card Builder Helper ─────────────────────────────────────

        static CardData Make(
            string id, string name, CardType type, CardFamily family,
            Rarity rarity, SlotType targetSlot,
            int buyCost = 0, int upkeep = 0,
            float demandDelta = 0f, float capacityDelta = 0f,
            float qualityDelta = 0f, float ratingDelta = 0f,
            float hygieneDelta = 0f, float staffStabilityDelta = 0f,
            float legalRiskDelta = 0f, float cashPerTurn = 0f,
            StaffTier staffTier = StaffTier.Intern, bool canPromote = false,
            float shortTermBenefit = 0f, float longTermRisk = 0f,
            CrisisType triggersCrisis = CrisisType.None,
            CrisisType crisisType = CrisisType.None,
            int crisisDuration = 0, string[] solutionTags = null,
            int duration = 0, string[] tags = null)
        {
            var card = ScriptableObject.CreateInstance<CardData>();
            card.cardId            = id;
            card.cardName          = name;
            card.cardType          = type;
            card.cardFamily        = family;
            card.rarity            = rarity;
            card.sector            = SectorType.Restaurant;
            card.isNeutral         = false;
            card.buyCost           = buyCost;
            card.upkeepPerTurn     = upkeep;
            card.targetSlot        = targetSlot;
            card.demandDelta       = demandDelta;
            card.capacityDelta     = capacityDelta;
            card.qualityDelta      = qualityDelta;
            card.ratingDelta       = ratingDelta;
            card.hygieneDelta      = hygieneDelta;
            card.staffStabilityDelta = staffStabilityDelta;
            card.legalRiskDelta    = legalRiskDelta;
            card.cashPerTurn       = cashPerTurn;
            card.startingTier      = staffTier;
            card.canPromote        = canPromote;
            card.shortTermBenefit  = shortTermBenefit;
            card.longTermRisk      = longTermRisk;
            card.triggersCrisis    = triggersCrisis;
            card.crisisType        = crisisType;
            card.crisisDuration    = crisisDuration;
            card.solutionTags      = solutionTags ?? System.Array.Empty<string>();
            card.duration          = duration;
            card.tags              = tags ?? System.Array.Empty<string>();
            return card;
        }
    }
}
