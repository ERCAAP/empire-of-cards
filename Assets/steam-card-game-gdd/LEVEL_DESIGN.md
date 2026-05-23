# Empire of Cards Level Design

> Görsel hedef: Toy Diorama. Sahne bir maket gibi okunur; işletmeler, insanlar, slot board ve district tek bakışta anlaşılır.

## 1. Level Design Goal

Level tasarımı oyuncuya şunu göstermelidir:

- İşletmem şu an nasıl çalışıyor?
- Müşteri pazarı bana mı rakibe mi akıyor?
- Hangi kararım sahada neyi değiştirdi?
- Hangi kriz fiziksel olarak büyüyor?

Sahne bir kart masası değil, oyuncunun işletme ekosistemidir. Kart slotları bu dioramanın kontrol panelidir.

## 2. Main Scene Layout

Ana gameplay sahnesi üç yatay/derinlik katmanından oluşur:

```text
┌──────────────────────────────────────────────┐
│ Rival Business                               │
│ Rakip işletme, kırmızı müşteri akışı         │
├──────────────────────────────────────────────┤
│ District / Market                            │
│ Gri müşteri havuzu, trendler, yollar         │
├──────────────────────────────────────────────┤
│ Player Business + Business Control Board     │
│ Oyuncu işletmesi, slotlar, kartlar           │
└──────────────────────────────────────────────┘
```

### 2.1 Player Business

Oyuncunun ana duygusal bağ alanıdır. Her venture aynı sistem slotlarını kullanır ama farklı işletme modülleriyle görünür.

Gerekli bölgeler:

- Giriş/karşılama noktası.
- Servis veya ürün teslim noktası.
- Çalışan istasyonları.
- Müşteri bekleme/kuyruk hattı.
- Venture'a özel operasyon alanı.
- Krizlerin göründüğü mikro-event noktaları.

### 2.2 District / Market

District market share'in fiziksel alanıdır.

Gerekli bölgeler:

- Nötr müşteri spawn noktaları.
- Oyuncu işletmesine giden mavi flow path.
- Rakip işletmesine giden kırmızı flow path.
- Trend/event anchor noktaları: festival, review burst, denetim aracı, viral çekim noktası.
- Rival pull ve player pull alanlarının görsel çakışabileceği orta bölge.

### 2.3 Rival Business

Rakip tam simülasyon sahnesi kadar detaylı olmak zorunda değildir; ama stratejisi okunmalıdır.

Gerekli görsel sinyaller:

- Rakip kampanya tabelaları.
- Kırmızı müşteri kuyruğu.
- Rakip kalite/fiyat/marketing duruşu.
- Rakibin kriz yaşadığı anlarda kırmızı akışın zayıflaması.

### 2.4 Business Control Board

Player Business'in yanında veya ön bandında konumlanır. Yaşayan sahayı kapatmaz.

Slot grupları:

- Operation
- Staff
- Marketing
- Supplier
- Temp Effect

Hover kuralı: Slot kartı seçilince sahadaki linked obje/NPC/flow highlight olur.

## 3. Camera Layers

Kamera dili sabit izometrik Toy Diorama'dır. Oyuncu serbest orbit yapmaz. Ana kamera orthographic çalışır; pitch 35-45 derece, yaw 35-45 derece, roll 0 kabul edilir. Kamera detayları için ana kaynak `CAMERA_LIGHTING_TURN_FLOW.md` dosyasıdır.

### 3.1 Board Overview

Varsayılan kamera. Player, district ve rival aynı kompozisyonda okunur.

Kullanım:

- Tur planlama.
- Market share okuma.
- End turn sonucu.

Teknik kural:

- Tüm board okunur.
- Card offer bandı açıldığında alt UI alanı için framing hafif yukarı alınabilir.
- Simulation Planning'de normal hızda, Card Offer'da slow tactical time ile akar.

### 3.2 Card Inspect Camera

Kart hover edildiğinde hafif yakınlaşır veya hedef slotu/parçayı vurgular.

Kural:

- Kamera agresif zıplamaz.
- Ghost preview sahada görünür.
- Slot ve world target birlikte parlar.
- Blend süresi 0.25-0.45 sn.

### 3.3 Slot Placement Camera

Kart commit edildiğinde kısa 0.5-1.5 saniyelik placement beat.

Örnek:

- Garson kartı slota oturur, aynı anda sahada garson kapıdan girer.
- Espresso makinesi karta bağlı olarak bara iner.
- Blend süresi 0.6-1.0 sn.

### 3.4 Event Focus Camera

Mikro-sinematik eventlerde kullanılır. 3-8 saniye arası sürer.

Kural:

- Problem kaynağına gider.
- Karar paneli dünyayı kapatmaz.
- Seçim sonrası consequence gösterir.
- Blend süresi 0.4-0.8 sn.
- Event sırasında simulation pause veya 0.1 hız.

### 3.5 Market Flow Camera

Büyük market share değişimlerinde district üst görünümüne çıkar.

Kullanım:

- Rakip kampanya.
- Rating collapse.
- Viral growth.
- Holding/scale transition.
- Blend süresi 0.8-1.2 sn.

### 3.6 Rival Reaction Camera

Rakip büyük hamle yaptığında kısa odak.

Örnek:

- Rakip indirim tabelası açar.
- Rakip önüne kırmızı kuyruk yığılır.
- Oyuncunun çalışanına rakip teklif balonu çıkar.

