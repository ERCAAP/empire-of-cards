# Empire of Cards Card and Board System

> Net karar: Empire of Cards klasik bir kart board oyunu değildir. Oyun, kart destekli 3D systemic business management oyunudur. Kartlar kararın formudur; board ise kararların yaşadığı işletme dünyasıdır.

## 1. Why Cards Exist

Kartlar üç işi yapar:

1. Oyuncuya karmaşık işletme kararlarını okunur paketler halinde sunar.
2. Tur içindeki stratejik tradeoff'u netleştirir.
3. Kalıcı kararların işletme kontrol board'unda izlenebilir kalmasını sağlar.

Kartlar oyunun kendisi değil, karar verme dilidir. Oyuncu kart seçtiği için değil, seçtiği kartın dünyayı nasıl değiştirdiğini gördüğü için eğlenmelidir.

## 2. Board Identity

Board iki katmandan oluşur:

### 2.1 Living Diorama

Ana oyun alanıdır. Kamera çoğu zaman burayı gösterir.

- Player Business
- District / Market
- Rival Business
- Müşteri akışları
- Çalışan davranışları
- Kuyruklar, siparişler, review balonları
- Rakip kampanyaları

### 2.2 Business Control Board

Yaşayan sahayı kapatmayan, işletmenin yanında veya altında duran slot alanıdır.

Burada kalıcı kartlar görünür:

- Operation
- Staff
- Marketing
- Supplier
- Temp Effect

Bu alan bir spreadsheet değildir. Oyuncu burada işletmesinin "aktif karar mimarisini" görür.

## 3. Is This a Card Board Game?

Hayır, ana oyun hissi card board değildir.

Doğru tanım:

> 3D business diorama + visible slot board + card-driven decisions.

Kart board oyunlarında ana okumayı kartlar ve grid sağlar. Empire of Cards'ta ana okumayı yaşayan işletme sağlar. Slot board sadece kararların hafızasıdır.

## 4. Hybrid Card Rule

Kart oynandığında iki sonuçtan biri olur:

1. Kart slotta kalır ve dünyada fiziksel karşılık üretir.
2. Kart anlık çözülür, dünyada kısa etki üretir ve sonra tag/temp effect bırakır.

Örnek:

- `Ek Garson`: Staff slotunda kart olarak kalır, sahada garson NPC oluşur.
- `Yeni Izgara`: Operation slotunda kart olarak kalır, mutfakta ızgara objesi oluşur.
- `Google Ads`: Marketing slotunda kart olarak kalır, district'te mavi müşteri pull üretir.
- `Sahte Yorum Patlaması`: anlık burst olabilir, rating parlaması ve `fake_reviews` tag'i bırakır.
- `Özür Kampanyası`: reaction olarak çözülür, negatif review balonlarını azaltır, slot kaplamaz veya kısa Temp Effect bırakır.

## 5. Card Lifecycle

```text
Offer
-> Inspect
-> Play Intent
-> Slot Validation
-> Slot Commit
-> World Manifestation
-> Resolve
-> Persist / Expire
-> Event Hooks
```

### 5.1 Offer

Her tur oyuncuya 3-5 kart sunulur. En az bir kart mevcut ana baskıya cevap vermelidir.

Offer paneli gösterir:

- Kart adı
- Venture teması
- Maliyet
- Slot hedefi
- Kalıcı mı geçici mi
- Ana tradeoff
- Risk etiketi varsa kısa uyarı

### 5.2 Inspect

Oyuncu kartın üstüne geldiğinde sahada önizleme görür:

- Hangi slotu kullanacağı parlar.
- Dünyada hangi objeyi/NPC'yi/akışı oluşturacağı ghost preview ile görünür.
- Demand, capacity, rating gibi ana etkiler küçük ikonlarla gösterilir.

### 5.3 Play Intent

Oyuncu kartı seçer. Eğer hedef slot boşsa doğrudan commit olur. Slot doluysa replacement kararına geçilir.

