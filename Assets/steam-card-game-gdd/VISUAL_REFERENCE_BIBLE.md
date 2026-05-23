# Empire of Cards Visual Reference Bible

> Bu dosya gönderilen üç görseli ana üretim referansı olarak kilitler. Amaç generic toy city değil, oynanabilir Empire of Cards ekranı üretmektir.

## 1. Reference Lock

### Reference 1: Gameplay Board

Ana oynanış ekranı şu kompozisyonu kullanır:

- Sahne ahşap masa üstünde duran fiziksel bir board gibi görünür.
- Ortada `District` yazılı yol kavşağı ve müşteri akışı vardır.
- Oyuncu işletmesi ekranın alt/ön tarafındadır.
- Rakip işletme ekranın üst/arka tarafındadır.
- Sol kenarda ahşap/plaka hissinde dikey stat HUD bulunur.
- Alt bantta kart eli ve slot board birlikte görünür.
- Sağ kenarda simülasyon döngüsü, event veya turn phase paneli bulunur.

Bu ekran oyunun default gerçek yüzüdür. Kart seçimi, kart inspect, slot commit, resolve ve market share değişimi bu sahneden kopmadan oynanır.

### Reference 2: System Overview

Sistem anlatım görseli doküman, pitch ve production alignment için kullanılır.

Bu dil şunları sabitler:

- Oyun kart board game gibi anlatılabilir ama asıl ürün 3D living diorama'dır.
- Kart sistemi Install, Burst, Policy, Risk, Reaction olarak okunur.
- Board üç katmanlıdır: oyuncu işletmesi, district/pazar, rakip.
- Teknik taraf data definitions, runtime state, systems ve presenters ayrımıyla kurulur.
- Meta ilerleme startup, local favorite, growth, chain ve holding aşamalarını gösterir.

### Reference 3: Event Overlay

Event ekranı popup değil, sahaya bağlı mikro-sinematik karar anıdır.

Zorunlu yapı:

- Kamera problem kaynağını gösterir.
- Sol/alt tarafta event'in sahadaki fiziksel beat'i görünür.
- Sağ tarafta ahşap board/panel hissinde seçim paneli açılır.
- Panel 1-3 seçenek verir.
- Seçim sonrası panel kapanır veya consequence paneline dönüşür.
- NPC, müşteri ve işletme sahada sonucu oynar.

## 2. Final Screen Composition

```text
┌──────────────────────────────────────────────────────────────┐
│ Left HUD      Rival Business / red pull        Off-board deck │
│ plaques       + rival queue + rival signs       props         │
│                                                              │
│               District road intersection                     │
│               gray / blue / red customer flow                │
│                                                              │
│               Player Business / blue pull                    │
│ Card hand      Slot Board: Operation Staff Marketing Supplier │
│ bottom band    Temp Effect                     Right panel    │
└──────────────────────────────────────────────────────────────┘
```

Screen regions:

- `World Frame`: ahşap masa ve board kenarı. Oyuncuya fiziksel masa hissi verir.
- `District Core`: ekranın en okunur orta alanı. Market share burada yaşar.
- `Player Business`: emotional attachment bölgesi. Staff, queue, object state burada görünür.
- `Rival Business`: stratejik baskı bölgesi. Rakip tam detaylı olmasa da hamlesi okunur.
- `Business Control Board`: kartların kalıcı işletme kararına dönüştüğü slot alanı.
- `Card Hand`: karar teklifleri. Sahadan ayrı ekran değildir.
- `Left HUD`: cash, rating, demand, staff stability, legal risk, market share.
- `Right Panel`: simulation phase, event choice, consequence, turn summary.

## 3. Environment Rules

Board environment şu modüllerden kurulmalıdır:

- Ahşap masa yüzeyi ve kalın masa/board çerçevesi.
- Kare veya dikdörtgen board tabanı.
- Merkezde okunur yol kavşağı.
- Kaldırım, küçük park adaları, banklar, çöp kutuları, ağaçlar ve sokak lambaları.
- Player ve rival işletmesi için köşe/kenar shop footprint.
- Customer flow için net path spline'ları.
- Spawn node'ları görsel gürültü yaratmadan saklanmış veya dekoratif olmalıdır.
- District label büyük ama gameplay'i kapatmayacak şekilde zemine işlenir.