## 3.7 Board Safe Zones

Ekran şu güvenli alanlarla tasarlanır:

- Top safe: HUD ve rival labels.
- Center safe: district customer flow.
- Bottom world safe: player business ve slot board.
- Bottom UI band: card offer, ekran yüksekliğinin yaklaşık yüzde 20-28'i.
- Event panel: problem anchor yakınında veya alt bandın üstünde.

Card offer bandı district akışını tamamen kapatmaz. Slot board player business ile görsel ilişkisini korur.

## 3.8 Lighting Rules

Işık modeli `Toy Studio`dur:

- Sıcak ana directional light.
- Yumuşak gölgeler.
- Player tarafı mavi accent.
- Rival tarafı kırmızı accent.
- Neutral district sıcak gri/ılık ışık.
- Krizlerde global ışık değişmez; lokal stress/risk/review pulse kullanılır.
- MVP'de gün-gece döngüsü yoktur.

## 4. Scale Stages

Her venture aynı ölçek aşamalarını kullanır.

### 4.1 Startup

- Küçük işletme.
- Az istasyon.
- Müşteri akışı seyrek.
- Slot board sade.
- Krizler kişisel ve görünür: tek garson, tek kasa, tek server.

### 4.2 Local Favorite

- Sadık müşteri segmenti görünür.
- İşletme dekoru/kalitesi artar.
- Gold accent müşteriler doğar.
- Rakip oyuncuyu ciddiye almaya başlar.

### 4.3 Growth Business

- Kuyruklar, kampanyalar, staff pressure belirginleşir.
- Slot baskısı stratejiyi zorlar.
- District'teki müşteri akışı daha yoğun olur.

### 4.4 Chain / Platform

- İkinci servis hattı, ikinci şube temsili veya platform ölçeği açılır.
- Yönetim katmanı gerekir.
- Eventler artık sadece operasyon değil, organizasyon krizleri de üretir.

### 4.5 Holding Candidate

- Kamera daha geniş açılır.
- Marka tabelası ve district dominasyonu görünür.
- Exit/investment eventleri doğar.

## 5. Venture Level Modules

### 5.1 Fast Food

Startup:

- Küçük mutfak, kasa, 2-3 masa, dış kapı kuyruğu.

Midgame:

- Delivery noktası, ek ızgara, daha yoğun servis hattı.

Lategame:

- Şube tabelası, franchise board, yoğun delivery akışı.

Kriz anchor'ları:

- Mutfak kaosu.
- Kasa kuyruğu.
- Delivery paket yığılması.
- Hijyen denetimi.

### 5.2 Cafe

Startup:

- Bar, birkaç masa, vitrin, sakin müşteri akışı.

Midgame:

- Takeaway penceresi, daha fazla oturma, Instagram çekim noktası.

Lategame:

- Marka cafe, ikinci lokasyon işareti, sadakat müşteri köşesi.

Kriz anchor'ları:

- Barista burnout.
- Masa bekleme.
- Viral reels kalabalığı.
- Ambiyans şikayeti.

### 5.3 Tech App

Startup:

- Küçük ofis, product desk, backend/server node, app store paneli.

Midgame:

- Support desk, growth wall, analytics ekranları, user stream yolları.

Lategame:

- Platform operations, büyük server cluster, community alanı.

Kriz anchor'ları:

- Crash spike.
- Support backlog.
- Developer burnout.
- Platform audit.

### 5.4 Giyim Mağazası

Startup:

- Vitrin, raflar, kasa, deneme kabini.

Midgame:

- Online paket alanı, daha fazla raf, fotoğraf/çekim köşesi.

Lategame:

- Zincir mağaza hissi, sezon panosu, depo temsili.

Kriz anchor'ları:

- Boş raf.
- Deneme kabini kuyruğu.
- İade bankosu.
- Influencer kalabalığı.

### 5.5 Market / Bakkal

Startup:

- Raf, kasa, taze ürün köşesi, veresiye defteri.

Midgame:

- WhatsApp sipariş hattı, kurye çıkışı, daha geniş raflar.

Lategame:

- Yerel zincir market, taze ürün hattı, vardiya yönetimi.

Kriz anchor'ları:

- SKT şikayeti.
- Veresiye defteri.
- Kasa kuyruğu.
- Fire/bozuk ürün alanı.

## 6. District Flow Design

Müşteri flow sistemi üç renkle çalışır:

- Gray: nötr pazar.
- Blue: oyuncuya çekilmiş müşteri.
- Red: rakibe çekilmiş müşteri.

Flow path kuralı:

- Her müşteri önce district orta alanında spawn olur.
- Pull hesaplandıktan sonra oyuncu veya rakip yoluna döner.
- Rating düşüşünde bazı blue müşteriler pale blue olur, sonra gray veya red'e kayabilir.
- Rival campaign sırasında kırmızı flow path daha parlak olur.

## 7. Level Acceptance Criteria

- Oyuncu ekranın ana görünümünden işletmesini, district'i ve rakibi aynı anda okuyabilmeli.
- Her kalıcı kartın sahada karşılığı olmalı.
- Her venture aynı layout omurgasını koruyup farklı işletme kimliği vermeli.
- Büyük market share değişimi müşteri renk akışıyla fiziksel görünmeli.
- Event kamera odağı problem kaynağını göstermeli.
- Toy Studio ışık gri/mavi/kırmızı müşteri ayrımını bozmayacak kadar temiz olmalı.
