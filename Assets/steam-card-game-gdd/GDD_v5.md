# GAME DESIGN DOCUMENT
# "Empire of Cards"

> Versiyon: 5.0 | Tarih: 2026-05-21
> Engine: Unity 6 (C#) | Platform: PC (Steam)
> Tur: Business simulation card-strategy hybrid


- Google reklam mi yoksa brosur mu?
- Temizlikci mi yoksa ikinci garson mu?
- Yeni ocak mi yoksa delivery servisi mi?

---

# 5. 8 Ana Stat

## 5.1 Stat Listesi

| Stat | Anlami | Restaurant Karsiligi |
|---|---|---|
| Cash | Anlik para | Kasadaki para |
| Demand | Gelen musteri talebi | Kac kisi dukkanina girmek istiyor |
| Capacity | Karsilayabilme gucu | Kac kisiye hizmet verebiliyorsun |
| Quality | Urun/hizmet kalitesi | Yemegin lezzeti, sunum, tazeligk |
| Rating | Dis puan (Google, Yorum) | Google Puani ⭐ (1-5) |
| Staff Stability | Ekip sagligi | Moral, yorgunluk, sadakat |
| Legal Risk | Yasal risk | Denetim, vergi, hijyen ihlali |
| Market Share | Pazar payi | 100 musteriden kaci sana geliyor |

## 5.2 Matematiksel Zincir (Core Loop)

```
TEDARIK + PERSONEL + MUTFAK => KALITE + KAPASITE
MARKETING + RATING => DEMAND (musteri talebi)

EGER Demand > Capacity:
  => Servis gecikir, siparisler karisir
  => Musteriler bekler, kizgin yorum yazar
  => Rating duser
  => Organik demand duser (kotu yorumlar yuzunden)
  => Rakip market share calar

EGER Demand <= Capacity:
  => Herkes memnun
  => Rating korunur/yukselir
  => Gelir artar
  => Organik demand artar (iyi yorumlar)
  => Market Share buyur
```

## 5.3 Turetilmis Metrikler (Restaurant)

| Metrik | Nasil Hesaplanir | Etkisi |
|---|---|---|
| Malzeme Kalitesi | Tedarik kartlari + Quality | Yemek tadini belirler |
| Servis Hizi | Personel sayisi + Kapasite | Bekleme suresi, yorum etkiler |
| Hijyen | Temizlikci + Tedarik kalitesi + Kapasite yuku | Denetim riski belirler |
| Google Puani | Rating'in gorunur hali (1-5 yildiz) | Organik musterileri belirler |

---

# 6. Musteri Sistemi

## 6.1 Musteri Tipleri

Musteriler kart DEGIL. Board'da hareket eden gorsel elementler.
Oyuncu musteriyi dogrudan kontrol ETMEZ. Dukkanin durumu musterileri CEKER.

| Musteri Tipi | Renk | Neye Bakar | Davranisi |
|---|---|---|---|
| Fiyat Avcisi | Kirmizi | En ucuz menu | Kalite umursamaz, ucuz olsun |
| Gurme | Altin | Kalite > 7 ve Rating > 4.2 | Gelmesi zor ama cok para birakir |
| Sadik Musteri | Mavi | Deneyim ve guven | Kolay terk etmez ama ederse geri gelmez |
| Influencer | Mor | Rating > 4.2 ve gorsel | Gelirse +rating, foto cekar, viral etki |
| Tesadufi | Gri | Rastgele | Marketing ve konum etkiler |
| Aile | Yesil | Temizlik ve cocuk dostu | Hijyen < 5 ise GELMEZ |

## 6.2 Musteri Akis Formulu

```
Musteri = Base Traffic (konum) 
        + Marketing Etkisi
        + Organik (Rating > 3.5 ise pozitif, degilse negatif)
        + Sezon Carpani
        + Ozel Olay Etkisi (rusu, bayram vb)
        - Rakip Cekim Gucu
```

## 6.3 Musteri Memnuniyeti

Her musteri hizmet aldiktan sonra MEMNUNIYET puani alir:

```
Memnuniyet = Kalite * 0.3
           + Servis Hizi * 0.25
           + Hijyen * 0.2
           + Fiyat Uygunlugu * 0.15
           + Ambiyans * 0.1

Memnun (> 7): Iyi yorum, tekrar gelir, arkadasina soyler
Notr (4-7): Yorum yazmaz, belki gelir belki gelmez
Mutsuz (< 4): Kotu yorum, geri GELMEZ, baskalarina soler
```

---

# 7. Zaman ve Ritim Sistemi

## 7.1 Sezon Dongusu

5 sezon, her biri 5 tur:

| Sezon | Turlar | Restaurant Etkisi |
|---|---|---|
| Ilkbahar | 1-5 | Normal trafik, acilis donemi, taze malzeme ucuz |
| Yaz | 6-10 | Trafik +%20, soguk icecek demand, dis mekan bonus |
| Guz | 11-15 | Okul acildi, ogle menusu demand, corbalar |
| Kis | 16-20 | Trafik -%10, sicak yemek demand, bayram piki |
| Ramazan | 21-25 | Iftar menusu MEGA demand, gece trafigi, gunduz yok |

## 7.2 Gunluk Ritim (Her Tur Icinde Otomatik)

Her tur 1 gunu temsil eder. Gun icindeki pikleri oyuncu GOZLEMLER:

```
SABAH:    Az musteri, hazirlik zamani
OGLE:     RUSU! Kapasite testi. Yetisemezsen → kotu yorum
IKINDI:   Sakin, tedarik kararlari islenir
AKSAM:    Ikinci pik. Farkli profil (aile, randevu)
GECE:     Temizlik. Hijyen guncellenir.
```

Bunlar KART degildir. Board'un kendi ritmidir. Tur cozumlenirken bu akis gorsel olarak gosterilir.

## 7.3 Ozel Gun Olaylari

Her 3-5 turda bir OZEL GUN tetiklenir:

| Olay | Tetikleyici | Etki |
|---|---|---|
| Ogle Rusu | Her 3 turda | Demand x1.5, kapasite testi |
| Hafta Sonu | Her 5 turda | Musteri +%50, personel izin ister |
| Bayram | Sezon bazli | Demand x2, fiyat artirabilirsin |
| Yerel Festival | Rastgele | 2 tur boyunca trafik +%30 |
| Yagmurlu Gun | Rastgele | Dis mekan musteri yok, delivery demand artar |

---

# 8. Calisan (Staff) Sistemi

## 8.1 Ise Alim = Kart Secimi

Calisanlar KART olarak gelir. Ise alim karari = hangi karti oynayacagin.

| Kart | Maas | Stat Etkisi | Senaryo |
|---|---|---|---|
| Stajyer Bulasikci | $3/tur | Hijyen +0.2 | Ucuz ama bazen tabak kirar |
| Cylak Garson | $5/tur | Kapasite +0.5 | Siparis karistirir, ogrenme sureci var |
| Deneyimli Garson | $15/tur | Kapasite +1.2, Rating +0.2 | Guvenilir, musteri memnun |
| Komi | $8/tur | Kalite +0.3, Kapasite +0.5 | Hazirlik yapar, sefe yardimci |
| Cylak Asci | $8/tur | Kalite +0.5 | Ogreniyor, bazen yavas |
| Deneyimli Sef | $30/tur | Kalite +2.0, Rating +0.3 | Menuyu degistirir, kalite yukselir |
| Temizlikci | $6/tur | Hijyen +1.5 | Sikici gorunur ama HAYATI |
| Kurye | $10/tur | Delivery Demand +2 | Paket servis acar |
| Kasiyerci | $7/tur | Kapasite +0.3, Hata -0.2 | Hesap karismaz |
| Growth Hacker | $25/tur | Demand +1.5, Legal Risk +3 | Buyutur ama agresif taktik |
| Community Manager | $10/tur | Rating +0.4, Yorum Yonetimi | Kotu yorumlara cevap verir |
| Sube Muduru | $20/tur | Staff Stability +2, Kapasite +1 | Ekibi yonetir, moral arttirir |
| CTO (Legendary) | $50/tur | Hersey +1.5 | Pahali ama inanilmaz etkili |

## 8.2 Calisan Gelisimi (Board Mekanigi, Kart Degil)

Calisanlar board'da zaman gecirdikce KENDILIGINDAN gelisir:

```
Her calisan karti su durumu tasir:
- Maas
- Stat katkisi
- Deneyim barı (0-10, her tur +1)
- Moral barı (0-10)
- Sadakat (0-10)
- Durum metni ("Ogreniyor", "Motivasyon dusuk", "Kidemli")
```

### Terfi Sistemi:

```
Deneyim 5'e ulasinca → TERFI POPUP:

SECENEK A: Terfi ver + Zam ($8 → $15/tur)
  → Stat katkisi artar, Moral +3, Sadakat +2

SECENEK B: Terfi ver ama zam yok
  → Stat katkisi biraz artar, Moral -2
  → 3 tur sonra istifa riski %50

SECENEK C: Terfi yok
  → Moral -4, 2 tur sonra istifa
  → Rakip aninda teklif verir
```

### Terfi Zinciri:

```
Stajyer Bulasikci → (5 tur) → Cylak Garson → (8 tur) → Deneyimli Garson
Cylak Asci → (5 tur) → Deneyimli Asci → (10 tur) → Sef
Komi → (5 tur) → Asci Yardimcisi → (8 tur) → Asci
```

## 8.3 Calisan Dramalari

### Istifa Tetikleyicileri:
- Moral < 3 → Her tur %20 istifa sansi
- Moral < 2 → %50 istifa sansi
- Rakip daha yuksek maas teklif ederse → Ozel popup

### Moral Dusuren Seyler:
- 3 tur ust uste fazla mesai (Demand > Capacity) → Moral -1/tur
- Zam verilmemesi → Moral -2
- Terfi reddi → Moral -3
- Baska calisan istifa etti → Herkesin morali -1

### Moral Arttiran Seyler:
- Zamaninda maas → Moral +0.5/tur
- Zam → Moral +2
- Bonus → Moral +3
- Iyi calisma ortami (Hijyen > 7, Staff Stability > 7) → Moral +0.5/tur

### Crunch Mode (Risk Karti):

```
CRUNCH MODE oynarsin: Tum personel bu tur x2 calisir
  → Kapasite x2 (1 tur)
  → Staff Stability -3
  → Her calisanin Moral -2
  → 3 tur ust uste crunch → %60 BURNOUT WAVE
     → Tum calisanlarin morali 2'ye duser
     → En dusuk moralli calisan ISTIFA EDER
```

---

# 9. Tedarik (Supplier) Sistemi

## 9.1 Tedarik Kartlari

| Kart | Maliyet | Kalite Etkisi | Hijyen Etkisi | Risk | Senaryo |
|---|---|---|---|---|---|
| Hal'den Taze Et | $20/tur | +1.5 | +0.5 | Yok | Pahali ama guvenli, musteri fark ediyor |
| Ucuz Donmus Et | $5/tur | +0.3 | -0.3 | Dusuk | Karli ama lezzet olumsuz, yorum riski |
| Karaborsa Et | $2/tur | +0.5 | -0.5 | Legal +8 | Nereden geldigi belirsiz, denetim gelirse YANDIN |
| Organik Tedarikci | $25/tur | +2.0 | +0.5 | Yok | Premium fiyat koyabilirsin, marj dusuk |
| Mahalle Firinci | $8/tur | +0.8 | +0.3 | Yok | Her gun taze ekmek, guvenilir |
| Toptan Ithal Gida | $3/tur | -0.5 | -0.2 | Dusuk | Cok ucuz, cok urun ama lezzet berbat |
| Yerel Manav | $10/tur | +1.0 | +0.5 | Yok | Taze sebze, mevsimlik menu imkani |
| Icecek Anlasmasi | $12/tur | +0.3 | 0 | Yok | Sabit fiyat, marj iyi |

## 9.2 Tedarik Dramalari

### Kalitesiz Tedarikci Zinciri:

```
Karaborsa Et + Toptan Ithal Gida sectin ($5/tur toplam, super ucuz!)
  ↓
Musteri: "Bu doner niye boyle kokuyor?" → Rating -0.3/tur
  ↓
5. turda: HIJYEN DENETIMI krizi tetiklendi!
  ↓
Denetim sonucu: Kalitesiz malzeme tespit edildi
  ↓
SECENEK:
  A) Avukat + Tedarikci Degis ($50) → temiz baslangic
  B) Rusvet ($20, Legal Risk +15) → bu sefer kurtulursun ama...
  C) Dukkan 2 tur kapali → para kaybi ama legal temiz
  
B secersen: 3 tur sonra → "BASKI! Rusvet ortaya cikti!"
  → Legal Risk +25, Rating -1.0, Para cezasi $100
```

### Tedarikci Biraktirma:

```
Organik Tedarikci var. Pahali ama kalite iyi.
Turn 12: Rakip ayni tedarikciye 2x fiyat teklif ediyor.
  ↓
POPUP: "Tedarikciniz ayrilmak istiyor!"
  A) Fiyat artir ($25 → $35/tur) → kalir
  B) Birak gitsin → kalite bir anda duser, yeni tedarikci bul
  C) Uzun vadeli kontrat ($50 tek sefer) → 10 tur garantili
```

---

# 10. Hijyen Sistemi

## 10.1 Hijyen Hesaplama

Hijyen gorunmez bir STAT ama sonuclari cok gorunur:

```
Hijyen = Baz (5.0)
       + Temizlikci Karti (+1.5 her temizlikci)
       + Tedarik Kalitesi (+0.5 kaliteli, -0.5 ucuz)
       - Kapasite Yuku (-0.3 her Demand > Capacity tur)
       - Zaman Bozulmasi (-0.1/tur dogal dusus)
```

## 10.2 Hijyen Esikleri

| Hijyen Skoru | Durum | Sonuc |
|---|---|---|
| 8-10 | PARLAK | Rating +0.2/tur, Aile musteriler gelir |
| 5-7 | NORMAL | Etki yok |
| 3-5 | KIRLI | "Pis mutfak" yorumlari baslar (Rating -0.2/tur) |
| 2-3 | TEHLIKELI | HIJYEN DENETIMI krizi %30/tur |
| 0-2 | KAPATIS | OTOMATIK: 3 tur kapali + $500 ceza |

## 10.3 Hijyen Dramalari

```
Temizlikci ALMADIN (para tasarrufu!)
  ↓ Tur 1-3: Fark yok
  ↓ Tur 4: Hijyen 5 → 4.2 (dogal dusus)
  ↓ Tur 6: Hijyen 3.8 → "Tuvaleti pis" yorumu (Rating -0.3)
  ↓ Tur 8: Hijyen 3.0 → DENETIM! (Legal Risk +15)
  ↓ Tur 9: KAPATILDI (3 tur para yok, musteri yok)

Tum bunlar 1 KART almamak yuzunden.
"Temizlikci" en sikici kart ama en HAYATI kart.
Bu "boring but essential" karari oyunu derinlestirir.
```

---

# 11. Kart Aileleri (Restaurant Referans)

## 11.1 MUTFAK / OPERATION Kartlari

| Kart | Maliyet | Etki | Senaryo |
|---|---|---|---|
| Kucuk Dukkan | $80 | Kapasite +2, Kalite +1 | Ilk mekani acar, hersey kucuk |
| Ekstra Masa | $40 | Kapasite +1.5 | Daha cok musteri oturur |
| Izgara Istasyonu | $60 | Kalite +1.2, Kapasite +1 | Menuye et yemekleri eklenir |
| Paket Servis Istasyonu | $50 | Delivery Demand +2 | Yeni gelir kanali, kurye lazim |
| Tatli Tezgahi | $35 | Kalite +0.5, Gelir +$10/tur | Tatli menusu acar, marj iyi |
| Mutfak Renovasyonu | $100 | Kalite +2, Kapasite +2 | Pahali ama herseyi iyilestirir |
| Dis Mekan (Teras) | $70 | Kapasite +3 (yaz), +0 (kis) | Mevsimlik kapasite, yaz harika |
| Franchise Mutfak | $150 | Kapasite x2 | Cag 4 karti, ikinci sube hissi |

## 11.2 PERSONEL / STAFF Kartlari

(Bolum 8.1'de detayli tanimlandi)

## 11.3 MARKETING Kartlari

| Kart | Maliyet | Etki | Senaryo |
|---|---|---|---|
| Brosur Dağıtımı | $15 | Demand +1.0 | Ucuz, yerel, etkisi sinirli |
| Google Isletme Profili | $20 (tek) | Organik +%15 | Maps'te gorunursun, uzun vadeli |
| Google Reklam | $25/tur | Demand +2.5 | Para doktukce gelir, durdurunca kesilir |
| Instagram Sayfasi | $10 (tek) | Rating +0.2, Demand +0.5/tur | Gorsel icerik, yemek fotolari |
| TikTok Influencer | $60 (tek) | Demand +4 (1 tur), sonra -2 | Viral olur ama kalici degil |
| Yemek App Anlasmasi | $30/tur | Delivery Demand +3, Komisyon -%15 | Yeni kanal ama komisyon keser |
| Yerel Etkinlik Sponsoru | $40 (tek) | Demand +2 (3 tur) | Festival, spor etkinligi |
| Sahte Yorum Satin Al | $15 | Rating +0.8 | HIZLI ama ban riski %25 |
| Sadakat Karti Sistemi | $25 (tek) | Sadik Musteri +%30 | 5 alisveriste 1 bedava |
| Referans Sistemi | $30 (tek) | Demand +0.5/tur (compound) | Yavas ama snowball etkisi |

## 11.4 TEDARIK / SUPPLIER Kartlari

(Bolum 9.1'de detayli tanimlandi)

## 11.5 RISK Kartlari (ucuz/bedava ama tehlikeli)

| Kart | Maliyet | Kisa Vade | Uzun Vade Riski |
|---|---|---|---|
| Sahte Yorum | $15 | Rating +0.8 | Store ban %25, tum organik sifirlanir |
| Sigortasiz Calisan | $0 (maas -40%) | Personel maliyeti duser | Legal Risk +5/tur, denetimde agir ceza |
| Karaborsa Malzeme | $2/tur | Maliyet duser | Kalite -0.5, Hijyen -0.5, denetim riski |
| Vergi Kacirma | $0 (vergi yok) | Net gelir +%30 | Audit gelirse $200 ceza + Legal Risk +20 |
| Crunch Mode | $0 | Kapasite x2 (1 tur) | Staff Stability -3, Burnout %60 |
| Rusvet (Denetim) | $20 | Denetim gecirilir | Ortaya cikarsa: Legal Risk +25, Rating -1 |
| Rakipten Sef Cal | $30 | Kalite +1.5 anlik | Rakip kizar, agresif saldiri baslar |
| Sahte Sertifika | $10 | Guven +1 | Tespit edilirse KAPATIS |
| Cocuk Isci | $0 | Ekstra el | Legal Risk +10/tur, skandal riski |
| Fiyat Manipulasyonu | $0 | Gelir +%20 | Musteri fark ederse Rating -1.5 |

## 11.6 REAKSIYON Kartlari (kriz gelince oynamalik)

| Kart | Maliyet | Ne Cozer | Senaryo |
|---|---|---|---|
| Acil Temizlik | $15 | Hijyen krizi | Profesyonel ekip gelir, 1 turda hijyen +3 |
| Ozur Kampanyasi | $25 | Rating krizi | Ucretsiz ikram + ozur, kizgin musteriler yavaslar |
| Avukat Tut | $50 | Legal kriz | Pahali ama legal risk sifirlar |
| Tedarikci Degistir | $20 | Kalite krizi | Mevcut tedarikciyi at, yenisini bul |
| Acil Ise Alim | $25 | Personel krizi | Rastgele calisan gelir, 1 tur stabilize |
| Menu Sadelist | $10 | Kapasite krizi | Menu kucultur, kalite yukselir, musteri azalir |
| Ucretsiz Ikram | $15 | Rating krizi | 1 tur boyunca herkeye tatli, Rating +0.5 |
| Ustay Degistir | $30 | Kalite krizi | Mevcut sef gider, yeni guclu sef gelir |
| Denetim Hazirlik | $20 | Legal risk yuksek | Legal Risk -10, Hijyen +2 (1 tur) |
| Rakip Analiz | $15 | Rakip baskisi | 3 tur boyunca rakibin tum kartlarini gor |

## 11.7 KRIZ Kartlari (otomatik tetiklenir)

| Kriz | Tetikleyici | Etki | Secenekler |
|---|---|---|---|
| Kotu Yorum Patlamasi | Rating < 3.5 + 2 tur update yok | Rating -1.0, Organik -%50 | A) Ozur kampanyasi ($25) B) Sahte yorum ($15+risk) |
| Hijyen Denetimi | Hijyen < 3 veya Legal Risk > 50 | Legal Risk +15, Kapanma riski | A) Temizlik+Avukat ($65) B) Rusvet ($20+risk) C) 2 tur kapat |
| Sef Istifa Etti | Sef morali < 3 veya rakip teklif | Kalite anlik -2, Mutfak bos | A) Counter-offer ($30) B) Acil ise alim ($25) C) Crunch |
| Tedarik Krizi | Tedarikci birakti veya mevsimsel | Malzeme yok, kalite -1.5 | A) Yeni tedarikci ($20) B) Ucuz alternatif ($5+kalite) |
| Kira Artti | Turn 10+ otomatik | Gider +$30/tur | A) Ode B) Yer degistir ($100, 2 tur kapali) C) Pazarlik |
| Viral Kotu Video | Risk karti oynadiysan | Rating -0.8, Demand -3 (1 tur) | A) PR Kriz ($30) B) Gormezden gel C) Espriyle yaklas ($5) |
| Gida Zehirlenmesi | Kalite < 3 + Hijyen < 4 | AGIR: Rating -2, Legal Risk +20, 1 tur kapali | A) Avukat+Tedarikci degis ($70) B) Sessiz kal (DAHA kotu) |
| Rakip Fiyat Kirdi | Rakip agresif ise | Fiyat Avcisi musteriler rakibe gitti | A) Sen de kir (marj duser) B) Kalite oyna C) Marketing artir |
| Calisan Hirsizligi | Staff Stability < 4 | Cash -$50, guven kaybi | A) Kovma+Yenisini al ($15) B) Gormezden gel (devam eder) |
| Sokak Calismasi | Rastgele | 3 tur: trafik -%40 | A) Delivery'ye yogunlas B) Indirim yap C) Bekle |
| Pandemi | Turn 15+ %10 sans | 5 tur: Kapasite %50, Delivery +%200 | A) Full delivery pivot B) Kucul+hayatta kal C) Kapat |