Environment decorative değil, simülasyon bilgisini taşır:

- Long queue varsa müşteriler path üzerinde yığılır.
- Marketing aktifse blue veya red pull çizgileri güçlenir.
- Rating düşüşünde customer path üstünde negatif review balonları çıkar.
- Legal event varsa denetim aracı veya inspector anchor sahaya girer.

## 4. UI Placement Rules

Sol HUD:

- Ekranın sol kenarında üstten alta ahşap/kağıt plakalar.
- Çok büyük tablo değil, 4-6 kritik sinyal.
- Değer değiştiğinde kısa pulse.
- Sahadaki world feedback ile aynı anda çalışır.

Alt kart eli:

- Ekranın alt yüzde 20-28 bandını kullanır.
- Kartlar hafif fan veya sıra halinde durabilir.
- Kart inspect sırasında kart büyür ama district'i kapatmaz.
- Kart slota sürüklenirken world ghost preview görünür.

Slot board:

- Player business'in önünde veya hemen yanında fiziksel kontrol masası gibi durur.
- Slot başlıkları: Operation, Staff, Marketing, Supplier, Temp Effect.
- Dolu kartlar küçük kalır ama hover ile büyür.
- Kart-world link çizgisi sadece hover/inspect anında görünür.

Sağ panel:

- Event ve simulation paneli aynı alan ailesini kullanır.
- Planning/resolve sırasında küçük phase paneli olabilir.
- Event sırasında panel büyür ama problem kaynağını kapatmaz.
- Panel ahşap çerçeveli, Toy Diorama diline uyumlu olmalıdır.

## 5. Customer And Market Share Visual Rules

Customer colors:

- `Gray`: karar vermemiş pazar.
- `Blue`: oyuncuya çekilmiş müşteri.
- `Red`: rakibe çekilmiş müşteri.
- `Gold Accent`: sadık veya yüksek değerli müşteri.

Market share sadece yüzde barı değildir:

- District'teki insan sayısı ve renk dağılımı ana göstergedir.
- Blue müşteriler kuyrukta bekleyip kızarsa önce pale blue olur, sonra gray veya red'e kayabilir.
- Rival kampanyasında red flow path ışıldar, bazı gray müşteriler o yöne döner.
- Recovery event sonrası negative review balonları azalır, gold accent müşteriler doğabilir.

## 6. Event Visual Rules

Her event şu dört görsel beat'i kullanır:

```text
Problem shown in world
-> NPC/customer reaction
-> player choice panel
-> visible consequence and new pressure label
```

Event örneği: Garson işi bırakmak istiyor.

- Problem: staff stress aura, garson kasanın yanında tartışır.
- Reaction: garson owner/manager noktasına yürür, konuşma balonu çıkar.
- Choice: sağ panelde `İkna et`, `Kabul et`, `Kriz çıkaranı kov` gibi seçenekler.
- Visible consequence: garson çıkar gider veya sakinleşir; Staff slotu boşalır/durumu değişir.
- New pressure: müşteri kuyruğu uzar, service speed düşer, rating baskısı etiketi doğar.

## 7. Asset Prompt Lock

AI concept veya 3D brief üretirken promptlar bu kelimeleri taşımalıdır:

- tabletop business board
- wooden frame
- fixed isometric gameplay view
- central district road intersection
- bottom player business
- top rival business
- gray blue red customer ownership
- bottom card hand
- physical slot board
- left stat plaques
- right event/simulation panel
- playable game screen, not poster

Promptlar şu çıktıları üretmemelidir:

- Sadece güzel şehir illüstrasyonu.
- Sadece kart oyunu masası.
- Fullscreen corporate dashboard.
- Dark cinematic scene.
- Gerçekçi restoran/fotoğraf.
- Fantasy card battler.

## 8. Acceptance Criteria

- BoardOverview ekranında oyuncu, district, rakip, sol HUD, alt kartlar ve slot board aynı anda okunur.
- Kartı oynamadan önce hedef slot ve dünyadaki ghost karşılığı görünür.
- Event paneli problem kaynağını kapatmaz.
- Müşteri ownership rengi 2 saniyeden kısa sürede anlaşılır.
- Görsel kompozisyon gönderilen üç referansla aynı ürün ailesinden görünür.
