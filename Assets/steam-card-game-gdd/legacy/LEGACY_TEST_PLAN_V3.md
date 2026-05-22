# Empire of Cards — QA Test Plan v3
**Tarih:** 2026-05-20  
**QA:** Tester #1 — Gameplay Logic  
**Kapsam:** GDD v3.0 (customer share-based win condition, 5 venture, 5 slot types, 25-turn)

---

## SMOKE TEST — En Kritik 5 Test (Önce Bunları Koştur)

| Sira | Test | Neden Kritik |
|------|------|--------------|
| 1 | TC016: 60+ musteri → WIN ekrani | Temel win kosuludur; bozuksa tüm oyun test edilemez |
| 2 | TC001: FastFood seçimi → Rival da FastFood | GDD'nin "ayni venture" kurali; break ederse rival denge çöker |
| 3 | TC014: Tur 25'te oyun zorla biter | Hard cap yoksa oyun sonsuz döner |
| 4 | TC006: Operation slotuna kart yerles | Temel slot placement mekanigi |
| 5 | TC011: 5 faz sirayla çalisiyor | Turn flow bozuksa hicbir test anlamsiz |

---

## TEST CASE TABLOSU

| TC ID | SISTEM | SENARYO | BEKLENEN SONUÇ | ÖNCELIK |
|-------|--------|---------|----------------|---------|
| **TC001** | Venture Seçimi | FastFood seçilir → StartNewRun() → rivalAI.Initialize(VentureType.FastFood) çagrilir | Rival'in ilk business name "Rival Diner" DEGIL, FastFood türünden bir isim olmali (BUG VAR — bakınız Kritik Bug #1) | CRITICAL |
| **TC002** | Venture Seçimi | Cafe seçilir → rivalAI.Initialize(VentureType.Cafe) çagrilir | Rival, Cafe türünde baslar; income/customer degerlerini Cafe'ye uygun alir | HIGH |
| **TC003** | Venture Seçimi | TechApp seçilir → rivalAI.Initialize(VentureType.TechApp) çagrilir | Rival, TechApp türünde baslar; income=0, customers=0 (2-tur delay) | HIGH |
| **TC004** | Venture Seçimi | ClothingStore seçilir → rivalAI.Initialize(VentureType.ClothingStore) çagrilir | Rival, ClothingStore türünde baslar | HIGH |
| **TC005** | Venture Seçimi | GroceryStore seçilir → rivalAI.Initialize(VentureType.GroceryStore) çagrilir | Rival, GroceryStore türünde baslar | HIGH |
| **TC006** | Slot Sistemi | Operation türünde bir CardData oluştur → TryPlace(card, SlotType.Operation, 0) | return true, slot[0] = card, EventBus.CardPlacedInSlot ateşlendi | HIGH |
| **TC007** | Slot Sistemi | Operation türünde bir kart → TryPlace(card, SlotType.Marketing, 0) dene | SlotManager'in kart tipi doğrulama yapmadigi için yanlislikla kabul edebilir (BUG VAR — bakınız Bug #2) | HIGH |
| **TC008** | Slot Sistemi | Tüm 4 Operation slotu dolu iken → TryPlace 5. kart → index 4 ile dene | Slot listesi sadece 4 eleman → slotIndex >= list.Count → return false | HIGH |
| **TC009** | Slot Sistemi | Tier 1 → Tier 2 atlamasi (Entrepreneur) → SlotManager.TryExpandSlot(Marketing) | _marketingMax 3'ten 4'e çikar, _marketingSlots.Count 4 olur | HIGH |
| **TC010** | Slot Sistemi | 3 TempEffect slotu dolu → 4. TempEffect kartı yerleştirilmeye calisiliyor | TryPlace index 3 ile → list.Count = 3 → return false. ANCAK: ClearExpiredTempEffects() tüm TempEffect slotlarini temizliyor, en eskiyi degil (BUG VAR — bakınız Bug #3) | MEDIUM |
| **TC011** | Turn Flow | StartNewRun() → tam 5 faz sirasinda çalisiyor mu? | Faz sirasi: EventPhase → DrawPhase → PlayPhase → ResolvePhase → RivalPhase | CRITICAL |
| **TC012** | Turn Flow | Tur 5 tamamlanir → SeasonType Spring → Summer'a geçiyor mu? | TURNS_PER_SEASON = 5 → Tur 6 basinda season = Summer | HIGH |
| **TC013** | Turn Flow | Tur 20'de (SOFT_CAP_TURN) EndCurrentTurn() çagrılır → income penalty | EconomyManager.ProcessEndOfTurn(): currentTurn > 20 → penalty = (turns-20) * 0.05f | HIGH |
| **TC014** | Turn Flow | currentTurn >= 25 (HARD_CAP_TURN = MAX_TURNS) → EndCurrentTurn() | EndRun(playerTerritories >= rivalTerritories) çagrilir; oyun biter | CRITICAL |
| **TC015** | Turn Flow (Resolve) | ResolvePhase çalışır | Adim sirasi: BusinessProduce(0.4s) → CustomerFlow(0.6s) → ComboCheck(1.0s) → TierCheck(0.3s/1.5s) → IncomeCalculation(0.8s) → DeteriorationCheck(0.5s) | HIGH |
| **TC016** | Win/Lose | playerTerritories = 6 → EndCurrentTurn() | WinLoseChecker.CheckWin(6, 6) = true → EndRun(true) → GameState.ScoreScreen. ANCAK: Win koşulu GDD'de "60/100 musteri" olarak degisti, kod hala territory bazli kontrol yapiyor (BUG VAR — bakınız Kritik Bug #4) | CRITICAL |
| **TC017** | Win/Lose | rivalTerritories = 7 → EndCurrentTurn() | WinLoseChecker.CheckLose(7, 7, money) = true → EndRun(false) → GameState.GameOver. Ayni mimari sorun gecerli | CRITICAL |
| **TC018** | Win/Lose | Tur 25, playerTerritories > rivalTerritories | EndRun(true) → WIN ekrani | HIGH |
| **TC019** | Win/Lose | Tur 25, rivalTerritories > playerTerritories | EndRun(false) → LOSE ekrani | HIGH |
| **TC020** | Market Share | MarketPool.CalculatePlayerMarketShare(q, p, r, m, s, l) | Agirlik formülü: quality*0.30 + price*0.20 + normalizedRating*0.20 + marketing*0.15 + speed*0.10 + loyalty*0.05; toplam = 1.0 ✓ | HIGH |
| **TC021** | Platform Rating | Marketing slotu en az 1 dolu → tur sonu DecayPlatformRating() çagrilmamali | EconomyManager.DecayPlatformRating() Marketing slot kontrolü yapiyor mu? Kod incelendi — dogrudan çagrim var, slot kontrolü eksik olabilir (BUG VAR — bakınız Bug #5) | MEDIUM |
| **TC022** | Platform Rating | platformRating = 1.0 → GetRatingCustomerMultiplier() | return 0.90f (below 3.0 bracket). GDD: "-10% musteri" — Kod: 0.90f multiplier ✓ | MEDIUM |
| **TC023** | Legal Risk | fbiRisk >= 0.76f (LegalRiskLevel.Certain = 76+) → FBISystem.CheckForRaid() | Raid tetiklenir; FBI_RAID_PENALTY = 300 para kesilir | HIGH |
| **TC024** | Rival AI | Rival Tier 1 → data.actionsPerTurn = 2 mi? | RivalPhase.Enter() → rival.TakeTurn() → for (int a = 0; a < data.actionsPerTurn; a++). Deger RivalData ScriptableObject'ten geliyor — Inspector'da dogru ayarlanmali | HIGH |
| **TC025** | Rival AI | Rival "aggressive" action → isSabotage = true → gm.BoardManager.SetProductionDisabledNextTurn(true) | Sonraki tur BusinessProduce adiminda ConsumeProductionDisabled() = true → "rival sabotage!" mesajı ateslenip atlanir | HIGH |
| **TC026** | Rival AI | rivalAI.Initialize(VentureType.FastFood) çagrilir | switch(playerVenture) case FastFood: yok! Switch sadece legacy type'lari (Diner, TechStartup, AdAgency, BlackMarket) kapsıyor — FastFood, Cafe, TechApp, ClothingStore, GroceryStore için case yok → fall-through default → rival ayni kalir (BUG VAR — bakınız Kritik Bug #1) | CRITICAL |
| **TC027** | Rival AI | Rival musteri hesabi: economy.CalculateRivalCustomers(rivalBusinesses) | Rival ayni 6-faktorlu market share formulünü kullanmali. Kod incelenmeli: RivalEconomy.CalculateRivalCustomers() formulü MarketPool'daki ile eslesmeli | HIGH |
| **TC028** | Edge Case | 0 aktif isletme, 0 musteri → ResolvePhase calisir | BoardManager.TickBusinesses() güvenli çalışır; CalculatePlayerCustomers() = 0; Income = 0; crash yok | HIGH |
| **TC029** | Edge Case | playerMoney = 0 → EndCurrentTurn() çagrilir | WinLoseChecker.CheckLose(rivalTerritories, loseReq, 0): playerMoney <= 0 → return true → EndRun(false). ONAYLANDI — bankrupt = lose mantigi var ✓ | HIGH |
| **TC030** | Edge Case | Tüm slot tipleri max'a ulasti → TryExpandSlot() denenir | Operation: _operationMax >= 8 → false; Staff >= 10 → false; Marketing >= 5 → false; Supplier >= 4 → false; TempEffect: her zaman false ✓ | MEDIUM |
| **TC031** | Edge Case | Deck bos iken DrawPhase çalişir | DeckManager.DrawCards(5): deck boşsa hand boş döner, NullRef yok, phase completes | MEDIUM |
| **TC032** | Regresyon | VentureType.Diner, TechStartup, AdAgency, BlackMarket enum degerlerinin seçilememesi | VentureSelectionUI sadece 5 yeni tipi sunmali; legacy enumlar sadece save-compat için var, UI'da görünmemeli | HIGH |

---

## BULUNAN KRITIK BUG LISTESI

---

### BUG #1 (KRITIK): RivalAI Yeni VentureType'lari Hiç Handle Etmiyor

**Severity:** Critical  
**Dosya:** `/Assets/_EmpireOfCards/Scripts/Gameplay/RivalAI.cs` satir 124-148  

**Steps:**
1. VentureSelectionUI'dan FastFood, Cafe, TechApp, ClothingStore veya GroceryStore seç
2. StartNewRun() → rivalAI.Initialize(VentureType.FastFood) çagrilir
3. Initialize(VentureType) içindeki switch statement'i incele

**Beklenen:** Rival, FastFood ile eslesen bir isletme ismi ve istatistikleriyle baslar  
**Actual:** switch sadece Diner/TechStartup/AdAgency/BlackMarket case'lerini kapsıyor. FastFood, Cafe, TechApp, ClothingStore, GroceryStore için case yok. Rival, default RivalData baslangic degerlerini korur — venture tipiyle eslesmez.

**EventBus chain:** StartNewRun → rivalAI.Initialize(VentureType) → switch → fallthrough default → rivalBusinesses[0].name = data.startingBusinessName (generic)

```csharp
// RivalAI.cs satir 124: Switch sadece legacy tipler:
case VentureType.Diner:
case VentureType.TechStartup:
case VentureType.AdAgency:
case VentureType.BlackMarket:
// FastFood, Cafe, TechApp, ClothingStore, GroceryStore icin case YOK
```

---

### BUG #2 (HIGH): SlotManager Kart Tipi Dogrulamasi Yapmıyor

**Severity:** High  
**Dosya:** `/Assets/_EmpireOfCards/Scripts/Gameplay/SlotManager.cs` satir 66-75

**Steps:**
1. Operation CardType'inda bir kart al
2. TryPlace(card, SlotType.Marketing, 0) cagir
3. Sonucu gözlemle

**Beklenen:** Kart tipi ile slot tipi uyusmazsa false dönmeli, EventBus ateslenmemeli  
**Actual:** TryPlace() sadece slotIndex sinirlari ve doluluk kontrolü yapiyor. CardType ↔ SlotType uyum kontrolü yok. Bir Employee kartini Supplier slotuna, bir Business kartini Staff slotuna koymak mümkün.

**EventBus chain:** TryPlace → list[slotIndex] = card → EventBus.CardPlacedInSlot() ateslenip yanlis bir slot assignment confirm edilir

---

### BUG #3 (MEDIUM): ClearExpiredTempEffects() En Eskiyi Degil Tumunu Temizliyor

**Severity:** Medium  
**Dosya:** `/Assets/_EmpireOfCards/Scripts/Gameplay/SlotManager.cs` satir 163-174

**Steps:**
1. 3 TempEffect slotunu farkli eventlerle doldur (olay1, olay2, olay3)
2. ResolvePhase → ClearExpiredTempEffects() çagrilir
3. Hangi kartlarin kaldigina bak

**Beklenen:** GDD Section 4.3: "TempEffect slotlari doluysa yeni event geldiginde en eski event düşer." Yani sadece süresi biten kartlar temizlenmeli.  
**Actual:** ClearExpiredTempEffects() tüm TempEffect slotlarindaki NULL olmayan her karti temizliyor. Event süresi bitmesine gerek yok, hepsi silinir. Bu hem "süre takibi" eksikligini hem de "en eski düşme" mantigi yerine "hepsini sil" anti-pattern'ini ortaya koyar.

---

### BUG #4 (CRITICAL): Win/Lose Kontrolü Eski Territory Sistemine Göre Kodlanmis

**Severity:** Critical  
**Dosya:** `/Assets/_EmpireOfCards/Scripts/Core/GameManager.cs` satir 253-276  
**Ilgili:** `/Assets/_EmpireOfCards/Scripts/Core/WinLoseChecker.cs`

**Steps:**
1. GDD v3.0: Win = 60/100 musteri (customer share-based)
2. Constants.cs'e bak: WIN_CUSTOMER_SHARE = 60 tanimli ✓
3. WinLoseChecker.CheckWin() ve GameManager.EndCurrentTurn()'a bak

**Beklenen:** CheckWin(playerCustomers, 60) — musteri sayisi kontrolü  
**Actual:** `WinLoseChecker.CheckWin(playerTerritories, winReq)` — hala territory bazli kontrol yapiliyor. playerCustomers hiç CheckWin'e geçilmiyor. WIN_CUSTOMER_SHARE sabiti tanimlanmis ama kullanilmiyor; WIN_TERRITORIES (6) sabiti kullanilmaya devam ediyor.

**Ayrica:** GameManager.EndCurrentTurn():
```csharp
int winReq = balanceData != null ? balanceData.winTerritories : Constants.WIN_TERRITORIES;
// balanceData.winTerritories territory bazli; musteri bazli olmali
```

**EventBus chain:** EndCurrentTurn → WinLoseChecker.CheckWin(playerTerritories, 6) → territory dogru olsa bile musteri-bazli win tetiklenmez

---

### BUG #5 (MEDIUM): Platform Rating Decay Marketing Slot Kontrolü Yapmıyor

**Severity:** Medium  
**Dosya:** `/Assets/_EmpireOfCards/Scripts/Gameplay/EconomyManager.cs` satir 341-344

**Steps:**
1. Marketing slotuna en az 1 kart yerleştir
2. Tur sonunda DecayPlatformRating() çagrilip çagrilmadigini gözlemle

**Beklenen:** GDD v3.0 Section 8: "Marketing slot dolu ise platform rating decay DURUR"  
**Actual:** DecayPlatformRating() metodunda `ModifyPlatformRating(-0.1f)` direkt uygulanir. Hangi sistemin bunu cagirdigina bakildiginda (ResolvePhase, EconomyManager) — Marketing slot kontrolü yapilip yapilmadigi belirsiz. DecayPlatformRating() içinde SlotManager.HasEmpty(SlotType.Marketing) kontrolü yok. Marketing slotu dolu olsa bile decay çalisabilir.

---

### BUG #6 (MEDIUM): Soft Cap Yorum Hatasi — Yorum "Turn 25" Diyor, Kod Turn 20 Kullanıyor

**Severity:** Medium (yorum hatasi, mantık dogru)  
**Dosya:** `/Assets/_EmpireOfCards/Scripts/Gameplay/EconomyManager.cs` satir 158

**Steps:**
1. EconomyManager.cs satir 158'e bak

**Actual:**
```csharp
// Soft cap penalty: -5% income per turn after turn 25 (GDD Section 1.7)
if (currentTurn > Constants.SOFT_CAP_TURN)  // SOFT_CAP_TURN = 20
```
Yorum "after turn 25" diyor, ama Constants.SOFT_CAP_TURN = 20 ve kod `currentTurn > 20` kontrolü yapiyor. GDD'ye göre soft cap turn 20'de basliyor. Yorum yanlis.

---

## REGRESYON TEST LISTESİ
(Eski Territory-Based → Yeni Customer Share-Based Geçisi)

| ID | REGRESYON SENARYOSU | KONTROL NOKTASI |
|----|---------------------|-----------------|
| R01 | WinLoseChecker.CheckWin() parametre: playerCustomers >= 60 mi, playerTerritories >= 6 mi? | WinLoseChecker.cs:13 |
| R02 | WinLoseChecker.CheckLose() parametre: rivalCustomers >= 60 mi, rivalTerritories >= 7 mi? | WinLoseChecker.cs:21 |
| R03 | GameManager.EndCurrentTurn() winReq degiskeni: balanceData.winCustomerShare mi, winTerritories mi? | GameManager.cs:256 |
| R04 | Turn-25 tiebreaker: playerCustomers >= rivalCustomers mi, playerTerritories >= rivalTerritories mi? | GameManager.cs:274 |
| R05 | WIN_TERRITORIES (6) ve LOSE_TERRITORIES (7) sabitleri artik kullanilmayin; WIN_CUSTOMER_SHARE (60) kullanilmali | Constants.cs:112-114 |
| R06 | VentureType.Diner, TechStartup, AdAgency, BlackMarket — VentureSelectionUI'da görünmüyor olmali | VentureSelectionUI.cs |
| R07 | RivalAI.Initialize(VentureType) — yeni 5 venture tipi için case tanimlanmali | RivalAI.cs:124 |
| R08 | TerritoryManager.CalculateTerritories() — artik müşteri payini territory'ye çevirmeyip dogrudan customer count dönüstürmeli ya da kaldırilmali | ResolvePhase.cs:119 |
| R09 | ResolvePhase.CustomerFlow adimi — customerShare = playerCustomers/marketPool hesabini yapip GameManager.SetPlayerCustomers() çagirmali | ResolvePhase.cs:105-121 |
| R10 | ScoreScreen — Territory skoru (SCORE_TERRITORY = 500) yerine customer share skoru kullanilmali | Constants.cs:132 |

---

## EDGE CASE LISTESI

| ID | EDGE CASE | RISK SEVIYESI | NOTLAR |
|----|-----------|---------------|--------|
| E01 | playerMoney = 1, SpendMoney(2) çagrilir | HIGH | PlayerResources.SpendMoney() bakiye kontrolü yapıyor mu? Negatife izin veriyor mu? (WinLoseChecker zaten <= 0 = lose yapiyor, ama intermediate state'de negatif mümkün mü?) |
| E02 | Deck bos, DrawPhase çalışır | MEDIUM | DeckManager.DrawCards(5) — shuffle/discard pile logic |
| E03 | Hem playerTerritories=6 hem rivalTerritories=7 ayni tur | HIGH | Kontrol sirasi önemli: CheckWin önce → player kazanir. EndCurrentTurn() satir 259-264 dogrulanmali |
| E04 | TempEffect slotu dolu iken yeni event gelir | MEDIUM | TryPlace index 3 → Count 3 → false. Ama "en eski düşür" mekanigi eksik (Bug #3) |
| E05 | Operation slot tam dolu (4/4) iken Upgrade kartı Operation'a yonlendirilmeye çalisiliyor | MEDIUM | PlayPhase.GetDefaultSlotType(): Upgrade → tag yoksa SlotType.Operation; slot doluysa DropZone reject etmeli |
| E06 | 0 isletme ile ResolvePhase | MEDIUM | boardManager.TickBusinesses() — bos liste → foreach güvenli. CalculatePlayerCustomers() → 0 döner. Income = 0. Crash yok beklenir |
| E07 | RivalAI.data = null ile TakeTurn() | HIGH | Satir 169: `if (data == null) return;` — guard var ✓ |
| E08 | BalanceData null iken MaxTurns okunuyor | MEDIUM | GameManager.MaxTurns: `balanceData != null ? balanceData.maxTurns : Constants.MAX_TURNS` — fallback var ✓ |
| E09 | Tier atlamasi ile slot genişler ama SlotManager.Init() çoktan çagrilmis | MEDIUM | TryExpandSlot() Init'e bagli degil, her zaman çagrılabilir ✓ |
| E10 | GameManager.Instance null iken ResolvePhase.ExecuteStep() | HIGH | Her case basinda `var gm = GameManager.Instance; if (gm == null) return;` benzeri kontrol eksik — null check sadece bazi bloklarda var |
| E11 | currentTurn = 20 (tam soft cap, > ile degil) | MEDIUM | `currentTurn > Constants.SOFT_CAP_TURN` (20 > 20 = false) → tur 20'de ceza yok, tur 21'de basliyor. GDD "Turn 20 sonrasinda" diyorsa bu dogru; "Turn 20 dahil" diyorsa bug |
| E12 | Marketing slotlari expand edildi (5/5), ek TryExpandSlot(Marketing) | MEDIUM | _marketingMax >= 5 → return false ✓ |
| E13 | Rival money = 0, "open_business" action seçilir | MEDIUM | RivalGrowth.OpenBusiness() içinde para kontrolü yapiliyor mu? |
| E14 | PlatformRating zaten 1.0 iken DecayPlatformRating() | LOW | Mathf.Clamp(..., 1.0f, 5.0f) → 1.0'in altina düsmez ✓ |

---

## TEST AKISI NOTU

EventPhase'de önemli bir durum tespiti: Kod `_turnManager.CurrentTurnNumber % interval == 0 && _turnManager.ActiveEvent == null` kontrolü yapıyor. Bu demek oluyor ki tur 6'da aktif event varsa, event atlanir (tur 9'a ertelenir). GDD bu davrasi tanimliyor mu dogrulanmali.

RivalPhase'de aksiyon sayisi dogrudan `data.actionsPerTurn` ile belirleniyor. Bu deger ScriptableObject'ten (Inspector) okunuyor — tier'a göre dinamik artis mekanigi kod tarafinda degil, data asset tarafinda el ile set ediliyor. Bu tasarim niyeti mi kontrol edilmeli.

---

*Test Plan — Empire of Cards QA Tester #1 — 2026-05-20*
