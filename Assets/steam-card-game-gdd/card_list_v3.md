# CARD LIST v3.0 — Empire of Cards
> Versiyon: 3.0 | Tarih: 2026-05-20
> Toplam: 75 venture-specific + 20 genel = 95 kart
> Designed by: Game Designer Agent

---

## TASARIM ILKELERI

**Kart denge felsefesi:**
- Her kart duruma gore iyi veya kotu. "Her zaman oyna" kart yok.
- Operation kartlari gelir uretir ama maliyet getirir. Bos kapasite = zarar.
- Staff kartlari bonus verir ama maas yukler. Fazla staff = kar erir.
- Marketing kartlari musteri cekar ama operasyon hazir degilse kotu yorum uretir.
- Supplier kartlari kalite-maliyet dengesi kurar. 2 slot = zor secim.
- Event/Crisis kartlari TempEffect slotuna duser, negatif baski olusturur.

**Rarity dagilimi (her 15 kartlik venture havuzu icin):**
- Common: 6 kart (temel operasyon, isler her zaman)
- Uncommon: 5 kart (daha guclü, stratejik secim)
- Rare: 4 kart (oyun degistirici, combo parcasi)

**Slot uyumu:** Her kart GDD v3.0 Section 4 slot sistemine uyar:
- Operation (4 baslangic, max 8) = Business tipi kartlar
- Staff (5 baslangic, max 10) = Employee tipi kartlar
- Marketing (3 baslangic, max 5) = Business tipi kartlar (marketing alt turu)
- Supplier (2 baslangic, max 4) = Business tipi kartlar (supplier alt turu)
- TempEffect (3 sabit) = Event tipi kartlar

---

# FAST FOOD KARTLARI (FFC01-FFC15)

**Venture kimliği:** Yuksek hacim, dusuk marj. Hiz-kalite dengesi. Teslimat komisyonu riski. Fire tehlikesi.

| ID | KART ADI | TIP | SLOT | RARITY | VentureType | OZELLIK |
|---|---|---|---|---|---|---|
| FFC01 | Burger Station | Business | Operation | Common | FastFood | Her tur +8 gelir, +3 musteri kapasitesi; maas ve hammadde gideri -5/tur |
| FFC02 | Doner Spit | Business | Operation | Common | FastFood | Her tur +6 gelir, +4 musteri kapasitesi; dusuk maliyet ama kalite skoru +0 |
| FFC03 | Drive-Thru Lane | Business | Operation | Uncommon | FastFood | Servis hizi +2, musteri kapasitesi +5; kira maliyeti +8/tur |
| FFC04 | Ghost Kitchen | Business | Operation | Rare | FastFood | Her tur +10 gelir, sifir kira ama platform komisyonu -%25 gelir; fiziksel musteri yok |
| FFC05 | Line Cook | Employee | Staff | Common | FastFood | Kalite +1, servis hizi +1; maas -12/tur |
| FFC06 | Head Chef | Employee | Staff | Uncommon | FastFood | Kalite +3, fire -%20; maas -25/tur, rakip tarafindan calinma riski |
| FFC07 | Delivery Rider | Employee | Staff | Common | FastFood | Teslimat geliri +15/tur; maas -10/tur, sigorta yoksa yasal risk +10 |
| FFC08 | Night Shift Crew | Employee | Staff | Uncommon | FastFood | Gece musteri +4/tur; moral -1 her 3 turda, fazla mesai sinerjisi |
| FFC09 | Neon Signboard | Business | Marketing | Common | FastFood | Platform puani +0.2, yaya musterisi +2/tur |
| FFC10 | Delivery App Listing | Business | Marketing | Uncommon | FastFood | Yeni musteri kanali +5/tur; platform komisyonu -%25 gelir, "Platform Zammi" eventine acik |
| FFC11 | Sidewalk Flyer | Business | Marketing | Common | FastFood | Bu tur musteri +3, maliyet dusuk; tek seferlik etki, surekli oynamak gerekir |
| FFC12 | Bulk Meat Deal | Business | Supplier | Common | FastFood | Hammadde maliyeti -%15; kalite -1, 4+ tur kullanilirsa "Kalite Krizi" event tetikler |
| FFC13 | Organic Supplier | Business | Supplier | Rare | FastFood | Kalite +2, musteri sadakati +1/tur; maliyet +20/tur |
| FFC14 | Health Inspection | Event | TempEffect | Uncommon | FastFood | 1 tur gelir yok (isletme kapali); hijyen sertifikasi varsa hasar sifir |
| FFC15 | Grease Fire | Event | TempEffect | Rare | FastFood | Mutfak 2 tur kapali, onarim -80; Operation slotu gecici olarak kullanilmaz |

