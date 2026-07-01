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
            cards.AddRange(CreateNeutralCards());
            return cards.ToArray();
        }

        // ── Kitchen / Operation ─────────────────────────────────────

        static CardData[] CreateKitchenCards()
        {
            return new[]
            {
                // ── Existing 3 cards (RES_K01-K03) ──────────────────────

                Make("RES_K01", "Kucuk Dukkan", CardType.Operation, CardFamily.Setup,
                    Rarity.Common, SlotType.Kitchen,
                    desc: "Mahalle arasinda kucuk ama samimi bir mekan.",
                    buyCost: 80, capacityDelta: 2f, qualityDelta: 1f),

                Make("RES_K02", "Ekstra Masa", CardType.Operation, CardFamily.Setup,
                    Rarity.Common, SlotType.Kitchen,
                    desc: "Birkaç masa daha sığdırdık, artık daha kalabalık.",
                    buyCost: 40, capacityDelta: 1.5f),

                Make("RES_K03", "Izgara Istasyonu", CardType.Operation, CardFamily.Growth,
                    Rarity.Uncommon, SlotType.Kitchen,
                    desc: "Mangal kokusunu taa sokaktan hissedersin.",
                    buyCost: 60, qualityDelta: 1.2f, capacityDelta: 1f),

                // ── New cards (RES_K04-K08) ─────────────────────────────

                Make("RES_K04", "Paket Servis Istasyonu", CardType.Operation, CardFamily.Growth,
                    Rarity.Uncommon, SlotType.Kitchen,
                    desc: "Delivery acildi! Musteri kapida bekliyor.",
                    buyCost: 50, capacityDelta: 1.5f, demandDelta: 1f,
                    tags: new[] { "delivery_unlock" }),

                Make("RES_K05", "Tatli Tezgahi", CardType.Operation, CardFamily.Setup,
                    Rarity.Common, SlotType.Kitchen,
                    desc: "Kunefe, baklava, sutlac -- tatli olmadan sofra olmaz.",
                    buyCost: 35, qualityDelta: 0.5f, cashPerTurn: 10f),

                Make("RES_K06", "Mutfak Renovasyonu", CardType.Operation, CardFamily.Growth,
                    Rarity.Rare, SlotType.Kitchen,
                    desc: "Her sey yenilendi: tezgah, ocak, aspirator.",
                    buyCost: 100, qualityDelta: 2f, capacityDelta: 2f),

                Make("RES_K07", "Dis Mekan Teras", CardType.Operation, CardFamily.Growth,
                    Rarity.Uncommon, SlotType.Kitchen,
                    desc: "Yazin dis mekanda yemek baska guzel. Kis icin dusunme.",
                    buyCost: 70, capacityDelta: 3f,
                    isSeasonal: true, tags: new[] { "summer_only" }),

                Make("RES_K08", "Franchise Mutfak", CardType.Operation, CardFamily.Growth,
                    Rarity.Legendary, SlotType.Kitchen,
                    desc: "Artik tek mutfak yetmez -- ikinci sube hazirligi.",
                    buyCost: 150, capacityDelta: 4f, qualityDelta: 1f),

                Make("RES_K09", "Breakfast Corner", CardType.Operation, CardFamily.Setup,
                    Rarity.Common, SlotType.Kitchen,
                    desc: "Sabah kahvaltisi servisi. Simit, peynir, cay.",
                    buyCost: 30, capacityDelta: 1f, cashPerTurn: 8f),

                Make("RES_K10", "Gece Mutfagi", CardType.Operation, CardFamily.Growth,
                    Rarity.Uncommon, SlotType.Kitchen,
                    desc: "Gece vardiyasi acildi. 24 saat hizmet.",
                    buyCost: 80, capacityDelta: 2f, upkeep: 15),

                Make("RES_K11", "Catering Servisi", CardType.Operation, CardFamily.Growth,
                    Rarity.Rare, SlotType.Kitchen,
                    desc: "Dis mekan yemek organizasyonlari. Kurumsal siparisler gelir.",
                    buyCost: 90, demandDelta: 2f, cashPerTurn: 15f),
            };
        }

        // ── Staff ───────────────────────────────────────────────────

        static CardData[] CreateStaffCards()
        {
            return new[]
            {
                // ── Existing 3 cards (RES_S01-S03) ──────────────────────

                Make("RES_S01", "Cylak Asci", CardType.Staff, CardFamily.Setup,
                    Rarity.Common, SlotType.Salon,
                    desc: "Yumurta bile yakamiyor ama ucuz ve hevesli.",
                    buyCost: 0, upkeep: 8, qualityDelta: 0.5f,
                    subSlot: "asci", staffTier: StaffTier.Intern, canPromote: true),

                Make("RES_S02", "Ucuz Garson", CardType.Staff, CardFamily.Setup,
                    Rarity.Common, SlotType.Salon,
                    desc: "Siparis aliyor, tabak tasiyor. Beklemeyin fazlasini.",
                    buyCost: 0, upkeep: 5, capacityDelta: 0.5f,
                    subSlot: "garson", staffTier: StaffTier.Intern, canPromote: true),

                Make("RES_S03", "Temizlikci", CardType.Staff, CardFamily.Setup,
                    Rarity.Common, SlotType.Salon,
                    desc: "Mutfak piril piril. Saglik mudurlugu gelsin isterse.",
                    buyCost: 0, upkeep: 6, hygieneDelta: 1.5f,
                    subSlot: "temizlikci", staffTier: StaffTier.Junior),

                // ── New cards (RES_S04-S11) ─────────────────────────────

                Make("RES_S04", "Komi", CardType.Staff, CardFamily.Setup,
                    Rarity.Common, SlotType.Salon,
                    desc: "Sebze dograr, tabak hazirlar. Sef'in sag kolu.",
                    buyCost: 0, upkeep: 8, qualityDelta: 0.3f, capacityDelta: 0.5f,
                    subSlot: "komi", staffTier: StaffTier.Intern, canPromote: true),

                Make("RES_S05", "Deneyimli Garson", CardType.Staff, CardFamily.Growth,
                    Rarity.Uncommon, SlotType.Salon,
                    desc: "Musterilerin adini biliyor. Bahsis rekortmeni.",
                    buyCost: 0, upkeep: 15, capacityDelta: 1.2f, ratingDelta: 0.2f,
                    subSlot: "garson", staffTier: StaffTier.Experienced, canPromote: true),

                Make("RES_S06", "Kurye", CardType.Staff, CardFamily.Setup,
                    Rarity.Common, SlotType.Salon,
                    desc: "Motor ustunde sogumadan teslim eder. Yagmur kar dinlemez.",
                    buyCost: 0, upkeep: 10, demandDelta: 1.5f,
                    subSlot: "kurye", staffTier: StaffTier.Junior,
                    tags: new[] { "delivery_staff" }),

                Make("RES_S07", "Kasiyerci", CardType.Staff, CardFamily.Setup,
                    Rarity.Common, SlotType.Salon,
                    desc: "Hesapta yanilmaz. Musteri kuyrugu erimez.",
                    buyCost: 0, upkeep: 7, capacityDelta: 0.3f,
                    subSlot: "kasiyer", staffTier: StaffTier.Junior),

                Make("RES_S08", "Growth Hacker", CardType.Staff, CardFamily.Growth,
                    Rarity.Rare, SlotType.Salon,
                    desc: "Sosyal medya sihirbazi. Etik mi? Tartisalim...",
                    buyCost: 0, upkeep: 25, demandDelta: 1.5f, legalRiskDelta: 3f,
                    subSlot: "growth", staffTier: StaffTier.Experienced),

                Make("RES_S09", "Community Manager", CardType.Staff, CardFamily.Growth,
                    Rarity.Uncommon, SlotType.Salon,
                    desc: "Kotu yorumlara guzel cevap yazar. Krizleri onler.",
                    buyCost: 0, upkeep: 10, ratingDelta: 0.4f,
                    subSlot: "community", staffTier: StaffTier.Junior),

                Make("RES_S10", "Sube Muduru", CardType.Staff, CardFamily.Growth,
                    Rarity.Rare, SlotType.Salon,
                    desc: "Dukkanin basinda duran adam. Herkes isini yapar.",
                    buyCost: 0, upkeep: 20, staffStabilityDelta: 2f, capacityDelta: 1f,
                    subSlot: "mudur", staffTier: StaffTier.Senior),

                Make("RES_S11", "Usta Sef", CardType.Staff, CardFamily.Growth,
                    Rarity.Legendary, SlotType.Salon,
                    desc: "Michelin yildizli mutfaktan geldi. Efsane lezzetler.",
                    buyCost: 0, upkeep: 50, qualityDelta: 2.5f, ratingDelta: 0.5f,
                    subSlot: "asci", staffTier: StaffTier.Master),

                Make("RES_S12", "Bulasikcici", CardType.Staff, CardFamily.Setup,
                    Rarity.Common, SlotType.Salon,
                    desc: "Tabaklar birikmesin. Hizli elden gecirir.",
                    buyCost: 0, upkeep: 5, hygieneDelta: 1f,
                    subSlot: "temizlikci", staffTier: StaffTier.Intern, canPromote: true),

                Make("RES_S13", "Pastaci", CardType.Staff, CardFamily.Growth,
                    Rarity.Uncommon, SlotType.Salon,
                    desc: "Pasta, tatli, borek. Menunun yildizi olur.",
                    buyCost: 0, upkeep: 18, qualityDelta: 1f, cashPerTurn: 8f,
                    subSlot: "pastaci", staffTier: StaffTier.Experienced),

                Make("RES_S14", "Valet Gorevlisi", CardType.Staff, CardFamily.Setup,
                    Rarity.Common, SlotType.Salon,
                    desc: "Park sorunu cozuldu. Musteri rahat gelir.",
                    buyCost: 0, upkeep: 8, capacityDelta: 0.5f, ratingDelta: 0.1f,
                    subSlot: "valet", staffTier: StaffTier.Junior),
            };
        }

        // ── Marketing ───────────────────────────────────────────────

        static CardData[] CreateMarketingCards()
        {
            return new[]
            {
                // ── Existing 2 cards (RES_M01-M02) ──────────────────────

                Make("RES_M01", "Brosur Dagitimi", CardType.Marketing, CardFamily.Setup,
                    Rarity.Common, SlotType.Marketing,
                    desc: "Kapiya birakilir, cogu cope gider ama ise yarar.",
                    buyCost: 15, demandDelta: 1f),

                Make("RES_M02", "Google Reklam", CardType.Marketing, CardFamily.Growth,
                    Rarity.Uncommon, SlotType.Marketing,
                    desc: "Yakininizda restaurant diye aratinca biz cikiyoruz.",
                    buyCost: 25, upkeep: 25, demandDelta: 2.5f),

                // ── New cards (RES_M03-M08) ─────────────────────────────

                Make("RES_M03", "Instagram Sayfasi", CardType.Marketing, CardFamily.Setup,
                    Rarity.Common, SlotType.Marketing,
                    desc: "Yemek fotosu cek, filtre at, paylas. Bedava reklam.",
                    buyCost: 10, ratingDelta: 0.2f, demandDelta: 0.5f),

                Make("RES_M04", "TikTok Influencer", CardType.Marketing, CardFamily.Growth,
                    Rarity.Rare, SlotType.Marketing,
                    desc: "Bir video cekti, 1 milyon izlendi. Ama etkisi gecici.",
                    buyCost: 60, demandDelta: 4f,
                    duration: 1),

                Make("RES_M05", "Yemek App Anlasmasi", CardType.Marketing, CardFamily.Growth,
                    Rarity.Uncommon, SlotType.Marketing,
                    desc: "Yemeksepeti/Getir anlasmasi. Siparis yagar ama komisyon keser.",
                    buyCost: 0, upkeep: 30, demandDelta: 3f,
                    tags: new[] { "delivery_app", "commission" }),

                Make("RES_M06", "Yerel Etkinlik Sponsoru", CardType.Marketing, CardFamily.Growth,
                    Rarity.Uncommon, SlotType.Marketing,
                    desc: "Mahalle festivaline sponsor olduk. Adimiz duyuldu.",
                    buyCost: 40, demandDelta: 2f,
                    duration: 3),

                Make("RES_M07", "Sadakat Karti", CardType.Marketing, CardFamily.Growth,
                    Rarity.Uncommon, SlotType.Marketing,
                    desc: "10 yemekte 1 bedava. Musteriler geri geliyor.",
                    buyCost: 25, ratingDelta: 0.3f,
                    tags: new[] { "loyalty_30pct" }),

                Make("RES_M08", "Referans Sistemi", CardType.Marketing, CardFamily.Growth,
                    Rarity.Rare, SlotType.Marketing,
                    desc: "Arkadasini getir, ikisine de indirim. Kartopu etkisi.",
                    buyCost: 30, demandDelta: 0.5f,
                    tags: new[] { "compound_demand" }),

                Make("RES_M09", "Yerel Gazete Ilani", CardType.Marketing, CardFamily.Setup,
                    Rarity.Common, SlotType.Marketing,
                    desc: "Mahalle gazetesine ilan verdik. Yasli kesim goruyor.",
                    buyCost: 15, demandDelta: 0.8f),

                Make("RES_M10", "YouTube Yemek Kanali", CardType.Marketing, CardFamily.Growth,
                    Rarity.Rare, SlotType.Marketing,
                    desc: "Mutfaktan canli yayin! Izleyiciler merak edip geliyor.",
                    buyCost: 40, demandDelta: 2f, ratingDelta: 0.2f,
                    tags: new[] { "content_marketing" }),
            };
        }

        // ── Supplier ────────────────────────────────────────────────

        static CardData[] CreateSupplierCards()
        {
            return new[]
            {
                // ── Existing 2 cards (RES_T01-T02) ──────────────────────

                Make("RES_T01", "Kasap (Hal'den Taze Et)", CardType.Supplier, CardFamily.Setup,
                    Rarity.Common, SlotType.Storage,
                    desc: "Her sabah taze et gelir. Kalite tartismasiz.",
                    buyCost: 0, upkeep: 20, qualityDelta: 1.5f, hygieneDelta: 0.5f,
                    subSlot: "kasap"),

                Make("RES_T02", "Ucuz Donmus Et", CardType.Supplier, CardFamily.Setup,
                    Rarity.Common, SlotType.Storage,
                    desc: "Dondurucudan cikma et. Ucuz ama sorgulanir.",
                    buyCost: 0, upkeep: 5, qualityDelta: 0.3f, hygieneDelta: -0.3f,
                    subSlot: "kasap"),

                // ── New cards (RES_T03-T07) ─────────────────────────────

                Make("RES_T03", "Organik Tedarikci", CardType.Supplier, CardFamily.Growth,
                    Rarity.Rare, SlotType.Storage,
                    desc: "Organik sertifikali ciftlikten geliyor. Pahali ama gurur verici.",
                    buyCost: 0, upkeep: 25, qualityDelta: 2.0f, ratingDelta: 0.3f,
                    subSlot: "manav"),

                Make("RES_T04", "Mahalle Firinci", CardType.Supplier, CardFamily.Setup,
                    Rarity.Common, SlotType.Storage,
                    desc: "Sicak ekmek kokusu sokaktan geliyor. Her sabah taze.",
                    buyCost: 0, upkeep: 8, qualityDelta: 0.8f, hygieneDelta: 0.3f,
                    subSlot: "firinci"),

                Make("RES_T05", "Yerel Manav", CardType.Supplier, CardFamily.Setup,
                    Rarity.Common, SlotType.Storage,
                    desc: "Sebze meyve taptaze. Mevsiminde ne varsa o gelir.",
                    buyCost: 0, upkeep: 10, qualityDelta: 1.0f, hygieneDelta: 0.5f,
                    subSlot: "manav"),

                Make("RES_T06", "Icecek Anlasmasi", CardType.Supplier, CardFamily.Setup,
                    Rarity.Common, SlotType.Storage,
                    desc: "Kola/Pepsi anlasmasini yaptin. Buzdolabi bedava geldi.",
                    buyCost: 0, upkeep: 12, qualityDelta: 0.3f, cashPerTurn: 5f,
                    subSlot: "icecek"),

                Make("RES_T07", "Toptan Ithal Gida", CardType.Supplier, CardFamily.Setup,
                    Rarity.Common, SlotType.Storage,
                    desc: "Cin'den gelen ucuz malzeme. Kaliteden sor(am)iyoruz.",
                    buyCost: 0, upkeep: 3, qualityDelta: -0.5f, hygieneDelta: -0.2f,
                    capacityDelta: 2f, subSlot: "toptan_gida"),

                Make("RES_T08", "Baharatci", CardType.Supplier, CardFamily.Setup,
                    Rarity.Common, SlotType.Storage,
                    desc: "Misir Carsisi'ndan gelen baharatlar. Lezzet patlayisi.",
                    buyCost: 0, upkeep: 6, qualityDelta: 0.5f,
                    subSlot: "baharat"),

                Make("RES_T09", "Soguk Zincir Deposu", CardType.Supplier, CardFamily.Growth,
                    Rarity.Uncommon, SlotType.Storage,
                    desc: "Kendi soguk depomuz var. Bozulma riski sifir.",
                    buyCost: 40, upkeep: 12, hygieneDelta: 1f, capacityDelta: 1f,
                    subSlot: "depo"),
            };
        }

        // ── Risk ────────────────────────────────────────────────────

        static CardData[] CreateRiskCards()
        {
            return new[]
            {
                // ── Existing 2 cards (RES_R01-R02) ──────────────────────

                Make("RES_R01", "Sahte Yorum", CardType.Risk, CardFamily.Risk,
                    Rarity.Common, SlotType.TempEffect,
                    desc: "Google'a 5 yildiz yazdirdik. Yakalanmasak iyi.",
                    buyCost: 15, ratingDelta: 0.8f,
                    shortTermBenefit: 0.8f, longTermRisk: 25f,
                    triggersCrisis: CrisisType.ReviewBomb),

                Make("RES_R02", "Sigortasiz Calisan", CardType.Risk, CardFamily.Risk,
                    Rarity.Common, SlotType.TempEffect,
                    desc: "Sigortayi niye yapayim, kimse denetlemiyor ki...",
                    buyCost: 0, legalRiskDelta: 5f,
                    shortTermBenefit: 0.4f, longTermRisk: 15f,
                    triggersCrisis: CrisisType.HygieneInspection),

                // ── New cards (RES_R03-R08) ─────────────────────────────

                Make("RES_R03", "Karaborsa Et", CardType.Risk, CardFamily.Risk,
                    Rarity.Uncommon, SlotType.TempEffect,
                    desc: "Nereden geldigini sorma. Ucuz ve lezzetli.",
                    buyCost: 0, upkeep: 2, qualityDelta: 0.5f,
                    legalRiskDelta: 8f, hygieneDelta: -0.5f,
                    shortTermBenefit: 0.5f, longTermRisk: 30f,
                    triggersCrisis: CrisisType.FoodPoisoning),

                Make("RES_R04", "Vergi Kacirma", CardType.Risk, CardFamily.Risk,
                    Rarity.Rare, SlotType.TempEffect,
                    desc: "Kasayi acmiyoruz. Parayi cebe atiyoruz.",
                    buyCost: 0, cashPerTurn: 30f, legalRiskDelta: 10f,
                    shortTermBenefit: 1f, longTermRisk: 40f),

                Make("RES_R05", "Crunch Mode", CardType.Risk, CardFamily.Risk,
                    Rarity.Uncommon, SlotType.TempEffect,
                    desc: "Herkese extra mesai. Bu hafta kimse eve gitmez.",
                    buyCost: 0, capacityDelta: 4f, staffStabilityDelta: -3f,
                    shortTermBenefit: 0.8f, longTermRisk: 20f,
                    duration: 1, triggersCrisis: CrisisType.StaffQuit),

                Make("RES_R06", "Rusvet", CardType.Risk, CardFamily.Risk,
                    Rarity.Rare, SlotType.TempEffect,
                    desc: "Mufattise zarfi uzat. Ya kabul eder ya ihbar eder.",
                    buyCost: 20, legalRiskDelta: -15f,
                    shortTermBenefit: 0.6f, longTermRisk: 50f,
                    duration: 1),

                Make("RES_R07", "Rakipten Sef Cal", CardType.Risk, CardFamily.Risk,
                    Rarity.Rare, SlotType.TempEffect,
                    desc: "Karsi restoranin sef'ine teklif yaptik. Rakip kiziyor.",
                    buyCost: 30, qualityDelta: 1.5f,
                    shortTermBenefit: 0.7f, longTermRisk: 20f,
                    tags: new[] { "rival_provoke" }),

                Make("RES_R08", "Fiyat Manipulasyonu", CardType.Risk, CardFamily.Risk,
                    Rarity.Uncommon, SlotType.TempEffect,
                    desc: "Fiyatlari oynattik. Kar marji artti ama riskli.",
                    buyCost: 0, cashPerTurn: 20f, ratingDelta: -0.2f,
                    shortTermBenefit: 0.5f, longTermRisk: 15f,
                    tags: new[] { "price_risk" }),

                Make("RES_R09", "Calisan Kayit Disi", CardType.Risk, CardFamily.Risk,
                    Rarity.Common, SlotType.TempEffect,
                    desc: "Kayit disi eleman. Ucuz iscilik ama denetim riski var.",
                    buyCost: 0, upkeep: 0, staffStabilityDelta: -1f,
                    legalRiskDelta: 6f, shortTermBenefit: 0.3f, longTermRisk: 20f,
                    triggersCrisis: CrisisType.HygieneInspection),

                Make("RES_R10", "Sahte Sertifika", CardType.Risk, CardFamily.Risk,
                    Rarity.Uncommon, SlotType.TempEffect,
                    desc: "Hijyen sertifikasi sahte. Mufattis gelene kadar sorun yok.",
                    buyCost: 10, hygieneDelta: 2f, legalRiskDelta: 12f,
                    shortTermBenefit: 0.6f, longTermRisk: 35f,
                    triggersCrisis: CrisisType.HygieneInspection),
            };
        }

        // ── Reaction ────────────────────────────────────────────────

        static CardData[] CreateReactionCards()
        {
            return new[]
            {
                // ── Existing 2 cards (RES_X01-X02) ──────────────────────

                Make("RES_X01", "Acil Temizlik", CardType.Reaction, CardFamily.Reaction,
                    Rarity.Common, SlotType.TempEffect,
                    desc: "Deterjan, kovalar, paspas. Her yer piril piril olacak.",
                    buyCost: 15, hygieneDelta: 3f,
                    duration: 1, tags: new[] { "hygiene_crisis" }),

                Make("RES_X02", "Ozur Kampanyasi", CardType.Reaction, CardFamily.Reaction,
                    Rarity.Common, SlotType.TempEffect,
                    desc: "Ozur dileriz, bir sonraki yemek bizden. Musteri geri kazanilir.",
                    buyCost: 25, ratingDelta: 0.5f,
                    duration: 1, tags: new[] { "rating_crisis" }),

                // ── New cards (RES_X03-X07) ─────────────────────────────

                Make("RES_X03", "Avukat Tut", CardType.Reaction, CardFamily.Reaction,
                    Rarity.Uncommon, SlotType.TempEffect,
                    desc: "Hukuki surecler baslatildi. Avukat para yer ama legalRisk duser.",
                    buyCost: 50, legalRiskDelta: -20f,
                    duration: 1, tags: new[] { "legal_crisis" }),

                Make("RES_X04", "Tedarikci Degistir", CardType.Reaction, CardFamily.Reaction,
                    Rarity.Common, SlotType.TempEffect,
                    desc: "Bu tedarikciyle olmadi. Yenisini bulduk.",
                    buyCost: 20, qualityDelta: 1f,
                    duration: 1, tags: new[] { "supplier_crisis" }),

                Make("RES_X05", "Menu Sadeles", CardType.Reaction, CardFamily.Reaction,
                    Rarity.Common, SlotType.TempEffect,
                    desc: "Menuyu kisalttik. Az ama oz. Kalite artti, cesitlilik azaldi.",
                    buyCost: 10, qualityDelta: 1f, demandDelta: -1f,
                    duration: 1),

                Make("RES_X06", "Ucretsiz Ikram", CardType.Reaction, CardFamily.Reaction,
                    Rarity.Common, SlotType.TempEffect,
                    desc: "Bugune ozel ikram! Musteri memnun, cep aglar.",
                    buyCost: 15, ratingDelta: 0.5f,
                    duration: 1, tags: new[] { "rating_crisis" }),

                Make("RES_X07", "Denetim Hazirlik", CardType.Reaction, CardFamily.Reaction,
                    Rarity.Uncommon, SlotType.TempEffect,
                    desc: "Mufattis gelecek diye duyulduk. Her sey hazir olmali.",
                    buyCost: 20, legalRiskDelta: -10f, hygieneDelta: 2f,
                    duration: 1, tags: new[] { "hygiene_crisis", "legal_crisis" }),

                Make("RES_X08", "Sigorta Talebi", CardType.Reaction, CardFamily.Reaction,
                    Rarity.Uncommon, SlotType.TempEffect,
                    desc: "Sigortadan hasar bedeli al. Buyuk krizlerde kurtarici.",
                    buyCost: 0, cashPerTurn: 40f,
                    duration: 1, tags: new[] { "financial_crisis" }),

                Make("RES_X09", "Halkla Iliskiler Aciklamasi", CardType.Reaction, CardFamily.Reaction,
                    Rarity.Common, SlotType.TempEffect,
                    desc: "Basin aciklamasi yap, ozur dile, seffaf ol.",
                    buyCost: 10, ratingDelta: 0.3f, legalRiskDelta: -5f,
                    duration: 1, tags: new[] { "rating_crisis", "legal_crisis" }),
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

                Make("crisis_staff_quit", "Sef Istifa", CardType.Crisis, CardFamily.Crisis,
                    Rarity.Uncommon, SlotType.TempEffect,
                    desc: "Bas sef kapıyı capti gitti. Mutfak basta kaldi.",
                    qualityDelta: -2f, staffStabilityDelta: -3f,
                    crisisType: CrisisType.StaffQuit, crisisDuration: 2,
                    solutionTags: new[] { "staff_crisis" }),

                Make("crisis_supply_shortage", "Tedarik Krizi", CardType.Crisis, CardFamily.Crisis,
                    Rarity.Uncommon, SlotType.TempEffect,
                    desc: "Tedarikci iflas etti. Malzeme yok, mutfak bos.",
                    qualityDelta: -1.5f, capacityDelta: -2f,
                    crisisType: CrisisType.SupplyShortage, crisisDuration: 3,
                    solutionTags: new[] { "supplier_crisis" }),

                Make("crisis_rent_increase", "Kira Artti", CardType.Crisis, CardFamily.Crisis,
                    Rarity.Rare, SlotType.TempEffect,
                    desc: "Ev sahibi kirayi ikiye katladi. Ya ode ya tasi.",
                    cashPerTurn: -40f,
                    crisisType: CrisisType.RentIncrease, crisisDuration: 0,
                    solutionTags: new[] { "financial_crisis" }),

                Make("crisis_food_poison", "Gida Zehirlenmesi", CardType.Crisis, CardFamily.Crisis,
                    Rarity.Rare, SlotType.TempEffect,
                    desc: "Musteriler hastanelik oldu. Haber kanallari kapida.",
                    ratingDelta: -1.5f, demandDelta: -3f, legalRiskDelta: 20f,
                    crisisType: CrisisType.FoodPoisoning, crisisDuration: 3,
                    solutionTags: new[] { "hygiene_crisis", "rating_crisis" }),

                Make("crisis_street_work", "Sokak Calismasi", CardType.Crisis, CardFamily.Crisis,
                    Rarity.Common, SlotType.TempEffect,
                    desc: "Belediye sokagi kazdi. Musteri gelemiyor.",
                    demandDelta: -2f, capacityDelta: -1f,
                    crisisType: CrisisType.StreetConstruction, crisisDuration: 2,
                    solutionTags: new[] { "external_crisis" }),
            };
        }

        static CardData[] CreateNeutralCards()
        {
            return new[]
            {
                MakeNeutral("RES_N01", "Acil Nakit", CardType.Reaction, CardFamily.Neutral,
                    Rarity.Common, SlotType.TempEffect,
                    desc: "Kasadan 50 TL cek. Soru sorma.",
                    buyCost: 0, cashPerTurn: 50f,
                    duration: 1),

                MakeNeutral("RES_N02", "Piyasa Arastirmasi", CardType.Marketing, CardFamily.Neutral,
                    Rarity.Common, SlotType.TempEffect,
                    desc: "Rakipleri incele, piyasayi tara. Bilgi guc demektir.",
                    buyCost: 20, demandDelta: 1f, ratingDelta: 0.1f,
                    duration: 2),

                MakeNeutral("RES_N03", "Sezonluk Menu", CardType.Operation, CardFamily.Neutral,
                    Rarity.Uncommon, SlotType.TempEffect,
                    desc: "Mevsime ozel menu. Yaz icecekleri, kis corbalari.",
                    buyCost: 15, qualityDelta: 0.5f, demandDelta: 1.5f,
                    duration: 1, isSeasonal: true),
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

        static CardData MakeNeutral(
            string id, string name, CardType type, CardFamily family,
            Rarity rarity, SlotType targetSlot,
            string desc = null,
            int buyCost = 0, int upkeep = 0,
            float demandDelta = 0f, float capacityDelta = 0f,
            float qualityDelta = 0f, float ratingDelta = 0f,
            float hygieneDelta = 0f, float cashPerTurn = 0f,
            int duration = 0, bool isSeasonal = false, string[] tags = null)
        {
            var card = Make(id, name, type, family, rarity, targetSlot,
                desc: desc, buyCost: buyCost, upkeep: upkeep,
                demandDelta: demandDelta, capacityDelta: capacityDelta,
                qualityDelta: qualityDelta, ratingDelta: ratingDelta,
                hygieneDelta: hygieneDelta, cashPerTurn: cashPerTurn,
                duration: duration, isSeasonal: isSeasonal, tags: tags);
            card.isNeutral = true;
            return card;
        }

        static CardData Make(
            string id, string name, CardType type, CardFamily family,
            Rarity rarity, SlotType targetSlot,
            string desc = null,
            int buyCost = 0, int upkeep = 0,
            float demandDelta = 0f, float capacityDelta = 0f,
            float qualityDelta = 0f, float ratingDelta = 0f,
            float hygieneDelta = 0f, float staffStabilityDelta = 0f,
            float legalRiskDelta = 0f, float cashPerTurn = 0f,
            string subSlot = null,
            StaffTier staffTier = StaffTier.Intern, bool canPromote = false,
            float shortTermBenefit = 0f, float longTermRisk = 0f,
            CrisisType triggersCrisis = CrisisType.None,
            CrisisType crisisType = CrisisType.None,
            int crisisDuration = 0, string[] solutionTags = null,
            int duration = 0, bool isSeasonal = false, string[] tags = null)
        {
            var card = ScriptableObject.CreateInstance<CardData>();
            card.cardId            = id;
            card.cardName          = name;
            card.description       = desc ?? string.Empty;
            card.cardType          = type;
            card.cardFamily        = family;
            card.rarity            = rarity;
            card.sector            = SectorType.Restaurant;
            card.isNeutral         = false;
            card.buyCost           = buyCost;
            card.upkeepPerTurn     = upkeep;
            card.targetSlot        = targetSlot;
            card.targetSubSlot     = subSlot ?? string.Empty;
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
            card.isSeasonal        = isSeasonal;
            card.tags              = tags ?? System.Array.Empty<string>();
            return card;
        }
    }
}
