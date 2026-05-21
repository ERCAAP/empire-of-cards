# GAME DESIGN DOCUMENT
# "Empire of Cards"

> Versiyon: 4.0 | Tarih: 2026-05-20
> Engine: Unity 6 (C#) | Platform: PC (Steam)
> Tür: Local business turf war card strategy

---

# 1. Yüksek Konsept

## 1.1 Oyuncu Vaadi

Oyuncu oyunun başında yerel bir işletme seçer ve aynı sektörde başlayan rakibine karşı aynı semtte, aynı müşteri havuzu içinde büyüme savaşı verir. Kart oynar ama his olarak kart oyunu değil, işletme yönetimi yaşar.

Bu oyunun fantezisi:

> "Küçük bir masadan başladım, doğru yatırımlar ve doğru risklerle semti ele geçirdim."

## 1.2 Oyunun Cümlesi

Küçük bir yerel işletmeyle başla, sektörüne özel kart havuzuyla büyü, müşteri memnuniyeti ile itibarını koru, rakibinle aynı mahalle pazarını paylaş ve market share savaşını kazan.

## 1.2.1 İlk Launch Surface Notu

İlk public yüzey ve pazarlama mesajı `local business turf war` etrafında kilitlenir.

İlk görünür venture üçlüsü:

- Restaurant
- Cafe
- Market

`Tech App` ve diğer daha uzak fantasy'ler teknik omurgada kalabilir, ancak ilk launch kimliğini belirlemez.

## 1.3 Tasarım İlkeleri

1. Kartlar karar temsil eder, büyü değil.
2. Sektör seçimi sadece kozmetik değildir; kart havuzunu, krizleri, büyüme yolunu ve rakip baskısını değiştirir.
3. Ortak omurga korunur; her venture kendi alt-slot diliyle farklılaşır.
4. Oyun ağır simülasyon değil, okunabilir kart-strateji hibritidir.
5. Başarılı hamleler zincir reaksiyon üretmeli, kötü planlama da yavaş çöküş yaratmalıdır.

## 1.4 Oyuncunun Her Tur Yaptığı Şey

1. Venture ve mevcut board durumuna uygun kartlar çeker.
2. Kartları slotlara yerleştirir veya anlık karar kartları oynar.
3. Sistem demand, capacity, quality, rating ve market share üzerinden çözülür.
4. Kriz varsa reaksiyon verir.
5. Rakibin aynı sektörde nasıl büyüdüğünü görür ve cevap üretir.

---

# 2. Venture Seçimi ve Rakip Kuralı

## 2.1 Açılış Kuralı

Run başında oyuncu 1 venture seçer.

İlk launch surface'te oyuncuya görünen set:

- Restaurant
- Cafe
- Market

Geniş venture omurgası teknik olarak korunabilir, ancak launch mesajı ve ilk seçim ekranı fiziksel yerel işletmelere odaklanır.

Rakip otomatik olarak aynı venture ile başlar.

Bu kararın sonuçları:

- Aynı müşteri havuzu için savaşılır.
- Aynı sektör krizleri iki tarafa da uygulanır.
- Kimin daha iyi kurduğu, daha doğru büyüdüğü ve daha temiz toparladığı görünür hale gelir.

## 2.2 Venture Seçiminin Etkilediği Sistemler

Seçilen venture şunları belirler:

- Starter deck
- Venture kart havuzu
- Alt-slot isimleri
- Türetilmiş metrikler
- Kriz havuzu
- Rakip davranış öncelikleri
- Görsel masa dili

## 2.3 Venture Dosyaları

Her venture'ın detaylı kuralları ayrı dosyadadır:

- `Assets/steam-card-game-gdd/businesses/fast_food.md`
- `Assets/steam-card-game-gdd/businesses/cafe.md`
- `Assets/steam-card-game-gdd/businesses/market_bakkal.md`

`tech_app.md` ve `giyim_magazasi.md` gelecek genişleme / post-launch venture yönleri olarak değerlendirilir.

---

# 3. Masa Düzeni

## 3.1 Üç Ana Bölge

Masa 3 ana bölgeye ayrılır:

1. Player Zone
2. District / Market Zone
3. Rival Zone

```text
┌──────────────────────────────────────────────┐
│ RIVAL ZONE                                   │
│ Rakibin slotları, kampanyaları, baskısı      │
├──────────────────────────────────────────────┤
│ DISTRICT / MARKET ZONE                       │
│ Müşteri akışı, trafik, trendler, olaylar     │
│ Görünürlük, organik talep, market paylaşımı  │
├──────────────────────────────────────────────┤
│ PLAYER ZONE                                  │
│ Senin işletmen, slotların, çalışanların       │
│ aktif kampanyaların ve krizlerin             │
└──────────────────────────────────────────────┘
```

## 3.2 Player Zone Amacı

Player Zone oyuncuya "bu benim işletmem" hissini vermelidir. Kartlar sadece sayaç değil, işletmenin gerçek parçaları gibi görünmelidir.

Örnek bağlar:

- Operation kartları fiziksel işletmeye oturur.
- Staff kartları operasyonun üstüne bağlanır.
- Marketing kartları District Zone'a görsel etki yollar.
- Supplier kartları üretim veya stok tarafına bağlanır.
- Temp Effect kartları işletmeyi bozulan veya baskı altındaki durumda gösterir.

## 3.3 District / Market Zone Amacı

Bu bölge oyunun rekabet kalbidir. Şunları temsil eder:

- Trafik
- Organik talep
- Görünürlük
- Yerel etkinlikler
- Sezon etkisi
- Rakip baskısı
- Müşteri yön değiştirmesi

Oyuncu burada şunu okumalı:

> "Pazar bana mı akıyor, rakibe mi kayıyor?"

## 3.4 Rival Zone Amacı

Rakip oyuncuyla aynı venture içinde büyür. Oyuncu rakibin:

- hangi alt-slotları doldurduğunu,
- kalite mi fiyat mı oynadığını,
- agresif marketing mi premium kalite mi seçtiğini,
- hangi krizden zarar gördüğünü

okuyabilmelidir.

---

# 4. Ortak Slot Omurgası

## 4.1 Ana Slot Aileleri

Tüm venture'lar aynı 5 ana slot ailesini kullanır:

1. `Operation`
2. `Staff`
3. `Marketing`
4. `Supplier`
5. `Temp Effect`

Bu ortak omurga teknik ve tasarım uyumu için sabittir.

## 4.2 Başlangıç Slot Sayıları

| Slot Türü | Başlangıç | Amaç |
|---|---:|---|
| Operation | 4 | Fiziksel veya çekirdek kapasite |
| Staff | 5 | Operasyonu ayakta tutan ekip |
| Marketing | 3 | Aynı anda yönetilen aktif büyüme baskısı |
| Supplier | 2 | Kalite/maliyet kararları |
| Temp Effect | 3 | Krizler, geçici trendler, geçici boost/penalty |

Başlangıç toplamı: 17 slot

## 4.3 Slot Felsefesi

Oyuncu her şeyi aynı anda yapamaz. Bu sınırlama stratejiyi doğurur.

Slot baskısının ürettiği kararlar:

- Yeni masa mı eklemeliyim, yoksa yeni aşçı mı almalıyım?
- Premium tedarik mi seçeyim, düşük maliyet mi?
- Google Ads mi açayım, influencer mı alayım, broşür mü bastırayım?
- Krizi hemen çözeyim mi, büyümeyi zorlayıp riski göze mi alayım?

## 4.4 Temp Effect Kuralı

Risk/illegal kartlar ve reaction kartları ayrı bir altıncı slot ailesi açmaz.

Onların çalışma mantığı:

- bazıları tek atımlık karar olarak çözülür,
- bazıları mevcut slotları etkiler,
- bazıları `Temp Effect` slotuna düşerek birkaç tur baskı yaratır.

Bu sayede masa karmaşıklaşmaz ama risk hissi korunur.

## 4.5 Slot Genişlemesi

Slot büyümesi venture kimliğine göre yorumlanır ama ana mantık aynıdır:

- Erken oyun: çekirdek işletme kurulur
- Orta oyun: uzmanlaşma ve darboğaz açma
- Geç oyun: şubeleşme, zincirleşme, ölçek

Örnek genel açılım:

- İlk büyük eşik: `Marketing +1`
- İkinci büyük eşik: `Operation +2`, `Staff +2`
- Üçüncü büyük eşik: `Supplier +1`, `Marketing +1`

---

# 5. Venture'a Özel Alt-Slotlar

## 5.1 Ortak Kural

Her venture aynı `SlotType` omurgasını kullanır ama alt-slot isimleri ve kuralları farklıdır.

Bu fark tasarımın çekirdeğidir: oyuncu her sektörde farklı işletme dili konuştuğunu hissetmelidir.

## 5.2 Fast Food Alt-Slotları

- Operation: Mutfak, Servis, Oturma, Delivery
- Staff: Asci, Kasiyer, Kurye, Temizlik, Sube Muduru
- Marketing: Brosur, Google, Sosyal Medya, Yemek App Kampanyasi
- Supplier: Kasap, Manav, Ekmek/Firinci, Icecek Anlasmasi

## 5.3 Cafe Alt-Slotları

- Operation: Bar, Oturma, Takeaway, Vitrin/Pastane
- Staff: Barista, Kasiyer, Floor Staff, Temizlik, Vardiya Sorumlusu
- Marketing: Instagram, Reels, Loyalty, Google Maps
- Supplier: Kahve, Sut, Tatli/Pastane, Ambiyans Is Ortakligi

## 5.4 Tech App Alt-Slotları

- Operation: Urun, Backend, Growth Pipeline, Support/Platform Ops
- Staff: Developer, Designer, Growth, Support, Product Manager
- Marketing: ASO, Performance Ads, Influencer, Community
- Supplier: Cloud, Tooling, Payment/Analytics, API Partner

## 5.5 Giyim Alt-Slotları

- Operation: Vitrin, Raf/Stok, Kasa, Online Siparis
- Staff: Satis Danismani, Kasiyer, Terzi, Depo, Mağaza Muduru
- Marketing: Instagram, Influencer, Indirim, Shopping Ads
- Supplier: Toptanci, Atolye, Kargo, Fotograf/Cekim Partneri

## 5.6 Market / Bakkal Alt-Slotları

- Operation: Raf, Taze Urun, Kasa, Mahalle Servis / WhatsApp
- Staff: Kasiyer, Reyon, Taze Urun Sorumlusu, Kurye, Vardiya
- Marketing: WhatsApp, Mahalle Afişi, Sadakat, Gece Acik
- Supplier: Hal, Distribütör, Mandıra/Fırın, Icecek Partneri

---

# 6. Kart Aileleri ve Kart Akışı

## 6.1 Kart Aileleri

Her venture kendi kart havuzunu aşağıdaki ailelerle kurar:

1. Kalıcı kurulum kartları
2. Aktif büyüme kartları
3. Risk / illegal kartları
4. Reaksiyon / çözüm kartları

## 6.2 Kalıcı Kurulum Kartları

Bunlar işletmenin omurgasını kurar:

- dükkan
- masa
- espresso makinesi
- backend upgrade
- vitrin sistemi
- buzdolabı

Çoğu `Operation`, `Staff` veya `Supplier` slotuna bağlanır.

## 6.3 Aktif Büyüme Kartları

Bunlar demand ve visibility üretir:

- broşür kampanyası
- influencer
- Google reklamı
- ASO iyileştirmesi
- indirim haftası
- yemek platform anlaşması

Çoğu `Marketing` slotunda kalır veya kısa süreli boost verir.

## 6.4 Risk / Illegal Kartları

Bunlar kısa vadeli güç sağlar ama orta vadede baskı üretir:

- sahte yorum
- sigortasız çalışan
- vergi kaçırma
- kalitesiz tedarik
- fake specialty iddiası
- store policy etrafından dolanma

Bu kartlar `Legal Risk`, `Reputation`, `Staff Stability` veya `Temp Effect` baskısı oluşturur.

## 6.5 Reaksiyon / Çözüm Kartları

Bunlar kriz geldikten sonra toparlama araçlarıdır:

- özür kampanyası
- tedarikçi değişimi
- acil işe alım
- ustayı değiştirme
- ücretsiz ikram
- refund / hotfix / recall

## 6.6 Venture'a Göre Kart Dağıtımı

Kart akışı venture ve board state'e göre filtrelenir:

- Erken oyun: kurulum ve temel hayatta kalma
- Orta oyun: uzmanlaşma, ilk krizler, rakibe cevap
- Geç oyun: zincir, franchise, ölçek veya platform dominasyonu

## 6.7 Board-State Aware Çekme Mantığı

Kart sistemi tamamen rastgele olmamalıdır. Aşağıdaki baskılar kart havuzunu etkiler:

- Capacity demand'in gerisinde kaldıysa toparlayıcı operasyon/staff kartları
- Rating düştüyse çözüm kartları
- Legal Risk arttıysa denetim ve savunma kartları
- Supplier zayıfsa kalite veya maliyet baskılı anlaşmalar

---

# 7. Ortak Stat Sistemi

## 7.1 Ana Statlar

Her venture şu ana statları kullanır:

- `Cash`
- `Demand`
- `Capacity`
- `Quality`
- `Reputation/Rating`
- `Staff Stability`
- `Legal Risk`
- `Market Share`

## 7.2 Stat Anlamları

### Cash

Anlık büyüme hızı ve kriz çözme kapasitesidir.

### Demand

Gelen müşteri veya kullanıcı isteğidir. Marketing, word of mouth ve rating ile yükselir.

### Capacity

Talebi gerçekten karşılayabilme gücüdür. Operation ve Staff ile oluşur.

### Quality

Ürün ya da hizmet kalitesidir. Supplier, staff seviyesi ve altyapı ile belirlenir.

### Reputation / Rating

Müşterinin seni dışarıdan nasıl gördüğüdür. Google yıldızı, app store skoru, yorum algısı ve sosyal proof bunun yüzüdür.

### Staff Stability

Ekibin ne kadar sağlıklı çalıştığını gösterir. Fazla mesai, maaş baskısı ve yetersiz kadro bunu düşürür.

### Legal Risk

Kısa yol kullanmanın zamanla biriken cezasıdır.

### Market Share

Kazanmaya en yakın ana metriktir. Talep çekmek yetmez; o talebi sürdürülebilir şekilde elde tutmak gerekir.

## 7.3 Türetilmiş Kurallar

Ana zincir kuralı:

```text
Demand > Capacity
→ servis/teslim gecikir
→ memnuniyet düşer
→ rating düşer
→ organik demand düşer
→ rakip market share çalar
```

Kalite zinciri:

```text
Supplier + Staff + Operation
→ Quality
→ memnuniyet
→ review/rating
→ organik büyüme
```

Risk zinciri:

```text
Illegal kısa yol
→ Legal Risk
→ denetim veya skandal
→ Reputation kaybı / para cezası / temp effect
```

---

# 8. Venture'a Özel Türetilmiş Metrikler

## 8.1 Fast Food ve Cafe

- Malzeme Kalitesi
- Servis Hizi
- Hijyen
- Google Puani

## 8.2 Tech App

- App Stability
- Store Rating
- Churn
- Infra Cost

## 8.3 Giyim Magazasi

- Stok Sagligi
- Sezon Uyumu
- Iade Baskisi
- Vitrin Cekiciligi

## 8.4 Market / Bakkal

- Fire
- SKT Baskisi
- Veresiye Riski
- Mahalle Sadakati

Bu metrikler ana statlardan türetilir; oyuncunun önüne ikinci bir karmaşık ekonomi sayfası olarak değil, karar sonuçlarını okunur hale getiren sektör lensi olarak çıkmalıdır.

---

# 9. Tur Akışı

## 9.1 Fazlar

Her tur aşağıdaki sırayla oynanır:

1. Draw
2. Planning
3. Play
4. Resolve
5. Crisis / Reaction
6. Rival
7. End of Turn Market Update

## 9.2 Draw

Oyuncu mevcut venture, board state ve oyun evresine göre kart çeker.

## 9.3 Planning

Oyuncu darboğazı okur:

- Demand mi eksik?
- Capacity mi eksik?
- Rating mi düşüyor?
- Supplier tarafı mı zayıf?
- Staff Stability mi kırılıyor?

## 9.4 Play

Oyuncu sınırlı sayıda aksiyonla kartları oynar:

- Slota yerleştirir
- Var olan slotu upgrade eder
- Anlık kart çözer
- Krize cevap verir

## 9.5 Resolve

Sistem şu sırayla hesaplanır:

1. Operation ve Staff kapasitesi
2. Supplier etkileri
3. Marketing görünürlüğü ve demand üretimi
4. Quality ve service sonucu
5. Rating / reputation güncellemesi
6. Cash gelir/gider hesabı
7. Market share kayması

## 9.6 Crisis / Reaction

Kriz kartları iki kaynaktan gelir:

- venture bazlı global olaylar
- oyuncunun kendi kararlarının tetiklediği olaylar

Örnek:

- düşük kalite + yüksek demand = kötü yorum patlaması
- sigortasız çalışan + yüksek denetim baskısı = ceza
- ucuz tedarik + sıcak hava = hijyen sorunu

## 9.7 Rival

Rakip aynı venture üzerinden karar verir. Oyuncunun seçtiği stratejiye göre cevap üretir:

- fiyat savaşı
- kalite yükseltme
- marketing baskısı
- staff poach
- itibar saldırısı

---

# 10. Ekonomi Döngüsü ve Kriz Çözümü

## 10.1 Temel Ekonomi Döngüsü

```text
Kurulum
→ talep yaratma
→ talebi taşıyacak kapasite kurma
→ kalite ve rating'i koruma
→ market share büyütme
→ yeni slot / yeni ölçek
```

## 10.2 Oyunun Gerçek Gerginliği

Oyuncu genelde üç problemden biriyle uğraşır:

1. Talep yokluğu
2. Talebi taşıyamama
3. Kısa yol yüzünden büyürken çürüme

## 10.3 Kriz Çözme Felsefesi

Her kriz en az iki çözüm yolu vermelidir:

- ucuz ama riskli
- pahalı ama temiz

Fast food örneği:

- Google yorum satın al
- daha kaliteli et ve yeni usta ile kalıcı çözüm kur

Tech örneği:

- sahte review ve paid traffic ile şişir
- hotfix, support yatırımı ve crash azaltımı yap

## 10.4 Toparlanma Hissi

Kriz kartları oyunu bitiren cezalar değil, oyuncuyu yön zorlayan dönemeçler olmalıdır. Oyuncu doğru toparlarsa daha güçlü dönebilmelidir.

---

# 11. Fast Food Referans Akışı

Fast food venture v4 tasarımının referans örneğidir.

## 11.1 Erken Oyun Örneği

Başlangıç:

- küçük dükkan
- sınırlı cash
- düşük Google puanı
- temel mutfak

İlk kararlar:

- dükkan satın al
- masa ekle
- usta işe al
- kasap ve manav bul
- Google işletme profili aç

## 11.2 İlk Büyüme Kararları

- broşür bastır
- ışıklarda broşür dağıtacak eleman tut
- yemek uygulamasıyla anlaş
- paket servisi aç
- sosyal medya reklamı ver

## 11.3 Baskı Noktaları

- talep artar ama mutfak yetişmez
- servis yavaşlar
- yorumlar düşer
- puan 4.4'ten 3.9'a iner
- organik müşteri kesilir

## 11.4 Kriz Sonrası Seçim Örnekleri

- sahte yorum satın al: hızlı düzeltme, ban riski
- kaliteli et anlaşması yap: pahalı ama kalıcı kalite artışı
- ustayı değiştir: staff yatırımı
- ücretsiz tatlı veya özür kampanyası yap: itibar toparlama

## 11.5 Fast Food'un Kimliği

Fast food venture yüksek tempo ve kapasite yönetimi venture'ıdır. Yanlış marketing ile en hızlı patlayan, doğru denge ile de en hızlı büyüyen sektördür.

---

# 12. Rakip Sistemi

## 12.1 Aynı Venture Rakip Kuralı

Rakip oyuncuyla aynı venture'da olduğu için adil ama baskılı bir yarış kurulur.

## 12.2 Rakip Davranış Aileleri

Rakip aşağıdaki stratejiler arasında döner:

- Agresif marketing
- Premium kalite
- Ucuz genişleme
- Defansif stabilizasyon
- Riskli kısa yol

## 12.3 Venture Bazlı Baskı Örnekleri

- Fast food: fiyat kırma, yorum savaşı, kurye baskısı
- Cafe: barista çalma, ambiyans savaşı, Google Maps üstünlüğü
- Tech: CAC yarışı, store rating savaşı, feature velocity baskısı
- Giyim: indirim savaşı, vitrin savaşı, influencer yarışı
- Market: fiyat baskısı, gece açık kalma farkı, WhatsApp sadakati

## 12.4 Görünürlük Kuralı

Rakip tüm detaylarıyla açık olmayabilir; ama oyuncu onun hangi yönde büyüdüğünü okuyabilmelidir.

---

# 13. Kazanma Yapısı

## 13.1 Ana Hedef

Kazanma yapısı kısa run 1v1 aynı-sektör market domination olarak kalır.

Varsayılan hedef:

- 25 tur içinde market share üstünlüğü kurmak
- belirli eşik: `%60 market share`

## 13.2 Alternatif Başarı Hissi

Run bitse bile skor şu eksenlerde okunur:

- market share
- final cash
- rating
- çözülen kriz sayısı
- ulaşılan ölçek seviyesi

## 13.3 Kaybetme Şartları

- iflas
- itibarın geri dönülemez çökmesi
- ağır legal darbe
- rakibin sektör içinde ezici dominasyonu

---

# 14. İçerik Katmanları

## 14.1 Ana GDD

Bu dosya global kuralları anlatır.

## 14.2 Venture Dokümanları

Her venture dosyası şunları içerir:

- venture kimliği
- başlangıç board'u
- alt-slot yapısı
- kart aileleri
- kriz havuzu
- erken/orta/geç oyun stratejileri

## 14.3 Teknik Eşleme

Doküman kurallarının koda nasıl taşınacağı şu dosyada tutulur:

- `Assets/steam-card-game-gdd/TECHNICAL_MAPPING.md`

---

# 15. Kabul Kriterleri

## 15.1 Doküman Kriterleri

- Ana GDD tek başına oyunun temel döngüsünü anlatır.
- 5 venture dosyasının her biri başlangıç board'u, kart akışı, krizleri ve büyüme yolunu içerir.
- Her venture için en az bir erken, bir orta ve bir geç oyun build örneği vardır.

## 15.2 Sistem Kriterleri

- Oyuncu venture seçince rakip aynı venture ile eşleşir.
- Kart çekimi venture ve board state ile ilişkili çalışır.
- Marketing operasyonu aşarsa rating baskısı doğar.
- Supplier ve staff kararları quality ve memnuniyet zinciri üretir.
- Rating artışı organik talep üretir.
- Rating düşüşü kriz ve çözüm kartlarını tetikler.

## 15.3 Playtest Senaryoları

- Fast food: agresif marketing, zayıf mutfak, yorum çöküşü
- Cafe: küçük ama premium kalite build'i
- Tech: düşük erken gelir, yüksek geç ölçek
- Giyim: yanlış sezon stok baskısı
- Market: düşük marj, stabil trafik, veresiye riski

---

# 16. MVP Önceliği

## 16.1 Tasarım Önceliği

MVP'de amaç tüm venture'ları aynı derinlikte bitirmek değil; aynı sistem diliyle oynanabilir hale getirmektir.

Öncelik sırası:

1. Venture seçimi
2. Aynı venture rakip
3. Ortak slot omurgası
4. Venture bazlı kart havuzu
5. Rating, quality, capacity zinciri
6. Venture bazlı krizler

## 16.2 Referans Venture

Fast food, sistemin okunabilirliğini test etmek için referans venture'dır. Diğer venture'lar aynı şablonla ama kendi ekonomik kimliğiyle uygulanır.

---

# 17. Uygulama Notu

Bu GDD sürümü kodu anlatan bir teknik taslak değil; tasarımın nihai omurgasıdır. Veri modeli, UI ve rival AI eşlemesi `TECHNICAL_MAPPING.md` içinde karar seviyesinde tanımlanır.
