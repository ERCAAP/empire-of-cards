# Venture: Cafe

## 1. Identity

Cafe daha yavaş tempolu ama sadakat, ambiyans ve kalite üzerinden güçlü büyüyen venture'dır. Fast Food'a göre demand spike daha düşük, müşteri bağlılığı ve premium fiyat potansiyeli daha yüksektir.

Fantasy:

> "Küçük bir köşe kahvecisini şehirde aranan bir marka haline getirdim."

## 2. Starting Board

- Küçük bar
- 3 masa
- 1 barista
- 1 kasiyer/floor staff
- Rating: 4.2
- Düşük/orta demand
- Nötr müşteri havuzu yüksek

## 3. Slot Language

| Slot | Alt-slotlar |
|---|---|
| Operation | Bar, Oturma, Takeaway, Vitrin/Pastane |
| Staff | Barista, Kasiyer, Floor Staff, Temizlik, Vardiya Sorumlusu |
| Marketing | Instagram, Reels, Loyalty, Google Maps |
| Supplier | Kahve, Süt, Tatlı/Pastane, Ambiyans İş Ortaklığı |
| Temp Effect | Barista Burnout, Masa Bekleme, Viral Reels, Ambiyans Şikayeti |

## 4. Card Families

### Build

- `Yeni Espresso Makinesi`: bar capacity +, quality +
- `Rahat Oturma Alanı`: dwell time +, seat pressure +
- `Takeaway Penceresi`: quick demand +, staff pressure +
- `Tatlı Vitrini`: basket size +, supplier dependency +

### Staff

- `Uzman Barista`: quality ++, wage +
- `Floor Staff`: service speed +
- `Vardiya Sorumlusu`: stability +
- `Temizlik Personeli`: hygiene/ambiance +

### Marketing

- `Instagram Çekimi`: visibility +
- `Reels Trend`: demand spike, staff pressure +
- `Sadakat Kartı`: repeat customer +
- `Google Maps Optimizasyonu`: organic demand +

### Supplier

- `Specialty Kahve`: quality +, cost +
- `Ucuz Süt`: cost -, quality risk
- `Yerel Pastane`: basket +, loyalty +
- `Dekor Partneri`: ambiance +

### Risk / Reaction

- `Fake Specialty Claim`: demand +, reputation risk
- `Özür Kahvesi`: angry customer recovery
- `Barista Eğitim Günü`: quality +, one-turn capacity -
- `Sessiz Saatler Kampanyası`: loyal customer +, demand smoothing

## 5. Derived Metrics

- `Ambiyans`: seating + cleanliness + decor.
- `Kahve Kalitesi`: barista + coffee supplier.
- `Masa Döngüsü`: seating capacity vs dwell time.
- `Sadakat`: rating + loyalty + repeat customers.

## 6. Crisis Pool

- `Barista Burnout`
- `Masa Bekleme Şikayeti`
- `Kahve Kalitesi Tartışması`
- `Viral Oldun, Bar Yetmedi`
- `Google Maps Rakip Baskısı`
- `Ambiyans Bozuldu`

## 7. Rival Behavior

- Barista transferi.
- Ambiyans yarışı.
- Google Maps rating savaşı.
- Influencer/reels baskısı.
- Premium kahve pozisyonlaması.

## 8. Build Examples

### Early Build: Premium Küçük Cafe

- Specialty Kahve
- Uzman Barista
- Rahat Oturma Alanı
- Google Maps Optimizasyonu

Sonuç: Yavaş büyür, rating ve sadakat güçlü olur.

### Mid Build: Viral Cafe

- Reels Trend
- Tatlı Vitrini
- Floor Staff
- Takeaway Penceresi

Sonuç: Demand artar; servis yetişmezse ambiyans ve rating düşer.

### Late Build: Marka Cafe

- Vardiya Sorumlusu
- Dekor Partneri
- Sadakat Kartı
- İkinci Lokasyon Event'i

Sonuç: Sadık müşteri tabanı güçlü, yönetim ve wage baskısı artar.

## 9. Chain Scenarios