### FAST FOOD COMBO'LARI

**Combo 1: "Franchise Formula"**
- Head Chef (FFC06) + Organic Supplier (FFC13) + Drive-Thru Lane (FFC03)
- Etki: Gelir +%30, platform puani +0.5, musteri kapasitesi 2x
- Neden eglenceli: Oyuncu 3 farkli slot turunde yatirim yaptiginda oduller. Kalite + hiz + kapasite uc ayagi birden tuttugunda "gercek fast food zinciri" hissi verir.

**Combo 2: "Volume Blitz"**
- Ghost Kitchen (FFC04) + Delivery App Listing (FFC10) + Delivery Rider (FFC07)
- Etki: Teslimat geliri 2x, musteri +8/tur; ama platform komisyonu cift kesilir, fiziksel musteri sifir
- Neden eglenceli: Yuksek gelir yuksek risk. Platform zammi eventi gelirse tum gelir yapisin sarsar. "Tum yumurtalari ayni sepete koyma" gerilimi.

---

# CAFE KARTLARI (CAF01-CAF15)

**Venture kimliği:** Loyalty odakli, barista bagimli. Espresso makinesi core asset. Yuksek brut marj ama ambiyans ve kalite zorunlu.

| ID | KART ADI | TIP | SLOT | RARITY | VentureType | OZELLIK |
|---|---|---|---|---|---|---|
| CAF01 | Espresso Machine | Business | Operation | Common | Cafe | Her tur +6 gelir, kahve kalitesi +1; bakim -10 her 5 turda |
| CAF02 | Terrace Setup | Business | Operation | Uncommon | Cafe | Musteri kapasitesi +4, Instagram gorunurlugu +1; kira +10/tur, kis mevsiminde etkisiz |
| CAF03 | Pastry Counter | Business | Operation | Common | Cafe | Her tur +4 gelir, musteri basi harcama +%15; fire riski her 3 turda |
| CAF04 | La Marzocco Upgrade | Business | Operation | Rare | Cafe | Kahve kalitesi +3 (kalici), platform puani +0.3; maliyet -300 tek seferlik, bakim -15/5 tur |
| CAF05 | SCA Barista | Employee | Staff | Rare | Cafe | Kalite +3, latte art aktif (Instagram sinerjisi); maas -30/tur, rakip 2x maas teklif riski |
| CAF06 | Trainee Barista | Employee | Staff | Common | Cafe | Kalite +0, temel isler; maas -12/tur, 5 tur sonra kalite +1 (deneyim) |
| CAF07 | Pastry Chef | Employee | Staff | Uncommon | Cafe | Pastane geliri +8/tur, musteri basi harcama +%10; maas -20/tur |
| CAF08 | Friendly Cashier | Employee | Staff | Common | Cafe | Musteri sadakati +1/tur, servis hizi +1; maas -10/tur |
| CAF09 | Loyalty Card Program | Business | Marketing | Uncommon | Cafe | Musteri churn -%30 (kalici), sadik musteri her 5'i 1 yeni musteri getirir |
| CAF10 | Latte Art Instagram | Business | Marketing | Rare | Cafe | Platform puani +0.3/tur, organik musteri +3/tur; SCA Barista GEREKLI, yoksa etki sifir |
| CAF11 | WiFi Free Sign | Business | Marketing | Common | Cafe | Freelancer musteri +2/tur; oturma suresi artar, gelir/saat duser |
| CAF12 | Specialty Beans | Business | Supplier | Uncommon | Cafe | Kahve kalitesi +2, premium fiyat +%20 gelir; maliyet +15/tur |
| CAF13 | Barista Milk | Business | Supplier | Common | Cafe | Latte kalitesi +1, mikro kopuk mumkun; maliyet +8/tur, market sutuyle degistirirsen kalite -2 |
| CAF14 | Milk Price Surge | Event | TempEffect | Uncommon | Cafe | 2 tur boyunca sut maliyeti +%40; ucuz sute gecersen kalite -2 ve kotu yorum |
| CAF15 | Barista Poached | Event | TempEffect | Rare | Cafe | En iyi barista gider, kalite -3, Instagram akisi durur; 3 tur surede yeni barista bulmalisin |

