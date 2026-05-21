# GAME DESIGN DOCUMENT
# "Empire of Cards"

> Versiyon: 5.0 | Tarih: 2026-05-21
> Engine: Unity 6 (C#) | Platform: PC (Steam)
> Tur: App Store card-strategy simulation

---

# 1. Yuksek Konsept

## 1.1 Oyuncu Vaadi

Oyuncu bir mobil uygulama veya oyun gelistiricisi. App Store / Google Play'de bir urun cikarip buyutuyor. Ayni kategorideki yapay zeka rakibe karsi market share savasi veriyor. Her tur kart oynayarak gercek startup kararlari veriyor.

> "Garajda baslayan bir fikri, milyonlarin indirdigi bir uygulamaya donusturdum."

## 1.2 Oyunun Cumlesi

App adi gir, kategori sec, MVP cikar, buyut, krizleri atla, rakibini ez ve app store'un kralina don. Sonra sat, holding kur ve daha buyuk hayallere basla.

## 1.3 Tasarim Ilkeleri

1. Her kart bir karar, buyu degil. "Senior Backend mi, Stajyer mi?" gercek bir startup sorusu.
2. Kategori secimi kozmetik degil: kart havuzunu, krizleri, gelir modelini ve rakip davranisini degistirir.
3. Senaryolar zincirlidir: "Ucuz hosting sectin > server crash > 1-star bomb > organik dusus > para bitti"
4. Oyun agir simulasyon degil, okunabilir kart-strateji hibritidir.
5. Risk/odul dengesi: Dark pattern kisa vadede kazandirir, uzun vadede yok eder.

## 1.4 Oyuncunun Her Tur Yaptigi Sey

1. Board durumunu okur: "Server kapasitem yetersiz, crash riski var"
2. Kart ceker (board state'e gore biased)
3. Kartlari slotlara yerlestirir veya anlik karar kartlari oynar
4. Sistem demand, capacity, quality, rating ve market share uzerinden cozulur
5. Kriz varsa tepki verir
6. Rakibin ayni kategoride nasil buyudugunu gorur

---

# 2. Acilis Akisi

## 2.1 Oyun Baslangic Sirasi

```
1. START GAME
   |
2. APP NAME gir (uygulamanin adi)
   |
3. "APP" veya "GAME" sec (2 ana dal)
   |
4. KATEGORI SEC (gercek magaza kategorileri)
   |
5. Board kurulur, rakip ayni kategoride acar
   |
6. OYUN BASLAR
```

## 2.2 Ana Dal: APP vs GAME

| | APP | GAME |
|---|---|---|
| Gelir modeli | Subscription, Freemium, IAP | Ad Revenue, IAP, Premium |
| Buyume | Organik + ASO + Paid | Viral + CPI + Cross-promo |
| Ana metrik | DAU, Retention, Churn | D1/D7 Retention, ARPU, LTV |
| Kriz turu | Privacy skandali, churn wave | Clone savasi, ad fatigue |
| Rakip baskisi | Feature copy, fiyat kirma | Reskin, kategori flood |

## 2.3 APP Kategorileri (MVP: 4)

| Kategori | Fantezi | Kimlik |
|---|---|---|
| Social & Messaging | "Yeni Tinder/WhatsApp yap" | Ag etkisi, viral loop, toxicity, veri skandali |
| AI Tools | "ChatGPT rakibi cikar" | GPU maliyeti, hallucination, hype cycle, etik |
| Health & Fitness | "Adim sayar / diyet app" | KVKK, saglik regulasyonu, seasonal churn |
| Fintech / E-Commerce | "Odeme app / marketplace" | Lisans baskisi, guvenlik, fraud, guven |

## 2.4 GAME Kategorileri (MVP: 3)

| Kategori | Fantezi | Kimlik |
|---|---|---|
| Hyper-casual | "Flappy Bird yap" | Ucuz CPI, hizli clone, ad revenue, 1 hafta omur |
| Casual / Puzzle | "Candy Crush yap" | Level design, IAP denge, whale hunting, soft launch |
| Mid-core / Strategy | "Clash of Clans yap" | Uzun dev, PvP balance, guild sistemi, LiveOps |

---

# 3. Masa Duzeni (Board)

## 3.1 Uc Ana Bolge

```
+----------------------------------------------------------+
| APP STORE / MARKET ZONE (paylasilan alan)                |
| Trending | Top Free | Top Paid | Featured                |
| Siralamalar, store rating, download sayisi               |
+----------------------------------------------------------+
| RIVAL ZONE (kisitli gorunur)                             |
| [?][?][Ads Campaign][Senior Dev][?]  Rating: 4.3         |
+----------------------------------------------------------+
| PLAYER ZONE (tam gorunur - senin ofisin)                 |
|                                                          |
| [PRODUCT]  [TEAM]    [GROWTH]   [INFRA]                 |
| 4 slot     5 slot    3 slot     2 slot                   |
|                                                          |
| [MARKET EVENTS] 3 slot                                   |
|                                                          |
| [ELINDEKI KARTLAR] 5 kart                               |
|                                                          |
| Cash | Aksiyon | Rating | DAU | Market Share             |
+----------------------------------------------------------+
```

## 3.2 Player Zone = Startup Ofisi

Slot isimlerinin hissi "ofis" olmali. Kartlar masanin uzerindeki post-it'ler, whiteboard notlari gibi hissetmeli.

## 3.3 Market Zone = App Store

Gercek bir app store siralamasi gorunur. Oyuncu "ben kacinci siradayim?" hissini yasamali.

## 3.4 Rival Zone

Rakibin masasi kisitli gorunur. Ama ne yone gittigini okuyabilirsin:
- Marketing agresif mi?
- Kalite mi oynuyor?
- Staff poach mi yapiyor?

---

# 4. Ortak Slot Omurgasi

## 4.1 5 Slot Ailesi

| Slot | App Dunyasi Karsiligi | Amac |
|---|---|---|
| Product (Operation) | MVP, Backend, Feature, Push Notif | Cekirdek urun kapasitesi |
| Team (Staff) | Developer, Designer, QA, Growth, PM | Urunu ayakta tutan ekip |
| Growth (Marketing) | ASO, Google Ads, Influencer, Referral | Demand iten aktif baski |
| Infra (Supplier) | AWS, Firebase, Stripe, Analytics SDK | Kalite ve maliyet kararlari |
| Market Events (Temp) | Krizler, viral anlar, store feature | Gecici durum ve baskilar |

## 4.2 Baslangic Slot Sayilari

| Slot | Cag 1 | Cag 2 | Cag 3 | Cag 4 |
|---|---|---|---|---|
| Product | 2 | 3 | 4 | 5 |
| Team | 2 | 3 | 5 | 6 |
| Growth | 1 | 2 | 3 | 3 |
| Infra | 1 | 1 | 2 | 2 |
| Market Events | 2 | 3 | 3 | 3 |
| Toplam | 8 | 12 | 17 | 19 |
| Aksiyon/tur | 2 | 3 | 4 | 5 |

## 4.3 Slot Felsefesi

Oyuncu her seyi ayni anda yapamaz. Cag 1'de 2 Product + 2 Team + 1 Growth + 1 Infra = sadece 6 aktif kart. Bu sinirlilik stratejiyi dogurur:

- Senior dev mi alayim yoksa 2 stajyer mi?
- AWS mi yoksa ucuz hosting mi?
- ASO mu yoksa TikTok influencer mi?
- Crash fix mi yoksa yeni feature mi?

---

# 5. 8 Ana Stat

## 5.1 Stat Listesi

| Stat | App Karsiligi | Anlami |
|---|---|---|
| Cash | Runway | Kac ay hayatta kalirsun |
| Demand | User Acquisition | Yeni kullanici akisi |
| Capacity | Server/Infra | Kac kullaniciyi tasiyabilirsin |
| Quality | App Quality (crash, UX) | Store rating'i belirler |
| Rating | Store Rating (1-5) | Organik buyumeyi belirler |
| Staff Stability | Team Morale / Burnout | Crunch = hizli ama uzun vadede cokus |
| Legal Risk | Compliance Risk | KVKK, sahte iddia, dark pattern |
| Market Share | Category Ranking | Kazanma kosulu |

## 5.2 Matematiksel Zincir

```
Product + Team + Infra => Quality + Capacity
Growth + Rating => Demand (Downloads)

EGER Demand > Capacity:
  => Server crash / yavas app / kotu UX
  => Rating duser
  => Organik demand duser
  => Rakip market share calar

EGER Demand <= Capacity:
  => Revenue artar
  => Rating korunur/yukselir
  => Market Share buyur
```

## 5.3 Turetilmis Metrikler (Kategoriye Gore)

### AI Tools:
- Model Accuracy, Hallucination Rate, GPU Cost, Hype Level

### Social & Messaging:
- Network Effect, Toxicity, Data Sensitivity, Engagement Rate

### Health & Fitness:
- Compliance Score, Seasonal Demand, Wearable Integration, Churn Rate

### Fintech:
- Security Score, License Status, Fraud Rate, User Trust

### Hyper-casual:
- CPI, D1 Retention, ARPU, Clone Count

### Casual / Puzzle:
- Level Depth, Whale Ratio, Session Length, IAP Conversion

### Mid-core:
- PvP Balance, Guild Activity, LiveOps Cadence, D30 Retention

---

# 6. 4 Cag Sistemi

## 6.1 Cag 1: GARAJ (Turn 1-6)

> "Bir fikrin var, bir laptopun var"

- 2 aksiyon/tur
- Slot: 2 Product, 2 Team, 1 Growth, 1 Infra, 2 TempEffect
- MVP cikar, ilk 100 kullanici
- Product-Market Fit aranir
- Kriz: Motivasyon kaybi, teknik borc, para bitmesi
- Yatirim: Bootstrap (kendi paran)

## 6.2 Cag 2: STARTUP (Turn 7-13)

> "Ilk kullanicilar geldi, simdi buyutme zamani"

- 3 aksiyon/tur
- Slot: +1 Product, +1 Team, +1 Growth, +1 TempEffect
- Seed/Series A firsati
- Ilk rakip baskisi baslar
- Kriz: 1-star bomb, server crash, clone cikti
- Yatirim: Seed Round ($50K-200K, %15-25 dilution)

## 6.3 Cag 3: SCALE-UP (Turn 14-19)

> "Artik bir sirketsin, olcek zamani"

- 4 aksiyon/tur
- Slot: +1 Product, +2 Team, +1 Growth, +1 Infra
- Series B, yeni pazarlar
- Rakip agresif: fiyat kirma, feature copy, staff poach
- Kriz: Burnout wave, privacy scandal, market shift
- Yatirim: Series A/B ($500K-10M)

## 6.4 Cag 4: DOMINANCE (Turn 20-25)

> "Ya hukmet ya yik"

- 5 aksiyon/tur
- Slot: +1 Product, +1 Team
- IPO/Exit/Acquisition firsati
- Regulasyon baskisi (antitrust)
- Kriz: Government hearing, platform ban, mass exodus
- Yatirim: Series C / IPO

---

# 7. Kart Aileleri

## 7.1 Kalici Kurulum (Product + Infra)

Her kart bir urun karari:
- MVP Build, Push Notification, Dark Mode, Backend Upgrade
- Onboarding Flow, In-App Purchase Module, Crash Monitoring
- Localization, Offline Mode, AI Feature Entegrasyonu

## 7.2 Takim (Team)

Her kart bir ise alim karari:
- Stajyer Dev (ucuz, hata yapar)
- Caylak Frontend (CSS bilir, React bilmez)
- Senior Backend (pahali, mimariyi duzeltir)
- Freelance Designer (2 tur calisir gider)
- Growth Hacker (buyutur ama agresif)
- QA Tester, DevOps, Community Manager, CTO

## 7.3 Buyume (Growth)

Her kart bir marketing karari:
- ASO Optimizasyonu, Google Ads, TikTok Influencer
- Product Hunt Launch, Referral Sistemi, Press Release
- Cross-Promo Deal, App Store Feature Bid, Trend-Jacking

## 7.4 Risk Kartlari (ucuz/bedava ama tehlikeli)

Her kart bir kestirme:
- Dark Pattern Onboarding (Retention +25%, Legal Risk +10)
- Kullanici Verisi Sat (+$30K gelir, Data Scandal %40)
- Sahte Download Bot (+5K goruntusu, store ban %25)
- Rakibin API Key Cali (feature unlock, Lawsuit %50)
- Crunch Mode (Capacity x2, Burnout %60)
- Fake Review, Fake Testimonials, Tax Kacirma, Code Copy
- Spam Push Notification, Lisanssiz Muzik

## 7.5 Reaksiyon Kartlari (kriz gelince oynamalik)

Her kart bir toparlanma araci:
- Hotfix Sprint ($20, crash coz)
- PR Kriz Yonetimi ($30, skandal coz)
- Avukat Tut ($50, legal risk sifirla)
- Refund Wave Yonetimi ($15, odeme krizi)
- Emergency Hire ($25, acil freelancer)
- Server Migration ($40, infra krizi)
- Store Appeal ($10, %60 basari)
- Competitor Analiz ($15, 3 tur rakip gorunurlugu)
- Pivot ($0, radikal: kategori degistir, board reset)

## 7.6 Kriz Kartlari (otomatik tetiklenir)

Her kriz bir senaryo ve secim:
- 1-Star Review Bomb: A) Hotfix+Ozur ($25) B) Fake review ($15+risk)
- Server Crash: A) Migration ($40) B) Ucuz fix ($10+tekrar %40)
- API Key Sizintisi: A) Avukat ($50) B) Sessiz kal (risk artar)
- Clone App Cikti: A) Hukuki yol ($50) B) Feature farki olustur
- App Store Rejection: A) Appeal ($10,%60) B) Policy fix+resubmit
- Founder Burnout: A) Tatil (1 tur kayip) B) Devam (kotulur)
- Viral Negatif Tweet: A) PR Kriz ($30) B) Trend-jacking ($5+risk)
- Policy Degisikligi: A) Uyum sagla ($25) B) Gormezden gel (removal %30)
- Yatirimci Baskisi: A) Pivot B) KPI fake (risk)
- Calisan Rakibe Gecti: A) Counter-offer ($30) B) Yenisini al
- Data Breach: A) Avukat+Transparency ($80) B) Sessiz kal

