# KART LİSTESİ — Empire of Cards MVP (40 Kart)

> Versiyon: 1.0 | Tarih: 2026-05-18
> Bu liste kağıt prototip + Unity prototip için referanstır.

---

## ÖZET

| Tip | Renk | Adet | Başlangıç Destesinde |
|---|---|---|---|
| İşletme | Mavi | 8 | 2 |
| Çalışan | Yeşil | 10 | 5 |
| Action | Kırmızı | 10 | 5 |
| Upgrade | Mor | 6 | 2 |
| Event | Sarı | 6 | 0 (ayrı deste) |
| **Toplam** | | **40** | **14 (başlangıç)** |

---

## BAŞLANGIÇ DESTESİ (14 kart)

Oyuncu her run'a bu desteyle başlar. Zayıf ama temel mekanikleri öğretir.

| # | Kart | Tip | Adet |
|---|---|---|---|
| B01 | Büfe | İşletme | 2 |
| C01 | Stajyer | Çalışan | 3 |
| C02 | Çaylak Pazarlamacı | Çalışan | 2 |
| A01 | El İlanı | Action | 3 |
| A02 | Küçük Yatırım | Action | 2 |
| U01 | Ofis Malzemeleri | Upgrade | 2 |

**Başlangıç parası:** 💰500

---

## İLK GİRİŞİM SEÇENEKLERİ

Oyuncu her run başında bir girişim seçer. Bu seçim başlangıç destesini ve board'u değiştirir. Class kilidi yoktur — sadece başlangıç yönü verir.

| # | Girişim | Board'a Yerleşen | Destene Eklenen | Başlangıç Parası | Toplam Deste |
|---|---|---|---|---|---|
| 1 | 🍔 BÜFE | B01 Büfe → Slot 1 | +1 Şef (C04) | 💰500 | 15 kart |
| 2 | 💻 TECH STARTUP | B04 Tech Startup → Slot 1 | +1 Hacker (C07) | 💰500 | 15 kart |
| 3 | 📢 REKLAM AJANSI | B08 Reklam Ajansı → Slot 1 | +1 Marketing Gurusu (C05) | 💰500 | 15 kart |
| 4 | 🕶️ KARANLIK PAZAR | Boş (oyuncu seçer) | +1 Dolandırıcı (C09) | 💰700 | 15 kart |

**Notlar:**
- Tutorial run'ında Büfe otomatik seçilir (yeni oyuncu seçim ekranını görmez).
- Girişim seçimi Shop içeriğini etkilemez — her zaman 40 kartlık havuz döner.
- Büfe girişiminde B01 hem board'a yerleşir hem destede kalır (yani destede hala 2x B01 var, biri board'da biri destede).
- Karanlik Pazar'da board boş başlar ama 💰200 ekstra para ile telafi edilir.

---

## İŞLETME KARTLARI (8 adet)

İşletmeler slota konur, kalıcıdır. Her tur otomatik gelir üretir ve müşteri çeker.

### B01 — Büfe ★
| Özellik | Değer |
|---|---|
| Nadirlik | Common |
| Satın Alma | 💰0 (başlangıçta var) |
| Gelir | 💰50/tur |
| Müşteri | 3/tur |
| Çalışan Slotu | 1 |
| Özel | Yok |
| Tag | `food`, `basic` |
> *Mütevazı başlangıç. Herkesin ilk işletmesi.*

### B02 — Kahveci ★
| Özellik | Değer |
|---|---|
| Nadirlik | Common |
| Satın Alma | 💰150 |
| Gelir | 💰80/tur |
| Müşteri | 5/tur |
| Çalışan Slotu | 2 |
| Özel | Trend aktifken gelir x1.5 |
| Tag | `food`, `coffee`, `trendy` |
> *Trend'lere duyarlı. Doğru zamanda çok kârlı.*

