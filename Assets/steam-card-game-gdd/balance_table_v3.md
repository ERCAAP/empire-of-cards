# BALANCE TABLE v3
# Empire of Cards — 95 Kart Ekonomik Değerleri
> Versiyon: 3.0 | Tarih: 2026-05-20
> Tasarımcı: Economy Designer #1 — Money Flow

---

## TASARIM PRENSİPLERİ

Net gelir formülü:
```
Net Income = Σ(operation.income × multipliers) + combo_bonuses
           - Σ(staff.salary) - tax(15%) - legalRiskPenalty - fire/neglect
```

Müşteri ağırlıkları: Quality 30%, Price 20%, PlatformRating 20%, Marketing 15%, Speed 10%, Loyalty 5%

Hedef ROI = 3-5 tur. Maaş/gelir oranı hedefi = %30-40. Başlangıç parası 500.

---

## BÖLÜM 1 — FAST FOOD (FFC01-FFC15)

### Venture profili
Net marj %3-15. Düşük unit economics, yüksek hacim. Servis hızı kritik.
Başlangıç geliri düşük tutulur; erken game'de kapasite > müşteri sorununa dikkat.

---

### 1a. OPERATION KARTLARI (4 kart)

| ID | İsim | buyCost | incomePerTurn | customersPerTurn | qualityScore | priceScore | serviceSpeedScore | activationDelay | Açıklama |
|----|------|---------|---------------|------------------|--------------|------------|-------------------|-----------------|----------|
| FFC01 | Temel Tezgah | 120 | 60 | 3 | 3 | 7 | 5 | 0 | Başlangıç kurulumu. Düşük kalite, hızlı. Erken game için temel kart. ROI: 2 tur → kasıtlı ucuz, oyuncuyu hızlı ayağa kaldırmak için. |
| FFC02 | Mangal İstasyonu | 200 | 110 | 4 | 6 | 6 | 6 | 0 | Özel lezzet = kalite sıçraması. ROI: 1.8 tur ama daha yüksek kalite müşteri çarpanı. Mid-game geçişi için tasarlandı. |
| FFC03 | Paket Servis İstasyonu | 280 | 140 | 5 | 4 | 7 | 9 | 0 | Hız odaklı. Servis hızı skoru yüksek → müşteri ağırlığında hız %10 katkı. Delivery platformuna zemin hazırlar. ROI: 2 tur. |
| FFC04 | Merkez Mutfak | 420 | 200 | 6 | 7 | 5 | 7 | 1 | Geç game operasyonu. 1 tur aktivasyon gecikmesi var (kurulum). İncomePerTurn yüksek ama büyük yatırım. ROI: 2.1 tur. Franchise yolunda kritik. |

**Neden bu değerler?** FF net marj %3-15. Temel Tezgah 60/tur ile başlangıç dengesi sağlar; 500 nakit ile alınabilir (120), ilk tur Ucuz Çalışan (FFS01, 150) ile birleşince sisteme girilir. Mangal İstasyonu'nun yüksek kalite skoru, müşteri çarpanındaki %30 quality ağırlığından gelir yaratır.

---

### 1b. STAFF KARTLARI (4 kart)

| ID | İsim | buyCost | salaryPerTurn | customerBonus | synergyBonus | Açıklama |
|----|------|---------|---------------|---------------|--------------|----------|
| FFS01 | Ucuz Çalışan | 150 | 20 | 1 | 0 | Temel işleri halleder. Düşük maaş (20) = maaş/gelir oranı 20/60 = %33, hedef aralıkta. Sinerji yok — başka venture'da değersiz. |
| FFS02 | Deneyimli Kasiyer | 220 | 35 | 2 | 6 | Hız +, hata azalır. Sinerji: FF ortamında kasiyerin %10 hız katkısı müşteri kaybını önler. Maaş/gelir = 35/110 = %32, dengeli. |
| FFS03 | Aşçıbaşı | 300 | 55 | 3 | 8 | Kalite sıçraması. FF'de kalite %30 ağırlığında, bu çalışan en yüksek sinerji bonusu verir. Maaş/gelir = 55/140 = %39, sınırda ama kalite getirisi dengeler. |
| FFS04 | Kurye | 180 | 30 | 2 | 5 | Paket Servis İstasyonu aktifken sinerji patlar. Tek başına zayıf, kombinasyonda güçlü — slot yönetimi kararı zorlaştırır. Maaş %25 = yeterince ucuz. |

---

### 1c. MARKETING KARTLARI (3 kart)

| ID | İsim | buyCost | platformRatingGain | marketingScoreKatkı | Açıklama |
|----|------|---------|--------------------|--------------------|----------|
| FFM01 | Broşür Kampanyası | 80 | 0.0 | 2 | Sıfır platform rating etkisi — sadece anlık müşteri. Düşük maliyet. Erken game için uygun. Tur başı +2 müşteri, bu 5+ müşteri aralığı için yeterli. |
| FFM02 | Sosyal Medya Reklamı | 130 | 0.2 | 3 | Platform rating hafif yükselir. Orta maliyet. Müşteri ağırlığında marketing %15 — bu kart o ağırlığı besler. |
| FFM03 | Google Reklam Tabelası | 180 | 0.4 | 5 | Kalıcı görünürlük. Rating kazancı 0.4 = 2 tur içinde platformu 3.0 → 4.0'a çıkarabilir (diğer bonuslarla). Pahalı ama uzun vadeli return. |

---

### 1d. SUPPLIER KARTLARI (2 kart)

| ID | İsim | buyCost | qualityScoreKatkı | costReductionPercent | Açıklama |
|----|------|---------|-------------------|---------------------|----------|
| FFP01 | Kasap Anlaşması | 200 | 2 | 10% | Kalite +2 → müşteri çarpanına %30 ağırlıkla direkt katkı. Maliyet %10 azalır = net gelir iyileşir. Ucuz hammadde spiralinin antidotu. ROI: 3 tur. |
| FFP02 | Toplu Gıda Distribütörü | 150 | 1 | 20% | Daha ucuz ama kalite katkısı yarı. Düşük fiyat skoru için: maliyet azalırsa margin artar, fiyat skoru +1 dolaylı. Erken game maliyet baskısı için. ROI: 2.5 tur. |

