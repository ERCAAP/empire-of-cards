# KAĞIT PROTOTİP REHBERİ — Empire of Cards

> Bu rehberle oyunu kağıtla oynayıp test edebilirsin.
> Hedef: "Kart koymak eğlenceli mi?" sorusuna cevap almak.
> Durum: Legacy-leaning paper reference. Aktif gameplay truth'u degildir.

---

## İHTİYACIN OLAN MALZEMELER

| Malzeme | Miktar | Not |
|---|---|---|
| Boş kart (veya kesilmiş kağıt) | 50 adet | 7x10 cm ideal |
| Kalem (renkli tercihen) | 5 renk | Mavi/yeşil/kırmızı/mor/sarı (kart tipleri) |
| Madeni para veya jeton | 30+ adet | Müşteri token'ı olarak |
| Zar (6 yüzlü) | 2 adet | Kripto + FBI için |
| Kağıt para (veya sayaç) | - | 💰 takibi için not kağıdı yeterli |
| Skor tablosu kağıdı | 1 adet | Tur tur not için |

---

## KARTLARI HAZIRLA

Her karta şunu yaz:

### İşletme kartı (MAVİ kenar):
```
┌─────────────┐
│ ★★ [NADİR]  │
│ MAVİ        │
│             │
│ KAHVECİ    │
│             │
│ 💰80/tur    │
│ 👤5 müşteri │
│ 🔧2 slot    │
│             │
│ Trend:x1.5  │
│ Tag: food   │
│      coffee │
└─────────────┘
```

### Çalışan kartı (YEŞİL kenar):
```
┌─────────────┐
│ ★ [COMMON]  │
│ YEŞİL       │
│             │
│ BARİSTA     │
│             │
│ 💰25 maaş   │
│ +3 müşteri  │
│ coffee: +6  │
│             │
│ Tag: food   │
│      coffee │
└─────────────┘
```

### Action kartı (KIRMIZI kenar):
```
┌─────────────┐
│ ★★ [UNC]    │
│ KIRMIZI     │
│             │
│ VİRAL       │
│ PAZARLAMA   │
│             │
│ 💰150       │
│ Bu tur tüm  │
│ müşteri x2  │
│             │
│ Tag:marketing│
└─────────────┘
```

Tüm 40 kartı `legacy/LEGACY_CARD_LIST.md` uzerinden tarihsel referans olarak yaz.

---

## OYUN KURULUMU

### Masa düzeni:
```
┌──────────────────────────────────────┐
│                                      │
│  RAKİP ALANI (kağıt üstünde takip)  │
│  [İşletme][İşletme][___]            │
│                                      │
│  ══════ MARKET ══════               │
│  Müşteri: [jeton sayısı]            │
│  Event: [açık kart]                 │
│  ═══════════════════                │
│                                      │
│  SENİN ALANIN                       │
│  [Slot 1] [Slot 2] [Slot 3]        │
│                                      │
│  💰: ___  Market%: ___  FBI%: ___   │
│                                      │
│  ELİN: [kart][kart][kart][kart][kart]│
│                                      │
│  [DESTE]  [ÇÖP]  [DÜKKAN: 3 kart] │
└──────────────────────────────────────┘
```

### Başlangıç:
1. Başlangıç destesini (14 kart) karıştır
2. Event destesini (6 kart) ayrı karıştır
3. Geri kalan kartları "dükkan destesi" olarak ayır
4. 💰500 yaz
5. Market müşterisi: 60 jeton ortaya koy
6. Rakip: 1 işletme (💰80/tur, 5 müşteri) + 1 çalışan olarak not al
7. 5 kart çek

---

## TUR AKIŞI (Adım adım)

### ADIM 1: EVENT (her 3 turda 1)
- Tur 3, 6, 9, 12, 15, 18'de event destesinden 1 kart aç
- Kartı masanın ortasına koy
- Süresini not et (1-2 tur)
- Süresi dolan event'leri kaldır

### ADIM 2: KART ÇEK
- Destenden 5 kart çek
- İstersen 1 kartı at, 1 yeni çek (redraw)

