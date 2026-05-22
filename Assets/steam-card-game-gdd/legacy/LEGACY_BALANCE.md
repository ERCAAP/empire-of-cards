# EKONOMİ BALANS TABLOSU — Empire of Cards

> Bu doküman oyunun ekonomik dengesini tanımlar.
> Tüm sayılar kağıt prototip testinden sonra ayarlanacak.

---

## TEMEL PARAMETRELER

| Parametre | Değer |
|---|---|
| Başlangıç parası | 💰500 |
| İşletme slotu (başlangıç) | 3 |
| İşletme slotu (max) | 5 |
| Aksiyon hakkı (başlangıç) | 3/tur |
| Aksiyon hakkı (max) | 5/tur |
| El kartı (her tur) | 5 |
| Redraw hakkı | 1/tur |
| Başlangıç destesi | 14 kart |
| Vergi oranı | %15 brüt gelirden |
| FBI başlangıç riski | %0 |
| FBI baskın eşiği | Risk% > Zar(1-100) → BASKIN |
| Tur sayısı (run) | 20 tur (premortem revizyonu: 25→20) |

---

## MARKET HAVUZU

Toplam müşteri sayısı sınırlı. Sen ve rakip aynı havuzdan çekiyorsunuz.

| Tur | Toplam Müşteri | Artış |
|---|---|---|
| 1 | 60 | - |
| 2 | 65 | +5 |
| 3 | 70 | +5 |
| 4 | 75 | +5 |
| 5 | 80 | +5 |
| 6-10 | 80-110 | +6/tur |
| 11-15 | 110-150 | +8/tur |
| 16-20 | 150-200 | +10/tur |

**Neden artıyor:** Pazar büyüyor. Geç oyunda daha fazla müşteri = daha büyük kavga.

### Market Share Hesaplama

```
Senin müşterilerin = İşletme bazal müşteri + Çalışan bonusu + Upgrade bonusu + Combo bonusu
Rakibin müşterileri = Rakip AI hesaplaması
Serbest müşteriler = Toplam - (Sen + Rakip)

Market Share = Senin müşterilerin / Toplam × 100

KAZANMA: Market Share ≥ %60
KAYBETME: Rakip Market Share ≥ %70 VEYA Para ≤ 0
```

---

## TUR BAŞINA GELİR/GİDER SİMÜLASYONU

### Senaryo: Normal Oyun (Ortalama Kararlar)

#### Erken Oyun (Tur 1-5)

| Tur | İşletmeler | Brüt Gelir | Maaşlar | Vergi(%15) | Net | Toplam Para | Market% |
|---|---|---|---|---|---|---|---|
| 1 | 1x Büfe | 💰50 | 💰35 (Stajyer×3) | 💰7 | +💰8 | 💰508 | 5% |
| 2 | 1x Büfe + Çaylak | 💰55 | 💰55 | 💰8 | -💰8 | 💰500 | 6% |
| 3 | 1x Büfe + Kahveci açılır | 💰130 | 💰55 | 💰19 | +💰56 | 💰556 | 11% |
| 4 | Büfe + Kahveci + Barista | 💰130+bonus | 💰80 | 💰22 | +💰48 | 💰604 | 16% |
| 5 | Aynı + El İlanı etkisi | 💰160 | 💰80 | 💰24 | +💰56 | 💰660 | 20% |

**Erken oyun hissi:** Para sıkı, her kuruş önemli. İkinci işletme açmak cesaret ister.

#### Orta Oyun (Tur 6-12)

| Tur | Durum | Brüt Gelir | Giderler | Net | Toplam Para | Market% |
|---|---|---|---|---|---|---|
| 6 | +Burger Zinciri | 💰230 | 💰140 | +💰55 | 💰715 | 25% |
| 8 | +Marketing Guru + combo | 💰320 | 💰185 | +💰87 | 💰900 | 32% |
| 10 | +Upgrade (Teslimat Ağı) | 💰400 | 💰200 | +💰140 | 💰1,200 | 40% |
| 12 | İlk combo patlıyor | 💰500 | 💰220 | +💰205 | 💰1,600 | 48% |

**Orta oyun hissi:** Motor çalışıyor. Combo'lar gelir artırıyor. Ama rakip de büyüyor.

#### Geç Oyun (Tur 13-20)

