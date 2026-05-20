# TECHNICAL MAPPING
# "Empire of Cards" v4

> Versiyon: 1.0 | Tarih: 2026-05-20
> Amaç: GDD v4 kurallarını mevcut Unity omurgasına çarpmadan uygulama planı

---

## 1. Hedef

Mevcut Unity veri modelini tamamen yıkmadan, venture-first slot simülasyonuna geçirmek.

Korunacak mevcut omurga:

- `VentureType`
- `SlotType`
- `CardData`
- venture selection akışı
- turn phase akışı

Yeniden yorumlanacak alanlar:

- business/employee/action/upgrade/event kart anlamları
- slotlara bağlanma kuralları
- board state çözümü
- rival davranışı

---

## 2. Yeni veya Yeniden Tanımlanacak Arayüzler

## 2.1 VentureBoardProfile

Amaç:

- venture'ın alt-slot yerleşimini
- açılış slot sayılarını
- slot upgrade eşiklerini
- görsel masa bağlarını

tek veri noktasında toplamak

Önerilen alanlar:

- `VentureType ventureType`
- `string displayName`
- `BoardSubSlotDefinition[] operationSubSlots`
- `BoardSubSlotDefinition[] staffSubSlots`
- `BoardSubSlotDefinition[] marketingSubSlots`
- `BoardSubSlotDefinition[] supplierSubSlots`
- `int startingOperationSlots`
- `int startingStaffSlots`
- `int startingMarketingSlots`
- `int startingSupplierSlots`
- `int maxOperationSlots`
- `int maxStaffSlots`
- `int maxMarketingSlots`
- `int maxSupplierSlots`
- `SlotUnlockStep[] unlockSteps`

## 2.2 VentureDeckProfile

Amaç:

- starter deck
- venture kart havuzu
- neutral kart havuzu
- crisis havuzu
- oyun evresine göre unlock mantığı

tek yerde toplansın

Önerilen alanlar:

- `VentureType ventureType`
- `string[] starterCardIds`
- `string[] earlyPoolCardIds`
- `string[] midPoolCardIds`
- `string[] latePoolCardIds`
- `string[] neutralCardIds`
- `string[] crisisCardIds`
- `DeckBiasRule[] drawBiasRules`

## 2.3 VentureEconomyProfile

Amaç:

- venture'a özel kalite, rating, demand ve cost katsayılarını taşımak

Önerilen alanlar:

- `VentureType ventureType`
- `float baseDemand`
- `float capacityToDemandPenalty`
- `float qualityToRatingWeight`
- `float ratingToOrganicDemandWeight`
- `float staffInstabilityPenalty`
- `float legalRiskTriggerWeight`
- `float marketShareGainWeight`
- `DerivedMetricRule[] derivedMetrics`

## 2.4 CardData Ek Kuralları

`CardData` tamamen değişmek zorunda değil ama şu yorumlar zorunlu hale gelmeli:

- her kartın `ventureType` aidiyeti net olmalı
- `targetSlotType` aktif olarak kullanılmalı
- alt-slot uyum bilgisi tanımlanmalı
- kartın aile tipi belirgin olmalı:
  - `Setup`
  - `Growth`
  - `Risk`
  - `Reaction`
  - `Crisis`

Önerilen yeni alanlar:

- `string targetSubSlotId`
- `CardFamily cardFamily`
- `bool entersTempEffectOnUse`
- `string[] crisisTags`
- `string[] solutionTags`
- `BoardPressureType[] preferredWhenBoardState`

---

## 3. Runtime Sistem Eşlemesi

## 3.1 Board State

Bugünkü yapıda business-centric çözüm var. v4'te venture board state çözümü gerekir.

Yeni runtime veri ihtiyacı:

- `currentCash`
- `currentDemand`
- `currentCapacity`
- `currentQuality`
- `currentRating`
- `currentStaffStability`
- `currentLegalRisk`
- `currentMarketShare`

venture özel derived alanlar:

- fast food/cafe: `ingredientQuality`, `serviceSpeed`, `hygiene`
- tech: `stability`, `churn`, `infraCost`
- giyim: `stockHealth`, `seasonFit`, `returnPressure`
- market: `spoilagePressure`, `sktPressure`, `creditLedger`, `localLoyalty`

## 3.2 Slot Çözümü