---

# 12. Senaryo Zincirleri

## 12.1 "Ucuz Yol" Zinciri

```
Stajyer + Ucuz Garson aldin ($8/tur toplam, tasarruf!)
  ↓
Siparis karisiyor, tabaklar kirli → Kalite -0.5
  ↓
Google yorumu: "Yemek soguk geldi, bardak pisti" → Rating 4.1→3.6
  ↓
Organik musteri %30 dustu
  ↓
Para bitti → Crunch Mode oynadin (bedava kapasite)
  ↓
Staff Stability dustu → Asci burnout
  ↓
Asci ISTIFA ETTI → Mutfak bos
  ↓
2 tur boyunca yemek cikaramadin → Rating 2.8
  ↓
SECENEK:
  A) Acil hire + Kaliteli tedarik → $55 ama toparlanirsin
  B) Sahte yorum + Ucuz isci → $20 ama zincir devam eder
  C) Dukkan sat → Run biter, kucuk exit
```

## 12.2 "Trafik Patlamasi" Zinciri

```
Google Reklam + TikTok Influencer oynadin → Demand +8!
  ↓
AMA: 2 garson + 1 asci = kapasite 5
  ↓
8 musteri geldi, 5'ine hizmet, 3'u bekledi
  ↓
Bekleyenler kizgin yorum yazdi → Rating -0.4
  ↓
Asci fazla mesai → Staff Stability -1
  ↓
3. tur ust uste → Asci ISTIFA
  ↓
SECENEK:
  A) Garson+Asci al (pahali ama kapasite artar)
  B) Marketing durdur (demand dusur, stabilize et)
  C) Paket Servis Kapat (demand dusur ama mevcut musteriye odaklan)
```

