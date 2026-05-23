# Empire of Cards Camera, Lighting and Turn Flow

> Net karar: sabit izometrik Toy Diorama kamera, canlı akan simülasyon, Toy Studio ışık. Serbest orbit yok.

## 1. Purpose

Bu doküman oyunun oynanırken nasıl görüneceğini ve kameranın tur boyunca nasıl davranacağını kesinleştirir.

Oyuncu her an şunları okuyabilmelidir:

- Board'da Player Business, District ve Rival nerede?
- Kart seçimi hangi slota ve dünyadaki hangi objeye/NPC'ye bağlanıyor?
- Resolve sırasında müşteri, çalışan, rating ve market share nasıl değişti?
- Event problemi nerede doğdu ve seçimden sonra sahada ne oldu?

## 2. Default Board View

Default görünüm `BoardOverview` kamerasıdır.

Kompozisyon:

```text
Top third    : Rival Business, red customer pull
Middle third : District / Market, gray customers and flow paths
Bottom third : Player Business + Business Control Board
Bottom UI    : Card Offer Band when active
```

Kamera:

- Projection: orthographic.
- Pitch: 35-45 derece.
- Yaw: 35-45 derece.
- Roll: 0.
- Default orthographic size: tüm board'u gösterecek kadar geniş.
- Player business bottom-safe-area dışında kalmayacak.
- Card offer bandı açıldığında kamera hafif yukarı framing yapabilir, ama sahneyi koparmaz.

## 3. Camera Control Rule

Oyuncu serbest kamera döndürmez.

İzin verilen kontrol:

- Mouse hover / selection.
- Optional zoom in/out: sınırlı 2 kademe.
- Optional focus shortcut: Player, District, Rival hazır focus noktaları.

MVP varsayımı:

- Serbest orbit yok.
- Kamera node ve virtual camera geçişleri sistem tarafından yönetilir.
- Okunabilirlik, oyuncu kontrolünden daha önemlidir.

## 4. Virtual Cameras

| Camera | Kullanım | Açı / Framing | Blend |
|---|---|---|---|
| BoardOverview | Normal oyun | Tüm board, izometrik | default |
| PlayerBusinessFocus | Kart seçimi ve player okumaları | Player alt bölge biraz yakın | 0.35-0.6 sn |
| CardInspect | Hover/inspect | Hedef slot + ghost preview | 0.25-0.45 sn |
| SlotPlacement | Kart commit | Slot + linked world target | 0.6-1.0 sn |
| EventFocus | Kriz/problem | Problem anchor yakın plan | 0.4-0.8 sn |
| MarketFlow | Büyük market share kayması | District üst/orta görünüm | 0.8-1.2 sn |
| RivalReaction | Rakip hamlesi | Rival business + red flow | 0.6-1.0 sn |
| ScaleView | Scale/holding geçişi | Daha geniş board görünümü | 1.0-1.5 sn |

Kamera priority:

1. Critical player event.
2. Active player card placement.
3. Major market share shift.
4. Rival major action.
5. Ambient events.

Aynı turda maksimum 2 güçlü kamera kesmesi yapılmalıdır. Diğer feedback world label veya VFX ile çözülür.

## 5. Turn Flow With Camera

### 5.1 Planning

- Camera: `BoardOverview`.
- Simulation speed: 1.0 veya 0.75.
- UI: HUD açık, card offer kapalı.
- Dünya: müşteriler yürür, çalışanlar loop animasyon yapar, kuyruklar canlı kalır.

Amaç: oyuncu işletme darboğazını sahadan okur.

### 5.2 Card Offer

- Camera: `BoardOverview`, gerekirse hafif `PlayerBusinessFocus`.
- Simulation speed: `slow tactical time` 0.35-0.5.
- UI: alt card offer bandı açılır.
- Dünya: tamamen donmaz, ama baskı artışı yavaşlar.

Kart seçim ekranı ayrı bir sahne değildir. Gameplay sahnesinin üstünde alt banttır.

### 5.3 Card Inspect

- Camera: `CardInspect` veya BoardOverview üzerinde hafif zoom.
- Simulation speed: 0.25-0.35.
- UI: hedef slot, kart tradeoff, risk tag görünür.
- Dünya: ghost preview gösterir.

Örnek:

- `Ek Garson` hover edilir.
- Staff slotu mavi parlar.
- Kapı girişinde ghost garson görünür.
- Mevcut kuyruk hattı hafif highlight olur.

### 5.4 Slot Commit

- Camera: `SlotPlacement`.
- Simulation speed: kısa süre 0.2 veya pause.
- UI: kart slota snap eder.
- Dünya: linked NPC/obje/flow oluşur.

Süre:

- Küçük kart commit: 0.6 sn.
- Önemli install: 1.0 sn.
- Replace/merge gibi kararlar: 1.0-1.5 sn consequence beat.

### 5.5 Resolve

- Camera: `BoardOverview`.
- Major market change varsa `MarketFlow`.
- Simulation speed: 1.0.
- Süre: 6-12 sn.

Resolve sırasında sırasıyla:

1. Müşteri flow güncellenir.
2. Queue/delay görünür.
3. Staff stress pulse oynar.
4. Rating/review balonları çıkar.
5. Market share renk kayması olur.
6. Yeni pressure label belirir.