### ADIM 3: OYNA (3 aksiyon)
Her aksiyon = 1 kart oynama:

**İşletme oynamak:**
- Kartı boş slota koy
- Satın alma maliyetini parandan düş
- Slot yoksa oynayamazsın

**Çalışan oynamak:**
- Kartı bir işletmenin altına koy
- İşletmenin çalışan slotu doluysa oynayamazsın
- Tur sonunda maaşı otomatik kesilecek

**Action oynamak:**
- Kartı oyna, efekti uygula, kartı çöpe at
- Maliyeti varsa parandan düş

**Upgrade oynamak:**
- Kartı masaya koy (genel) veya işletmenin yanına (spesifik)
- Maliyeti varsa parandan düş

**Kart satmak (aksiyon HARCAMAZ):**
- İstemediğin kartı çöpe at
- Kartın satın alma maliyetinin %40'ı kadar para al

**Hiçbir şey yapmamak:**
- "Tur Bitir" diyebilirsin, aksiyon kalmasa bile

### ADIM 4: SİSTEM ÇALIŞIR

Bu sırayla hesapla:

**4a. İşletme geliri:**
```
Her işletme için: Taban gelir × modifier'lar = brüt gelir
Modifier: çalışan bonusu, upgrade bonusu, event etkisi, combo bonusu
```

**4b. Müşteri hesabı:**
```
Her işletme için: Taban müşteri + çalışan bonusu + upgrade bonusu
Toplam müşterilerin = Tüm işletmelerin müşteri toplamı
```

**4c. Combo kontrolü:**
```
COMBO_MATRIX.md'ye bak.
Aktif kartların tag'leri combo koşulunu sağlıyor mu?
Sağlıyorsa: bonus uygula + "COMBO!" de (sesli!)
```

**4d. Gelir/gider hesabı:**
```
Brüt gelir = Tüm işletme gelirleri toplamı
- Maaşlar = Tüm çalışan maaşları toplamı
- Vergi = Brüt gelir × %15 (muhasebeci varsa %7.5)
+ Combo bonusları
+ İllegal gelirler (Dolandırıcı vs)
= Net gelir → Paraya ekle
```

**4e. Market share:**
```
Market share = Senin müşterilerin / Toplam market müşterisi × 100
Not al.
```

**4f. FBI kontrolü (illegal kart varsa):**
```
Toplam FBI risk yüzdesini hesapla
Zar at (1-6): 
  1 = 1-16, 2 = 17-33, 3 = 34-50, 4 = 51-66, 5 = 67-83, 6 = 84-100
  (veya 2 zar at, topla, 2-12 arasını %'ye çevir)
  
Basit yöntem: 
  Risk ≥ %50 ise → 1 zar at, 1-3 = BASKIN, 4-6 = kurtuldun
  Risk %25-49 ise → 1 zar at, 1-2 = BASKIN, 3-6 = kurtuldun  
  Risk < %25 ise → 1 zar at, 1 = BASKIN, 2-6 = kurtuldun

BASKIN: 💰300 ceza + en pahalı illegal çalışan kovulur + risk sıfırlanır
```

### ADIM 5: RAKİP OYNAR

Rakip için basit kurallar (sen oyna):

**Rakip her turda 2 hamle yapar. Sırayla kontrol et:**

1. Rakip parası ≥ 200 VE 3'ten az işletmesi var → Yeni işletme aç (💰100-200 arası bir tane)
2. Rakip işletmesinde boş çalışan slotu var → Çalışan al (💰30-50 maaşlı)
3. Senin market share'in > %45 → Agresif hamle: senden 3 müşteri çal
4. Event aktif VE rakibe uygun → Event bonusunu kullan
5. Hiçbiri değilse → Normal büyüme: +💰50 gelir, +2 müşteri

**Rakip geliri (basit):**
- Rakibin işletme sayısı × 💰80 + çalışan sayısı × 💰20 = brüt
- Maaş: çalışan sayısı × 💰30
- Net = brüt - maaş