## 12.3 "Dark Side" Zinciri

```
Sahte yorum satin aldin (Rating 3.5→4.3)
  ↓
Musteri artti, para geldi
  ↓
Sigortasiz calisan aldin (maas -%40!)
  ↓
Vergi kacirdin (net gelir +%30!)
  ↓
Karaborsa et aldin (maliyet dusuk!)
  ↓
PATLAMA: Hijyen Denetimi + Vergi Denetimi + Sigorta Kontrolu
  ↓
Legal Risk 85 → Avukat $50 + $200 ceza
  ↓
Sahte yorumlar tespit edildi → Rating 4.3→2.1
  ↓
KAPATIS: 3 tur kapalı + para cezasi
  ↓
SECENEK:
  A) Avukat+Temiz baslangic ($150) → Zor ama mumkun
  B) Iflas → Run biter
```

---

# 13. Rakip Sistemi

## 13.1 Ayni Sektor Kurali

Rakip oyuncuyla AYNI sektorde. Ayni musteri havuzu icin savasirsiniz.

## 13.2 Rakip Davranis Aileleri

| Strateji | Davranisi | Sana Etkisi |
|---|---|---|
| Agresif Marketing | Brosur, reklam, indirim | Senin musterini calar |
| Premium Kalite | Iyi sef, kaliteli malzeme | Gurme musteriler ona gider |
| Ucuz Genisleme | Ucuz isci, ucuz malzeme | Fiyat avcilari ona gider |
| Defansif | Sadik musteri odakli | Senin saldirini absorbe eder |
| Kirli Oyun | Sahte yorum, staff poach | Sana zarar verir |

