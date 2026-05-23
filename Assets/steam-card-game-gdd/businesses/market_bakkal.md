# Venture: Market / Bakkal

## 1. Identity

Market/Bakkal düşük marj, stabil trafik, mahalle sadakati ve nakit akışı venture'ıdır. En dramatik krizleri hızlı büyümeden çok küçük kararların birikmesi üretir: veresiye, fire, SKT, düşük marj ve gece açık kalma baskısı.

Fantasy:

> "Mahalle bakkalından güvenilen yerel market zincirine büyüdüm."

## 2. Starting Board

- Küçük raf alanı
- Taze ürün köşesi
- 1 kasa
- 1 kasiyer
- 1 reyon çalışanı
- Rating: 4.1
- Sadakat orta
- Cash düşük ama trafik stabil

## 3. Slot Language

| Slot | Alt-slotlar |
|---|---|
| Operation | Raf, Taze Ürün, Kasa, Mahalle Servis / WhatsApp |
| Staff | Kasiyer, Reyon, Taze Ürün Sorumlusu, Kurye, Vardiya |
| Marketing | WhatsApp, Mahalle Afişi, Sadakat, Gece Açık |
| Supplier | Hal, Distribütör, Mandıra/Fırın, İçecek Partneri |
| Temp Effect | Veresiye Baskısı, SKT Riski, Fire, Nakit Sıkışması |

## 4. Card Families

### Build

- `Ek Raf`: capacity/product variety +
- `Taze Ürün Dolabı`: quality +, fire risk if unmanaged
- `Hızlı Kasa`: queue -
- `WhatsApp Sipariş Hattı`: local demand +, kurye pressure +

### Staff

- `Ek Kasiyer`: checkout capacity +
- `Reyon Görevlisi`: stock health +
- `Taze Ürün Sorumlusu`: fire/SKT risk -
- `Kurye`: delivery capacity +
- `Vardiya Sorumlusu`: stability +

### Marketing

- `Mahalle Afişi`: local awareness +
- `Sadakat Defteri`: repeat customer +, veresiye risk +
- `Gece Açık`: demand +, staff stress +
- `WhatsApp Kampanyası`: delivery/local demand +

### Supplier

- `Güvenilir Hal`: quality +, cost +
- `Ucuz Distribütör`: margin +, SKT/quality risk +
- `Yerel Fırın/Mandıra`: loyalty +, morning traffic +
- `İçecek Partneri`: margin +

### Risk / Reaction

- `Veresiye Aç`: loyalty +, cash risk +
- `SKT'li Ürün İndirimi`: cash recovery, reputation risk
- `Acil Nakit Borcu`: cash +, future cost
- `Fire Temizliği`: cash -, quality/reputation recovery

## 5. Derived Metrics

- `Fire`: taze ürün demand mismatch + poor staff.
- `SKT Baskısı`: ucuz distribütör + düşük stok sistemi.
- `Veresiye Riski`: loyalty tools + low cash discipline.
- `Mahalle Sadakati`: rating + repeat service + local supplier.

## 6. Crisis Pool

- `Veresiye Defteri Şişti`
- `Taze Ürün Fire Verdi`
- `SKT Şikayeti`
- `Kasa Kuyruğu`
- `Gece Vardiyası Yoruldu`
- `Rakip İndirim Market Açtı`
- `WhatsApp Siparişleri Yetişmedi`

## 7. Rival Behavior

- Fiyat baskısı.
- Gece açık kalma farkı.
- Sadakat kampanyası.
- WhatsApp teslimat yarışı.
- Ucuz distribütörle margin savaşı.

## 8. Build Examples

### Early Build: Güvenilir Mahalle Marketi

- Yerel Fırın/Mandıra
- Reyon Görevlisi
- Mahalle Afişi
- Hızlı Kasa

Sonuç: Stabil traffic, düşük kriz.

### Mid Build: WhatsApp Servisi

- WhatsApp Sipariş Hattı
- Kurye
- WhatsApp Kampanyası
- Ek Raf

Sonuç: Demand artar; kurye ve stok yönetimi baskısı.

