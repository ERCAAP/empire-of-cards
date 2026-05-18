# GAME DESIGN DOCUMENT
# "Empire of Cards"

> Versiyon: 2.0 | Tarih: 2026-05-18
> Engine: Unity (C#) | Platform: PC (Steam)
> Ekip: Solo Developer | Fiyat: $9.99-$12.99

---

# BÖLÜM 1: OYUN NEDİR?

## 1.1 Tek Cümle

Kartlarla iş kurarsın, çalışan alırsın, rakibinle aynı müşteri havuzu için savaşırsın.

## 1.2 Oyuncunun Yaptığı Şey

Masanın başında oturuyorsun. Önünde boş slotlar var. Elinde kartlar var.

Her tur:
1. Bir event gelir — dünya değişir
2. Kart çekersin
3. 3 hakkın var — kartları masaya koyarsın
4. Masa çalışır — para gelir, müşteri gelir, combo patlar
5. Rakip oynar

**Amaç:** Masadaki 10 bölgeden 6'sını ele geçir. Bu senin market hakimiyetin.

## 1.3 Neden Eğlenceli?

| An | Hissi |
|---|---|
| Kart koydun, anında müşteri token'ları masana kaydı | "Büyüyorum!" |
| İki kart birleşti, COMBO patladı | "Oha bunu keşfettim!" |
| Rakip müşterilerini çaldı | "Seni gidi..." |
| Kriz geldi, işletmen kapandı | "Hayır! Plan yapmalıyım!" |
| Son turda tam %60'a ulaştın | "EVEEET!" |

---

# BÖLÜM 2: MASA DÜZENİ

## 2.1 Masada Ne Var?

```
┌──────────────────────────────────────────────────────┐
│                                                      │
│  RAKİP TARAFI                                        │
│  [Rakip İşletme 1]  [Rakip İşletme 2]  [Boş]       │
│   └─Çalışan          └─Çalışan                      │
│                                                      │
│  ┌─────────── BÖLGE HARİTASI ──────────────┐        │
│  │ [1][2][3][4][5]  [6][7][8][9][10]       │        │
│  │  ████████░░░░░    ▓▓▓▓▓▓▓░░░░░         │        │
│  │  SEN: 4 bölge     RAKİP: 3 bölge       │        │
│  │  Boş: 3 bölge                           │        │
│  └─────────────────────────────────────────┘        │
│                                                      │
│  📰 AKTİF EVENT: "Kahve Çılgınlığı" (1 tur kaldı)  │
│                                                      │
│  SENİN TARAFIN                                       │
│  [İşletme 1]     [İşletme 2]     [ + Yeni Slot ]    │
│   └─Çalışan 1     └─Çalışan 1                       │
│   └─Çalışan 2     └─___boş                          │
│                                                      │
│  💰 620    ⚖️ FBI: %12    🔄 Tur 8/15               │
│  Aksiyon: ●●● (3 kaldı)                             │
│                                                      │
│  ┌────────── ELİNDEKİ KARTLAR ──────────┐           │
│  │ [Kart] [Kart] [Kart] [Kart] [Kart]  │           │
│  └──────────────────────────────────────┘           │
│                                                      │
│  [DESTE: 22]   [SAT]   [DÜKKAN]   [TUR BİTİR]     │
└──────────────────────────────────────────────────────┘
```

## 2.2 Her Alanın Açıklaması

### Bölge Haritası (Ortadaki 10 kutu)

Bu oyunun kazanma mekaniği. Soyut "%60 market share" yerine **görsel alan kontrolü.**

```
[■][■][■][■][░][░][░][▓][▓][▓]
 SEN: 4      BOŞ: 3   RAKİP: 3
```

- 10 bölge var. Her bölge = %10 market share.
- Senin bölgelerin (mavi), rakibin bölgeleri (kırmızı), boş bölgeler (gri).
- **6 bölge ele geçirirsen = KAZANDIN.**
- **Rakip 7 bölge ele geçirirse = KAYBETTİN.**
- Bölgeler müşteri sayına göre otomatik dağılır.

**Neden bu sistem?** Yüzde sayısı soyut. Ama 10 kutunun 6'sını kaplamak GÖRSEL. Oyuncu "1 bölge daha!" derken gerilim hisseder.

### Senin Tarafın (Alttaki slotlar)

Buraya işletme kartlarını koyarsın.

```
Başlangıç: 3 slot
[Slot 1]  [Slot 2]  [Slot 3]

Upgrade ile: 4-5 slot
[Slot 1]  [Slot 2]  [Slot 3]  [Slot 4]  [Slot 5]
```

**Neden 3 slot?** Az slot = her kart koyma kararı önemli. 10 slot olsa her şeyi koyardın, karar olmazdı.

Her işletmenin altında **çalışan yuvaları** var:

```
[Kahveci ☕]
 └─ [Barista]     ← Çalışan 1
 └─ [___boş___]   ← Çalışan 2 (daha koymadın)
```

### Rakip Tarafı (Üstteki slotlar)

Rakip AI'ın işletmeleri. Oyuncu dokunmaz ama görür. Rakibin ne yaptığını takip edersin.

### El Kartları (En altta)

Her tur 5 kart çekersin. Bunlardan 3 tanesini oynayabilirsin (3 aksiyon hakkı).

### Dükkan

Her turda 3 rastgele kart satılır. Para ile satın alırsın → destene girer.

---

# BÖLÜM 3: KART TİPLERİ

5 tip kart var. Her birinin masadaki yeri ve işlevi farklı.

## 3.1 İşletme Kartları (Mavi)

**Nereye gider:** Senin tarafındaki boş slota.
**Ne yapar:** Her tur otomatik para kazanır + müşteri çeker.
**Kalıcı mı:** Evet. Koyduğun yerde kalır (kapanana kadar).

```
Kart koyma anı:
  Boş slot → [Kahveci ☕ koydun] → Anında 5 müşteri token'ı masaya çıkar
                                    (fiziksel feedback!)
```

**Evrim sistemi:** İşletmeler yeterli müşteri çekince seviye atlar.

```
Büfe (Lv.1)  →  Dükkan (Lv.2)  →  Mağaza (Lv.3)
💰50/tur        💰80/tur           💰120/tur
3 müşteri       5 müşteri          8 müşteri
1 çalışan       2 çalışan          2 çalışan

Koşul: 15 tur boyunca toplam 40 müşteri çekince → seviye atlar
Efekt: Kart görseliz değişir, stat artar, "LEVEL UP!" text patlar
```

**MVP İşletmeler (8 adet):**

| # | İsim | Maliyet | Gelir/tur | Müşteri/tur | Çalışan Slot | Özel |
|---|---|---|---|---|---|---|
| B01 | Büfe | Bedava | 💰50 | 3 | 1 | Başlangıç kartı. Evrim: Büfe→Restoran→Zincir |
| B02 | Kahveci | 💰150 | 💰80 | 5 | 2 | Trend aktifken gelir x1.5 |
| B03 | Burger Zinciri | 💰250 | 💰100 | 6 | 3 | En çok çalışan slotu |
| B04 | Tech Startup | 💰200 | 💰0→150 | 0→4 | 2 | İlk 3 tur gelir yok, sonra patlama |
| B05 | Gece Kulübü | 💰350 | 💰180 | 10 | 2 | Sadece trend varken çalışır |
| B06 | Organik Çiftlik | 💰120 | 💰40 | 2 | 1 | Tüm food işletmelerine +💰20 bonus |
| B07 | Kripto Borsası | 💰300 | 💰0-250 | 2 | 1 | Her tur zar at: rastgele gelir |
| B08 | Reklam Ajansı | 💰200 | 💰60 | 3 | 2 | Tüm işletmelere +2 müşteri |

## 3.2 Çalışan Kartları (Yeşil)

**Nereye gider:** Bir işletmenin çalışan yuvasına.
**Ne yapar:** İşletmenin gücünü artırır. Her birinin passif + aktif yeteneği var.
**Kalıcı mı:** Evet (ayrılana veya kovulana kadar).

**YENİ: Aktif Yetenek Sistemi**

Her çalışanın 2 yeteneği var:

```
BARISTA
├── Passif: +3 müşteri/tur (kahvecide +6)
└── Aktif: [1 aksiyon harca] "Latte Festivali" → Bu tur müşteri x2

Yani çalışanlar sadece "oturup stat veren" kartlar değil.
Oyuncu bazen "bu tur Barista'nın aktif yeteneğini mi kullanayım,
yoksa yeni kart mı oynayayım?" diye karar verir.
```

**MVP Çalışanlar (10 adet):**

| # | İsim | Maaş/tur | Passif | Aktif (1 aksiyon) | Özel |
|---|---|---|---|---|---|
| C01 | Stajyer | 💰15 | +1 müşteri | "Koştur": +3 müşteri bu tur | Zayıf ama ucuz |
| C02 | Çaylak Pazarlamacı | 💰20 | Gelir +%10 | "Kampanya": +5 müşteri bu tur | Başlangıç kartı |
| C03 | Barista | 💰25 | +3 müşteri (coffee: +6) | "Latte Festivali": müşteri x2 bu tur | Kahveci combo parçası |
| C04 | Şef | 💰30 | +3 müşteri (food: gelir+💰30) | "Özel Menü": 1 tur gelir x1.5 | Food combo parçası |
| C05 | Marketing Gurusu | 💰45 | Gelir +%25 | "Viral Kampanya": tüm işletmelere +3 müşteri | Combo parçası |
| C06 | Influencer | 💰50 | +5 müşteri (trend: +12) | "Story At": rakipten 5 müşteri çal | Trend bağımlı |
| C07 | Hacker | 💰60 | Rakipten -4 müşteri | "Veri Sız": rakibin 1 işletmesi 1 tur gelir yok | İllegal, FBI +%10/tur |
| C08 | Muhasebeci | 💰30 | Vergi -%50 | "Vergi Planı": bu tur vergi %0 (legal) | Sıkıcı ama sağlam |
| C09 | Dolandırıcı | 💰40 | +💰120/tur (illegal) | "Ponzi": +💰300 ama sonraki tur -💰150 | FBI +%12/tur |
| C10 | Sadık Müdür | 💰45 | Transfer koruması, +💰20 | "Motivasyon": tüm çalışanlar +1 müşteri bu tur | Savunmacı |

## 3.3 Action Kartları (Kırmızı)

**Nereye gider:** Hiçbir yere. Oynanır, efekt olur, çöpe gider.
**Ne yapar:** Anlık güçlü etki.
**Kalıcı mı:** Hayır. Tek kullanımlık.

| # | İsim | Maliyet | Etki |
|---|---|---|---|
| A01 | El İlanı | 💰0 | +3 müşteri bu tur (başlangıç kartı) |
| A02 | Küçük Yatırım | 💰0 | Anında +💰150 (başlangıç kartı) |
| A03 | Viral Pazarlama | 💰150 | Bu tur TÜM müşteri x2 |
| A04 | Düşmanca Devralma | 💰400 | Rakibin en zayıf işletmesini kapat |
| A05 | Sahte Yorumlar | 💰80 | +8 müşteri. FBI +%12 |
| A06 | Fiyat Kırma | Gelir %50 | Rakipten 8 müşteri çal |
| A07 | Sabotaj | 💰250 | Rakip 1 tur üretim yapamaz. FBI +%15 |
| A08 | Yatırımcı Sunumu | 💰0 | +💰600 anında. Sonraki 3 tur gelir %15'i yatırımcıya |
| A09 | Acil İşe Alım | 💰100 | Desteden rastgele 1 çalışan çek, hemen oyna |
| A10 | Tasfiye | 1 işletme | İşletmeyi sat, değerinin 2x'i para al |

## 3.4 Upgrade Kartları (Mor)

**Nereye gider:** İşletmenin yanına veya masaya genel.
**Ne yapar:** Kalıcı iyileştirme.
**Kalıcı mı:** Evet.

| # | İsim | Maliyet | Etki |
|---|---|---|---|
| U01 | Ofis Malzemeleri | 💰0 | 1 işletme gelir +%10 (başlangıç kartı) |
| U02 | Otomasyon | 💰300 | 1 işletme gelir +%30. Ama 1 çalışan slotu kapanır |
| U03 | Teslimat Ağı | 💰250 | Tüm işletmelere +2 müşteri/tur |
| U04 | Reklam Panosu | 💰120 | +3 müşteri/tur (genel) |
| U05 | Güvenlik Sistemi | 💰280 | FBI riski -%25 |
| U06 | Yapay Zeka Asistanı | 💰400 | +1 aksiyon hakkı (3→4) |

## 3.5 Event Kartları (Sarı)

**Nereye gider:** Masanın ortasına. Oyuncu oynamaz — otomatik gelir.
**Ne yapar:** Dünyayı değiştirir. Hem seni hem rakibi etkiler.
**Kalıcı mı:** 1-2 tur aktif, sonra kalkar.

| # | İsim | Süre | Etki |
|---|---|---|---|
| E01 | Kahve Çılgınlığı | 2 tur | Food/coffee işletmeleri +%50 müşteri |
| E02 | Ekonomik Kriz | 2 tur | TÜM gelirler -%30. İşletme seviyesi düşebilir! |
| E03 | Viral Trend | 1 tur | Marketing kartları 2x etkili |
| E04 | Veri Sızıntısı | 1 tur | Tech işletmeleri -5 müşteri (güvenlik varsa bağışık) |
| E05 | Yatırımcı Sezonu | 1 tur | Finance kartlar 2x etkili |
| E06 | İptal Kültürü | 1 tur | FBI riski >%30 olan: tüm müşteri -%40 |

---

# BÖLÜM 4: BİR TUR NASIL İŞLER

## 4.1 Özet

```
Her tur 5 adım. Toplam ~1.5 dakika.

ADIM 1: EVENT          → Dünya değişir (sen izlersin)
ADIM 2: KART ÇEK       → 5 kart alırsın
ADIM 3: OYNA           → 3 aksiyon harcarsın (ANA KARAR ANI)
ADIM 4: MASA ÇALIŞIR   → Sistem hesaplar, efektler oynar (sen izlersin)
ADIM 5: RAKİP OYNAR    → Düşman hamle yapar (sen izlersin)
```

## 4.2 Adım Adım Detay

### ADIM 1: EVENT (Otomatik — 3 saniye)

Her 3 turda 1 event gelir (tur 3, 6, 9, 12, 15).

```
Masanın ortasında kart ters duruyordur. Açılır:

  ╔══════════════════════╗
  ║  📰 KAHVE ÇILGINLIĞI ║
  ║                      ║
  ║  Food işletmeleri     ║
  ║  +%50 müşteri!       ║
  ║                      ║
  ║  Süre: 2 tur         ║
  ╚══════════════════════╝

Ekran efekti: Kart döner, parlak ışık, gazete açılma sesi.
```

Oyuncu event'i okur ve BU TURA göre strateji kurar:
- "Kahve trendi geldi, Influencer'ı kahveciye koysam süper olur!"
- "Kriz geldi, yeni işletme açmak yerine para biriktireyim."

### ADIM 2: KART ÇEK (5 saniye)

Destenden 5 kart çekilir. Masanın altında fan şeklinde dizilir.

```
ELİN:
[Burger Zinciri] [Influencer] [El İlanı] [Sahte Yorum] [Otomasyon]
     mavi            yeşil      kırmızı     kırmızı       mor
```

**1 redraw hakkın var:** Bir kartı atıp yerine yenisini çekebilirsin.

Karar: "Sahte Yorum bana lazım değil bu tur, değiştireyim."

### ADIM 3: OYNA (30-90 saniye — Ana karar anı)

**3 aksiyon hakkın var.** Her kart oynama = 1 aksiyon. Çalışan aktif yeteneği = 1 aksiyon.

Örnek tur:

```
Aksiyon 1: Burger Zinciri'ni 3. slota koydum (💰250 ödedim)
           → Anında: Kart slota snap olur, 6 müşteri token'ı masaya çıkar
           → Ses: "thud" + "pop pop pop" (müşteriler)

Aksiyon 2: Influencer'ı Kahveci'nin altına koydum
           → Anında: Influencer kartı yerine oturur
           → Kahve Çılgınlığı aktif + Influencer = müşteri +12
           → Tokens masaya akar
           → "TREND BOOST!" text belirir

Aksiyon 3: Barista'nın aktif yeteneğini kullandım: "Latte Festivali"
           → Bu tur Kahveci müşterisi x2
           → Ekran sallanır, kahve efekti

Tur Bitir butonuna bas.
```

**Oyuncu ayrıca şunları da yapabilir (aksiyon HARCAMAZ):**
- Kart satmak (istemediği kartı 💰 karşılığı çöpe at)
- Kart yakmak (💰0 ama deste küçülür = iyi kartları sık çekersin)
- Dükkanı kontrol etmek

### ADIM 4: MASA ÇALIŞIR (Otomatik — 5-8 saniye)

**Bu an oyunun KALBI. Oyuncu hiçbir şey yapmaz. İzler ve tatmin olur.**

Unity'de bu faz adım adım animasyonla oynar:

```
───── Adım 4a: İŞLETMELER ÜRETİR ─────
  Büfe      → ☕☕☕ (3 ürün animasyonu)
  Kahveci   → ☕☕☕☕☕☕☕☕ (8 ürün — Barista + Influencer + trend!)
  Burger    → 🍔🍔🍔🍔🍔🍔 (6 ürün)

───── Adım 4b: MÜŞTERİLER KAYAR ─────
  Bölge haritasındaki token'lar güncellenir:
  Boş bölgelerden senin tarafına token kayar
  [■][■][■][■][■][░][░][▓][▓][▓]
  SEN: 4 → 5!  Animasyon: 5. kutu maviye döner

───── Adım 4c: COMBO KONTROLÜ ─────
  Barista + Kahveci + Kahve Çılgınlığı = ✅ "LATTE SANATI!"
  → Ekran sallanır
  → "COMBO! LATTE SANATI +40💰 +4 müşteri" text patlar
  → Altın parıltı efekti
  → Özel ses

───── Adım 4d: GELİR HESAPLANIR ─────
  💰 +50  Büfe
  💰 +160 Kahveci (80 + trend×1.5 + combo)
  💰 +100 Burger
  💰 -110 Maaşlar (Stajyer+Barista+Influencer+Çaylak)
  💰 -46  Vergi (%15)
  ─────────────
  💰 +154 NET
  
  Para counter animasyonla sayar: 620 → 774
  Ses: "ka-ching ka-ching ka-ching"

───── Adım 4e: BOZULMA KONTROLÜ ─────
  FBI riski %12 → Zar atılır → %12 < zar(47) → Kurtuldun!
  Çalışan draması → Bu tur yok
  İşletme kapanma → Bu tur yok
```

**BOZULMA MEKANİĞİ — Neden önemli:**

Sadece büyüme varsa oyun monotonlaşır. Masada şeyler **bozulmalı**:

| Bozulma | Tetik | Ne olur | Görsel |
|---|---|---|---|
| İşletme kapanması | Kriz event'i sırasında gelir <💰20 olan | 2 tur kapalı (kart ters döner) | Kart griye döner, "KAPALI" damgası |
| Çalışan ayrılma | 8 turdan fazla çalışan + maaş artışı vermezsen | Kart masadan kalkar | Çalışan yürüyerek çıkar animasyonu |
| FBI baskını | İllegal risk > zar | Para cezası + illegal çalışan kovulur | Kırmızı flash, siren, rozet animasyonu |
| Rakip sabotajı | Rakip agresif moddayken | 1 işletme 1 tur çalışmaz | Duman efekti |
| Müşteri kaybı | Rakip senden bölge alınca | Token'lar senin tarafından karşıya kayar | Token kayma animasyonu |

### ADIM 5: RAKİP OYNAR (Otomatik — 5 saniye)

```
  🏢 MEGACORP hamle yapıyor...
  
  → "Teknoloji Mağazası" açtı
  → "Pazarlamacı" işe aldı
  → Senin 2 müşterini çekti
  
  Bölge haritası güncellendi:
  [■][■][■][■][■][░][▓][▓][▓][▓]
  SEN: 5    RAKİP: 4    Boş: 1
  
  💬 "Yaklaşıyorum..." — MegaCorp
```

Rakip eylemleri ŞEFFAF. Oyuncu ne olduğunu görür, gelecek tura göre plan yapar.

---

# BÖLÜM 5: SLOT SİSTEMİ

## 5.1 İşletme Slotları

**Neden sınırlı?** Sınırsız slot = karar yok. 3 slot = "hangisini koyayım?" kararı.

```
BAŞLANGIÇ: 3 slot
[Slot 1]  [Slot 2]  [Slot 3]

Her slotun altında çalışan yuvaları var:
[Slot 1        ]
 └─ [Çalışan 1]
 └─ [Çalışan 2]  ← İşletmeye göre 1-3 arası

Slot açma yolları:
- "Extra Slot" upgrade kartı (💰800) → +1 slot
- Ascension 0'da max 5 slot
```

## 5.2 Çalışan Yuvaları

Her işletmenin kaç çalışan alacağı **o işletmeye bağlı:**

| İşletme | Çalışan Slotu | Neden |
|---|---|---|
| Büfe | 1 | Küçük işletme |
| Kahveci | 2 | Orta |
| Burger Zinciri | 3 | Büyük — çok sinerji potansiyeli ama çok maaş |
| Tech Startup | 2 | Orta |
| Gece Kulübü | 2 | Orta |
| Organik Çiftlik | 1 | Küçük support |
| Kripto Borsası | 1 | Tek adam operasyonu |
| Reklam Ajansı | 2 | Orta |

**Çalışanı doğru işletmeye koymak önemli:**
- Barista → Kahveci'ye koyarsan: +6 müşteri (2x güçlü)
- Barista → Burger'e koyarsan: +3 müşteri (normal)

Bu karar oyunun temel stratejisi.

## 5.3 Masanın Büyümesi (Tur Tur)

```
TUR 1:  [Büfe]    [___]    [___]           ← 1 işletme, 1 çalışan
         └─Stajyer

TUR 5:  [Büfe]    [Kahveci] [___]          ← 2 işletme, 3 çalışan
         └─Stajyer  └─Barista
         └─Paz.cı   └─___

TUR 10: [Büfe]    [Kahveci] [Burger]       ← 3 işletme, 5 çalışan, 2 upgrade
         └─Stajyer  └─Barista └─Şef
         └─Paz.cı   └─Influer └─Mark.Guru
                                └─___
        [Otomasyon] [Teslimat Ağı]         ← Upgrade'ler masada

TUR 15: [Restoran] [Kahve Z.] [Burger Z.] [Reklam A.] ← 4 işletme (2'si evrim geçirmiş)
         └─Stajyer  └─Barista   └─Şef       └─Mark.Guru
         └─Paz.cı   └─Influer   └─Müdür     └─___
                     └─Muhase.   └─Doland.
        [Otomasyon] [Teslimat] [Güvenlik] [AI Asist.]

MASA DOLU = "İmparatorluk kurdum" hissi
```

---

# BÖLÜM 6: BÖLGE HARİTASI (Kazanma Mekaniği)

## 6.1 Nasıl Çalışır

10 bölge. Her bölge = bir müşteri grubu.

```
Her turun sonunda:
  Senin toplam müşteri = işletme + çalışan + combo + upgrade toplamı
  Rakibin toplam müşteri = rakip hesaplaması
  
  10 bölge şöyle dağıtılır:
  - Senin oranın kadar bölge sana
  - Rakibin oranın kadar bölge rakibe
  - Geri kalan boş
  
  Örnek:
  Sen: 45 müşteri, Rakip: 30 müşteri, Toplam market: 100
  Sen: %45 → 4-5 bölge
  Rakip: %30 → 3 bölge
  Boş: 2-3 bölge
```

## 6.2 Bölge El Değiştirmesi

Bölgeler her tur yeniden hesaplanır. Ama geçiş **animasyonla** olur:

```
Önceki tur: [■][■][■][■][░][░][░][▓][▓][▓]
Bu tur:     [■][■][■][■][■][░][░][░][▓][▓]
                          ↑              ↑
                    Boş→Sana        Rakipten→Boşa

Animasyon: Token mavi renge döner (senin tarafına geçiş)
Ses: "ding" + çıtırtı
```

## 6.3 Kazanma / Kaybetme

| Durum | Sonuç |
|---|---|
| 6+ bölge senin | **KAZANDIN** — Run biter |
| 7+ bölge rakibin | **KAYBETTİN** — Rakip domine etti |
| Paran 💰0'a düşerse | **İFLAS** — Run biter |
| 15 tur doldu, 6 bölge yok | Skor hesaplanır, "yeterli değil" |

**Dinamik bitiş:** %60'a (6 bölge) ulaştığın AN run biter. 15. turu beklemek zorunda değilsin. Erken bitirirsen bonus skor.

---

# BÖLÜM 7: COMBO SİSTEMİ

## 7.1 Combo Nedir

2-3 kart aynı anda masadayken otomatik tetiklenen sinerji.

**Combo ANINDA tetiklenir.** Adım 4'ü (sistem çalışır) beklemez. Kart koyduğun an koşul sağlanıyorsa → patlama.

## 7.2 10 Combo

| # | İsim | Gerekli Kartlar | Bonus | Zorluk |
|---|---|---|---|---|
| 1 | Latte Sanatı | Kahveci + Barista | Kahveci gelir +💰40, +4 müşteri | Kolay |
| 2 | Organik Sinerji | Organik Çiftlik + herhangi food işletme | Tüm food'lara +💰30 | Kolay |
| 3 | Güvenli Suç | Güvenlik Sistemi + illegal çalışan | FBI risk artışı %50 azalır | Kolay |
| 4 | Viral Fırtına | Influencer + Viral Trend event'i | Influencer'ın işletmesi müşteri x3 | Orta |
| 5 | Fast Food İmparatorluğu | Burger + Şef + Teslimat Ağı | Burger gelir x2, serbest müşterilerin %30'u sana | Orta |
| 6 | Yeraltı İmparatorluğu | Dolandırıcı + Hacker | +💰200/tur ekstra. FBI +%8 ekstra | Orta |
| 7 | Reklam Bombardımanı | Reklam Ajansı + Mark. Guru + Reklam Panosu | Tüm işletmelere +8 müşteri | Zor |
| 8 | Kriz Avcısı | Para >💰1000 + Ekonomik Kriz event | Dükkan %50 indirim, rakipten 1 çalışan transfer | Orta |
| 9 | AI Devrimi | Tech Startup(aktif) + Otomasyon + AI Asistanı | Tech gelir x3, +1 ekstra aksiyon | Zor |
| 10 | Monopol | 6+ bölge senin + 4 işletme aktif | Tüm gelir +%20, rakip tur başı -3 müşteri | Otomatik |

## 7.3 Combo Feedback

Combo tetiklendiğinde (anında):
1. **Ekran sallanır** (screen shake, 0.3 saniye)
2. **Büyük text belirir** ("LATTE SANATI!" golden text, yukarı kayarak kaybolur)
3. **Özel ses çalar** (combo_trigger.wav)
4. **Kart çevresinde parıltı** (glow efekti 2 saniye)
5. **Bonus uygulanır** (para counter hızla sayar, müşteri token'ları akar)

---

# BÖLÜM 8: RAKİP AI

## 8.1 Rakip Ne Yapar

Rakip de aynı kurallarla oynuyor (hile yok). Her tur 2 hamle yapar.

**Karar ağacı (basit, rule-based):**

```
HER TUR:
  1. Senin bölge sayın > 5 ise → AGRESIF: sabotaj veya müşteri çalma
  2. Rakip para > işletme maliyeti VE 3'ten az işletme → YENİ İŞLETME
  3. Boş çalışan slotu var → ÇALIŞAN AL
  4. Event rakibe uygun → EVENT BONUSU KULLAN
  5. Hiçbiri değilse → NORMAL BÜYÜME (+💰50, +2 müşteri)
```

## 8.2 Rakip Kişiliği

MVP'de 1 rakip: **MegaCorp** (Normal zorluk)

```
MegaCorp:
- Strateji: Dengeli büyüme, orta agresiflik
- Kişilik: Kurumsal, soğuk
- Taunt'lar:
  → Büyürken: "Pazar payımız artıyor."
  → Sen büyürken: "İlginç bir hamle..."
  → Agresifken: "Bu sektörde ikimize yer yok."
  → Kaybederken: "Bu böyle bitmez."
```

## 8.3 Rakip Büyüme Takvimi

| Tur | Rakip Durumu |
|---|---|
| 1 | 1 işletme (💰80/tur), 1 çalışan. Bölge: 2 |
| 5 | 2 işletme, 2-3 çalışan. Bölge: 3 |
| 8 | 2-3 işletme, 3-4 çalışan. Bölge: 3-4 |
| 12 | 3 işletme, 4-5 çalışan, agresif. Bölge: 4-5 |
| 15 | 3-4 işletme, 5-6 çalışan. Bölge: 4-6 |

---

# BÖLÜM 9: EKONOMİ

## 9.1 Para Akışı

```
HER TUR:
  + İşletme gelirleri (otomatik)
  + Combo bonusları
  + İllegal gelirler (Dolandırıcı vs.)
  - Çalışan maaşları (otomatik)
  - Vergi (brüt gelir × %15, muhasebeci ile %7.5)
  = NET → Paraya eklenir/çıkarılır

Para 0'a düşerse = İFLAS = oyun biter
```

## 9.2 Denge Hedefleri

| Tur | Beklenen Para | Beklenen Bölge | His |
|---|---|---|---|
| 1-3 | 💰400-550 | 1-2 | Para sıkı, her kuruş önemli |
| 4-7 | 💰500-800 | 2-3 | İlk combo, ilk büyüme hissi |
| 8-11 | 💰700-1200 | 3-5 | Motor çalışıyor, rakip baskısı başlıyor |
| 12-15 | 💰1000-2500 | 4-6 | Final kavgası, kriz riski |

## 9.3 FBI Sistemi

```
İllegal kart kullanınca → FBI risk sayacı artar
Her tur sonu → kontrol:
  random(0-100) < FBI risk → BASKIN!
  
BASKIN:
  → 💰300 ceza
  → En pahalı illegal çalışan kovulur
  → Risk sıfırlanır
  
Güvenlik Sistemi: risk artışını %50 azaltır
```

---

# BÖLÜM 10: RUN YAPISI

## 10.1 15 Tur, 3 Aşama

```
AŞAMA 1: KURULUŞ (Tur 1-5)
├── İlk işletme + çalışanlar
├── Para sıkıntısı
├── Basit kararlar
├── Oyuncu mekaniği öğrenir
└── İlk combo keşfi (tur 4-5)

AŞAMA 2: BÜYÜME (Tur 6-10)
├── 2-3 işletme aktif
├── Combo'lar güçleniyor
├── Rakip agresifleşiyor
├── Eventler stratejiyi değiştiriyor
└── Bölge kavgası kızışıyor

AŞAMA 3: FİNAL (Tur 11-15)
├── Ya domine et ya hayatta kal
├── Büyük combo'lar / riskli hamleler
├── Krizler ve bozulmalar
├── 6. bölgeye ulaşırsan run biter (erken bitiş bonusu)
└── 15. tur sonunda 6 bölge yoksa: skor hesaplanır
```

## 10.2 Dinamik Bitiş

Run 15 turda bitmek ZORUNDA DEĞİL:
- 6 bölge aldığın anda → "MARKET HAKİMİYETİ!" → Run biter + bonus
- Tur 8'de 6 bölge aldıysan = "Speed Run" başarımı
- 15 tur sonunda 6 bölge yoksa = skor hesaplanır, "yeterli değil"

## 10.3 Skor

| Metrik | Puan |
|---|---|
| Final bölge sayısı | × 500 |
| Toplam kazanılan para | × 1 |
| Aktif combo sayısı | × 200 |
| Aktif işletme sayısı | × 100 |
| Erken bitiş bonusu | Kalan tur × 300 |
| FBI'dan kaçma | Her kaçış × 50 |
| Kazandıysan | +1000 |

---

# BÖLÜM 11: META PROGRESSION

Run'lar arası kalıcı ilerleme.

```
Her run → XP kazanırsın (skor bazlı)

XP → Yeni kartlar açılır:
  50 XP  → Uncommon kartlar dükkan havuzuna girer
  200 XP → Rare kartlar
  500 XP → Yeni rakip açılır (Shadow Inc.)
  1000 XP → Epic kartlar
  2000 XP → Yeni rakip (The Cartel)
  5000 XP → Legendary kartlar

Ascension sistemi (run kazandıkça):
  Ascension 1 → Rakip daha agresif
  Ascension 2 → Başlangıç parası 💰400 (💰500 yerine)
  Ascension 3 → Kriz eventleri daha sık
```

---

# BÖLÜM 12: UNITY SAHNE TASARIMI

## 12.1 Sahne Listesi

```
Scenes/
├── Boot.unity          ← Splash screen, yükleme
├── MainMenu.unity      ← Ana menü
├── Game.unity          ← Ana oyun sahnesi (TEK SAHNE)
└── Score.unity         ← Run sonu skor ekranı (overlay olarak da olabilir)
```

## 12.2 Game.unity — Hierarchy

```
Game (Scene Root)
│
├── ─── MANAGERS ───
│   ├── GameManager          [GameManager.cs]
│   │                         Tur yönetimi, state machine, oyun akışı
│   ├── TurnManager          [TurnManager.cs]
│   │                         5 faz döngüsü, faz geçişleri
│   ├── EconomyManager       [EconomyManager.cs]
│   │                         Para, gelir, gider, vergi hesaplama
│   ├── ComboSystem          [ComboSystem.cs]
│   │                         Aktif kartları tara, combo tetikle
│   ├── EventSystem          [EventSystem.cs]
│   │                         Event destesi, çekme, uygulama
│   ├── RivalAI              [RivalAI.cs]
│   │                         Karar ağacı, hamle, büyüme
│   ├── DeckManager          [DeckManager.cs]
│   │                         Deste, çekme, atma, karıştırma
│   ├── ShopManager          [ShopManager.cs]
│   │                         Dükkan kartları, satın alma
│   ├── SaveManager          [SaveManager.cs]
│   │                         JSON save/load, meta progression
│   └── AudioManager         [AudioManager.cs]
│                              Müzik, SFX, volume
│
├── ─── KAMERA ───
│   └── Main Camera          [CameraController.cs]
│                              Ortographic, screen shake
│
├── ─── MASA (BOARD) ───
│   ├── Background           [SpriteRenderer]
│   │                         Ahşap/kumaş masa dokusu
│   │
│   ├── TerritoryMap         [TerritoryMap.cs]
│   │   ├── Slot_01          [TerritorySlot.cs] — her biri bir bölge
│   │   ├── Slot_02
│   │   ├── ...
│   │   └── Slot_10
│   │   │                     10 bölge kutusu, renk değişimi (mavi/kırmızı/gri)
│   │
│   ├── EventArea            [EventDisplay.cs]
│   │   └── EventCard        Aktif event kartı gösterimi
│   │
│   ├── PlayerArea           [PlayerArea.cs]
│   │   ├── BusinessSlot_01  [BusinessSlot.cs]
│   │   │   ├── BusinessCard [CardUI.cs] — işletme kartı görseli
│   │   │   ├── EmpSlot_01   [EmployeeSlot.cs] — çalışan yuvası
│   │   │   └── EmpSlot_02
│   │   ├── BusinessSlot_02
│   │   │   ├── BusinessCard
│   │   │   ├── EmpSlot_01
│   │   │   └── EmpSlot_02
│   │   ├── BusinessSlot_03
│   │   ├── BusinessSlot_04  (başta gizli, upgrade ile açılır)
│   │   └── BusinessSlot_05  (başta gizli)
│   │
│   ├── RivalArea            [RivalArea.cs]
│   │   ├── RivalSlot_01     [RivalDisplay.cs]
│   │   ├── RivalSlot_02
│   │   └── RivalSlot_03
│   │   │                     Rakip işletme + çalışan gösterimi (read-only)
│   │
│   ├── UpgradeArea          [UpgradeArea.cs]
│   │   └── UpgradeSlot_01-04 Aktif upgrade kartları
│   │
│   └── DiscardPile          [DiscardPile.cs]
│       └── SellZone         Kart satma/yakma alanı
│
├── ─── EL (HAND) ───
│   └── HandArea             [HandManager.cs]
│       ├── CardSlot_01      [CardUI.cs + CardDrag.cs]
│       ├── CardSlot_02      DOTween ile fan düzeni
│       ├── CardSlot_03      Hover: kart büyür, detay gösterir
│       ├── CardSlot_04      Drag: kart sürüklenir, valid slot'lar yanar
│       └── CardSlot_05      Drop: kart snap olur + efekt patlar
│
├── ─── UI (Canvas) ───
│   └── GameCanvas           [Canvas — Screen Space Overlay]
│       ├── TopBar            [TopBarUI.cs]
│       │   ├── MoneyDisplay  💰 sayacı (TextMeshPro + DOTween counter)
│       │   ├── TurnCounter   "Tur 8/15"
│       │   └── FBIRiskBar    Tehlike barı (yeşil→kırmızı)
│       │
│       ├── ActionBar         [ActionBarUI.cs]
│       │   ├── ActionDot_1   ● (dolu/boş)
│       │   ├── ActionDot_2   ●
│       │   ├── ActionDot_3   ●
│       │   └── ActionDot_4   (upgrade ile açılır)
│       │
│       ├── EndTurnButton     [Button — "TUR BİTİR"]
│       ├── ShopButton        [Button — "DÜKKAN"]
│       ├── DeckButton        [Button — "DESTE: 22"]
│       │
│       ├── ShopPanel         [ShopPanel.cs — overlay, başta gizli]
│       │   ├── ShopCard_1
│       │   ├── ShopCard_2
│       │   ├── ShopCard_3
│       │   └── CloseButton
│       │
│       ├── ComboPopup        [ComboPopup.cs — combo text animasyonu]
│       ├── EventPopup        [EventPopup.cs — event kart açılış animasyonu]
│       ├── RivalPopup        [RivalPopup.cs — rakip hamle gösterimi]
│       │
│       └── ScoreOverlay      [ScoreScreen.cs — run sonu, başta gizli]
│           ├── ScoreBreakdown
│           ├── FinalGrade
│           └── PlayAgainButton
│
└── ─── EFEKTLER ───
    └── VFXPool              [EffectPool.cs — object pooling]
        ├── CoinRainPrefab   [ParticleSystem] — para yağmuru
        ├── ComboGlowPrefab  [SpriteRenderer + DOTween] — kart parıltısı
        ├── TokenMovePrefab  [SpriteRenderer + DOTween] — müşteri token hareketi
        └── ScreenFlashPrefab [Image + DOTween] — kırmızı/altın flash
```

## 12.3 ScriptableObject Yapısı

### CardData.cs
```csharp
[CreateAssetMenu(fileName = "NewCard", menuName = "EoC/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Temel")]
    public string cardId;           // "B02_kahveci"
    public string cardName;         // "Kahveci"
    public CardType type;           // Business, Employee, Action, Upgrade, Event
    public Rarity rarity;           // Common, Uncommon, Rare
    public int buyCost;             // Dükkan fiyatı
    public string description;
    public Sprite icon;
    public Sprite cardFrame;        // Tip rengine göre

    [Header("İşletme")]
    public int incomePerTurn;
    public int customersPerTurn;
    public int employeeSlots;
    public string[] tags;           // "food", "coffee", "trendy"

    [Header("Çalışan")]
    public int salaryPerTurn;
    public string passiveEffect;
    public string activeAbilityName;
    public string activeAbilityDesc;
    public int fbiRiskPerTurn;      // 0 = legal

    [Header("Action")]
    public string actionEffect;
    public int actionCost;

    [Header("Upgrade")]
    public string upgradeEffect;
    public bool isGlobal;           // true = tüm masayı etkiler

    [Header("Event")]
    public int duration;            // Kaç tur aktif
    public string eventEffect;
}
```

### ComboData.cs
```csharp
[CreateAssetMenu(fileName = "NewCombo", menuName = "EoC/Combo Data")]
public class ComboData : ScriptableObject
{
    public string comboId;
    public string comboName;        // "Latte Sanatı"
    public string[] requiredTags;   // {"coffee", "barista_active"}
    public string[] requiredCardIds;// Spesifik kart gerekiyorsa
    public bool requiresEvent;      // Event bağımlı mı
    public string requiredEventId;

    [Header("Bonus")]
    public int bonusIncome;
    public int bonusCustomers;
    public float incomeMultiplier;  // 1.0 = normal, 2.0 = x2

    [Header("Feedback")]
    public string displayText;      // "LATTE SANATI!"
    public Color glowColor;
    public AudioClip comboSound;
}
```

## 12.4 State Machine (Tur Akışı)

```csharp
public enum TurnPhase
{
    EVENT_PHASE,      // Event çek ve göster
    DRAW_PHASE,       // 5 kart çek
    PLAY_PHASE,       // Oyuncu kart oynar (3 aksiyon)
    RESOLVE_PHASE,    // Sistem hesaplar, animasyonlar oynar
    RIVAL_PHASE,      // Rakip AI hamle yapar
    TURN_END          // Tur sonu kontrolleri, sonraki tura geçiş
}

// TurnManager her phase'i sırayla çalıştırır.
// Her phase bir Coroutine veya async Task.
// Phase geçişlerinde kısa bekleme (animasyon süresi).
```

## 12.5 Kart Sürükle-Bırak (CardDrag.cs)

```
OYUNCU KARTI SÜRÜKLEMEYE BAŞLADIĞINDA:
  1. Kart büyür (DOTween scale 1.0 → 1.2)
  2. Geçerli hedefler YANAR (yeşil glow):
     - İşletme kartıysa → boş business slot'lar yanar
     - Çalışan kartıysa → uygun işletmelerin çalışan yuvaları yanar
     - Upgrade kartıysa → işletmeler + genel alan yanar
     - Action kartıysa → masanın ortası yanar

OYUNCU KARTI GEÇERLİ HEDEFE BIRAKTIĞINDA:
  1. Kart hedefe SNAP olur (DOTween pozisyon, 0.2s)
  2. "thud" sesi çalar
  3. Aksiyon sayacı 1 azalır (●●● → ●●○)
  4. ANLIK feedback:
     - İşletme: müşteri token'ları çıkar
     - Çalışan: işletme kısa parlama efekti
     - Combo tetiklendiyse: COMBO efekti patlar
  5. Para güncellenir (maliyet varsa)

OYUNCU KARTI GEÇERSİZ YERE BIRAKTIĞINDA:
  1. Kart eline geri döner (DOTween, 0.3s)
  2. "buzz" sesi
```

---

# BÖLÜM 13: GÖRSEL & SES

## 13.1 Stil

**Flat 2D + depth.** Satirik business karikatür.

- Kartlar temiz, okunabilir, renkli kenarlar
- Masa ahşap doku, sıcak tonlar
- Token'lar fiziksel jeton hissi (hafif gölge)
- Efektler parlak, abartılı (juice > gerçekçilik)

## 13.2 Minimum Ses Listesi (Prototip)

| Ses | Kullanım |
|---|---|
| card_place | Kart konunca |
| card_draw | Kart çekince |
| coin_ching | Para kazanınca |
| coin_cascade | Büyük para (combo) |
| combo_trigger | Combo patlarken |
| negative_buzz | Kötü olay |
| button_click | UI tıklama |
| turn_bell | Yeni tur |

## 13.3 Müzik (2 parça, crossfade)

1. **Sakin** — Erken turlar, dükkan, menü. Lofi jazz.
2. **Yoğun** — Geç turlar, rakip baskısı, kriz. Tempolu, bass.

---

# BÖLÜM 14: GELİŞTİRME PLANI

## Revize Takvim (32 Hafta)

```
HAFTA 1-4: PROTOTİP
  ✦ Kart sürükle-bırak (Unity)
  ✦ 3 slot, 5 test kartı
  ✦ Basit ekonomi (para artsın)
  ✦ 1 tur döngüsü çalışsın
  ✦ Bölge haritası (10 kutu)
  ✦ Juice: screen shake + coin rain + combo text
  → TEST: "Kart koymak eğlenceli mi?"

HAFTA 5-10: CORE LOOP
  ✦ 5 faz tur sistemi
  ✦ 20 kart (yarısı)
  ✦ 5 combo + feedback
  ✦ 1 rakip AI
  ✦ Event sistemi
  ✦ Dükkan
  ✦ 5 harici playtester bul
  → TEST: "Bir run eğlenceli mi?"

HAFTA 11-16: MVP
  ✦ 40 kart tamamı
  ✦ 10 combo
  ✦ İşletme evrim sistemi
  ✦ Çalışan aktif yetenek
  ✦ Bozulma mekaniği (kapanma, ayrılma, FBI)
  ✦ Ses efektleri (8 SFX)
  ✦ 2 müzik parçası
  ✦ Skor ekranı
  ✦ STEAM SAYFASI AÇILIR (hafta 14)
  → TEST: Demo build

HAFTA 17-22: POLISH + DEMO
  ✦ Balance pass (playtest verileriyle)
  ✦ Next Fest demo
  ✦ Trailer
  ✦ Streamer outreach
  ✦ Meta progression (unlock sistemi)
  ✦ 3 Ascension seviyesi

HAFTA 23-28: EARLY ACCESS
  ✦ EA lansmanı (40 kart)
  ✦ Topluluk geri bildirimi
  ✦ Balance düzeltmeleri
  ✦ Bug fix

HAFTA 29-32: İLK GÜNCELLEME
  ✦ +20 kart
  ✦ +5 combo
  ✦ 2. rakip tipi
```

---

# BÖLÜM 15: KURALLAR ÖZETİ (Tek Sayfa)

Tüm oyunu tek sayfada özetleyen referans:

```
═══════════════════════════════════════════
         EMPIRE OF CARDS — KURALLAR
═══════════════════════════════════════════

AMAÇ: 10 bölgeden 6'sını ele geçir.

BAŞLANGIÇ: 💰500, 14 kart deste, 3 slot.

HER TUR (5 adım):
  1. EVENT    → Her 3 turda 1 event açılır
  2. ÇEK      → 5 kart çek, 1 redraw
  3. OYNA     → 3 aksiyon: kart koy veya yetenek kullan
  4. SİSTEM   → Gelir, müşteri, combo, FBI kontrolü
  5. RAKİP    → AI 2 hamle yapar

KART TİPLERİ:
  Mavi   = İşletme  → Slota koy, kalıcı, gelir üretir
  Yeşil  = Çalışan  → İşletmeye koy, passif + aktif yetenek
  Kırmızı = Action  → Tek kullanım, güçlü etki
  Mor    = Upgrade  → Kalıcı iyileştirme
  Sarı   = Event    → Otomatik, dünyayı değiştirir

PARA:
  + İşletme gelirleri
  + Combo bonusları
  - Çalışan maaşları (otomatik)
  - Vergi %15 (muhasebeci: %7.5)
  Para = 0 → İFLAS

BÖLGE (market share):
  Müşterilerin çok → bölge çok
  6 bölge = KAZANDIN
  Rakip 7 bölge = KAYBETTİN

COMBO:
  2-3 kart aynı anda aktif → otomatik tetiklenir
  Bonus gelir/müşteri + görsel efekt

FBI:
  İllegal kart = risk artar
  Her tur sonu zar atılır
  Yakalanırsan: 💰300 ceza + çalışan kovulur

DÜKKAN:
  Her tur 3 kart satılır
  Para ile satın al → destene girer

═══════════════════════════════════════════
```

---

> Bu doküman v2.0'dır. Tüm tartışma, premortem, ve tasarım revizyonlarını içerir.
> Engine: Unity (C#) | Son güncelleme: 2026-05-18