## 13.3 Rakip Gorsel Ipuclari

Rakibin tam board'unu goremezsin ama:
- Her tur 1 aksiyon bildirimi: "Rakip yeni sef aldi"
- Rating'i ve musteri sayisi gorunur
- Hangi slot tiplerini doldurdugu icon olarak gorunur
- Bazen taunt mesaji: "Senin musterilerin bana geliyor!"

## 13.4 Rakip Baski Ornekleri (Restaurant)

- Fiyat kirma: Senin fiyat avcisi musterilerin kacar
- Yorum savasi: Rakip iyi yorum aldikca senin organic duser
- Sef calmak: Senin en iyi calisanina teklif verir
- Delivery baskisi: Paket serviste senin onune gecer
- Yerel etkinlik: Rakip sponsor oldu, 3 tur demand senden kacar

---

# 14. Ekonomi Dongusu

## 14.1 Gelir Formulu

```
Brut Gelir = Hizmet Verilen Musteri × Ortalama Siparis Tutari
Siparis Tutari = Menu Fiyati × Kalite Carpani × Musteri Tipi Carpani

Gider = Personel Maaslari + Tedarik Maliyeti + Kira + Marketing
      + Vergi + Kredi Faizi

Net Kar = Brut Gelir - Gider
```

## 14.2 Fiyatlandirma Karari

