# IMPLEMENTATION GAP AUDIT
# Empire of Cards

> Tarih: 2026-05-22
> Kapsam: Repo-first audit
> Amaç: "Hangi GDD güncel, kodda ne çalışıyor, oyun sahnesinde ne eksik, ekonomi nasıl planlanmış, bundan sonra neyi tamamlamalıyız?" sorularına tek dokümanda cevap vermek.

---

## 1. Executive Summary

Mevcut proje artık eski `business-centric / territory` prototipinden çıkıp `venture-first / same-market-rival / board-pressure` modeline geçmiş durumda. Çekirdek omurga çalışıyor:

- venture seçimi var
- rakip aynı venture ile başlıyor
- 25 turluk akış var
- 5 ana slot ailesi aktif
- resolve sonunda demand / capacity / quality / rating / legal risk / market share hesaplanıyor
- gameplay sahnesi büyük ölçüde runtime kuruluyor

Ama proje henüz AGENTS.md’de tarif edilen "gerçek işletme simülasyonu" derinliğine ulaşmış değil. Şu anki build daha çok:

- venture temalı ekonomik kart-strateji oyunu
- sınırlı board simülasyonu
- hafif kriz ve rakip baskısı

seviyesinde.

En kritik açıklar:

1. GDD doküman seti parçalı; v4 ile legacy v2/v3 dokümanlar karışık duruyor.
2. EconomyManager güncel omurga ama bazı yan ekonomi sistemleri canlı döngüye tam bağlanmamış.
3. Oyun sahnesi teknik olarak çalışıyor ama bazı manager/sistemler legacy yük taşıyor.
4. AGENTS.md’de hedeflenen HR, supplier contract, multi-branch, legal workflow, poaching, inflation gibi derin sim katmanları henüz tam uygulanmamış.
5. Bazı action/economy parçaları eski business modelinden kalma ve v4 venture simülasyonuna tam oturmuyor.

---

## 2. GDD Truth Stack

## 2.1 Birincil Kaynaklar

Bu dosyalar şu an için gerçek source of truth kabul edilmeli:

| Dosya | Durum | Not |
|---|---|---|
| `Assets/steam-card-game-gdd/GDD.md` | `Authoritative` | v4 ana tasarım omurgası |
| `Assets/steam-card-game-gdd/TECHNICAL_MAPPING.md` | `Authoritative` | v4 tasarımın koda nasıl eşleneceğini tanımlıyor |
| `Assets/steam-card-game-gdd/businesses/fast_food.md` | `Authoritative` | referans venture |
| `Assets/steam-card-game-gdd/businesses/cafe.md` | `Authoritative` | venture kimliği güncel |
| `Assets/steam-card-game-gdd/businesses/tech_app.md` | `Authoritative` | venture kimliği güncel |
| `Assets/steam-card-game-gdd/businesses/giyim_magazasi.md` | `Authoritative` | venture kimliği güncel |
| `Assets/steam-card-game-gdd/businesses/market_bakkal.md` | `Authoritative` | venture kimliği güncel |

## 2.2 İkincil Referanslar

Bunlar ürün vizyonu ve içerik planı için yararlı ama gameplay source of truth değiller:

| Dosya | Durum | Not |
|---|---|---|
| `Assets/steam-card-game-gdd/ART_DIRECTION_GUIDE.md` | `Reference` | v2 tabanlı; görsel yön için yararlı |
| `Assets/steam-card-game-gdd/ASSET_LIST.md` | `Reference` | asset/checklist dökümanı |
| `Assets/steam-card-game-gdd/COMBO_MATRIX.md` | `Reference / stale-leaning` | eski aktif kart modeline daha yakın |
| `Assets/steam-card-game-gdd/MARKET_RESEARCH.md` | `Reference` | ürün/pazar kararı için |
| `Assets/steam-card-game-gdd/PAPER_PROTOTYPE.md` | `Reference / stale-leaning` | eski prototip dili taşıyor |
| `Assets/steam-card-game-gdd/PREMORTEM.md` | `Reference` | ürün riski için yararlı |
| `Assets/steam-card-game-gdd/NEXT_SESSION_PROMPTS.md` | `Session log` | tarihsel not, source of truth değil |

## 2.3 Legacy / Çelişkili Dokümanlar

Bu dosyalar yeni kod omurgasıyla çelişiyor veya eski modeli anlatıyor:

| Dosya | Durum | Problem |
|---|---|---|
| `Assets/steam-card-game-gdd/legacy/LEGACY_BALANCE.md` | `Legacy / conflicting` | 20 turn dili taşıyor, eski ekonomi varsayımları var |
| `Assets/steam-card-game-gdd/legacy/LEGACY_CARD_LIST.md` | `Legacy` | 40 kartlık eski MVP/business kart listesi |
| `Assets/steam-card-game-gdd/legacy/LEGACY_CARD_LIST_V3.md` | `Legacy` | v3 kart sistemi |
| `Assets/steam-card-game-gdd/legacy/LEGACY_BALANCE_TABLE_V3.md` | `Legacy` | eski income/customer modeli |
| `Assets/steam-card-game-gdd/legacy/LEGACY_TEST_PLAN_V3.md` | `Legacy` | eski phase sırası ve territory bug’ları |
| `Assets/steam-card-game-gdd/legacy/LEGACY_NARRATIVE_V3.md` | `Partial legacy` | flavor text olarak kullanılabilir, sistem truth değil |

## 2.4 v5 Redesign Durumu

`Assets/steam-card-game-gdd/v5-redesign/README.md` klasöründe v5 başlığı var ama README dışında aktif içerik yok. Şu an v5 source of truth değil; placeholder durumunda.

---

## 3. Kodun Şu Anki Gerçek Oyun Modeli

## 3.1 Canlı Oyun Döngüsü

Kodda çalışan çekirdek run yapısı:

- `25 turn`
- `5 season x 5 turn`
- domination check turn `6` sonrası
- early shop bias `5` turn
- event/crisis cadence normalde `3` turda bir
- turn flow:
  - Draw
  - Planning
  - Play
  - Resolve
  - CrisisReaction
  - Rival
  - MarketUpdate

Bu yapı v4 GDD ile genel olarak uyumlu.

## 3.2 Runtime Mimari

Gameplay sahnesi ağırlıklı olarak runtime’da kuruluyor:

- `GameSceneBootstrap`
- `ManagerFactory`
- `SceneRuntimeFactory`
- `HUDBuilder`
- `WiringService`

Yani sahnedeki kritik sistemlerin çoğu prefab/inspector yerine kodla oluşturuluyor.

## 3.3 Sahne Stack

Build settings’e göre aktif scene stack:

| Scene | Durum |
|---|---|
| `Assets/Scenes/Boot.unity` | aktif |
| `Assets/Scenes/MainMenu.unity` | aktif |
| `Assets/Scenes/Game.unity` | aktif |

Ek olarak:

| Scene | Durum |
|---|---|
| `Assets/Scenes/LegacyGameScene.unity` | repo’da var ama build’de değil; legacy/deneme sahnesi |

---

## 4. Oyun Sahnesi Audit

## 4.1 Şu An Ne Çalışıyor

Gameplay scene tarafında çalışan ana katmanlar:

| Sistem | Durum | Not |
|---|---|---|
| 3D kamera kurulumu | `Implemented` | `SceneRuntimeFactory` |
| ışık / post-process | `Implemented` | runtime oluşturuluyor |
| 3D board | `Implemented` | `Board3D.BuildBoard()` |
| hand presentation | `Implemented` | `Hand3D` runtime kuruluyor |
| HUD canvas | `Implemented` | `HUDBuilder` |
| top bar / rating / legal risk / market share UI | `Implemented` | UI event-driven güncelleniyor |
| venture selection | `Implemented` | `GameSceneBootstrap` |
| run name / tech category prompt | `Implemented` | bootstrap içinde |
| tutorial overlay | `Implemented` | bootstrap içinde |

## 4.2 Board Yapısı

Board3D tarafında görünen mantık v4 ile uyumlu:

- Player zone
- Shared market / market band
- Rival zone
- Operation / Staff / Marketing / Supplier / Temp Effect slot aileleri
- market blokları
- signal/intention alanları

Ama hâlâ legacy izler var:

| Kalıntı | Etki |
|---|---|
| `business slots` compatibility | kavramsal gürültü yaratıyor |
| `TerritoryManager` adı | sistem artık territory değil market-share ağırlıklı |
| bazı eski event/action isimleri | yeni venture modeline tam oturmuyor |

## 4.3 Scene Tarafındaki Eksikler

Gameplay scene teknik olarak ayağa kalkıyor ama aşağıdakiler eksik:

| Eksik | Öncelik | Neden |
|---|---|---|
| legacy `LegacyGameScene.unity` temizliği | P1 | sahne karışıklığı yaratıyor |
| runtime hierarchy dökümantasyonu | P1 | yeni sistem eklerken onboard kolaylaştırır |
| venture-specific contextual UI | P1 | ikinci sıra derived metric’ler yeterince görünür değil |
| legal/inspection UX | P1 | risk artıyor ama gerçek prosedürel his zayıf |
| salary/credit/insurance/tax karar panelleri | P1 | sistem var, oyuncu kararı yüzeye tam çıkmıyor |
| audio clip entegrasyonu | P2 | `AudioManager` placeholder |
| sahne debug overlay / economy inspector | P2 | balans iterasyonu yavaşlıyor |

---

## 5. Ekonomi Nasıl Planlanmış

## 5.1 Canlı Ekonomi Omurgası

Ana resolve mantığı `EconomyManager` içinde venture snapshot modeliyle çalışıyor. Her tur sonunda şu statlar yeniden türetiliyor:

- `cash`
- `demand`
- `capacity`
- `quality`
- `rating`
- `staffStability`
- `legalRisk`
- `marketShare`
- `derivedMetrics`

Kodun gerçek ekonomik düşüncesi şu:

1. Board’daki slot kompozisyonu talepleri ve operasyonu üretir.
2. `rating` organik demand üretir.
3. `capacity < demand` ise overload oluşur.
4. overload, zayıf quality ve staff instability rating’i düşürür.
5. rating ve quality market share’i etkiler.
6. gross income seasonal multiplier ile çarpılır.
7. salary, upkeep, tax ve bazı finance subsystems net income’dan düşer.
8. rakip baskısı market share kazanımını törpüler.

## 5.2 Koddan Çıkan Basit Formüller

Bu formüller birebir ekonominin kavramsal merkezini gösterir:

```text
demand =
    baseDemand
  + operation demand
  + marketing demand
  + temp demand
  + rating-based organic demand
  + tech category modifier

capacity =
    startingCapacity
  + operation capacity
  + staff capacity
  + supplier capacity
  + temp capacity

quality =
    startingQuality
  + operation quality
  + staff quality
  + supplier quality
  + temp quality

staffStability =
    startingStaffStability
  + staff stability bonuses
  + temp stability bonuses
  - marketing overload penalty

ratingDelta =
    quality contribution
  - overload penalty
  - staff instability penalty
  + marketing rating effects
  + temp rating effects
  + tech modifier

grossIncome =
    servedDemand * baseRevenuePerDemand
  + per-card cash deltas
  then * seasonMultiplier

netIncome =
    grossIncome
  - salaries
  - upkeep
  - tax
  - subsystem expenses

marketShare =
    oldShare
  + rating contribution
  + quality contribution
  + servedDemand pressure
  - overload impact
  - rival pressure impact
```

## 5.3 Venture Bazlı Ekonomi Kimliği

GDD ve economy profile tasarımı venture’lara şunu hedefliyor:

| Venture | Ekonomik kimlik |
|---|---|
| Fast Food | yüksek tempo, kapasite kırılması, hızlı growth ama çabuk çöküş |
| Cafe | daha yavaş, rating ve sadakat odaklı |
| Tech App | geç açılan ama rating/organic scale ile sıçrayan |
| Clothing Store | stock/season/trend baskılı |
| Grocery Store | düşük marj, istikrar, tekrar talep |

Bu kimliklerin çekirdek katsayıları canlı sistemde var, ama bazı venture-spesifik alt simler henüz tam derin değil.

---

## 6. Ekonomide Eksik veya Yarım Bırakılan Katmanlar

## 6.1 Kodda Var Ama Ana Döngüye Tam Bağlı Değil

| Sistem | Durum | Sorun |
|---|---|---|
| `InflationSystem` | `Partial / dormant` | class var, event var, ama ana resolve’e bağlı değil |
| `CustomerLoyaltySystem` | `Partial / dormant` | class var ama manager factory/wiring içinde aktif kullanım görünmüyor |
| `DebtTracker` | `Partial / likely legacy` | EconomyManager kendi debt yaklaşımını zaten taşıyor |
| `MarketPool` | `Partial / legacy-leaning` | eski customer pool yaklaşımından kalma, ana v4 share modeli snapshot üzerinden dönüyor |

## 6.2 Kodda Var Ama UX’e Yansımıyor