### 5.4 Slot Validation

Kart şu soruları kontrol eder:

- Hedef slot tipi mevcut mu?
- Slot boş mu?
- Kart upgrade/merge yapabilir mi?
- Maliyet ödenebilir mi?
- Crisis/reaction kartı için aktif kriz var mı?
- Venture uyumlu mu?

### 5.5 Slot Commit

Kalıcı kartlar slota yerleşir. Slotta kart görünür kalır.

Slot kartı şunları göstermelidir:

- Kart adı
- Tip ikonu
- Durum/stress/durability göstergesi
- Upgrade seviyesi
- Linked world actor/object bağlantı çizgisi veya highlight

### 5.6 World Manifestation

Kart commit edildiği anda dünya değişir.

Manifestation tipleri:

- `Actor`: çalışan, kurye, developer, satış danışmanı.
- `Object`: masa, raf, espresso makinesi, server node, vitrin.
- `Flow`: müşteri akışı, delivery hattı, user stream.
- `Aura`: policy etkisi, düşük maaş baskısı, premium kalite algısı.
- `Marker`: risk, denetim gölgesi, fake review glitch.
- `Temp`: kısa süreli kampanya, kriz, toparlanma efekti.

### 5.7 Resolve

Kartın stat etkisi simülasyona girer. World manifestation sadece görsel değil, state'e bağlıdır.

### 5.8 Persist / Expire

Kalıcı kartlar slotta kalır. Geçici kartlar süre dolunca:

- slottan çıkar,
- temp effect'i biter,
- world manifestation kaybolur veya normalleşir,
- gerekirse event tag bırakır.

### 5.9 Event Hooks

Kart gelecekte event doğurabilir:

- `underpaid_staff`
- `fake_reviews`
- `over_capacity`
- `cheap_supplier`
- `rival_attention`

## 6. Card Behavior Types

Teknik omurga beş davranış tipi kullanır.

| Type | Slotta Kalır mı? | Dünya Karşılığı | Örnek |
|---|---|---|---|
| Install | Evet | Obje/NPC/sistem açar | Ek Garson, Yeni Izgara |
| Burst | Hayır veya kısa Temp | Kısa animasyon/akış | Broşür Dağıt, Flash Sale |
| Policy | Evet | Davranış kuralı/aura | Düşük Maaş Politikası |
| Risk | Duruma göre | Glitch, gölge, risk marker | Sahte Yorum |
| Reaction | Genelde hayır | Kriz sonucu animasyonu | Özür Kampanyası |

## 7. Venture Surface Names

Oyuncu teknik type adlarını doğrudan görmek zorunda değildir. Venture kendi dilini kullanır.

| Venture | Install Dili | Policy Dili | Risk Dili |
|---|---|---|---|
| Fast Food | Ekipman, Personel, Tedarik | Vardiya, Ücret, Menü | Sahte Yorum, Sigortasız |
| Cafe | Bar, Ambiyans, Barista | Fiyat, Sessiz Saat, Servis | Fake Specialty |
| Tech App | Feature, Infra, Team | Roadmap, Pricing, Sprint | Fake Reviews, Crunch |
| Giyim | Vitrin, Stok, Koleksiyon | İade, Fiyat, Sezon | Fake Scarcity, Kopya Trend |
| Market | Raf, Taze Ürün, Servis | Veresiye, Gece Açık | SKT İndirimi |

## 8. Slot Pressure

Slot limitleri serttir. Oyuncu her şeyi aynı anda kuramaz.

Başlangıç:

- Operation: 4
- Staff: 5
- Marketing: 3
- Supplier: 2
- Temp Effect: 3

Slot doluyken oyuncu şu kararlardan birini verir:

- `Replace`: eski kart çıkar, yeni kart girer.
- `Upgrade`: aynı aileden kart mevcut kartı güçlendirir.
- `Merge`: iki benzer kart daha güçlü ama daha pahalı karta dönüşür.
- `Discard / Sell`: kart oynanmaz veya mevcut kart küçük cash karşılığı çıkarılır.

Bu kararlar sonuç üretir:

- Çalışanı çıkarmak staff stability düşürebilir.
- Supplier değiştirmek kısa kalite dalgalanması yaratabilir.
- Marketing kampanyasını kesmek demand düşüşü doğurabilir.
- Operation sökmek capacity kaybı veya sunk cost üretir.

## 9. Placement Rules by Slot

### 9.1 Operation

Operation kartları işletmenin fiziksel kapasitesidir. Slotta kalır ve dünyada obje açar.

Örnek:

- `Yeni Izgara`: mutfakta ızgara.
- `Espresso Makinesi`: barda makine.
- `Backend Upgrade`: tech sahasında server node.
- `Ek Raf`: markette raf.

### 9.2 Staff

Staff kartları slotta çalışan kartı olarak kalır ve sahada NPC üretir.

Çalışan kartlarının özel durumu:

- Stress göstergesi vardır.
- İsimli olabilir.
- Quit, poach, training, burnout event'lerine bağlanır.
- Replace etmek duygusal sonuç doğurabilir.

### 9.3 Marketing

Marketing kartları ikiye ayrılır:

- Kalıcı kampanya: slotta kalır, district pull üretir.
- Burst kampanya: anlık demand spike üretir, sonra temp/risk bırakır.

### 9.4 Supplier

Supplier kartları slotta kalır, quality/cost davranışını belirler. Dünyada ürün kalitesi, teslimat, stok veya servis animasyonuna yansır.

### 9.5 Temp Effect

Kriz, geçici boost veya penalty burada görünür. Temp slotları dolarsa yeni kriz daha ağır sonuçla doğabilir veya eski temp effect'i uzatabilir.

## 10. Where the Fun Is

Kartı sürüklemek tek başına eğlence değildir. Eğlence şu anlardan gelir:

1. Oyuncu doğru darboğazı okur.
2. Kartı oynayınca dünya hemen değişir.
3. Beklenmedik ama adil bir zincir reaksiyon doğar.
4. Mikro-event oyuncuyu dramatik bir tradeoff'a zorlar.
5. Sonuç sahada görünür.
6. Yeni baskı bir sonraki turu ilginç yapar.

Bu yüzden kart UI hızlı, temiz ve karar odaklı olmalıdır. Aksiyon hissi kriz seçimlerinde ve world reaction'da yoğunlaşır.

## 11. Board Readability Rules

- Slot board yaşayan sahayı kapatmamalı.
- Her kalıcı kartın dünyadaki karşılığı bulunmalı.
- Dünya karşılığı olmayan kalıcı kart tasarım hatasıdır; en azından aura/marker üretmelidir.
- Slotta kalan kart ile sahadaki obje/NPC arasında hover highlight olmalıdır.
- District müşteri akışı her zaman görünür kalmalıdır.
- Rakip hamlesi kırmızı müşteri akışı veya rakip board değişimiyle okunmalıdır.

## 12. Example Turn

Setup:

- Fast Food
- Staff slotları dolu.
- Demand capacity'yi aşmış.
- Offer'da `Ek Garson` gelir.

Player action:

1. Oyuncu `Ek Garson` kartını inspect eder.
2. Staff slotları dolu olduğu için mevcut çalışan kartları parlar.
3. Oyuncu düşük performanslı garsonu replace eder.
4. Eski garson sahadan çıkar, staff stability -5.
5. Yeni garson NPC olarak girer, service capacity +12.
6. Kuyruk bir sonraki resolve'da kısalır.
7. Eski garsonun kovulması `staff_resentment_minor` tag'i bırakır.

Bu tek hamle hem kart board kararını hem yaşayan dünya sonucunu üretir.
