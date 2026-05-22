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
                    new[] { "FF01", "FF02", "FF03", "FF04", "FF05", "FF06", "FF07", "FF08", "FF10", "FF11", "NT01", "NT02" },
                    new[] { "FF02", "FF04", "FF05", "FF06", "FF07", "FF08", "FF10", "FF11", "FF14", "NT02", "NT03" },
                    new[] { "FF04", "FF06", "FF07", "FF08", "FF10", "FF11", "FF12", "FF13", "FF14", "NT03" },
                    new[] { "NT01", "NT02", "NT03", "NT04", "NT05" },
                    new[] { "FF09", "FF12", "FF13" }),
                CreateDeckProfile(VentureType.Cafe,
                    new[] { "CF02", "CF03", "CF04", "CF05", "CF06", "NT01" },
                    new[] { "CF01", "CF02", "CF03", "CF04", "CF05", "CF06", "CF07", "CF08", "CF10", "CF11", "NT01", "NT04" },
                    new[] { "CF02", "CF04", "CF05", "CF06", "CF07", "CF08", "CF10", "CF11", "CF12", "CF15", "NT02", "NT04" },
                    new[] { "CF04", "CF06", "CF07", "CF08", "CF10", "CF11", "CF12", "CF13", "CF14", "CF15", "NT03" },
                    new[] { "NT01", "NT02", "NT03", "NT04", "NT05" },
                    new[] { "CF09", "CF13", "CF14" }),
                CreateDeckProfile(VentureType.TechApp,
                    new[] { "TC02", "TC03", "TC04", "TC05", "TC06", "NT02" },
                    new[] { "TC01", "TC02", "TC03", "TC04", "TC05", "TC06", "TC07", "TC08", "NT02", "NT04" },
                    new[] { "TC02", "TC04", "TC05", "TC06", "TC07", "TC08", "NT02", "NT03" },
                    new[] { "TC04", "TC06", "TC07", "TC08", "TC09", "NT05" },
                    new[] { "NT01", "NT02", "NT03", "NT04", "NT05" },
                    new[] { "TC09" }),
                CreateDeckProfile(VentureType.ClothingStore,
                    new[] { "CL02", "CL03", "CL04", "CL05", "CL06", "NT01" },
                    new[] { "CL01", "CL02", "CL03", "CL04", "CL05", "CL06", "CL07", "CL08", "CL10", "CL11", "NT01", "NT04" },
                    new[] { "CL02", "CL04", "CL05", "CL06", "CL07", "CL08", "CL10", "CL11", "CL12", "CL15", "NT03", "NT04" },
                    new[] { "CL04", "CL06", "CL07", "CL08", "CL09", "CL10", "CL11", "CL12", "CL13", "CL14", "CL15", "NT05" },
                    new[] { "NT01", "NT02", "NT03", "NT04", "NT05" },
                    new[] { "CL09", "CL13", "CL14" }),
                CreateDeckProfile(VentureType.GroceryStore,
                    new[] { "GR02", "GR03", "GR04", "GR05", "GR06", "NT01" },
                    new[] { "GR01", "GR02", "GR03", "GR04", "GR05", "GR06", "GR07", "GR08", "GR10", "GR11", "NT01", "NT04" },
                    new[] { "GR02", "GR04", "GR05", "GR06", "GR07", "GR08", "GR10", "GR11", "GR12", "GR15", "NT02", "NT03" },
                    new[] { "GR04", "GR06", "GR07", "GR08", "GR09", "GR10", "GR11", "GR12", "GR13", "GR14", "GR15", "NT05" },
                    new[] { "NT01", "NT02", "NT03", "NT04", "NT05" },
                    new[] { "GR09", "GR13", "GR14" })
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

            var themeProfiles = CreateThemeProfiles();
            var progressionArcs = CreateProgressionArcs();
            var eventChains = CreateEventChains();
            var playbooks = CreatePlaybooks(themeProfiles, progressionArcs, eventChains);

            return new[]
            {
                CreateVenture(
                    VentureType.FastFood,
                    "Fast Food",
                    "High traffic, high pressure. Win by balancing growth and kitchen capacity.",
                    "Fast local scaling with review risk.",
                    "Open the counter, add seating, lock kitchen flow, then push local buzz without breaking the line.",
                    "Turn 1: expand service flow.\nTurn 2: add kitchen or hygiene support.\nTurn 3: push flyers or Google once the rush is absorbable.",
                    new[] { "FF02", "FF03", "FF04", "FF06", "FF05", "FF10", "FF11", "FF14" },
                    playbooks[0],
                    progressionArcs[0],
                    themeProfiles[0],
                    byId["FF01"], boardProfiles[0], deckProfiles[0], economyProfiles[0]),
                CreateVenture(
                    VentureType.Cafe,
                    "Cafe",
                    "Loyalty and quality driven. Build social proof and neighborhood habits.",
                    "Premium quality and sticky regulars.",
                    "Open the room, seat guests, hire the barista, then lock beans and reviews before chasing social demand.",
                    "Turn 1: add seating and bar rhythm.\nTurn 2: stabilize beans, milk, and floor flow.\nTurn 3: push Maps, reels, and regular loyalty once service feels real.",
                    new[] { "CF02", "CF03", "CF04", "CF06", "CF05", "CF10", "CF11", "CF12" },
                    playbooks[1],
                    progressionArcs[1],
                    themeProfiles[1],
                    byId["CF01"], boardProfiles[1], deckProfiles[1], economyProfiles[1]),
                CreateVenture(
                    VentureType.TechApp,
                    "Tech App",
                    "Slow start, explosive upside. Stability and rating matter before scale.",
                    "Product, growth, and retention balancing act.",
                    "Ship the first working product, harden backend, and only then spend to acquire users at scale.",
                    "Turn 1: stabilize the core product.\nTurn 2: reinforce backend and cloud reliability.\nTurn 3: turn on acquisition once reviews and uptime are safe.",
                    new[] { "TC02", "TC03", "TC04", "TC05", "TC06", "TC11", "TC13", "TC14" },
                    playbooks[2],
                    progressionArcs[2],
                    themeProfiles[2],
                    byId["TC01"], boardProfiles[2], deckProfiles[2], economyProfiles[2]),
                CreateVenture(
                    VentureType.ClothingStore,
                    "Clothing Store",
                    "Seasonal demand, inventory pressure, visual merchandising wars.",
                    "Trend and stock management duel.",
                    "Dress the storefront, widen inventory, add fit support, then market the look once returns are under control.",
                    "Turn 1: improve display and floor conversion.\nTurn 2: secure fit and fabric quality.\nTurn 3: advertise the collection only after the return risk is covered.",
                    new[] { "CL02", "CL03", "CL04", "CL05", "CL06", "CL10", "CL11", "CL12" },
                    playbooks[3],
                    progressionArcs[3],
                    themeProfiles[3],
                    byId["CL01"], boardProfiles[3], deckProfiles[3], economyProfiles[3]),
                CreateVenture(
                    VentureType.GroceryStore,
                    "Grocery Store",
                    "Low margin, repeat traffic, spoilage and neighborhood loyalty.",
                    "Stable demand with tight operational margins.",
                    "Open the shelf, smooth checkout, secure morning supply, then lean into convenience and repeat orders.",
                    "Turn 1: remove checkout friction.\nTurn 2: protect freshness and spoilage.\nTurn 3: add WhatsApp or late-night convenience once basics are stable.",
                    new[] { "GR02", "GR03", "GR04", "GR05", "GR06", "GR10", "GR11", "GR15" },
                    playbooks[4],
                    progressionArcs[4],
                    themeProfiles[4],
                    byId["GR01"], boardProfiles[4], deckProfiles[4], economyProfiles[4])
            };
        }

        private static VentureData CreateVenture(
            VentureType type,
            string name,
            string description,
            string playstyle,
            string openingFantasy,
            string openingPlanSummary,
            string[] openingSequenceCardIds,
            VenturePlaybook playbook,
            VentureProgressionArc progressionArc,
            VentureBoardThemeProfile themeProfile,
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
            venture.openingFantasy = openingFantasy;
            venture.openingPlanSummary = openingPlanSummary;
            venture.openingSequenceCardIds = openingSequenceCardIds;
            venture.requiresCustomName = true;
            venture.requiresRunCategorySelection = type == VentureType.TechApp;
            venture.startingBusiness = startingCard;
            venture.boardProfile = boardProfile;
            venture.deckProfile = deckProfile;
            venture.economyProfile = economyProfile;
            venture.playbook = playbook;
            venture.progressionArc = progressionArc;
            venture.themeProfile = themeProfile;
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

        private static VenturePlaybook[] CreatePlaybooks(
            VentureBoardThemeProfile[] themeProfiles,
            VentureProgressionArc[] progressionArcs,
            VentureEventChain[] eventChains)
        {
            return new[]
            {
                CreatePlaybook(VentureType.FastFood, "Rush, seating, kitchen rhythm, and review protection decide whether traffic turns into profit.", "Queue pressure, hygiene, local buzz, and margin discipline carry the lane.", new[] { "Queue Pressure", "Kitchen Throughput", "Review Heat", "Staff Fatigue" }, new[] { "FF02", "FF03", "FF04", "FF06", "FF05", "FF10", "FF11", "FF14" }, progressionArcs[0], themeProfiles[0], eventChains[0]),
                CreatePlaybook(VentureType.Cafe, "The room, the bar, the beans, and the regular loop must click before social demand matters.", "Bar flow, drink consistency, ambience, and neighborhood trust drive the run.", new[] { "Bar Throughput", "Bean Quality", "Ambience", "Regular Loyalty" }, new[] { "CF02", "CF03", "CF04", "CF06", "CF05", "CF10", "CF11", "CF12" }, progressionArcs[1], themeProfiles[1], eventChains[1]),
                CreatePlaybook(VentureType.TechApp, "Product reliability, growth efficiency, and trust compound only if backend survives scale.", "CAC, retention, stability, infra burn, and reviews define the app war.", new[] { "CAC", "Retention", "Stability", "Infra Burn" }, new[] { "TC02", "TC03", "TC04", "TC05", "TC06", "TC11", "TC13", "TC14" }, progressionArcs[2], themeProfiles[2], eventChains[2]),
                CreatePlaybook(VentureType.ClothingStore, "Display, inventory confidence, fit trust, and trend timing all need to land together.", "Footfall, conversion, return pressure, and fabric trust decide the season.", new[] { "Footfall", "Conversion", "Fit Trust", "Return Pressure" }, new[] { "CL02", "CL03", "CL04", "CL05", "CL06", "CL10", "CL11", "CL12" }, progressionArcs[3], themeProfiles[3], eventChains[3]),
                CreatePlaybook(VentureType.GroceryStore, "Freshness, checkout flow, and neighborhood convenience must feel dependable before you widen demand.", "Spoilage, basket size, shelf fill, and local trust shape the store.", new[] { "Freshness", "Checkout Delay", "Shelf Fill", "Neighborhood Loyalty" }, new[] { "GR02", "GR03", "GR04", "GR05", "GR06", "GR10", "GR11", "GR15" }, progressionArcs[4], themeProfiles[4], eventChains[4])
            };
        }

        private static VenturePlaybook CreatePlaybook(
            VentureType type,
            string fantasy,
            string economy,
            string[] metrics,
            string[] openingSequence,
            VentureProgressionArc arc,
            VentureBoardThemeProfile theme,
            VentureEventChain eventChain)
        {
            var playbook = ScriptableObject.CreateInstance<VenturePlaybook>();
            playbook.ventureType = type;
            playbook.fantasySummary = fantasy;
            playbook.economySummary = economy;
            playbook.primaryMetrics = metrics;
            playbook.openingSequenceCardIds = openingSequence;
            playbook.progressionArc = arc;
            playbook.themeProfile = theme;
            playbook.eventChain = eventChain;
            playbook.name = $"Playbook_{type}";
            return playbook;
        }

        private static VentureProgressionArc[] CreateProgressionArcs()
        {
            return new[]
            {
                CreateProgressionArc(VentureType.FastFood,
                    CreateOpeningBeats(
                        ("ff_open_counter", 1, "Open the floor before you chase volume.", "The counter is live, but the rush still needs seating and visible service flow around the grill.", "Play Extra Tables first. Add kitchen support before flyer demand.", new[] { "FF02", "FF03", "FF10" }, new[] { "ff_tables" }),
                        ("ff_staff_line", 2, "Put people on the line.", "A real rush needs labor behind the counter, not only a sign and a grill.", "Hire Line Cook or Front Counter Server now.", new[] { "FF03", "FF10", "FF11" }, new[] { "ff_line" }),
                        ("ff_quality_lock", 3, "Protect food quality before the reviews hit.", "Ingredient trust and hygiene are the first real multiplier in this lane.", "Take Premium Butcher or Night Cleaner before stacking growth.", new[] { "FF04", "FF11", "FF14" }, new[] { "ff_quality" }),
                        ("ff_local_buzz", 5, "Now the neighborhood should notice you.", "The board can start absorbing traffic once the line stops wobbling.", "Use Google Business or Flyer Team only if capacity still holds.", new[] { "FF05", "FF06", "NT01" }, new[] { "ff_buzz" })),
                    new[] { "FF02", "FF03", "FF10", "FF11", "FF04" },
                    new[] { "FF04", "FF11", "FF06", "NT02", "FF14" },
                    new[] { "FF05", "FF06", "FF10", "NT01", "NT05" },
                    new[] { "FF09", "FF12", "FF13", "NT03" },
                    new[] { "FF08", "FF14", "NT03", "NT04" },
                    new[] { "FF06", "FF10", "FF11", "NT05" }),
                CreateProgressionArc(VentureType.Cafe,
                    CreateOpeningBeats(
                        ("cf_room_open", 1, "Make the cafe feel open, not just named.", "The espresso bar exists, but guests still need a room, a seat, and visible flow around the counter.", "Play Window Seating first. Then line up Senior Barista.", new[] { "CF02", "CF03", "CF10" }, new[] { "cf_tables" }),
                        ("cf_shift_real", 2, "The room needs a real shift behind it.", "A cafe does not feel alive until the bar and floor have an actual operator holding consistency.", "Hire Senior Barista now. Floor Runner is the next stabilizer.", new[] { "CF03", "CF10", "NT04" }, new[] { "cf_barista" }),
                        ("cf_taste_lock", 3, "Lock the taste before you chase the crowd.", "Beans, milk, and drink consistency are what turn one-time curiosity into loyalty.", "Play Specialty Beans or Milk Contract before Instagram.", new[] { "CF04", "CF11", "CF08" }, new[] { "cf_beans" }),
                        ("cf_regular_loop", 5, "Now you can build the regular loop.", "Service is credible enough to convert trust into discovery, reels, and repeat mornings.", "Use Maps Reviews first, then Reels or Stamp Card.", new[] { "CF06", "CF05", "CF12" }, new[] { "cf_regulars" })),
                    new[] { "CF02", "CF03", "CF10", "CF04", "CF11" },
                    new[] { "CF04", "CF11", "CF06", "CF08", "NT02" },
                    new[] { "CF05", "CF06", "CF12", "NT01", "NT05" },
                    new[] { "CF09", "CF13", "CF14", "NT03" },
                    new[] { "CF08", "CF15", "NT03", "NT04" },
                    new[] { "CF12", "CF05", "CF06", "NT05" }),
                CreateProgressionArc(VentureType.TechApp,
                    CreateOpeningBeats(
                        ("tc_core_stable", 1, "Ship the core before you buy growth.", "The product is live, but the stack still needs more reliability before users arrive at scale.", "Play Backend Upgrade or a second product card first.", new[] { "TC02", "TC03", "TC11" }, new[] { "tc_stack" }),
                        ("tc_build_team", 2, "Put real builders behind the MVP.", "The app still feels fragile until a developer or support layer turns the launch into a system.", "Hire Core Developer first. Support follows once the loop is real.", new[] { "TC03", "TC14", "NT04" }, new[] { "tc_team" }),
                        ("tc_infra_cost", 3, "Stability economics come before growth economics.", "Cloud, analytics, and payment infrastructure decide whether growth becomes retention or chaos.", "Take Cloud Credits or Export Pipeline before the biggest acquisition cards.", new[] { "TC04", "TC11", "TC08" }, new[] { "tc_infra" }),
                        ("tc_growth_switch", 5, "Now turn reliability into user growth.", "You finally have enough product trust to scale discovery without instantly burning reviews.", "Lead with ASO Push. Add paid acquisition only if rating still looks safe.", new[] { "TC05", "TC06", "TC13" }, new[] { "tc_growth" })),
                    new[] { "TC02", "TC03", "TC04", "TC11", "TC14" },
                    new[] { "TC04", "TC11", "TC08", "NT02", "NT04" },
                    new[] { "TC05", "TC06", "TC13", "TC16", "TC22" },
                    new[] { "TC09", "TC12", "TC18", "TC21", "TC24", "TC27" },
                    new[] { "TC08", "TC17", "NT03", "NT04" },
                    new[] { "TC16", "TC20", "TC22", "NT05" }),
                CreateProgressionArc(VentureType.ClothingStore,
                    CreateOpeningBeats(
                        ("cl_floor_ready", 1, "Dress the floor before you advertise the brand.", "The storefront exists, but customers still need depth, fit confidence, and visible merchandise logic.", "Play Inventory Rail first. Then add fit support.", new[] { "CL02", "CL03", "CL10" }, new[] { "cl_display" }),
                        ("cl_style_staff", 2, "Style needs staff, not only display.", "A clothing board feels empty until a stylist or tailor can convert browsing into trust.", "Hire Floor Stylist first. Tailor is the next stabilizer.", new[] { "CL03", "CL10", "NT04" }, new[] { "cl_team" }),
                        ("cl_fit_lock", 3, "Protect fit and fabric before trend traffic.", "Returns and weak materials are what make a promising fashion run collapse early.", "Play Reliable Atelier or Premium Fabric Mill before aggressive demand.", new[] { "CL04", "CL11", "CL15" }, new[] { "cl_fit" }),
                        ("cl_story_push", 5, "Now the collection is ready to be seen.", "The board has enough fit confidence to turn display into conversion and trend pull.", "Use Lookbook or Window Story Display once return pressure feels covered.", new[] { "CL05", "CL12", "CL06" }, new[] { "cl_story" })),
                    new[] { "CL02", "CL03", "CL10", "CL04", "CL11" },
                    new[] { "CL04", "CL11", "CL08", "CL15", "NT02" },
                    new[] { "CL05", "CL12", "CL06", "NT01", "NT05" },
                    new[] { "CL09", "CL13", "CL14", "NT03" },
                    new[] { "CL08", "CL15", "NT03", "NT04" },
                    new[] { "CL12", "CL11", "CL05", "NT05" }),
                CreateProgressionArc(VentureType.GroceryStore,
                    CreateOpeningBeats(
                        ("gr_checkout_first", 1, "Make the store function before you extend convenience.", "Fresh shelves are live, but the basket still needs smoother checkout and a clearer trip flow.", "Play Checkout Upgrade first. Then add staff before widening demand.", new[] { "GR02", "GR03", "GR10" }, new[] { "gr_checkout" }),
                        ("gr_staff_trust", 2, "Neighborhood trust starts with the people inside.", "A grocery board does not feel dependable until the register and shelf rhythm have actual support.", "Hire Trusted Cashier first. Stocker follows if lines stay messy.", new[] { "GR03", "GR10", "NT04" }, new[] { "gr_staff" }),
                        ("gr_fresh_lock", 3, "Protect freshness before you widen reach.", "Morning supply quality and spoilage discipline are what make repeat traffic stick in grocery.", "Take Morning Hal Route or Cold Chain Dairy before new convenience demand.", new[] { "GR04", "GR11", "GR15" }, new[] { "gr_fresh" }),
                        ("gr_convenience", 5, "Now convenience can become loyalty.", "The store is credible enough to convert reliability into repeat neighborhood demand.", "Use WhatsApp Orders or Late Night Sign once checkout still feels under control.", new[] { "GR05", "GR06", "GR12" }, new[] { "gr_loyalty" })),
                    new[] { "GR02", "GR03", "GR10", "GR04", "GR11" },
                    new[] { "GR04", "GR11", "GR15", "NT02", "NT04" },
                    new[] { "GR05", "GR06", "GR12", "NT01", "NT05" },
                    new[] { "GR09", "GR13", "GR14", "NT03" },
                    new[] { "GR08", "GR15", "NT03", "NT04" },
                    new[] { "GR12", "GR06", "GR05", "NT05" })
            };
        }

        private static VentureProgressionArc CreateProgressionArc(
            VentureType type,
            TurnScriptBeat[] beats,
            string[] openingPool,
            string[] stabilizePool,
            string[] scalePool,
            string[] crisisPool,
            string[] recoverPool,
            string[] latePool)
        {
            var arc = ScriptableObject.CreateInstance<VentureProgressionArc>();
            arc.ventureType = type;
            arc.openingBeats = beats;
            arc.openingPoolCardIds = openingPool;
            arc.stabilizePoolCardIds = stabilizePool;
            arc.scalePoolCardIds = scalePool;
            arc.crisisPoolCardIds = crisisPool;
            arc.recoverPoolCardIds = recoverPool;
            arc.latePoolCardIds = latePool;
            arc.name = $"ProgressionArc_{type}";
            return arc;
        }

        private static TurnScriptBeat[] CreateOpeningBeats(params (string id, int turn, string headline, string detail, string move, string[] cards, string[] props)[] raw)
        {
            var beats = new TurnScriptBeat[raw.Length];
            for (int i = 0; i < raw.Length; i++)
            {
                beats[i] = new TurnScriptBeat
                {
                    beatId = raw[i].id,
                    turnNumber = raw[i].turn,
                    headline = raw[i].headline,
                    detail = raw[i].detail,
                    recommendedMove = raw[i].move,
                    priorityCardIds = raw[i].cards,
                    highlightPropIds = raw[i].props
                };
            }

            return beats;
        }

        private static VentureBoardThemeProfile[] CreateThemeProfiles()
        {
            return new[]
            {
                CreateThemeProfile(VentureType.FastFood, "RUSH WINDOW", "REVIEW LOOP", "LOCAL PULL", CreateCameraProfile(-0.02f), new[]
                {
                    MakeProp("ff_tables", "FF02", SlotType.Operation, new Vector3(-1.90f, 0.26f, -1.20f), new Vector3(0.72f, 0.18f, 0.48f), new Color(0.33f, 0.20f, 0.14f), new Color(0.86f, 0.44f, 0.16f), "TABLES"),
                    MakeProp("ff_line", "FF03", SlotType.Staff, new Vector3(-0.40f, 0.26f, -0.10f), new Vector3(0.50f, 0.34f, 0.42f), new Color(0.22f, 0.22f, 0.22f), new Color(0.95f, 0.70f, 0.34f), "LINE"),
                    MakeProp("ff_quality", "FF04", SlotType.Supplier, new Vector3(2.35f, 0.26f, 1.05f), new Vector3(0.58f, 0.30f, 0.48f), new Color(0.25f, 0.16f, 0.15f), new Color(0.74f, 0.38f, 0.20f), "QUALITY"),
                    MakeProp("ff_buzz", "FF06", SlotType.Marketing, new Vector3(-4.25f, 0.26f, 1.10f), new Vector3(0.48f, 0.42f, 0.24f), new Color(0.15f, 0.20f, 0.22f), new Color(0.25f, 0.62f, 0.92f), "BUZZ")
                }),
                CreateThemeProfile(VentureType.Cafe, "MORNING RUSH", "REGULAR LOOP", "NEIGHBORHOOD PULL", CreateCameraProfile(0.00f), new[]
                {
                    MakeProp("cf_tables", "CF02", SlotType.Operation, new Vector3(-1.90f, 0.26f, -1.20f), new Vector3(0.70f, 0.16f, 0.52f), new Color(0.28f, 0.18f, 0.12f), new Color(0.73f, 0.52f, 0.28f), "TABLES"),
                    MakeProp("cf_barista", "CF03", SlotType.Staff, new Vector3(-0.48f, 0.28f, -0.08f), new Vector3(0.44f, 0.38f, 0.40f), new Color(0.18f, 0.18f, 0.18f), new Color(0.92f, 0.79f, 0.54f), "BAR"),
                    MakeProp("cf_beans", "CF04", SlotType.Supplier, new Vector3(2.35f, 0.26f, 1.08f), new Vector3(0.54f, 0.34f, 0.48f), new Color(0.22f, 0.15f, 0.10f), new Color(0.64f, 0.40f, 0.18f), "BEANS"),
                    MakeProp("cf_regulars", "CF12", SlotType.Marketing, new Vector3(-4.18f, 0.28f, 1.14f), new Vector3(0.46f, 0.44f, 0.24f), new Color(0.14f, 0.18f, 0.22f), new Color(0.34f, 0.74f, 0.48f), "REGULARS")
                }),
                CreateThemeProfile(VentureType.TechApp, "USER FLOW", "TRUST LOOP", "CHANNEL PULL", CreateCameraProfile(0.08f), new[]
                {
                    MakeProp("tc_stack", "TC02", SlotType.Operation, new Vector3(-1.70f, 0.28f, -1.18f), new Vector3(0.60f, 0.44f, 0.32f), new Color(0.14f, 0.20f, 0.28f), new Color(0.24f, 0.58f, 0.94f), "STACK"),
                    MakeProp("tc_team", "TC03", SlotType.Staff, new Vector3(-0.10f, 0.28f, -0.10f), new Vector3(0.44f, 0.34f, 0.34f), new Color(0.18f, 0.18f, 0.20f), new Color(0.72f, 0.88f, 1.00f), "TEAM"),
                    MakeProp("tc_infra", "TC04", SlotType.Supplier, new Vector3(2.30f, 0.28f, 1.08f), new Vector3(0.58f, 0.44f, 0.30f), new Color(0.12f, 0.18f, 0.26f), new Color(0.32f, 0.70f, 1.00f), "INFRA"),
                    MakeProp("tc_growth", "TC05", SlotType.Marketing, new Vector3(-4.18f, 0.28f, 1.10f), new Vector3(0.44f, 0.52f, 0.24f), new Color(0.10f, 0.16f, 0.22f), new Color(0.22f, 0.90f, 0.88f), "GROWTH")
                }),
                CreateThemeProfile(VentureType.ClothingStore, "FOOTFALL", "FIT TRUST", "TREND PULL", CreateCameraProfile(0.03f), new[]
                {
                    MakeProp("cl_display", "CL02", SlotType.Operation, new Vector3(-1.95f, 0.26f, -1.20f), new Vector3(0.76f, 0.34f, 0.26f), new Color(0.24f, 0.15f, 0.20f), new Color(0.88f, 0.38f, 0.60f), "DISPLAY"),
                    MakeProp("cl_team", "CL03", SlotType.Staff, new Vector3(-0.20f, 0.28f, -0.10f), new Vector3(0.44f, 0.38f, 0.34f), new Color(0.20f, 0.16f, 0.18f), new Color(1.00f, 0.76f, 0.84f), "STYLE"),
                    MakeProp("cl_fit", "CL04", SlotType.Supplier, new Vector3(2.30f, 0.28f, 1.08f), new Vector3(0.54f, 0.40f, 0.30f), new Color(0.22f, 0.14f, 0.18f), new Color(0.76f, 0.34f, 0.50f), "FIT"),
                    MakeProp("cl_story", "CL12", SlotType.Marketing, new Vector3(-4.18f, 0.28f, 1.10f), new Vector3(0.46f, 0.46f, 0.22f), new Color(0.18f, 0.14f, 0.20f), new Color(0.94f, 0.50f, 0.74f), "LOOK")
                }),
                CreateThemeProfile(VentureType.GroceryStore, "BASKET FLOW", "FRESH TRUST", "LOCAL PULL", CreateCameraProfile(-0.03f), new[]
                {
                    MakeProp("gr_checkout", "GR02", SlotType.Operation, new Vector3(-1.90f, 0.26f, -1.22f), new Vector3(0.80f, 0.18f, 0.26f), new Color(0.16f, 0.20f, 0.14f), new Color(0.42f, 0.80f, 0.34f), "CHECKOUT"),
                    MakeProp("gr_staff", "GR03", SlotType.Staff, new Vector3(-0.16f, 0.28f, -0.10f), new Vector3(0.42f, 0.38f, 0.32f), new Color(0.18f, 0.20f, 0.16f), new Color(0.86f, 0.95f, 0.70f), "STAFF"),
                    MakeProp("gr_fresh", "GR04", SlotType.Supplier, new Vector3(2.30f, 0.28f, 1.08f), new Vector3(0.62f, 0.30f, 0.44f), new Color(0.16f, 0.22f, 0.16f), new Color(0.34f, 0.72f, 0.30f), "FRESH"),
                    MakeProp("gr_loyalty", "GR05", SlotType.Marketing, new Vector3(-4.20f, 0.28f, 1.10f), new Vector3(0.46f, 0.42f, 0.22f), new Color(0.14f, 0.18f, 0.12f), new Color(0.44f, 0.84f, 0.42f), "LOCAL")
                })
            };
        }

        private static VentureBoardThemeProfile CreateThemeProfile(
            VentureType type,
            string marketLabel,
            string trustLabel,
            string pullLabel,
            BoardCameraProfile cameraProfile,
            VentureBoardProp[] props)
        {
            var theme = ScriptableObject.CreateInstance<VentureBoardThemeProfile>();
            theme.ventureType = type;
            theme.marketFocusLabel = marketLabel;
            theme.trustFocusLabel = trustLabel;
            theme.pullFocusLabel = pullLabel;
            theme.rivalPressureLabel = "RIVAL PRESSURE";
            theme.cameraProfile = cameraProfile;
            theme.props = props;
            theme.name = $"Theme_{type}";
            return theme;
        }

        private static BoardCameraProfile CreateCameraProfile(float xOffset)
        {
            return new BoardCameraProfile
            {
                cameraPosition = new Vector3(xOffset, 16.6f, -4.6f),
                cameraEuler = new Vector3(68.5f, 0f, 0f),
                fieldOfView = 40f,
                handAnchorPosition = new Vector3(0f, 0.84f, -3.06f),
                handAnchorEuler = new Vector3(-2f, 0f, 0f),
                handSpacing = 1.08f,
                handFanAngle = 5.5f,
                handVerticalArc = 0.08f
            };
        }

        private static VentureBoardProp MakeProp(string propId, string triggerCardId, SlotType slotType, Vector3 pos, Vector3 scale, Color idle, Color active, string label)
        {
            return new VentureBoardProp
            {
                propId = propId,
                triggerCardId = triggerCardId,
                slotType = slotType,
                localPosition = pos,
                localScale = scale,
                idleColor = idle,
                activeColor = active,
                label = label
            };
        }

        private static VentureEventChain[] CreateEventChains()
        {
            return new[]
            {
                CreateEventChain(VentureType.FastFood, new[]
                {
                    MakeEventWindow("ff_opening_friction", 3, 5, BoardPressureType.CapacityShortfall, "Opening friction", "Rush traffic is exposing kitchen and queue limits.", "FF09", "FF12"),
                    MakeEventWindow("ff_margin_squeeze", 6, 9, BoardPressureType.LowCash, "Margin squeeze", "Delivery fees and quality pressure are starting to bite.", "FF13", "FF09"),
                    MakeEventWindow("ff_reputation_risk", 10, 16, BoardPressureType.LowRating, "Reputation risk", "Weak cleanup or service wobble can now spiral publicly.", "FF12", "FF09")
                }),
                CreateEventChain(VentureType.Cafe, new[]
                {
                    MakeEventWindow("cf_opening_friction", 3, 5, BoardPressureType.CapacityShortfall, "Opening friction", "The first real rush exposes a weak floor and slow drink handoff.", "CF14", "CF09"),
                    MakeEventWindow("cf_quality_break", 6, 9, BoardPressureType.WeakQuality, "Quality break", "Supply inconsistency starts leaking into guest trust.", "CF13", "CF14"),
                    MakeEventWindow("cf_staff_crack", 10, 16, BoardPressureType.StaffInstability, "Shift crack", "Burnout and complaints turn into public pressure.", "CF09", "CF14")
                }),
                CreateEventChain(VentureType.TechApp, new[]
                {
                    MakeEventWindow("tc_opening_friction", 3, 5, BoardPressureType.LowRating, "Opening friction", "Early users are already stress-testing the core experience.", "TC09", "TC12"),
                    MakeEventWindow("tc_scale_shock", 6, 9, BoardPressureType.CapacityShortfall, "Scale shock", "Growth is exposing backend, infra, or trust weak spots.", "TC21", "TC24", "TC18"),
                    MakeEventWindow("tc_late_crisis", 10, 16, BoardPressureType.HighLegalRisk, "Trust crisis", "The product is big enough now that trust failures become expensive.", "TC18", "TC27", "TC15")
                }),
                CreateEventChain(VentureType.ClothingStore, new[]
                {
                    MakeEventWindow("cl_opening_friction", 3, 5, BoardPressureType.LowDemand, "Opening friction", "The floor looks promising, but stock and season bets are still fragile.", "CL09", "CL14"),
                    MakeEventWindow("cl_fit_risk", 6, 9, BoardPressureType.WeakQuality, "Fit risk", "Returns and quality claims are starting to define the brand.", "CL13", "CL14"),
                    MakeEventWindow("cl_margin_risk", 10, 16, BoardPressureType.LowCash, "Margin risk", "Wrong inventory timing is now turning into balance-sheet pressure.", "CL09", "CL13")
                }),
                CreateEventChain(VentureType.GroceryStore, new[]
                {
                    MakeEventWindow("gr_opening_friction", 3, 5, BoardPressureType.CapacityShortfall, "Opening friction", "Checkout and shelf rhythm are under pressure from repeat trips.", "GR13", "GR09"),
                    MakeEventWindow("gr_freshness_risk", 6, 9, BoardPressureType.WeakQuality, "Freshness risk", "Spoilage and cold-chain discipline are becoming visible to regulars.", "GR09", "GR14"),
                    MakeEventWindow("gr_trust_drop", 10, 16, BoardPressureType.LowRating, "Neighborhood trust drop", "Local rumor and empty shelf pain can now flip baskets toward the rival.", "GR14", "GR13")
                })
            };
        }

        private static VentureEventChain CreateEventChain(VentureType type, VentureEventWindow[] windows)
        {
            var chain = ScriptableObject.CreateInstance<VentureEventChain>();
            chain.ventureType = type;
            chain.windows = windows;
            chain.name = $"EventChain_{type}";
            return chain;
        }

        private static VentureEventWindow MakeEventWindow(string windowId, int turnStart, int turnEnd, BoardPressureType pressure, string title, string detail, params string[] cardIds)
        {
            return new VentureEventWindow
            {
                windowId = windowId,
                turnStart = turnStart,
                turnEnd = turnEnd,
                requiredPressure = pressure,
                title = title,
                detail = detail,
                preferredEventCardIds = cardIds
            };
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
            yield return MakeStaff("FF10", "Front Counter Server", VentureType.FastFood, "cashier", "Moves queue friction off the grill line.", 36, 0.4f, 1.2f, 0.3f, 0.8f, new[] { CardTag.Food, CardTag.Support });
            yield return MakeStaff("FF11", "Night Cleaner", VentureType.FastFood, "cleaning", "Protects hygiene before the next rush hits.", 32, 0.1f, 0.5f, 0.9f, 1.0f, new[] { CardTag.Support, CardTag.Defensive });
            yield return MakeCrisis("FF12", "Hygiene Inspection", VentureType.FastFood, "Missed cleaning turns into a trust-killing inspection.", "hygiene_crisis", 2, -0.7f, -1.3f, 9f, new[] { CardTag.Risky, CardTag.Support });
            yield return MakeCrisis("FF13", "Delivery Fee Squeeze", VentureType.FastFood, "Platform commissions spike and delivery margins collapse.", "delivery_fee", 2, -0.4f, -0.4f, 4f, new[] { CardTag.Pricing, CardTag.Risky });
            yield return MakeTemp("FF14", "Deep Clean Reset", VentureType.FastFood, CardFamily.Reaction, "deep_clean", "Emergency cleanup protects trust after hygiene slips.", 24, 0.2f, 0.3f, -12f, 0.9f, 1, null, new[] { "hygiene_crisis", "review_crisis" }, new[] { CardTag.Support, CardTag.Defensive });
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
            yield return MakeStaff("CF10", "Floor Runner", VentureType.Cafe, "floor", "Clears tables and keeps the bar flowing.", 34, 0.3f, 1.1f, 0.3f, 0.8f, new[] { CardTag.Support, CardTag.Basic });
            yield return MakeSupplier("CF11", "Milk Contract", VentureType.Cafe, "milk", "Reliable milk supply saves rush-hour consistency.", 36, 0.2f, 1.0f, 0.3f, 0.2f, new[] { "bean_shortage", "slow_service" }, new[] { CardTag.Support, CardTag.Coffee });
            yield return MakeMarketing("CF12", "Regulars Stamp Card", VentureType.Cafe, "loyalty", "Builds sticky morning traffic and repeat orders.", 24, 1.5f, 0.35f, 7f, new[] { CardTag.Marketing, CardTag.Support });
            yield return MakeCrisis("CF13", "Bean Shortage", VentureType.Cafe, "Your bean supplier slips and drink quality takes a hit.", "bean_shortage", 2, -0.5f, -1.1f, 5f, new[] { CardTag.Coffee, CardTag.Risky });
            yield return MakeCrisis("CF14", "Slow Service Backlash", VentureType.Cafe, "A packed weekend turns into public complaints about waits.", "slow_service", 2, -1.1f, -0.6f, 4f, new[] { CardTag.Support, CardTag.Risky });
            yield return MakeTemp("CF15", "Reset The Shift", VentureType.Cafe, CardFamily.Reaction, "shift_reset", "Extra help and workflow resets calm the floor fast.", 22, 0.3f, 0.8f, -8f, 0.8f, 1, null, new[] { "staff_crisis", "slow_service" }, new[] { CardTag.Support, CardTag.Management });
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
            yield return MakeOperation("TC10", "Template Engine", VentureType.TechApp, "product", "Turns design work into reusable output.", 95, 1.4f, 1.0f, 1.4f, 0.8f, 4.5f, 4.5f, 3.3f, new[] { CardTag.Tech, CardTag.Scaling });
            yield return MakeSupplier("TC11", "Export Pipeline", VentureType.TechApp, "api", "Keeps large exports and collaboration smoother.", 42, 0.2f, 1.2f, 0.4f, 0.25f, new[] { "stability_crisis", "rating_drop" }, new[] { CardTag.Tech, CardTag.Support });
            yield return MakeCrisis("TC12", "File Corruption Wave", VentureType.TechApp, "Broken exports and corrupted projects trigger angry teams.", "design_corruption", 2, -1.0f, -1.0f, 6f, new[] { CardTag.Tech, CardTag.Risky });
            yield return MakeMarketing("TC13", "Habit Streaks", VentureType.TechApp, "community", "Healthy habit loops grow daily retention.", 34, 1.8f, 0.35f, 10f, new[] { CardTag.Marketing, CardTag.Support });
            yield return MakeStaff("TC14", "Coach Network", VentureType.TechApp, "supporthire", "Boosts trust and user accountability.", 48, 0.3f, 1.0f, 0.9f, 0.9f, new[] { CardTag.Support, CardTag.Management });
            yield return MakeCrisis("TC15", "Trust Breach", VentureType.TechApp, "Workout data confusion damages coaching credibility.", "health_trust", 2, -0.8f, -0.7f, 7f, new[] { CardTag.Tech, CardTag.Risky });
            yield return MakeMarketing("TC16", "Discovery Feed Tuning", VentureType.TechApp, "creator", "Better match quality lifts acquisition fast.", 38, 2.2f, 0.15f, 14f, new[] { CardTag.Marketing, CardTag.Viral });
            yield return MakeTemp("TC17", "Moderation Strike Team", VentureType.TechApp, CardFamily.Reaction, "support", "Absorbs abuse spikes before reviews crater.", 28, 0.1f, 0.5f, -6f, 0.8f, 1, null, new[] { "privacy_backlash", "rating_drop" }, new[] { CardTag.Support, CardTag.Defensive });
            yield return MakeCrisis("TC18", "Privacy Backlash", VentureType.TechApp, "Users revolt after trust boundaries are crossed.", "privacy_backlash", 2, -1.2f, -0.5f, 10f, new[] { CardTag.Tech, CardTag.Illegal });
            yield return MakeOperation("TC19", "Prompt Lab", VentureType.TechApp, "product", "Turns experiments into sticky AI workflows.", 108, 1.6f, 1.0f, 1.5f, 0.9f, 4f, 5f, 3.6f, new[] { CardTag.Tech, CardTag.AI });
            yield return MakeSupplier("TC20", "GPU Reservation", VentureType.TechApp, "cloud", "Secures throughput when demand spikes.", 62, 0.2f, 1.0f, 0.5f, 0.3f, new[] { "stability_crisis", "cost_spike" }, new[] { CardTag.Tech, CardTag.AI });
            yield return MakeCrisis("TC21", "Inference Cost Spike", VentureType.TechApp, "Model demand explodes and serving cost bites hard.", "cost_spike", 2, -0.5f, -0.3f, 5f, new[] { CardTag.Tech, CardTag.AI });
            yield return MakeMarketing("TC22", "Live Ops Calendar", VentureType.TechApp, "community", "Events and quests keep sessions alive.", 32, 2.0f, 0.2f, 12f, new[] { CardTag.Marketing, CardTag.Entertainment });
            yield return MakeSupplier("TC23", "Rewarded Ad Stack", VentureType.TechApp, "payments", "Adds monetization with some user irritation.", 36, 0.4f, 0.2f, 0.8f, -0.05f, new[] { "retention_crash" }, new[] { CardTag.Tech, CardTag.Marketing });
            yield return MakeCrisis("TC24", "Retention Collapse", VentureType.TechApp, "Fresh installs bounce after weak day-one retention.", "retention_crash", 2, -1.3f, -0.4f, 4f, new[] { CardTag.Tech, CardTag.Risky });
            yield return MakeOperation("TC25", "Prototype Mill", VentureType.TechApp, "product", "Ships experiments at relentless speed.", 82, 1.9f, 0.8f, 0.2f, 0.7f, 3f, 4f, 3.2f, new[] { CardTag.Tech, CardTag.Aggressive });
            yield return MakeMarketing("TC26", "CPI Burst", VentureType.TechApp, "ads", "Cheap installs now, fragile quality later.", 26, 2.7f, -0.05f, 18f, new[] { CardTag.Marketing, CardTag.Aggressive });
            yield return MakeCrisis("TC27", "Clone Swarm", VentureType.TechApp, "Copycat apps flood your lane and shred cheap traffic.", "clone_swarm", 2, -1.0f, -0.4f, 7f, new[] { CardTag.Tech, CardTag.Risky });
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
            yield return MakeStaff("CL10", "In-House Tailor", VentureType.ClothingStore, "tailor", "Fixes fit issues before they become return waves.", 44, 0.2f, 0.9f, 0.9f, 0.9f, new[] { CardTag.Support, CardTag.Luxury });
            yield return MakeSupplier("CL11", "Premium Fabric Mill", VentureType.ClothingStore, "wholesale", "Sharper materials lift trust and hold margins.", 48, 0.2f, 1.3f, 0.5f, 0.25f, new[] { "quality_claim", "inventory_crisis" }, new[] { CardTag.Support, CardTag.Luxury });
            yield return MakeMarketing("CL12", "Window Story Display", VentureType.ClothingStore, "shoppingads", "A stronger storefront narrative converts trend traffic.", 28, 1.7f, 0.2f, 10f, new[] { CardTag.Marketing, CardTag.Trendy });
            yield return MakeCrisis("CL13", "Return Surge", VentureType.ClothingStore, "Sizing issues trigger a painful spike in returns.", "returns", 2, -0.6f, -0.9f, 5f, new[] { CardTag.Support, CardTag.Risky });
            yield return MakeCrisis("CL14", "Influencer Mismatch", VentureType.ClothingStore, "A partnership lands with the wrong audience and trust slips.", "influencer_mismatch", 2, -0.9f, -0.5f, 3f, new[] { CardTag.Influencer, CardTag.Risky });
            yield return MakeTemp("CL15", "Alteration Voucher", VentureType.ClothingStore, CardFamily.Reaction, "alteration_voucher", "Buys time and goodwill when fit complaints spread.", 24, 0.3f, 0.6f, -8f, 0.8f, 1, null, new[] { "returns", "quality_claim" }, new[] { CardTag.Support, CardTag.Luxury });
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
            yield return MakeStaff("GR10", "Shelf Stocker", VentureType.GroceryStore, "stocker", "Keeps lanes full during rush windows.", 30, 0.4f, 1.2f, 0.2f, 0.7f, new[] { CardTag.Support, CardTag.Basic });
            yield return MakeSupplier("GR11", "Cold Chain Dairy", VentureType.GroceryStore, "dairy", "Extends freshness and steadies trust around staples.", 38, 0.2f, 1.1f, 0.4f, 0.2f, new[] { "freshness", "skt" }, new[] { CardTag.Organic, CardTag.Support });
            yield return MakeMarketing("GR12", "Mahalle Defteri", VentureType.GroceryStore, "loyalty", "Neighborhood credit builds loyalty at a margin cost.", 16, 1.2f, 0.25f, 5f, new[] { CardTag.Marketing, CardTag.Support });
            yield return MakeCrisis("GR13", "Shelf Gap Panic", VentureType.GroceryStore, "Empty shelves push regulars toward the rival store.", "shelf_gap", 2, -1.0f, -0.4f, 2f, new[] { CardTag.Basic, CardTag.Risky });
            yield return MakeCrisis("GR14", "Neighborhood Trust Drop", VentureType.GroceryStore, "Rumors around stale goods spread through the block.", "trust_drop", 2, -0.8f, -0.8f, 6f, new[] { CardTag.Support, CardTag.Risky });
            yield return MakeTemp("GR15", "Freshness Audit", VentureType.GroceryStore, CardFamily.Reaction, "fresh_audit", "Emergency checks and relabeling calm freshness fears.", 20, 0.2f, 0.5f, -12f, 0.8f, 1, null, new[] { "freshness", "skt", "trust_drop" }, new[] { CardTag.Support, CardTag.Defensive });
        }

        private static IEnumerable<CardData> CreateNeutralCards()
        {
            yield return MakeMarketing("NT01", "Local Buzz", VentureType.FastFood, "neutral_marketing", "A flexible marketing bump for any venture.", 25, 1.2f, 0.2f, 8f, new[] { CardTag.Marketing, CardTag.Basic }, true);
            yield return MakeSupplier("NT02", "Cash Buffer", VentureType.FastFood, "neutral_supplier", "Small financial breathing room every turn.", 30, 0f, 0.2f, 0.2f, 0f, new[] { "stability" }, new[] { CardTag.Finance, CardTag.Support }, true, 18f, 4f);
            yield return MakeTemp("NT03", "Compliance Sweep", VentureType.FastFood, CardFamily.Reaction, "compliance", "Reduces legal risk when things get messy.", 18, 0f, 0f, -18f, 0.2f, 1, null, new[] { "privacy", "review_crisis", "quality_claim", "skt" }, new[] { CardTag.Support, CardTag.Defensive }, true);
            yield return MakeTemp("NT04", "Emergency Hire", VentureType.FastFood, CardFamily.Reaction, "emergency_hire", "Stabilizes capacity and staff morale for one turn.", 20, 0.4f, 1.0f, -4f, 0.1f, 1, null, new[] { "staff_crisis", "slow_service", "stability_crisis" }, new[] { CardTag.Hiring, CardTag.Support }, true);
            yield return MakeMarketing("NT05", "Seasonal Push", VentureType.FastFood, "neutral_late", "Late-run momentum card for any venture.", 40, 1.8f, 0.35f, 12f, new[] { CardTag.Marketing, CardTag.Scaling }, true);
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
            card.staffRole = ResolveStaffRole(venture, subSlot);
            card.defaultTrialTurns = 2;
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
            card.workloadDeltaPerTurn = family == CardFamily.Risk ? Mathf.Max(0.5f, demand * 0.8f) : -0.75f;
            card.fatigueDeltaPerTurn = family == CardFamily.Risk ? 1 : -2;
            card.moraleDeltaPerTurn = family == CardFamily.Risk ? -1 : 1;
            card.loyaltyDeltaPerTurn = family == CardFamily.Risk ? -1 : 0;
            card.burnoutRiskDeltaPerTurn = family == CardFamily.Risk ? 0.08f : -0.12f;
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
            card.workloadDeltaPerTurn = Mathf.Abs(demandPenalty) + Mathf.Abs(qualityPenalty);
            card.fatigueDeltaPerTurn = 1;
            card.moraleDeltaPerTurn = -1;
            card.burnoutRiskDeltaPerTurn = 0.08f;
            card.crisisTags = new[] { crisisTag };
            card.buildingScale = new Vector3(0.85f, 0.7f, 0.85f);
            card.buildingColor = new Color(0.48f, 0.10f, 0.10f);
            card.buildingLabel = name;
            return card;
        }

        private static StaffRole ResolveStaffRole(VentureType venture, string subSlot)
        {
            if (string.IsNullOrWhiteSpace(subSlot))
                return StaffRole.Generalist;

            switch (subSlot)
            {
                case "chef": return StaffRole.Chef;
                case "cashier": return StaffRole.Cashier;
                case "courier": return StaffRole.Courier;
                case "cleaning": return StaffRole.Cleaning;
                case "manager":
                case "lead": return StaffRole.Manager;
                case "barista": return StaffRole.Barista;
                case "floor": return StaffRole.Floor;
                case "developer": return StaffRole.Developer;
                case "designer": return StaffRole.Designer;
                case "growthhire": return StaffRole.Growth;
                case "supporthire": return StaffRole.Support;
                case "pm": return StaffRole.ProductManager;
                case "sales": return StaffRole.Sales;
                case "tailor": return StaffRole.Tailor;
                case "stocker": return StaffRole.Stocker;
                case "freshkeeper": return StaffRole.FreshKeeper;
                default:
                    return venture == VentureType.Cafe ? StaffRole.Floor : StaffRole.Generalist;
            }
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
