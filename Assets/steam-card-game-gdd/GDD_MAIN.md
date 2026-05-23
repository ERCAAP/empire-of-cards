# Empire of Cards

> GDD v5.0 | Unity 6 | PC / Steam | Stylized 3D business strategy

## 1. Creative North Star

Empire of Cards geleneksel bir card battler değildir. Kartlar saldırı, büyü veya soyut bonus değil; işletme kararlarıdır. Oyuncu kart seçer, ama asıl oyun bir işletme dioramasının nasıl tepki verdiğini okumaktır.

Ana fantezi:

> "Küçük bir startup kurdum, büyümenin bedellerini yönettim, rakipleri geçtim, holdinge dönüştüm, exit yaptım ve daha büyük bir imparatorluk için tekrar başladım."

Oyunun kalbi `decision -> visible world reaction -> systemic consequence -> new pressure` zinciridir. Oyuncu doğru karar verdiğinde pazarın kendi tarafına aktığını, yanlış karar verdiğinde işletmenin içeriden çatırdadığını görmelidir.

Net tür kararı:

> Empire of Cards klasik bir kart board oyunu değildir. Oyun, kart destekli 3D systemic business management oyunudur.

Kartlar seçimin ve kalıcı işletme mimarisinin görünür formudur. Board ise yaşayan işletme dünyasıdır. Kalıcı kartlar slotta görünür kalabilir, ama asıl değerleri sahada çalışan, masa, raf, backend node, kampanya akışı veya risk baskısı olarak görünmeleridir.

## 2. Design Pillars

1. Her karar dünyada görünür olmalı.
2. Kartlar karar arayüzüdür; oyun hissi 3D reactive business diorama'dan gelir.
3. Büyüme her zaman yeni baskı üretir.
4. Başarı sadece demand yaratmak değil, demand'i taşıyacak işletmeyi kurmaktır.
5. Krizler cezalandırıcı popup değil, dramatik ama sistemik dönemeçlerdir.
6. Rakip aynı pazarda fiziksel olarak hissedilir: müşteriler ona da akar, onun hamleleri district'te görünür.
7. Oyuncu işletmesine duygusal bağ kurmalıdır: çalışanlar, müşteriler, yorumlar, kuyruklar ve krizler isimli/okunur olmalıdır.

## 2.1 Visual Direction

Görsel yön `Toy Diorama` olarak kilitlidir.

Bu şu anlama gelir:

- İşletmeler maket gibi okunur.
- İnsanlar stylized, küçük ama büyük silhouette'li olur.
- Slot board ve kartlar fiziksel masa/diorama hissiyle uyumlu kalır.
- Gerçekçilikten önce okunabilirlik ve sistem feedback'i gelir.
- Placeholder assetlerle prototip yapılır, final kalite aynı Toy Diorama diline yükseltilir.

## 3. Player Promise

Oyuncu her tur şunu yaşar:

1. İşletmenin mevcut darboğazını okur.
2. Kartlar arasından bir karar seçer.
3. Kart sahaya fiziksel etki olarak iner.
4. Simülasyon demand, capacity, quality, rating, staff stability ve market share'i çözer.
5. Dünya tepki verir: müşteriler yön değiştirir, çalışanlar zorlanır, kuyruklar büyür, yorumlar düşer veya yükselir.
6. Event varsa kısa mikro-sinematik karar anı yaşanır.
7. Yeni baskı bir sonraki turun stratejik problemini oluşturur.

## 4. Core Loop

```text
Draw / Offer
-> Planning
-> Card Decision
-> Slot Commit
-> World Manifestation
-> Physical Board Effect
-> Simulation Resolve
-> Event / Crisis Reaction
-> Rival Response
-> Market Share Shift
-> End of Turn Pressure Preview
```

### 4.1 Draw / Offer

Oyuncuya her tur 3-5 karar kartı sunulur. Kart havuzu venture, mevcut board state ve baskılara göre filtrelenir. Amaç tamamen rastgele el değil, işletmenin gerçek durumuna anlamlı cevaplar üretmektir.

Örnek filtreler:

