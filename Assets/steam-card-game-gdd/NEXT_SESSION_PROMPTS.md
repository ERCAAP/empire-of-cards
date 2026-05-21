# Empire of Cards -- Next Session Prompts
## Ne Bitti, Ne Kaldi, Her Role Ozel Prompt

---

## TAMAMLANANLAR (bir onceki session)

- GDD v3.0 tam yazildi (25 tur, 5 venture tipi, 5 slot tipi, 100-musteri market)
- 5 ayri business arastirma dosyasi: fast_food.md, cafe.md, tech_app.md, giyim_magazasi.md, market_bakkal.md
- `GameEnums.cs`: SlotType, SeasonType, LegalRiskLevel, CustomerSegment, RivalMove, VentureType (5 yeni Ingilizce)
- `Constants.cs`: Slot v2, Customer Market weights, Platform Rating, Legal Risk, Season, Cash Flow sabitleri
  - MAX_TURNS=25, SOFT_CAP_TURN=20, HARD_CAP_TURN=25 -- zaten dogru
  - WIN_CUSTOMER_SHARE=60, TOTAL_MARKET_CUSTOMERS=100 -- zaten dogru
- `EventBus.cs`: 15+ yeni event (slot, season, market share, platform rating, cash flow, legal risk)
- `SlotManager.cs`: Yeni 5-slot yoneticisi olusturuldu
- `Board3D.cs`: 3-zone layout (Player / Market / Rival) yeniden insa edildi
- `SlotZone3D.cs`: 5 yeni slot tipi icin CanAccept() eklendi
- `EconomyManager.cs`: PlatformRating, CashFlow sistemi eklendi
- `MarketPool.cs`: CalculatePlayerMarketShare() eklendi
- `WiringService.cs`, `ManagerFactory.cs`: SlotManager entegre edildi
- Tum .cs dosyalarindan Turkce kaldirildi (enum, degisken, comment, UI string)
- HUDBuilder.cs: 4 venture -> 5 venture, Ingilizce metinler
- TierPopup.cs, VentureSelectionUI.cs, AssetGenerator.cs: Ingilizce hale getirildi
- PlayPhase.cs: GetDefaultSlotType() helper eklendi

---

## BILINEN KRITIK SORUNLAR (bu session'da cozulmeli)