---

# 8. Ekonomi Dongusu

## 8.1 Gelir Modelleri

| Model | Nasil | Risk |
|---|---|---|
| Freemium | %2-5 odeme yapar | Dusuk ARPU |
| Subscription | Aylik tahmin edilebilir | Churn riski |
| Ad Revenue | Kullanici = reklam geliri | Ad fatigue |
| Premium | Tek seferlik | Dusuk download |
| IAP | Uygulama ici satin alma | Whale bagimliligi |

## 8.2 Gelir Formulu

```
Aylik Gelir = ActiveUsers x ARPU x RetentionRate x SeasonMultiplier
Giderler = TeamSalary + InfraCost + MarketingSpend + LoanInterest
Net = Gelir - Giderler
Valuation = Net x GrowthMultiplier x CategoryMultiplier
```

## 8.3 Yatirim Turlari

| Tur | Ne zaman | Ne verir | Ne alir |
|---|---|---|---|
| Bootstrap | Turn 1-4 | Kendi paran | Hicbir sey |
| Seed Round | Turn 5-8 | $50K-200K | %15-25 dilution |
| Series A | Turn 10-14 | $500K-2M | %20-30 dilution |
| Series B | Turn 16-20 | $2M-10M | %15-25 dilution, KPI |
| IPO / Exit | Turn 22-25 | Buyuk para | Oyun sonu trigger |