- Demand düşükse marketing ve visibility kartları artar.
- Demand capacity'yi geçtiyse staff/operation/triage kartları artar.
- Rating düşüyorsa apology, quality fix, review recovery kartları artar.
- Staff stability düşükse wage, schedule, conflict, hiring kartları artar.
- Legal risk yükseldiyse audit, cover-up, compliance veya scandal kartları havuza girer.

### 4.2 Planning

Oyuncu district'i ve işletmeyi okuyarak darboğazı belirler:

- Müşteri az mı?
- Müşteri çok ama servis yetişmiyor mu?
- Çalışanlar kopma noktasında mı?
- Kalite düşük olduğu için kötü yorum mu geliyor?
- Rakip daha fazla pazar mı çekiyor?

Bu fazda UI yardımcı olur ama cevap menüde değil sahada okunmalıdır.

### 4.3 Card Decision

Kart oynanınca etki tipine göre davranır:

- `Build`: sahaya yeni fiziksel obje, istasyon, masa, raf, backend node veya araç ekler.
- `Staff`: işletmeye çalışan NPC ekler veya mevcut çalışanı upgrade eder.
- `Marketing`: district'te görsel çekim alanı, müşteri akışı, reklam objesi veya sosyal buzz üretir.
- `Supplier`: kalite/maliyet davranışını değiştirir, ürün/servis animasyonunu etkiler.
- `Risk`: kısa vadeli boost verir, ileride event tetikleyebilecek risk tag'i bırakır.
- `Reaction`: aktif krizi çözer veya yönünü değiştirir.

Kart davranışının teknik omurgası beş ana tiptir:

- `Install`: slotta kalır ve dünyada obje/NPC/sistem açar.
- `Burst`: anlık çözülür, kısa dünya etkisi üretir.
- `Policy`: slotta kalır ve işletme davranış kuralını değiştirir.
- `Risk`: kısa vadeli avantaj verir, risk tag'i veya marker bırakır.
- `Reaction`: kriz sırasında seçilir, genelde kalıcı slot kaplamaz.

Venture'lar bu tipleri kendi diliyle gösterir. Fast Food'da `Install` oyuncuya ekipman/personel/tedarik gibi görünür; Tech App'te feature/infra/team gibi görünür.

### 4.4 Slot Commit

Kalıcı kartlar ilgili slota yerleşir ve orada görünür kalır. Slotlar sert limitlidir; oyuncu her şeyi aynı anda kuramaz.

Slot doluysa oyuncu şu kararlardan birini verir:

- `Replace`: eski kart çıkar, yeni kart girer.
- `Upgrade`: mevcut kart güçlenir.
- `Merge`: iki benzer karar daha pahalı/güçlü bir karara dönüşür.
- `Discard / Sell`: kart oynanmaz veya mevcut kart çıkarılır.

Bu kararların da işletme sonucu vardır. Çalışanı değiştirmek morale zarar verebilir, supplier değiştirmek kalite dalgalanması yaratabilir, kampanyayı kesmek demand'i düşürebilir.

### 4.5 World Manifestation

Slotta kalan kart mutlaka dünyada karşılık üretir:

- Staff kartı -> çalışan NPC.
- Operation kartı -> işletme objesi veya kapasite istasyonu.
- Marketing kartı -> district müşteri akışı, reklam objesi veya çekim alanı.
- Supplier kartı -> ürün kalitesi, teslimat animasyonu veya stok davranışı.
- Policy kartı -> çalışan/müşteri davranışını değiştiren görünür aura/marker.

### 4.6 Physical Board Effect

Kartın etkisi anında dünyada görünmelidir.

Örnekler:

- `Black Hat Reviews`: rating kısa süreli parlar, mavi müşteri akışı artar, ama review ikonlarının bir kısmı glitch/fake görünür.
- `Garson Al`: işletmeye yeni çalışan girer, servis hattındaki bekleme süresi düşer.
- `Düşük Maaş Politikası`: cash drain azalır, çalışan stress ikonları daha hızlı birikir.
- `Yemek App Kampanyası`: district'ten oyuncuya delivery order akışı başlar, kurye/delivery kapasitesi baskılanır.

### 4.7 Simulation Resolve

Her tur çözüm sırası:

