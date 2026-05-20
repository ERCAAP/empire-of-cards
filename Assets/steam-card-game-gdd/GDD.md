# GAME DESIGN DOCUMENT
# "Empire of Cards"

> Versiyon: 3.0 | Tarih: 2026-05-20
> Engine: Unity 6 (C#) | Platform: PC (Steam)
> Ekip: Solo Developer | Fiyat: $9.99–$12.99

---

# BÖLÜM 1: OYUN NEDİR?

## 1.1 Tek Cümle

Küçük bir işletmeyle başlar, aynı semtte aynı sektörde rakibinle kapışır, marketi kim domine ederse kazanır.

## 1.2 Oyuncunun Yaptığı Şey

Masanın başında oturuyorsun. Slotlar var. Elinde kartlar var. Kararlar veriyorsun.

Her tur:
1. Kartlar gelir — girişim türüne özel
2. Slotlara yerleştirirsin
3. Sistem çalışır — para gelir, müşteri gelir, kriz patlar
4. Ekonomi hesaplanır
5. Rakip oynar

**Amaç:** Semtteki müşteri havuzunun %60'ını ele geçir. 25 tur dolmadan.

## 1.3 Neden Eğlenceli?

| An | His |
|---|---|
| Slot doldurdun, müşteri aktı | "Büyüyorum!" |
| Zincir reaksiyon patlattın | "Oha, bunu hesaplamamıştım!" |
| Rakip çalışanını çaldı | "Seni gidi..." |
| Maaş geciktirince işletme çöktü | "Kendi kendimi batırdım." |
| 25. turda %60'ı aldın | "EVEEET!" |

## 1.4 Tasarım Felsefesi

Bu oyun şunu sormaktan kaçınmaz:

> "Yanlış zamanda doğru kart bile işletmeyi batırabilir mi?"

**Evet.** Fazla masa, müşteri yokken açılırsa zarar. Influencer kampanyası, mutfak kapasitesi yokken yapılırsa kötü yorum. Maaş geciktirilirse iyi çalışan gider.

Her karar başka sistemi etkiler. Oyuncu sadece "güçlü kart" seçmez; **işletmenin durumunu okur ve ona göre karar verir.**

---

# BÖLÜM 2: GİRİŞİM SEÇİMİ

## 2.1 Konsept

Oyun başladığında oyuncuya "İlk Girişimini Seç" ekranı gösterilir.

**Kritik Kural:** Rakip de oyuncuyla **aynı tür işletmeyle** başlar. Fast food seçtiysen rakip de fast food açar. Aynı semtte, aynı sektörde, aynı müşteri havuzu için savaşırsınız.

Bu kural şunu sağlar:
- Rakip aynı avantaj/dezavantajlarla başlar
- Kimin daha iyi yönettiği kazanır
- Sektör dinamikleri her iki taraf için de geçerlidir

## 2.2 Girişim Seçenekleri

| Girişim | Başlangıç Durumu | Zorluk |
|---|---|---|
| Fast Food Zinciri | 3 masa, 2 çalışan, düşük Google puanı | Orta |
| Cafe | Temel espresso makinesi, 1 barista | Orta |
| Tech Mobil Uygulama | Sıfır kullanıcı, 2 tur delay gelir | Zor |
| Giyim Mağazası | Küçük stok, düşük görünürlük | Orta-Zor |
| Market / Bakkal | Temel raf, 1 personel, dar marj | Kolay-Orta |

> Her girişim türünün kart havuzu, event listesi ve özel mekanikleri o girişime ait ayrı MD dosyasında tanımlanmıştır.
> `Assets/steam-card-game-gdd/businesses/` klasörüne bak.

## 2.3 Başlangıç Kaynakları

Her girişimde oyuncu:
- Küçük işletme (başlangıç seviyesi)
- Sınırlı nakit
- Az müşteri
- Düşük kalite / düşük puan
- Boş slotlar

ile başlar.

## 2.4 Tutorial

İlk run'da tutorial açılır. Girişim otomatik seçilir (Fast Food). Tutorial tamamlandıktan sonra girişim seçim ekranı açılır.

---

# BÖLÜM 3: ŞİRKET SEVİYESİ (Company Tier)

## 3.1 Konsept

Oyuncunun işletme durumuna göre şirket seviyesi otomatik belirlenir. Yeni kart tipi gerektirmez — sadece board state kontrolü.

## 3.2 Tier Tablosu

| Tier | İsim | Koşul | Görsel Feedback |
|------|------|-------|-----------------|
| 1 | ESNAF | Oyun başı (varsayılan) | Küçük logo, sade board |
| 2 | GİRİŞİMCİ | İyi operasyon + 1+ combo | Logo büyür, popup |
| 3 | ŞİRKET | Güçlü market pozisyonu | Board rengi değişir, müzik yoğunlaşır |
| 4 | HOLDİNG | Dominasyon yakın | Taç efekti, altın border |

## 3.3 Tier Kuralları

- **Tier asla düşmez.** Bir kere Girişimci oldun mu Esnaf'a dönemezsin.
- **Tier atlamada:** Popup + ses + kısa animasyon. Oyun 1–2 saniye durur.
- **Slot genişlemesi:** Her tier atlamada yeni slotlar açılır (bkz. Bölüm 4.5).
- **Skor bonusu:** Run sonu tier bonus verir.

---

# BÖLÜM 4: OYUN TAHTASI VE SLOT SİSTEMİ

## 4.1 Masa Yapısı

Masa 3 ana bölgeye ayrılır:

```
┌─────────────────────────────────────────┐
│         RAKİP İŞLETME ALANI             │
│   [Operation] [Staff] [Marketing]       │
├─────────────────────────────────────────┤
│           SEMT / MARKET ALANI           │
│  Müşteri havuzu | Trafik | Trendler     │
│  Reklam görünürlüğü | Aktif eventler   │
├─────────────────────────────────────────┤
│         OYUNCU İŞLETME ALANI            │
│   [Operation] [Staff] [Marketing]       │
│   [Supplier]  [Temp Effects]            │
└─────────────────────────────────────────┘
```

**Oyuncu İşletme Alanı:** Alt bölge. Slotlar burada.
**Semt / Market Alanı:** Orta bölge. Müşteri hareketi, trafik, trendler, aktif eventler burada görünür.
**Rakip İşletme Alanı:** Üst bölge. Rakibin slotları sınırlı bilgiyle görünür (dedektif hissi).

## 4.2 Kart Yerleştirme Felsefesi

Kartlar oynanıp çöpe **atılmaz.** Slotlara yerleştirilir ve işletmenin kalıcı parçası olur.

Bu sistem şunu hissettirmelidir:
> "Masada gerçek bir işletme kuruyorum. Her kart bir karar, her slot bir yatırım."

Kartlar görsel olarak işletme dünyasına bağlanır:
- **Tedarikçi kartları:** Mutfak/depo alanına yerleşir
- **Marketing kartları:** Semt alanını etkiler (görsel ok animasyonu)
- **Operation kartları:** Fiziksel mekânı büyütür
- **Temp kartları:** Aktif kriz/event olarak görünür — çözülünce kalkar

## 4.3 Slot Türleri (Başlangıç)

### OPERATION SLOTS — 4 adet
İşletmenin fiziksel altyapısı ve kapasitesi.

Buraya yerleşen kartlar:
- Masa, Mutfak Upgrade, Paket Servis İstasyonu, Kahve Makinesi, Fırın, Self-Service Tezgah, Depo

**Ne kontrol eder:** Müşteri kapasitesi, servis hızı, üretim gücü.

**Kritik denge:**
- Az operation → müşteri gelemez, kapasite yetersiz
- Fazla operation → müşteri yoksa boş kapasite = gereksiz maliyet + kira artışı

### STAFF SLOTS — 5 adet
Aktif çalışanlar.

Buraya yerleşen kartlar:
- Aşçı, Kasiyer, Temizlikçi, Kurye, Müdür, Barista, Stajyer, Güvenlik

**Ne kontrol eder:** Verimlilik, servis hızı, moral, operasyon stabilitesi.

**Kritik denge:**
- Az personel → müşteri kuyruğu → kötü yorum → puan düşer
- Fazla personel → gereksiz maaş yükü → kâr erir
- Düşük moral → hata artar → kalite düşer → müşteri kaçar

### MARKETING SLOTS — 3 adet
Aktif reklam kampanyaları.

Buraya yerleşen kartlar:
- Broşür Kampanyası, Influencer Anlaşması, Google Reklamı, TikTok Kampanyası, Billboard, Sosyal Medya Yönetimi

**Ne kontrol eder:** Müşteri edinimi, görünürlük, marka bilinirliği.

**Kritik uyarı:**
> Operasyon hazır değilken fazla marketing yapılırsa müşteri patlar → servis çöker → kötü yorum → puan düşer → uzun vadede zarar.

### SUPPLIER SLOTS — 2 adet
Aktif tedarikçi anlaşmaları.

Buraya yerleşen kartlar:
- Premium Kasap, Ucuz Hammadde, Organik Tedarikçi, İçecek Markası Anlaşması, Taze Sebzeci

**Ne kontrol eder:** Kalite skoru, maliyet, müşteri memnuniyeti, fire riski.

**Kritik denge:**
- 2 slot kısıtlaması → oyuncu kalite mi fiyat mı seçmeli?
- Premium + Premium = kalite max ama maliyet ağır
- Ucuz + Ucuz = maliyet düşük ama kalite çöker

### TEMP EFFECT SLOTS — 3 adet
Geçici durumlar, aktif krizler, anlık fırsatlar.

Buraya gelen kartlar/eventler:
- Sağlık Denetimi, Viral Trend, Personel Grevi, İndirim Haftası, Gıda Zehirlenmesi, Google Cezası, Rakip Saldırısı

**Ne kontrol eder:** Anlık baskı ve kaos. Event çözülünce slot boşalır.

**Kural:** Temp slotlar doluysa yeni event geldiğinde en eski event düşer (veya en ağırı kalır — tasarım kararı).

## 4.4 Başlangıç Slot Özeti

| Slot Türü | Başlangıç | Maksimum |
|---|---|---|
| Operation | 4 | 8 |
| Staff | 5 | 10 |
| Marketing | 3 | 5 |
| Supplier | 2 | 4 |
| Temp Effect | 3 | 3 (sabit) |
| **TOPLAM** | **17** | **30** |

## 4.5 Slot Genişleme Sistemi

İşletme büyüdükçe yeni slotlar açılır:

| Tier | Açılan Slotlar |
|---|---|
| Tier 1 → Tier 2 (Girişimci) | Marketing +1 |
| Tier 2 → Tier 3 (Şirket) | Staff +2, Operation +2 |
| Tier 3 → Tier 4 (Holding) | Supplier +1, Marketing +1 |
| İkinci Şube Açıldığında | Operation +2 (yeni şube) |

## 4.6 Slot Baskısı ve Strateji

**Slot kısıtlaması oyunun stratejik derinliğini buradan alır.**

Oyuncu her tur şu soruları sorar:
- "Marketing slotum dolu — Influencer mı çıkarsam, Google Reklamı mı bıraksam?"
- "Staff slotuma başka çalışan almak için kimi çıkarmam lazım?"
- "Temp slotum dolu, yeni kriz geliyor — hangisini çözmeliyim önce?"

## 4.7 Build Arketipleri

Slot kısıtlamaları farklı oyun stilleri doğurur:

### 1. Agresif Marketing Build
Marketing 3/3 dolu. Hızlı müşteri artışı. Operation slotları yetersiz kalırsa servis çöker.
- **Risk:** Operasyon krizi → kötü yorum spirali
- **Ödül:** Hızlı market share kazanımı

### 2. Premium Kalite Build
Güçlü tedarikçiler + iyi personel. Az reklam. Müşteri sadakati yüksek.
- **Risk:** Yavaş büyüme, rakip önce dominasyon alabilir
- **Ödül:** Stabil gelir, düşük kriz riski

### 3. Ucuz Genişleme Build
Düşük maaş, ucuz hammadde, hızlı operation büyümesi.
- **Risk:** Kalite düşüklüğünden puan çöküşü, personel istifaları
- **Ödül:** Erken kapasite avantajı

### 4. Dengeli Stabil Build
Tüm slotlarda orta seviye yatırım. Düşük risk, yavaş dominasyon.
- **Risk:** Agresif rakibe yavaş kalır
- **Ödül:** Krizlere dayanıklılık

---

# BÖLÜM 5: EKONOMİ SİSTEMİ

## 5.1 Temel Formül

```
Net Kâr = Gelir - Giderler

Gelir = müşteri_sayısı × ortalama_harcama × kalite_katsayısı
Giderler = kira + maaşlar + hammadde + reklam + bakım + vergi + kredi_faizi
```

## 5.2 Nakit Akışı

**Nakit akışı kârdan önce gelir.** Kârlı ama nakitsiz işletme iflas eder.

Örnek senaryo:
- Aylık gelir: 100K TL
- Aylık gider: 90K TL (kârlı görünüyor)
- Ama: Yemeksepeti ödemesi 30 gün sonra, maaş bu hafta → nakit yok → iflas

**Oyun mekaniği:** Her tur sonunda nakit bakiye kontrol edilir. Negatife düşerse "Nakit Kriz" event'i tetiklenir. 3 tur üst üste negatif = iflas.

## 5.3 Gelir Kaynakları

| Kaynak | Gecikme | Not |
|---|---|---|
| Doğrudan satış (fiziksel) | Aynı tur | En hızlı |
| Delivery platform (Yemeksepeti vb.) | 1–2 tur sonra | Komisyon %25–35 |
| Online satış (Trendyol vb.) | 1 tur sonra | Komisyon %15–25 |
| App Store iOS | 3 tur sonra | Apple %30 keser, 45 gün |
| Google Play Android | 2 tur sonra | Google %15–30, ayın 15'i |
| Abonelik/SaaS | Her tur sabit | Churn riski var |
| Kurumsal sözleşme | Sabit/tur | Uzun vade |

## 5.4 Gider Kalemleri

### Sabit Giderler (her tur düşülür)
- Kira: işletme büyüdükçe artar
- Temel personel maaşları
- Sigorta (ödenmezse yasal risk artar)
- Lisans/ruhsat yenileme (her X turda)

### Değişken Giderler
- Hammadde/tedarik: ciro ile orantılı
- Reklam: aktif marketing kartlarına göre
- Bakım ve onarım: operation slotlarının yaşına göre
- Platform komisyonları: online satıştan otomatik kesilir

### Gizli Giderler (oyuncu başta görmez)
- Fire (food waste): taze ürün işletmelerinde
- Personel devir maliyeti
- Stok değer kaybı (giyimde sezon geçişi)
- Kredi faizi
- Ceza ödemeleri (yasal risk olayları)

## 5.5 Maaş Sistemi

Her tur personel maaşları ödenir. Oyuncu seçenekleri:

| Seçim | Kısa Vade | Uzun Vade |
|---|---|---|
| Zamanında öde | Nakit eksilir | Moral yüksek, verimli |
| Geciktir | Nakit korunur | Moral -2, istifa riski artar |
| Eksik öde | Nakit kısmen korunur | Moral -1, sadakat düşer |
| Avans ver | Nakit eksilir fazla | Moral +2, sadakat +1 |

**Maaş geciktirme zinciri:**
Maaş gecikir → moral düşer → hata artar → kalite düşer → müşteri memnuniyeti azalır → kötü yorum → puan düşer → müşteri kaçar → gelir düşer → maaş daha da gecikir.

## 5.6 Personel Sigorta Sistemi

| Model | Maliyet | Yasal Risk |
|---|---|---|
| Sigortalı + SGK | +%37 maaş üzerine | Sıfır |
| Sigortasız | Maliyet yok | Yüksek (30K–60K TL/kişi ceza) |
| Günlük Yevmiye | Esnek | Orta (SGK yükümlülüğü tartışmalı) |

## 5.7 Kredi / Borç Sistemi

Oyuncu nakit sıkıntısında kredi çekebilir:

| Kredi Türü | Miktar | Faiz/tur | Koşul |
|---|---|---|---|
| Küçük İşletme Kredisi | 200 | %5 | Tier 1'den itibaren |
| Orta Kredi | 500 | %8 | Tier 2 ve üstü |
| Büyük Yatırım Kredisi | 1000 | %12 | Tier 3 ve üstü |
| Acil Likidite | 100 | %15 | Her zaman, 2 tur vade |

**Kredi notu:** Çok borçlanırsan bir sonraki kredi daha pahalı. Ödemeleri geciktirirsen faiz artar.

**Strateji notu:** Kredi = yatırım fırsatı veya hayatta kalma aracı. Yanlış kullanılırsa faiz işletmeyi eritir.

## 5.8 Vergi Sistemi

Her 5 turda vergi dönemi gelir:

- Vergi = net kâr × %20 (basitleştirilmiş KV oranı)
- Hazırlıklıysan ödersin, geçersin
- Nakit yoksa vergi borcu oluşur → faiz işler → 2 tur ödenmezse denetim

**Risk kartı:** "Vergiden Kaç" kartı oynandıysa vergi yükümlülüğü düşer ama denetim riski artar.

## 5.9 Enflasyon / Piyasa Baskısı

Her 3–4 turda enflasyon eventi gelir:
- Hammadde maliyetleri +%10–25
- Kira yenileme (ev sahibi zam yapar — kabul et veya taşın)
- Personel maaş beklentisi artar

**Zincir:** Enflasyon → maliyet artar → fiyat artıramazsan marj düşer → kâr erir.

---

# BÖLÜM 6: PERSONEL SİSTEMİ

## 6.1 Çalışan Değerleri

Her çalışanın 4 değeri var:

| Değer | Etki | Ne Değiştirir |
|---|---|---|
| Moral | Düşerse hata oranı artar | Maaş, muamele, iş yükü |
| Yorgunluk | Artarsa performans düşer | Fazla mesai, çalışma saati |
| Sadakat | Düşerse rakip çekebilir | Moral, maaş, takdir |
| Deneyim (XP) | Artarsa verimlilik artar | Zaman + iş yükü |

## 6.2 Moral Zinciri

```
Maaş gecikir / iş yükü fazla / muamele kötü
↓
Moral düşer
↓
Hata oranı artar
↓
Servis yavaşlar / kalite düşer
↓
Müşteri memnuniyeti azalır
↓
Kötü yorum / puan düşer
↓
Müşteri kaçar
```

## 6.3 Fazla Mesai

"Fazla Mesai" kartı oynandığında:
- Bu tur kapasite +%50
- Yorgunluk +2
- 3 tur üst üste fazla mesai → "Personel Grevi" riski

## 6.4 Çalışan Gelişimi (XP)

Her tur çalışan deneyim puanı biriktirir:
- Seviye 1 → 2: Hata oranı -%20
- Seviye 2 → 3: Verimlilik +%15, müşteri memnuniyeti +1
- Seviye 3 → 4: Özel yetenek açılır (şef → "Lezzet Ustası", barista → "Latte Art")

Deneyimli çalışan = değerli. Rakip bu çalışanı çalmaya çalışır.

## 6.5 Rakip Çalışan Çalma (Headhunting)

Rakip, oyuncunun düşük sadakatli çalışanlarını hedef alır:
- Teklif: mevcut maaşın 1.5–2x
- Oyuncu 1 tur içinde karşı teklif yapabilir
- Yapmazsa çalışan gider (slot boşalır)
- Deneyimli çalışan giderse: kalite aniden düşer, eski seviye için yenisini eğitmek gerekir

---

# BÖLÜM 7: MÜŞTERİ SİSTEMİ

## 7.1 Market Havuzu

Semtte **sınırlı müşteri** vardır. Oyuncu ve rakip aynı havuz için savaşır.

```
Toplam Semt Müşteri Havuzu = 100 (sabit başlangıç)
Oyuncu payı + Rakip payı = 100
```

Müşteri dağılımı her tur şu faktörlere göre hesaplanır:

| Faktör | Ağırlık |
|---|---|
| Kalite skoru | %30 |
| Fiyat rekabeti | %20 |
| Platform puanı (Google/App Store) | %20 |
| Reklam görünürlüğü | %15 |
| Servis hızı | %10 |
| Müşteri sadakati | %5 |

## 7.2 Müşteri Segmentleri

| Segment | Özelliği | Nasıl Çekilir |
|---|---|---|
| Fiyat Hassas | Ucuzu seçer, sadakatsiz | Fiyat düşür, indirim yap |
| Kalite Arayan | Pahalıyı öder, sadık kalır | Kalite skoru yüksek tut |
| Trend Takipçisi | Viral olursa gelir, geçince gider | Marketing + trend eventleri |
| Sadık Müşteri | Alışkanlıkla gelir | Sürekli iyi deneyim |
| Yeni Müşteri | Reklam veya tavsiyeyle gelir | Marketing + word of mouth |

## 7.3 Müşteri Sadakati

Her pozitif deneyim sadakati artırır. Sadık müşteri:
- Her tur %70 ihtimalle geri gelir (reklam olmadan)
- Word of mouth zinciri: her 5 sadık müşteri 1 yeni müşteri getirir
- Fiyata daha az hassas (kalite düşse bile hemen gitmez)

**Sadakat nasıl kırılır:**
- Kalite ani düşüş
- Uzun servis bekleme
- Rakibin güçlü influencer kampanyası
- Fiyat farkı çok büyüyünce

## 7.4 Word of Mouth (Ağızdan Ağıza)

Sessiz ama en güçlü müşteri kazanım mekanizması:
- 5 sadık müşteri → her tur 1 yeni müşteri organik
- Google puanı 4.5+ → organik yeni müşteri
- Viral event → büyük spike ama kısa süreli

---

# BÖLÜM 8: PUAN SİSTEMİ (Google / App Store Rating)

## 8.1 Genel Kural

Her işletme türünün bir **Platform Puanı** var (1.0–5.0):
- Fast Food / Cafe / Market / Giyim → Google Business Rating
- Tech App → App Store Rating (iOS veya Android)

## 8.2 Puanın Müşteriye Etkisi

| Puan | Müşteri Etkisi |
|---|---|
| 4.5–5.0 | Organik müşteri gelir (+%20) |
| 4.0–4.4 | Normal, nötr |
| 3.5–3.9 | Yeni müşteri -%30 |
| 3.0–3.4 | Yeni müşteri -%60 |
| 3.0 altı | Pratik ölüm — organik müşteri gelmez |

## 8.3 Puan Değişimi

| Olay | Puan Değişimi |
|---|---|
| Müşteriden yorum ist (organik) | +0.1–0.3 |
| İyi servis turu (kalite yüksek) | +0.1 |
| Kötü servis / kalite düşüklüğü | -0.2 |
| Kriz eventi (zehirlenme, skandal) | -0.5–1.0 |
| Sahte yorum satın al (başarılı) | +0.3 (geçici) |
| Sahte yorum tespit (Google/Apple) | -0.5 + ban riski |
| Influencer kampanyası | +0.2 |

## 8.4 Puan Kurtarma

Düşük puan kurtarılabilir ama zaman alır:
- Her "iyi tur" (kalite yüksek + hızlı servis) +0.1
- "PR Kampanyası" kartı: +0.3 anlık
- "Müşteri Memnuniyeti Programı" upgrade: +0.1/tur
- Minimum kurtarma süresi: 3–5 tur (kritik düşüşten)

---

# BÖLÜM 9: STOK YÖNETİMİ

## 9.1 Hangi İşletmeler İçin Geçerli?

- Fast Food, Cafe, Market/Bakkal → **taze ürün fire riski**
- Giyim Mağazası → **sezon stok batığı riski**
- Tech App → **stok yönetimi yok** (dijital ürün)

## 9.2 Fire Mekaniği

Taze ürün işletmelerinde:
- Her 2–3 turda stokun %10–20'si bozulur (gelir kaybı)
- "Stok Yönetim Sistemi" upgrade: fire -%50
- Buzdolabı arızası eventi: tüm taze stok yok

## 9.3 Sezon Stok Dengesi

Giyim mağazasında:
- Sezon başı: büyük stok alımı (nakit bağlanır)
- Sezon sonu: satılmayan stok → indirim (marj düşer) veya bekle (nakit bağlı kalır)
- Yanlış trend: stok tamamen batabilir

**Stok kararı mekaniği:** Her sezon geçişinde oyuncuya seçenek sunulur:
1. İndirim yap: gelir -%30 bu tur, stok temizlenir
2. Bekle: müşteri -%20, nakit bağlı
3. Depola: küçük maliyet, gelecek sezon dene

## 9.4 Tedarik Güvenilirliği

Tedarikçi slotuna yerleştirilen kart türüne göre:
- Premium tedarikçi: güvenilir, pahalı, her tur tutarlı kalite
- Ucuz tedarikçi: tutarsız, bazen "bu hafta malzeme kötüydü" eventi tetikler
- Veresiye tedarik: nakit tasarrufu ama tedarikçi anlaşmazlığı riski

---

# BÖLÜM 10: LOKASYON VE TRAFİK

## 10.1 Konum Seçimi (Oyun Başı)

Oyuncu başlangıçta konum seçer:

| Konum | Kira | Başlangıç Trafiği |
|---|---|---|
| Ücra köşe | Düşük | Çok düşük |
| Yan sokak | Orta | Orta |
| Ana cadde | Yüksek | Yüksek |
| AVM / alışveriş merkezi | Çok yüksek + ciro payı | Çok yüksek |

## 10.2 Trafik Mekaniği

**Konum trafiği** her tur pasif müşteri sağlar:
- Ana cadde: +5 pasif müşteri/tur
- Yan sokak: +2 pasif müşteri/tur
- Ücra köşe: +0 pasif (sadece aktif reklam işe yarar)

**Traffic area bonus'ları:**
- Işık lambası yanında: araç bekleme trafiği +3
- Okul/üniversite yanında: öğle saati +5
- Hastane yanında: sürekli trafik +3, düşük çek/müşteri
- Site/apartman girişi: sabah/akşam saati +4

## 10.3 Rakip Aynı Bölgeye Açılırsa

Rakip oyuncu ile aynı trafik alanındaysa:
- Pasif trafik ikiye bölünür
- Görünürlük rekabeti başlar (marketing kartları daha kritik)
- "Komşu Rekabeti" eventi tetiklenebilir

---

# BÖLÜM 11: ZİNCİR REAKSİYON SİSTEMİ

## 11.1 Temel İlke

**Her karar başka bir sistemi etkiler.** Oyunun temelidir.

Oyuncu şunu hissetmeli:
> "Kendi problemlerimi ben yaratıyorum."

## 11.2 Zincir Türleri

### Deterministik Zincirler (her zaman olur)
Belli kart kombinasyonları belli zinciri tetikler.

Örnek:
```
Ucuz Hammadde [Supplier slot'a yerleşti]
→ Kalite skoru -2
→ Google puanı -0.3/tur
→ Yeni müşteri azalır
→ Gelir düşer
```

### Olasılıklı Zincirler (%X ihtimalle)
Oyuncunun kararları riski artırır ama garantili değil.

Örnek:
```
Maaş 3 tur gecikti
→ %50 ihtimalle istifa
→ İstifa olursa slot boşalır
→ Kapasite düşer
```

### Kümülatif Zincirler (zamanla birikir)
Küçük kötü kararlar birikerek kriz yaratır.

Örnek:
```
Her tur %10 fire var (gizli)
→ 5 turda %50 stok eridi
→ 6. turda gelir aniden düşer
→ Oyuncu "neden düştü" diye şaşırır
```

## 11.3 Referans Domino Zinciri

**"Büyüme Tuzağı"** — en yaygın yeni oyuncu hatası:
```
Fazla reklam yap (3/3 marketing slot dolu)
↓ Müşteri patladı (+20)
↓ Operation kapasitesi yetersiz (3/4 slot dolu)
↓ Servis yavaşladı
↓ Kalite düştü
↓ Google puanı -0.4
↓ Kötü yorumlar birikte
↓ Müşteri kaçıyor (ama reklam hâlâ para yiyor)
↓ Gelir düştü, maliyet aynı
↓ Nakit kriz
↓ Maaşlar gecikti
↓ Çalışan istifa etti
↓ ÇÖKÜŞ
```

---

# BÖLÜM 12: EVENT SİSTEMİ

## 12.1 Event Kaynakları

| Kaynak | Açıklama |
|---|---|
| Deterministik | Oyuncunun geçmiş kararlarından doğar |
| Rastgele | Dünya olayları, random tetikleyici |
| Rakip kaynaklı | Rakibin hamlelerinden doğar |
| Mevsimsel | Takvim bazlı, öngörülebilir |

## 12.2 Event Tetikleyici Kuralları

**Deterministik eventler** belli koşullar oluşunca kesinlikle gelir:
- Ucuz hammadde 4+ tur kullanıldıysa → "Kalite Krizi" (gıda)
- Maaş 3 tur geciktiyse → "Personel Grevi" riski
- Google puanı 3.0'a düştüyse → "İtibar Krizi"
- Vergi 2 tur ödenmedi → "Vergi Denetimi"
- Sigortasız çalışan 3+ tur → "SGK Denetimi"

**Rastgele eventler** her tur %X ihtimalle gelir. Oyuncunun durumuna göre ihtimal değişir:
- İyi durumda: pozitif event ihtimali artar (Viral Trend, Food Festival)
- Kötü durumda: negatif event ihtimali artar

## 12.3 Event Kategorileri

| Kategori | Örnekler |
|---|---|
| Kalite Krizi | Gıda zehirlenmesi, bozuk ürün, skandal |
| Operasyon Krizi | Ekipman arızası, elektrik kesintisi |
| Personel Krizi | Personel grevi, toplu istifa, kavga |
| Tedarik Krizi | Tedarikçi sorunu, hammadde yokluğu |
| Dijital Kriz | App Store red, veri sızıntısı, Google cezası |
| Hukuki Kriz | Vergi denetimi, SGK denetimi, belediye kapama |
| Pozitif Event | Viral video, food festival, trend dalga |
| Rakip Kaynaklı | Rakip yatırım aldı, rakip kampanya başlattı |
| Mevsimsel | Ramazan, bayram, okul sezonu, yaz yavaşlığı |

## 12.4 Event Süresi

Her event kaç tur süreceğini belirtir:
- Anlık (1 tur): olay olur, biter
- Kısa (2–3 tur): baskı devam eder, çözüm gerekebilir
- Uzun (4+ tur): yapısal problem, aktif müdahale şart

## 12.5 Event Çözüm Mekaniği

Bazı eventler çözülebilir:
- "Sağlık Denetimi" → "Hijyen Sertifikası" kartı oynandıysa 0 hasar
- "Personel Grevi" → acil zam ver (nakit -50) veya 1 tur kaybet
- "Tedarik Sorunu" → yedek tedarikçi slotu varsa geç, yoksa 2 tur hammadde yok

---

# BÖLÜM 13: HUKUKİ / DENETİM SİSTEMİ

## 13.1 Yasal Risk Puanı

Her işletmenin gizli bir **Yasal Risk Puanı** var (0–100):
- 0–20: güvenli
- 21–50: denetim ihtimali artar
- 51–80: denetim yakın
- 81–100: denetim garantili

Risk puanı şunlarla artar:
- Sigortasız çalışan: +15/tur
- Vergiden kaçmak: +10/tur
- SKT geçmiş ürün: +20
- Ruhsatsız çalışmak: +25/tur
- Sahte yorum: +5

## 13.2 Denetim Türleri

| Denetim | Tetikleyici | Başarısız Olursa |
|---|---|---|
| SGK Denetimi | Sigortasız çalışan 3+ tur | Ceza: 30K–60K TL/kişi |
| Sağlık/Hijyen | Risk puanı 50+ | İşletme 1–2 tur kapalı |
| Vergi | Vergi kaçırma 2+ tur | 3x vergi + faiz |
| Gıda Güvenliği | UCuz hammadde + düşük kalite | Kapatma + haber |
| Belediye | Ruhsat sorunu | Mühür = kapatma |

## 13.3 Denetimden Geçme

Denetim geldiğinde:
- Yasal durum temizse: geçer, küçük bonus (müşteri güveni +1)
- Yasal risk 50+: %50–80 yakalanma ihtimali
- Yakalanırsa: ceza + event

---

# BÖLÜM 14: MEVSİMSELLİK

## 14.1 Mevsim Takvimi (25 Tur)

| Turlar | Sezon | Genel Etki |
|---|---|---|
| 1–5 | İlkbahar | Normal başlangıç |
| 6–10 | Yaz | Konum bağımlı |
| 11–15 | Sonbahar | Okul açılışı, yükseliş |
| 16–20 | Kış | Sezona göre değişir |
| 21–25 | Ramazan/Bayram zonu | Büyük spike |

## 14.2 İşletmeye Göre Mevsimsel Etkiler

| İşletme | Yaz | Kış | Ramazan | Okul Dönemi |
|---|---|---|---|---|
| Fast Food | Nötr | +%15 | Karışık | +%10 |
| Cafe | -%15 | +%25 | Nötr | +%20 |
| Tech App | Nötr | Nötr | Nötr | Nötr |
| Giyim | -%10 (indirim) | +%20 | +%15 | +%25 |
| Market | Nötr | +%10 | +%35 | +%10 |

## 14.3 Mevsimsel Eventler

- Ramazan (3 tur): yemek işletmelerinde öğle -%50, iftar +%30
- Okul Açılışı (2 tur): giyim +%25, kafe +%20
- Yılbaşı (2 tur): giyim +%30, market +%15
- Yaz Tatili (3 tur): şehirdeyse -%10, tatil beldesinde +%40
- Black Friday (1 tur): tüm işletmeler müşteri +%20

---

# BÖLÜM 15: RAKİP SİSTEMİ

## 15.1 Rakip Kimdir?

Rakip aynı işletme türüyle başlar. Aynı semtte, aynı müşteri havuzu için savaşır.

Rakip oyuncudan farklı kararlar alabilir:
- Pazara daha agresif girebilir (marketing önce)
- Kalite önceliklendirici olabilir
- Ucuz büyüme stratejisi deneyebilir

## 15.2 Rakip Hamle Havuzu

Her tur rakip aşağıdakilerden birini yapar:

| Hamle | Etkisi |
|---|---|
| Fiyat Düşür | Fiyat hassas müşteri rakibe geçer |
| Marketing Kampanyası | Oyuncudan müşteri çeker |
| Kalite Artır | Kalite arayan müşteri kaymaya başlar |
| Personel Çal | Oyuncunun düşük sadakatli çalışanına teklif |
| Yatırım Bul | Rakip 3 tur süreyle büyük hamle yapabilir |
| Yeni Şube Aç | Başka bölgede müşteri toplar |
| Sabote Et | Rakip Google'a şikayet eder (düşük ihtimalli) |

## 15.3 Rakip Zekası

Rakip oyuncunun zayıf noktalarını okur:
- Maaşlar gecikiyorsa: personel çalma hamlesi gelir
- Google puanı düşükse: agresif marketing başlar
- Nakit krizi varsa: rakip genişleme moduna geçer
- Oyuncu güçlüyse: rakip savunmaya çekilir, kalite artırır

## 15.4 Rakip Büyüme Takvimi

Rakip oyuncunun performansına göre büyür veya küçülür. Oyuncu baskı uygularsa rakip yavaşlar. Oyuncu kriz yaşarsa rakip hızlanır.

---

# BÖLÜM 16: TUR YAPISI

## 16.1 6 Faz

### FAZ 1 — KART ÇEKME

Oyuncuya girişim türüne özel kartlar gelir.
- Standart: 5 kart çek, 3 tanesini tut
- Marketing slotu doluysa marketing kartı oynayamazsın (slot stratejisi)
- Girişim türüne göre kart havuzu değişir

### FAZ 2 — PLANLAMA

Oyuncunun **3 action point**'i var.

Her kart oynamak veya aksiyon almak 1 action:
- Kart oyna (slota yerleştir veya kullan): 1 action
- Slottaki kartı çıkar: 1 action
- Maaş öde / geciktir kararı: 0 action (zorunlu karar)
- Kredi çek: 1 action

### FAZ 3 — SİMÜLASYON

Tüm kararlar çalışır:
- Müşteri hareketi hesaplanır (formül uygulanır)
- Reklam etkisi oluşur
- Tedarikçi kalitesi uygulanır
- Çalışan performansı hesaplanır

### FAZ 4 — OPERASYON

İç sistemler çalışır:
- Servis hızı = kapasite / müşteri sayısı
- Kalite = hammadde + şef seviyesi + moral
- Fire hesaplanır (gıda işletmeleri)
- Stok güncellenir

### FAZ 5 — EKONOMİ

Gelir–gider hesaplanır:
- Gelir = müşteri × harcama × kalite katsayısı
- Giderler tek tek düşülür
- Platform komisyonları kesilir
- Maaşlar ödenir (veya geciktirme kararı)
- Nakit bakiyesi güncellenir
- Vergi dönemi kontrolü
- İflas kontrolü

### FAZ 6 — EVENT + RAKİP

1. Event çekilir (deterministik kontrol önce, sonra random)
2. Event uygulanır
3. Rakip hamle yapar
4. Rakip hamlesi efektleri uygulanır
5. Tur sayacı +1
6. Kazanma/kaybetme koşulları kontrol edilir

---

# BÖLÜM 17: İKİNCİ ŞUBE / FRANCHISE

## 17.1 Açılış Koşulları

Oyuncu şu koşulları sağlayınca ikinci şube açabilir:
- Company Tier 3 (Şirket) veya üstü
- Nakit rezervi yeterli (minimum 300 nakit)
- İlk şube karlı (son 3 tur net pozitif)

## 17.2 İkinci Şube Etkisi

- Yeni operation slot grubu açılır (+4 operation)
- Yeni müşteri havuzuna erişim (farklı semt)
- Maliyet: kira + personel 2x

**Risk:** Yönetim dikkatinin bölünmesi. İlk şube ihmal edilirse çöker.

## 17.3 Franchise Seçeneği

Tier 4'te (Holding) franchise kurma seçeneği açılır:
- Rakibe franchise verme (uzun vadeli gelir, rakibi büyütme riski)
- Üçüncü şube açma (maksimum genişleme)

---

# BÖLÜM 18: KAZANMA / KAYBETME

## 18.1 Kazanma Koşulları

| Koşul | Açıklama |
|---|---|
| Market Dominance | Müşteri havuzunun %60'ına ulaş |
| Rakibi İflas Ettir | Rakip nakit biterse oyun biter |
| 25. Tur Sonu | Daha güçlü marka (müşteri oranı + tier) kazanır |

## 18.2 Kaybetme Koşulları

| Koşul | Açıklama |
|---|---|
| İflas | Nakit 3 tur üst üste negatif |
| Operasyon Çöküşü | Tüm staff slotları boşaldı, işletme durdu |
| Rakip %60 | Rakip dominance aldı |
| Belediye Kapama | Yasal risk sonucu işletme kapatıldı |

## 18.3 Erken Bitiş Bonusu

25. turdan önce kazanılırsa kalan tur sayısı × bonus katsayısı puanı artırır.

## 18.4 Run Sonu Skor

| Faktör | Katkı |
|---|---|
| Son müşteri payı | %40 |
| Company Tier | %20 |
| Nakit rezervi | %15 |
| Puan (Google/App Store) | %15 |
| Tur sayısı (erken bitiş bonus) | %10 |

---

# BÖLÜM 19: KART SİSTEMİ (GENEL KURALLAR)

## 19.1 Kart Kategorileri

| Kategori | Açıklama |
|---|---|
| Operation | Fiziksel altyapı slota yerleşir |
| Staff | Çalışan slota yerleşir |
| Marketing | Reklam kampanyası slota yerleşir |
| Supplier | Tedarikçi slota yerleşir |
| Action | Tek seferlik kullanılır, slota girmez |
| Upgrade | Kalıcı etki, mevcut slot özelliğini geliştirir |
| Risk | Yüksek getiri, yüksek risk |
| Event | Genellikle temp slota düşer |

## 19.2 Kart Havuzu Yapısı

- **%80:** Seçilen girişim türüne özel kartlar
- **%20:** Genel kartlar (tüm işletme türlerine uygulanabilir)

## 19.3 Genel Kartlar (Tüm Girişimlerde Gelebilir)

**Ekonomi:**
- Banka Kredisi Çek (Action): nakit +200, faiz %8/tur
- Vergi Planlaması (Action): bu dönem vergi -%20
- Maliyet Optimizasyonu (Upgrade): tüm giderler -%10

**Personel:**
- Takım Motivasyonu (Action): tüm çalışanlar moral +2 bu tur
- Personel Eğitimi (Action): rastgele 1 çalışan XP +2
- İş Güvenliği Sistemi (Upgrade): yasal risk -10

**Strateji:**
- Piyasa Araştırması (Action): rakibin mevcut hamlesini gör
- Kriz Yönetimi (Upgrade): negatif event etkisi -%25
- İkinci Şube Hazırlığı (Upgrade): şube açma maliyeti -%20

## 19.4 Kart Denge Kuralları

- **Her kart her zaman iyi değildir.** Kartın değeri işletmenin durumuna göre değişir.
- Boş slota bakılır: slot yoksa kart oynanamaz
- Sinerji > tek kart gücü: doğru kombinasyon her kartta kazanır
- Risk kartları: yüksek getiri ama deterministik olumsuz zincir tetikleyebilir

---

# BÖLÜM 20: META PROGRESSION VE SKOR

## 20.1 Run Sonu

Her run sonunda:
- Skor hesaplanır (Bkz. 18.4)
- Kilit başarımlar kontrol edilir
- Meta XP verilir

## 20.2 Meta XP ve Kilit Açma

| XP Miktarı | Açılan İçerik |
|---|---|
| 100 | Yeni event kartı |
| 250 | Yeni girişim türü |
| 500 | Zorluk modu açılır |
| 1000 | Senaryo modu |

## 20.3 Zorluk Seviyeleri

| Seviye | Fark |
|---|---|
| Kolay | Rakip pasif, eventler hafif |
| Normal | Varsayılan |
| Zor | Rakip agresif, enflasyon hızlı |
| Acımasız | Nakit kriz riski 2x, rakip her zayıflığı kullanır |

---

# BÖLÜM 21: TEKNİK MİMARİ NOTU

GDD, kod mimarisini belirlemez. Aşağıdakiler referans niteliğindedir:

- **Namespace:** `EmpireOfCards.{Domain}` (Core, Gameplay, UI, Data, Bootstrap, World)
- **Mimari:** Bootstrap → WiringService → GameManager → StateMachine + TurnManager
- **Event sistemi:** Statik EventBus, ClearAll() scene unload'da
- **Kart verisi:** ScriptableObject (runtime'da kopyalanır, asıl veri değiştirilmez)
- **Slot sistemi:** SlotZone3D bileşenleri Board3D üzerinde

---

# BÖLÜM 22: MVP KAPSAMI

## 22.1 MVP (İlk Oynanabilir)

- [ ] 1 girişim türü (Fast Food)
- [ ] 17 başlangıç slot
- [ ] 30 kart (Fast Food havuzu)
- [ ] Temel ekonomi sistemi
- [ ] Temel personel sistemi (moral + maaş)
- [ ] Google puan sistemi
- [ ] Rakip (basit AI, 5 hamle)
- [ ] 6 faz tur yapısı
- [ ] 3 kazanma/kaybetme koşulu

## 22.2 Alpha

- [ ] Tüm 5 girişim türü
- [ ] Slot genişleme sistemi (tier bazlı)
- [ ] Tam event havuzu (30+ event)
- [ ] Kredi/borç sistemi
- [ ] Mevsimsellik
- [ ] Rakip AI (zayıf nokta okuma)

## 22.3 Beta

- [ ] İkinci şube sistemi
- [ ] Meta progression
- [ ] Zorluk seviyeleri
- [ ] Balans tuning (tüm girişimler)
- [ ] Ses + müzik sistemi
- [ ] UI polish

---

> **Önemli:** Her işletme türünün kart listesi, özel mekanikleri, event listesi ve zincir reaksiyon örnekleri için:
> `Assets/steam-card-game-gdd/businesses/` klasörüne bak.
> GDD yalnızca tüm işletmeler için geçerli CORE SİSTEMLERİ içerir.
