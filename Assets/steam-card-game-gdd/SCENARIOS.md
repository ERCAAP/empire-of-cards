# Empire of Cards Scenario Library

> Bu dosya kart kararlarının sahaya, simülasyona ve event zincirlerine nasıl aktığını örnekler.

## Scenario 1: Black Hat Growth Collapse

### Setup

- Venture: Fast Food
- Turn: 2
- Player rating: 4.1
- Capacity: düşük
- Staff stability: orta
- Rakip: defansif kalite stratejisi

### Card

`Black Hat Review Boost`

Etki:

- Kısa vadede demand artar.
- Rating yüzeysel olarak yükselir.
- `fake_reviews` ve `overpromised_quality` tag'i eklenir.

### Visible Board Effect

- District'te gri müşteriler hızla oyuncuya döner.
- Review balonları mavi parlar ama bazıları glitch efekti taşır.
- İşletme önünde kuyruk büyür.

### Resolve

```text
Demand > Capacity
-> Kitchen pressure artar
-> Garson stress yükselir
-> Servis gecikir
-> Bazı mavi müşteriler pale blue olur
```

### Event

`Garsonlar Yetişemiyor`

Kamera mutfak/servis kaosuna iner. Garson patrona yürür.

Seçenekler:

- `Ek vardiya ücreti ver`: cash gideri artar, staff stability toparlanır.
- `Acil eleman çağır`: kısa vadeli capacity artar, quality düşebilir.
- `İdare edin`: cash korunur, `staff_resentment` tag'i eklenir.

### New Pressure

- Ek ödeme seçilirse: `wage_cost_up`
- Acil eleman seçilirse: `temporary_quality_risk`
- Geçiştirme seçilirse: `staff_quit_risk`

## Scenario 2: Underpaid Staff Quit

### Setup

- Oyuncu önceki turlarda `Düşük Maaş Politikası` seçmiştir.
- Demand yükselmiştir.
- Staff stability 28'e düşmüştür.

### Trigger

Hard trigger: staff stability < 30.

### Micro-Cinematic

- Kamera servis hattına iner.
- İsimli çalışan tepsiyi bırakır.
- İki çalışan onun arkasında toplanır.
- Event paneli açılır.

### Choice

- `Maaşları yükselt`: cash gideri sürekli artar, stability +25.
- `Kriz çıkaranları kov`: iki staff çıkar, capacity -20, stability +5.
- `Prim sözü ver`: immediate cost yok, `broken_promise_risk` tag'i eklenir.

### Consequence

Kovma seçilirse:

```text
Staff count düşer
-> Capacity düşer
-> Queue büyür
-> Müşteri delay artar
-> Rating -0.3
-> Market share -4
```

## Scenario 3: Tech App Bad UX Launch

### Setup

- Venture: Tech App
- Oyuncu growth ads almıştır.
- Product/design yatırımı zayıftır.

### Card

`Aggressive Launch Campaign`

### Visible Board Effect

- District yerine app user stream görünür: gri kullanıcı ikonları mavi onboarding hattına akar.
- Support kanalında ticket balonları birikir.
- App store panelinde rating yıldızları titrer.

### Resolve

```text
Demand yüksek
-> App stability yetersiz
-> Support capacity düşük
-> Churn artar
-> Store rating düşer
```

### Event

`Kötü UI/UX Yorumları Patladı`

Seçenekler:

- `Hotfix sprint`: staff stress +, stability risk, app stability +.
- `Designer hire`: cash -, quality +, uzun vadeli rating recovery.
- `Fake positive reviews`: rating kısa boost, legal/platform risk +.

## Scenario 4: Cafe Premium Slow Growth

### Setup

- Venture: Cafe
- Oyuncu premium kahve supplier ve iyi barista seçmiştir.
- Marketing zayıftır.

### Result

- Demand düşük/orta.
- Quality yüksek.
- Rating yavaş yükselir.
- Gold loyal customers oluşur.

### Event

`Sadık Müşteri Tavsiyesi`

Bir regular müşteri arkadaşını getirir. Kamera masaya kısa odaklanır.

Seçenekler:

- `Loyalty kartı başlat`: repeat demand +, küçük cash cost.
- `Instagram çekimi yap`: visibility +, staff stress +.
- `Premium fiyatı artır`: cash/customer +, demand risk.

## Scenario 5: Giyim Influencer Stock Crash

### Setup

- Venture: Giyim Mağazası
- Oyuncu influencer campaign oynar.
- Stok sağlığı düşük.

### Visible Board Effect