1. Base traffic ve district trend hesaplanır.
2. Player/rival demand pull hesaplanır.
3. Customer pool gri/mavi/kırmızı olarak dağılır.
4. Operation + staff gerçek capacity üretir.
5. Supplier + staff + operation quality üretir.
6. Demand-capacity farkı queue, delay ve stress üretir.
7. Service outcome müşteri memnuniyetini belirler.
8. Rating/reputation güncellenir.
9. Cash, wages, supplier cost ve revenue hesaplanır.
10. Market share fiziksel müşteri dağılımı ve yüzde olarak kayar.
11. Event trigger taraması yapılır.

## 5. The Living Board

Masa üç ana bölgeden oluşur.

```text
┌──────────────────────────────────────────────┐
│ Rival Business                               │
│ Rakibin işletmesi, çalışanları, kampanyaları │
├──────────────────────────────────────────────┤
│ District / Market                            │
│ Gri, mavi, kırmızı müşteri akışı ve trendler │
├──────────────────────────────────────────────┤
│ Player Business                              │
│ Oyuncunun işletmesi, slotları, krizleri      │
└──────────────────────────────────────────────┘
```

### 5.1 Player Business

Oyuncunun işletmesi sahadaki ana bağ kurma alanıdır. Kartlar buraya fiziksel parça olarak yerleşir. İşletme düzenli, kaotik, prestijli, ucuz, yoğun veya çökmekte olan bir karakter kazanmalıdır.

Player Business iki katmanla okunur:

- `Living Diorama`: çalışanlar, müşteriler, kuyruk, servis, ürün ve krizlerin fiziksel sahası.
- `Business Control Board`: kalıcı kartların durduğu slot alanı.

Control board yaşayan sahayı kapatmaz. Oyuncu hover yaptığında slotta duran kart ile sahadaki karşılığı birlikte parlar. Örneğin `Ek Garson` kartı Staff slotunda görünür; aynı anda garson NPC sahada servis yapar.

### 5.2 District / Market

District oyunun canlı pazar panosudur. Müşteriler burada nötr dolaşır, marketing ve reputation etkilerine göre oyuncuya veya rakibe akar. District aynı zamanda event'lerin sosyal yankısını gösterir: kötü yorumlar, trend balonları, trafik sıkışması, festival, denetim aracı, viral paylaşım gibi.

### 5.3 Rival Business

Rakip tüm detaylarıyla simüle edilmek zorunda değildir; ama stratejisi okunmalıdır. Oyuncu rakibin ucuz fiyat mı, premium kalite mi, agresif marketing mi, riskli kısa yol mu oynadığını sahadaki müşteri akışından ve rakip işletme göstergelerinden anlamalıdır.

## 6. Customer Color Language

- `Gray`: karar vermemiş, pazarda dolaşan nötr müşteri.
- `Blue`: oyuncuya çekilmiş müşteri.
- `Red`: rakibe çekilmiş müşteri.
- `Pale Blue`: memnuniyeti düşen, kaybedilme riski olan oyuncu müşterisi.
- `Pale Red`: rakipten koparılabilecek müşteri.
- `Gold Accent`: sadık/prime müşteri, yüksek rating ve repeat business üretir.

Müşteri rengi sadece kozmetik değildir. Renk kayması market share'in fiziksel karşılığıdır.

## 7. Core Stats

| Stat | Anlam | Dünyadaki Görsel Karşılık |
|---|---|---|
| Cash | Büyüme ve kriz çözme gücü | Kasa, ödeme animasyonu, bütçe uyarısı |
| Demand | Gelmek isteyen müşteri/kullanıcı | District'ten işletmeye akan müşteri sayısı |
| Capacity | Talebi taşıma gücü | Servis hızı, kuyruk uzunluğu, boş/dolu istasyonlar |
| Quality | Ürün/hizmet kalitesi | Memnuniyet ikonları, ürün animasyonu, iade/şikayet |
| Rating | Dış itibar | Review balonları, yıldız paneli, organik trafik |
| Staff Stability | Ekip sağlığı | Stress ikonları, tartışma, yavaşlama, işi bırakma |
| Legal Risk | Kısa yol bedeli | Denetim gölgesi, glitch/fake işaretleri, ceza eventleri |
| Market Share | Pazar hakimiyeti | Gri/mavi/kırmızı kalabalık oranı ve akış yönü |

## 8. Chain Reaction Rules

### 8.1 Growth Collapse Chain

