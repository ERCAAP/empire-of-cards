# Empire of Cards — GDD Design Prompt

> Bu prompt, GDD yazarken kullanılacak tasarım direktiflerini içerir.
> Versiyon: 1.0 | Tarih: 2026-05-20

---

## GÖREV

Empire of Cards GDD'sini yaz veya güncelle.
GDD çok iyi olmalı. Aşağıdaki tüm sistemleri eksiksiz, derinlemesine açıkla.

---

## TEMEL GEREKSİNİMLER

GDD şunları kapsamalı:

- Oyun tahtasının fiziksel düzeni
- Tur sayısı algoritması (dinamik bitiş sistemi)
- Kartların oyunu nasıl etkilediği (kart → slot → efekt → müşteri → bölge)
- Board Slot System — oyunun kalbi
- Strateji build arketipleri
- Slot baskısının stratejiyi nasıl yarattığı

---

## OYUN TAHTASI DÜZENİ

Oyun tahtası 3 ana alana bölünmeli:

```
====================================================
TABLE SLOT / BOARD LAYOUT SYSTEM
====================================================

The tabletop board must NOT feel like a simple UI.

The business board itself is one of the CORE gameplay systems.

The GDD must deeply explain:
- the physical tabletop layout
- slot placement logic
- why slots are limited
- how slot pressure creates strategy
- how expansion changes board organization

====================================================
BOARD STRUCTURE
====================================================

The table is divided into 3 major areas:

1. Player Business Zone
2. District / Market Zone
3. Rival Business Zone

The player business is located at the bottom side of the table.

The rival business is located at the opposite side.

The district/market exists in the center and represents:
- customers
- traffic
- advertising visibility
- trends
- public activity
- customer flow

The district should visually and mechanically represent market competition.

====================================================
PLAYER BUSINESS SLOT SYSTEM
====================================================

The player business board uses a LIMITED SLOT SYSTEM.

Cards are NOT simply played and discarded.

Cards are physically placed into business slots and become part of the company structure.

This system should feel like:
"I am physically building and organizing my business."

The GDD must explain:
- slot logic
- slot pressure
- build archetypes
- why limited space creates strategic depth

====================================================
SLOT TYPES
====================================================

The starting business board contains:

1. OPERATION SLOTS (4)
Represents the physical business infrastructure.

Examples:
- Dining Tables
- Kitchen Upgrade
- Delivery Station
- Coffee Machine
- Oven
- Self-Service Counter

Purpose:
Controls operational capacity and customer throughput.

IMPORTANT:
Expansion can become dangerous if demand is low.

Example:
Adding too many tables:
- increases maintenance cost
- increases staffing needs
- wastes money if customer traffic is low

====================================================

2. STAFF SLOTS (5)
Represents active employees.

Examples:
- Chef
- Cashier
- Cleaner
- Delivery Driver
- Manager

Purpose:
Controls efficiency, speed, morale, and operational stability.

The GDD must explain:
- staffing pressure
- overstaffing
- understaffing
- morale interactions
- burnout chains

====================================================

3. MARKETING SLOTS (3)
Represents active marketing campaigns.

Examples:
- Flyer Campaign
- Influencer Deal
- Social Media Ads
- Google Ads
- Billboard Campaign

Purpose:
Controls customer acquisition and visibility.

IMPORTANT:
Too much marketing without operational support can damage reputation.

Example:
Massive customer influx:
- overwhelms kitchen
- slows service
- causes bad reviews

====================================================

4. SUPPLIER SLOTS (2)
Represents supplier agreements.

Examples:
- Premium Butcher
- Cheap Ingredients
- Organic Supplier
- Beverage Brand Partnership

Purpose:
Controls quality/cost balance.

The GDD must explain:
- supplier reliability
- cost pressure
- quality chains
- supply problems
- customer perception

====================================================

5. TEMP EFFECT SLOTS (3)
Represents temporary conditions and active crises.

Examples:
- Health Inspection
- Viral Trend
- Employee Strike
- Discount Week
- Social Media Scandal
- Food Poisoning Incident

Purpose:
Creates temporary pressure and dynamic gameplay changes.

====================================================
SLOT DESIGN PHILOSOPHY
====================================================

The player MUST NOT be able to do everything at once.

Limited slots create:
- specialization
- strategic identity
- meaningful tradeoffs
- operational tension

The GDD must explain:
- why slot limitation improves gameplay
- how players prioritize strategies
- how different builds emerge

====================================================
STRATEGIC BUILD EXAMPLES
====================================================

The GDD must explain possible build archetypes such as:

1. Aggressive Marketing Build
- many marketing campaigns
- rapid customer growth
- high operational collapse risk

2. Premium Quality Build
- strong suppliers
- expensive staff
- high customer loyalty
- slower expansion

3. Cheap Expansion Build
- low salaries
- cheap ingredients
- rapid scaling
- high reputation risk

4. Balanced Stable Build
- moderate growth
- lower risk
- slower domination

====================================================
CARD PLACEMENT PHILOSOPHY
====================================================

Cards should visually connect to the business world.

Examples:
- supplier cards connect to kitchen area
- marketing cards affect district area
- operation cards physically expand the restaurant
- temporary cards appear as active problems/events

The board should evolve visually during the run.

The player should feel:
"My business is physically growing, evolving, and collapsing on the table."
```