### CAFE COMBO'LARI

**Combo 1: "Third Wave Perfection"**
- La Marzocco Upgrade (CAF04) + SCA Barista (CAF05) + Specialty Beans (CAF12)
- Etki: Kahve kalitesi MAX, platform puani +0.8, gelir +%40, "Latte Sanati" ozel etiketi
- Neden eglenceli: Cafe'nin ultimate formu. 3 farkli slotta yuksek yatirim gerektirir. Basarinca oyuncu "premium kahveci" oldugunun farkina varir. Ama maaliyet devasa -- nakit yonetimi zorlar.

**Combo 2: "Community Hub"**
- Loyalty Card Program (CAF09) + WiFi Free Sign (CAF11) + Friendly Cashier (CAF08)
- Etki: Musteri churn -%50, sadik musteri orani 2x, organik buyume +2/tur
- Neden eglenceli: Yavas ama saglam buyume. Rakibin agresif marketingine dayanikli bir kale kurar. "Mahalle kafesi" hissi -- musteri gitmez. Ama hizli kazanma yolu degil, sabir ister.

---

# TECH APP KARTLARI (TEC01-TEC15)

**Venture kimliği:** Platform fee, viral buyume potansiyeli, delayed income (2-3 tur gecikme). En dusuk giris maliyeti, en yuksek risk. Olcekleme = eksponansiyel buyume veya eksponansiyel maliyet.

