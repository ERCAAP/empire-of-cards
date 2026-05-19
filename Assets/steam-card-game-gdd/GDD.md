# GAME DESIGN DOCUMENT
# "Empire of Cards"

> Versiyon: 2.2 | Tarih: 2026-05-20
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

# BÖLÜM 1.5: İLK GİRİŞİM SEÇİMİ (First Venture)

## 1.5.1 Konsept

Oyun başladığında oyuncuya "İlk girişimini seç" ekranı gösterilir. Bu bir class/archetype kilidi **DEĞİLDİR.** Sadece başlangıç yönü verir: 1 işletme tahtaya yerleşir + destene 1 bonus kart eklenir. Run'ın geri kalanında oyuncu istediği stratejiyi izleyebilir.

**Neden bu sistem?** Oyuncu boş masaya bakınca "nereden başlayacağım?" hissi yaşıyor. İlk Girişim bu sorunu çözer — ilk turdan itibaren masada bir şey var, elde bir yön var.

## 1.5.2 Girişim Seçenekleri (4 adet)

### 🍔 1. BÜFE (Diner)

Büfe tahtaya yerleşik gelir. Destene +1 Şef eklenir. Food combo'lara doğru yönlendirir ama kilitlemez.

| Özellik | Değer |
|---|---|
| Board | B01 Büfe otomatik Slot 1'e yerleşir |
| Deste | Standart 14 kart + 1 Şef (C04) = 15 kart |
| Başlangıç Parası | 💰500 |
| Yönlendirme | Food combo'lar (Organik Sinerji, Fast Food İmparatorluğu) |

### 💻 2. TECH STARTUP

Tech Startup tahtaya yerleşik gelir (delay sayacı 1'den başlar = 2 tur bekleme). Destene +1 Hacker eklenir.

| Özellik | Değer |
|---|---|
| Board | B04 Tech Startup otomatik Slot 1'e yerleşir |
| Deste | Standart 14 kart + 1 Hacker (C07) = 15 kart |
| Başlangıç Parası | 💰500 |
| Yönlendirme | Tech combo'lar (AI Devrimi), agresif oyun |

### 📢 3. REKLAM AJANSI

Reklam Ajansı tahtaya yerleşik gelir. Destene +1 Marketing Gurusu eklenir.

| Özellik | Değer |
|---|---|
| Board | B08 Reklam Ajansı otomatik Slot 1'e yerleşir |
| Deste | Standart 14 kart + 1 Marketing Gurusu (C05) = 15 kart |
| Başlangıç Parası | 💰500 |
| Yönlendirme | Marketing combo'lar (Reklam Bombardımanı, Viral Fırtına) |

### 🕶️ 4. KARANLIK PAZAR

Hiçbir business yerleşmez. Ama +200 ekstra para ve destene +1 Dolandırıcı eklenir. Riskli ama hızlı para.

| Özellik | Değer |
|---|---|
| Board | Boş (oyuncu kendi seçer) |
| Deste | Standart 14 kart + 1 Dolandırıcı (C09) = 15 kart |
| Başlangıç Parası | 💰700 |
| Yönlendirme | İllegal strateji, hızlı para, yüksek risk |

## 1.5.3 Tasarım Kuralları

- **Pasif bonus yok.** Girişim seçimi sadece başlangıç kartları verir, run boyunca pasif etki uygulamaz.
- **Class kilidi yok.** Büfe seçen oyuncu Tech Startup da açabilir, Hacker da alabilir.
- **Shop yanliligi (ilk 5 tur):** Ilk 5 turda 3 dukkan kartindan en az 1'i oyuncunun girisim sektorune uygun tag tasir (bkz. Bolum 1.7.3). Tur 6'dan itibaren tam 40 kart havuzundan saf rastgele.
- **Seçim sadece başlangıç yönü verir, run'ı kilitlemez.** Oyuncu istediği anda farklı stratejiye geçebilir.
- **Tutorial run'ında Büfe otomatik seçilir.** Yeni oyuncu seçim ekranını görmez, doğrudan Büfe ile başlar. İlk run tamamlandıktan sonra seçim ekranı açılır.

---

# BÖLÜM 1.6: ŞİRKET SEVİYESİ (Company Tier)

## 1.6.1 Konsept

Oyuncunun board state'ine göre şirket seviyesi otomatik belirlenir. Yeni kart tipi gerektirmez. Sadece board state kontrolü + görsel/ses feedback sistemi. Oyuncuya "büyüyorum" hissini somutlaştırır — soyut bir skor yerine, şirketinin ismi ve görünümü değişir.

## 1.6.2 Tier Tablosu (4 Seviye)

| Tier | İsim | Koşul | Görsel Feedback |
|------|------|-------|-----------------|
| 1 | ESNAF | Oyun başı (varsayılan) | Küçük logo, sade board |
| 2 | GİRİŞİMCİ | 2+ aktif business + 1+ combo tetiklenmiş | "TEBRİKLER: GİRİŞİMCİ oldun!" popup, logo büyür |
| 3 | ŞİRKET | 3 aktif business + 2+ combo + 4+ territory | Board rengi değişir, rival ciddi olmaya başlar, müzik yoğunlaşır |
| 4 | HOLDİNG | 3 aktif business + 3+ combo + 5+ territory | Taç efekti, dominant his, board kenarları altın yanar |

## 1.6.3 Tier Kuralları

- **Kontrol zamanı:** Tier her tur sonunda Resolve Phase'de kontrol edilir (combo check'ten sonra, gelir hesaplamasından önce).
- **Tier asla düşmez.** Bir kere Girişimci oldun mu geri Esnaf olamazsın. İşletme kapansa bile tier korunur.
- **Tier atlama anında:** Popup + ses efekti + kısa animasyon oynar. Oyun 1-2 saniye durur, oyuncu anı yaşar.
- **Skor bonusu:** Tier, run sonu skor hesaplamasına bonus verir:

| Tier | Skor Bonusu |
|------|-------------|
| Tier 1 — ESNAF | +0 |
| Tier 2 — GİRİŞİMCİ | +200 |
| Tier 3 — ŞİRKET | +500 |
| Tier 4 — HOLDİNG | +1000 |

## 1.6.4 Unity Entegrasyonu

```
Resolve Phase sırası (güncellenmiş):
  Adım 4a: İşletmeler üretir
  Adım 4b: Müşteriler kayar
  Adım 4c: Combo kontrolü
  Adım 4c.5: TIER KONTROLÜ ← YENİ
  Adım 4d: Gelir hesaplanır
  Adım 4e: Bozulma kontrolü
```