### 5.6 Event

- Camera: `EventFocus`.
- Simulation speed: 0 veya 0.1.
- UI: event choice paneli problem anchor yakınında veya alt bantta.
- Lighting: lokal crisis highlight.

Event seçiminden sonra:

- Panel kapanır.
- Consequence sahada oynar.
- Kamera 0.3-0.6 sn içinde BoardOverview'a döner veya MarketFlow'a geçer.

### 5.7 Rival Reaction

- Camera: `RivalReaction`.
- Simulation speed: 0.75-1.0.
- UI: kısa rival action label.
- Dünya: kırmızı tabela, kırmızı queue veya red flow görünür.

Eğer rival hamlesi müşteri kaydırıyorsa:

- RivalReaction -> MarketFlow.
- Red customer pull 1-2 sn görünür.

### 5.8 End Turn

- Camera: `BoardOverview`.
- UI: compact end turn summary.
- Simulation speed: 0.75.
- Dünya: next pressure world label kalır.

Amaç: oyuncu bir sonraki turun ana problemini bilir.

## 6. Lighting Model

Işık modeli `Toy Studio`.

### 6.1 Base Light

- Sıcak ana directional light.
- Yumuşak gölge.
- Ambient light açık ve temiz.
- Kontrast yüksek ama karanlık alan yok.
- Board'un hiçbir bölgesi bilgi kaybettirecek kadar karanlık olmaz.

### 6.2 Accent Lights

- Player side: blue accent.
- Rival side: red accent.
- Neutral district: warm gray/cream ambient.

Accent ışıklar müşteri renkleriyle çelişmemelidir. Mavi müşteri ve kırmızı müşteri her zaman okunmalıdır.

### 6.3 Crisis Lighting

Krizlerde global ışık değişmez. Lokal vurgu kullanılır:

- Staff stress: kırmızı rim/ring.
- Legal risk: soğuk kırmızı/mor marker.
- Rating damage: yıldız çatlama ve kırmızı review burst.
- Rating recovery: sıcak altın pulse.
- Cash crisis: kasa çevresinde sarı/kırmızı uyarı pulse.

### 6.4 No Day/Night Loop in MVP

MVP'de gün-gece döngüsü yoktur.

Sebep:

- Okunabilirlik riskini azaltır.
- Müşteri renklerini korur.
- Kamera ve UI testlerini basitleştirir.

Gece veya zaman hissi sadece belirli kart/eventlerde lokal olarak kullanılır. Örnek: Market `Gece Açık` policy kartı işletme çevresinde küçük gece ışığı yaratabilir, ama tüm board kararmamalıdır.

## 7. Safe Frame and UI Zones

Ekran alanı:

- Top safe: HUD ve rival labels.
- Center safe: district customer flow.
- Bottom world safe: player business ve slot board.
- Bottom UI band: card offer, 20-28% ekran yüksekliği.
- Event panel: problem anchor yakınında veya alt bandın üstünde.

Kural:

- Card offer bandı district müşteri akışını tamamen kapatmamalı.
- Slot board player business ile ilişkili kalmalı.
- World labels 1-2 sn sonra kaybolmalı.
- Event paneli problem kaynağını kapatmamalı.

## 8. Venture Camera Anchors

### 8.1 Fast Food

- `KitchenAnchor`: mutfak kaosu, ızgara, hijyen.
- `CounterAnchor`: kasa kuyruğu, müşteri şikayeti.
- `DeliveryAnchor`: paket yığılması, kurye krizi.
- `DiningAnchor`: masa/oturma memnuniyeti.

### 8.2 Cafe

- `BarAnchor`: barista, espresso, burnout.
- `SeatingAnchor`: masa bekleme, ambiyans.
- `TakeawayAnchor`: hızlı servis baskısı.
- `ViralCornerAnchor`: reels/Instagram eventleri.

### 8.3 Tech App

- `ProductDeskAnchor`: feature/UX kararları.
- `ServerAnchor`: crash, backend, cloud.
- `SupportAnchor`: ticket backlog.
- `StoreRatingAnchor`: app store rating ve review.

### 8.4 Giyim Mağazası

- `WindowAnchor`: vitrin, influencer çekimi.
- `RackAnchor`: stok/boş raf.
- `CheckoutAnchor`: kasa kuyruğu.
- `ReturnsAnchor`: iade baskısı.

### 8.5 Market / Bakkal

- `CheckoutAnchor`: kasa, veresiye, kuyruk.
- `ShelfAnchor`: raf/stok.
- `FreshProduceAnchor`: fire/SKT.
- `DeliveryAnchor`: WhatsApp/kurye.

## 9. Camera and Lighting Acceptance Criteria

- BoardOverview'da Player, District, Rival ve slot board aynı anda okunur.
- CardInspect oyuncuya kartın nereye gideceğini ve dünyada neye dönüşeceğini gösterir.
- SlotPlacement kartı ve linked world manifestation'ı aynı beat içinde gösterir.
- Resolve sırasında müşteri rengi, queue, staff stress ve rating sonucu fiziksel görünür.
- EventFocus problem kaynağını gösterir, panel olayı kapatmaz.
- Toy Studio ışık gri/mavi/kırmızı müşteri ayrımını bozmaz.
- Kriz vurgusu sahneyi karartmadan dikkat çeker.