Oyuncu menu fiyatini KART ile belirler:

| Kart | Fiyat Seviyesi | Etki |
|---|---|---|
| Ekonomik Menu | Dusuk ($) | Fiyat avcilari gelir, marj dusuk, hacim yuksek |
| Standart Menu | Orta ($$) | Dengeli musteri profili |
| Premium Menu | Yuksek ($$$) | Sadece gurme gelir, marj yuksek, hacim dusuk |
| Sezonluk Menu | Degisken | Mevsime gore bonus, hazirlık gerektirir |

## 14.3 Kredi Sistemi

| Kredi | Tutar | Faiz | Sure | Kosul |
|---|---|---|---|---|
| Mikro Kredi | $100 | %5/tur | 5 tur | Her zaman |
| Is Kredisi | $300 | %8/tur | 8 tur | Cag 2+ |
| Buyuk Yatirim | $800 | %12/tur | 12 tur | Cag 3+ |
| Acil Nakit | $50 | %15/tur | 3 tur | Her zaman, acil |

---

# 15. 4 Cag Sistemi

## 15.1 Cag 1: GARAJ (Tur 1-6)

> "Kucuk bir dukkan, buyuk hayaller"

- 2 aksiyon/tur, 9 slot
- Baslangic: Kucuk dukkan + $300 + 1 cylak asci
- Hedef: Ilk musterileri cek, ayakta kal
- Tehlike: Para bitmesi, motivasyon kaybi
- Musteri: Sadece tesadufi + fiyat avcisi
- Rakip: Pasif, kendini kuruyor