---

### 1e. EVENT KARTLARI (2 kart)

| ID | İsim | eventDuration | eventMultiplier | negatifEtki |
|----|------|---------------|-----------------|-------------|
| FFE01 | Yemek Festivali | 2 | +40% | Yok. Pozitif event — kaliteli oynanış için ödül. Operasyon hazır değilse kapasiteyi aşar → servis çöker → sonraki tur rating -0.3 (zincirleme). |
| FFE02 | Hammadde Krizi | 2 | -25% | Ucuz tedarikçi varsa etki -35%. Maliyet +%20 bu süre. Ucuz Tedarikçi seçiminin gizli bedeli — oyuncu başta görmez, 4-5. turda patlar. |

---

## BÖLÜM 2 — CAFE (CAF01-CAF15)

### Venture profili
Net marj %10-20. Yüksek ürün marjı ama yüksek sabit gider. Kalite ve platform rating çok kritik.
Barista sinerji kombosu core mekanik.

---

### 2a. OPERATION KARTLARI (4 kart)

| ID | İsim | buyCost | incomePerTurn | customersPerTurn | qualityScore | priceScore | serviceSpeedScore | activationDelay |
|----|------|---------|---------------|------------------|--------------|------------|-------------------|-----------------|
| CAF01 | Temel Espresso Makinesi | 150 | 80 | 3 | 4 | 6 | 5 | 0 |
| CAF02 | Specialty Kahve İstasyonu | 250 | 140 | 4 | 7 | 4 | 6 | 0 |
| CAF03 | Teras Genişleme | 320 | 160 | 6 | 5 | 6 | 5 | 0 |
| CAF04 | La Marzocco Makine | 480 | 240 | 5 | 10 | 3 | 7 | 1 |

**Neden bu değerler?** Cafe'nin unit income FF'den %25-30 yüksek (net marj farkı). Temel Espresso 80/tur ile 150 maliyet = ROI 1.9 tur — başlangıç makul. La Marzocco kalite 10/10 → müşteri çarpanı %30 quality weight = maksimum çarpan = 480 maliyet haklı. PriceScore 3 = pahalı kahve, kalite hassas müşteri çeker, fiyat hassas kaybeder — bilinçli trade-off.

---

### 2b. STAFF KARTLARI (4 kart)

| ID | İsim | buyCost | salaryPerTurn | customerBonus | synergyBonus |
|----|------|---------|---------------|---------------|--------------|
| CAF05 | Stajyer Barista | 160 | 22 | 1 | 0 |
| CAF06 | Barista | 240 | 42 | 2 | 6 |
| CAF07 | SCA Sertifikalı Barista | 350 | 65 | 4 | 8 |
| CAF08 | Kafe Müdürü | 280 | 50 | 2 | 7 |

**Neden bu değerler?** SCA Barista maaş 65/tur. Gelir 140-240/tur. Oran = 65/190 (orta gelir) = %34. Hedef %30-40 içinde. Sinerji 8 = specialty kahve + La Marzocco ile en yüksek combo puanı. Stajyer maaş 22 = çok ucuz ama sinerji 0, kalite düşürür → kasıtlı trade-off.

---

### 2c. MARKETING KARTLARI (3 kart)

| ID | İsim | buyCost | platformRatingGain | marketingScoreKatkı |
|----|------|---------|--------------------|--------------------|
| CAF09 | Instagram Post Kampanyası | 90 | 0.3 | 3 |
| CAF10 | Latte Art Yarışması | 110 | 0.5 | 4 |
| CAF11 | Mikro-Influencer İşbirliği | 170 | 0.2 | 5 |

**Neden?** Cafe için Instagram en güçlü kanal. Latte Art Yarışması = barista sinerjisi ile ek 0.5 rating, rakibi geride bırakmak için en hızlı yol. PlatformRating müşteriye %20 ağırlıkla etki eder; 0.5 kazanç = dikkat çekici.

---

### 2d. SUPPLIER KARTLARI (2 kart)

| ID | İsim | buyCost | qualityScoreKatkı | costReductionPercent |
|----|------|---------|-------------------|---------------------|
| CAF12 | Specialty Kahve Tedarikçisi | 220 | 3 | 0% |
| CAF13 | Barista Sütü Anlaşması | 160 | 2 | 0% |

**Neden?** Specialty tedarikçi ROI: kalite +3 → müşteri çarpanı artışı = yaklaşık 3 tur. 0% cost reduction çünkü premium girdi = daha pahalı, bu doğru. Ucuz blend temel kartı değil — oyuncu premium yolunu seçmek zorunda.

---

### 2e. EVENT KARTLARI (2 kart)

| ID | İsim | eventDuration | eventMultiplier | negatifEtki |
|----|------|---------------|-----------------|-------------|
| CAF14 | Kahve Çılgınlığı Trendi | 2 | +50% | Kapasitesi yoksa servis çöker. Yüksek gelir ama yönetilemezse rating düşer. |
| CAF15 | Barista Ayrıldı | 3 | -30% | SCA Barista yokken tetiklenirse kalite -2, Instagram akışı durur, rating -0.4/tur. |

---

## BÖLÜM 3 — TECH APP (TEC01-TEC15)

### Venture profili
Net marj %20-60. Ölçeklenebilir ama başta 2-3 tur sıfır gelir (delay). Churn sistemi var.
En zor venture — gecikme ve platform komisyonu double penalty.

---

### 3a. OPERATION KARTLARI (4 kart)

| ID | İsim | buyCost | incomePerTurn | customersPerTurn | qualityScore | priceScore | serviceSpeedScore | activationDelay |
|----|------|---------|---------------|------------------|--------------|------------|-------------------|-----------------|
| TEC01 | MVP Lansmanı | 100 | 0 | 0 | 3 | 8 | 6 | 2 |
| TEC02 | Freemium Sistemi | 200 | 90 | 4 | 5 | 9 | 7 | 1 |
| TEC03 | Abonelik Modeli | 320 | 180 | 4 | 7 | 5 | 8 | 1 |
| TEC04 | Kurumsal SaaS | 450 | 260 | 3 | 8 | 4 | 9 | 2 |