### B03 — Burger Zinciri ★★
| Özellik | Değer |
|---|---|
| Nadirlik | Uncommon |
| Satın Alma | 💰250 |
| Gelir | 💰100/tur |
| Müşteri | 6/tur |
| Çalışan Slotu | 3 |
| Özel | En çok çalışan slotu. Combo potansiyeli yüksek. |
| Tag | `food`, `chain` |
> *Çok çalışan = çok sinerji. Ama maaşlar da artar.*

### B04 — Tech Startup ★★
| Özellik | Değer |
|---|---|
| Nadirlik | Uncommon |
| Satın Alma | 💰200 |
| Gelir | 💰0 ilk 3 tur → 💰150/tur sonra |
| Müşteri | 0 → 4/tur |
| Çalışan Slotu | 2 |
| Özel | İlk 3 tur gelir yok. 4. turda aktif olur. |
| Tag | `tech`, `startup` |
> *Sabır ister. Ama tutarsa çok kârlı.*

### B05 — Gece Kulübü ★★★
| Özellik | Değer |
|---|---|
| Nadirlik | Rare |
| Satın Alma | 💰350 |
| Gelir | 💰180/tur (sadece trend varken) |
| Müşteri | 10/tur (sadece trend varken) |
| Çalışan Slotu | 2 |
| Özel | Herhangi bir trend aktifken çalışır. Trend yoksa 💰0. |
| Tag | `entertainment`, `nightlife`, `trendy` |
> *Yüksek ödül, yüksek risk. Trend yoksa boş duruyor.*

### B06 — Organik Çiftlik ★
| Özellik | Değer |
|---|---|
| Nadirlik | Common |
| Satın Alma | 💰120 |
| Gelir | 💰40/tur |
| Müşteri | 2/tur |
| Çalışan Slotu | 1 |
| Özel | Masadaki tüm `food` işletmelerine +💰20/tur bonus. |
| Tag | `food`, `organic`, `support` |
> *Tek başına zayıf. Ama food combo'larını güçlendiriyor.*

### B07 — Kripto Borsası ★★★
| Özellik | Değer |
|---|---|
| Nadirlik | Rare |
| Satın Alma | 💰300 |
| Gelir | 💰0-250/tur (rastgele: zar at, 1-2: 0, 3-4: 💰100, 5-6: 💰250) |
| Müşteri | 2/tur |
| Çalışan Slotu | 1 |
| Özel | Her tur gelir rastgele. Kumar hissi. |
| Tag | `tech`, `crypto`, `risky` |
> *Kumar. Bazen sıfır, bazen jackpot.*

### B08 — Reklam Ajansı ★★
| Özellik | Değer |
|---|---|
| Nadirlik | Uncommon |
| Satın Alma | 💰200 |
| Gelir | 💰60/tur |
| Müşteri | 3/tur |
| Çalışan Slotu | 2 |
| Özel | Masadaki tüm işletmelere +2 müşteri/tur. |
| Tag | `marketing`, `support` |
> *Kendi geliri düşük ama tüm sistemi güçlendiriyor.*

---

## ÇALIŞAN KARTLARI (10 adet)

Çalışanlar işletmelerin üstüne konur. İşletmenin gücünü artırır.
Her çalışanın tur başına maaşı var (otomatik kesilir).

### C01 — Stajyer ★
| Özellik | Değer |
|---|---|
| Nadirlik | Common |
| Maaş | 💰15/tur |
| Etki | +1 müşteri |
| Tag | `basic` |
> *Ucuz, zayıf. Başlangıçta elde olan.*

### C02 — Çaylak Pazarlamacı ★
| Özellik | Değer |
|---|---|
| Nadirlik | Common |
| Maaş | 💰20/tur |
| Etki | İşletme geliri +%10 |
| Tag | `marketing`, `basic` |
> *Küçük ama tutarlı bonus.*

### C03 — Barista ★
| Özellik | Değer |
|---|---|
| Nadirlik | Common |
| Maaş | 💰25/tur |
| Etki | +3 müşteri. `coffee` işletmesinde: +6 müşteri |
| Tag | `food`, `coffee` |
> *Kahveci'ye koyarsan ikiye katlanır.*