| Tur | Durum | Brüt Gelir | Giderler | Net | Toplam Para | Market% |
|---|---|---|---|---|---|---|
| 13 | Kriz event'i! | 💰350 | 💰220 | +💰77 | 💰1,677 | 45% |
| 15 | Rakip agresif, müşteri çalıyor | 💰420 | 💰240 | +💰117 | 💰1,900 | 50% |
| 17 | Büyük combo + Viral Pazarlama | 💰650 | 💰260 | +💰292 | 💰2,400 | 55% |
| 20 | Final — ya %60 ya değil | 💰550 | 💰260 | +💰207 | 💰2,800 | 58-62% |

**Geç oyun hissi:** Kavga yoğun. Bir kriz her şeyi değiştirebilir. Son turlar gerilimli.

---

## RAKİP AI EKONOMİSİ

Rakip de aynı ekonomi kurallarıyla çalışır (hile yok):

| Tur | Rakip Durum | Rakip Market% |
|---|---|---|
| 1 | 1 orta işletme (💰80/tur, 5 müşteri), 1 çalışan | 8% |
| 5 | +1 işletme, +1 çalışan | 18% |
| 10 | 3 işletme, 4 çalışan, 1 upgrade | 35% |
| 15 | 3-4 işletme, 5-6 çalışan, agresif hamleler | 42% |
| 20 | Tam kapasite | 45-55% |

**Rakip büyüme hızı oyuncudan biraz yavaş** (çünkü AI combo optimize edemez).
Ama Ascension yükseldikçe rakip daha hızlı büyür.

### Rakip Hamle Sıklığı
| Zorluk | Hamle/Tur |
|---|---|
| Kolay | 1-2 |
| Normal | 2 |
| Zor | 2-3 |

---

## FBI BASKIN MEKANİĞİ

```
FBI Risk = Toplam illegal etki puanı

Her illegal kart → risk artar:
  Hacker: +%10/tur (aktif olduğu sürece)
  Dolandırıcı: +%12/tur
  Sahte Yorumlar: +%12 (tek seferlik)
  Sabotaj: +%15 (tek seferlik)

Güvenlik Sistemi: -%25 (kalıcı)

Her tur sonu:
  Zar = random(1-100)
  Eğer FBI Risk > Zar → BASKIN!

BASKIN SONUÇLARI:
  → 💰300 ceza
  → En yüksek gelirli illegal çalışan kovulur
  → FBI riski sıfırlanır (temiz sayfa)
```

**Örnek:**
- Tur 8: Hacker aktif 3 turdur (+%30), Sahte Yorumlar kullanıldı (+%12) = %42 risk
- Zar: 55 → Kurtuldun (%42 < 55)
- Tur 9: Risk %52 → Zar: 38 → BASKIN! 💰300 ceza, Hacker kovuldu, risk → %0

---

## VERGİ SİSTEMİ

```
Vergi = Brüt Gelir × %15

Muhasebeci varsa: Vergi = Brüt Gelir × %7.5
2 Muhasebeci varsa: Vergi = Brüt Gelir × %3 (minimum)

Vergi Kaçırma (illegal): Vergi = 💰0, FBI riski +%20
```

---

## DENGE HEDEFLERİ

Bu sayıların tutması gereken oranlar:

| Hedef | Oran | Neden |
|---|---|---|
| İlk işletme açma | Tur 3-4 | Çok erken = gerilim yok, çok geç = sıkılır |
| İlk combo | Tur 6-8 | "Oha" anı çok geç gelmemeli |
| Para sıkıntısı hissi | Tur 1-8 | Her karar önemli hissetmeli |
| Rahat hissi | Asla tamamen | Oyuncu asla "param bol" dememeli |
| Market %60 ulaşılabilirlik | Tur 18-20 | Son 2-3 turda gerilimli olmalı |
| Rakip tehdit hissi | Tur 10+ | Ortadan sonra rakip baskısı artmalı |
| FBI baskın olasılığı | %20-40 arası | Illegal oynamak cazip ama riskli |
| Ortalama run süresi | 30-40 dk | "One more run" uzunluğu |
| Kart satın alma sıklığı | Her 2-3 turda 1 | Deste sürekli büyümeli ama hızlı değil |

---

## MAAŞ TABLOSU (Tüm çalışanlar)