**Neden bu değerler?** MVP Lansmanı = 0 income, 2 tur delay. Oyuncu ilk 2 turda para kazanmaz, nakit kriz baskısı hisseder — bu kasıtlı zorluk. Freemium = yüksek müşteri (priceScore 9) ama düşük income; churn modeli burada çalışır. Abonelik = recurring revenue, dengeli. SaaS = geç game enterprise, platform komisyonu (%30) bu income'dan düşüldükten sonra gerçek return 182/tur — ROI 2.5 tur.

**Tech için platform komisyonu notu:** incomePerTurn değerleri platform komisyonu kesilmeden ÖNCE gross değerlerdir. Net = gross × 0.70 (iOS) veya × 0.85 (Android).

---

### 3b. STAFF KARTLARI (4 kart)

| ID | İsim | buyCost | salaryPerTurn | customerBonus | synergyBonus |
|----|------|---------|---------------|---------------|--------------|
| TES01 | Junior Developer | 180 | 28 | 1 | 0 |
| TES02 | Senior Developer | 300 | 55 | 2 | 7 |
| TES03 | Growth Hacker | 260 | 48 | 3 | 6 |
| TES04 | CTO (Teknik Direktör) | 400 | 80 | 2 | 8 |

**Neden?** CTO maaş 80/tur = yüksek ama SaaS gelir 260 ile oran 80/260 = %31. Sinerji 8 = abonelik sistemi + SaaS ile combo. Growth Hacker = customerBonus 3 = en yüksek müşteri katkısı, marketing rolü + staff slota yerleşir.

---

### 3c. MARKETING KARTLARI (3 kart)

| ID | İsim | buyCost | platformRatingGain | marketingScoreKatkı |
|----|------|---------|--------------------|--------------------|
| TEM01 | ASO Optimizasyonu | 90 | 0.4 | 4 |
| TEM02 | Product Hunt Lansmanı | 140 | 0.3 | 5 |
| TEM03 | Google/Meta Reklam | 160 | 0.1 | 3 |

**Neden?** ASO = App Store'da organik indirme artışı → platformRating 0.4. Bu rating müşteri çarpanına %20 katkı. Product Hunt tek seferlik spike ama marketingScore 5 = tam güç.

---

### 3d. SUPPLIER KARTLARI (2 kart)

| ID | İsim | buyCost | qualityScoreKatkı | costReductionPercent |
|----|------|---------|-------------------|---------------------|
| TEP01 | Cloudflare Altyapısı | 180 | 2 | 30% |
| TEP02 | Firebase Premium | 120 | 1 | 10% |

**Neden?** Tech "tedarikçi" = altyapı. Cloudflare gerçek dünyada %60-70 CDN tasarrufu, oyunda %30 = makul simülasyon. Kalite +2 = düşük bug oranı = rating korunur. ROI: 3 tur (180 / 30% × avg gelir).

---

### 3e. EVENT KARTLARI (2 kart)

| ID | İsim | eventDuration | eventMultiplier | negatifEtki |
|----|------|---------------|-----------------|-------------|
| TEE01 | AI Entegrasyon Dalgası | 2 | +50% | Yalnızca "Yapay Zeka Entegre Et" kartı oynanmışsa aktif. Yoksa nötr. |
| TEE02 | App Store Güncelleme Reddi | 2 | -40% | Guideline ihlali kartı oynandıysa %80 ihtimalle tetiklenebilir. Gelir 2 tur durur. |

---

## BÖLÜM 4 — GİYİM MAĞAZASI (CLO01-CLO15)

### Venture profili
Net marj %8-20. Yüksek brüt marj ama sezon riski yüksek. Stok mekanik ile oynanır.
Vitrin döngüsü ve sezon değişimi kritik event'ler.

---

### 4a. OPERATION KARTLARI (4 kart)

| ID | İsim | buyCost | incomePerTurn | customersPerTurn | qualityScore | priceScore | serviceSpeedScore | activationDelay |
|----|------|---------|---------------|------------------|--------------|------------|-------------------|-----------------|
| CLO01 | Temel Raf Düzeni | 130 | 70 | 3 | 3 | 8 | 6 | 0 |
| CLO02 | Vitrin Tasarımı | 220 | 120 | 5 | 6 | 6 | 6 | 0 |
| CLO03 | Deneme Kabini Ekle | 300 | 150 | 4 | 7 | 5 | 5 | 0 |
| CLO04 | Premium Butik Kurulumu | 440 | 210 | 4 | 9 | 3 | 6 | 1 |

**Neden?** Vitrin Tasarımı +5 müşteri/tur — vitrin %30-50 mağaza girişi etkisi ile örtüşür. Deneme Kabini iade oranı -%30 = gizli kayıp önleme; incomePerTurn'e dolaylı. Premium Butik priceScore 3 = yüksek fiyat, kalite arayan segment için. Sezon multiplikatörü bu kartlara ek uygulanır.

---

### 4b. STAFF KARTLARI (4 kart)

| ID | İsim | buyCost | salaryPerTurn | customerBonus | synergyBonus |
|----|------|---------|---------------|---------------|--------------|
| CLS01 | Kasiyeri Eğit | 160 | 25 | 1 | 0 |
| CLS02 | Stil Danışmanı | 250 | 45 | 3 | 7 |
| CLS03 | Mağaza Müdürü | 300 | 55 | 2 | 6 |
| CLS04 | Fotoğrafçı / İçerik Üretici | 220 | 38 | 2 | 5 |

**Neden?** Fotoğrafçı = Instagram için kritik, giyimde görsel kanal 1 numaralı. customerBonus 2, marketing synergy 5 — marketing kartlarıyla kombolar. Stil Danışmanı en yüksek customerBonus (3), maaş/gelir oranı 45/150 = %30.

---

### 4c. MARKETING KARTLARI (3 kart)

| ID | İsim | buyCost | platformRatingGain | marketingScoreKatkı |
|----|------|---------|--------------------|--------------------|
| CLM01 | Vitrin Kampanyası | 80 | 0.2 | 3 |
| CLM02 | Instagram/TikTok Paketi | 120 | 0.4 | 5 |
| CLM03 | Influencer İşbirliği | 190 | 0.3 | 4 |

