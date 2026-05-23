# TECHNICAL MAPPING
# "Empire of Cards" v5

> Versiyon: 2.0 | Tarih: 2026-05-23
> Amaç: GDD v5 question-driven live board modelini mevcut Unity omurgasına minimum yıkımla eşlemek

---

## 1. Hedef

Mevcut Unity omurgasını koruyarak tasarım odağını `visible slot placement` yaklaşımından `question-driven live board` modeline geçirmek.

Korunacak omurga:

- `VentureType`
- `SlotType`
- `CardData`
- `TurnManager` ve turn phase akışı
- `EventBus`
- venture selection akışı

Yeniden yorumlanacak alanlar:

- board'un görsel görevi
- slotların oyuncuya sunuluş biçimi
- kart yerleştirme mantığı
- turn brief ve question çözümü
- customer movement presentation

---

## 2. Tasarım Kavramı -> Teknik Kavram Eşlemesi

## 2.1 SlotType Korunur, Sunum Değişir

`SlotType` enum teknik olarak kalır:

- `Operation`
- `Staff`
- `Marketing`
- `Supplier`
- `TempEffect`

Yeni yorum:

- oyuncu visible slot box görmez
- runtime bu enum'ları `BusinessAnchorDefinition` ile sahnedeki gerçek işletme noktalarına map eder
- aynı enum hem build placement hem temp effect organize etmek için kullanılır

## 2.2 Question Panel Runtime Varlığı

Sorular yeni runtime kavramı olarak ele alınır.

Önerilen veri yapıları:

- `QuestionDefinition`
- `QuestionRuntimeState`
- `QuestionOutcomeRule`

`QuestionDefinition` alanları:

- `string questionId`
- `VentureType ventureType`
- `string headline`
- `string detail`
- `string primaryTag`
- `string optionalSupportTag`
- `string[] allowedFamilies`
- `string[] riskWarnings`
- `string[] followUpQuestionIds`
- `string[] resultingTempEffectIds`

`QuestionRuntimeState` alanları:

- `string questionId`
- `int spawnedTurn`
- `bool isForced`
- `CardData committedPrimaryCard`
- `CardData committedSupportCard`
- `QuestionResolutionState resolutionState`
- `string generatedFollowUpId`

## 2.3 Business Anchor Tanımı

Görsel slot yerine anchor kullanılmalı.

Önerilen veri yapısı:

- `BusinessAnchorDefinition`

Alanlar:

- `string anchorId`
- `SlotType slotType`
- `string displayLabel`
- `string ventureSpecificRole`
- `Vector3 localPosition`
- `bool acceptsPersistentBuild`
- `bool acceptsTempEffect`
- `string[] preferredTags`

Bu veri `VentureBoardProfile` ya da benzeri bir board profilinde tutulmalı.

---

## 3. CardData ve Kart Çözümleme

## 3.1 CardData Yeni Kuralları

`CardData` yapısı tamamen yıkılmadan genişletilir.

Zorunlu yorum değişiklikleri:

- `targetSlotType` artık visible slot hedefi değil, anchor family hedefidir
- `targetSubSlotId` yerine ya da yanında `anchorId` kullanımı açılmalıdır
- her kart tag tabanlı eşleşme taşımalıdır
- her kartın build mi response mu risk/opportunity mi olduğu net olmalıdır

Önerilen ek alanlar:

- `CardFamily cardFamily`
- `string[] primaryTags`
- `string[] secondaryTags`
- `bool isPersistentBuild`
- `bool canAnswerQuestion`
- `bool canBecomeTempEffect`
- `string[] ventureAffinity`
- `int payrollCost`
- `int upfrontCost`
- `float delayedRiskWeight`

## 3.2 Dual Placement Davranışı

Bir el kartı 4 sonuca gidebilir:

- `question queue`
- `business anchor`
- `temp effect strip`
- `discard/history`

Bu yüzden drop doğrulama yalnızca `SlotType` ile değil, `placement context` ile yapılmalıdır.

Önerilen context enum:

- `QuestionPanel`
- `BusinessAnchor`
- `TempEffectStrip`

## 3.3 Kart Doğrulama Akışı

Soru paneli için doğrulama:

- kart `canAnswerQuestion` mı
- primary tag eşleşiyor mu
- support rolü destek etiketiyle uyumlu mu
- riskli ama legal çözümler kabul ediliyor mu

Business anchor için doğrulama:

- kart `isPersistentBuild` mi
- anchor family eşleşiyor mu
- venture-specific role uyumlu mu
- unlock koşulu sağlanıyor mu

---

## 4. Turn Flow Eşlemesi

## 4.1 Mevcut Fazlar Korunur

Mevcut turn phase akışı korunabilir:

- Draw
- Planning
- Play
- Resolve
- CrisisReaction
- Rival
- MarketUpdate

Yeni yorum:

- `Planning`: brief + question spawn
- `Play`: question placement + build placement
- `Resolve`: question outcome + economy + customer flow
- `CrisisReaction`: forced question ve temp effect zincirleri
- `Rival`: visible rival pressure

## 4.2 Turn Brief

`TurnBriefData` genişletilebilir ya da yanında yeni payload taşınabilir.

Ek ihtiyaçlar:

- aktif soru başlıkları
- önerilen baskı etiketi
- build ihtiyacı
- rival threat summary