- District'te gri müşteriler mağazaya koşar.
- Raflar boşalır.
- Deneme kabini ve kasa önünde kalabalık oluşur.

### Resolve

```text
Demand spike
-> Stock shortage
-> Wrong size complaints
-> Returns pressure
-> Rating düşüşü
```

### Event

`Viral Oldun Ama Stok Yetmedi`

Seçenekler:

- `Acil toptancı anlaşması`: cash -, stok +, kalite riski.
- `Ön sipariş aç`: demand korunur, delay riski.
- `Kampanyayı kapat`: demand düşer, reputation hasarı sınırlanır.

## Scenario 6: Market Veresiye Cash Trap

### Setup

- Venture: Market / Bakkal
- Mahalle sadakati yüksek.
- Cash düşük.
- Veresiye tag'i birikmiş.

### Trigger

Cash düşük + `veresiye_risk` tag'i.

### Micro-Cinematic

- Kasada defter açılır.
- Sadık müşteri ödeme yapmadan ürün alır.
- Kasiyer patrona bakar.

### Choices

- `Veresiyeyi kapat`: sadakat düşer, cash akışı stabilize olur.
- `Limit koy`: orta çözüm, bazı müşteriler grileşir.
- `Devam et`: sadakat korunur, cash kriz riski artar.

## Scenario 7: Rival Price War

### Setup

- Oyuncu market share'de yüzde 55'e ulaşmıştır.
- Rakip ucuz genişleme stratejisine geçer.

### Rival Move

`Rival Discount Week`

### Visible Board Effect

- Rakip işletmede kırmızı indirim tabelaları çıkar.
- District'te bazı gri ve pale blue müşteriler kırmızıya döner.
- Player tarafında market share barı baskı uyarısı verir.

### Player Reaction Cards

- `Quality Promise`: sadık müşteri tutma.
- `Match Discount`: cash margin düşer, demand korunur.
- `Ignore and Improve Service`: kısa vadede market share kaybı, uzun vadede rating.

## Scenario 8: Holding Exit Offer

### Setup

- Scale stage: Holding Candidate
- Market share: 65+
- Rating: 4.3+
- Cash pozitif

### Event

`Yatırım Fonu Satın Alma Teklifi Verdi`

### Choices

- `Exit yap`: run biter, meta sermaye kazanılır.
- `Devam et ve büyüt`: daha yüksek hedef, daha güçlü rakip baskısı.
- `Kısmi yatırım al`: cash boost, karar özgürlüğü ve kâr payı tradeoff'u.

### Visible Consequence

Exit yapılırsa district'te marka tabelası büyür, kamera holding ölçeğine açılır, sonra yeni startup seçim ekranına geçilir.

---

# Full Turn Scenario Templates

Bu bölüm senaryoları prototip için uygulanabilir akışa indirir. Her örnek kartın nasıl seçildiğini, slotta nasıl durduğunu, dünyada nasıl göründüğünü ve sonraki baskıyı nasıl doğurduğunu gösterir.

## Full Scenario A: Fast Food Black Hat + Düşük Maaş + Garson Quit

### 1. Setup

- Turn: 3
- Player venture: Fast Food
- Active cards: `Düşük Maaş Politikası` Staff policy slotunda, `Google Ads` Marketing slotunda.
- Empty slots: Staff 1 boş, Marketing dolu.
- Demand: 62
- Capacity: 44
- Rating: 4.2
- Staff stability: 38
- Rival strategy: premium quality.

### 2. Card Offer

Oyuncuya şu kartlar gelir:

- `Black Hat Review Boost` - Risk/Burst.
- `Ek Garson` - Install/Staff.
- `Yeni Izgara` - Install/Operation.
- `Özür Menüsü` - Reaction, henüz aktif kriz olmadığı için pasif görünüyor.

### 3. Player Choice

Oyuncu `Black Hat Review Boost` seçer. Kart kalıcı slot kaplamaz; Temp Effect ve risk tag bırakır.

### 4. Slot Placement

- Temp Effect slotunda `Fake Review Surge` 2 tur görünür.
- `fake_reviews` ve `overpromised_quality` tag'leri run state'e eklenir.
- `Düşük Maaş Politikası` Staff policy slotunda kalmaya devam eder.

### 5. World Manifestation

- Rating yıldızları kısa süre mavi parlar.
- District'te gri müşteriler oyuncuya hızla akar.
- Review balonlarının bazıları glitch'li görünür.
- Mutfakta ve servis hattında hızlanma animasyonları başlar.

### 6. Resolve

```text
Demand 62 -> 82
Capacity 44 sabit kalır
Overload +38
Staff stress +18
Rating önce +0.2 görünür, sonra servis gecikmesi yüzünden -0.4 risk kazanır
```

