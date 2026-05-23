# Venture: Fast Food

## 1. Identity

Fast Food yüksek tempo, kapasite yönetimi ve yorum baskısı venture'ıdır. Oyuncu hızlı müşteri çekebilir, ama mutfak, servis ve delivery bunu taşıyamazsa en hızlı çöken sektör de budur.

Fantasy:

> "Küçük bir burger/döner standından mahalle zincirine büyüdüm."

## 2. Starting Board

- Küçük dükkan
- 1 temel mutfak istasyonu
- 1 kasa/servis noktası
- 2 çalışan: aşçı ve kasiyer
- Düşük cash
- Rating: 4.0
- Market share: oyuncu 35, rakip 35, nötr 30

## 3. Slot Language

| Slot | Alt-slotlar |
|---|---|
| Operation | Mutfak, Servis, Oturma, Delivery |
| Staff | Aşçı, Kasiyer, Kurye, Temizlik, Şube Müdürü |
| Marketing | Broşür, Google, Sosyal Medya, Yemek App Kampanyası |
| Supplier | Kasap, Manav, Ekmek/Fırıncı, İçecek Anlaşması |
| Temp Effect | Kuyruk Baskısı, Hijyen Sorunu, Fake Review Şüphesi |

## 4. Card Families

### Build

- `Yeni Izgara`: kitchen capacity +, cash -
- `Ek Masa`: seat capacity +, cleaning pressure +
- `Delivery Çantası`: delivery demand açılır, kurye ihtiyacı +
- `Hızlı Servis Tezgahı`: service speed +

### Staff

- `Usta Aşçı`: quality +, wage cost +
- `Ek Garson`: service capacity +, wage cost +
- `Kurye Al`: delivery capacity +
- `Şube Müdürü`: staff stability +, wage cost +

### Marketing

- `Broşür Dağıt`: local demand +
- `Google Ads`: visibility +, cost per turn
- `Yemek App Kampanyası`: delivery demand ++, margin -
- `Sosyal Medya Challenge`: demand spike, quality risk if capacity low

### Supplier

- `Kaliteli Et Anlaşması`: quality +, cost +
- `Ucuz Tedarikçi`: cost -, hygiene/reputation risk +
- `Yerel Fırın`: quality +, stability +
- `Toplu İçecek Anlaşması`: cash margin +

### Risk / Reaction

- `Sahte Yorum Satın Al`: rating short boost, legal/platform risk +
- `Sigortasız Çalışan`: wage cost -, legal risk +, staff stability -
- `Özür Menüsü`: rating recovery, cash -
- `Acil İşe Alım`: capacity +, quality risk

## 5. Derived Metrics

- `Servis Hızı`: capacity / demand.
- `Mutfak Baskısı`: demand - mutfak kapasitesi.
- `Google Puanı`: rating.
- `Hijyen Güveni`: supplier quality + cleaning staff - overload.

## 6. Crisis Pool

- `Kuyruk Kapıya Taştı`
- `Garsonlar Yetişemiyor`
- `Kötü Yorum Patlaması`
- `Ucuz Et Skandalı`
- `Yemek App Siparişleri Birikti`
- `Denetim Geldi`
- `Rakip Fiyat Kırdı`
- `Kurye İşi Bıraktı`

## 7. Rival Behavior

Rakip fast food'da şu hamleleri sık kullanır:

- Fiyat kırma.
- Google yorum savaşı.
- Delivery kampanyası.
- Usta aşçı transferi.
- Ucuz tedarikle hızlı genişleme.

## 8. Build Examples

### Early Build: Dengeli Açılış

- Yeni Izgara
- Ek Garson
- Yerel Fırın
- Broşür Dağıt

Sonuç: Yavaş ama sağlıklı büyüme, düşük kriz riski.

### Mid Build: Agresif Delivery

- Yemek App Kampanyası
- Kurye Al
- Delivery Çantası
- Hızlı Servis Tezgahı

Sonuç: Demand hızlı artar; kurye ve mutfak capacity zayıfsa rating düşer.

### Late Build: Zincirleşme

- Şube Müdürü
- Toplu İçecek Anlaşması
- Usta Aşçı
- İkinci Şube Temp Effect'i

Sonuç: Market share artar, wage ve yönetim baskısı büyür.

## 9. Chain Scenarios

### Good Growth

Broşür + Ek Garson + Kaliteli Et -> demand artar ama quality korunur -> rating yükselir -> organic traffic artar.