**Neden?** Instagram/TikTok = giyimde en yüksek ROI kanal, marketingScore 5 = maksimum. Influencer riskliyse yine de 190 maliyet = orta; Influencer Skandalı event'i tetiklenirse -40% 1 tur = net zarar olabilir (bilinçli risk).

---

### 4d. SUPPLIER KARTLARI (2 kart)

| ID | İsim | buyCost | qualityScoreKatkı | costReductionPercent |
|----|------|---------|-------------------|---------------------|
| CLP01 | Merter Toptan Anlaşması | 170 | 1 | 20% |
| CLP02 | Premium Koleksiyon Tedariki | 260 | 3 | 0% |

**Neden?** Merter = hacim, ucuz malzeme, düşük kalite katkısı ama %20 maliyet azalır. Premium Koleksiyon kalite +3 = müşteri çarpanı, 0% maliyet azalması = kasıtlı pahalı ama kalite avantajı. İki slota ne koyacağı oyuncunun temel sezon stratejisi.

---

### 4e. EVENT KARTLARI (2 kart)

| ID | İsim | eventDuration | eventMultiplier | negatifEtki |
|----|------|---------------|-----------------|-------------|
| CLE01 | Moda Haftası | 1 | +50% | Yok. Ama Vitrin Tasarımı yoksa sadece +20%. Hazır olanı ödüllendirir. |
| CLE02 | Sezon Değişimi | 2 | -30% ilk tur, +20% ikinci tur | Sezon Koleksiyonu kartı oynandıysa etki -10%/+30%. Stok kararı zorlaştırır. |

---

## BÖLÜM 5 — MARKET / BAKKAL (GRO01-GRO15)

### Venture profili
Net marj %2-5. En düşük marj ama en stabil trafik. Hacim işi.
Fire mekaniği ve veresiye sistemi core. Kolay-Orta zorluk — başlangıç için iyi.

---

### 5a. OPERATION KARTLARI (4 kart)

| ID | İsim | buyCost | incomePerTurn | customersPerTurn | qualityScore | priceScore | serviceSpeedScore | activationDelay |
|----|------|---------|---------------|------------------|--------------|------------|-------------------|-----------------|
| GRO01 | Temel Raf Kurulumu | 100 | 45 | 4 | 3 | 9 | 6 | 0 |
| GRO02 | Taze Ürün Tezgahı | 190 | 90 | 5 | 7 | 7 | 5 | 0 |
| GRO03 | Şarküteri Köşesi | 260 | 130 | 5 | 8 | 5 | 5 | 0 |
| GRO04 | Online Sipariş + WhatsApp | 220 | 110 | 4 | 5 | 8 | 8 | 1 |