### 7. Event Trigger

Hard/soft trigger birleşir:

- Demand capacity'nin yüzde 180'ine yaklaşır.
- Staff stability düşük maaş policy yüzünden hızla düşer.
- `staff_quit_risk` oluşur.

Event: `Garsonlar İşi Bırakmak Üzere`

### 8. Micro-Cinematic

Kamera servis hattına iner. Garson tepsiyi bırakır, iki kızgın müşteri kapıya yönelir. Garson patrona yürür:

> "Bu tempoda bu maaşla çalışmam."

### 9. Choices

- `Maaşı yükselt`: wage cost +, staff stability +25, `underpaid_staff` tag'i kalkar.
- `Yeni garson al, bunu gönder`: Staff slot değişimi, capacity kısa süre -8 sonra +12, staff stability -10.
- `Bir tur daha dayan`: cash korunur, `staff_walkout_pending` tag'i eklenir.

### 10. Visible Consequence

Oyuncu "Bir tur daha dayan" seçerse:

- Garson sahaya döner ama üstünde kırmızı stress halkası kalır.
- İki mavi müşteri pale blue olur.
- Rakip tarafında kırmızı müşteri çekim çizgisi belirir.

### 11. New Pressure

- Sonraki tur kart havuzu staff/reaction ağırlıklı olur.
- Rating düşüşü daha olasıdır.
- Eğer capacity çözülmezse garson gerçekten işi bırakır ve Staff slot kartı yanarak çıkar.

## Full Scenario B: Cafe Viral Reels + Barista Burnout

### 1. Setup

- Venture: Cafe
- Active cards: `Uzman Barista`, `Specialty Kahve`, `Rahat Oturma Alanı`.
- Marketing slotu boş.
- Demand düşük ama rating 4.5.
- Bar capacity orta.

### 2. Card Offer

- `Reels Trend`
- `Floor Staff`
- `Google Maps Optimizasyonu`
- `Barista Eğitim Günü`

### 3. Player Choice

Oyuncu `Reels Trend` seçer. Kart Burst/Risk tipindedir.

### 4. Slot Placement

- Marketing slotunu kalıcı doldurmaz.
- Temp Effect slotuna `Viral Reels` 1 tur girer.
- `viral_attention` tag'i eklenir.

### 5. World Manifestation

- Cafe önünde telefonla video çeken müşteriler belirir.
- District'te gri müşteriler hızla maviye döner.
- Barista'nın çalışma animasyonu hızlanır.

### 6. Resolve

```text
Demand spike
Bar capacity yetersiz
Dwell time yüksek olduğu için masa dönüşü yavaş
Barista stress +22
Rating delayed risk
```

### 7. Event Trigger

Event: `Barista Burnout`

Kamera barista'nın yanlış sipariş hazırlamasına ve derin nefes almasına odaklanır.

### 8. Choices

- `Floor Staff al`: Staff slotu dolar, service speed +, wage +.
- `Kampanyayı yavaşlat`: Viral temp effect biter, demand düşer, rating korunur.
- `Baristayı fazla mesaiye zorla`: Bu tur capacity +, `burnout_debt` tag'i.

### 9. Consequence

Fazla mesai seçilirse müşteri akışı korunur ama barista kartında stress/durability göstergesi kırmızıya döner. Bir sonraki burnout event'i daha sert gelir ve barista quit veya kalite düşüşü doğurabilir.

### 10. New Pressure

- Staff slotu ve wage economy kritik hale gelir.
- Sadık müşteriler bekleme süresine daha dayanıklıdır, yeni müşteriler hızlı kaçar.

## Full Scenario C: Tech App Bad UX Launch + Support Backlog

### 1. Setup

- Venture: Tech App
- Active cards: `Performance Ads`, `Cheap Cloud`.
- Eksik: UX Designer, Support Hire.
- Store rating: 4.1.
- Cash orta.

### 2. Card Offer

- `Influencer Launch`
- `UX Designer`
- `Support Hire`
- `Fake Store Reviews`

### 3. Player Choice

Oyuncu `Influencer Launch` seçer.

### 4. Slot Placement

- Temp Effect slotuna `Viral Surge` girer.
- Marketing slotu doluysa `Performance Ads` aktif kalır; iki büyüme etkisi üst üste biner.

### 5. World Manifestation

- User stream hızla maviye akar.
- Server node ısınır.
- Support panelinde ticket balonları üst üste çıkar.
- App store yıldızları titrer.

### 6. Resolve