## 15.2 Cag 2: BUYUME (Tur 7-13)

> "Musteriler geliyor, simdi yetismek lazim"

- 3 aksiyon/tur, 14 slot (+5 yeni slot acilir)
- Hedef: Kapasite yetistir, kaliteyi koru, rating yukset
- Tehlike: Demand > Capacity krizi, ilk kotu yorumlar
- Musteri: Gurme ve sadik musteriler baslar
- Rakip: Ilk agresif hamle, fiyat kirma veya marketing

## 15.3 Cag 3: OLCEK (Tur 14-19)

> "Artik ciddi bir isletmesin"

- 4 aksiyon/tur, 18 slot (+4 yeni slot)
- Hedef: Market share %40+, ikinci gelir kanali (delivery veya sube)
- Tehlike: Calisan burnout, tedarik krizi, rakip agresif
- Musteri: Influencer ve aile musteriler gelir
- Rakip: Cok agresif: sef calma, fiyat savasi, marketing blitz

## 15.4 Cag 4: HAKIMIYET (Tur 20-25)

> "Ya hukmet ya yikil"

- 5 aksiyon/tur, 21 slot (+3 yeni slot)
- Hedef: %60 market share veya max valuation ile exit
- Tehlike: Regulasyon, pandemi, rakip son hamle
- Musteri: Tum tipler aktif
- Rakip: Hayatta kalma modunda veya olum kalimindan saldiriyor

---

# 16. Tur Akisi (7 Faz)

## 16.1 Fazlar (toplam ~1 dakika)