### C04 — Şef ★
| Özellik | Değer |
|---|---|
| Nadirlik | Common |
| Maaş | 💰30/tur |
| Etki | +3 müşteri. `food` işletmesinde: gelir +💰30 |
| Tag | `food` |
> *Yemek sektöründe güçlü.*

### C05 — Marketing Gurusu ★★
| Özellik | Değer |
|---|---|
| Nadirlik | Uncommon |
| Maaş | 💰45/tur |
| Etki | İşletme geliri +%25. Marketing combo'larını tetikler. |
| Tag | `marketing`, `guru` |
> *Pahalı ama güçlü. Combo parçası.*

### C06 — Influencer ★★
| Özellik | Değer |
|---|---|
| Nadirlik | Uncommon |
| Maaş | 💰50/tur |
| Etki | +5 müşteri. Trend aktifken: +12 müşteri |
| Tag | `marketing`, `influencer`, `trendy` |
> *Trend varken patlıyor. Yokken ortalama.*

### C07 — Hacker ★★★
| Özellik | Değer |
|---|---|
| Nadirlik | Rare |
| Maaş | 💰60/tur |
| Etki | Rakipten -4 müşteri çal. FBI riski +%10/tur. |
| Tag | `tech`, `illegal` |
> *Güçlü ama tehlikeli. Her tur baskın riski artar.*

### C08 — Muhasebeci ★
| Özellik | Değer |
|---|---|
| Nadirlik | Common |
| Maaş | 💰30/tur |
| Etki | Vergi -%50 (normalde %15 → %7.5). |
| Tag | `finance` |
> *Sıkıcı ama her kuruşu kurtarır. Combo yok ama sağlam.*

### C09 — Dolandırıcı ★★★
| Özellik | Değer |
|---|---|
| Nadirlik | Rare |
| Maaş | 💰40/tur |
| Etki | +💰120/tur direkt nakit (illegal). FBI riski +%12/tur. |
| Tag | `illegal`, `finance` |
> *Hızlı para. Ama FBI kapıda.*

### C10 — Sadık Müdür ★★
| Özellik | Değer |
|---|---|
| Nadirlik | Uncommon |
| Maaş | 💰45/tur |
| Etki | Bu işletmedeki çalışanlar transfer edilemez. +💰20/tur. |
| Tag | `management` |
> *Rakip çalışan çalamaz. Güvenlik.*

---

## ACTION KARTLARI (10 adet)

Tek kullanımlık. Oynanır, efekt olur, kart gider.

### A01 — El İlanı ★
| Özellik | Değer |
|---|---|
| Nadirlik | Common |
| Maliyet | 💰0 |
| Etki | Bu tur +3 müşteri (rastgele 1 işletmeye) |
| Tag | `marketing`, `basic` |
> *Bedava ama zayıf. Başlangıç kartı.*

### A02 — Küçük Yatırım ★
| Özellik | Değer |
|---|---|
| Nadirlik | Common |
| Maliyet | 💰0 |
| Etki | Anında +💰150 |
| Tag | `finance`, `basic` |
> *Acil nakit. Başlangıç kartı.*

### A03 — Viral Pazarlama ★★
| Özellik | Değer |
|---|---|
| Nadirlik | Uncommon |
| Maliyet | 💰150 |
| Etki | Bu tur TÜM işletmelerin müşterisi x2 |
| Tag | `marketing`, `viral` |
> *Doğru turda oynayınca patlıyor.*

### A04 — Düşmanca Devralma ★★★
| Özellik | Değer |
|---|---|
| Nadirlik | Rare |
| Maliyet | 💰400 |
| Etki | Rakibin en zayıf işletmesini kapat. |
| Tag | `aggressive` |
> *Pahalı ama rakibi doğrudan zayıflatıyor.*