```text
New users +++
Backend stability Cheap Cloud yüzünden düşer
Support backlog artar
UX friction churn üretir
Store rating -0.5 risk kazanır
```

### 7. Event Trigger

Event: `Kötü UI/UX Yorumları Patladı`

### 8. Choices

- `UX Designer işe al`: Staff slotu dolar, quality recovery başlar, cash/wage -.
- `Hotfix sprint`: Reaction, crash/backlog kısa düzelir, developer stress +.
- `Fake Store Reviews`: Risk, rating kısa boost, platform audit tag.

### 9. Consequence

Fake review seçilirse store rating paneli mavi parlar ama glitch yıldızlar kalır. Kullanıcı akışı bir tur korunur. `platform_audit_pending` tag'i ileride audit event'i tetikler.

### 10. New Pressure

- Product quality veya support çözülmezse churn kalıcı hale gelir.
- Rakip feature copy veya clean UX kampanyasıyla kırmızı pull kazanır.

## Full Scenario D: Giyim Influencer Spike + Stok Yetmezliği

### 1. Setup

- Venture: Giyim Mağazası
- Active cards: `Vitrin Yenile`, `Kaliteli Atölye`.
- Stok sistemi yok.
- Staff: 1 satış danışmanı, 1 kasiyer.

### 2. Card Offer

- `Influencer Kombini`
- `Stok Sistemi`
- `Depo Personeli`
- `Sezon İndirimi`

### 3. Player Choice

Oyuncu `Influencer Kombini` seçer.

### 4. Slot Placement

- Temp Effect slotuna `Viral Ürün` girer.
- `influencer_spike` tag'i eklenir.

### 5. World Manifestation

- Vitrindeki ürün parlamaya başlar.
- District'ten mavi müşteri akışı hızlanır.
- Raflardaki ürünler görsel olarak azalır.
- Deneme kabini ve kasa önü kalabalıklaşır.

### 6. Resolve

```text
Demand spike
Stock health hızla düşer
Checkout capacity zorlanır
Wrong size complaints başlar
Returns pressure oluşur
```

### 7. Event Trigger

Event: `Viral Oldun Ama Stok Yetmedi`

### 8. Choices

- `Acil toptancı anlaşması`: Supplier temp, stok +, quality risk.
- `Ön sipariş aç`: Demand korunur, delay pressure.
- `Kampanyayı kapat`: Demand düşer, reputation hasarı sınırlanır.

### 9. Consequence

Acil toptancı seçilirse raflar dolar ama ürün renk/kalite dili biraz solar. İade baskısı event hook olarak kalır.

### 10. New Pressure

- Stok sistemi veya depo personeli alınmazsa bir sonraki spike daha ağır çöker.
- Rakip influencer hamlesi oyuncunun stok zafiyetini kullanabilir.

## Full Scenario E: Market Veresiye Sadakati + Cash Trap

### 1. Setup

- Venture: Market / Bakkal
- Active cards: `Sadakat Defteri`, `Yerel Fırın/Mandıra`.
- Cash düşük.
- Mahalle sadakati yüksek.
- `veresiye_risk` tag'i 2 turdur aktif.

### 2. Card Offer

- `Veresiye Aç`
- `Hızlı Kasa`
- `Acil Nakit Borcu`
- `Fire Temizliği`

### 3. Player Choice

Oyuncu kısa vadede müşteri kaybetmemek için `Veresiye Aç` seçer.

### 4. Slot Placement

- Policy olarak Marketing/Sadakat slotuna bağlanır.
- Mevcut `Sadakat Defteri` ile merge olabilir: repeat loyalty ++, cash risk ++.

### 5. World Manifestation

- Kasada defter animasyonu görünür.
- Regular müşteriler gold accent kazanır.
- Cash sayaç pulse ile düşer.

### 6. Resolve

```text
Repeat traffic artar
Immediate cash düşer
Revenue delayed olur
Cash threshold kritik seviyeye iner
```

### 7. Event Trigger

Event: `Veresiye Defteri Şişti`

### 8. Choices

- `Veresiyeyi kapat`: cash discipline +, loyalty -.
- `Limit koy`: cash risk azalır, bazı gold müşteriler pale blue olur.
- `Devam et`: loyalty korunur, bankruptcy pressure +.

### 9. Consequence

Limit koyulursa kasada küçük tartışma animasyonu olur. Bazı gold müşteriler griye döner, ama cash drain yavaşlar.

### 10. New Pressure

- Sonraki kart havuzu cash recovery, supplier margin ve sadakat onarımına kayar.
- Rakip indirim market hamlesi sadakati kırma fırsatı bulur.