| Sistem | Durum | Eksik yüzey |
|---|---|---|
| `SalarySystem` | `Implemented backend` | gerçek maaş karar UI akışı eksik |
| `CreditSystem` | `Implemented backend` | kredi teklifi / repayment baskısı zayıf sunuluyor |
| `InsuranceSystem` | `Implemented backend` | oyuncu davranışına anlamlı bağlanma az |
| `TaxPeriodSystem` | `Implemented backend` | denetim / ödeme / borç stresi yeterince görünür değil |
| `StockSystem` | `Implemented backend` | özellikle giyim ve market venture için daha görünür yapılmalı |

## 6.3 AGENTS.md Hedefine Göre Büyük Açıklar

AGENTS.md’de tarif edilen “gerçek işletme simülasyonu” için şu katmanlar henüz eksik:

| Alan | Mevcut durum | Eksik parça |
|---|---|---|
| HR | temel staff stateleri var | job posting, applicant pool, interview, trial, payroll flow yok |
| Salary | basit backend var | negotiation ve bordro kararı yok |
| Burnout / morale | staff state var | oyuncuya dönük yönetim loop’u zayıf |
| Supplier | kart tabanlı | kontrat, reliability, delivery failure loop eksik |
| Inventory | kısmi stock etkisi var | tam stok akışı yok |
| Multi-branch | yok | expansion gerçek işletme adımı değil |
| Legal | risk sayacı var | denetim, dava, compliance kararı sınırlı |
| Rival poaching | stub var | cevap akışları tamamlanmamış |
| Inflation | constants + class var | canlı economy integration yok |
| App venture specifics | temel identity var | funnel, bugs, server scaling, security derinliği sınırlı |

---

## 7. Legacy / Mimari Riskler

## 7.1 En Kritik Legacy Çatışmaları

| Parça | Risk | Öneri |
|---|---|---|
| `ActionCardResolver` | eski business-centric effect mantığı taşıyor | venture-first action taxonomy’ye refactor |
| `TerritoryManager` adı ve rolü | kavram kirliliği | `MarketShareVisualizer` veya benzeri role sadeleştir |
| eski GDD/BALANCE dosyaları | yanlış balans kararı alınmasına yol açar | arşiv klasörüne taşı veya `LEGACY_` prefix koy |
| `LegacyGameScene.unity` | hangi scene aktif sorusunu bulanıklaştırıyordu | legacy olarak ayrıldı |

## 7.2 Tamamlanmamış Kod Noktaları

Doğrudan görülen unfinished noktalar:

| Dosya | Eksik |
|---|---|
| `Assets/_EmpireOfCards/Scripts/Gameplay/RivalAI.cs` | poach response metodları boş |
| `Assets/_EmpireOfCards/Scripts/Audio/AudioManager.cs` | gerçek clip wiring TODO |
| `Assets/_EmpireOfCards/Scripts/Gameplay/FBI/` | belirgin canlı sistem yok |

---

## 8. Tamamlamamız Gerekenler

## 8.1 Önce Temizlenecekler

İlk iş yeni geliştirme öncesi truth cleanup yapılmalı:

1. `GDD.md + TECHNICAL_MAPPING.md + business docs` dışındaki gameplay docs’ları `reference` ve `legacy` diye ayır.
2. legacy gameplay docs aktif kökten ayrılmalı veya `legacy/` altına taşınmalı.
3. `LegacyGameScene.unity` build dışı legacy sahne olarak ayrılmalı.
4. `ActionCardResolver` ve benzeri business-centric dosyalar için migration listesi çıkarılmalı.

## 8.2 Ekonomi İçin Somut Tamamlama Sırası

### Faz 1: Mevcut omurgayı sabitle

- `EconomyManager` formüllerini tek resmi economy spec dosyasına dök
- venture başına expected ranges tanımla:
  - early demand
  - healthy capacity
  - stable rating band
  - acceptable legal risk band
  - target net income band
- resolve sonrası debug telemetry üret:
  - overload
  - served demand
  - rating delta breakdown
  - share delta breakdown

### Faz 2: Dormant ekonomi sistemlerini bağla

- `InflationSystem`i gerçek turn cadence’e bağla
- `CustomerLoyaltySystem`i active runtime’a bağla
- `TaxPeriodSystem`i UI pop-up ve decision step ile görünür yap
- `CreditSystem` için repayment pressure ve event hook ekle

### Faz 3: Venture-specific gerçekçilik ekle

- Fast Food:
  - food waste
  - hygiene inspection
  - staffing/capacity burnout
