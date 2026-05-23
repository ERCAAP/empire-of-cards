# Empire of Cards UI/UX Design

> UI destekleyici katmandır; asıl bilgi yaşayan diorama'da görünmelidir.

## 1. UX Goals

- Oyuncu her tur ana darboğazı hızlı okumalı.
- Kart seçimi stratejik ama ağır menü gibi hissettirmemeli.
- Slot doluysa karar net olmalı: replace, upgrade, merge, discard/sell.
- Event seçimleri dramatik, kısa ve sonuç odaklı olmalı.
- Kritik stat değişimleri hem UI'da hem sahada görünmeli.

## 2. Screen Flow

```text
Boot
-> Main Menu
-> Venture Select
-> Run Setup
-> Gameplay
   -> Card Offer
   -> Card Inspect
   -> Slot Commit / Replacement
   -> Resolve
   -> Event Choice
   -> End Turn Summary
-> Run End
-> Exit / Meta Progress
```

## 3. Venture Select

Amaç: oyuncunun sektör seçiminin gameplay anlamını göstermesi.

Her venture kartı gösterir:

- Venture adı.
- Ana oyun baskısı.
- Başlangıç zorluğu.
- Kart dili.
- Rakip davranış önizlemesi.

Örnek:

- Fast Food: yüksek demand, capacity krizi.
- Cafe: kalite/sadakat, yavaş büyüme.
- Tech App: growth/stability/churn.
- Giyim: stok/sezon/influencer.
- Market: sadakat/nakit/fire.

## 4. Gameplay HUD

HUD minimum ama sürekli okunur olmalı.

Üst veya yan bant:

- Cash
- Rating
- Staff Stability
- Legal Risk
- Market Share
- Turn
- Current Pressure

Kural:

- HUD sahayı kapatmaz.
- Değer değişince kısa pulse olur.
- Büyük değişimler world label ile de görünür.

## 5. Card Offer UX

Kart offer bandı ekranın alt bölümünde olur.

Card offer ayrı bir ekran değildir. Gameplay sahnesi canlı kalır; kamera `BoardOverview`dan hafif `PlayerBusinessFocus` hissine kayabilir. Simülasyon `slow tactical time` moduna iner, dünya tamamen donmaz.

Kart üzerinde:

- Ad
- Venture icon
- Behavior type icon
- Cost
- Slot target
- Persistence marker
- Kısa tradeoff

Kart renk dili:

- Install: solid frame.
- Burst: bright edge.
- Policy: rule/icon marker.
- Risk: glitch/red accent.
- Reaction: alert/choice style.

## 6. Card Inspect UX

Hover/selection sırasında:

- Target slot highlight olur.
- Sahadaki ghost preview görünür.
- Stat etkileri küçük ikonla gösterilir.
- Risk tag varsa kısa uyarı çıkar.
- Kamera `CardInspect` davranışıyla 0.25-0.45 sn içinde hedefe hafif yaklaşır.
- Inspect bittiğinde kamera sert kesmeden BoardOverview'a döner.

Örnek:

`Ek Garson` inspect:

- Staff slotu parlar.
- Kapıdan girecek garson ghost'u görünür.
- Service capacity + ve wage cost + ikonları belirir.

## 7. Slot Placement UX

### 7.1 Empty Slot

Kart sürüklenir veya seçilip hedef slota commit edilir.

Feedback:

- Slot glow.
- Kart snap.
- World manifestation.
- Small stat pulse.
- Kamera `SlotPlacement` davranışıyla kart ve linked world target'ı aynı beat içinde gösterir.

### 7.2 Full Slot

Slot doluysa replacement panel çıkar. Panel küçük ve context içi olmalı.

Seçenekler:

- Replace
- Upgrade
- Merge
- Discard / Sell

Her seçenek net consequence gösterir:

- Capacity change.
- Wage/cash change.
- Staff stability/reputation risk.
- Linked world actor/object sonucu.

### 7.3 Linked Hover

Slot kartı hover:

- Sahadaki obje/NPC highlight.
- Eğer kart stress/durability taşıyorsa küçük durum chip'i.

Sahadaki obje/NPC hover:

- Bağlı kart highlight.
- İlgili stat tooltip.

## 8. Event Choice UX

Event paneli mikro-sinematik odağın yanında veya alt bandında çıkar.

Panel yapısı:

- Problem başlığı.
- 1 kısa neden cümlesi.
- 1-3 seçim.
- Her seçimde tradeoff label.

Kural:

- Uzun metin yok.
- Seçim sonucu tam sayı listesi gibi değil, niyet ve risk olarak gösterilir.
- Seçim sonrası panel hemen kapanır, consequence dünyada oynar.

## 9. End Turn Summary

End turn summary kısa olmalı.

Gösterir:

- Market share delta.
- Rating delta.
- Cash delta.
- Ana yeni pressure.
- Rakip hamlesi özeti.

