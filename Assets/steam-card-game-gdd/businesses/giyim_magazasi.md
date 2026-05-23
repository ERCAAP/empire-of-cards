# Venture: Giyim Mağazası

## 1. Identity

Giyim Mağazası sezon, stok, vitrin ve influencer etkisiyle oynar. Büyük demand spike yaratabilir, fakat yanlış stok ve iade baskısı cash'i hızla eritir.

Fantasy:

> "Küçük butik mağazadan trend belirleyen bir moda zincirine dönüştüm."

## 2. Starting Board

- Küçük mağaza
- Vitrin alanı
- Temel raf/stok
- 1 satış danışmanı
- 1 kasiyer
- Rating: 4.0
- Stok sağlığı orta

## 3. Slot Language

| Slot | Alt-slotlar |
|---|---|
| Operation | Vitrin, Raf/Stok, Kasa, Online Sipariş |
| Staff | Satış Danışmanı, Kasiyer, Terzi, Depo, Mağaza Müdürü |
| Marketing | Instagram, Influencer, İndirim, Shopping Ads |
| Supplier | Toptancı, Atölye, Kargo, Fotoğraf/Çekim Partneri |
| Temp Effect | Stok Açığı, İade Baskısı, Sezon Kaçırma, Viral Ürün |

## 4. Card Families

### Build

- `Vitrin Yenile`: foot traffic +
- `Stok Sistemi`: stock health +
- `Online Sipariş Kanalı`: demand +, kargo pressure +
- `Hızlı Kasa`: checkout capacity +

### Staff

- `Satış Danışmanı`: conversion +
- `Terzi`: quality/fit +
- `Depo Personeli`: stock handling +
- `Mağaza Müdürü`: stability +

### Marketing

- `Instagram Lookbook`: demand +
- `Influencer Kombini`: spike demand +
- `Sezon İndirimi`: stock clear +, margin -
- `Shopping Ads`: online demand +

### Supplier

- `Kaliteli Atölye`: quality +, cost +
- `Ucuz Toptancı`: cost -, return risk +
- `Hızlı Kargo`: online satisfaction +
- `Profesyonel Çekim`: marketing efficiency +

### Risk / Reaction

- `Fake Scarcity`: demand +, trust risk
- `Acil Stok Tamamlama`: cash -, stock +
- `İade Politikası Esnet`: reputation +, cash -
- `Trend Kopyası`: demand +, legal/reputation risk

## 5. Derived Metrics

- `Stok Sağlığı`: stock system + supplier reliability - demand spike.
- `Sezon Uyumu`: trend match + marketing timing.
- `İade Baskısı`: low quality + bad fit + online demand.
- `Vitrin Çekiciliği`: visual merchandising + marketing.

## 6. Crisis Pool

- `Viral Ürün Stokta Yok`
- `Yanlış Beden Şikayetleri`
- `İade Dalgası`
- `Sezonu Kaçırdın`
- `Kargo Gecikmesi`
- `Rakip Influencer Kaptı`
- `Tedarikçi Kalite Sorunu`

## 7. Rival Behavior

- Influencer yarışı.
- İndirim savaşı.
- Vitrin üstünlüğü.
- Hızlı sezon dönüşü.
- Online shopping ads baskısı.

## 8. Build Examples

### Early Build: Butik Kalite

- Kaliteli Atölye
- Satış Danışmanı
- Vitrin Yenile
- Instagram Lookbook

Sonuç: Conversion ve rating güçlü, büyüme kontrollü.

### Mid Build: Viral Moda

- Influencer Kombini
- Online Sipariş Kanalı
- Hızlı Kargo
- Depo Personeli

Sonuç: Demand spike, stok ve iade baskısı.

### Late Build: Zincir Mağaza

- Stok Sistemi
- Mağaza Müdürü
- Shopping Ads
- Sezon İndirimi

Sonuç: Büyük hacim, margin ve stok optimizasyonu oyunu.

## 9. Chain Scenarios

### Good Growth

Vitrin + kaliteli atölye + satış danışmanı -> conversion + rating -> sadık müşteri.

### Overgrowth Collapse

Influencer + düşük stok -> ürün kalmaz -> müşteriler kırmızıya döner -> iade ve rating baskısı.

### Risk Debt

Fake scarcity + trend kopyası -> demand boost -> güven kaybı veya legal event.

## 10. Detailed Example Cards

| Kart | Tip | Slot Davranışı | Sahadaki Görünüm | Sistem Etkisi | Risk/Event Hook |
|---|---|---|---|---|---|
| Vitrin Yenile | Install | Operation slotta kalır | Vitrin değişir | Foot traffic + | Rival copy |
| Stok Sistemi | Install | Operation slotta kalır | Raf/etiket düzeni | Stock health + | Setup cost |
| Online Sipariş Kanalı | Install | Operation slotta kalır | Paket hattı | Demand + | Kargo pressure |
| Satış Danışmanı | Install | Staff slotta kalır | NPC müşteri yönlendirir | Conversion + | Poach risk |
| Terzi | Install | Staff slotta kalır | Terzi köşesi | Fit quality +, returns - | Wage cost |
| Influencer Kombini | Burst/Risk | Temp effect | Viral kıyafet akışı | Demand spike | Stock shortage |
| Sezon İndirimi | Policy | Marketing slotta kalır | İndirim tabelaları | Stock clear +, margin - | Price expectation |
| Kaliteli Atölye | Install | Supplier slotta kalır | Ürün kalitesi artar | Quality +, cost + | Slow restock |
| Fake Scarcity | Risk | Marketing/Temp | "Son ürün" marker | Demand + | Trust damage |
| İade Politikası Esnet | Reaction/Policy | Temp veya Marketing | İade bankosu | Reputation +, cash - | Abuse risk |

## 11. Deep Venture Scenarios

### 11.1 İyi Büyüme: Butik Kalite

`Kaliteli Atölye`, `Satış Danışmanı`, `Terzi` ve `Vitrin Yenile` birlikte çalışır. Müşteriler daha yavaş ama yüksek memnuniyetle gelir. İade baskısı düşer, rating artar, sadık müşteri segmenti oluşur.

### 11.2 Aşırı Büyüme Çöküşü: Influencer Stok Patlaması

`Influencer Kombini` demand spike üretir. Stok sistemi yoksa raflar boşalır, beden şikayetleri çıkar. Event: `Viral Ürün Stokta Yok`. Oyuncu acil toptancı, ön sipariş veya kampanyayı kapatma arasında seçim yapar.

### 11.3 Çalışan Krizi: Kasa ve Deneme Kabini Baskısı

Yoğun kampanya sırasında satış danışmanı ve kasiyer yetersiz kalır. Deneme kabini önü kalabalıklaşır, çalışan stress ikonları artar. Oyuncu geçici staff alabilir, indirim temposunu düşürebilir veya müşterileri bekletebilir.

### 11.4 Rakip Baskısı: Influencer Kaptırma

Rakip aynı influencer'ı kapar. District'te kırmızı moda akışı belirir, oyuncu vitrinindeki bazı gri müşteriler rakibe döner. Oyuncu özgün koleksiyon, fiyat indirimi veya mikro-influencer ağıyla cevap verir.

### 11.5 Riskli Kısa Yol: Fake Scarcity Güven Kaybı

`Fake Scarcity` hızlı satış üretir. Müşteriler ürünün aslında bol olduğunu fark ederse güven kırılır. Review balonları "aldatıldık" tonuna döner. Özür + iade politikası toparlar, inkar etmek uzun vadeli brand trust düşürür.