Mevcut `SlotType` enum korunur.

Yeni yorum:

- `Operation`: venture'ın çekirdek üretim ve kapasite omurgası
- `Staff`: bu omurgayı canlı tutan insan gücü
- `Marketing`: demand iten aktif baskı
- `Supplier`: quality ve cost eğrisi
- `TempEffect`: krizler ve geçici durumlar

Alt-slotlar enum ile değil profil verisiyle çözülmeli. Böylece teknik omurga sabit kalır, venture özelleşmesi veriden gelir.

## 3.3 Çekme Mantığı

Kart çekimi salt rarity bazlı olmamalı.

Yeni bias kuralları:

- demand düşükse growth/setup ağırlığı artar
- capacity demand'in altında kaldıysa operation/staff ağırlığı artar
- rating düştüyse reaction/quality kartları öne gelir
- legal risk yükseldiyse defensive/reaction kartları öne gelir
- geç oyunda scale kartları açılır

Bu mantık `VentureDeckProfile` + board snapshot ile çalışmalı.

---

## 4. UI ve Sunum Eşlemesi

## 4.1 Venture Selection

Mevcut venture selection akışı korunur.

Geliştirme:

- venture kartında alt-slot kimliği kısa metinle gösterilir
- her venture için "oyun tarzı" etiketi eklenir
- rakibin aynı venture başlayacağı açıkça belirtilir

## 4.2 Board UI

Yeni ihtiyaçlar:

- ortak slot ailesi görünür kalmalı
- venture alt-slot etiketleri değişebilir olmalı
- District Zone demand/traffic/rating baskısını gösterebilmeli
- Temp Effect slotları aktif krizleri okunur göstermeli

## 4.3 Göstergeler

Top bar veya yan panel şu statları zorunlu taşır:

- Cash
- Demand
- Capacity
- Quality
- Rating
- Staff Stability
- Legal Risk
- Market Share

venture bazlı ikinci sıra göstergeler context panelde açılır.

---

## 5. Rival AI Eşlemesi

## 5.1 Sabit Kural

Rakip oyuncuyla aynı venture içinden kart çeker.

## 5.2 Davranış Profili

Rival AI her tur şu soruları cevaplar:

- demand mi artırmalı?
- kapasite mi toparlamalı?
- rating mi korumalı?
- riskli kısa yola mı başvurmalı?
- oyuncunun güçlü tarafına karşı hangi karşı baskıyı kurmalı?

## 5.3 Venture Bazlı Öncelikler

- Fast food: fiyat savaşı, delivery baskısı, yorum savaşı
- Cafe: Google Maps ve barista baskısı
- Tech: ASO, paid growth, crash sonrası hızlı toparlama
- Giyim: indirim ve vitrin baskısı
- Market: yakınlık, gece servis ve fiyat baskısı

## 5.4 Okunabilirlik

Rakip kararları tam gizlenmemeli. UI oyuncuya şunu göstermeli:

- rakip bugün quality oynadı
- rakip marketing blitz açtı
- rakip supplier maliyete düştü

---

## 6. İçerik Uygulama Sırası

## 6.1 Faz 1

- `GDD.md` v4
- 5 venture dokümanı v4
- teknik eşleme notu

## 6.2 Faz 2

- `VentureBoardProfile`
- `VentureDeckProfile`
- `VentureEconomyProfile`
- `CardData` yorum ve alan güncellemeleri

## 6.3 Faz 3

- board state çözümleyici
- draw bias sistemi
- rating/quality/capacity zinciri
- venture bazlı crisis tetikleyici

## 6.4 Faz 4

- rival AI refactor
- District Zone okunabilirliği
- venture özel UI etiketleri

---

## 7. Kabul Kriterleri

- Teknik omurga `VentureType` ve `SlotType` ile uyumlu kalır.
- Alt-slot farklılığı veri odaklı çözülür, enum patlaması yaratılmaz.
- Kartların venture aidiyeti ve hedef slot mantığı netleşir.
- Draw sistemi board-state aware hale gelir.
- Rakip aynı venture içinde anlamlı stratejiler üretebilir.

---

## 8. Açık Not

Bu belge uygulama kararlarını sabitler ama henüz kod patch'i değildir. Bir sonraki uygulama fazında veri profilleri ve runtime çözümleyiciler eklenirken bu belge referans kabul edilir.