**Rakip müşterisi:**
- İşletme sayısı × 5 + çalışan sayısı × 2

**Rakip büyüme takvimi:**
| Tur | Rakip yapacakları |
|---|---|
| 1-4 | 1 işletme + 1-2 çalışan ile başlar, yavaş büyür |
| 5-8 | 2. işletme açar, 3-4 çalışan |
| 9-12 | 3. işletme, agresif oynamaya başlar |
| 13-16 | Tam güçte, senden müşteri çalmaya odaklanır |
| 17-20 | Kaybediyorsa → desperate hamle, kazanıyorsa → konsolide |

---

## KAZANMA / KAYBETME

| Durum | Sonuç |
|---|---|
| Tur 20 sonunda market share ≥ %60 | KAZANDIN |
| Tur 20 sonunda market share < %60 | Skor hesapla, tekrar dene |
| Herhangi bir turda rakip ≥ %70 | KAYBETTİN |
| Herhangi bir turda para ≤ 0 | İFLAS — KAYBETTİN |

---

## SKOR HESAPLAMA (Run sonunda)

| Metrik | Çarpan |
|---|---|
| Toplam kazanılan para (run boyunca) | x1 |
| Final market share (%) | x50 |
| Aktif combo sayısı | x200 |
| Hayatta kalan işletme sayısı | x100 |
| Kaç tur FBI'dan kurtuldun | x50 |
| Kaybetmeden bitirdiysen | +1000 bonus |

---

## PLAYTEST SKOR TABLOSU

Her run için bu tabloyu doldur:

```
RUN #___  Tarih: ___

Tur | Para  | İşletme | Çalışan | Müşteri | Market% | Rakip% | Event      | Notlar
----|-------|---------|---------|---------|---------|--------|------------|--------
1   |       |         |         |         |         |        |            |
2   |       |         |         |         |         |        |            |
3   |       |         |         |         |         |        |            |
4   |       |         |         |         |         |        |            |
5   |       |         |         |         |         |        |            |
6   |       |         |         |         |         |        |            |
7   |       |         |         |         |         |        |            |
8   |       |         |         |         |         |        |            |
9   |       |         |         |         |         |        |            |
10  |       |         |         |         |         |        |            |
11  |       |         |         |         |         |        |            |
12  |       |         |         |         |         |        |            |
13  |       |         |         |         |         |        |            |
14  |       |         |         |         |         |        |            |
15  |       |         |         |         |         |        |            |
16  |       |         |         |         |         |        |            |
17  |       |         |         |         |         |        |            |
18  |       |         |         |         |         |        |            |
19  |       |         |         |         |         |        |            |
20  |       |         |         |         |         |        |            |

SONUÇ: KAZANDIM / KAYBETTİM / İFLAS
SKOR: ___
Süre: ___ dakika

NOTLAR:
- Eğlenceli miydi? (1-10): ___
- Sıkıldığım tur var mıydı? Hangisi: ___
- En iyi an: ___
- En kötü an: ___
- Dengeyle ilgili sorun: ___
- "Bir daha oynamak ister miyim?" (Evet/Hayır): ___
```

---

## PLAYTEST TESTÇİLER İÇİN SORULAR

Run sonunda tester'a sor:

1. **Eğlence:** "1-10, ne kadar eğlendin?"
2. **Anlama:** "Kuralları anlamakta zorlandığın yer oldu mu?"
3. **Karar:** "Zor bir karar verdiğin an var mıydı? Anlat."
4. **Sıkılma:** "Sıkıldığın tur oldu mu? Hangisi?"
5. **Combo:** "Combo fark ettin mi? Ne hissettin?"
6. **Rakip:** "Rakip tehditkâr hissettirdi mi?"
7. **Para:** "Para hiç anlamsızlaştı mı? Hep sıkı mıydı?"
8. **Süre:** "Oyun çok mu uzundu, çok mu kısaydı, tam mıydı?"
9. **Tekrar:** "Bir daha oynamak ister misin?"
10. **Satın alma:** "Bu oyun Steam'de $10 olsa alır mıydın?"