## 4.3 Decision Record

Turn memory için veri modeli gerekir.

Önerilen yapı:

- `DecisionRecord`

Alanlar:

- `int turnNumber`
- `string questionId`
- `string questionHeadline`
- `string primaryCardId`
- `string supportCardId`
- `string buildCardId`
- `string outcomeLabel`
- `string[] carriedEffects`

Bu kayıt UI decision ledger tarafından tüketilir.

---

## 5. Board State ve Ekonomi Çözümü

## 5.1 Core State

Ana runtime state görünür metrikleri taşımalıdır:

- `cash`
- `customerPull`
- `serviceCapacity`
- `quality`
- `reputation`
- `staffStability`
- `legalRisk`
- `marketShare`

## 5.2 Support Metrics

Venture-specific support metrics ayrı paketlerde tutulmalıdır.

Örnek:

- fast food/cafe: `ingredientQuality`, `hygiene`, `serviceSpeed`
- tech app: `productStability`, `churn`, `serverLoad`
- giyim: `stockFreshness`, `seasonalFit`, `returnPressure`
- market: `spoilagePressure`, `shelfHealth`, `localCreditLoad`

## 5.3 Hidden Accumulation Pressures

Bu alanlar görünür panelde özetlenebilir ama doğrudan ana HUD'a yüklenmez:

- burnout buildup
- review instability
- inspection readiness
- supplier reliability drift
- rival pressure bank

## 5.4 Resolve Sırası

Önerilen çözüm sırası:

1. queued question sonuçları
2. persistent build etkileri
3. staff ve operation kapasitesi
4. supplier ve quality etkileri
5. demand/pull hesabı
6. trust/reputation hesabı
7. cashflow
8. risk accumulation
9. customer flow snapshot
10. market share update

## 5.5 TurnResolutionReport

Turn sonu için explicit rapor yapısı gerekir.

Önerilen alanlar:

- `int turnNumber`
- `int cashDelta`
- `float pullDelta`
- `float reputationDelta`
- `float riskDelta`
- `float marketShareDelta`
- `int customersToPlayer`
- `int customersToRival`
- `string[] reasons`
- `DecisionRecord[] records`

---

## 6. Customer Movement Sunumu

## 6.1 Presentation Model

Customer movement gerçek simülasyon değil, simülasyonu görünür kılan presentation katmanıdır.

Önerilen yapı:

- `CustomerFlowSnapshot`

Alanlar:

- `int neutralCount`
- `int movedToPlayer`
- `int movedToRival`
- `int loyalPlayerCount`
- `int loyalRivalCount`
- `string dominantReason`

## 6.2 Hesap ve Sunum Ayrımı

Market share ve trust ekonomiden hesaplanır.

Customer movement:

- bu sonucu sahneye çevirir
- aynı tur neden-sonuç ilişkisini görünür kılar
- gameplay logic için tek kaynak olmaz

---

## 7. UI ve Input Eşlemesi

## 7.1 InputManager3D

Mevcut `InputManager3D` şu yeni drop hedeflerini bilmelidir:

- question panel collider / target
- business anchor collider / target
- temp effect target

`SlotZone3D` mantığı ya yeniden yorumlanmalı ya da adapter ile anchor temsiline çevrilmelidir.

## 7.2 Hand3D

El kartları şu state'leri taşımalıdır:

- `InHand`
- `QueuedToQuestion`
- `CommittedToBuild`
- `ConvertedToTempEffect`
- `Discarded`

## 7.3 Side Panels

UIManager ve ilgili paneller şu yeni verileri göstermelidir:

- current turn question list
- decision history
- last turn report
- rival response summary
- active effect strip

---

## 8. Rival AI Eşlemesi

## 8.1 Rakip Yalnızca Delta Basmaz

Rival AI output'u sayısal etki yanında sahne baskısı da üretmelidir.

Yeni görünür sonuç örnekleri:

- campaign prop active
- customer tug-of-war bias
- poaching threat badge
- supplier pressure alert

## 8.2 Question Kaynağı Olarak Rakip

Rakip hamlesi yeni soru da doğurabilir.

Örnek:

- "Rakip indirim açtı, fiyatı mı güveni mi savunacaksın?"
- "Rakip ustanı yokluyor, maaş mı sadakat mi?"

---

## 9. İçerik Profilleri

## 9.1 Venture Docs ile Teknik Profil Ayrımı

Venture tasarım dokümanları:

- question family
- pressure identity
- example flows

Teknik profiller:

- board anchors
- deck pools
- economy coefficients
- question pools

## 9.2 Gerekli Yeni Profil Tipleri

- `QuestionPoolProfile`
- `BusinessAnchorProfile`
- `CustomerFlowProfile`
- `RivalPressureProfile`

Var olan venture playbook yapıları bunlara genişletilebilir.

---

## 10. Kabul Kriterleri

- `SlotType` teknikte kalır ama oyuncuya visible slot box olarak görünmez
- sorular runtime entity olarak tanımlanır
- kartlar dual placement davranışına sahip olur
- turn report ve decision ledger veri modeli açıkça tanımlanır
- customer movement presentation, market share hesabından beslenir
- rival baskısı board üzerinde görünür hale gelir

Bu eşleme sağlanırsa v5 tasarım yönü mevcut Unity omurgasıyla uyumlu ilerleyebilir.