### Late Build: Yerel Zincir

- Vardiya Sorumlusu
- Taze Ürün Dolabı
- Güvenilir Hal
- İçecek Partneri

Sonuç: Sadakat ve margin artar, operasyon karmaşıklığı yükselir.

## 9. Chain Scenarios

### Good Growth

Yerel supplier + hızlı kasa + sadakat -> repeat traffic -> stable cash.

### Overgrowth Collapse

WhatsApp kampanyası + kurye yetersizliği -> teslimat gecikmesi -> rating düşüşü -> müşteri rakibe akar.

### Risk Debt

Veresiye aç + cash düşük -> nakit krizi -> borç veya sadakat kırma kararı.

## 10. Detailed Example Cards

| Kart | Tip | Slot Davranışı | Sahadaki Görünüm | Sistem Etkisi | Risk/Event Hook |
|---|---|---|---|---|---|
| Ek Raf | Install | Operation slotta kalır | Yeni raf açılır | Variety/capacity + | Stock complexity |
| Taze Ürün Dolabı | Install | Operation slotta kalır | Sebze/meyve dolabı | Quality + | Fire/SKT risk |
| Hızlı Kasa | Install | Operation slotta kalır | Kasa hattı hızlanır | Queue - | Maintenance |
| Ek Kasiyer | Install | Staff slotta kalır | Kasiyer NPC | Checkout capacity + | Wage cost |
| Kurye | Install | Staff slotta kalır | Mahalle teslimatı | Delivery capacity + | Traffic stress |
| Sadakat Defteri | Policy | Marketing slotta kalır | Defter/regular marker | Repeat + | Veresiye risk |
| Gece Açık | Policy/Risk | Marketing slotta kalır | Gece ışıkları | Demand + | Staff stress |
| Güvenilir Hal | Install | Supplier slotta kalır | Taze ürün teslimatı | Quality +, cost + | Margin pressure |
| SKT'li Ürün İndirimi | Risk/Burst | Temp effect | İndirimli sepet | Cash recovery | Reputation risk |
| Fire Temizliği | Reaction | Slot kaplamaz | Raf temizleme | Quality recovery, cash - | Waste event close |

## 11. Deep Venture Scenarios

### 11.1 İyi Büyüme: Güvenilir Mahalle Akışı

`Yerel Fırın/Mandıra`, `Hızlı Kasa`, `Reyon Görevlisi` ve `Mahalle Afişi` stabil trafik üretir. Müşteriler hızlı girip çıkar, sadakat artar, market share yavaş ama güvenli büyür.

### 11.2 Aşırı Büyüme Çöküşü: WhatsApp Siparişleri Taştı

`WhatsApp Sipariş Hattı` ve `WhatsApp Kampanyası` local delivery demand'i artırır. Kurye yoksa sipariş balonları birikir, müşteri şikayeti artar. Oyuncu kurye alır, kampanyayı sınırlar veya geç teslimatı kabul eder.

### 11.3 Çalışan Krizi: Gece Vardiyası Yoruldu

`Gece Açık` demand sağlar ama vardiya staff yoksa çalışan stress artar. Kamera gece ışıkları altında yorgun kasiyere iner. Seçimler: vardiya sorumlusu al, gece açık süresini kısalt, çalışanları zorla.

### 11.4 Rakip Baskısı: İndirim Market Açıldı

Rakip ucuz fiyatla kırmızı müşteri akışı yaratır. Oyuncu fiyat eşleyebilir, mahalle sadakatini güçlendirebilir veya taze ürün kalitesine oynayabilir. Fiyat eşlemek margin'i ezer; kalite oynamak kısa vadede market share kaybettirebilir.

### 11.5 Riskli Kısa Yol: Veresiye Cash Trap

`Sadakat Defteri` ve `Veresiye Aç` mahalle bağlılığını artırır. Cash düşerse kasa defteri event'i tetiklenir. Veresiyeyi kapatmak sadakati düşürür; limit koymak orta çözüm; devam etmek iflas riskini büyütür.