### Overgrowth Collapse

Black Hat Review + Yemek App Kampanyası -> demand patlar -> mutfak yetmez -> kötü yorum -> müşteri rakibe akar.

### Risk Debt

Ucuz Tedarikçi + Sigortasız Çalışan -> cash korunur -> legal/hygiene risk birikir -> denetim veya skandal event'i.

## 10. Detailed Example Cards

| Kart | Tip | Slot Davranışı | Sahadaki Görünüm | Sistem Etkisi | Risk/Event Hook |
|---|---|---|---|---|---|
| Yeni Izgara | Install | Operation slotta kalır | Mutfakta yeni ızgara açılır | Capacity +, cash - | Kitchen pressure azalır |
| Ek Garson | Install | Staff slotta kalır | Garson NPC servise girer | Service capacity +, wage + | Underpaid ise quit risk |
| Kurye Al | Install | Staff slotta kalır | Kurye dış hatta görünür | Delivery capacity + | Traffic spike event |
| Kaliteli Et | Install | Supplier slotta kalır | Ürün daha iyi görünür | Quality +, cost + | Loyalty customer |
| Ucuz Tedarikçi | Risk | Supplier slotta kalır | Malzeme kasaları soluk görünür | Cost -, quality risk | Hygiene scandal |
| Google Ads | Policy | Marketing slotta kalır | District'te mavi çekim alanı | Demand +, recurring cost | Overcapacity |
| Yemek App Kampanyası | Policy | Marketing slotta kalır | Delivery order akışı | Delivery demand ++, margin - | Courier overload |
| Broşür Dağıt | Burst | Slot kaplamaz | Sokakta broşür animasyonu | Local demand + | Çöp/rahatsızlık şikayeti |
| Düşük Maaş Politikası | Policy/Risk | Staff policy slotunda kalır | Çalışanlarda stress aura | Wage cost -, stability drain | Staff revolt |
| Özür Menüsü | Reaction | Temp veya slot kaplamaz | Ücretsiz ikram animasyonu | Rating recovery, cash - | Honest brand tag |

## 11. Deep Venture Scenarios

### 11.1 İyi Büyüme: Dengeli Burger Akışı

Oyuncu `Yeni Izgara`, `Ek Garson`, `Kaliteli Et` ve `Broşür Dağıt` oynar. Izgara Operation slotunda, garson Staff slotunda, et Supplier slotunda kalır. Sahada mutfak hızlanır, servis daha akıcı olur, gri müşteriler yavaşça maviye döner. Demand capacity'yi çok aşmadığı için rating yükselir ve gold regular müşteri doğar.

### 11.2 Aşırı Büyüme Çöküşü: App Siparişleri Taştı

`Yemek App Kampanyası` Marketing slotunda kalır ve delivery order akışı başlatır. Kurye yoksa paketler kapıda birikir, mutfak pressure artar, mavi müşteriler pale blue olur. Event: `Yemek App Siparişleri Birikti`. Oyuncu kurye alırsa wage artar, kampanyayı kapatırsa demand düşer, görmezden gelirse rating kırılır.

### 11.3 Çalışan Krizi: Düşük Maaş Patladı

`Düşük Maaş Politikası` birkaç tur cash'i korur. Demand yükselince garson stress aura'sı kırmızıya döner. Kamera servis hattına iner; garson patrona gelir. Seçimler: maaşları yükselt, kriz çıkaranı kov, söz ver. Kovma capacity düşürür; söz verme `staff_resentment` tag'i bırakır.

### 11.4 Rakip Baskısı: Fiyat Savaşı

Rakip `Discount Week` hamlesi yapar. Rakip tarafında kırmızı tabela çıkar, district'te bazı gri müşteriler kırmızıya akar. Oyuncu `Kalite Sözü`, `Fiyat Eşle`, veya `Servisi Hızlandır` reaction seçenekleriyle cevap verir. Her seçenek market share, margin ve rating'i farklı etkiler.

### 11.5 Riskli Kısa Yol: Ucuz Et Skandalı

Oyuncu `Ucuz Tedarikçi` ile margin kazanır. Birkaç tur sonra sıcak hava trendi ve yüksek demand birleşirse `Ucuz Et Skandalı` tetiklenir. Müşteriler kırmızı review bırakır, denetim aracı district'e girer. Temiz çözüm pahalı supplier değiştirme; riskli çözüm sahte yorumla üstünü kapatma.