| Faz | Sure | Oyuncu Ne Yapar |
|---|---|---|
| 1. DRAW | 3 sn | 5 kart cekilir (board state'e gore biased) |
| 2. PLANNING | 10 sn | Dashboard okur: "Hijyen dusuk, kapasite yetersiz" |
| 3. PLAY | 30 sn | 2-5 karti (caga gore) fiziksel olarak board'a koyar |
| 4. RESOLVE | 15 sn | Sistem hesaplar, animasyonla sonuc gosterilir |
| 5. CRISIS | 5 sn | Kriz varsa tetiklenir, tepki secenekleri cikar |
| 6. RIVAL | 5 sn | Rakibin hamlesi gosterilir |
| 7. MARKET UPDATE | 5 sn | Siralama guncellenir, sezon etkisi |

## 16.2 Resolve Detay (Faz 4)

Tur cozulmesi su sirada yapilir:

```
1. Mutfak + Personel + Tedarik → Kalite + Kapasite hesapla
2. Hijyen guncelle (temizlikci, kapasite yuku, tedarik etkisi)
3. Marketing → Demand hesapla
4. Rating guncelle (kalite, servis hizi, hijyen, yorumlar)
5. Musteri akisi: kim geldi, kime hizmet verildi, kim bekledi
6. Gelir/Gider hesapla, vergi ve kredi faizi uygula
7. Staff moral/deneyim/sadakat guncelle
8. Market Share kaydir
9. Calisan terfi kontrol, istifa kontrol
10. Sonuclari animasyonla goster
```

## 16.3 Draw Bias (Kart Cekme Yanliligi)

Kart cekimi tamamen rastgele DEGIL. Board durumuna gore bias uygulanir:

| Board Durumu | Bias | Mantik |
|---|---|---|
| Kapasite < Demand | Personel + Mutfak kartlari oncelikli | Yetisemiyorsun, eleman/ekipman lazim |
| Rating < 3.5 | Reaksiyon + Kalite kartlari | Toparlaman lazim |
| Hijyen < 4 | Temizlik + Tedarik kartlari | Hijyen acil |
| Legal Risk > 40 | Savunma + Avukat kartlari | Legal baski var |
| Cash < $50 | Ucuz kartlar + Kredi | Paran bitiyor |
| Staff Stability < 4 | Moral boost + Yedek personel | Ekip dagilliyor |

---

# 17. Kazanma ve Kaybetme

## 17.1 Kazanma Kosullari (25 tur icinde)

- %60 Market Share VEYA
- Hedef Valuation'a ulasma

## 17.2 Kaybetme Kosullari

| Kosul | Tetikleyici | Detay |
|---|---|---|
| Iflas | Cash <= 0 | 3 tur ust uste negatif gelir |
| Itibar Cokusu | Rating <= 1.5 (3+ tur) | Kimse gelmiyor, toparlanamaz |
| Legal Felaket | Legal Risk >= 90 | Denetim kapatiyor |
| Rakip Hakimiyeti | Rakip Market Share >= %70 | Ezici usunluk |
| Kapatis | Hijyen < 1 veya 3 tur ust uste kapali | Belediye kapatma |

## 17.3 Run Sonu Skor

| Eksen | Puan |
|---|---|
| Market Share | %1 = 10 puan |
| Final Cash | $100 = 5 puan |
| Rating | ⭐ x 100 puan |
| Cozulen Kriz Sayisi | x 50 puan |
| Ulasilan Cag | Cag x 200 puan |
| Temiz Oyun (0 risk karti) | +500 bonus |
| Hizli Finish (< 20 tur) | +300 bonus |

Not: Skor = Grade (S/A/B/C/D/F) + Exit Valuation

---

# 18. Meta-Progression (Roguelike Loop)

## 18.1 Run Dongusu

```
RUN 1: "Omer'in Doner" (Restaurant)
  Garaj → Buyume → Olcek → Cikis ($500K)
  → Unlock: "Serial Entrepreneur" perk, +%20 starting cash
  → Unlock: Tech App sektoru

RUN 2: "QuickChat" (Tech App)
  $500K birikim ile basla, daha guclu rakip
  → Cikis ($5M) → HOLDING KURULDU

RUN 3+: Holding modunda multi-isletme
  2-3 isletme ayni anda yonet
  Sinerji kartlari: Ortak Tedarik, Cross-Marketing
  → $50M+ → Yeni sektorler + Legendary kartlar
```

## 18.2 Her Run Arasi Tasinan Seyler

| Tasinir | Tasinmaz |
|---|---|
| Birikmis sermaye (%30) | Board durumu (sifirlanir) |
| Acilmis sektorler | Calisanlar (sifirlanir) |
| Perk'ler | Aktif krizler |
| Legendary kartlar (nadir) | Tedarik anlasmalar |
| Deneyim puani | Rating (sifirlanir) |

## 18.3 Perk Sistemi

| Perk | Unlock Kosulu | Etki |
|---|---|---|
| Serial Entrepreneur | Ilk basarili exit | +%20 starting cash |
| Staff Magnet | 3 calisan max level'a ulassin | Personel kartlari %20 ucuz |
| Supplier Network | 5 farkli tedarikci kullan | Tedarik kartlari 1 tur erken gelir |
| Crisis Manager | 10 kriz basarili coz | Kriz damage -%30 |
| Marketing Guru | %50+ market share 3 tur tut | Marketing kartlari %50 daha etkili |
| Clean Player | Risk karti kullanmadan kazan | Baslangic Legal Risk = 0 (normalde 5) |

## 18.4 Holding Modu

| Startup Modu | Holding Modu |
|---|---|
| 1 isletme yonet | 2-3 isletme ayni anda |
| Basit board | Multi-board (tab ile gec) |
| Bireysel krizler | Portfolio krizleri |
| Kucuk kredi | M&A, buyuk yatirim |
| Rakip = 1 isletme | Rakip = baska holding |
| 25 tur | 30 tur |

---

# 19. Gorsel Dil ve His

## 19.1 Board Hissi

- Kart oyunu masasi DEGIL
- Yukari bakisli 3D dukkan ici (izometrik veya kus bakisi)
- Kartlar fiziksel obje gibi: surukle, birak, yerine oturur
- Calisanlar slot'ta idle animasyon yapar
- Musteriler sokakta yurur, dukkanina girer cikar
- Mutfakta duman, tabak sesleri
- Rating degisince yildizlar animasyonla degisir

## 19.2 Kart Hissi

- Kartlar 3D, elde fan seklinde durur
- Suruklerken hafif buyur, birakinca yerine oturur
- Kirmizi kartlar (risk) biraz titrer
- Yesil kartlar (cozum) hafif parlar
- Legendary kartlar altin cerceve + parlama

## 19.3 Kriz Hissi

- Kriz gelince ekran hafif kirmiziya doner
- Alarm sesi + titresim
- Kriz karti masanin ortasina DUSER
- 2 secenek karti sagda solda belirir
- Secim yapinca sonuc animasyonu

## 19.4 Tur Sonu Hissi

- Musteriler geliyor animasyonu (yururler)
- Para sayaci yukari/asagi ticker
- Rating yildizlari animasyonla degisir
- Market share bar kayar
- Rakibin hamlesi kisa bildirimle gosterilir

---

# 20. Ses Tasarimi

## 20.1 Ambient

- GARAJ: Sessiz, laptop sesi, kahve makinesi
- BUYUME: Mutfak sesleri, musteri muhabbeti, tabak cakinisi
- OLCEK: Yogun mutfak, telefon, siparis bagirma
- HAKIMIYET: Tam restoran ambiyans, muzik, kahkaha

## 20.2 Efektler

- Kart oynama: satisfying "tok" sesi
- Kriz: alarm zili
- Musteri geldi: kapi zili
- Para kazandi: kasa sesi
- Rating yukseldi: yildiz bling sesi
- Calisan istifa: uzgun nota

---

# 21. Teknik Mimari

## 21.1 Korunacak Kararlar

- Bootstrap → WiringService → EventBus pattern
- Singleton GameManager
- 3D kart sistemi (Card3D, Hand3D, Board3D, SlotZone3D)
- Polling-based turn phases (7 faz)
- CardData ScriptableObject

## 21.2 Namespace Yapisi

```
EmpireOfCards.Core            - GameManager, TurnManager, EventBus
EmpireOfCards.Core.TurnPhases - 7 faz (Draw, Planning, Play, Resolve, Crisis, Rival, Market)
EmpireOfCards.Gameplay        - EconomyManager, BoardManager, DeckManager
EmpireOfCards.Gameplay.Staff  - StaffStateSystem, terfi, moral
EmpireOfCards.Gameplay.Economy - SalarySystem, CreditSystem, TaxSystem
EmpireOfCards.Gameplay.Rival  - RivalAI, RivalDecisionTree
EmpireOfCards.Gameplay.Customer - CustomerSystem, musteri tipleri, memnuniyet
EmpireOfCards.Data            - CardData, SectorProfile, CustomerProfile
EmpireOfCards.Bootstrap       - WiringService, ManagerFactory, ContentFactory
EmpireOfCards.UI              - UIManager, TopBar, Popups, Dashboard
EmpireOfCards.World           - Card3D, Hand3D, Board3D, SlotZone3D
EmpireOfCards.Save            - SaveManager, MetaProgression
EmpireOfCards.Audio           - AudioManager
EmpireOfCards.VFX             - VFXManager
```

## 21.3 Yeni Sistemler (v4'te yoktu)

- CustomerSystem: Musteri tipleri, memnuniyet, akis
- HygieneSystem: Hijyen hesaplama, denetim tetikleme
- StaffProgressionSystem: Terfi, deneyim, moral
- PricingSystem: Menu fiyatlama, musteri profili etkisi
- CrisisChainSystem: Zincirleme kriz tetikleme
- MetaProgressionSystem: Run arasi birikim, perk, unlock

---

# 22. MVP Kapsami ve Oncelik

## 22.1 MVP = Restaurant, Eksiksiz

| Oncelik | Sistem | Detay |
|---|---|---|
| P0 | Board + Slot + Kart yerlestirme | Temel oynanis |
| P0 | 7 faz tur dongusu | Oyun akisi |
| P0 | 8 stat + resolve zinciri | Matematiksel cekirdek |
| P0 | 50+ Restaurant karti | Icerik |
| P0 | Kriz + Reaksiyon sistemi | Drama |
| P0 | Rakip AI (temel) | Rekabet |
| P0 | Kazanma/Kaybetme | Hedef |
| P1 | Musteri tipleri + memnuniyet | Derinlik |
| P1 | Calisan gelisimi + terfi | Bag kurma |
| P1 | Hijyen sistemi | Gerilim |
| P1 | Sezon + zaman olaylari | Ritim |
| P1 | 4 Cag slot genislemesi | Ilerleme hissi |
| P2 | Meta-progression (basit) | Tekrar oynama |
| P2 | Skor sistemi | Basari hissi |
| P3 | Ikinci sektor (Tech App) | Icerik genislemesi |
| P3 | Holding modu | Endgame |

## 22.2 Post-MVP Roadmap

- Update 1: Tech App sektoru + meta-progression
- Update 2: Moda/Giyim sektoru + sezon mekamigi
- Update 3: Market/Bakkal + Holding modu
- Update 4: Fintech + IPO mekamigi
- Update 5: Multiplayer? (cok uzun vade)

---

# 23. Kabul Kriterleri

## 23.1 MVP Tamamlanma Kriterleri

- [ ] Oyuncu dukkan adi girer, oyun baslar
- [ ] 5 kart ceker, 2-5 karti board'a koyar (caga gore)
- [ ] Kartlar fiziksel olarak mutfak/salon/depo'ya oturur
- [ ] Tur sonunda: kalite, kapasite, demand, rating hesaplanir
- [ ] Musteriler sokakta gorsel olarak gorunur
- [ ] Kriz tetiklenir, secenek sunulur
- [ ] Rakip her tur 1 hamle yapar
- [ ] 25 turda kazanma veya kaybetme
- [ ] Skor ekrani + grade
- [ ] En az 10 farkli kriz senaryosu
- [ ] En az 50 kart
- [ ] Calisan istifa edebilir
- [ ] Hijyen denetimi tetiklenebilir
- [ ] Sezon degisimi demand'i etkiler

## 23.2 His Kriterleri

- [ ] Oyuncu "kart oynuyorum" degil "dukkan yonetiyorum" hisseder
- [ ] Her kayip "haksizlik" degil "benim hatam" hisseder
- [ ] Her kriz en az 2 cozum sunar (ucuz+riskli vs pahali+temiz)
- [ ] Rakip gercekci hisseder, oyuncu stratejisini okuyabilir
- [ ] 1 run ~25 dakika surer
- [ ] Oyuncu bittikten sonra "bir daha oynayayim" demeli
