using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap.Data
{
    public static class V4ContentFactory
    {
        public static CardData[] CreateCards()
        {
            var cards = new List<CardData>();

            cards.AddRange(CreateFastFoodCards());
            cards.AddRange(CreateCafeCards());
            cards.AddRange(CreateTechCards());
            cards.AddRange(CreateClothingCards());
            cards.AddRange(CreateGroceryCards());
            cards.AddRange(CreateNeutralCards());

            return cards.ToArray();
        }

        public static VentureBoardProfile[] CreateBoardProfiles()
        {
            return new[]
            {
                CreateBoardProfile(
                    VentureType.FastFood, "Fast Food",
                    new[] { ("kitchen", "Kitchen"), ("service", "Service"), ("seating", "Seating"), ("delivery", "Delivery") },
                    new[] { ("chef", "Chef"), ("cashier", "Cashier"), ("courier", "Courier"), ("cleaning", "Cleaning"), ("manager", "Manager") },
                    new[] { ("flyers", "Flyers"), ("google", "Google"), ("social", "Social"), ("platform", "Delivery App") },
                    new[] { ("butcher", "Butcher"), ("vegetable", "Greengrocer"), ("bakery", "Bakery"), ("drinks", "Drinks") }),
                CreateBoardProfile(
                    VentureType.Cafe, "Cafe",
                    new[] { ("bar", "Bar"), ("seating", "Seating"), ("takeaway", "Takeaway"), ("pastry", "Pastry") },
                    new[] { ("barista", "Barista"), ("cashier", "Cashier"), ("floor", "Floor"), ("cleaning", "Cleaning"), ("lead", "Lead") },
                    new[] { ("instagram", "Instagram"), ("reels", "Reels"), ("loyalty", "Loyalty"), ("maps", "Maps") },
                    new[] { ("beans", "Beans"), ("milk", "Milk"), ("desserts", "Desserts"), ("ambience", "Ambience") }),
                CreateBoardProfile(
                    VentureType.TechApp, "Tech App",
                    new[] { ("product", "Product"), ("backend", "Backend"), ("growth", "Growth"), ("support", "Support") },
                    new[] { ("developer", "Developer"), ("designer", "Designer"), ("growthhire", "Growth"), ("supporthire", "Support"), ("pm", "PM") },
                    new[] { ("aso", "ASO"), ("ads", "Ads"), ("community", "Community"), ("creator", "Creator") },
                    new[] { ("cloud", "Cloud"), ("analytics", "Analytics"), ("payments", "Payments"), ("api", "API") }),
                CreateBoardProfile(
                    VentureType.ClothingStore, "Clothing Store",
                    new[] { ("showcase", "Showcase"), ("inventory", "Inventory"), ("checkout", "Checkout"), ("online", "Online") },
                    new[] { ("sales", "Sales"), ("tailor", "Tailor"), ("cashier", "Cashier"), ("stocker", "Stocker"), ("manager", "Manager") },
                    new[] { ("instagram", "Instagram"), ("influencer", "Influencer"), ("discount", "Indirim"), ("shoppingads", "Shopping Ads") },
                    new[] { ("wholesale", "Wholesale"), ("atelier", "Atelier"), ("shipping", "Shipping"), ("studio", "Studio") }),
                CreateBoardProfile(
                    VentureType.GroceryStore, "Grocery Store",
                    new[] { ("shelves", "Shelves"), ("fresh", "Fresh"), ("checkout", "Checkout"), ("delivery", "WhatsApp") },
                    new[] { ("cashier", "Cashier"), ("freshkeeper", "Fresh Keeper"), ("stocker", "Stocker"), ("courier", "Courier"), ("lead", "Shift Lead") },
                    new[] { ("whatsapp", "WhatsApp"), ("posters", "Posters"), ("loyalty", "Loyalty"), ("latenight", "Late Night") },
                    new[] { ("market", "Wholesale Market"), ("distributor", "Distributor"), ("dairy", "Dairy"), ("bakery", "Bakery") })
            };
        }

        public static VentureEconomyProfile[] CreateEconomyProfiles()
        {
            return new[]
            {
                CreateEconomyProfile(VentureType.FastFood, 550f, 3f, 4f, 3.2f, 3.2f, 6.5f, "ingredient_quality", "Ingredient Quality", 4f, "service_speed", "Service Speed", 4f, "hygiene", "Hygiene", 5f),
                CreateEconomyProfile(VentureType.Cafe, 575f, 2.8f, 3.5f, 3.8f, 3.4f, 7f, "bean_quality", "Bean Quality", 4.2f, "consistency", "Drink Consistency", 4f, "ambience", "Ambience", 5.2f),
                CreateEconomyProfile(VentureType.TechApp, 520f, 1.5f, 2.5f, 3f, 3f, 6.2f, "stability", "App Stability", 4f, "churn", "Churn", 2.5f, "infra_cost", "Infra Cost", 3f),
                CreateEconomyProfile(VentureType.ClothingStore, 600f, 2.4f, 3.2f, 3.6f, 3.1f, 6.4f, "stock_health", "Stock Health", 4.5f, "season_fit", "Season Fit", 4f, "return_pressure", "Return Pressure", 2.5f),
                CreateEconomyProfile(VentureType.GroceryStore, 530f, 3.4f, 4.1f, 3.1f, 3.3f, 6.8f, "spoilage", "Spoilage", 2.2f, "credit_ledger", "Credit Ledger", 2f, "local_loyalty", "Local Loyalty", 4.4f)
            };
        }

        public static VentureDeckProfile[] CreateDeckProfiles()
        {
            return new[]
            {
                CreateDeckProfile(VentureType.FastFood,
                    new[] { "FF02", "FF03", "FF04", "FF05", "FF06", "NT01" },
                    new[] { "FF01", "FF02", "FF03", "FF04", "FF05", "FF06", "FF07", "FF08", "NT01", "NT02" },
                    new[] { "FF02", "FF04", "FF05", "FF06", "FF07", "FF08", "NT02", "NT03" },
                    new[] { "FF04", "FF06", "FF07", "FF08", "FF09", "NT03" },
                    new[] { "NT01", "NT02", "NT03", "NT04", "NT05" },
                    new[] { "FF09" }),
                CreateDeckProfile(VentureType.Cafe,
                    new[] { "CF02", "CF03", "CF04", "CF05", "CF06", "NT01" },
                    new[] { "CF01", "CF02", "CF03", "CF04", "CF05", "CF06", "CF07", "CF08", "NT01", "NT04" },
                    new[] { "CF02", "CF04", "CF05", "CF06", "CF07", "CF08", "NT02", "NT04" },
                    new[] { "CF04", "CF06", "CF07", "CF08", "CF09", "NT03" },
                    new[] { "NT01", "NT02", "NT03", "NT04", "NT05" },
                    new[] { "CF09" }),
                CreateDeckProfile(VentureType.TechApp,
                    new[] { "TC02", "TC03", "TC04", "TC05", "TC06", "NT02" },
                    new[] { "TC01", "TC02", "TC03", "TC04", "TC05", "TC06", "TC07", "TC08", "NT02", "NT04" },
                    new[] { "TC02", "TC04", "TC05", "TC06", "TC07", "TC08", "NT02", "NT03" },
                    new[] { "TC04", "TC06", "TC07", "TC08", "TC09", "NT05" },
                    new[] { "NT01", "NT02", "NT03", "NT04", "NT05" },
                    new[] { "TC09" }),
                CreateDeckProfile(VentureType.ClothingStore,
                    new[] { "CL02", "CL03", "CL04", "CL05", "CL06", "NT01" },
                    new[] { "CL01", "CL02", "CL03", "CL04", "CL05", "CL06", "CL07", "CL08", "NT01", "NT04" },
                    new[] { "CL02", "CL04", "CL05", "CL06", "CL07", "CL08", "NT03", "NT04" },
                    new[] { "CL04", "CL06", "CL07", "CL08", "CL09", "NT05" },
                    new[] { "NT01", "NT02", "NT03", "NT04", "NT05" },
                    new[] { "CL09" }),
                CreateDeckProfile(VentureType.GroceryStore,
                    new[] { "GR02", "GR03", "GR04", "GR05", "GR06", "NT01" },
                    new[] { "GR01", "GR02", "GR03", "GR04", "GR05", "GR06", "GR07", "GR08", "NT01", "NT04" },
                    new[] { "GR02", "GR04", "GR05", "GR06", "GR07", "GR08", "NT02", "NT03" },
                    new[] { "GR04", "GR06", "GR07", "GR08", "GR09", "NT05" },
                    new[] { "NT01", "NT02", "NT03", "NT04", "NT05" },
                    new[] { "GR09" })
            };
        }

        public static VentureData[] CreateVentures(
            CardData[] allCards,
            VentureBoardProfile[] boardProfiles,
            VentureDeckProfile[] deckProfiles,
            VentureEconomyProfile[] economyProfiles)
        {
            var byId = new Dictionary<string, CardData>();
            foreach (var card in allCards)
                byId[card.cardId] = card;

            return new[]
            {
                CreateVenture(VentureType.FastFood, "Fast Food", "High traffic, high pressure. Win by balancing growth and kitchen capacity.", "Fast local scaling with review risk.", byId["FF01"], boardProfiles[0], deckProfiles[0], economyProfiles[0]),
                CreateVenture(VentureType.Cafe, "Cafe", "Loyalty and quality driven. Build social proof and neighborhood habits.", "Premium quality and sticky regulars.", byId["CF01"], boardProfiles[1], deckProfiles[1], economyProfiles[1]),
                CreateVenture(VentureType.TechApp, "Tech App", "Slow start, explosive upside. Stability and rating matter before scale.", "Product, growth, and retention balancing act.", byId["TC01"], boardProfiles[2], deckProfiles[2], economyProfiles[2]),
                CreateVenture(VentureType.ClothingStore, "Clothing Store", "Seasonal demand, inventory pressure, visual merchandising wars.", "Trend and stock management duel.", byId["CL01"], boardProfiles[3], deckProfiles[3], economyProfiles[3]),
                CreateVenture(VentureType.GroceryStore, "Grocery Store", "Low margin, repeat traffic, spoilage and neighborhood loyalty.", "Stable demand with tight operational margins.", byId["GR01"], boardProfiles[4], deckProfiles[4], economyProfiles[4])
            };
        }

        private static VentureData CreateVenture(
            VentureType type,
            string name,
            string description,
            string playstyle,
            CardData startingCard,
            VentureBoardProfile boardProfile,
            VentureDeckProfile deckProfile,
            VentureEconomyProfile economyProfile)
        {
            var venture = ScriptableObject.CreateInstance<VentureData>();
            venture.ventureType = type;
            venture.ventureName = name;
            venture.description = description;
            venture.playstyleSummary = playstyle;
            venture.startingBusiness = startingCard;
            venture.boardProfile = boardProfile;
            venture.deckProfile = deckProfile;
            venture.economyProfile = economyProfile;
            venture.name = $"Venture_{type}";
            return venture;
        }

        private static VentureBoardProfile CreateBoardProfile(
            VentureType type,
            string displayName,
            (string id, string label)[] operations,
            (string id, string label)[] staff,
            (string id, string label)[] marketing,
            (string id, string label)[] suppliers)
        {
            var profile = ScriptableObject.CreateInstance<VentureBoardProfile>();
            profile.ventureType = type;
            profile.displayName = displayName;
            profile.operationSubSlots = ToSubSlots(operations);
            profile.staffSubSlots = ToSubSlots(staff);
            profile.marketingSubSlots = ToSubSlots(marketing);
            profile.supplierSubSlots = ToSubSlots(suppliers);
            profile.unlockSteps = new[]
            {
                new SlotUnlockStep { id = $"{type}_tier2", requiredTurn = 6, marketingDelta = 1 },
                new SlotUnlockStep { id = $"{type}_tier3", requiredTurn = 12, operationDelta = 2, staffDelta = 1 },
                new SlotUnlockStep { id = $"{type}_tier4", requiredTurn = 18, supplierDelta = 1, staffDelta = 1 }
            };
            profile.name = $"BoardProfile_{type}";
            return profile;
        }

        private static VentureEconomyProfile CreateEconomyProfile(
            VentureType type,
            float startingCash,
            float startingDemand,
            float startingCapacity,
            float startingQuality,
            float startingRating,
            float startingStaffStability,
            string metric1Id,
            string metric1Label,
            float metric1Start,
            string metric2Id,
            string metric2Label,
            float metric2Start,
            string metric3Id,
            string metric3Label,
            float metric3Start)
        {
            var profile = ScriptableObject.CreateInstance<VentureEconomyProfile>();
            profile.ventureType = type;
            profile.startingCash = startingCash;
            profile.startingDemand = startingDemand;
            profile.startingCapacity = startingCapacity;
            profile.startingQuality = startingQuality;
            profile.startingRating = startingRating;
            profile.startingStaffStability = startingStaffStability;
            profile.startingMarketShare = 12f;
            profile.derivedMetrics = new[]
            {
                new DerivedMetricRule { id = metric1Id, labelKey = $"metric.{metric1Id}", fallbackLabel = metric1Label, startingValue = metric1Start },
                new DerivedMetricRule { id = metric2Id, labelKey = $"metric.{metric2Id}", fallbackLabel = metric2Label, startingValue = metric2Start },
                new DerivedMetricRule { id = metric3Id, labelKey = $"metric.{metric3Id}", fallbackLabel = metric3Label, startingValue = metric3Start }
            };
            profile.name = $"EconomyProfile_{type}";
            return profile;
        }

        private static VentureDeckProfile CreateDeckProfile(
            VentureType type,
            string[] starter,
            string[] early,
            string[] mid,
            string[] late,
            string[] neutral,
            string[] crisis)
        {
            var profile = ScriptableObject.CreateInstance<VentureDeckProfile>();
            profile.ventureType = type;
            profile.starterCardIds = starter;
            profile.earlyPoolCardIds = early;
            profile.midPoolCardIds = mid;
            profile.latePoolCardIds = late;
            profile.neutralCardIds = neutral;
            profile.crisisCardIds = crisis;
            profile.drawBiasRules = new[]
            {
                new DeckBiasRule { pressure = BoardPressureType.LowDemand, preferredFamily = CardFamily.Growth, bonusWeight = 2.5f },
                new DeckBiasRule { pressure = BoardPressureType.CapacityShortfall, preferredFamily = CardFamily.Setup, bonusWeight = 2.5f },
                new DeckBiasRule { pressure = BoardPressureType.LowRating, preferredFamily = CardFamily.Reaction, bonusWeight = 3f },
                new DeckBiasRule { pressure = BoardPressureType.HighLegalRisk, preferredFamily = CardFamily.Reaction, bonusWeight = 2.5f },
                new DeckBiasRule { pressure = BoardPressureType.WeakQuality, preferredFamily = CardFamily.Setup, bonusWeight = 2f }
            };
            profile.name = $"DeckProfile_{type}";
            return profile;
        }

        private static BoardSubSlotDefinition[] ToSubSlots((string id, string label)[] input)
        {
            var result = new BoardSubSlotDefinition[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                result[i] = new BoardSubSlotDefinition
                {
                    id = input[i].id,
                    fallbackLabel = input[i].label,
                    labelKey = $"slot.{input[i].id}"
                };
            }
            return result;
        }

        private static IEnumerable<CardData> CreateFastFoodCards()
        {
            yield return MakeOperation("FF01", "Street Counter", VentureType.FastFood, "kitchen", "Small but efficient fast food setup.", 90, 2.5f, 2.5f, 1.2f, 1.8f, 4f, 5f, 4f, new[] { CardTag.Food, CardTag.Basic });
            yield return MakeOperation("FF02", "Extra Tables", VentureType.FastFood, "seating", "More seats for local traffic.", 70, 1f, 1.8f, 0.5f, 0f, 3.5f, 4f, 4f, new[] { CardTag.Food, CardTag.Scaling });
            yield return MakeStaff("FF03", "Line Cook", VentureType.FastFood, "chef", "Keeps the rush moving.", 45, 0.6f, 1.8f, 0.5f, 0.8f, new[] { CardTag.Food, CardTag.Support });
            yield return MakeSupplier("FF04", "Premium Butcher", VentureType.FastFood, "butcher", "Better meat, better reviews.", 60, 0.4f, 1.7f, 0.3f, 0.2f, new[] { "ingredient_quality" }, new[] { CardTag.Food, CardTag.Organic });
            yield return MakeMarketing("FF05", "Flyer Team", VentureType.FastFood, "flyers", "Push foot traffic from the neighborhood.", 35, 2.2f, 0.2f, 15f, new[] { CardTag.Marketing, CardTag.Basic });
            yield return MakeMarketing("FF06", "Google Business Push", VentureType.FastFood, "google", "Improves local search and reviews.", 45, 1.2f, 0.55f, 10f, new[] { CardTag.Marketing, CardTag.Viral });
            yield return MakeTemp("FF07", "Fake Reviews", VentureType.FastFood, CardFamily.Risk, "review_rush", "Short-term boost, long-term trouble.", 20, 1.3f, 0f, 30f, 0.7f, 2, new[] { "fake_reviews", "review_manipulation" }, null, new[] { CardTag.Illegal, CardTag.Marketing });
            yield return MakeTemp("FF08", "Apology Combo", VentureType.FastFood, CardFamily.Reaction, "review_recovery", "Stabilize reputation after service issues.", 30, 0.4f, 0f, -10f, 0.8f, 1, null, new[] { "review_crisis", "quality_drop" }, new[] { CardTag.Support, CardTag.Marketing });
            yield return MakeCrisis("FF09", "Review Storm", VentureType.FastFood, "Rush order collapse sparks bad reviews.", "review_crisis", 2, -1.3f, -0.8f, 12f, new[] { CardTag.Risky, CardTag.Marketing });
        }

        private static IEnumerable<CardData> CreateCafeCards()
        {
            yield return MakeOperation("CF01", "Espresso Bar", VentureType.Cafe, "bar", "Core coffee setup.", 100, 2f, 2f, 1.4f, 1.4f, 5f, 4f, 4f, new[] { CardTag.Coffee, CardTag.Basic });
            yield return MakeOperation("CF02", "Window Seating", VentureType.Cafe, "seating", "More dwell time and neighborhood presence.", 75, 1f, 1.2f, 0.8f, 0f, 4f, 3.5f, 5f, new[] { CardTag.Coffee, CardTag.Trendy });
            yield return MakeStaff("CF03", "Senior Barista", VentureType.Cafe, "barista", "Drives consistency and visual appeal.", 48, 0.5f, 1.3f, 0.8f, 0.9f, new[] { CardTag.Coffee, CardTag.Support });
            yield return MakeSupplier("CF04", "Specialty Beans", VentureType.Cafe, "beans", "Raises perceived quality immediately.", 55, 0.2f, 1.8f, 0.4f, 0.3f, new[] { "bean_quality" }, new[] { CardTag.Coffee, CardTag.Organic });
            yield return MakeMarketing("CF05", "Instagram Reels", VentureType.Cafe, "instagram", "Visual coffee content brings new eyes.", 30, 1.8f, 0.35f, 12f, new[] { CardTag.Marketing, CardTag.Influencer });
            yield return MakeMarketing("CF06", "Maps Reviews", VentureType.Cafe, "maps", "Improves discovery and trust.", 40, 1.0f, 0.65f, 8f, new[] { CardTag.Marketing, CardTag.Viral });
            yield return MakeTemp("CF07", "Unlicensed Playlist", VentureType.Cafe, CardFamily.Risk, "mesam", "Cheap ambience with a legal edge.", 10, 0.6f, 0f, 24f, 0.2f, 2, new[] { "music_license" }, null, new[] { CardTag.Illegal, CardTag.Risky });
            yield return MakeTemp("CF08", "Customer Care Round", VentureType.Cafe, CardFamily.Reaction, "complaint_round", "Recovers trust after slow or bad service.", 25, 0.5f, 0f, -8f, 0.7f, 1, null, new[] { "review_crisis", "slow_service" }, new[] { CardTag.Support, CardTag.Marketing });
            yield return MakeCrisis("CF09", "Barista Burnout", VentureType.Cafe, "The star barista burns out during a busy week.", "staff_crisis", 2, -1.0f, -1.2f, 6f, new[] { CardTag.Risky, CardTag.Management });
        }

        private static IEnumerable<CardData> CreateTechCards()
        {
            yield return MakeOperation("TC01", "MVP Build", VentureType.TechApp, "product", "Your first working product loop.", 110, 1.3f, 1.4f, 1f, 1f, 4f, 5f, 3f, new[] { CardTag.Tech, CardTag.Startup });
            yield return MakeOperation("TC02", "Backend Upgrade", VentureType.TechApp, "backend", "Stability before explosive growth.", 85, 0.4f, 2.2f, 0.8f, 0.3f, 3f, 5f, 4.5f, new[] { CardTag.Tech, CardTag.Logistics });
            yield return MakeStaff("TC03", "Core Developer", VentureType.TechApp, "developer", "Adds throughput and reliability.", 50, 0.4f, 1.6f, 0.6f, 0.7f, new[] { CardTag.Tech, CardTag.Support });
            yield return MakeSupplier("TC04", "Cloud Credits", VentureType.TechApp, "cloud", "Reduces infra pain while scaling.", 45, 0.2f, 0.8f, 0.5f, 0.2f, new[] { "infra_cost", "stability" }, new[] { CardTag.Tech, CardTag.Support });
            yield return MakeMarketing("TC05", "ASO Push", VentureType.TechApp, "aso", "Better store conversion and discovery.", 35, 1.7f, 0.55f, 9f, new[] { CardTag.Marketing, CardTag.Tech });
            yield return MakeMarketing("TC06", "Paid User Acquisition", VentureType.TechApp, "ads", "Fast installs at real cash cost.", 55, 2.4f, 0.15f, 20f, new[] { CardTag.Marketing, CardTag.Aggressive });
            yield return MakeTemp("TC07", "Dark Pattern Onboarding", VentureType.TechApp, CardFamily.Risk, "dark_pattern", "Boosts short-term installs but hurts trust.", 15, 1.6f, 0f, 26f, -0.4f, 2, new[] { "dark_pattern", "privacy" }, null, new[] { CardTag.Illegal, CardTag.Risky });
            yield return MakeTemp("TC08", "Hotfix Sprint", VentureType.TechApp, CardFamily.Reaction, "hotfix", "Rebuild rating after a crash wave.", 35, 0.2f, 0.8f, -10f, 0.9f, 1, null, new[] { "stability_crisis", "rating_drop" }, new[] { CardTag.Support, CardTag.Tech });
            yield return MakeCrisis("TC09", "Crash Wave", VentureType.TechApp, "A release breaks the core flow and triggers review damage.", "stability_crisis", 2, -1.2f, -0.8f, 8f, new[] { CardTag.Tech, CardTag.Risky });
        }

        private static IEnumerable<CardData> CreateClothingCards()
        {
            yield return MakeOperation("CL01", "Storefront Refresh", VentureType.ClothingStore, "showcase", "First visual hook for walk-ins.", 95, 1.8f, 1.2f, 1.2f, 1.2f, 5f, 5.5f, 3.5f, new[] { CardTag.Luxury, CardTag.Basic });
            yield return MakeOperation("CL02", "Inventory Rail", VentureType.ClothingStore, "inventory", "More product depth, more pressure to sell through.", 80, 0.8f, 2.0f, 0.8f, 0f, 4f, 4f, 3f, new[] { CardTag.Scaling, CardTag.Basic });
            yield return MakeStaff("CL03", "Floor Stylist", VentureType.ClothingStore, "sales", "Helps convert browsers to buyers.", 42, 0.6f, 1.1f, 0.5f, 0.8f, new[] { CardTag.Support, CardTag.Luxury });
            yield return MakeSupplier("CL04", "Reliable Atelier", VentureType.ClothingStore, "atelier", "Better fit, lower return pressure.", 55, 0.3f, 1.5f, 0.3f, 0.2f, new[] { "stock_health", "return_pressure" }, new[] { CardTag.Support, CardTag.Luxury });
            yield return MakeMarketing("CL05", "Instagram Lookbook", VentureType.ClothingStore, "instagram", "Visual demand spike for new drops.", 30, 2.0f, 0.25f, 13f, new[] { CardTag.Marketing, CardTag.Influencer });
            yield return MakeMarketing("CL06", "Flash Discount", VentureType.ClothingStore, "discount", "Moves stock quickly but can cheapen the brand.", 25, 1.8f, -0.15f, 12f, new[] { CardTag.Marketing, CardTag.Pricing });
            yield return MakeTemp("CL07", "Cheap Fabric Batch", VentureType.ClothingStore, CardFamily.Risk, "cheap_fabric", "Better short-term margin, worse long-term returns.", 10, 0.9f, -0.5f, 18f, -0.3f, 2, new[] { "quality_claim", "returns" }, null, new[] { CardTag.Illegal, CardTag.Risky });
            yield return MakeTemp("CL08", "Return Desk Reset", VentureType.ClothingStore, CardFamily.Reaction, "returns_reset", "Protects trust after sizing or quality issues.", 28, 0.4f, 0.4f, -8f, 0.7f, 1, null, new[] { "returns", "review_crisis" }, new[] { CardTag.Support, CardTag.Management });
            yield return MakeCrisis("CL09", "Season Misread", VentureType.ClothingStore, "You bought the wrong season too early.", "inventory_crisis", 2, -0.8f, -0.6f, 4f, new[] { CardTag.Scaling, CardTag.Risky });
        }

        private static IEnumerable<CardData> CreateGroceryCards()
        {
            yield return MakeOperation("GR01", "Fresh Shelf", VentureType.GroceryStore, "fresh", "Fresh products create repeat neighborhood demand.", 85, 2.1f, 1.4f, 1.1f, 1.0f, 4f, 6f, 3f, new[] { CardTag.Basic, CardTag.Organic });
            yield return MakeOperation("GR02", "Checkout Upgrade", VentureType.GroceryStore, "checkout", "Reduces friction during rush hours.", 70, 0.6f, 1.9f, 0.4f, 0f, 3f, 5f, 3.5f, new[] { CardTag.Basic, CardTag.Support });
            yield return MakeStaff("GR03", "Trusted Cashier", VentureType.GroceryStore, "cashier", "Handles volume and keeps regulars happy.", 40, 0.5f, 1.3f, 0.4f, 0.9f, new[] { CardTag.Support, CardTag.Basic });
            yield return MakeSupplier("GR04", "Morning Hal Route", VentureType.GroceryStore, "market", "Improves freshness and lowers spoilage risk.", 50, 0.4f, 1.4f, 0.4f, 0.2f, new[] { "spoilage", "local_loyalty" }, new[] { CardTag.Organic, CardTag.Support });
            yield return MakeMarketing("GR05", "WhatsApp Orders", VentureType.GroceryStore, "whatsapp", "Convenience grows loyalty and repeat demand.", 20, 1.4f, 0.45f, 6f, new[] { CardTag.Marketing, CardTag.Support });
            yield return MakeMarketing("GR06", "Late Night Sign", VentureType.GroceryStore, "latenight", "Wins urgent neighborhood trips.", 18, 1.6f, 0.15f, 8f, new[] { CardTag.Marketing, CardTag.Basic });
            yield return MakeTemp("GR07", "SKT Shuffle", VentureType.GroceryStore, CardFamily.Risk, "skt_shuffle", "Cuts spoilage today, creates huge trust risk tomorrow.", 10, 0.7f, -0.4f, 28f, -0.5f, 2, new[] { "skt", "freshness" }, null, new[] { CardTag.Illegal, CardTag.Risky });
            yield return MakeTemp("GR08", "Neighborhood Apology", VentureType.GroceryStore, CardFamily.Reaction, "local_repair", "Restores trust after freshness issues.", 22, 0.5f, 0.2f, -10f, 0.8f, 1, null, new[] { "freshness", "review_crisis" }, new[] { CardTag.Support, CardTag.Marketing });
            yield return MakeCrisis("GR09", "Spoilage Wave", VentureType.GroceryStore, "Fresh inventory turns faster than expected.", "freshness", 2, -0.9f, -0.7f, 10f, new[] { CardTag.Organic, CardTag.Risky });
        }

        private static IEnumerable<CardData> CreateNeutralCards()
        {
            yield return MakeMarketing("NT01", "Local Buzz", VentureType.Diner, "neutral_marketing", "A flexible marketing bump for any venture.", 25, 1.2f, 0.2f, 8f, new[] { CardTag.Marketing, CardTag.Basic }, true);
            yield return MakeSupplier("NT02", "Cash Buffer", VentureType.Diner, "neutral_supplier", "Small financial breathing room every turn.", 30, 0f, 0.2f, 0.2f, 0f, new[] { "stability" }, new[] { CardTag.Finance, CardTag.Support }, true, 18f, 4f);
            yield return MakeTemp("NT03", "Compliance Sweep", VentureType.Diner, CardFamily.Reaction, "compliance", "Reduces legal risk when things get messy.", 18, 0f, 0f, -18f, 0.2f, 1, null, new[] { "privacy", "review_crisis", "quality_claim", "skt" }, new[] { CardTag.Support, CardTag.Defensive }, true);
            yield return MakeTemp("NT04", "Emergency Hire", VentureType.Diner, CardFamily.Reaction, "emergency_hire", "Stabilizes capacity and staff morale for one turn.", 20, 0.4f, 1.0f, -4f, 0.1f, 1, null, new[] { "staff_crisis", "slow_service", "stability_crisis" }, new[] { CardTag.Hiring, CardTag.Support }, true);
            yield return MakeMarketing("NT05", "Seasonal Push", VentureType.Diner, "neutral_late", "Late-run momentum card for any venture.", 40, 1.8f, 0.35f, 12f, new[] { CardTag.Marketing, CardTag.Scaling }, true);
        }

        private static CardData MakeOperation(string id, string name, VentureType venture, string subSlot, string desc, int cost, float demand, float capacity, float quality, float cash, float price, float speed, float buildingHeight, CardTag[] tags)
        {
            var card = CardHelper.CreateV4Card(id, name, CardType.Business, CardFamily.Setup, venture, SlotType.Operation, subSlot, Rarity.Common, cost, desc, tags);
            card.playCost = 0;
            card.demandDelta = demand;
            card.capacityDelta = capacity;
            card.qualityDelta = quality;
            card.cashDeltaPerTurn = cash;
            card.priceScore = price;
            card.serviceSpeedScore = speed;
            card.qualityScore = quality + 4f;
            card.buildingScale = new Vector3(0.9f, buildingHeight, 0.9f);
            card.buildingColor = GetVentureColor(venture, 0.95f);
            card.buildingLabel = name;
            return card;
        }

        private static CardData MakeStaff(string id, string name, VentureType venture, string subSlot, string desc, int upkeep, float demand, float capacity, float quality, float stability, CardTag[] tags)
        {
            var card = CardHelper.CreateV4Card(id, name, CardType.Employee, CardFamily.Setup, venture, SlotType.Staff, subSlot, Rarity.Common, 0, desc, tags);
            card.buyCost = upkeep * 2;
            card.playCost = 0;
            card.salaryPerTurn = upkeep;
            card.demandDelta = demand;
            card.capacityDelta = capacity;
            card.qualityDelta = quality;
            card.staffStabilityDelta = stability;
            card.upkeepCostPerTurn = upkeep;
            card.buildingScale = new Vector3(0.6f, 1.1f, 0.6f);
            card.buildingColor = GetVentureColor(venture, 1.10f);
            card.buildingLabel = name;
            return card;
        }

        private static CardData MakeSupplier(string id, string name, VentureType venture, string subSlot, string desc, int upkeep, float demand, float quality, float cash, float rating, string[] solutionTags, CardTag[] tags, bool isGeneral = false, float cashDelta = 0f, float capacity = 0.3f)
        {
            var card = CardHelper.CreateV4Card(id, name, CardType.Upgrade, CardFamily.Setup, venture, SlotType.Supplier, subSlot, Rarity.Uncommon, upkeep, desc, tags);
            card.isGeneralCard = isGeneral;
            card.playCost = 0;
            card.upkeepCostPerTurn = upkeep;
            card.demandDelta = demand;
            card.capacityDelta = capacity;
            card.qualityDelta = quality;
            card.cashDeltaPerTurn = cashDelta != 0f ? cashDelta : cash;
            card.ratingDeltaPerTurn = rating;
            card.solutionTags = solutionTags ?? new string[0];
            card.qualityBoostAmount = quality;
            card.costReductionPercent = cash > 0f ? 0.08f : 0f;
            card.buildingScale = new Vector3(0.8f, 0.8f, 0.8f);
            card.buildingColor = GetVentureColor(venture, 0.85f);
            card.buildingLabel = name;
            return card;
        }

        private static CardData MakeMarketing(string id, string name, VentureType venture, string subSlot, string desc, int upkeep, float demand, float rating, float cashCost, CardTag[] tags, bool isGeneral = false)
        {
            var card = CardHelper.CreateV4Card(id, name, CardType.Upgrade, CardFamily.Growth, venture, SlotType.Marketing, subSlot, Rarity.Common, upkeep, desc, tags);
            card.isGeneralCard = isGeneral;
            card.playCost = Mathf.Max(4, Mathf.RoundToInt(cashCost * 0.45f));
            card.demandDelta = demand;
            card.ratingDeltaPerTurn = rating;
            card.upkeepCostPerTurn = cashCost;
            card.platformRatingGain = rating;
            card.cashDeltaPerTurn = 0f;
            card.preferredPressures = new[] { BoardPressureType.LowDemand };
            card.buildingScale = new Vector3(0.7f, 0.6f, 0.7f);
            card.buildingColor = GetVentureColor(venture, 1.20f);
            card.buildingLabel = name;
            return card;
        }

        private static CardData MakeTemp(string id, string name, VentureType venture, CardFamily family, string subSlot, string desc, int cost, float demand, float capacity, float legalRisk, float rating, int duration, string[] crisisTags, string[] solutionTags, CardTag[] tags, bool isGeneral = false)
        {
            var card = CardHelper.CreateV4Card(id, name, CardType.Upgrade, family, venture, SlotType.TempEffect, subSlot, Rarity.Uncommon, cost, desc, tags);
            card.isGeneralCard = isGeneral;
            card.playCost = Mathf.Max(6, Mathf.RoundToInt(cost * 0.5f));
            card.demandDelta = demand;
            card.capacityDelta = capacity;
            card.legalRiskDeltaPerTurn = legalRisk;
            card.ratingDeltaPerTurn = rating;
            card.tempEffectDuration = duration;
            card.crisisTags = crisisTags ?? new string[0];
            card.solutionTags = solutionTags ?? new string[0];
            card.entersTempEffectOnUse = true;
            card.preferredPressures = family == CardFamily.Reaction
                ? new[] { BoardPressureType.LowRating, BoardPressureType.HighLegalRisk, BoardPressureType.StaffInstability }
                : new[] { BoardPressureType.LowCash };
            card.buildingScale = new Vector3(0.75f, 0.65f, 0.75f);
            card.buildingColor = family == CardFamily.Risk ? new Color(0.65f, 0.18f, 0.18f) : new Color(0.18f, 0.45f, 0.62f);
            card.buildingLabel = name;
            return card;
        }

        private static CardData MakeCrisis(string id, string name, VentureType venture, string desc, string crisisTag, int duration, float demandPenalty, float qualityPenalty, float legalRisk, CardTag[] tags)
        {
            var card = CardHelper.CreateV4Card(id, name, CardType.Event, CardFamily.Crisis, venture, SlotType.TempEffect, crisisTag, Rarity.Uncommon, 0, desc, tags);
            card.playCost = 0;
            card.tempEffectDuration = duration;
            card.eventDuration = duration;
            card.demandDelta = demandPenalty;
            card.qualityDelta = qualityPenalty;
            card.legalRiskDeltaPerTurn = legalRisk;
            card.crisisTags = new[] { crisisTag };
            card.buildingScale = new Vector3(0.85f, 0.7f, 0.85f);
            card.buildingColor = new Color(0.48f, 0.10f, 0.10f);
            card.buildingLabel = name;
            return card;
        }

        private static Color GetVentureColor(VentureType venture, float multiplier)
        {
            Color baseColor = venture switch
            {
                VentureType.FastFood => new Color(0.77f, 0.29f, 0.17f),
                VentureType.Cafe => new Color(0.50f, 0.36f, 0.22f),
                VentureType.TechApp => new Color(0.23f, 0.50f, 0.83f),
                VentureType.ClothingStore => new Color(0.66f, 0.21f, 0.46f),
                VentureType.GroceryStore => new Color(0.23f, 0.60f, 0.28f),
                _ => new Color(0.45f, 0.45f, 0.45f)
            };

            return new Color(
                Mathf.Clamp01(baseColor.r * multiplier),
                Mathf.Clamp01(baseColor.g * multiplier),
                Mathf.Clamp01(baseColor.b * multiplier));
        }
    }
}