```text
Marketing boost
-> Demand artar
-> Capacity yetersiz kalır
-> Queue ve employee stress artar
-> Service delay oluşur
-> Rating düşer
-> Blue customers pale blue olur
-> Rakip kırmızı customer pull kazanır
-> Market share kayar
```

### 8.2 Quality Trust Chain

```text
Supplier upgrade + trained staff
-> Quality artar
-> Review sentiment yükselir
-> Organic demand artar
-> Gold loyal customers oluşur
-> Market share daha stabil hale gelir
```

### 8.3 Risk Debt Chain

```text
Illegal / black hat decision
-> Short-term demand veya cash gain
-> Hidden risk tag birikir
-> Trigger threshold aşılır
-> Scandal / audit / staff revolt event
-> Rating, cash veya capacity darbesi
-> Yeni systemic pressure
```

## 9. Venture Selection

Run başında oyuncu bir venture seçer. Rakip otomatik olarak aynı venture'da başlar. Bu kural rekabeti adil ve okunur yapar: iki işletme aynı müşteri havuzu için farklı stratejilerle yarışır.

İlk kapsam:

1. Fast Food
2. Cafe
3. Tech App
4. Giyim Mağazası
5. Market / Bakkal

Venture seçimi şunları değiştirir:

- Starter deck
- Slot isimleri
- Kriz havuzu
- Türetilmiş metrikler
- Rakip davranış öncelikleri
- Görsel tema ve sahadaki iş dili

## 10. Slot Backbone

Tüm venture'lar aynı ana slot omurgasını kullanır:

| Slot | Başlangıç | Rol |
|---|---:|---|
| Operation | 4 | Fiziksel veya çekirdek üretim/servis kapasitesi |
| Staff | 5 | Çalışan gücü, hız, kalite ve stabilite |
| Marketing | 3 | Demand ve visibility üretimi |
| Supplier | 2 | Maliyet/kalite dengesi |
| Temp Effect | 3 | Krizler, geçici boost/penalty ve risk sonuçları |

Alt-slot isimleri venture'a göre değişir. Bu sayede teknik omurga sabit kalır, tema ve karar dili değişir.

Slot limitleri serttir. Sert limit oyuncunun işletme stratejisini görünür kılar: daha fazla marketing açmak için eski kampanyayı kesmek, yeni çalışan almak için birini çıkarmak veya supplier kalitesini değiştirmek gerçek bir karar olmalıdır.

## 11. Turn Structure

### 11.1 Start of Turn

- Aktif temp effect süreleri azalır.
- District trendleri güncellenir.
- Rakip strateji niyeti kısa görsel sinyalle gösterilir.

### 11.2 Decision Offer

- Oyuncuya kart seçenekleri gelir.
- Kartlar maliyet, risk, slot hedefi ve muhtemel dünya etkisini kısa şekilde gösterir.

### 11.3 Play

- Oyuncu kart seçer.
- Kart sahadaki ilgili slota veya district etkisine dönüşür.
- Kamera kısa bir placement beat gösterebilir.

### 11.4 Resolve

- Simülasyon zinciri çalışır.
- Müşteri akışı ve çalışan davranışları güncellenir.

### 11.5 Event / Reaction

- Trigger olmuş event varsa mikro-sinematik sequence çalışır.
- Oyuncu 1-3 seçenekten birini seçer.
- Sonuç dünyada görünür ve statlara işlenir.

### 11.6 Rival

- Rakip kendi kararını uygular.
- District'te kırmızı pull veya rakip işletme değişimi görünür.

### 11.7 End of Turn

- Market share shift gösterilir.
- Bir sonraki turun ana baskısı preview edilir.

## 12. Meta Progression: Startup to Holding

Tek run semt/pazar hakimiyeti etrafında oynanır. Uzun vadeli meta progression ise işletme imparatorluğu kurma fantezisini taşır.

### 12.1 Run Goal

Varsayılan run hedefi:

- 25 tur içinde en az yüzde 60 market share.
- İflas, ağır legal darbe veya rating çöküşü olmadan ayakta kalmak.

### 12.2 Scale Stages