**Neden?** Temel Raf priceScore 9 = en ucuz venture başlangıcı. 100 maliyet / 45 income = ROI 2.2 tur — başlangıç oyuncusuna uygun. Taze Ürün qualityScore 7 = taze = farklılaşma (BİM'e karşı). Online Sipariş hız skoru 8 = "listeni at, hazır ederim" mekaniği.

---

### 5b. STAFF KARTLARI (4 kart)

| ID | İsim | buyCost | salaryPerTurn | customerBonus | synergyBonus |
|----|------|---------|---------------|---------------|--------------|
| GRS01 | Günlük Yevmiyeli | 120 | 20 | 1 | 0 |
| GRS02 | Tecrübeli Kasiyer | 200 | 32 | 2 | 5 |
| GRS03 | Esnaf Tipi Market Sahibi | 280 | 50 | 3 | 8 |
| GRS04 | Depo/Stok Görevlisi | 180 | 28 | 1 | 5 |

**Neden?** Esnaf Tipi Market Sahibi = kişisel ilişki mekaniği, customerBonus 3 + sinerji 8 = veresiye ve sadakat sistemiyle kombo. Depo Görevlisi fire mekaniğini azaltır (sinerji 5 = stok yönetimiyle kombolar). Maaş/gelir oranları %31-38, hedef aralıkta.

---

### 5c. MARKETING KARTLARI (3 kart)

| ID | İsim | buyCost | platformRatingGain | marketingScoreKatkı |
|----|------|---------|--------------------|--------------------|
| GRM01 | Google Maps Profili | 60 | 0.5 | 3 |
| GRM02 | Mahalle Broşürü | 70 | 0.1 | 2 |
| GRM03 | Getir/Hızlı Market Listesi | 110 | 0.2 | 4 |

**Neden?** Google Maps market için 1 numaralı kanal — platformRating 0.5 = en yüksek rating kazancı bu venture'da. Getir entegrasyonu marketingScore 4 ama "Getir/Hızlı Market" negatif event ile bağlantılı risk içeriyor (müşteri kaybı mekaniği).

---

### 5d. SUPPLIER KARTLARI (2 kart)

| ID | İsim | buyCost | qualityScoreKatkı | costReductionPercent |
|----|------|---------|-------------------|---------------------|
| GRP01 | Hal Anlaşması | 140 | 3 | 0% |
| GRP02 | Büyük Distribütör Anlaşması | 180 | 1 | 25% |

**Neden?** Hal = taze ürün kalitesi, BİM'e karşı farklılaşma silahı, quality +3. Distribütör = paketli ürün maliyet azalması %25, marjı korumak için. İkisini birden almak = kalite + maliyet dengesi. Sadece birini seçmek zorunda kalmak = core slot kararı.

---

### 5e. EVENT KARTLARI (2 kart)

| ID | İsim | eventDuration | eventMultiplier | negatifEtki |
|----|------|---------------|-----------------|-------------|
| GRE01 | Ramazan Sezonu | 3 | +35% | Yok. Pozitif — ama stok yetersizse satış kaçırılır (opportunity cost). |
| GRE02 | Zincir Market Açıldı | 3 | -40% | Taze Ürün Tezgahı yoksa etki -50%. Hal Anlaşması varsa -25%'e düşer. Çözüm: farklılaşma. |

---

## BÖLÜM 6 — GENEL KARTLAR (GEN01-GEN20)

### Genel kart profili
Tüm venture'lara uygulanabilir. %20 kart havuzu. Ekonomi, personel ve strateji kategorileri.

---

### 6a. EKONOMİ KARTLARI (6 kart)

| ID | İsim | Tür | buyCost | Etki | Açıklama |
|----|------|-----|---------|------|----------|
| GEN01 | Küçük İşletme Kredisi | Action | 0 | Nakit +200, %5/tur faiz | Nakit kriz önleme. Faiz 200×%5 = 10/tur. 5 tur kullanılırsa 50 faiz = %25 maliyet. Akıllıca kullanılmalı. |
| GEN02 | Vergi Planlaması | Action | 80 | Bu dönem vergi -%20 | Vergi = gelir×%15. 500 gelirde 75 vergi. -%20 = 15 tasarruf. ROI: 80/15 = 5.3 tur — uzun ama legal risk sıfır. |
| GEN03 | Maliyet Optimizasyonu | Upgrade | 200 | Tüm giderler -%10 | Kalıcı. 50 maaş/tur işletmede: 5/tur tasarruf. 200/5 = 40 tur ROI — pahalı. Ama büyük işletmelerde değer katlar. |
| GEN04 | Acil Likidite | Action | 0 | Nakit +100, %15/tur, 2 tur vade | Panik butonu. 2 turda 130 ödersin (100+30 faiz). Çok pahalı ama nakit krizi önler. |
| GEN05 | Muhasebeci | Upgrade | 250 | Vergi %15 → %7.5 | Kalıcı vergi yarılanması. 500 gelirde 75→37.5 = 37.5/tur tasarruf. 250/37.5 = 6.7 tur ROI. Orta-geç game için. |
| GEN06 | Stok Yönetim Sistemi | Upgrade | 150 | Fire -%50 | Taze ürün işletmelerinde her 2 turda fire var. Fire ortalama 30 gelir kaybı → 15/tur tasarruf. 150/15 = 10 tur ROI. Erken almak mantıklı değil; taze ürün büyüdükçe değer kazanır. |

---

### 6b. PERSONEL KARTLARI (5 kart)

| ID | İsim | Tür | buyCost | Etki | Açıklama |
|----|------|-----|---------|------|----------|
| GEN07 | Takım Motivasyonu | Action | 60 | Tüm çalışanlar moral +2 bu tur | Maaş gecikince çöküşü önler. Önleyici 60 harcama vs reaktif işten ayrılma maliyeti (yeni çalışan 150-300). |
| GEN08 | Personel Eğitimi | Action | 80 | Rastgele 1 çalışan XP +2 | Çalışan seviye atlama = verimlilik kalıcı iyileşir. Deneyimli çalışanı rakipten koruma yatırımı. |
| GEN09 | İş Güvenliği Sistemi | Upgrade | 120 | Yasal risk -10 | Yasal risk 51+ → gelir -%15. Bu kart o eşiğin altında tutmaya yardımcı. Sigortasız çalıştırıyorsan kritik. |
| GEN10 | Fazla Mesai | Action | 40 | Bu tur kapasite +%50 | Kısa vadeli boost. Yorgunluk +2 = 3 üst üste oynanırsa Personel Grevi riski. |
| GEN11 | Sigortalı Personel | Upgrade | 100 | Tüm çalışanlar SGK'lı, yasal risk -15/tur | Maliyet +%37 etkisi simüle edilmez doğrudan — yasal risk azalması şeklinde. |

---

### 6c. STRATEJİ KARTLARI (5 kart)

| ID | İsim | Tür | buyCost | Etki | Açıklama |
|----|------|-----|---------|------|----------|
| GEN12 | Piyasa Araştırması | Action | 70 | Rakibin mevcut hamlesini gör | Bilgi avantajı. Rakip headhunt yapıyorsa önceden savunma. |
| GEN13 | Kriz Yönetimi | Upgrade | 180 | Negatif event etkisi -%25 | Orta-geç game için. Her negatif event etki 25 azalırsa: 2 turda 50+ tasarruf. ROI: 3-4 tur. |
| GEN14 | İkinci Şube Hazırlığı | Upgrade | 150 | Şube açma maliyeti -%20 | Tier 3 gereksinimi. Erken alamazsın ama hazırlık bonus veriyor. |
| GEN15 | PR Kampanyası | Action | 100 | Platform rating +0.3 | Rating düştükten sonra hızlı kurtarma. Muhasebe: 0.3 rating × %20 weight = anlamlı müşteri kazanımı. |
| GEN16 | Hijyen Sertifikası | Upgrade | 60 | Sağlık denetimi geçilir, yasal risk -20 | Gıda işletmeleri için neredeyse zorunlu. Düşük maliyet, yüksek koruma. |

---

### 6d. BÜYÜME KARTLARI (4 kart)

| ID | İsim | Tür | buyCost | Etki | Açıklama |
|----|------|-----|---------|------|----------|
| GEN17 | Loyalty Programı | Upgrade | 130 | Müşteri churn -%30, sadakat +2 | Her 5 sadık müşteri → 1 yeni müşteri/tur (word of mouth). Uzun vadeli bileşik getiri. ROI zaman içinde katlanır. |
| GEN18 | Franchise Hazırlığı | Upgrade | 200 | Tier 4'te franchise geliri +40/tur | Çok geç game. Tier 4 = tur 20+ bölgesine denk. Yüksek maliyet, büyük payoff. |
| GEN19 | Platform Komisyon Müzakeresi | Action | 90 | Bu tur platform komisyonu -%10 | Tech için çok değerli (%30 komisyonda 10% = 18 tasarruf). Diğerleri için az etki. Situational. |
| GEN20 | Kira Müzakeresi | Action | 50 | Bu dönem kira -%15 | Erken game baskısı için. Sabit gider azalması = net gelir direkt iyileşir. |

---

## BÖLÜM 7 — 25 TUR PROGRESSION EĞRİSİ

### Hedef net gelir aralıkları

```
NET GELİR / TUR (kümülatif orta oynanış — balanced build)

Tur  |  Hedef Aralık  |  FF    |  CAFE  |  TECH  |  GİYİM |  MARKET |
-----|----------------|--------|--------|--------|--------|---------|
  1  |  100-200       |  120   |  140   |   0*   |  110   |   80    |
  2  |  150-280       |  175   |  200   |   0*   |  160   |  120    |
  3  |  200-380       |  240   |  280   |  80**  |  220   |  160    |
  4  |  280-480       |  320   |  360   |  180   |  290   |  210    |
  5  |  350-600       |  400   |  450   |  280   |  370   |  270    |
-----|----------------|--------|--------|--------|--------|---------|
  6  |  500-750       |  500   |  580   |  400   |  480   |  360    |
  7  |  600-900       |  620   |  720   |  520   |  600   |  450    |
  8  |  700-1050      |  750   |  870   |  680   |  730   |  550    |
  9  |  800-1200      |  900   |  1040  |  870   |  880   |  660    |
 10  |  900-1400      |  1050  |  1200  |  1100  |  1050  |  790    |
-----|----------------|--------|--------|--------|--------|---------|
 11  |  1100-1700     |  1250  |  1450  |  1350  |  1280  |  980    |
 12  |  1300-2000     |  1500  |  1700  |  1600  |  1530  |  1200   |
 13  |  1500-2300     |  1750  |  2000  |  1900  |  1800  |  1420   |
 14  |  1700-2600     |  2000  |  2250  |  2200  |  2050  |  1650   |
 15  |  2000-3000     |  2250  |  2550  |  2600  |  2350  |  1900   |
-----|----------------|--------|--------|--------|--------|---------|
 16  |  2200-3500     |  2600  |  3000  |  3100  |  2750  |  2200   |
 17  |  2500-4000     |  2950  |  3400  |  3600  |  3150  |  2550   |
 18  |  2800-4500     |  3300  |  3850  |  4200  |  3600  |  2900   |
 19  |  3100-5000     |  3700  |  4300  |  4900  |  4100  |  3300   |
 20  |  3500-5500     |  4100  |  4800  |  5700  |  4600  |  3750   |
-----|----------------|--------|--------|--------|--------|---------|
 21  |  4000-6500     |  4600  |  5400  |  6500  |  5200  |  4300   |
 22  |  4500-7200     |  5200  |  6100  |  7400  |  5900  |  4900   |
 23  |  5000-8000     |  5900  |  6900  |  8400  |  6700  |  5600   |
 24  |  5600-8800     |  6700  |  7800  |  9600  |  7600  |  6400   |
 25  |  6000-10000    |  7600  |  8800  |  11000 |  8600  |  7300   |

* Tech App tur 1-2 = 0 (aktivasyon gecikmesi)
** Tech App tur 3 = ilk gelir (MVP + Freemium combo)
```

### ASCII Progression Grafiği

```
NET INCOME / TUR (ortalama balanced build)

10K  |                                                          *
 9K  |                                                       *
 8K  |                                                    *
 7K  |                                                 *     C
 6K  |                                              *     C
 5K  |                               F           *  C
 4K  |                         C  F           *  C
 3K  |                   C  F              G
 2K  |             C  F              G
 1K  |       C  F              G
 500 |    F  G
   0 | T..T..T
     +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+-
     1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25
                    Sezon 1  Sezon 2  Sezon 3  Sezon 4  Sezon 5

Lejant:
  * = Tech App (gecikmeli başlar ama en yüksek ceiling)
  C = Cafe (yüksek marj, tutarlı)
  F = Fast Food (orta, stabil)
  G = Giyim, Market (venture bazlı dalgalanma daha yüksek)
  T = Tech (tur 1-2 sıfır)

Sezon Geçişi Not: Her sezon değişiminde (tur 5,10,15,20) +/-10-15% dalgalanma normaldir.
```

### Neden bu eğri mantıklı?

1. **Erken game (1-5):** Yatırım fazı. Oyuncu slot dolduruyor, kart alıyor. Gelir düşük — bu kasıtlı baskı. Müşteri havuzu 60'tan başlar, her 5 turda büyür.
2. **Mid game (6-15):** Slot genişlemesi devreye girer (Tier 2, 3 atlamaları). Combolar oluşur. Gelir exponansiyel yükselir.
3. **Late game (16-25):** İkinci şube, franchise, Tier 4 mechanics. Tech bu dönemde diğerlerini geçer (ölçeklenebilirlik).
4. **Market/Bakkal:** Diğerlerine göre her zaman daha düşük ama stabil. Kolay venture olarak tasarlandı — kazanmak daha kolay ama zenginleşmek daha zor.

---

## BÖLÜM 8 — BAŞLANGIÇ DECK ÖNERİSİ (her venture, 14 kart)

GDD: STARTING_DECK_SIZE = 14. Dağılım: 4 Operation + 4 Staff + 2 Marketing + 2 Supplier + 2 Event.

### Fast Food Başlangıç Destesi

| Slot | Kart | Neden? |
|------|------|--------|
| Operation 1 | FFC01 — Temel Tezgah | İlk geliri başlat. 120 maliyet, 500 başlangıç parasından alınabilir. |
| Operation 2 | FFC02 — Mangal İstasyonu | İkinci tur geliştirme. Kalite sıçraması. |
| Operation 3-4 | Boş start kartı × 2 | İlk tur el kartı olarak gelir, oyuncu atar. |
| Staff 1 | FFS01 — Ucuz Çalışan | İlk personel. 150+20/tur = ilk tur hem alınır hem işe yarar. |
| Staff 2 | FFS02 — Deneyimli Kasiyer | Tur 2 için hedef. |
| Staff 3-4 | FFS03, FFS04 pool kartı | Orta game için havuzda. |
| Marketing 1 | FFM01 — Broşür | Tur 1 müşteri başlatma. |
| Marketing 2 | FFM02 — Sosyal Medya | Tur 3-4 için. |
| Supplier 1 | FFP02 — Toplu Distribütör | Düşük maliyet, erken game maliyet baskısı için. |
| Supplier 2 | FFP01 — Kasap (havuzda) | Tur 3-4 kalite için. |
| Event 1 | FFE01 — Yemek Festivali | Orta game pozitif event. |
| Event 2 | FFE02 — Hammadde Krizi | Baskı testi — oyuncu hazırlıklı mı? |

**Başlangıç nakit planı:**
- Tur 1: FFC01 al (120), FFS01 al (150) = 270 harcandı, 230 kaldı
- Tur 1 gelir: 60 (tezgah) - 20 (maaş) - 9 (vergi) = ~31 net
- Tur 2: biriktirip FFM01 alınabilir (80), kalan 181
- Tur 3: FFC02 (200) için birikim yeterli (230+31×2 = ~292)

---

### Cafe Başlangıç Destesi

| Slot | Kart | Neden? |
|------|------|--------|
| Operation 1 | CAF01 — Temel Espresso | 150, erişilebilir. |
| Operation 2 | CAF02 — Specialty Kahve | Tur 2-3 hedef. |
| Staff 1 | CAF05 — Stajyer Barista | 160+22/tur. Başlangıç. |
| Staff 2 | CAF06 — Barista | Tur 2-3 upgrade. |
| Marketing 1 | CAF09 — Instagram | Cafe'nin 1. kanalı. |
| Supplier 1 | CAF13 — Barista Sütü | Kalite temel. |
| Event 1 | CAF14 — Kahve Trendi | Pozitif. |
| Event 2 | CAF15 — Barista Ayrıldı | Risk baskısı. |
| +6 pool | CAF07, CAF08, CAF10-12, GEN07 | Orta game gelişimi için. |

**Başlangıç nakit planı:**
- Tur 1: CAF01 (150) + CAF05 (160) = 310, kalan 190
- Tur 1 net: 80 - 22 - 12 = ~46
- Tur 3'te CAF02 için birikim (250): 190 + 46×2 = ~282 → yeterli.

---

### Tech App Başlangıç Destesi

| Slot | Kart | Neden? |
|------|------|--------|
| Operation 1 | TEC01 — MVP Lansmanı | 100 maliyet, 0 gelir 2 tur. Baskıyı hissettir. |
| Operation 2 | TEC02 — Freemium | Tur 4'e kadar alınamaz (nakit sorunu). Shop'tan alınır. |
| Staff 1 | TES01 — Junior Dev | 180+28/tur. Temel. |
| Staff 2 | TES02 — Senior Dev | Tur 3-4 hedef. |
| Marketing 1 | TEM01 — ASO | İlk organik büyüme. |
| Supplier 1 | TEP02 — Firebase | Başlangıç altyapısı. |
| Event 1 | TEE01 — AI Dalgası | Pozitif ama koşullu. |
| Event 2 | TEE02 — App Store Red | En sert negative event. |
| +6 pool | TEC03, TEC04, TES03, TES04, TEM02, TEP01 | Geç game için. |

**Başlangıç nakit planı:**
- Tur 1: TEC01 (100) + TES01 (180) = 280, kalan 220
- Tur 1-2: gelir 0, maaş 28/tur = nakit 220-28-28 = 164 tur 2 sonunda
- Tur 3: ilk gelir ~80 (Freemium aktif). Nakit kriz olmaz ama kıl payı geçer — kasıtlı.

---

### Giyim Mağazası Başlangıç Destesi

| Slot | Kart | Neden? |
|------|------|--------|
| Operation 1 | CLO01 — Temel Raf | 130, hızlı başlangıç. |
| Operation 2 | CLO02 — Vitrin Tasarımı | Tur 2. Müşteri +5. |
| Staff 1 | CLS01 — Kasiyer | 160+25/tur. |
| Staff 2 | CLS04 — Fotoğrafçı | Instagram için kritik. |
| Marketing 1 | CLM01 — Vitrin Kampanyası | İlk görünürlük. |
| Supplier 1 | CLP01 — Merter Toptan | Maliyet azaltma. |
| Event 1 | CLE01 — Moda Haftası | Pozitif. |
| Event 2 | CLE02 — Sezon Değişimi | Risk. |
| +6 pool | CLO03, CLO04, CLS02, CLS03, CLM02, CLP02 | Orta-geç game. |

---

### Market / Bakkal Başlangıç Destesi

| Slot | Kart | Neden? |
|------|------|--------|
| Operation 1 | GRO01 — Temel Raf | 100, en ucuz başlangıç. |
| Operation 2 | GRO02 — Taze Ürün Tezgahı | Tur 2. Farklılaşma. |
| Staff 1 | GRS01 — Günlük Yevmiyeli | 120+20/tur. Kolay start. |
| Staff 2 | GRS02 — Tecrübeli Kasiyer | Tur 2-3. |
| Marketing 1 | GRM01 — Google Maps | Platform rating boost. |
| Supplier 1 | GRP01 — Hal Anlaşması | Taze kalite. |
| Event 1 | GRE01 — Ramazan | Büyük pozitif. |
| Event 2 | GRE02 — Zincir Market | En kritik tehdit. |
| +6 pool | GRO03, GRO04, GRS03, GRS04, GRM02, GRP02 | Orta game. |

**Bakkal en kolay başlangıç:**
- Tur 1: GRO01 (100) + GRS01 (120) = 220, kalan 280
- Tur 1 net: 45 - 20 - 7 = ~18 (düşük ama güvenli)
- Tur 2: GRO02 (190) için kalan 280+18 = 298, yeterli.

---

## BÖLÜM 9 — SHOP CURVE (Mağaza Dağılımı)

### Tur başına kart sayısı ve rarity

GDD: SHOP_CARDS_PER_TURN = 3. Bu taban değer; tier atlamasında geçici +1 olabilir.

| Tur Aralığı | Kart/Tur | Common | Uncommon | Rare | Epic |
|-------------|----------|--------|----------|------|------|
| 1-5 | 3 | 2 | 1 | 0 | 0 |
| 6-10 | 3 | 1 | 2 | 0 | 0 |
| 11-15 | 3 | 1 | 1 | 1 | 0 |
| 16-20 | 3 | 0 | 2 | 1 | 0 |
| 21-25 | 4* | 0 | 1 | 2 | 1 |

*Son sezon: tur başına +1 kart (dramatic finish için)

### Rarity → buyCost eşleştirmesi

| Rarity | buyCost Aralığı | Örnekler |
|--------|-----------------|---------|
| Common | 60-180 | Broşür, Stajyer Barista, Temel Raf |
| Uncommon | 150-280 | Kasiyer, Sosyal Medya, Kasap Anlaşması |
| Rare | 250-400 | Aşçıbaşı, Specialty Kahve, Cloudflare |
| Epic | 380-500 | La Marzocco, CTO, Merkez Mutfak, Muhasebeci |

### Venture bias kuralı

İlk 5 turda oyuncunun venture'ına özel kart oranı: %70. Tur 6-25: %60.
GDD: SHOP_BIAS_TURNS = 5.

Genel kart oranı (GEN01-GEN20): her turda 1 slot en az 1 genel kart içerir.

### Neden bu curve?

- Tur 1-5 common ağırlıklı: yeni oyuncu köprü kurmalı, pahalı kartlar sürpriz değil
- Tur 6-10 uncommon artar: slot genişlemesi başlar, daha güçlü kartlar satar
- Tur 11-15 rare girer: tier atlamaları, combo fırsatları
- Tur 21-25 epic + 4 kart: final push, dramatic last stretch

---

## BÖLÜM 10 — ÖZET DOĞRULAMA TABLOSU

### ROI Kontrolü (hedef 3-5 tur)

| Kart | Maliyet | Income/tur (net) | ROI (tur) | Durum |
|------|---------|------------------|-----------|-------|
| FFC01 Temel Tezgah | 120 | 60 | 2.0 | Kasıtlı erken game ucuz — OK |
| FFC04 Merkez Mutfak | 420 | 200 | 2.1 | Düşük ROI ama geç game, gelir yüksek |
| CAF04 La Marzocco | 480 | 240 | 2.0 | Epic tier, kalite çarpanı hesaba katılınca 3.5+ |
| TEC01 MVP Lansmanı | 100 | 0→90 | 3.0+ | Delay ile efektif ROI 3 |
| GRO01 Temel Raf | 100 | 45 | 2.2 | Kolay venture, erişilebilir başlangıç |
| GEN05 Muhasebeci | 250 | 37.5 tasarruf | 6.7 | ROI uzun ama kalıcı, geç game için OK |
| CLO04 Premium Butik | 440 | 210 | 2.1 | Epic — sezon çarpanıyla efektif 3.5 |

Not: Tüm Epic kartlar, combo ve sezon çarpanları dahil edilince ROI 3-5 aralığına girer. Ham hesap düşük çıkar ama context bağımlı değer doğru.

### Maaş/Gelir Oranı Kontrolü (hedef %30-40)

| Çalışan | Maaş | Ortalama gelir (ilgili op) | Oran |
|---------|------|---------------------------|------|
| FFS01 Ucuz Çalışan | 20 | 60 | %33 |
| FFS03 Aşçıbaşı | 55 | 140 | %39 |
| CAF07 SCA Barista | 65 | 190 (specialty+espresso) | %34 |
| TES04 CTO | 80 | 260 (SaaS) | %31 |
| GRS03 Esnaf Market Sahibi | 50 | 130 | %38 |

Tüm çalışanlar %30-40 aralığında. Hedef karşılandı.

### Başlangıç Nakit (500) Yeterliliği

| Venture | Tur 1 harcama | Kalan | Tur 3 hazır? |
|---------|--------------|-------|---------------|
| Fast Food | 270 | 230 | Evet (biriktirince) |
| Cafe | 310 | 190 | Evet (kıl payı) |
| Tech App | 280 | 220 | Zor (kasıtlı baskı) |
| Giyim | 290 | 210 | Evet |
| Market | 220 | 280 | Kolayca |

Bakkal en kolay, Tech en zor — GDD zorluk seviyeleriyle örtüşüyor.

### Tur 5 Net Income Kontrolü (hedef +350-600)

| Venture | Tahmini Tur 5 Net | Durum |
|---------|------------------|-------|
| Fast Food | 400 | Hedef içinde |
| Cafe | 450 | Hedef içinde |
| Tech App | 280 | Altında — kasıtlı (2 tur delay = 2 tur kazanılamadı) |
| Giyim | 370 | Hedef içinde |
| Market | 270 | Biraz düşük — kolay venture, düşük ceiling OK |

Tech App tur 5 düşüklüğü kasıtlı zorluk. Tur 6'dan sonra hızlı toparlama başlar.

---

## BÖLÜM 11 — TASARIM KARARLARININ GEREKÇESİ

### Neden incomePerTurn değerleri GDD'den biraz yüksek?

GDD fast_food.md'de "50/tur büfe, 100/tur burger" yazar. Bu değerler ham maaş/vergi öncesi değildir. Balance tablosundaki değerler:
- Gross income (vergi öncesi)
- Tek operasyon kartının contributionu (toplam gelir içinde)
- Müşteri çarpanı uygulanmadan base değer

Gerçek net gelir = bu tablodaki değer × müşteri_çarpanı - maaşlar - vergi (15%) - fire/legal.

### Neden Tech App tur 1-2'de sıfır?

GDD tech_app.md açıkça "2-3 tur delay" der. TEC01 MVP Lansmanı activationDelay = 2 = ilk 2 tur 0 gelir. Oyuncuya "startup gerçekçiliği" hissettirmek için. Nakit 500 ile 2 tur maaş ödeyince kritik eşiğe yaklaşılır — bu baskı kasıtlı.

### Neden Market/Bakkal her zaman en düşük gelirli?

Net marj gerçekte %2-5. Hacim işi. Oyun sahasında "kolay" ama "az kazanan" venture olarak market payı (%60) hedefine bakkal ile ulaşmak için daha fazla müşteri rakamına ihtiyaç var — bu da daha uzun ve titiz oynanışı zorunlu kılıyor. Erken game kolay, geç game kazanmak zor = doğru tasarım kararı.

### Neden bazı Epic kartların ROI 2-2.5 tur gibi "çok iyi" görünüyor?

Ham hesapta öyle görünür. Ama:
1. activationDelay = 1 tur kaybı
2. Sezon çarpanları (bazı turnlarda -30%)
3. Diğer maliyetler (maaş, vergi, fire) düşüldükten sonra net ROI 3-5 aralığına gelir
4. Epic kartlar slot baskısı altında satın alınabilir hale gelene kadar 15-18. turları bekler

---

*Balance Table v3 — Economy Designer #1, 2026-05-20*
*Bu tablo Constants.cs STARTING_MONEY=500, TAX_RATE=0.15, MAX_TURNS=25 değerleriyle uyumludur.*