---

# 9. Tur Akisi (7 Faz)

## 9.1 Fazlar

1. **Draw** - Kart cek (board state'e gore biased)
2. **Planning** - Dashboard oku: "Server crash riski var, QA eksik"
3. **Play** - Kartlari oyna (2-5 aksiyon, caga gore)
4. **Resolve** - Sistem hesaplar:
   - Product + Team + Infra => Quality + Capacity
   - Growth + Rating => Demand
   - Demand vs Capacity => Revenue / Rating
5. **Crisis/Reaction** - Kriz tetiklenir, oyuncu tepki verir
6. **Rival** - Rakip hamle yapar
7. **Market Update** - Store siralamasi guncellenir

## 9.2 Resolve Detay

```
1. Operation + Staff kapasitesi
2. Infra (Supplier) etkileri
3. Growth (Marketing) gorunurluk ve demand uretimi
4. Quality ve service sonucu
5. Rating guncelleme
6. Cash gelir/gider
7. Market share kaymasi
```

---

# 10. Store Mekanigi (Yeni)

## 10.1 ASO (App Store Optimization)

ASO kart ailesi store rating'e ve organic download'a etki eder:
- Keyword optimizasyonu
- Screenshot kalitesi
- Description duzenleme
- Her ASO karari rating'e +0.1-0.3 etki

## 10.2 Store Feature

Nadir ama cok guclu bir Temp Effect:
- Apple/Google seni feature eder
- 3 tur boyunca Demand x3
- Tetikleme: Rating > 4.5 + Quality > 7 + sansa bagli

## 10.3 Update Cycle

Her X turda versiyon cikarmak zorunlu:
- Cikarmazsan "Abandoned App" etiketi
- Rating -0.5/tur
- Organik download kesilir

## 10.4 Review Management

Negatif yorumlara cevap verme mekanigi:
- Community Manager karti ile otomatik
- Manuel cevap = aksiyon harcar ama daha etkili
- Fake review riski: kisa vade rating boost, uzun vade ban

---

# 11. Rakip Sistemi

## 11.1 Ayni Kategori Kural

Rakip oyuncuyla ayni kategoride. Adil ama baskili yaris.

## 11.2 Rakip Davranis Aileleri

- Agresif marketing (CPI savasi)
- Kalite odakli (feature ustunlugu)
- Ucuz klonlama (reskin, copy)
- Defansif (retention odakli)
- Riskli (dark pattern, fake review)

## 11.3 Kategori Bazli Baski

- AI Tools: Model accuracy savasi, API fiyat savasi
- Social: Viral loop savasi, influencer avlama
- Hyper-casual: Clone flood, CPI kirmak
- Fintech: Guvenlik sertifikasi, lisans savasi

---

# 12. Kazanma Yapisi

## 12.1 Ana Hedef

25 tur icinde:
- %60 market share VEYA
- Hedef valuation'a ulasma ($XM exit)

## 12.2 Skor Eksenleri

- Market share
- Final cash
- Store rating
- Cozulen kriz sayisi
- Ulasilan olcek (Cag)
- Exit valuation

## 12.3 Kaybetme Sartlari

- Iflas (cash <= 0)
- Rating cokmesi (Rating <= 1.5, 3+ tur)
- Legal felaket (Legal Risk >= 90)
- Rakip dominasyonu (Rakip market share >= 70)
- Store'dan kaldirilma (ban)

---

# 13. Meta-Progression (Roguelike Loop)

## 13.1 Run Dongusu

```
RUN 1: "FotoFix" (AI Photo Editor)
  Garaj > Startup > Scale > Exit ($5M)
  Unlock: Serial Entrepreneur perk, +%20 starting cash

RUN 2: "BotChat" (AI Assistant)
  $5M ile basla, daha guclu rakip
  Exit ($50M) > HOLDING KURULDU

RUN 3+: Holding modunda multi-app
  2-3 app ayni anda, sinerji kartlari
```

## 13.2 Holding Modu

| Startup | Holding |
|---|---|
| 1 app yonet | 2-3 app |
| Basit board | Multi-board (tab) |
| Bireysel krizler | Portfolio krizleri |
| Seed/Series A | M&A, IPO |
| Rakip = 1 startup | Rakip = baska holding |
| 25 tur | 30 tur |

## 13.3 Unlock Sistemi

| Kosul | Unlock |
|---|---|
| Ilk Exit ($1M+) | Serial Entrepreneur perk |
| $10M Exit | Holding modu |
| 3 farkli kategori | Diversified Portfolio perk |
| $100M toplam | Unicorn Hunter, legendary kartlar |
| $500M holding | IPO mekanizmasi |
| Tum kategoriler | Tech Mogul, hardest mode |

---

# 14. MVP Onceligi

## 14.1 Referans Kategori

AI Tools, sistemin okunabilirligini test etmek icin referans kategoridir.

## 14.2 MVP Kapsami

1. App Name girisi
2. App/Game secimi (ilk MVP: sadece App)
3. Kategori secimi (ilk MVP: AI Tools)
4. 25 tur oynanis
5. 4 Cag slot genislemesi
6. Tam resolve zinciri
7. Kriz + reaksiyon sistemi
8. Rakip AI
9. Kazanma/kaybetme

## 14.3 Post-MVP

- Diger kategoriler (Social, Health, Fintech, Game kategorileri)
- Yatirim turu sistemi
- Meta-progression / Holding modu
- Store Feature mekanigi
- Update Cycle zorunlulugu

---

# 15. Teknik Mimari

## 15.1 Korunacak Mimari Kararlar

- Bootstrap > WiringService > EventBus pattern
- Singleton GameManager
- 3D kart sistemi (Card3D, Hand3D, Board3D, SlotZone3D)
- Polling-based turn phases (7 faz)
- CardData ScriptableObject

## 15.2 Yeniden Yazilacak

- VentureType > AppCategory enum'a donusur
- Kart icerikleri tamamen yeni (app dunyasi)
- Acilis akisi: VentureSelection > AppSetupFlow (Name > Type > Category)
- Slot isimleri: Operation>Product, Staff>Team, Marketing>Growth, Supplier>Infra
- Ekonomi: Yatirim turlari, valuation hesabi
- Rival: Kategori bazli davranislar

## 15.3 Namespace Yapisi

```
EmpireOfCards.Core        - GameManager, TurnManager, EventBus, PlayerResources
EmpireOfCards.Core.TurnPhases - 7 faz
EmpireOfCards.Gameplay    - EconomyManager, BoardManager, DeckManager, RivalAI
EmpireOfCards.Data        - CardData, AppCategoryProfile, VentureProfiles
EmpireOfCards.Bootstrap   - WiringService, ManagerFactory, ContentFactory
EmpireOfCards.UI          - UIManager, TopBar, Popups, Indicators
EmpireOfCards.World       - Card3D, Hand3D, Board3D, SlotZone3D
EmpireOfCards.Save        - SaveManager
EmpireOfCards.Audio       - AudioManager
EmpireOfCards.VFX         - VFXManager
```