- Cafe:
  - ambience/loyalty/repeat visit loop
- Grocery:
  - spoilage / shrinkage / shelf rotation
- Clothing:
  - season-fit / return pressure / trend mismatch
- Tech App:
  - bug debt / crash risk / server cost / app store rating propagation

### Faz 4: Simülasyonu karta göm, ama sadece kartta bırakma

Kartlar sistem tetikleyicisi olmalı; sistemin kendisi olmamalı. Özellikle:

- hiring kartı tek başına “çalışan spawn” etmemeli
- supplier kartı tek başına “kalite +1” olmamalı
- bunun yerine:
  - state açmalı
  - risk oluşturmalı
  - sonraki turlarda zincir üretmeli

---

## 9. En Öncelikli Eksik Sistem Listesi

Bu liste “oyun hissini en çok güçlendirecek” sıraya göre düzenlenmiştir.

| Öncelik | Sistem | Neden şimdi |
|---|---|---|
| P1 | Economy telemetry + debug panel | balansı körlemesine yapmamak için |
| P1 | Inflation integration | ekonomi zaman içinde sertleşmeli |
| P1 | Customer loyalty integration | rating dışı uzun vadeli büyüme hissi için |
| P1 | Rival poaching completion | rakip daha canlı görünür |
| P1 | Tax / audit visible loop | legal risk gerçek baskıya dönüşür |
| P1 | ActionCardResolver refactor | eski model sızıntısını kesmek için |
| P1 | legacy doc cleanup | tasarım kararları yanlış referans almasın |
| P2 | supplier contract system | gerçek işletme hissini güçlendirir |
| P2 | inventory / spoilage | fast food / grocery / clothing için kritik |
| P2 | payroll / salary negotiation UX | AGENTS vizyonuna yaklaşır |
| P2 | branch expansion | midgame/endgame derinliği getirir |
| P3 | audio content pass | polish |

---

## 10. Pratik Uygulama Planı

## Sprint 1: Truth Cleanup + Telemetry

- legacy GDD dosyalarını ayır
- economy debug overlay ekle
- turn report’i daha analitik hale getir
- scene/runtime map’i repo docs’a yaz

Çıktı:

- herkes aynı truth stack’i kullanır
- balans kararları veriyle alınır

## Sprint 2: Economy Wiring

- `InflationSystem` bağlanır
- `CustomerLoyaltySystem` bağlanır
- `TaxPeriodSystem` decision UI alır
- `CreditSystem` repayment/event pressure kazanır

Çıktı:

- ekonomi daha yaşayan, daha uzun vadeli olur

## Sprint 3: Rival ve Legal Pressure

- poaching responses tamamlanır
- denetim / lawsuit / compliance event zincirleri açılır
- rival intent UI güçlendirilir

Çıktı:

- rakip ve hukuk baskısı gerçek bir ikinci oyuncu gibi davranır

## Sprint 4: Venture Reality Pass

- Fast Food, Cafe, Grocery launch venture’ları derinleştirilir
- Clothing ve Tech daha sonra Phase 2 olarak ele alınır

Çıktı:

- AGENTS.md’deki Phase 1 önceliği ile kod hizalanır

---

## 11. Karar

Projede "temel oyun yok" durumu yok. Temel oyun var. Asıl problem:

- source of truth dağınık
- bazı sistemler canlı resolve loop’una tam bağlanmamış
- bazı dosyalar eski business modelini hâlâ taşıyor
- derin simülasyon katmanları AGENTS vizyonunun gerisinde

Bu yüzden doğru strateji yeni feature spam’i değil; önce:

1. truth cleanup
2. economy wiring
3. rival/legal pressure completion
4. Phase 1 venture derinleştirme

olmalı.

---

## 12. Kısa Sonuç

Bugünkü kod tabanı şu cümleyle özetlenebilir:

> "Venture-first kart strateji omurgası kuruldu; şimdi bunu gerçek işletme simülasyonuna dönüştürecek ikinci katmanı tamamlamamız gerekiyor."

Bu ikinci katmanın merkezi de ekonomidir. EconomyManager doğru çekirdeği kurmuş; eksik olan şey formüllerin etrafındaki yaşayan sistemlerdir:

- enflasyon
- sadakat
- vergi baskısı
- kredi stresi
- stok ve supplier güvenilirliği
- personel kararı ve rakip müdahalesi

Bu tamamlandığında oyun sadece kart yerleştirme değil, gerçekten işletme yönetme hissi verecek.