Tier kontrolü için `CompanyTierManager.cs` gerekir. Bu manager her tur sonunda board state'i okur (aktif business sayısı, tetiklenmiş combo sayısı, territory sayısı) ve mevcut tier'ı güncellemesi gerekip gerekmediğini kontrol eder.

---

# BÖLÜM 1.7: RAKİP AYNA SİSTEMİ (Rival Mirror)

## 1.7.1 Konsept

Oyun basinda oyuncunun sectigi girisim tipine gore rakip de **ayni turde** is kurarak baslar. Bu "ayna" etkisi, oyuncunun ilk turlardan itibaren dogrudan bir rakiple karsilasmasini saglar -- ayni sektorde iki rakip firmanin cekismesi hissi yaratir.

**Neden bu sistem?** Oyuncu Bufe secip rakip Tech Startup ile baslayinca sektorel cekisme hissi zayifliyor. Ayni sektorden baslamak "bu mahallede iki bufe mi olur?" gerilimini ilk turdan verir.

## 1.7.2 Ayna Tablosu

| Oyuncu Secimi | Rakip Baslangic Isleri | Rakip Gelir | Rakip Musteri | Not |
|---|---|---|---|---|
| Bufe | Rival Bufe | 50/tur | 3 | Birebir ayni statlar |
| Tech Startup | Rival Tech Startup | 0 (ilk 3 tur), sonra 150 | 0 (ilk 3 tur), sonra 4 | Ayni delay mekanigi |
| Reklam Ajansi | Rival Reklam Ajansi | 60/tur | 3 + tum isletmelere +2 | Ayni destek etkisi |
| Karanlik Pazar | MegaCorp HQ (varsayilan) | 80/tur | 5 | Oyuncunun isletmesi yok, rakip varsayilana doner |

## 1.7.3 Shop Yanliligi (Shop Bias)

Ilk 5 tur boyunca dukkan, oyuncunun girisim sektorune uygun en az 1 kart gosterir:

| Girisim | Zorunlu Tag (3 karttan 1'i) |
|---|---|
| Bufe | `food` |
| Tech Startup | `tech` |
| Reklam Ajansi | `marketing` |
| Karanlik Pazar | `illegal` |

Tur 6'dan itibaren dukkan tamamen rastgeledir (yanlilik kalkar).

## 1.7.4 Dinamik Oyun Suresi

Sabit tur limiti kaldirildi. Oyun organik olarak biter:

| Kosul | Sonuc |
|---|---|
| Oyuncu 6 bolge alirsa | **KAZANDIN** -- run biter |
| Rakip 7 bolge alirsa | **KAYBETTIN** -- rakip domine etti |
| Tur 25 sonrasi | Yumusak cap: her iki taraf da tur basina -%5 gelir cezasi alir (baskiya zorlar) |
| Tur 30 | Sert cap: kim daha cok bolgeye sahipse kazanir (esitlikte rakip kazanir) |

**Neden?** Sabit 20 tur bazen cok kisa (epik build'ler tamamlanamiyordu), bazen cok uzun (erken domination sonrasi bos turlar). Dinamik sistem hizli oyunlara (tur 12) ve epik maratonlara (tur 28) izin verir. Yumusak cap oyunu bitmeye zorlar, sert cap garantili bir son verir.

## 1.7.5 Isletme Bakimi (Business Maintenance)

Isletmeler surekli ilgi ister. Cok uzun sure ihmal edilen isletmeler verim kaybeder.

**Ihmal Sayaci:** Her isletme icin ayri tutulan bir sayac. Eger bir turda o isletmeye calisan eklenmez VEYA upgrade yapilmazsa sayac +1 artar. Herhangi bir calisan veya upgrade eklendiginde sayac sifirlanir.

| Ihmal Turu | Ardisik Tur | Etki | Gorsel |
|---|---|---|---|
| Hafif Ihmal | 4 tur | Gelir -%20 | Isletme karti hafif kararir |
| Agir Ihmal | 6 tur | Gelir -%40 | Isletme karti belirgin sekilde kararir, uyari ikonu |

**Tasarim Amaci:** Oyuncuyu "kur ve unut" stratejisinden uzaklastirmak. Isletmelere duzenli yatirim yapmak onemli. Bu ayni zamanda calisan ve upgrade kartlarina ek deger katiyor -- sadece stat icin degil, ihmal sayacini sifirlamak icin de kullanilirlar.

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
│  💰 620    ⚖️ FBI: %12    🔄 Tur 8                   │
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
| 6+ bolge senin | **KAZANDIN** -- Run biter |
| 7+ bolge rakibin | **KAYBETTIN** -- Rakip domine etti |
| Paran 0'a duserse | **IFLAS** -- Run biter |
| Tur 25+ | Yumusak cap: -%5 gelir cezasi/tur (iki tarafa) |
| Tur 30, 6 bolge yok | Sert cap: en cok bolgeye sahip olan kazanir |

**Dinamik bitis:** %60'a (6 bolge) ulastigin AN run biter. Sabit tur limiti yok. Erken bitirirsen bonus skor. Tur 25'ten sonra baski artar, tur 30'da oyun kesin biter.

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
| 1 | 1 isletme (girisim aynasi -- oyuncuyla ayni tip), 1 calisan. Bolge: 2 |
| 5 | 2 isletme, 2-3 calisan. Bolge: 3 |
| 8 | 2-3 isletme, 3-4 calisan. Bolge: 3-4 |
| 12 | 3 isletme, 4-5 calisan, agresif. Bolge: 4-5 |
| 15 | 3-4 isletme, 5-6 calisan. Bolge: 4-6 |
| 20 | 4 isletme, 6-7 calisan. Bolge: 5-6 |
| 25+ | Yumusak cap etkili -- rakip de -%5 gelir/tur. Bolge: 5-7 |

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

| Tur | Beklenen Para | Beklenen Bolge | His |
|---|---|---|---|
| 1-5 | 400-600 | 1-2 | Para siki, her kurus onemli. Shop yanliligi yardimci |
| 6-12 | 600-1200 | 2-4 | Ilk combo, buyume, ihmal sayacina dikkat |
| 13-20 | 1000-2500 | 4-6 | Motor calisiyor, rakip baskisi, bolge kavgasi |
| 21-24 | 2000-3500 | 5-6 | Final kavgasi, kriz riski, cogu oyun burada biter |
| 25-30 | Azalan (-%5/tur) | 5-7 | Yumusak cap baskisi, her iki taraf zayifliyor |

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

## 10.1 Dinamik Tur Yapisi, 4 Asama

```
ASAMA 1: KURULUS (Tur 1-5)
+-- Ilk isletme + calisanlar
+-- Para sikintisi
+-- Basit kararlar
+-- Oyuncu mekanigi ogrenir
+-- Ilk combo kesfi (tur 4-5)
+-- Shop yanliligi aktif (girisim sektorune uygun kart)

ASAMA 2: BUYUME (Tur 6-15)
+-- 2-3 isletme aktif
+-- Combo'lar gucleniyor
+-- Rakip agresiflesiyor
+-- Eventler stratejiyi degistiriyor
+-- Bolge kavgasi kizisiyor
+-- Ihmal sayaclarina dikkat (4+ tur = gelir dususu)

ASAMA 3: GECIS (Tur 16-24)
+-- Ya domine et ya hayatta kal
+-- Buyuk combo'lar / riskli hamleler
+-- Krizler ve bozulmalar
+-- 6. bolgeye ulasirsan run biter (erken bitis bonusu)

ASAMA 4: BASKI (Tur 25-30)
+-- Yumusak cap: Her tur -%5 gelir cezasi (iki tarafa da)
+-- Oyun bitise zorlanir
+-- Tur 30 = SERT CAP: En cok bolgeye sahip olan kazanir
```

## 10.2 Dinamik Bitis

Run sabit turda bitmez. Organik olarak sona erer:
- 6 bolge aldigin anda -> "MARKET HAKIMIYETI!" -> Run biter + bonus
- Tur 8'de 6 bolge aldiysan = "Speed Run" basarimi
- Tur 25 sonrasi: yumusak cap -- her iki taraf tur basina -%5 gelir cezasi alir
- Tur 30: sert cap -- en cok bolgeye sahip olan kazanir (esitlikte rakip kazanir)

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

BOLGE (market share):
  Musterilerin cok -> bolge cok
  6 bolge = KAZANDIN
  Rakip 7 bolge = KAYBETTIN
  Tur 25+: -%5 gelir/tur (yumusak cap)
  Tur 30: en cok bolgeye sahip olan kazanir (sert cap)

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

> Bu dokuman v2.2'dir. Tum tartisma, premortem, ve tasarim revizyonlarini icerir.
> v2.1 eklemeleri: Rakip Ayna Sistemi, Shop Yanliligi, Isletme Bakimi, Dinamik Oyun Suresi
> v2.2 eklemeleri: Appendix A -- Sektor Deneyim Senaryolari, Etik Ikilem Sistemi, Sonuc Zinciri
> Engine: Unity (C#) | Son guncelleme: 2026-05-20

---

# APPENDIX A: SEKTOR DENEYIM SENARYOLARI (Business Experience Scenarios)

> Bu appendix, her is tipi icin GERCEKCI hikaye akislari, etik ikilemler, kestirme yollar ve sonuclari tasarlar. Oyuncu sadece kart koymaz -- gercek is kararlari verir.

---

## A.1 TASARIM FELSEFESI

Empire of Cards bir "is kurma simulasyonu" degil, bir **is DENEYIMI simulasyonu**dur. Oyuncu sunu hissetmeli:

1. **Kestirme yollar CAZIP gorunmeli** -- anlik kazanc buyuk, risk belirsiz
2. **Risk GECKMELI olmali** -- FBI belki gelir... belki gelmez
3. **Kucuk adimlar buyuk sorunlara donusmeli** -- "bir kerelik" psikolojisi
4. **Durust yol YAVAS ama GUVENLI olmali** -- sabir oduller
5. **Geri donus MUMKUN ama PAHALI olmali** -- iflas = oyun sonu degil

---

## A.2 ETIK IKILEM SISTEMI (Ethical Dilemma System)

### A.2.1 Temel Mekanik

Her riskli action/upgrade karti oynandiginda, ekranda **Ikilem Popup** cikar:

```
  +================================+
  |   KARAR ZAMANI                 |
  |                                |
  |   "Sahte Yorumlar" oynamak    |
  |   istiyorsun.                  |
  |                                |
  |   KAZANC: +8 musteri HEMEN    |
  |   RISK: FBI +%12              |
  |   (Mevcut FBI: %8 -> %20)     |
  |                                |
  |   [ONAYLA]     [VAZGEC]       |
  +================================+
```

**Tasarim Kurali:** Kazanc SOMUT ve ANLIK gosterilir ("+8 musteri HEMEN"). Risk SOYUT ve BELIRSIZ gosterilir ("FBI +%12" -- bu ne demek? Yakalanir miyim?). Bu asimetri gercek is hayatindaki yolsuzluk psikolojisini yansitir.

### A.2.2 "Sadece Bir Kerelik" Mekaniği (Escalation Tracker)

Oyun her illegal/risky aksiyonu sessizce sayar. Artan sayi sunu tetikler:

| Illegal Aksiyon Sayisi | Etki |
|---|---|
| 1-2 | Normal FBI risk artisi |
| 3-4 | FBI risk artisi x1.5 (suc profili olusuyor) |
| 5-6 | FBI risk artisi x2.0 + "Supheli Sirket" etiketi (rival taunt degisir) |
| 7+ | FBI risk artisi x2.5 + event havuzuna "Buyuk Juri Sorusturmasi" eklenir |

Oyuncu bunu GORMEZ. Sadece hisseder: "Neden FBI riskim bu kadar hizli artiyor?"

### A.2.3 Cazibe Tasarimi (Temptation Design)

Her riskli kartin "cazip ani" vardir -- ozellikle su durumlarda popup icinde EKSTRA motivasyon gosterilir:

| Durum | Ekstra Cazibe |
|---|---|
| Paran < 200 | "Iflasin 1 tur uzaginda. Bu seni kurtarabilir." |
| Rakip 5+ bolge | "Rakip domine ediyor. Normal yoldan yetisemezsin." |
| Son 5 tur (25+) | "Son sanslar... kural kitabini yak." |
| Combo icin 1 kart eksik | "Bu karti oynarsan COMBO tetiklenir!" |

---

## A.3 SONUC ZINCIRI (Consequence Chain)

5 katli sonuc sistemi. Her kat oncekinden agir ama GERI DONUS mekanigi var.

### Seviye 1: UYARI (Warning)

| Ozellik | Deger |
|---|---|
| Tetik | FBI riski %15-25 arasinda yakalanma |
| Ceza | 150 para cezasi |
| Gorsel | Sari flash, uyari sesi, "UYARI: Supheli faaliyet tespit edildi" |
| Geri Donus | Otomatik -- cezayi ode, devam et |

### Seviye 2: GECICI KAPANMA (Temporary Shutdown)

| Ozellik | Deger |
|---|---|
| Tetik | FBI riski %25-40 arasinda yakalanma VEYA 2. kez Level 1 |
| Ceza | 300 para + 1 isletme 2 tur kapali + illegal calisan kovulur |
| Gorsel | Kirmizi flash, siren, isletme kartina "KAPALI" damgasi |
| Geri Donus | 2 tur bekle VEYA "Avukat" calisanini kullan (aninda ac) |

### Seviye 3: SKANDAL (Major Scandal)

| Ozellik | Deger |
|---|---|
| Tetik | FBI riski %40-60 arasinda VEYA sektor-ozel skandal event'i |
| Ceza | 600 para + TUM isletmeler 1 tur gelir -%50 + 10 musteri kaybi |
| Gorsel | Ekran sallanir, gazete manset efekti: "SKANDAL: [Sirket Adi] Yolsuzluk!" |
| Geri Donus | "Halka Ozur" action karti (200 para, 3 musteri geri kazanir) VEYA "PR Krizi Yonetimi" upgrade (500 para, skandal etkisini yarilatiyor) |

### Seviye 4: FBI BASKINI (FBI Raid)

| Ozellik | Deger |
|---|---|
| Tetik | FBI riski %60+ VEYA 3. kez yakalanma |
| Ceza | 1000 para + TUM illegal calisanlar kovulur + 1 tur TUM isletmeler durur + FBI riski sifirlanir |
| Gorsel | Tam ekran kirmizi flash, siren sesi, FBI rozet animasyonu, "FBI BASKINI!" text |
| Geri Donus | "Tanik Koruma" action karti (tutar 400 -- FBI riskini sifirlar ve 5 tur boyunca artisini yarilatiyor) |

### Seviye 5: TAM KAPANMA (Complete Shutdown)

| Ozellik | Deger |
|---|---|
| Tetik | FBI riski %80+ VEYA iflas + yakalanma VEYA "Buyuk Juri" event'i |
| Ceza | TUM isletmeler kapanir, para %70 el konulur, TUM calisanlar gider |
| Gorsel | Ekran kararir, zincir sesi, "OYUN BITTI... mi?" text, sonra comeback ekrani |
| Geri Donus | "Yeni Baslangiç" mekanigi: 1 isletme sec (en dusuk tier), 200 para ile yeniden basla. Tier KORUNUR. "Pheonix" skoru icin bonus puan |

---

## A.4 SEKTOR SENARYOLARI

---

### A.4.1 MOBIL UYGULAMA / TECH STARTUP

**Hikaye Akisi:**
```
Tur 1-3:   Basit uygulama yap, kullanici topla (delay mekanigi -- 3 tur bekle)
Tur 4-8:   Buyume -- kullanici sayisi artiyor, gelir basliyor
Tur 9-14:  Karar ani -- durust buyume mi, kestirme mi?
Tur 15+:   Ya saglam SaaS sirket ya da skandala gomulmus startup
```

**DURUST YOL:**

| Kart | Tip | Maliyet | Etki |
|---|---|---|---|
| App Developer | Calisan (C11) | Maas 35/tur | Tech isletmelerde +4 musteri, passif: bug fix (isletme kapanma riski -%30) |
| UX Designer | Calisan (C12) | Maas 40/tur | Tech isletmelerde gelir +%20, aktif: "Kullanici Arastirmasi" -- sonraki turda musteri x1.5 |
| SaaS Donusumu | Upgrade (U07) | 350 | Tech isletme geliri sabit +80/tur olur (rastgelelik kalkar), +2 calisan slotu |
| Bulut Altyapisi | Upgrade (U08) | 300 | Tech isletmelerde musteri cap'i %50 artar |

**Durust Combo:** "Temiz Kod" -- App Developer + UX Designer + Tech Startup(aktif) = Gelir +%40, musteri kaybi yok, isletme ASLA kapanmaz (bug-free)

**KESTIRME YOL:**

| Kart | Tip | Maliyet | Etki | Risk |
|---|---|---|---|---|
| Growth Hacker | Calisan (C13) | Maas 50/tur | +8 musteri/tur (agresif taktikler) | FBI +%6/tur |
| Dark Pattern | Action (A11) | 100 | +12 musteri bu tur (kullanicilari kandirma) | FBI +%10, sonraki tur -4 musteri (churn) |
| Sahte Indirimler | Action (A12) | 60 | +200 para aninda (fake sale metrics) | FBI +%8 |
| Veri Madenciligi | Upgrade (U09) | 200 | +100 para/tur (kullanici verileri sat) | FBI +%5/tur, "Veri Sizintisi" event olasiligini x2 yapar |

**Kestirme Combo:** "Buyume Hack'i" -- Growth Hacker + Dark Pattern = +20 musteri bu tur AMA FBI +%18

**SEKTORE OZEL EVENT'LER:**

| Event | Sure | Etki |
|---|---|---|
| E07: App Store Bani | 2 tur | Tech isletmeler 2 tur gelir SIFIR (uygulama kaldirildi). Growth Hacker varsa TETIKLENIR, yoksa normal havuzda |
| E08: Veri Sizintisi | 1 tur | Tech isletmeler -8 musteri, 300 para ceza. Bulut Altyapisi varsa hasar yariya iner |
| E09: Yatirimci Ilgisi | 1 tur | Tech isletmeler gelir x2 bu tur (funding round!) |

**GERI DONUS KARTLARI:**

| Kart | Tip | Maliyet | Etki |
|---|---|---|---|
| Pivot to SaaS | Action (A13) | 200 | Tech isletmenin TUM illegal bonuslarini kaldir, yerine sabit +60 gelir/tur koy. FBI risk artisi sifirla. |
| Etik AI Manifestosu | Upgrade (U10) | 250 | Tech isletmelerde FBI risk artisi SIFIR olur. Musteri kaybi -%50. |

---

### A.4.2 YEMEK / RESTORAN ZINCIRI

**Hikaye Akisi:**
```
Tur 1-3:   Kucuk bufe, durust malzemeler, az gelir
Tur 4-8:   Zincire genisle, tedarik kararlari
Tur 9-14:  Ucuz malzeme mi, kaliteli mi? Organik mi, sahte organik mi?
Tur 15+:   Ya guvenilir zincir ya da saglik skandaliyla kapali
```

**DURUST YOL:**

| Kart | Tip | Maliyet | Etki |
|---|---|---|---|
| Sef (mevcut C04) | Calisan | Maas 30 | +3 musteri (food: gelir +30), aktif: Ozel Menu |
| Gida Guvenligi Muduru | Calisan (C14) | Maas 35/tur | Food isletmelerde: saglik denetimi event'lerine BAGISIK, +2 musteri (guven) |
| Organik Ciftlik (mevcut B06) | Isletme | 120 | Tum food'lara +20 bonus |
| Farm-to-Table | Upgrade (U11) | 280 | 1 food isletme gelir +%35, musteri +3, "Organik" tag'i kazanir |

**Durust Combo:** "Organik Sinerji" (mevcut #2) + "Gurme Mutfak" -- Sef + Gida Guvenligi Muduru + Farm-to-Table = Food gelir x1.8, musteri kaybi SIFIR, saglik event'lerine tam bagisiklik

**KESTIRME YOL:**

| Kart | Tip | Maliyet | Etki | Risk |
|---|---|---|---|---|
| Ucuz Et Tedarikcisi | Upgrade (U12) | 80 | Food isletme maliyeti -%40, gelir +%15 | FBI +%4/tur, "Saglik Denetimi" event olasiligi x3 |
| Kose Kesme | Action (A14) | 50 | Bu tur food gelir x1.5 (ucuz malzeme, buyuk porsiyon) | FBI +%6, %20 sans: sonraki tur "Gida Zehirlenmesi" event'i |
| Sahte Organik Etiketi | Action (A15) | 100 | Food isletmeye "Organik" tag'i ekle (sahte) -- Organik combo bonuslari AL ama illegal | FBI +%10, yakalanirsa TUM organik bonuslari geri alinir + 400 ceza |
| Son Kullanim Hilesi | Upgrade (U13) | 60 | Food isletme giderleri -%50 (suresi gecmis malzeme) | FBI +%3/tur, her tur %10 sans: musteri zehirlenmesi (-6 musteri, 200 ceza) |

**Kestirme Combo:** "Kitle Uretimi" -- Ucuz Et + Son Kullanim Hilesi + Burger Zinciri = Gelir x2 AMA her tur %25 sans buyuk saglik skandali

**SEKTORE OZEL EVENT'LER:**

| Event | Sure | Etki |
|---|---|---|
| E10: Saglik Denetimi | 1 tur | TUM food isletmeleri denetlenir. Illegal upgrade varsa: isletme 3 tur KAPALI + 400 ceza. Gida Guvenligi Muduru varsa: GECERSIN |
| E11: Gida Zehirlenmesi Salgini | 2 tur | Ucuz Et veya Son Kullanim varsa: -15 musteri, isletme kapali. Yoksa: -3 musteri (genel etki) |
| E12: Gurme Trend'i | 2 tur | Organik tag'li food isletmeler gelir x2, musteri x1.5 |

**GERI DONUS KARTLARI:**

| Kart | Tip | Maliyet | Etki |
|---|---|---|---|
| Halka Ozur Yemegi | Action (A16) | 150 | Skandal sonrasi: 5 musteri geri kazan, food isletme hemen ac |
| Tedarik Zinciri Reformu | Upgrade (U14) | 300 | TUM illegal food upgrade'lerini kaldir. Yerine: food gelir +%20 (temiz). FBI riski -20 puan |

---

### A.4.3 MODA / GIYIM MARKASI

**Hikaye Akisi:**
```
Tur 1-3:   Kucuk butik, yerel tasarimlar
Tur 4-8:   Seri uretime gec, isci maliyeti karari
Tur 9-14:  Ucuz iscilik mi, tasarim hirsizligi mi, vergi kacirma mi?
Tur 15+:   Ya saygin marka ya da boykot edilen skandal markasi
```

**DURUST YOL:**

| Kart | Tip | Maliyet | Etki |
|---|---|---|---|
| Moda Tasarimcisi | Calisan (C15) | Maas 45/tur | Luxury/Fashion isletmelerde +5 musteri, gelir +%15. Trend aktifken x2 |
| Surdurulebilir Uretim | Upgrade (U15) | 350 | Fashion isletme: isci skandali event'lerine BAGISIK, musteri +4 (etik tuketici) |
| Marka Elcisi | Calisan (C16) | Maas 55/tur | TUM isletmelere +3 musteri (marka bilinirli), aktif: "Marka Lansmani" -- bu tur musteri x2 |

**Durust Combo:** "Etik Moda" -- Moda Tasarimcisi + Surdurulebilir Uretim + Luxury Boutique = Gelir x1.6, skandal event'lerine tam bagisiklik, her tur +2 ekstra musteri (sadik kitle)

**KESTIRME YOL:**

| Kart | Tip | Maliyet | Etki | Risk |
|---|---|---|---|---|
| Sweatshop Mudurlugu | Upgrade (U16) | 100 | Fashion/Luxury isletme calisan maaslari -%60, gelir +%20 | FBI +%7/tur, "Isci Skandali" event olasiligi x3 |
| Tasarim Hirsizligi | Action (A17) | 80 | +10 musteri bu tur (sahte tasarimci marka kopyasi) | FBI +%12, %30 sans: "Telif Davasi" -- 500 ceza |
| Vergi Cenneti | Upgrade (U17) | 200 | Vergi SIFIR olur (offshore hesap) | FBI +%5/tur, "Vergi Denetimi" event olasiligi x2 |
| Hizli Moda | Upgrade (U18) | 150 | Fashion isletme gelir +%40, musteri +6 (ucuz trend kopyalari) | Her 3 turda -2 musteri (marka erozyonu), FBI +%3/tur |

**Kestirme Combo:** "Ucuz Imparatorluk" -- Sweatshop + Hizli Moda = Gelir x2.5 AMA FBI +%15/tur ve her 2 turda "Boykot Riski" kontrolu (%20 sans: -8 musteri)

**SEKTORE OZEL EVENT'LER:**

| Event | Sure | Etki |
|---|---|---|
| E13: Isci Skandali | 2 tur | Sweatshop varsa: -12 musteri, isletme 2 tur kapali, sosyal medya firtinasi. Yoksa: -2 musteri |
| E14: Vergi Denetimi | 1 tur | Vergi Cenneti varsa: 800 ceza + upgrade kaldirilir. Yoksa: normal vergi |
| E15: Moda Haftasi | 2 tur | Fashion/Luxury isletmeler gelir x2, musteri x1.5. Trend tag'li isletmeler ekstra +5 musteri |

**GERI DONUS KARTLARI:**

| Kart | Tip | Maliyet | Etki |
|---|---|---|---|
| Marka Yenilenmesi | Action (A18) | 300 | Boykot/skandal sonrasi: TUM moda musteri kaybini sifirla, +5 musteri |
| Adil Ticaret Sertifikasi | Upgrade (U19) | 400 | TUM illegal moda upgrade'lerini kaldir. Fashion gelir +%25, musteri +5 (etik marka imaji) |

---

### A.4.4 KRIPTO / FINTECH

**Hikaye Akisi:**
```
Tur 1-3:   Token lansman veya borsa kur (yuksek volatilite)
Tur 4-8:   Hacim artir, topluluk kur
Tur 9-14:  Pump and dump mi, organik buyume mi?
Tur 15+:   Ya regulasyona uyumlu fintech ya da donmus hesaplar
```

**DURUST YOL:**

| Kart | Tip | Maliyet | Etki |
|---|---|---|---|
| Blockchain Developer | Calisan (C17) | Maas 50/tur | Crypto isletmelerde volatiliteyi azaltir (min gelir +50), +3 musteri |
| Regulasyon Uyumu | Upgrade (U20) | 400 | Crypto isletme: "Regulatory Crackdown" event'ine BAGISIK, gelir sabit +120/tur (rastgelelik kalkar) |
| DeFi Altyapisi | Upgrade (U21) | 350 | Crypto isletmeye +3 calisan slotu, gelir +%30 |

**Durust Combo:** "Guvenilir Borsa" -- Blockchain Developer + Regulasyon Uyumu + Crypto Exchange = Gelir sabit 180/tur (rastgelelik YOK), musteri +6, regulator event'lerine tam bagisik

**KESTIRME YOL:**

| Kart | Tip | Maliyet | Etki | Risk |
|---|---|---|---|---|
| Crypto Bro | Calisan (C18) | Maas 30/tur | +6 musteri (influencer marketing), Crypto gelir +%30 | FBI +%5/tur, "Rug Pull" suphesi |
| Pump and Dump | Action (A19) | 0 | +400 para ANINDA (fiyat manipulasyonu) | FBI +%15, sonraki 2 tur Crypto gelir SIFIR (crash) |
| Sahte Hacim | Upgrade (U22) | 100 | Crypto musteri x2 (borsada wash trading) | FBI +%8/tur |
| Rug Pull | Action (A20) | 0 | +800 para ANINDA ama Crypto isletme KALICI KAPANIR | FBI +%25, TUM crypto bonuslari kaybolur |

**Kestirme Combo:** "Ponzi Zinciri" -- Crypto Bro + Sahte Hacim + Dolandirici(C09) = +300 para/tur AMA FBI +%20/tur ve her tur %15 sans "Market Crash"

**SEKTORE OZEL EVENT'LER:**

| Event | Sure | Etki |
|---|---|---|
| E16: Regulatory Crackdown | 2 tur | TUM crypto isletmeler dondurulur (gelir SIFIR). Regulasyon Uyumu varsa: BAGISIK |
| E17: Market Crash | 1 tur | Crypto isletme gelir SIFIR + -6 musteri. Sahte Hacim varsa: ek -10 musteri |
| E18: Bull Run | 2 tur | Crypto isletmeler gelir x3, musteri x2. Altın cag! |

**GERI DONUS KARTLARI:**

| Kart | Tip | Maliyet | Etki |
|---|---|---|---|
| DeFi Pivot | Action (A21) | 250 | Kapanmis Crypto isletmeyi yeniden ac, TUM illegal bonuslari temizle, sabit +100 gelir/tur |
| Topluluk Guven Onarimi | Upgrade (U23) | 300 | Pump/crash sonrasi: 8 musteri geri kazan, FBI risk -15 puan |

---

### A.4.5 GAYRIMENKUL / INSAAT

**Hikaye Akisi:**
```
Tur 1-3:   Ucuz arsa al, renovasyon yap
Tur 4-8:   Daire insaat, ev cevirme
Tur 9-14:  Ucuz malzeme mi, rusvet mi, izinsiz insaat mi?
Tur 15+:   Ya saygindeveloper ya da mahkum
```

**DURUST YOL:**

| Kart | Tip | Maliyet | Etki |
|---|---|---|---|
| Mimar | Calisan (C19) | Maas 45/tur | Construction isletmelerde gelir +%25, musteri +3, "Bina Cokme" riskini SIFIRLAR |
| Yesil Bina Sertifikasi | Upgrade (U24) | 350 | Construction isletme gelir +%20, musteri +4 (cevreci imaj), "Deprem Testi" event'ine GECERSIN |

**Durust Combo:** "Kaliteli Insaat" -- Mimar + Yesil Bina Sertifikasi = Gelir +%50, HICBIR insaat event'i seni etkilemez

**KESTIRME YOL:**

| Kart | Tip | Maliyet | Etki | Risk |
|---|---|---|---|---|
| Serefsiz Muteahhit | Calisan (C20) | Maas 20/tur | Construction gelir +%40 (ucuz is) | FBI +%6/tur, her tur %8 sans "Bina Cokme" |
| Malzeme Kismasi | Action (A22) | 50 | Bu tur Construction gelir x2 (ucuz beton, ince demir) | FBI +%8, "Bina Denetimi" event olasiligi x3 |
| Imar Rusveti | Action (A23) | 150 | Yeni isletme slotu AC (izinsiz insaat) | FBI +%15, %20 sans: "Imar Iptali" -- slot geri kapanir + 500 ceza |
| Izinsiz Kat | Upgrade (U25) | 100 | Construction gelir +%50 (ekstra kat, ekstra daire) | FBI +%5/tur, "Deprem Testi" event'inde OTOMATIK KAPANIR |

**SEKTORE OZEL EVENT'LER:**

| Event | Sure | Etki |
|---|---|---|
| E19: Bina Denetimi | 1 tur | Illegal upgrade varsa: isletme 3 tur KAPALI + 500 ceza. Mimar varsa: GECERSIN |
| E20: Bina Cokme | 1 tur | Serefsiz Muteahhit veya Malzeme Kismasi varsa: isletme KALICI KAPANIR + 800 ceza + -10 musteri. Yoksa: etki yok |
| E21: Emlak Patlamasi | 2 tur | Construction isletmeler gelir x2, musteri x1.5 |

**GERI DONUS KARTLARI:**

| Kart | Tip | Maliyet | Etki |
|---|---|---|---|
| Bina Guclendirme | Action (A24) | 400 | Kapanmis Construction isletmeyi ac, TUM illegal upgrade'leri kaldir, gelir +%15 (guvenli) |
| Belediye Anlasma | Upgrade (U26) | 350 | Construction: imar sorunlarina bagisik, FBI risk -10 puan |

---

### A.4.6 REKLAM / PAZARLAMA AJANSI

**Hikaye Akisi:**
```
Tur 1-3:   Kucuk ajans, durust kampanyalar
Tur 4-8:   Buyuk musteriler kazan, vaatler buyut
Tur 9-14:  Sahte metrikler mi, gercek sonuclar mi?
Tur 15+:   Ya saygin ajans ya da dava edilen dolandirici
```

**DURUST YOL:**

| Kart | Tip | Maliyet | Etki |
|---|---|---|---|
| Kreatif Direktor | Calisan (C21) | Maas 45/tur | Marketing isletmelerde gelir +%25, aktif: "Yaratici Kampanya" -- TUM isletmelere +4 musteri bu tur |
| Veri Analisti | Calisan (C22) | Maas 35/tur | Marketing isletmelerde musteri hedefleme: her tur +3 musteri (garanti, randomsiz) |

**Durust Combo:** "Durust Reklam" -- Kreatif Direktor + Veri Analisti + Reklam Ajansi = TUM isletmelere +6 musteri/tur (garanti), gelir +%30

**KESTIRME YOL:**

| Kart | Tip | Maliyet | Etki | Risk |
|---|---|---|---|---|
| Click Bot Ciftligi | Upgrade (U27) | 150 | Marketing musteri x2 (sahte trafik) | FBI +%6/tur, "Platform Bani" event olasiligi x3 |
| Viral Dolandiricilik | Action (A25) | 80 | +15 musteri bu tur (sahte viral kampanya) | FBI +%10, sonraki tur -8 musteri (gercek ortaya cikinca) |
| Calinti Kreative | Action (A26) | 60 | Marketing gelir x1.5 bu tur (baskasinin isini kopyala) | FBI +%8, %25 sans: "Telif Davasi" 400 ceza |
| Sisirmis Metrikler | Upgrade (U28) | 120 | Marketing isletme gelir +%40 (musteriye sahte rapor) | FBI +%5/tur, her 4 turda %20 sans: musteri sirketi terk eder (-5 musteri, -100 gelir) |

**SEKTORE OZEL EVENT'LER:**

| Event | Sure | Etki |
|---|---|---|
| E22: Platform Bani | 2 tur | Click Bot varsa: Marketing isletme 2 tur KAPALI + bot upgrade kaldirilir. Yoksa: etki yok |
| E23: Musteri Davasi | 1 tur | Sisirmis Metrikler varsa: 600 ceza + upgrade kaldirilir. Yoksa: -100 para (kucuk anlasamazlik) |
| E24: Reklam Festivali | 1 tur | Marketing isletmeler gelir x2, TUM isletmelere +5 ekstra musteri |

**GERI DONUS KARTLARI:**

| Kart | Tip | Maliyet | Etki |
|---|---|---|---|
| Ajans Rebrand | Action (A27) | 250 | TUM illegal marketing upgrade'lerini kaldir, +5 musteri, FBI risk -10 |
| Seffaflik Raporu | Upgrade (U29) | 200 | Marketing: sahte metrik riski SIFIR, musteri kaybi -%50, FBI risk artisi -%50 |

---

## A.5 CROSECTOR COMBOLAR (Cross-Sector Combos)

Birden fazla sektore yatirim yapan oyunculari odullendiren ozel combolar:

| # | Isim | Gerekli | Bonus | Zorluk |
|---|---|---|---|---|
| 11 | Dikey Entegrasyon | Food isletme + Construction isletme + Mimar | Her ikisine gelir +%30, musteri +4 | Orta |
| 12 | Dijital Donusum | Herhangi isletme + Tech Startup(aktif) + App Developer | Secilen isletme musteri +8, gelir +%20 | Orta |
| 13 | Marka Imparatorlugu | Fashion isletme + Ad Agency + Marka Elcisi | TUM isletmelere +5 musteri, Fashion gelir x2 | Zor |
| 14 | Kripto Odeme | Crypto Exchange(aktif) + herhangi 2 isletme | 2 isletmenin geliri +%15, Crypto +4 musteri | Orta |
| 15 | Tam Legal Holding | 4 isletme aktif + SIFIR FBI riski + Tier 3+ | TUM gelir +%25, her tur +3 musteri, "Clean Business" skoru +500 | Cok Zor |
| 16 | Karanlik Imparatorluk | 3+ illegal upgrade/calisan aktif + FBI riski %50+ | TUM gelir +%50 AMA her tur FBI kontrolu x2 | Intihar Misyonu |

---

## A.6 SEKTOR-OZEL CALISANLAR OZET TABLOSU

Mevcut calisanlara (C01-C10) ek olarak sektor uzmanlarinin tam listesi:

| ID | Isim | Maas | Sektor | Passif | Aktif | Risk |
|---|---|---|---|---|---|---|
| C11 | App Developer | 35 | Tech | +4 musteri, bug fix -%30 kapanma | "Deploy": Tech gelir x1.3 bu tur | Yok |
| C12 | UX Designer | 40 | Tech | Gelir +%20 | "Kullanici Arastirmasi": sonraki tur musteri x1.5 | Yok |
| C13 | Growth Hacker | 50 | Tech | +8 musteri (agresif) | "Hack the System": +15 musteri bu tur | FBI +%6/tur |
| C14 | Gida Guvenligi Muduru | 35 | Food | Saglik event bagisikligi, +2 musteri | "Denetim Hazirlik": 3 tur saglik event'lerine tam koruma | Yok |
| C15 | Moda Tasarimcisi | 45 | Fashion | +5 musteri, gelir +%15 | "Koleksiyon Lansmani": musteri x2 bu tur (trend varsa x3) | Yok |
| C16 | Marka Elcisi | 55 | Fashion | TUM isletmelere +3 musteri | "Marka Lansmani": bu tur musteri x2 | Yok |
| C17 | Blockchain Dev | 50 | Crypto | Volatilite azalt (min +50) , +3 musteri | "Smart Contract": Crypto gelir sabit 150 bu tur | Yok |
| C18 | Crypto Bro | 30 | Crypto | +6 musteri, gelir +%30 | "Shill Campaign": +12 musteri bu tur | FBI +%5/tur |
| C19 | Mimar | 45 | Construction | Gelir +%25, +3 musteri, cokme riski SIFIR | "Blueprint": sonraki insaat karti %50 indirimli | Yok |
| C20 | Serefsiz Muteahhit | 20 | Construction | Gelir +%40 | "Hizli Insaat": Construction gelir x2 bu tur | FBI +%6/tur |
| C21 | Kreatif Direktor | 45 | Marketing | Gelir +%25 | "Yaratici Kampanya": TUM +4 musteri | Yok |
| C22 | Veri Analisti | 35 | Marketing | +3 musteri (garanti) | "Hedefli Reklam": secilen isletmeye +8 musteri | Yok |

---

## A.7 GERI DONUS MEKANIGI (Comeback System)

### A.7.1 "Phoenix" Skor Bonusu

Tam kapanma (Seviye 5) sonrasi geri donup oyunu KAZANAN oyuncu ekstra skor alir:

| Durum | Bonus Puan |
|---|---|
| Seviye 5 sonrasi geri don + oyunu kazan | +2000 |
| Seviye 4 sonrasi geri don + oyunu kazan | +1000 |
| Seviye 3 sonrasi geri don + oyunu kazan | +500 |
| Hic yakalanmadan kazan (Clean Run) | +800 |

### A.7.2 "Yeni Baslangiç" Mekanigi

Seviye 5 (Tam Kapanma) sonrasi oyun BITMEZ. Bunun yerine:

```
1. Ekran kararir. "Her sey bitti..." text'i belirir.
2. 2 saniye bekleme.
3. "...mi acaba?" text'i belirir.
4. "YENI BASLANGIC" ekrani:
   - 1 isletme sec (mevcut isletmelerinden en dusuk tier)
   - 200 para ile basla
   - TUM illegal kart/upgrade/calisan GIDER
   - Tier KORUNUR (motivasyon)
   - FBI riski SIFIRLANIR
   - Escalation tracker SIFIRLANIR
5. Oyun devam eder. Rakip avantajli ama oyuncu temiz sayfa ile baslar.
```

### A.7.3 Genel Geri Donus Kartlari (Sektorler arasi)

| Kart | Tip | Maliyet | Etki |
|---|---|---|---|
| Avukat | Calisan (C23) | Maas 60/tur | FBI riski -%10/tur (passif), aktif: "Savunma": bir sonraki FBI kontrolunu OTOMATIK GECIR |
| Halka Ozur | Action (A28) | 200 | Skandal sonrasi: 5 musteri geri kazan, 1 kapali isletmeyi hemen ac |
| PR Krizi Yonetimi | Upgrade (U30) | 500 | TUM skandal cezalari -%50, musteri kayiplari -%50 |
| Lobicilik | Upgrade (U31) | 600 | FBI risk cap'i %50 olur (asla %50'yi gecmez), vergi -%25 |
| Tanik Koruma | Action (A29) | 400 | FBI riskini SIFIRLA + 5 tur boyunca FBI risk artisi -%50 |

---

## A.8 UYGULAMA NOTLARI (Implementation Notes)

### A.8.1 Yeni Tag'ler

Mevcut tag sistemi (CardTag enum) genisletilmeli:

```
Mevcut:   Food, Coffee, Tech, Crypto, Marketing, Luxury, ...
Yeni:     Fashion, Construction, Illegal, Organic, Sustainable, 
          Fraudulent, Regulated, CleanBusiness
```

### A.8.2 Escalation Tracker

GameManager veya EconomyManager'a yeni field:

```
int illegalActionCount;     // Toplam illegal aksiyon sayisi
float fbiRiskMultiplier;    // illegalActionCount'a gore 1.0 - 2.5 arasi
```

### A.8.3 Sektor Event Havuzu

EventSystem'e sektor-bazli event filtering:

```
- Temel event havuzu: E01-E06 (mevcut, her zaman aktif)
- Sektor event'leri: Oyuncunun AKTIF isletme tag'lerine gore havuza eklenir
  Ornek: Food isletme varsa -> E10, E11, E12 havuza girer
  Ornek: Illegal upgrade varsa -> o sektore ozel negatif event olasiligi artar
```

### A.8.4 Ikilem Popup

UIManager'a yeni popup tipi. Mevcut ComboPopup/EventPopup/RivalPopup yapisini takip eder:

```
DilemmaPopup.cs
  - Show(CardData riskyCard, float currentFbiRisk, float riskIncrease, string reward)
  - OnConfirm -> karti oyna
  - OnCancel -> kart eline doner (aksiyon HARCANMAZ)
```

### A.8.5 Consequence Chain Manager

Yeni manager veya EconomyManager'a extension. FBI baskin sonucunu belirler:

```
int consecutiveCatches;     // Art arda yakalanma sayisi
ConsequenceLevel GetLevel() // consecutiveCatches + fbiRisk'e gore Level 1-5 dondurur
```

---

## A.9 DENGE HEDEFLERI (Balance Targets)

| Yol | Erken Oyun (Tur 1-8) | Orta Oyun (Tur 9-18) | Gec Oyun (Tur 19+) |
|---|---|---|---|
| Durust | Yavas buyume, 400-700 para, 2 bolge | Sabit buyume, 800-1500 para, 3-4 bolge | Saglam motor, 1500-3000 para, 5-6 bolge |
| Kestirme | Hizli buyume, 600-1200 para, 3 bolge | Yuksek risk, 1000-2500 para VEYA skandal, 3-5 bolge | Ya domine (6+ bolge) ya da cokus (FBI) |
| Karisik | En esnek, 500-900 para, 2-3 bolge | Stratejik riskler, 900-2000 para, 3-5 bolge | Dengeyi bul, 1500-3000 para, 5-6 bolge |

**Hedef:** Durust yol ile kazanma orani %55, kestirme yol ile %40, karisik %50. Kestirme yol DAHA HIZLI ama DAHA RISKLI -- beklenen deger benzer olmali ama varyans cok daha yuksek.

---

> Appendix A sonu. Bu senaryo tasarimlari MVP sonrasi ilk guncelleme (Hafta 29-32) icin oncelikli icerik olarak planlanmistir. Temel sistemler (Escalation Tracker, Consequence Chain, Dilemma Popup) MVP'de implement edilebilir, sektor kartlari kademeli olarak eklenir.