### A05 — Sahte Yorumlar ★★
| Özellik | Değer |
|---|---|
| Nadirlik | Uncommon |
| Maliyet | 💰80 |
| Etki | +8 müşteri (1 işletmeye). FBI riski +%12. |
| Tag | `marketing`, `illegal` |
> *Ucuz müşteri. Ama risk var.*

### A06 — Fiyat Kırma ★★
| Özellik | Değer |
|---|---|
| Nadirlik | Uncommon |
| Maliyet | Bu tur gelirin %50'si |
| Etki | Rakipten 8 müşteri çal. |
| Tag | `aggressive`, `pricing` |
> *Gelirinden vazgeç, müşteri kazan.*

### A07 — Sabotaj ★★★
| Özellik | Değer |
|---|---|
| Nadirlik | Rare |
| Maliyet | 💰250 |
| Etki | Rakip 1 tur üretim yapamaz. FBI riski +%15. |
| Tag | `aggressive`, `illegal` |
> *Güçlü ama çok riskli.*

### A08 — Yatırımcı Sunumu ★★
| Özellik | Değer |
|---|---|
| Nadirlik | Uncommon |
| Maliyet | 💰0 |
| Etki | Anında +💰600. Ama sonraki 3 tur gelirinin %15'i yatırımcıya. |
| Tag | `finance`, `investor` |
> *Hızlı büyük para. Ama uzun vadede ödersin.*

### A09 — Acil İşe Alım ★
| Özellik | Değer |
|---|---|
| Nadirlik | Common |
| Maliyet | 💰100 |
| Etki | Destenden rastgele 1 çalışan kartı çek ve hemen oyna (aksiyon harcamaz). |
| Tag | `hiring` |
> *Hızlı çalışan. Ama ne geleceği belli değil.*

### A10 — Tasfiye Satışı ★
| Özellik | Değer |
|---|---|
| Nadirlik | Common |
| Maliyet | 1 işletmeyi feda et |
| Etki | Feda edilen işletmenin satın alma fiyatının 2x'i kadar 💰 al. |
| Tag | `finance`, `desperate` |
> *Batıyorken son çare. Ya da stratejik hamle.*

---

## UPGRADE KARTLARI (6 adet)

İşletmeye veya sisteme konur. Kalıcı etki.

### U01 — Ofis Malzemeleri ★
| Özellik | Değer |
|---|---|
| Nadirlik | Common |
| Maliyet | 💰0 (başlangıçta var) |
| Etki | 1 işletmenin geliri +%10 (kalıcı) |
| Tag | `basic`, `office` |
> *Küçük ama bedava. Başlangıç kartı.*

### U02 — Otomasyon ★★
| Özellik | Değer |
|---|---|
| Nadirlik | Uncommon |
| Maliyet | 💰300 |
| Etki | 1 işletmenin geliri +%30 (kalıcı). O işletmenin 1 çalışan slotu kapanır. |
| Tag | `tech`, `automation` |
> *Güçlü gelir artışı. Ama 1 çalışan slotunu kaybedersin. Trade-off.*

### U03 — Teslimat Ağı ★★
| Özellik | Değer |
|---|---|
| Nadirlik | Uncommon |
| Maliyet | 💰250 |
| Etki | TÜM işletmelere +2 müşteri/tur (kalıcı) |
| Tag | `logistics` |
> *Birden fazla işletmen varsa çok değerli.*

### U04 — Reklam Panosu ★
| Özellik | Değer |
|---|---|
| Nadirlik | Common |
| Maliyet | 💰120 |
| Etki | +3 müşteri/tur (kalıcı, işletme bağımsız) |
| Tag | `marketing` |
> *Ucuz, basit, etkili.*

### U05 — Güvenlik Sistemi ★★★
| Özellik | Değer |
|---|---|
| Nadirlik | Rare |
| Maliyet | 💰280 |
| Etki | FBI baskın riski -%25 (kalıcı) |
| Tag | `security` |
> *İllegal strateji oynuyorsan şart.*