### Good Growth

Specialty Kahve + Barista + Loyalty -> gold customers -> stable organic demand.

### Overgrowth Collapse

Reels Trend + küçük bar -> kuyruk -> barista burnout -> kötü yorum -> müşteri rakibe döner.

### Risk Debt

Fake Specialty Claim -> premium demand -> kahve meraklısı müşteri yakalar -> expose event -> rating darbesi.

## 10. Detailed Example Cards

| Kart | Tip | Slot Davranışı | Sahadaki Görünüm | Sistem Etkisi | Risk/Event Hook |
|---|---|---|---|---|---|
| Yeni Espresso Makinesi | Install | Operation slotta kalır | Barda yeni makine | Bar capacity +, quality + | Maintenance cost |
| Uzman Barista | Install | Staff slotta kalır | İsimli barista NPC | Quality ++, wage + | Burnout if overloaded |
| Rahat Oturma Alanı | Install | Operation slotta kalır | Yeni masa/koltuklar | Dwell +, loyalty + | Seat pressure |
| Takeaway Penceresi | Install | Operation slotta kalır | Dış servis penceresi | Quick demand + | Staff pressure |
| Specialty Kahve | Install | Supplier slotta kalır | Kahve paketleri/ritüel | Quality +, cost + | Coffee nerd loyalty |
| Reels Trend | Burst/Risk | Temp effect bırakır | Telefon/viral ikonları | Demand spike | Bar overload |
| Sadakat Kartı | Policy | Marketing slotta kalır | Gold regular marker | Repeat demand + | Loyalty dependency |
| Google Maps Optimizasyonu | Policy | Marketing slotta kalır | Harita pin pull | Organic demand + | Rival maps war |
| Fake Specialty Claim | Risk | Marketing/Temp | Parlak ama glitch label | Demand + | Expose event |
| Özür Kahvesi | Reaction | Slot kaplamaz | Ücretsiz kahve ikramı | Angry customer recovery | Honest brand tag |

## 11. Deep Venture Scenarios

### 11.1 İyi Büyüme: Sadık Mahalle Cafe'si

Oyuncu `Specialty Kahve`, `Uzman Barista`, `Rahat Oturma Alanı` ve `Sadakat Kartı` kurar. Kartlar slotta kalır, sahada barista ritmi ve gold regular müşteriler görünür. Demand yavaş artar ama rating stabil yükselir. Rakip indirim yapsa bile sadık müşteriler hemen kırmızıya dönmez.

### 11.2 Aşırı Büyüme Çöküşü: Reels Viral Oldu

`Reels Trend` anlık burst olarak demand spike üretir. Bar capacity düşükse sıra dışarı taşar, barista stress yükselir. Event: `Viral Oldun, Bar Yetmedi`. Oyuncu takeaway penceresi açabilir, kampanyayı yavaşlatabilir veya baristayı fazla mesaiye zorlayabilir.

### 11.3 Çalışan Krizi: Barista Burnout

Uzman barista yüksek kalite üretir ama tek başına uzun süre yüksek demand taşırsa burnout event'i doğar. Kamera barista'nın latte art'ı bozmasına ve patrona yürüyüşüne odaklanır. Eğitim günü capacity'yi düşürür ama stability kazandırır; yeni barista almak cash'i zorlar.

### 11.4 Rakip Baskısı: Google Maps Yarışı

Rakip Google Maps rating kampanyası açar, kırmızı harita pinleri district'te parlar. Oyuncu `Google Maps Optimizasyonu`, `Özür Kahvesi` veya `Instagram Çekimi` ile cevap verir. Yanlış cevap kısa vadede demand'i artırıp barı tekrar zorlayabilir.

### 11.5 Riskli Kısa Yol: Fake Specialty Expose

Oyuncu `Fake Specialty Claim` ile premium müşteri çeker. Kahve meraklısı bir müşteri kalite farkını fark ederse expose event'i tetiklenir. Seçimler: gerçek specialty supplier'a geç, özür yayınla, yorumu bastır. Bastırmak legal/reputation risk'i büyütür.