---

## SLOT TABLOSU (Başlangıç)

| Slot Türü   | Başlangıç Sayısı | Ne Gider | Stratejik Baskı |
|-------------|-----------------|----------|-----------------|
| Operation   | 4               | Masa, Mutfak, Tezgah, Fırın | Kapasite vs. Maliyet |
| Staff       | 5               | Aşçı, Kasiyer, Kurye, Müdür | Yeterli personel vs. Maaş yükü |
| Marketing   | 3               | Broşür, Influencer, Google Reklamı | Müşteri çekimi vs. Operasyon baskısı |
| Supplier    | 2               | Premium Tedarik, Ucuz Tedarik | Kalite vs. Maliyet dengesi |
| Temp Effect | 3               | Kriz, Salgın, Viral Trend | Anlık baskı ve kaos |

**Toplam başlangıç: 17 slot**

---

## TUR SAYISI ALGORİTMASI

Oyun sabit turda bitmez. Dinamik bitiş:

- **Kazanma:** 6/10 bölge alınca run anında biter
- **Kaybetme:** Rakip 7 bölge alırsa
- **Yumuşak cap:** Tur 25 sonrası her tura -%5 gelir (ikiye de)
- **Sert cap:** Tur 30 — kim fazla bölgeye sahipse kazanır

---

## KARTLARIN OYUNU ETKİLEMESİ

```
Kart Oyna
    ↓
Doğru Slota Yerleştir
    ↓
Slot Efekti Aktif
    ↓
Müşteri / Gelir / Kalite Değişir
    ↓
Bölge Dağılımı Güncellenir
    ↓
Kazanma / Kaybetme Koşulu Kontrol Edilir
```

Her slot türünün farklı etki zinciri var:
- **Operation slotu doluysa** → daha fazla müşteri kapasitesi
- **Staff slotu eksikse** → operasyon yavaşlar, müşteri kaybı
- **Marketing slotu aşırı doluysa** → müşteri gelir ama karşılayamazsın
- **Supplier kalitesi düşükse** → müşteri memnuniyeti düşer, sadakat azalır
- **Temp Effect slotu doluysa** → aktif kriz var, slot boşalana kadar etkisi devam eder

---

## GÖRSEL TASARIM NOTU

Slotlar sadece inventory görünmemeli.

Her slot işletmenin gerçek bir bölümünü temsil etmeli:
- Operation slotları → restoranın içi (fiziksel ekipman görünümü)
- Staff slotları → çalışan görselleri masada durmalı
- Marketing slotları → semt reklam alanı, billboard
- Supplier slotları → mutfak bağlantısı, depo
- Temp Effect slotları → aktif durum paneli (uyarı rengi)

**Tahtanın run boyunca görsel olarak büyümesi, değişmesi, kriz anlarında çöküş hissi vermesi şarttır.**

---

> Bu prompt, GDD güncellemelerinde ve yeni bölüm eklemelerinde referans olarak kullanılmalıdır.