| ID | KART ADI | TIP | SLOT | RARITY | VentureType | OZELLIK |
|---|---|---|---|---|---|---|
| TEC01 | MVP Launch | Business | Operation | Common | TechApp | 2 tur gelir yok (gelistirme suresi), 3. turdan itibaren +5 gelir/tur; ilk yatirim |
| TEC02 | Subscription Model | Business | Operation | Uncommon | TechApp | Her tur sabit +10 gelir; churn -%5-15 kullanici/tur, retention kartlariyla sinerji |
| TEC03 | In-App Purchase | Business | Operation | Common | TechApp | Kullanici basi +3 gelir; Apple %30 komisyon keser, kullanici deneyimi -1 |
| TEC04 | Cloud Backend | Business | Operation | Rare | TechApp | Kullanici kapasitesi 3x, crash riski -%80; maliyet kullanici sayisiyla orantili artar |
| TEC05 | Senior Developer | Employee | Staff | Rare | TechApp | Bug orani -%50, yeni ozellik hizi 2x; maas -35/tur, burnout riski 8+ turda |
| TEC06 | Intern Developer | Employee | Staff | Common | TechApp | Temel bug fix; maas -8/tur, hata orani +%10 (deneyimsiz) |
| TEC07 | UX Designer | Employee | Staff | Uncommon | TechApp | Retention +%20, App Store rating +0.2; maas -20/tur |
| TEC08 | Community Manager | Employee | Staff | Common | TechApp | Kullanici geri bildirimi aktif, churn -%10; maas -12/tur |
| TEC09 | ASO Optimization | Business | Marketing | Uncommon | TechApp | Organik indirme +%30 (kalici); tek seferlik maliyet, uzun vadeli deger |
| TEC10 | Product Hunt Launch | Business | Marketing | Rare | TechApp | Kullanici spike: 2 tur boyunca kullanici 3x; sonrasinda churn yuksek, backend spike |
| TEC11 | Referral System | Business | Marketing | Common | TechApp | Viral katsayi +0.3, her kullanici 0.3 yeni kullanici getirir; geciken ama buyuyen etki |
| TEC12 | Firebase Free Tier | Business | Supplier | Common | TechApp | Baslangic backend ucretsiz; 10K kullanicida maliyet spike, vendor lock-in |
| TEC13 | Cloudflare Workers | Business | Supplier | Uncommon | TechApp | Backend maliyet -%40 (Firebase'e gore); gecis maliyeti -60, olceklemede avantaj |
| TEC14 | Server Crash | Event | TempEffect | Uncommon | TechApp | 1 tur gelir yok, kullanici -%10; Cloud Backend varsa hasar -%80 |
| TEC15 | App Store Rejection | Event | TempEffect | Rare | TechApp | 2 tur gelir yok (inceleme sureci); guideline uyumu varsa 1 tura duser |

### TECH APP COMBO'LARI

**Combo 1: "Viral Machine"**
- Product Hunt Launch (TEC10) + Referral System (TEC11) + Cloud Backend (TEC04)
- Etki: Kullanici akisi 5x, viral katsayi 0.6'ya cikar; backend maliyeti patlayabilir
- Neden eglenceli: "Unicorn anini" yakalama gerilimi. Backend hazir degilse sunucu coker ve tum kullanicilarini kaybedersin. Hazirsa -- eksponansiyel buyume. Risk/odul dengesi mukemmel.

**Combo 2: "Lean Startup"**
- MVP Launch (TEC01) + Firebase Free Tier (TEC12) + Intern Developer (TEC06)
- Etki: Sifir maliyet baslangic, ilk 5 turda nakit korunur; ama olcekleme noktasinda Firebase spike + intern hatalari patlar
- Neden eglenceli: "Garaj startup" fantezisi. En ucuza baslayip buyumeye calisma. Ama bir noktada ya yatirim yapacaksin ya da "Sunucu Cokusu" domino zinciri baslar. Gecis zamanlama karari kritik.

---

# CLOTHING STORE KARTLARI (CLO01-CLO15)

**Venture kimliği:** Sezon gecisleri, vitrin refresh zorunlulugu, trend takibi. Stok = para, satilmayan stok = batan para. Gorsel pazarlama gucu.

| ID | KART ADI | TIP | SLOT | RARITY | VentureType | OZELLIK |
|---|---|---|---|---|---|---|
| CLO01 | Display Rack | Business | Operation | Common | ClothingStore | Musteri kapasitesi +3, vitrin gorunurlugu +1; her 2 turda "vitrin eskir" -- yenile veya musteri -%20 |
| CLO02 | Fitting Room | Business | Operation | Uncommon | ClothingStore | Iade orani -%30, musteri basi gelir +%10; yer kaplar, kapasite -%1 |
| CLO03 | Online Storefront | Business | Operation | Uncommon | ClothingStore | Yeni musteri kanali +4/tur; komisyon -%15-25, iade orani +%20 (online), kargo maliyeti |
| CLO04 | Flagship Window | Business | Operation | Rare | ClothingStore | Vitrin etkisi 2x, musteri +5/tur; kira +15/tur, her 2 turda yenileme zorunlu |
| CLO05 | Style Consultant | Employee | Staff | Uncommon | ClothingStore | Musteri basi gelir +%15, iade orani -%15; maas -20/tur |
| CLO06 | Stock Manager | Employee | Staff | Common | ClothingStore | Stok batigi riski -%40, sezon gecisi kaybi -%20; maas -15/tur |
| CLO07 | Social Media Intern | Employee | Staff | Common | ClothingStore | Instagram icerigi +1/tur, marketing sinerjisi; maas -8/tur, kalite tutarsiz |
| CLO08 | Visual Merchandiser | Employee | Staff | Rare | ClothingStore | Vitrin eskime suresi 2x uzar (4 tur), musteri cekimi +%25; maas -22/tur |
| CLO09 | Outfit of the Day | Business | Marketing | Uncommon | ClothingStore | Bu tur musteri +6, Instagram gorunurlugu +2; TikTok trend aktifse etki 2x |
| CLO10 | Seasonal Campaign | Business | Marketing | Common | ClothingStore | Platform puani +0.2, musteri +3/tur; sezon degisiminde etki sifirlanir, yeniden oynamak gerekir |
| CLO11 | Influencer Collab | Business | Marketing | Rare | ClothingStore | Musteri +10 bu tur, marka bilinirliği +2; maliyet -150, "Influencer Skandali" event riski |
| CLO12 | Merter Wholesale | Business | Supplier | Common | ClothingStore | Stok maliyeti -%20, min 50 adet; trend riski -- yanlis trend = stok batigi |
| CLO13 | Premium Collection | Business | Supplier | Uncommon | ClothingStore | Marj +%15, kalite musteri +2/tur; maliyet +25/tur, sezon sonunda satilmayan stok riski |
| CLO14 | Season Transition | Event | TempEffect | Common | ClothingStore | 1 tur gelir -%30 (gecis donemi); Sezon Koleksiyonu (CLO13) aktifse sonraki 2 tur +%20 |
| CLO15 | Trend Flop | Event | TempEffect | Rare | ClothingStore | Stok degerinin %50'si kayip, nakit -100; Stock Manager varsa hasar -%40 |

### CLOTHING STORE COMBO'LARI

**Combo 1: "Fashion Empire"**
- Flagship Window (CLO04) + Visual Merchandiser (CLO08) + Premium Collection (CLO13)
- Etki: Vitrin etkisi 3x, musteri +12/tur, marj +%25; ama maliyet devasa, sezon gecisinde her sey yenilenmeli
- Neden eglenceli: "Lux butik" fantezisi. Yuksek marj yuksek maliyet. Sezon gecisi her seferinde gerilim yaratiyor -- zamaninda yenilersin ya da 3x yatirimın boşa gider.

**Combo 2: "Fast Fashion Machine"**
- Merter Wholesale (CLO12) + Online Storefront (CLO03) + Stock Manager (CLO06)
- Etki: Dusuk maliyet + genis erisim + stok kontrolu; hacim odakli buyume, dusuk marj ama stabil
- Neden eglenceli: BIM/LC Waikiki tarzi hacim stratejisi. Risk dusuk ama kazanc da sinirli. Rakip premium giderse musteri kaybedersin. Pozisyon secimi = kimlik secimi.

---

# GROCERY STORE KARTLARI (GRO01-GRO15)

**Venture kimliği:** Spoilage (bozulma) riski, veresiye sistemi, zincir market rekabeti. En dusuk marj (%2-5) ama en stabil musteri trafigi. Gunluk ihtiyac = tekrarlayan musteri.

| ID | KART ADI | TIP | SLOT | RARITY | VentureType | OZELLIK |
|---|---|---|---|---|---|---|
| GRO01 | Fresh Produce Stand | Business | Operation | Common | GroceryStore | Musteri +3/tur, yuksek marj; her 2 turda %15 fire kaybi, buzdolabi GEREKLI |
| GRO02 | Industrial Fridge | Business | Operation | Uncommon | GroceryStore | Fire riski -%50, taze urun kapasitesi 2x; elektrik maliyeti +8/tur, arizalanma riski |
| GRO03 | WhatsApp Orders | Business | Operation | Common | GroceryStore | Ek musteri +2/tur, zincir marketin yapamadigi kisisel hizmet; personel zamani gerekli |
| GRO04 | Night Window | Business | Operation | Uncommon | GroceryStore | Gece musterisi +3/tur (BIM kapattiktan sonra); ek personel veya kendi zamanin gerekli |
| GRO05 | Veteran Cashier | Employee | Staff | Common | GroceryStore | Servis hizi +1, musteri memnuniyeti +1; maas -15/tur |
| GRO06 | Stock Boy | Employee | Staff | Common | GroceryStore | Raf duzeni +1, fire -%10, SKT takibi; maas -10/tur |
| GRO07 | Deli Counter Worker | Employee | Staff | Uncommon | GroceryStore | Taze urun satisi +%25, musteri +2/tur (acik peynir/zeytin); maas -18/tur, hijyen riski |
| GRO08 | Night Guard | Employee | Staff | Uncommon | GroceryStore | Hirsizlik -%80, gece calismasi mumkun; maas -12/tur |
| GRO09 | Local Bread Deal | Business | Marketing | Common | GroceryStore | Loss leader: sifir marj ama musteri +2/tur (ekmek alirken baska urun alir) |
| GRO10 | Neighborhood Flyer | Business | Marketing | Common | GroceryStore | Bu tur musteri +3, lokal gorunurluk +1; ucuz ve etkili, tek seferlik |
| GRO11 | Loyalty Tab System | Business | Marketing | Uncommon | GroceryStore | Musteri sadakati +3/tur; her 3 turda %30 ihtimalle batik veresiye (-60 nakit) |
| GRO12 | Wholesale Distributor | Business | Supplier | Common | GroceryStore | Paketli urun maliyeti -%15, stabil tedarik; minimum siparis zorunlu |
| GRO13 | Local Farm Direct | Business | Supplier | Rare | GroceryStore | Taze urun kalitesi +2, fire -%20, "organik" etiketi; maliyet +12/tur, mevsimsel tedarik riski |
| GRO14 | Fridge Breakdown | Event | TempEffect | Uncommon | GroceryStore | Tum taze stok kayip (-80 nakit), 2 tur taze urun satilamaz; Industrial Fridge varsa hasar -%60 |
| GRO15 | Chain Store Opens | Event | TempEffect | Rare | GroceryStore | 3 tur boyunca musteri -%40; farklilasma (taze urun, gece servis, veresiye) varsa etki -%15'e duser |

### GROCERY STORE COMBO'LARI

**Combo 1: "Mahalle Bakkalin Silahi"**
- Fresh Produce Stand (GRO01) + Local Farm Direct (GRO13) + Deli Counter Worker (GRO07)
- Etki: Taze urun kalitesi MAX, fire -%30, musteri +7/tur, "Organik Mahalle Marketi" ozel etiketi
- Neden eglenceli: Zincir marketin asla yapamayacagi seyi yapiyorsun -- kisisel, taze, guvenilir. Chain Store Opens eventi geldiginde bu combo seni korur. "Kucuk ama guclu" hissi.

**Combo 2: "7/24 Mahalle Noktasi"**
- Night Window (GRO04) + WhatsApp Orders (GRO03) + Loyalty Tab System (GRO11)
- Etki: 24 saat erisim + dijital siparis + musteri baglama; stabil musteri akisi ama veresiye batik riski
- Neden eglenceli: BIM gece 21:00'de kapaniyor, sen hala aciksin. WhatsApp'tan siparis aliyorsun. Veresiye vererek musteri bagliyorsun. Ama veresiye batigi tum sistemi sarsabilir. Gerilim surekli.

---

# GENEL (NEUTRAL) KARTLAR (GEN01-GEN20)

**Her venture'da gelebilir. Kart havuzunun %20'si.**

## Genel Action Kartlari (tek seferlik, slota girmez)

| ID | KART ADI | TIP | SLOT | RARITY | VentureType | OZELLIK |
|---|---|---|---|---|---|---|
| GEN01 | Bank Loan | Action | None | Common | None | Aninda +200 nakit; faiz %8/tur, 5 tur vade, odemezsen kredi notu duser |
| GEN02 | Tax Planning | Action | None | Uncommon | None | Bu donem vergi -%30; bir sonraki donemde denetim riski +10 |
| GEN03 | Market Research | Action | None | Common | None | Rakibin bu turki hamlesi gorunur; bilgi = guc, dogru hamleyi secebilirsin |
| GEN04 | Emergency Repair | Action | None | Common | None | Herhangi bir "arizali" Operation slotunu aninda tamir et; maliyet -50 |
| GEN05 | Price War | Action | None | Uncommon | None | Bu tur fiyat hassas musteri +5, gelir -%30; rakipten musteri calar ama marj erir |

## Genel Upgrade Kartlari (kalici, slota girmez -- mevcut karti gelistirir)

| ID | KART ADI | TIP | SLOT | RARITY | VentureType | OZELLIK |
|---|---|---|---|---|---|---|
| GEN06 | Cost Optimization | Upgrade | None | Uncommon | None | Tum giderler -%10 (kalici); yatirim -80, 8 turda kendini amorti eder |
| GEN07 | Security System | Upgrade | None | Uncommon | None | Yasal risk -15, hirsizlik -%60 (kalici); yatirim -60 |
| GEN08 | POS Terminal | Upgrade | None | Common | None | Servis hizi +1, hata orani -%20 (kalici); yatirim -40 |
| GEN09 | Hygiene Certificate | Upgrade | None | Common | None | Saglik denetimi basari orani %100; yatirim -30, yilllik yenileme -10 |
| GEN10 | Insurance Policy | Upgrade | None | Rare | None | Negatif event hasari -%40 (kalici); maliyet +15/tur |

## Genel Staff Kartlari

| ID | KART ADI | TIP | SLOT | RARITY | VentureType | OZELLIK |
|---|---|---|---|---|---|---|
| GEN11 | Accountant | Employee | Staff | Uncommon | None | Vergi orani -%50, nakit akisi gorunurlugu; maas -18/tur |
| GEN12 | Security Guard | Employee | Staff | Common | None | Hirsizlik -%80, gece operasyon mumkun; maas -12/tur |
| GEN13 | Cleaning Staff | Employee | Staff | Common | None | Hijyen skoru +2, denetim riski -%20; maas -10/tur |
| GEN14 | Manager | Employee | Staff | Rare | None | Tum staff morali +1/tur, verimlilik +%10; maas -30/tur, sadakat yuksekse rakip calamaz |
| GEN15 | Intern | Employee | Staff | Common | None | Temel isler, maas -5/tur; 5 tur sonra deneyimli calisan olur (+1 kalite), dusuk sadakat |

## Genel Event Kartlari (krizler -- TempEffect slotuna duser)

| ID | KART ADI | TIP | SLOT | RARITY | VentureType | OZELLIK |
|---|---|---|---|---|---|---|
| GEN16 | Tax Audit | Event | TempEffect | Uncommon | None | Vergi borcu 3x odemeli; Accountant varsa hasar -%50, vergiden kacildiysa yakalanma %80 |
| GEN17 | Staff Strike | Event | TempEffect | Rare | None | 2 tur boyunca tum staff etkisiz; acil zam (-50 nakit) ile 1 tura indirilir |
| GEN18 | Inflation Wave | Event | TempEffect | Common | None | 2 tur boyunca tum maliyetler +%25; Supplier slotundaki kartlar etkiyi -%10 azaltir |
| GEN19 | Viral Moment | Event | TempEffect | Rare | None | 2 tur boyunca musteri 2x; operasyon hazir degilse servis coker ve platform puani -0.5 |
| GEN20 | Power Outage | Event | TempEffect | Common | None | 1 tur boyunca gelir -%50; buzdolabi olan isletmelerde taze stok kaybi ek -40 nakit |

---

# DENGE OZETI

## Venture Basina Guc/Zayiflik Matrisi

| Venture | Guclu Yonu | Zayif Yonu | Riskli Kart | Guvenli Kart |
|---|---|---|---|---|
| FastFood | Yuksek hacim, hizli gelir | Dusuk marj, fire, platform bagimlilik | Ghost Kitchen (FFC04) | Line Cook (FFC05) |
| Cafe | Yuksek marj (%75-87 brut), sadakat | Barista bagimlilik, mevsimsellik | Latte Art Instagram (CAF10) | Loyalty Card Program (CAF09) |
| TechApp | Sinirsiz olcekleme, dusuk giris | Delayed income, churn, platform komisyon | Product Hunt Launch (TEC10) | ASO Optimization (TEC09) |
| ClothingStore | Yuksek brut marj, gorsel cekicilik | Sezon riski, stok batigi, trend bagimlilik | Influencer Collab (CLO11) | Stock Manager (CLO06) |
| GroceryStore | Stabil gunluk trafik, sadakat | En dusuk marj, fire, zincir baskisi | Loyalty Tab System (GRO11) | Wholesale Distributor (GRO12) |

## Slot Dagilimi (tum kartlar)

| Slot Tipi | Venture Kartlari | Genel Kartlar | Toplam |
|---|---|---|---|
| Operation | 20 (5x4) | 0 | 20 |
| Staff | 20 (5x4) | 5 | 25 |
| Marketing | 15 (5x3) | 0 | 15 |
| Supplier | 10 (5x2) | 0 | 10 |
| TempEffect | 10 (5x2) | 5 | 15 |
| None (Action/Upgrade) | 0 | 10 | 10 |
| **TOPLAM** | **75** | **20** | **95** |

## Rarity Dagilimi

| Rarity | Venture | Genel | Toplam |
|---|---|---|---|
| Common | 30 (5x6) | 8 | 38 |
| Uncommon | 25 (5x5) | 7 | 32 |
| Rare | 20 (5x4) | 5 | 25 |
| **TOPLAM** | **75** | **20** | **95** |

## CardType Dagilimi (enum uyumu)

| CardType (enum) | Toplam |
|---|---|
| Business | 45 (Operation+Marketing+Supplier venture kartlari) |
| Employee | 25 (Staff venture + genel) |
| Action | 5 (genel action) |
| Upgrade | 5 (genel upgrade) |
| Event | 15 (TempEffect venture + genel) |
| **TOPLAM** | **95** |

---

# TUM COMBO LISTESI

| # | Venture | Combo Adi | Gerekli Kartlar | Slot Gereksinimleri | Etki Ozeti |
|---|---|---|---|---|---|
| 1 | FastFood | Franchise Formula | FFC06 + FFC13 + FFC03 | Staff + Supplier + Operation | Gelir +%30, puan +0.5, kapasite 2x |
| 2 | FastFood | Volume Blitz | FFC04 + FFC10 + FFC07 | Operation + Marketing + Staff | Teslimat geliri 2x, musteri +8/tur |
| 3 | Cafe | Third Wave Perfection | CAF04 + CAF05 + CAF12 | Operation + Staff + Supplier | Kalite MAX, puan +0.8, gelir +%40 |
| 4 | Cafe | Community Hub | CAF09 + CAF11 + CAF08 | Marketing + Marketing + Staff | Churn -%50, organik buyume +2/tur |
| 5 | TechApp | Viral Machine | TEC10 + TEC11 + TEC04 | Marketing + Marketing + Operation | Kullanici 5x, viral 0.6 |
| 6 | TechApp | Lean Startup | TEC01 + TEC12 + TEC06 | Operation + Supplier + Staff | Sifir maliyet baslangic |
| 7 | ClothingStore | Fashion Empire | CLO04 + CLO08 + CLO13 | Operation + Staff + Supplier | Vitrin 3x, musteri +12/tur |
| 8 | ClothingStore | Fast Fashion Machine | CLO12 + CLO03 + CLO06 | Supplier + Operation + Staff | Dusuk maliyet hacim buyumesi |
| 9 | GroceryStore | Mahalle Bakkalin Silahi | GRO01 + GRO13 + GRO07 | Operation + Supplier + Staff | Taze kalite MAX, musteri +7/tur |
| 10 | GroceryStore | 7/24 Mahalle Noktasi | GRO04 + GRO03 + GRO11 | Operation + Operation + Marketing | 24 saat erisim + dijital + sadakat |

---

# TASARIM NOTLARI (Game Designer)

## Neden Bu Kartlar Eglenceli

**1. Her kart bir ikilem:**
Hic bir kart "bedava guc" degil. Ghost Kitchen yuksek gelir ama platform bagimli. Loyalty Tab musteri baglar ama veresiye batigi gelir. SCA Barista kalite verir ama rakip calabilir. Oyuncu her kartta "buna deger mi?" sorusunu sorar.

**2. Combo kesfetme ani:**
Oyuncu ilk kez La Marzocco + SCA Barista + Specialty Beans birlestirdiginde "Third Wave Perfection" popup'i gorur. Bu ani "OHAAA" dedirten ani. Combo tabloda yazili ama oyuncu kesfettiginde hissedecegi sey saf memnuniyet.

**3. Kriz anlarinda karar baski:**
Fridge Breakdown geldiginde oyuncunun elinde Emergency Repair varsa ani karar: "Simdi mi kullanayim yoksa daha buyuk kriz icin saklayayim mi?" Bu gerilim tur basina 2-3 kez olusur.

**4. Venture kimlik farki:**
FastFood oynayan "hacim ve hiz" dusunsun. Cafe oynayan "kalite ve sadakat" dusunsun. TechApp oynayan "olcekleme ve zamanlama" dusunsun. Ayni oyun, 5 farkli zihinsel model. Tekrar oynanabilirlik buradan gelir.

**5. Risk/odul egri her venture icin farkli:**
- FastFood: orta risk, orta odul, hizli geri donus
- Cafe: dusuk risk (dogrusu yapilirsa), yuksek marj, yavas buyume
- TechApp: yuksek risk, sifir veya unicorn, gecikme gerilimi
- ClothingStore: sezonsal risk, yuksek marj piramidi, zamanlama kritik
- GroceryStore: dusuk risk, dusuk odul, stabil ama sikici -- kredi ve veresiye ile heyecan artar

## Teknik Uyum Notu

Tum kartlar su enum'lara uyar:
- `CardType`: Business, Employee, Action, Upgrade, Event
- `SlotType`: Operation, Staff, Marketing, Supplier, TempEffect
- `VentureType`: FastFood, Cafe, TechApp, ClothingStore, GroceryStore (veya None)
- `Rarity`: Common, Uncommon, Rare (Epic/Legendary post-MVP)

Kart havuzu orani: %80 venture-specific, %20 genel -- GDD Section 19.2 ile uyumlu.