| # | Sorun | Etkilenen Dosyalar | Atanan Prompt |
|---|-------|--------------------|---------------|
| 1 | RivalAI.Initialize(VentureType) switch sadece eski enum'lari handle ediyor (Diner, TechStartup, AdAgency, BlackMarket). 5 yeni venture (FastFood, Cafe, TechApp, ClothingStore, GroceryStore) YOK. | RivalAI.cs (satir 117-149) | PROMPT 5 |
| 2 | GameManager'da SlotManager referansi YOK. WiringService slotManager.Init() cagiriyor ama GameManager'a baglamiyor. | GameManager.cs, WiringService.cs | PROMPT 6 |
| 3 | CardData.cs v2 alanlari tamamen eksik (ventureType, targetSlotType, qualityScore, priceScore, platformRatingGain). Eski yapi hala aktif. | CardData.cs | PROMPT 3 |
| 4 | BoardManager vs SlotManager duplikasyon: BoardManager hala aktif (GameManager, ResolvePhase, EconomyManager, ComboSystem, CompanyTierSystem hepsi kullaniyor). SlotManager'a hicbir manager bagli degil. | BoardManager.cs, SlotManager.cs | PROMPT 6 |
| 5 | CompanyTierSystem.EvaluateTier(int playerTerritories) hala territory-based. Customer share'e cevrilmeli. | CompanyTierSystem.cs | PROMPT 4 |
| 6 | ResolveStep enum'da SeasonCheck ve MarketShareCalculation adimlari eksik. | GameEnums.cs (satir 70-79), ResolvePhase.cs | PROMPT 4 |
| 7 | GameScene.unity muhtemelen eski duplikat. Sahneler: Boot, Game, GameScene, MainMenu. | Assets/Scenes/ | PROMPT 7 |
| 8 | BalanceDefs.cs maxTurns=30 (yanlis, Constants.cs'de 25). startingBusinessSlots=3 (eski), winTerritories/loseTerritories hala territory-based. | BalanceDefs.cs | PROMPT 3 |
| 9 | GameManager.EndCurrentTurn() hala territory-based win/lose check yapiyor. | GameManager.cs (satir 252-278) | PROMPT 6 |
| 10 | WinLoseChecker.cs tamamen territory parametreleri aliyor. Customer-share'e cevrilmeli. | WinLoseChecker.cs | PROMPT 6 |
| 11 | BalanceDefs.CreateRival() ventureMatchedNames/Income/Customers dizileri eski 4 venture'a gore. | BalanceDefs.cs (satir 166-169) | PROMPT 5 |

---

## YAPILACAKLAR LISTESI

### KATMAN 1 -- Tasarim (once bunlar)
- [ ] **Game Designer**: 5 venture icin kart listesi -- her venture 15-20 kart, slot tipleri eslestirilmis
- [ ] **Economy Designer**: Her kartin income/cost/customer degerleri -- balance tablosu
- [ ] **Narrative Designer**: Kart flavor text, rival diyaloglari, event aciklamalari (Ingilizce)

### KATMAN 2 -- Kod (tasarim bittikten sonra)
- [ ] **Senior Dev 2**: `CardData.cs` yeni alanlar ekle (ventureType, slotType, qualityScore, priceScore, platformRatingEffect, legalRiskEffect) -- ESKi alanlari KALDIRMA, yenileri ekle
- [ ] **Senior Dev 2**: `BalanceDefs.cs` maxTurns=25, customer-based win condition, 5 yeni venture mirror data + `AssetGenerator.cs` tam yeniden yazim -- 5 venture x 15-20 kart = ~80 venture-specific + 20 genel = 100 kart
- [ ] **Senior Dev 1**: `ResolvePhase.cs` guncelle -- SeasonCheck + MarketShareCalculation adimlari ekle, SlotManager cagirilari, platform rating decay, season efektleri, market share hesabi, legal risk biriktirme
- [ ] **Senior Dev 1**: `CompanyTierSystem.cs` -- territory-based'den customer share-based'e cevir (20/45/60 musteri esikleri), SlotManager.TryExpandSlot() cagir
- [ ] **Senior Dev 1**: `GameEnums.cs` ResolveStep enum'a SeasonCheck ve MarketShareCalculation ekle
- [ ] **Senior Dev 3**: `RivalAI.cs` -- 5 yeni VentureType case'i ekle, eski 4'u kaldir, venture mirror sistemi, yeni slot sistemiyle rivala Operation/Staff slot sayisi atanmasi
- [ ] **Senior Dev 2**: `VentureSelectionUI.cs` -> GameManager.StartNewRun(ventureType) baglantisi
- [ ] **Lead Dev**: `GameManager.cs` SlotManager field/property ekle, EndCurrentTurn() win/lose check'i territory'den customer share'e cevir
- [ ] **Lead Dev**: `WiringService.cs` GameManager'a SlotManager referansi ver
- [ ] **Lead Dev**: `WinLoseChecker.cs` CheckWin/CheckLose parametrelerini customer share'e cevir
- [ ] **Lead Dev**: BoardManager -> SlotManager gecis plani: hangi methodlar devredilecek, hangileri kalacak? ResolvePhase ve EconomyManager SlotManager'i da kullanmali
- [ ] **Senior Dev 3**: `BalanceDefs.CreateRival()` ventureMatchedNames/Income/Customers dizilerini 5 yeni venture'a guncelle

### KATMAN 3 -- Sahne & Level Design
- [ ] **Level Designer**: 4 sahne ZATEN VAR (Boot, Game, GameScene, MainMenu). GameScene muhtemelen eski -- Game ile birlestir veya kaldir. Yeni sahne OLUSTURMA.
- [ ] **Level Designer**: GameSceneBootstrap'i sahneye bagla, kamera acisi, lighting, HUD pozisyonlari
- [ ] **Level Designer**: Board'u sahneye yerlestir, hand anchor pozisyonu, platform rating HUD gostergesi ekle

### KATMAN 4 -- QA
- [ ] **QA**: Turn flow testi (event->draw->play->resolve->rival, 25 tur)
- [ ] **QA**: Her venture tipiyle oynanabilirlik testi
- [ ] **QA**: Market share hesabi dogrulama
- [ ] **QA**: RivalAI 5 yeni venture ile calisma testi
- [ ] **QA**: Win/lose condition musteri payina gore calisiyor mu?
- [ ] **QA**: BoardManager->SlotManager gecisi sonrasi regresyon testi

---

---

# PROMPTS -- YENI SOHBETTE KULLAN

Her promptu ayri bir agent/sohbet olarak kullan. Once 1'i, sonra 2'yi calistir.

---

## PROMPT 0 -- MASTER CONTEXT (Her yeni sohbetin basina yapistir)

```
Sen Empire of Cards oyununun bir ekip uyesisin.

PROJE DURUMU:
- Unity 6 kart oyunu, PC/Steam hedef, solo gelistirici
- Architecture: Bootstrap -> WiringService -> GameManager -> StateMachine + TurnManager
- EventBus (static): tek inter-manager iletisim yolu
- 5 Turn phase: Event -> Draw -> Play -> Resolve -> Rival
- 25 tur, 5 sezon (5'er tur), kazanma: 60/100 musteri payi
- 5 Venture tipi: FastFood, Cafe, TechApp, ClothingStore, GroceryStore
- 5 Slot tipi: Operation, Staff, Marketing, Supplier, TempEffect
- 3-zone board: Player Zone (alt) / Customer Market Zone (orta) / Rival Zone (ust)
- Rival her zaman oyuncunun sectigi venture tipini mirror eder
- 4 Unity sahne MEVCUT: Boot.unity, Game.unity, GameScene.unity (eski/duplikat), MainMenu.unity
  - Sahneler: Assets/Scenes/ dizininde (NOT: _EmpireOfCards icinde degil)
  - GameOver ayri sahne DEGIL -- GameState.GameOver olarak handle ediliyor
- Legacy enum'lar hala var (Diner, TechStartup, AdAgency, BlackMarket) -- save uyumlulugu icin tutuyoruz ama yeni kodda KULLANMA

KOD KURALLARI -- MUTLAK:
- Tum C# kodu Ingilizce: enum degerleri, degisken adlari, method adlari, commentler
- Turkce SADECE GDD markdown dosyalarinda ve LocalizationManager key'lerinde
- EventBus pattern: OnEnable'da subscribe, OnDisable'da unsubscribe
- Init() pattern: her manager WiringService tarafindan Init() ile wire edilir
- GameObject.Find() ve FindObjectOfType() YASAK
- Yeni manager = Init() method + WiringService registration + GameManager.Init() parameter

PROJE DIZINI: /Users/omerercan/Documents/card-games-buisnes
GDD: Assets/steam-card-game-gdd/GDD.md
BUSINESS RESEARCH: Assets/steam-card-game-gdd/businesses/
SCRIPTS: Assets/_EmpireOfCards/Scripts/
```

---

## PROMPT 1 -- GAME DESIGNER (Kart Listesi Tasarimi)

```
Sen Empire of Cards oyununun Game Designer'isin.

[PROMPT 0'I BURAYA YAPISTIR]

GOREVIN: 5 venture tipi icin OYNANABILIR kart listesi tasarla.

Her venture icin gereken kart sayisi:
- 4 Operation karti (Business tipi -- slot'a yerlesince income+customer uretir)
- 4 Staff karti (Employee tipi -- operations'a atanir, bonus verir)
- 3 Marketing karti (Action/Upgrade tipi -- MarketingSlot'a gider, platform rating artirir)
- 2 Supplier karti (Upgrade tipi -- SupplierSlot'a gider, maliyet dusurur/kalite artirir)
- 2 Event/Crisis karti (TempEffect slot -- gecici negatif etki)
= venture basina ~15 kart -> 5 venture x 15 = 75 venture-specific kart

Ayrica 20 genel (neutral) kart:
- 5 genel Action karti
- 5 genel Upgrade karti  
- 5 genel Staff karti
- 5 Event karti (genel krizler)

Her kart icin sunu belirt:
KART ADI | TIP | SLOT | RARITY | VentureType | OZELLIK (tek cumle)
KART ADI | TIP | SLOT | RARITY | VentureType | COST | INCOME/TURN | CUSTOMERS/TURN

KURALLAR:
- Kart isimleri Ingilizce, kisa, akilda kalici (max 3 kelime)
- Her venture'in guclu yonu, zayif yonu ve ozel mekanigi karta yansisin
- FastFood: hiz-kalite dengesi, teslimat komisyonu riski
- Cafe: loyalty sistemi, barista SCA egitimi, espresso machine core
- TechApp: platform fee, viral buyume, delayed income
- ClothingStore: sezon gecisleri, vitrin refresh, trend takibi
- GroceryStore: spoilage (bozulma) riski, veresiye sistemi, zincir rekabeti
- Combolar: ayni venture kartlari bir arada ek bonus verir

Ciktiyi bir markdown tablosu olarak ver.
Sonra her venture icin 2 adet COMBO tarif et (ornek: "Espresso Machine + Barista SCA = +30% income").
```

---

## PROMPT 2 -- ECONOMY DESIGNER (Balance Tablosu)

```
Sen Empire of Cards oyununun Economy Designer'isin.

[PROMPT 0'I BURAYA YAPISTIR]

MEVCUT EKONOMI SISTEMI:
- Net Income = toplam(operation.income x multipliers) + combo_bonuses - toplam(staff.salary) - tax(15%) - legalRiskPenalty
- Platform Rating 1.0-5.0 -> musteri carpani (4.5+=+25%, 4.0+=+15%, 3.5+=+5%, alti=-10%)
- 100 musteri havuzu, agirlikli formul: Quality 30%, Price 20%, PlatformRating 20%, Marketing 15%, Speed 10%, Loyalty 5%
- Kazanma: 60 musteri (60%) -- 25 turda

GOREVIN:
Game Designer'in tasarladigi kart listesini al, her kart icin ekonomik degerler belirle:

1. OPERATION KARTLARI icin:
   - buyCost: 100-500 arasi (erken game=ucuz, gec game=pahali)
   - incomePerTurn: venture tipine gore gercekci marj (FastFood net marj 3-15%, Cafe 10-25%, Tech 20-60%)
   - customersPerTurn: 1-8 arasi
   - qualityScore: 1-10 (market share hesabinda kullanilir)
   - priceScore: 1-10 (dusuk fiyat = yuksek skor)
   - activationDelay: TechApp icin 2-3 tur

2. STAFF KARTLARI icin:
   - salaryPerTurn: 20-80 arasi
   - customerBonus: 1-4
   - synergyBonus: 4-8 (kendi venture tipinde calisinca)

3. MARKETING KARTLARI icin:
   - platformRatingGain: 0.2-0.8
   - marketingScore katkisi: 1-5
   - buyCost: 80-200

4. SUPPLIER KARTLARI icin:
   - qualityScore katkisi: 1-3
   - maliyet azaltma: 10-30%
   - buyCost: 150-300

5. 25 TUR PROGRESSION EGRISI:
   - Tur 1-5: Oyuncu 300-600 arasi net income/tur
   - Tur 6-10: 600-1200
   - Tur 11-15: 1200-2000
   - Tur 16-20: 2000-3500
   - Tur 21-25: 3500-6000
   
   Bu egriyi saglayan bir starting deck + shop curve oner.

Cikti: Her kart icin sayisal deger tablosu + "bu degerler neden mantikli" kisa aciklama.
```

---

## PROMPT 3 -- SENIOR DEV 2 (CardData Yeniden Yaz + BalanceDefs)

```
Sen Empire of Cards'in Senior Game Developer #2'sisin -- Economy Code.

[PROMPT 0'I BURAYA YAPISTIR]

MEVCUT DOSYALAR (oku once):
- Assets/_EmpireOfCards/Scripts/Data/CardData.cs
- Assets/_EmpireOfCards/Scripts/Bootstrap/Data/BalanceDefs.cs
- Assets/_EmpireOfCards/Scripts/Editor/AssetGenerator.cs
- Assets/_EmpireOfCards/Scripts/Core/Enums/GameEnums.cs (SlotType, VentureType enums mevcut)
- Assets/_EmpireOfCards/Scripts/Core/Constants.cs (tum yeni sabitler mevcut)
- Assets/_EmpireOfCards/Scripts/Data/GameBalanceData.cs

MEVCUT DURUM -- CardData.cs:
CardData.cs su anda ESKI yapida. Business/Employee/Action/Upgrade/Event header'lari altinda
eski alanlar var (incomePerTurn, employeeSlots, hasTrendBonus, trendIncomeMultiplier,
randomIncome*, foodBonus*, evolvedForm, vs.). Yeni v2 alanlari (ventureType, targetSlotType,
qualityScore, priceScore, platformRatingGain, legalRiskPerTurn, costReductionPercent, vb.)
henuz YOK. Eski alanlari KALDIRMA -- derleme kirilir. Yeni alanlari EKLE.

MEVCUT DURUM -- BalanceDefs.cs:
BalanceDefs.CreateBalance() icinde yanlis degerler var:
- maxTurns = 30 (YANLIS -- Constants.cs'de 25, burasi da 25 olmali)
- startingBusinessSlots = 3 (ESKI -- yeni sistem STARTING_OPERATION_SLOTS=4)
- maxBusinessSlots = 5 (ESKI -- yeni sistem MAX_OPERATION_SLOTS=8)
- winTerritories = 6 ve loseTerritories = 7 (ESKI -- customer share'e cevrilmeli)
- gb.totalTerritories = 10 (ESKI -- TOTAL_MARKET_CUSTOMERS=100 olmali)
CreateRival() icinde ventureMatchedNames/Income/Customers dizileri eski 4 venture icin
(Diner, TechStartup, AdAgency, BlackMarket). 5 yeni venture icin guncellenmeli.

GOREV 1 -- CardData.cs'e yeni alanlar ekle:
```csharp
// Venture & Slot System v2
public VentureType ventureType;        // Hangi venture'a ait (General = tum)
public SlotType targetSlotType;        // Hangi slota gidiyor
public bool isGeneralCard;             // true = tum venture'larda cikabilir

// Market Share katkisi (Operation kartlari icin)
public float qualityScore;             // 0-10, market share kalite agirligi
public float priceScore;               // 0-10, dusuk fiyat = yuksek skor
public float serviceSpeedScore;        // 0-10

// Platform Rating efekti (Marketing kartlari icin)
public float platformRatingGain;       // MarketingSlot'a koyunca her tur kazanilir
public float platformRatingOnPlay;     // Aninda etki (tek seferlik)

// Legal Risk efekti
public int legalRiskPerTurn;           // Her tur biriken risk (negatif = azalir)
public int legalRiskOnPlay;            // Kart oynaninca anlik etki

// Supplier efektleri
public float costReductionPercent;     // SupplierSlot'tayken tum operation kartlarina
public float qualityBoostAmount;       // qualityScore'a eklenir
```

GOREV 2 -- BalanceDefs.cs guncelle:
- maxTurns: 30 -> 25
- startingBusinessSlots: 3 -> 4 (STARTING_OPERATION_SLOTS)
- maxBusinessSlots: 5 -> 8 (MAX_OPERATION_SLOTS)
- totalTerritories: 10 -> TOTAL_MARKET_CUSTOMERS = 100
- winTerritories: 6 -> WIN_CUSTOMER_SHARE = 60
- loseTerritories: 7 -> 60 (rival da 60 musteri alirsa kaybedersin)
- Market pool: 100 total customers (zaten 60 base + growth, degistirme)
- CreateRival() icinde ventureMatchedNames/Income/Customers dizilerini 5 yeni venture icin guncelle:
  - FastFood: "Rival Fast Food", income=45, customers=4
  - Cafe: "Rival Cafe", income=55, customers=3
  - TechApp: "Rival Tech App", income=0 (delayed), customers=0
  - ClothingStore: "Rival Clothing Store", income=50, customers=3
  - GroceryStore: "Rival Grocery Store", income=40, customers=5

GOREV 3 -- AssetGenerator.cs tam yeniden yaz:
Economy Designer'in balance tablosunu kullanarak 5 venture x ~15 kart = 75 kart + 20 genel kart uret.
Her kart CreateBusiness/CreateEmployee/CreateUpgrade helper methodlariyla olusturulsun.
Kart ID formati: "FFC01_Grill" (FastFood Card 01), "CAF02_Barista", "TEC03_ServerFarm", vb.

Tum kod Ingilizce. Turkce yok.
```

---

## PROMPT 4 -- SENIOR DEV 1 (ResolvePhase + Season + CompanyTier + SlotManager Entegrasyonu)

```
Sen Empire of Cards'in Senior Game Developer #1'isin -- Core Gameplay.

[PROMPT 0'I BURAYA YAPISTIR]

MEVCUT DOSYALAR (oku once):
- Assets/_EmpireOfCards/Scripts/Core/TurnPhases/ResolvePhase.cs
- Assets/_EmpireOfCards/Scripts/Gameplay/SlotManager.cs (yeni, slot sistemi)
- Assets/_EmpireOfCards/Scripts/Gameplay/EconomyManager.cs (PlatformRating, CashFlow mevcut)
- Assets/_EmpireOfCards/Scripts/Gameplay/CompanyTierSystem.cs
- Assets/_EmpireOfCards/Scripts/Core/Constants.cs (tum yeni sabitler mevcut)
- Assets/_EmpireOfCards/Scripts/Core/EventBus.cs (yeni eventler mevcut)
- Assets/_EmpireOfCards/Scripts/Core/Enums/GameEnums.cs (ResolveStep enum burada)
- Assets/_EmpireOfCards/Scripts/Gameplay/BoardManager.cs (ESKI sistem, hala aktif)

MEVCUT DURUM -- ResolvePhase.cs:
ResolveStep enum (GameEnums.cs satir 70-79):
BusinessProduce -> CustomerFlow -> ComboCheck -> TierCheck -> IncomeCalculation -> DeteriorationCheck
EKSIK adimlar: SeasonCheck (CustomerFlow sonrasi) ve MarketShareCalculation (ayri adim).
ResolvePhase sinifindan SlotManager'a hicbir referans yok -- hep BoardManager kullaniyor.

MEVCUT DURUM -- CompanyTierSystem.cs:
EvaluateTier(int playerTerritories) parametresi territory-based.
Init(BoardManager board, ComboSystem combo) -- BoardManager'a bagli.
CalculateTier() icinde Constants.TIER_*_TERRITORIES kullaniliyor.

GOREV 1 -- GameEnums.cs ResolveStep enum'u guncelle:
```csharp
public enum ResolveStep
{
    BusinessProduce,        // 4a: Businesses produce
    CustomerFlow,           // 4b: Customers flow
    SeasonCheck,            // 4b.5: Season change check + multiplier
    MarketShareCalculation, // 4b.6: Market share hesabi (quality, price, rating, marketing, speed, loyalty)
    ComboCheck,             // 4c: Combo check
    TierCheck,              // 4c.5: Company tier evaluation
    IncomeCalculation,      // 4d: Income calculated
    DeteriorationCheck      // 4e: Deterioration check (legal risk, closure, leaving, platform decay)
}
```

GOREV 2 -- ResolvePhase.cs guncelle:

SeasonCheck (CustomerFlow SONRASINA ekle):
- GameManager.CurrentTurn / Constants.TURNS_PER_SEASON -> hangi SeasonType hesapla
- SeasonType degismisse EventBus.SeasonChanged() cagir
- Season multiplier'i hesapla -> EconomyManager.currentSeasonMultiplier'a ata

MarketShareCalculation (SeasonCheck SONRASINA ekle):
- SlotManager'dan active Operation kartlarinin qualityScore, priceScore, serviceSpeedScore ortalamasi al
- EconomyManager.PlatformRating al
- MarketPool.CalculatePlayerMarketShare(quality, price, platformRating, marketingScore, speed, loyalty) cagir
- Sonucu 100 ile carp -> player customer count
- EventBus.MarketShareUpdated(playerCount, rivalCount) cagir

DeteriorationCheck guncelle:
- Legal Risk: FBISystem.AccumulateRiskFromBoard() cagrisini koru
- Ek: Marketing slotlari doluysa EconomyManager.platformRating decay'i ATLA (mevcut kart korur)
- Marketing slotu bossa EconomyManager.DecayPlatformRating() cagir

GOREV 3 -- CompanyTierSystem.cs guncelle:
- EvaluateTier() parametresini int playerTerritories -> int playerCustomers olarak degistir
- Tier 1->2: 20+ musteri (Constants'a yeni sabit ekle: TIER_ENTREPRENEUR_CUSTOMERS=20)
- Tier 2->3: 45+ musteri + 1+ active combo (TIER_CORPORATION_CUSTOMERS=45)
- Tier 3->4: 60+ musteri + 2+ active combo + Operation slot max doluluk 75% (TIER_CONGLOMERATE_CUSTOMERS=60)
- Constants.cs'deki eski TIER_*_TERRITORIES sabitlerini TIER_*_CUSTOMERS ile degistir
- Tier atlama aninda SlotManager.TryExpandSlot() cagir (GDD 4.5 slot expansion)
- Init() parametresine SlotManager ekle: Init(BoardManager board, ComboSystem combo, SlotManager slots)

Tum kod Ingilizce.
```

---

## PROMPT 5 -- SENIOR DEV 3 (RivalAI Venture Mirror Sistemi)

```
Sen Empire of Cards'in Senior Game Developer #3'usun -- World & AI Systems.

[PROMPT 0'I BURAYA YAPISTIR]

MEVCUT DOSYALAR (oku once):
- Assets/_EmpireOfCards/Scripts/Gameplay/RivalAI.cs
- Assets/_EmpireOfCards/Scripts/Gameplay/Rival/RivalDecisionTree.cs
- Assets/_EmpireOfCards/Scripts/Gameplay/Rival/RivalEconomy.cs
- Assets/_EmpireOfCards/Scripts/Gameplay/Rival/RivalGrowth.cs
- Assets/_EmpireOfCards/Scripts/Data/RivalData.cs
- Assets/_EmpireOfCards/Scripts/Core/Enums/GameEnums.cs (RivalMove enum mevcut)
- Assets/_EmpireOfCards/Scripts/Bootstrap/Data/BalanceDefs.cs (CreateRival() burada)

MEVCUT DURUM -- RivalAI.cs:
KRITIK BUG: Initialize(VentureType playerVenture) methodu (satir 117-149) sadece eski
enum degerlerini handle ediyor: Diner, TechStartup, AdAgency, BlackMarket.
Yeni 5 venture (FastFood, Cafe, TechApp, ClothingStore, GroceryStore) switch'te YOK.
Player yeni venture secerse rival default'a dusuyor ve yanlis isim/income/customer aliyor.

MEVCUT DURUM -- BalanceDefs.cs CreateRival():
ventureMatchedNames, ventureMatchedIncome, ventureMatchedCustomers dizileri
eski 4 venture'a gore indeksli (satir 166-169). 5 yeni venture icin guncellenmeli.

GOREV 1 -- RivalAI.Initialize(VentureType) switch statement'ini YENIDEN YAZ:
- 5 yeni VentureType case'i EKLE: FastFood, Cafe, TechApp, ClothingStore, GroceryStore
- Legacy enum'lari (Diner, TechStartup, AdAgency, BlackMarket) KALDIR (switch'ten)
- Her venture icin ilk rival business bilgileri:
  - FastFood: name="Rival Fast Food", income=45, customers=4
  - Cafe: name="Rival Cafe", income=55, customers=3
  - TechApp: name="Rival Tech App", income=0, customers=0 (delayed start)
  - ClothingStore: name="Rival Clothing Store", income=50, customers=3
  - GroceryStore: name="Rival Grocery Store", income=40, customers=5

GOREV 2 -- Rival Venture Mirror Sistemi:

RivalAI.Init() metoduna ventureType parametresi ekle:
```csharp
public void Init(RivalData data, VentureType playerVenture)
```

RivalAI'in kendi venture tipi = playerVenture (GDD: rival her zaman ayni sektorde rakip olur).

RivalBusiness class'ina ekle:
- VentureType ventureType
- float qualityScore (1-10)
- float platformRating (1.0-5.0, starts 2.5)
- int legalRisk (0-100)
- SlotType[] activeSlots

GOREV 3 -- RivalDecisionTree.cs guncelle:

Mevcut RivalMove enum: PriceWar, MarketingBlitz, QualityImprove, StaffPoach, SeekInvestment, OpenBranch, Sabotage

Her move icin etki:
- PriceWar: rival.priceScore += 2 (bu tur), musteri ceker price-sensitive segmenti
- MarketingBlitz: rival.platformRating += 0.4f
- QualityImprove: rival.qualityScore += 1.5f
- StaffPoach: player'in Staff slotundan bir kartin customerBonus'unu cal (1 tur)
- SeekInvestment: rival.rivalIncome += rivalIncome * 0.3f (bu tur)
- OpenBranch: rival.maxOperationSlots++ (max 6)
- Sabotage: player'in en yuksek income'lu Operation slotunu 1 tur devre disi birak, legalRisk += 15

Tier 1: 2 move/tur | Tier 2: 3 move/tur | Tier 3: 4 move/tur (GDD)

GOREV 4 -- RivalEconomy.cs guncelle:
Rival income = qualityScore * customers * ventureMultiplier
Rival musteri = (qualityScore * 0.30 + priceScore * 0.20 + platformRating_normalized * 0.20 + ...) * 100
(ayni CalculatePlayerMarketShare formulu, rival icin mirror)

GOREV 5 -- BalanceDefs.CreateRival() guncelle:
ventureMatchedNames, ventureMatchedIncome, ventureMatchedCustomers dizilerini
5 yeni venture'a gore yeniden yaz. Eski 4 venture verisini kaldir.

Tum kod Ingilizce.
```

---

## PROMPT 6 -- LEAD DEVELOPER (Architecture Review + SlotManager Entegrasyonu + Win Condition)

```
Sen Empire of Cards'in Lead Developer'isin -- Architecture otoritesi.

[PROMPT 0'I BURAYA YAPISTIR]

MEVCUT DOSYALAR (oku once):
- Assets/_EmpireOfCards/Scripts/Bootstrap/WiringService.cs
- Assets/_EmpireOfCards/Scripts/Bootstrap/ManagerFactory.cs
- Assets/_EmpireOfCards/Scripts/Bootstrap/GameSceneBootstrap.cs (yoksa olustur)
- Assets/_EmpireOfCards/Scripts/Core/GameManager.cs
- Assets/_EmpireOfCards/Scripts/Core/WinLoseChecker.cs
- Assets/_EmpireOfCards/Scripts/UI/VentureSelectionUI.cs
- Assets/_EmpireOfCards/Scripts/Gameplay/SlotManager.cs
- Assets/_EmpireOfCards/Scripts/Gameplay/BoardManager.cs

MEVCUT DURUM -- GameManager.cs:
Satir 41-55: boardManager, comboSystem, territoryManager, fbiSystem, rivalAI, shopManager, vb.
referanslari var ama SlotManager referansi YOK.
Satir 252-278: EndCurrentTurn() hala territory-based win/lose check yapiyor:
  - WinLoseChecker.CheckWin(playerTerritories, winReq) -- playerTerritories kullaniyor
  - WinLoseChecker.CheckLose(rivalTerritories, loseReq, resources.Money) -- rivalTerritories kullaniyor
  - Tur limiti asiminda: EndRun(playerTerritories >= rivalTerritories) -- territory karsilastirmasi

MEVCUT DURUM -- WinLoseChecker.cs:
Tamamen territory parametreleri aliyor:
  - CheckWin(int playerTerritories, int winRequirement)
  - CheckLose(int rivalTerritories, int loseRequirement, int playerMoney)
Customer share'e cevrilmeli.

MEVCUT DURUM -- WiringService.cs:
Satir 80-81: m.slotManager.Init() cagrilmis ama GameManager'a verilmemis.
BoardManager hala her yerde kullaniliyor (CompanyTierSystem, ShopManager, vb. referans aliyor).
SlotManager'a hicbir baska manager bagli degil.

GOREV 1 -- GameManager'a SlotManager referansi ekle:
```csharp
[SerializeField] private SlotManager slotManager;
public SlotManager SlotManager => slotManager;
```
WiringService'de GameManager.SetSlotManager(m.slotManager) cagir (ya da Init parametresine ekle).

GOREV 2 -- GameManager.EndCurrentTurn() win/lose check'i YENIDEN YAZ:
```csharp
// ESKI (kaldir):
// if (WinLoseChecker.CheckWin(playerTerritories, winReq)) ...
// if (WinLoseChecker.CheckLose(rivalTerritories, loseReq, resources.Money)) ...

// YENI:
int winReq = Constants.WIN_CUSTOMER_SHARE; // 60
if (WinLoseChecker.CheckWin(playerCustomerShare, winReq)) { EndRun(true); return; }
if (WinLoseChecker.CheckLose(rivalCustomerShare, winReq, resources.Money)) { EndRun(false); return; }
if (currentTurn >= MaxTurns) { EndRun(playerCustomerShare >= rivalCustomerShare); return; }
```

GOREV 3 -- WinLoseChecker.cs customer share'e cevir:
```csharp
public static bool CheckWin(int playerCustomers, int winRequirement)
{
    return playerCustomers >= winRequirement;
}
public static bool CheckLose(int rivalCustomers, int loseRequirement, int playerMoney)
{
    if (rivalCustomers >= loseRequirement) return true;
    if (playerMoney <= 0) return true;
    return false;
}
```

GOREV 4 -- VentureSelection -> GameManager baglantisi:
VentureSelectionUI'da venture secilince:
1. GameManager.SetVentureType(VentureType chosen) cagir
2. WiringService'de RivalAI.Init(rivalData, chosenVenture) yeniden cagir
3. ShopManager'a bias ver: ilk 5 tur secilen venture tipindeki kartlar %70 olasilikla cikar

WiringService.WireAll() imzasini degistirme -- bunun yerine GameManager'a:
```csharp
public void StartNewRun(VentureType chosenVenture)
```
ekle ve icinde RivalAI + ShopManager'i venture'a gore initialize et.

GOREV 5 -- BoardManager -> SlotManager gecis plani:
BoardManager.cs (eski system: maxSlots=3, business-only) hala aktif:
- GameManager.Init() BoardManager aliyor
- WiringService BoardManager wire ediyor
- ResolvePhase, EconomyManager, ComboSystem, CompanyTierSystem hepsi BoardManager kullaniyor
SlotManager.cs yeni 5-slot sistemi -- ama hicbir manager ona bagli degil.

SEN KARAR VER:
A) BoardManager tamamen kaldirilsin, SlotManager devraisin -- tum referanslar guncellenir
B) BoardManager wrapper olarak kalsin, icerde SlotManager'a delegate etsin
C) Ikisi paralel calisson -- eskiyi yeni sisteme yavas gecis (ornegin: ResolvePhase once
   SlotManager'dan Operation kartlarini alsin, yoksa BoardManager'a fallback)

Hangi metholar devredilecek, hangileri kalacak? Acil: ResolvePhase ve EconomyManager
SlotManager'i da kullanmali. Karar ver ve IMPLEMENT et.

GOREV 6 -- Architecture Audit:
Su siraya gore tum Init() cagrilarinin dogru sirada oldugunu kontrol et:
Data factories -> Manager creation -> WiringService.WireAll() -> GameManager.StartNewRun()

Eger circular dependency varsa raporla ve coz.

Tum kod Ingilizce. Yeni abstraction ekleme -- sadece gerekli olan.
```

---

## PROMPT 7 -- LEVEL DESIGNER (Unity Sahne Duzenlemesi)

```
Sen Empire of Cards'in Level Designer'isin.

[PROMPT 0'I BURAYA YAPISTIR]

MEVCUT DOSYALAR (oku once):
- Assets/_EmpireOfCards/Scripts/World/Board3D.cs (3-zone layout hazir)
- Assets/_EmpireOfCards/Scripts/Bootstrap/GameSceneBootstrap.cs
- Assets/_EmpireOfCards/Scripts/Bootstrap/BootSceneController.cs
- Assets/_EmpireOfCards/Scripts/Bootstrap/MainMenuController.cs

MEVCUT DURUM -- Sahneler:
4 sahne ZATEN VAR: Assets/Scenes/ dizininde:
- Boot.unity -- acilis sahnesi
- MainMenu.unity -- ana menu
- Game.unity -- oyun sahnesi
- GameScene.unity -- muhtemelen ESKI DUPLIKAT

GameOver ayri sahne DEGIL -- GameState.GameOver olarak GameManager icinde handle ediliyor.
Yeni sahne OLUSTURMA. Mevcut sahneleri DUZENLE.

GOREV 1 -- Sahne Temizligi:
- GameScene.unity'yi kontrol et. Game.unity ile ayni ise KALDIR (ya da bos birak, Unity Editor'de silinir).
- Sahne akisi: Boot -> MainMenu -> Game (GameOver da Game icinde)
- BootSceneController.cs: SlotManager, SaveManager yukle, MainMenu'ye gec
- MainMenuController.cs: VentureSelectionUI goster

GOREV 2 -- Game Sahnesinde Board Kurulumu:
GameSceneBootstrap.cs icinde:
1. Camera pozisyonu: position (0, 8, -8), rotation (40, 0, 0) -- masayi yukaridan gorsun
2. Board3D.BuildBoard() cagir
3. Hand3D anchor: kameranin alt kisminda, board'un onunde (0, 0.5f, -7f)
4. Lighting: Directional light, warm color, slight shadow

GOREV 3 -- HUD Guncellemeleri (HUDBuilder.cs'e eklenecekler):
- Platform Rating gostergesi: 1-5 arasi yildiz veya bar (TopBar'da)
- Legal Risk gostergesi: 0-100 progress bar, renk: yesil->sari->kirmizi
- Season gostergesi: hangi sezon, kacinci tur
- Customer Market bar: player mavisi vs rival kirmizisi (100 musteri paylasimi)

Placeholder: cube'lar ve TMP text yeterli, gercek asset yok.
Tum kod Ingilizce.
```

---

## PROMPT 8 -- NARRATIVE DESIGNER (Kart Metinleri + Rival Diyaloglari)

```
Sen Empire of Cards'in Narrative Designer'isin.

[PROMPT 0'I BURAYA YAPISTIR]

OYUN TONU: Gritty business simulation + dark humor. Gercekci is dunyasi acimasizligi, ama hafif ironik.
Ornek ton: "Your espresso machine breaks on opening day. Classic."

GOREV 1 -- Her venture icin 15 kartin flavor text'ini yaz (Ingilizce, max 2 satir):
Format: KART ADI | FLAVOR TEXT

FastFood ornekler:
- Fry Station | "First in. Last out. Always greasy."
- Delivery Driver | "Three orders. Two bags. One is wrong. Every time."

Cafe ornekler:
- Espresso Machine | "The heart of the operation. Also the most expensive thing you own."
- SCA Certified Barista | "She can do latte art. She knows it."

TechApp ornekler:
- App Store Listing | "4.2 stars. Someone gave 1 star because the icon is blue."
- Push Notification | "Please don't uninstall. We're begging."

ClothingStore ornekler:
- Storefront Display | "Changed it. Customer said the old one was better."
- Seasonal Inventory | "Summer collection arrives in November. Retail logic."

GroceryStore ornekler:
- Fresh Produce Shelf | "70% fresh. 30% an act of faith."
- Neighborhood Reputation | "They've been coming here since before you were born. Don't mess it up."

GOREV 2 -- Rival Diyaloglari (25 tur icin, tier'a gore):
Tier 1 rival (kucuk rakip): 5 taunt
Tier 2 rival (buyuyen rakip): 5 taunt  
Tier 3+ rival (agir rakip): 5 taunt + 2 strategy comment

GOREV 3 -- Event Karti Aciklamalari (10 event icin):
Format: EVENT ADI | ACIKLAMA (1-2 cumle, ne oldugu ve etkisi)
Ornek: "Health Inspector Visit | They find the freezer. Operation cards -20% income this turn."
```

---

## PROMPT 9 -- QA TESTER (Oyun Akisi Testi Plani)

```
Sen Empire of Cards'in QA Tester'isin.

[PROMPT 0'I BURAYA YAPISTIR]

MEVCUT TEST EDILECEK SISTEMLER:
- 5 venture tipi baslangic secimi
- 5 slot tipi (Operation, Staff, Marketing, Supplier, TempEffect) kart yerlestirme
- 25 tur akisi, 5 sezon
- Market share hesabi (100 musteri, 6-faktorlu agirlik)
- Platform Rating (1.0-5.0, decay, marketing ile koruma)
- Legal Risk (0-100, FBISystem entegrasyonu)
- Rival AI (venture mirror, tier'a gore move sayisi)
- Kazanma kosulu: 60/100 musteri payi (TERRITORY DEGIL -- customer share)
- Win/lose: WinLoseChecker artik customer share parametreleri almali

GOREV: Test plani yaz. Su formati kullan:

TEST CASE ID | SISTEM | SENARYO | BEKLENEN SONUC | GERCEK SONUC (bos)

Minimum 25 test case:
- TC001: FastFood secimi -> Rival da FastFood mi basliyor?
- TC002: Marketing slot dolu -> Platform rating decay durdu mu?
- TC003: Tur 5'te season degisti mi?
- TC004: 60 musteri asildi -> kazanma ekrani gorundu mu?
- TC005: Rival 60 musteri aldi -> kaybetme ekrani gorundu mu?
- TC006: Tur 25'te oyun bitti mi? (HARD_CAP_TURN=25)
- TC007: RivalAI.Initialize(VentureType.Cafe) -> rival "Rival Cafe" mi? (eski bug kontrolu)
- TC008: SlotManager'a Operation karti yerlestirildi -> BoardManager'da da gorunuyor mu? (gecis testi)
- TC009: CompanyTierSystem 20+ musteri -> Entrepreneur tier'a yukseldi mi?
- TC010: CompanyTierSystem 45+ musteri + 1 combo -> Corporation tier mi?
- ...vb.

Kritik bug'lari tespit etmek icin edge case'leri dahil et:
- 0 musterili oyun (tum slotlar bos)
- Platform rating 1.0'a dustu -> ne olur?
- Legal risk 100 -> raid gerceklesti mi?
- Tur 25 -> oyun kapaniyor mu?
- Player bankrupt (money <= 0) -> lose tetiklendi mi?
- Eski venture enum (Diner, TechStartup) ile baslatma denemesi -> ne olur?
```

---

## ONCE BUNLARI CALISTIR (Sira onemli)

1. **PROMPT 1** (Game Designer) -> kart listesi al
2. **PROMPT 2** (Economy Designer) -> kart listesini al, balance ver
3. **PROMPT 3** (Senior Dev 2) -> CardData + BalanceDefs + AssetGenerator yaz
4. **PROMPT 4** (Senior Dev 1) -> ResolvePhase + CompanyTierSystem + GameEnums guncelle
5. **PROMPT 5** (Senior Dev 3) -> RivalAI guncelle (VentureType bug fix DAHiL)
6. **PROMPT 6** (Lead Dev) -> SlotManager entegrasyonu, win condition degisimi, architecture kontrol
7. **PROMPT 7** (Level Designer) -> mevcut Unity sahnelerini duzenle (yeni sahne OLUSTURMA)
8. **PROMPT 8** (Narrative) -> metinler
9. **PROMPT 9** (QA) -> test (tum yukardaki degisiklikler bittikten sonra)

---

## DOSYA REFERANSLARI (Yeni sohbette bunlari oku)

```
# Core
Assets/_EmpireOfCards/Scripts/Core/Enums/GameEnums.cs
Assets/_EmpireOfCards/Scripts/Core/Constants.cs
Assets/_EmpireOfCards/Scripts/Core/EventBus.cs
Assets/_EmpireOfCards/Scripts/Core/GameManager.cs
Assets/_EmpireOfCards/Scripts/Core/TurnManager.cs
Assets/_EmpireOfCards/Scripts/Core/PlayerResources.cs
Assets/_EmpireOfCards/Scripts/Core/WinLoseChecker.cs
Assets/_EmpireOfCards/Scripts/Core/InputManager3D.cs
Assets/_EmpireOfCards/Scripts/Core/StateMachine/IState.cs
Assets/_EmpireOfCards/Scripts/Core/StateMachine/StateMachine.cs
Assets/_EmpireOfCards/Scripts/Core/TurnPhases/ResolvePhase.cs
Assets/_EmpireOfCards/Scripts/Core/TurnPhases/PlayPhase.cs

# Bootstrap
Assets/_EmpireOfCards/Scripts/Bootstrap/WiringService.cs
Assets/_EmpireOfCards/Scripts/Bootstrap/ManagerFactory.cs
Assets/_EmpireOfCards/Scripts/Bootstrap/GameSceneBootstrap.cs
Assets/_EmpireOfCards/Scripts/Bootstrap/BootSceneController.cs
Assets/_EmpireOfCards/Scripts/Bootstrap/MainMenuController.cs
Assets/_EmpireOfCards/Scripts/Bootstrap/CardDataFactory.cs
Assets/_EmpireOfCards/Scripts/Bootstrap/HUDBuilder.cs
Assets/_EmpireOfCards/Scripts/Bootstrap/Data/BalanceDefs.cs
Assets/_EmpireOfCards/Scripts/Bootstrap/Data/CardHelper.cs
Assets/_EmpireOfCards/Scripts/Bootstrap/Data/VentureDataFactory.cs
Assets/_EmpireOfCards/Scripts/Bootstrap/Data/BusinessCardDefs.cs
Assets/_EmpireOfCards/Scripts/Bootstrap/Data/EmployeeCardDefs.cs
Assets/_EmpireOfCards/Scripts/Bootstrap/Data/ActionCardDefs.cs
Assets/_EmpireOfCards/Scripts/Bootstrap/Data/UpgradeCardDefs.cs
Assets/_EmpireOfCards/Scripts/Bootstrap/Data/EventCardDefs.cs
Assets/_EmpireOfCards/Scripts/Bootstrap/Data/ComboDefs.cs

# Gameplay
Assets/_EmpireOfCards/Scripts/Gameplay/SlotManager.cs
Assets/_EmpireOfCards/Scripts/Gameplay/BoardManager.cs
Assets/_EmpireOfCards/Scripts/Gameplay/EconomyManager.cs
Assets/_EmpireOfCards/Scripts/Gameplay/CompanyTierSystem.cs
Assets/_EmpireOfCards/Scripts/Gameplay/RivalAI.cs
Assets/_EmpireOfCards/Scripts/Gameplay/AbilitySystem.cs
Assets/_EmpireOfCards/Scripts/Gameplay/ActionCardResolver.cs
Assets/_EmpireOfCards/Scripts/Gameplay/DeckManager.cs

# Gameplay / Economy
Assets/_EmpireOfCards/Scripts/Gameplay/Economy/MarketPool.cs
Assets/_EmpireOfCards/Scripts/Gameplay/Economy/IncomeCalculator.cs
Assets/_EmpireOfCards/Scripts/Gameplay/Economy/TaxCalculator.cs
Assets/_EmpireOfCards/Scripts/Gameplay/Economy/DebtTracker.cs

# Gameplay / Board
Assets/_EmpireOfCards/Scripts/Gameplay/Board/ClosureManager.cs
Assets/_EmpireOfCards/Scripts/Gameplay/Board/BusinessEvolution.cs
Assets/_EmpireOfCards/Scripts/Gameplay/Board/EmployeeTenure.cs
Assets/_EmpireOfCards/Scripts/Gameplay/Board/BoardQueries.cs

# Gameplay / FBI
Assets/_EmpireOfCards/Scripts/Gameplay/FBI/RiskCalculator.cs
Assets/_EmpireOfCards/Scripts/Gameplay/FBI/RaidExecutor.cs

# Gameplay / Combo
Assets/_EmpireOfCards/Scripts/Gameplay/Combo/ComboEvaluator.cs
Assets/_EmpireOfCards/Scripts/Gameplay/Combo/ComboBonusProvider.cs

# Gameplay / Rival
Assets/_EmpireOfCards/Scripts/Gameplay/Rival/RivalDecisionTree.cs
Assets/_EmpireOfCards/Scripts/Gameplay/Rival/RivalEconomy.cs
Assets/_EmpireOfCards/Scripts/Gameplay/Rival/RivalGrowth.cs

# World (3D)
Assets/_EmpireOfCards/Scripts/World/Board3D.cs
Assets/_EmpireOfCards/Scripts/World/SlotZone3D.cs
Assets/_EmpireOfCards/Scripts/World/Card3D.cs
Assets/_EmpireOfCards/Scripts/World/CardFactory.cs
Assets/_EmpireOfCards/Scripts/World/Hand3D.cs

# UI
Assets/_EmpireOfCards/Scripts/UI/UIManager.cs
Assets/_EmpireOfCards/Scripts/UI/Cards/DropZone.cs
Assets/_EmpireOfCards/Scripts/UI/TierPopup.cs
Assets/_EmpireOfCards/Scripts/UI/VentureSelectionUI.cs

# Data
Assets/_EmpireOfCards/Scripts/Data/CardData.cs
Assets/_EmpireOfCards/Scripts/Data/VentureData.cs
Assets/_EmpireOfCards/Scripts/Data/RivalData.cs
Assets/_EmpireOfCards/Scripts/Data/GameBalanceData.cs
Assets/_EmpireOfCards/Scripts/Editor/AssetGenerator.cs

# Scenes (Assets/Scenes/ dizininde -- _EmpireOfCards icinde DEGIL)
Assets/Scenes/Boot.unity
Assets/Scenes/MainMenu.unity
Assets/Scenes/Game.unity
Assets/Scenes/GameScene.unity  # MUHTEMELEN ESKI DUPLIKAT -- kontrol et

# GDD
Assets/steam-card-game-gdd/GDD.md
Assets/steam-card-game-gdd/businesses/fast_food.md
Assets/steam-card-game-gdd/businesses/cafe.md
Assets/steam-card-game-gdd/businesses/tech_app.md
Assets/steam-card-game-gdd/businesses/giyim_magazasi.md
Assets/steam-card-game-gdd/businesses/market_bakkal.md
```