1. `Startup`: küçük işletme, düşük cash, düşük capacity, yüksek kırılganlık.
2. `Local Favorite`: rating oturur, sadık müşteri oluşur.
3. `Growth Business`: marketing ve capacity yarışı başlar.
4. `Chain / Platform`: ikinci şube, delivery, online kanal veya ekip katmanı açılır.
5. `Holding Candidate`: birden fazla gelir kanalı, güçlü market share ve exit opsiyonu.

### 12.3 Exit Loop

Holding seviyesine ulaşan oyuncu işletmeyi satabilir.

Exit sonuçları:

- Yeni run için başlangıç sermayesi.
- Venture açılımları veya pasif avantajlar.
- Daha zorlu district/rival seçenekleri.
- Eski holdingin dünyada legacy etkisi: marka güveni, yatırımcı ağı, deneyimli mentor kartları.

Exit, oyunu bitirmek değil, daha büyük ölçekte tekrar başlamak anlamına gelir.

## 13. Win / Lose Conditions

### Win

- Run sonunda yüzde 60 veya üstü market share.
- Alternatif: rakibin cash/rating çöküşü ve oyuncunun minimum sürdürülebilir işletme eşiğini koruması.
- Meta win: holding seviyesine ulaşmak ve başarılı exit yapmak.

### Lose

- Cash negatifte kalır ve borç/bridge çözümü yoktur.
- Rating geri dönülemez eşiğin altına iner.
- Legal risk ağır cezaya dönüşür.
- Rakip yüzde 70 üstü market share ile pazarı kilitler.
- Staff collapse yüzünden işletme birkaç tur servis veremez.

## 14. MVP Design Priority

İlk oynanabilir prototip tüm venture'ları tasarımsal olarak kapsar, fakat sistem derinliği önce Fast Food üzerinden doğrulanır.

Öncelik:

1. Kart seçimi ve kartın sahaya fiziksel inmesi.
2. District müşteri renk akışı.
3. Demand-capacity-quality-rating zinciri.
4. Employee stress ve quit event'i.
5. Rakip market pull.
6. Venture'a özel kriz ve kart isimleri.
7. Holding/exit meta tasarımının dokümanda hazır olması.

## 15. Related Documents

- `EVENT_SYSTEM.md`: mikro-sinematik reactive event sistemi.
- `TECHNICAL_MAPPING.md`: Unity veri modeli ve sistem eşlemesi.
- `SCENARIOS.md`: karttan sahaya akan zincir senaryolar.
- `CARD_AND_BOARD_SYSTEM.md`: hibrit kart, slot ve 3D board sistemi.
- `LEVEL_DESIGN.md`: sahne, district, kamera ve venture level modülleri.
- `CODE_ARCHITECTURE.md`: Unity kod mimarisi, state/system/presenter ayrımı.
- `ASSET_MANIFEST.md`: Toy Diorama asset, prefab, UI, VFX ve audio ihtiyaçları.
- `ASSET_PROMPTS.md`: AI concept promptları ve 3D production brief şablonları.
- `ASSET_PRODUCTION_PIPELINE.md`: asset üretim sırası, naming, import ve QA kuralları.
- `UI_UX_DESIGN.md`: ekran akışları, kart UX, slot UX ve event choice UX.
- `CAMERA_LIGHTING_TURN_FLOW.md`: sabit izometrik kamera, Toy Studio ışık ve canlı tur akışı.
- `businesses/fast_food.md`
- `businesses/cafe.md`
- `businesses/tech_app.md`
- `businesses/giyim_magazasi.md`
- `businesses/market_bakkal.md`

## 16. Production Documentation Rule

Ana GDD oyunun tasarım niyetini tanımlar. Production dokümanları bu niyeti uygulanabilir çalışma planına çevirir:

- Level designer `LEVEL_DESIGN.md` ile sahne ve kamera düzenini çıkarır.
- Unity developer `CODE_ARCHITECTURE.md` ve `TECHNICAL_MAPPING.md` ile sistemleri kurar.
- Artist/modeler `ASSET_MANIFEST.md`, `ASSET_PROMPTS.md` ve `ASSET_PRODUCTION_PIPELINE.md` ile asset listesini, promptları ve üretim kalitesini yönetir.
- UI designer `UI_UX_DESIGN.md` ile flow ve ekranları tasarlar.

İlk oynanabilir uygulama Fast Food üzerinden kurulabilir, ancak dokümantasyon kapsamı tam oyundur.