### U06 — Yapay Zeka Asistanı ★★★
| Özellik | Değer |
|---|---|
| Nadirlik | Rare |
| Maliyet | 💰400 |
| Etki | +1 aksiyon hakkı (3 → 4 olur) |
| Tag | `tech`, `ai` |
> *Oyunun en güçlü upgrade'i. Tur başına 1 ekstra hamle.*

---

## EVENT KARTLARI (6 adet)

Ayrı desteden çekilir. Oyuncu oynamaz — otomatik açılır.
Her 3 turda 1 event gelir. 1-2 tur aktif kalır.

### E01 — Kahve Çılgınlığı
| Özellik | Değer |
|---|---|
| Süre | 2 tur |
| Etki | `food` ve `coffee` işletmeleri +%50 müşteri |
| Kimin İşine Yarar | Kahveci, Burger Zinciri, Organik Çiftlik sahipleri |
> *Yemek sektörü patlıyor.*

### E02 — Ekonomik Kriz
| Özellik | Değer |
|---|---|
| Süre | 2 tur |
| Etki | TÜM işletmelerin geliri -%30. Çalışan maaşları değişmez. |
| Kimin İşine Yarar | Nakit rezervi olanlar hayatta kalır. Rakip daha çok zarar görebilir. |
> *Herkes acı çeker. Ama hazırlıklı olan fırsat yakalar.*

### E03 — Viral Trend
| Özellik | Değer |
|---|---|
| Süre | 1 tur |
| Etki | `marketing` tag'li kartlar bu tur 2x etkili. `trendy` tag'li işletmeler +%50 müşteri. |
| Kimin İşine Yarar | Influencer, Marketing Guru, Reklam Ajansı sahipleri |
> *Marketing heavy strateji burada parlıyor.*

### E04 — Veri Sızıntısı
| Özellik | Değer |
|---|---|
| Süre | 1 tur |
| Etki | `tech` işletmeleri -5 müşteri. `security` upgrade'i varsa etkilenmezsin. |
| Kimin İşine Yarar | Güvenlik Sistemi olanlar bağışık. Tech ağırlıklı rakibi cezalandırır. |
> *Tech odaklıysan dikkat. Güvenlik yatırımı önemli.*

### E05 — Yatırımcı Sezonu
| Özellik | Değer |
|---|---|
| Süre | 1 tur |
| Etki | `finance` ve `investor` tag'li kartlar 2x etkili. |
| Kimin İşine Yarar | Yatırımcı Sunumu, Küçük Yatırım oynayacaklar |
> *Bu turda yatırım kartı oyna = jackpot.*

### E06 — İptal Kültürü
| Özellik | Değer |
|---|---|
| Süre | 1 tur |
| Etki | FBI riski %30'un üstünde olan oyuncu: tüm müşteriler -%40. |
| Kimin İşine Yarar | Temiz oynayan. İllegal rakibi cezalandırır. |
> *Kirli oynuyorsan felaket. Temiz oynuyorsan fırsat.*

---

## DÜKKAN SİSTEMİ

Her turda dükkan 3 rastgele kart gösterir (başlangıç destesi hariç).
Oyuncu para ile satın alabilir. Satın alınan kart desteye girer.

**Dükkan fiyatları = Kart satın alma maliyeti.**
**Kart satma = Satın alma maliyetinin %40'ı.**
**Kart yakma = 💰0 ama deste küçülür (değerli!)**

---

## KART DAĞILIM ÖZETİ

### Nadirliğe göre:
| Nadirlik | Adet | Oran |
|---|---|---|
| Common ★ | 18 | %45 |
| Uncommon ★★ | 13 | %32.5 |
| Rare ★★★ | 9 | %22.5 |

### Tag'lere göre (combo için önemli):
| Tag | Kart Sayısı |
|---|---|
| `food` | 7 |
| `marketing` | 8 |
| `tech` | 4 |
| `finance` | 5 |
| `illegal` | 3 |
| `trendy` | 3 |
| `basic` | 7 |
| `aggressive` | 3 |
| `support` | 2 |