| Çalışan | Maaş/tur | Gelir katkısı/tur (tahmini) | ROI (tur) |
|---|---|---|---|
| Stajyer | 💰15 | +💰10-20 | 8-15 tur (kötü) |
| Çaylak Pazarlamacı | 💰20 | +💰15-30 | 5-8 tur (orta) |
| Barista (kahvecide) | 💰25 | +💰40-60 | 2-3 tur (çok iyi) |
| Şef (food'da) | 💰30 | +💰30-50 | 3-4 tur (iyi) |
| Marketing Gurusu | 💰45 | +💰50-80 | 3-4 tur (iyi) |
| Influencer (trend varken) | 💰50 | +💰80-150 | 1-2 tur (mükemmel) |
| Influencer (trend yokken) | 💰50 | +💰30-40 | 5-8 tur (kötü) |
| Hacker | 💰60 | +💰40-80 (dolaylı) | 3-5 tur (riskli) |
| Muhasebeci | 💰30 | +💰20-60 (vergi tasarrufu) | 3-5 tur (sağlam) |
| Dolandırıcı | 💰40 | +💰120 (direkt) | 1 tur (OP ama riskli) |
| Sadık Müdür | 💰45 | +💰20 + koruma | Hesaplanamaz (stratejik) |

---

## İŞLETME KARLILIK TABLOSU

| İşletme | Maliyet | Brüt Gelir/tur | Geri Ödeme (tur) | Risk |
|---|---|---|---|---|
| Büfe | 💰0 | 💰50 | 0 (bedava) | Düşük |
| Kahveci | 💰150 | 💰80 (120 trend) | 2 tur | Düşük |
| Burger Zinciri | 💰250 | 💰100 | 2.5 tur | Düşük |
| Tech Startup | 💰200 | 💰0→150 | 4-5 tur | Orta |
| Gece Kulübü | 💰350 | 💰180 (0 trend yok) | 2 tur (trend) / ∞ (yok) | Yüksek |
| Organik Çiftlik | 💰120 | 💰40+bonus | 3 tur | Düşük |
| Kripto Borsası | 💰300 | 💰0-250 | ??? (kumar) | Çok Yüksek |
| Reklam Ajansı | 💰200 | 💰60+global | 3-4 tur | Düşük |

---

## STRATEJİ ARKETİPLERİ

Ekonomi dengesinin farklı stratejileri desteklemesi gerekir:

### 1. "Temiz İşadamı" 🏢
- Kahveci + Burger + Şef + Marketing Guru
- Düzenli gelir, düşük risk
- Market share yavaş ama sağlam büyür
- Hedef: Tur 19-20'de %60

### 2. "Suç İmparatorluğu" 🕶️
- Hacker + Dolandırıcı + Sahte Yorumlar + Sabotaj
- Hızlı büyüme, FBI riski çok yüksek
- Güvenlik Sistemi şart
- Hedef: Tur 14-16'da %60 (veya FBI'dan tur 12'de batış)

### 3. "Trend Sörfçüsü" 🏄
- Kahveci + Gece Kulübü + Influencer + Viral Pazarlama
- Trend varken patlıyor, yokken sıkıntı
- Event'lere çok bağımlı
- Hedef: Doğru event'lerle tur 15-17'de %60

### 4. "Tech Baronu" 💻
- Tech Startup + Reklam Ajansı + Otomasyon + AI Asistanı
- Yavaş başlangıç, geç patlama
- Combo: Tech + Otomasyon + AI = motor
- Hedef: Tur 10'dan sonra hızlanma, tur 18-20'de %60

### 5. "Hayatta Kalma" 🔥
- Her şeyden biraz, krizlere adaptasyon
- Reaktif strateji, event'lere göre uyum
- Jack of all trades, master of none
- Hedef: Esnek ama asla dominant değil

**Denge kuralı:** Hiçbir strateji diğerlerini kesin olarak yenmemeli. Hepsi viable olmalı.

---

## PLAYTEST SORULARI

Ekonomi dengesini test ederken şu soruları sor:

1. Tur 5'te para sıkıntısı hissediyor musun? (Hedef: Evet)
2. İlk combo ne zaman patladı? (Hedef: Tur 6-8)
3. Hiç "yapacak bir şey yok" dediğin tur oldu mu? (Hedef: Hayır)
4. Rakip tehditkâr hissettirdi mi? (Hedef: Tur 10+ evet)
5. Son turda gerilim var mıydı? (Hedef: Evet, %55-65 arası)
6. İllegal yol cazip geldi mi? (Hedef: Evet ama korkutucu)
7. Para hiç anlamsızlaştı mı? (Hedef: Hayır, asla)
8. Run ne kadar sürdü? (Hedef: 30-40 dk)