Bu ekran modal gibi uzun kalmamalı; oyuncu dünyaya geri dönmelidir.

## 9.1 Resolve UX

Kart commit sonrası 6-12 saniyelik resolve penceresi çalışır.

Bu sırada:

- Card offer bandı pasifleşir veya kapanır.
- Kamera BoardOverview'da kalır; büyük market shift varsa MarketFlow'a geçer.
- Customer flow, queue, staff stress, rating ve market share feedbackleri dünyada oynar.
- Oyuncuya uzun tablo verilmez; ana değişimler world label ve kısa HUD pulse ile gösterilir.

## 10. Exit / Meta Screen

Holding exit event'i sonrası gelir.

Gösterir:

- Final market share.
- Holding value.
- Exit reward.
- Yeni startup avantajları.
- Unlock edilen venture/meta bonus.

Görsel dil: Toy Diorama sahne genişler, marka tabelası büyür, sonra yeni startup seçimine geçilir.

## 11. UI Technology Direction

İlk öneri: UGUI.

Sebep:

- Hızlı prototip.
- DOTween ile placement/pulse animasyonları kolay.
- Event paneli ve card offer hızlı kurulabilir.
- Mevcut paketlerle uyumlu.

UI Toolkit production polish için sonra değerlendirilebilir; ilk hedef oynanabilir core loop'tur.

## 12. Readability Rules

- UI hiçbir zaman yaşayan sahayı tamamen kapatmaz.
- Kart seçimi sırasında hedef slot ve dünya karşılığı birlikte görünür.
- Event sırasında problem kaynağı kamerada görünür.
- En önemli pressure tek cümleyle gösterilir.
- Renkler gameplay anlamı taşır: gri/mavi/kırmızı müşteri renkleri UI renkleriyle çelişmemelidir.
- Text küçük objelerin üstüne binmemelidir; world labels kısa ve geçici olmalıdır.

## 13. UX Acceptance Criteria

- Oyuncu kartı oynamadan önce nereye gideceğini ve dünyada neye dönüşeceğini görür.
- Slot doluyken bir sonraki adım belirsiz kalmaz.
- Event seçiminde oyuncu neden karar verdiğini ve sonucu ne bekleyeceğini anlar.
- End turn sonunda oyuncu bir sonraki turun ana problemini bilir.
- UI, Toy Diorama hissini bozacak kadar ağır veya kurumsal görünmez.

## 14. Locked Screen Composition

Gönderilen görsel referanslara göre gameplay UI şu yerleşimi kullanır:

- Sol dikey HUD: ekran genişliğinin yaklaşık yüzde 10-14'ü.
- Merkez dünya: ekranın ana alanı; district ve müşteri flow kapatılmaz.
- Alt card hand: ekran yüksekliğinin yaklaşık yüzde 20-28'i.
- Slot board: alt bandın içinde veya hemen üstünde, player business ile fiziksel ilişkili.
- Sağ panel: ekran genişliğinin yaklaşık yüzde 16-24'ü; simulation phase, event veya consequence için kullanılır.

Sol HUD içerikleri:

- Cash
- Rating
- Demand veya traffic pressure
- Staff stability
- Legal risk
- Market share

Sağ panel modları:

- `SimulationLoop`: turn phase listesi ve aktif faz vurgusu.
- `EventChoice`: problem başlığı, 1 kısa neden, 1-3 seçim.
- `VisibleConsequence`: seçimin sahadaki sonucu ve yeni pressure.
- `EndTurnSummary`: market share, rating, cash ve ana pressure delta.

## 15. Card Drag And Slot Commit UX

Kart oynama fiziksel ama kısa olmalıdır:

1. Oyuncu kartı hover eder.
2. Kart yükselir, target slot parlar.
3. Sahada ghost preview belirir.
4. Oyuncu kartı slota sürükler veya seçip slota tıklar.
5. Slot uygunsa kart snap animasyonuyla oturur.
6. Linked world object/NPC/flow aynı anda spawn veya activate olur.
7. Kart-slot-world link çizgisi 0.8-1.2 sn görünür, sonra kaybolur.

Invalid feedback:

- Yanlış slotta kısa shake.
- Slot doluysa `Replace / Upgrade / Merge / Discard-Sell` paneli açılır.
- Cash yetmiyorsa kart karar olarak kalabilir ama commit kilitlenir.

## 16. Event Overlay UX Reference

Event overlay sağ paneli büyütür ama dünyayı kapatmaz.

Panel yapısı:

- Üst icon: event category.
- Başlık: örnek `Garsonun Ayrılmak İstemesi`.
- Choice area: maksimum 3 seçenek.
- Consequence area: seçim sonrası görsel sonuç açıklaması.

World inset sadece gerektiğinde kullanılır. Öncelik ana sahada NPC beat'i göstermektir; inset panel, problem kaynağı kamera açısından küçük kalıyorsa kullanılır.
