# TECH MOBİL UYGULAMA — İşletme Dosyası
> Empire of Cards | Versiyon: 1.0 | Tarih: 2026-05-20

---

## 1. İşletmeye Genel Bakış

Tech mobile app = en düşük giriş maliyeti ama en yüksek risk. Solo dev sıfır TL ile başlayabilir (kendi zamanı), ama uygulamaların %90'ı hiçbir gelir üretmez. Başarılı olanlar ise inanılmaz ölçeklenir.

**Zorluklar:** Product-market fit bulmak çok zor, App Store/Play Store komisyonları (Apple %30, Google %15–30), kullanıcı edinme maliyeti (CAC) sürekli artıyor, churn oranı yüksek (%5–15/ay), gelir 3–18 ay gecikme ile başlar.

**Fırsatlar:** Ölçeklenebilirlik neredeyse sınırsız, marginal maliyet neredeyse sıfır (1 vs 1M kullanıcı için aynı kod), recurring revenue (SaaS/subscription), global pazar erişimine anında.

---

## 2. Başlangıç Gereksinimleri

**Sermaye:**
- Solo dev MVP: 20K–120K TL
- Küçük ekip: 350K–900K TL
- Profesyonel: 1.5M–4.5M TL

**Zorunlu hesaplar:**
- Apple Developer: $99/yıl (~3.5K TL)
- Google Play: $25 tek seferlik
- Domain + hosting: 2K–5K TL/yıl
- Hukuki (KVKK, kullanım şartları): 10K–30K TL

**Platform seçimi (oyun başında):**
| Platform | Avantaj | Dezavantaj |
|---|---|---|
| Sadece Android | Hızlı yayın, az kural, 30 gün + ayın 15'i ödeme | Düşük ARPU |
| Sadece iOS | Yüksek kaliteli kullanıcı, yüksek ARPU | Apple sıkı kurallar, 45 gün ödeme gecikmesi |
| Her ikisi | Geniş kitle | 2x maliyet, 2x compliance |

**Uygulama türü seçimi (oyun başında):**
| Tür | Guideline Zorluğu | Onay Süresi |
|---|---|---|
| Utility App | Basit | 1–3 gün |
| Sosyal/Mesajlaşma | Veri gizliliği, GDPR | 1–2 hafta |
| Video Generator/AI Araç | İçerik politikaları, yüksek denetim | 1–4 hafta |
| Oyun | IAP kuralları, çocuk güvenliği | 3–7 gün |

**İlk 3 adım:**
1. Problem doğrula (landing page + anket)
2. MVP geliştir (1–2 özellik, minimum)
3. İlk 100 kullanıcı bul (Reddit, ProductHunt, arkadaşlar)

**Fatal mistakes:** Feature creep (50 özellik planla, 0 bitir), erken altyapı harcama, monetization'ı sonraya bırakma, App Store kurallarını bilmemek.

---

## 3. Gelir Modeli

**Modeller:**
| Model | Gelir/Kullanıcı/ay | Not |
|---|---|---|
| Freemium + IAP | $0.50–5 | %95–98 hiç ödeme yapmaz |
| Subscription/SaaS | $5–30 | Churn %5–15/ay |
| Tek seferlik satış | $1–10 | Büyüme zor |
| Reklam | $0.01–0.10 | Çok kullanıcı lazım |

**Platform komisyonları:**
- Apple App Store: %30 (ilk 1M$/yıl küçük işletmelerde %15), ödeme **45 gün** sonra
- Google Play: %15 ilk 1M$, %30 sonrası, ödeme her **ayın 15'i** (~30 gün)
- Web/doğrudan: %0 ama Stripe/iyzico %2.49–3.5

**SaaS büyüme pattern'i:**
- Ay 1–3: $0–500/ay
- Ay 4–6: $500–2000/ay
- Ay 7–12: $2000–10K/ay
- Yıl 2+: $10K–50K/ay (product-market fit varsa)
- GERÇEK: %90 app bu rakamlara ulaşamaz.

**Ödeme gecikmesi mekaniği (oyun için kritik):**
- iOS sattıysan parayı 45 gün sonra alırsın → nakit akışı kritik
- Android sattıysan parayı ayın 15'inde alırsın → maksimuim 45 gün bekleyebilirsin
- Bu gecikme nedeniyle kârlı app bile nakit krizine girebilir

---

## 4. Maliyet Yapısı

**Backend altyapı (kullanıcı sayısına göre):**

| Servis | 1K kullanıcı | 10K | 100K | 1M |
|---|---|---|---|---|
| Firebase | $0–25/ay | $25–100 | $500–2K | $5K–10K |
| Cloudflare Workers | $0–5 | $5–25 | $50–200 | $200–800 |
| AWS | $10–50 | $50–200 | $500–2K | $5K–20K |

**Firebase vs Cloudflare kararı:**
- Firebase = kolay başlangıç ama pahalı ölçekleme + yüksek vendor lock-in
- Cloudflare Workers = daha ucuz ölçekleme (%60–70 tasarruf 1M kullanıcıda), düşük lock-in
- Oyunda: "Backend Upgrade" kartı Firebase → Cloudflare geçişini temsil eder

**Image format etkisi (CDN maliyeti):**
| Format | Boyut | CDN Maliyeti (1M kullanıcı) |
|---|---|---|
| PNG | 500KB–2MB | $3K–5K/ay |
| JPG | 100KB–500KB | $1K–2K/ay |
| WebP | 50KB–300KB | $800–1.5K/ay |

**Gizli maliyetler:**
- Apple review red = 1–2 hafta gecikme (gelir kaybı)
- Security patch'leri = sürekli zaman yatırımı
- Müşteri destek = zaman veya para
- Legal compliance (KVKK, GDPR) = 10K–30K TL

---

## 5. Müşteri Edinme Yolları

| Kanal | CAC | Not |
|---|---|---|
| Organic (ASO) | $0 | Yavaş ama kaliteli |
| Product Hunt | $0 | Tek seferlik spike |
| Google Ads | $0.50–5/install | Pahalılaşıyor |
| Meta Ads | $0.30–3/install | Fraud riski |
| Influencer | $500–10K/kampanya | Ölçülmesi zor |
| Referral sistemi | Düşük | Viral katsayı artışı |

**ASO faktörleri:** App ismi (keyword içermeli), ekran görüntüleri (ilk 2 screenshot karar verdirtir), rating 4.0+ (altı = -%50 indirme), 100+ yorum = güvenilirlik.

---

## 6. Tech Stack / Tedarik Zinciri

**Core tech decisions:**
| Kategori | Ucuz Seçenek | Premium Seçenek |
|---|---|---|
| Auth | Firebase Auth (ücretsiz) | Auth0 ($23+/ay) |
| Database | SQLite/Supabase free | PlanetScale ($29+/ay) |
| Push | Firebase FCM (ücretsiz) | OneSignal ($9+/ay) |
| Analytics | Firebase (ücretsiz) | Mixpanel ($25+/ay) |
| CDN | Cloudflare free | Pro ($20/ay) |
| Images | PNG/JPG | WebP (-%60–70 CDN maliyet) |

---

## 7. Dijital Varlık

**App Store rating etkisi:**
- 4.5–5.0: optimum, Editor's Choice potansiyeli
- 4.0–4.4: -%10–20 indirme
- 3.5–3.9: -%30–50 indirme
- 3.0 altı: -%80+, pratik olarak ölü

**Sahte review riskleri:** Apple = uygulamayı kaldırır (en ağır ceza). Google = review silinir + ranking düşüşü. $1–5/review maliyeti ama yakalanma riski yüksek.

**ASO optimizasyonu:** İyi ekran görüntüleri + keyword'lü başlık + 4.5+ rating = organik indirme %200–400 artabilir.

---

## 8. Rakip Dinamikleri

**Rakip yatırım aldıysa (Seed Round):**
- $50K–500K/ay reklam bütçesi
- Developer'larına 3x maaş teklifi
- Premium özellikleri ücretsiz yapar
- 3–6 ayda tüm özelliklerini kopyalar
- TechCrunch/ProductHunt spotlight

**AI entegrasyonunun önemi:**
- Rakip AI entegre ettiyse: değer önerisi güçlenir, maliyet düşer
- AI app başka AI araçlarıyla maliyet optimize edebilir
- "AI Devrimi" event'i aktifken AI entegreli app gelir x1.5

**Büyük firma alanına girerse:** Genellikle ölürsün. "Google bunu yaptıysa biz niye varız?" algısı.

---

## 9. Kart Listesi

### Geliştirme Kartları
| Kart | Etki | Maliyet |
|---|---|---|
| MVP Yayınla | Kullanıcı edinme başlar | 50 |
| Performans Optimizasyonu | Retention +%20 | 80 |
| A/B Test | Conversion +%15 | 40 |
| Dark Mode | Kullanıcı memnuniyeti +1 | 30 |
| Push Notification | Retention +%10 | 20 |
| Bug Fix Sprint | Crash rate -%50, rating +0.3 | 60 |

### Backend/Altyapı Kartları
| Kart | Etki | Maliyet |
|---|---|---|
| Firebase Kullan | Hızlı başlangıç | 20 | Ölçeklemede pahalı |
| Cloudflare Geç | Backend maliyet -%40 | 100 |
| CDN Kur | Yükleme hızı +, kullanıcı memnuniyeti +1 | 60 |
| WebP Geçişi | CDN maliyet -%60 | 40 |
| Veritabanı Optimize | Maliyet -%20, hız + | 80 |
| Self-Host | Tam kontrol, bakım yükü | 150 |

### Monetizasyon Kartları
| Kart | Etki | Maliyet |
|---|---|---|
| Freemium Model | Kullanıcı artışı hızlı, gelir yavaş | 30 |
| Abonelik Sistemi | Sabit gelir, churn yönetimi | 80 |
| IAP Ekle | Anlık gelir potansiyeli | 40 | Apple %30 keser |
| Reklam Entegrasyonu | Sabit gelir, UX bozulur | 30 |
| Enterprise Sözleşmesi | Büyük gelir, özelleştirme yükü | — |

### Marketing Kartları
| Kart | Etki | Maliyet |
|---|---|---|
| ASO Optimizasyonu | Organik indirme +%30 | 50 |
| Product Hunt Lansmanı | Kullanıcı spike (2 tur) | 80 |
| Influencer Kampanyası | Kullanıcı +10 bu tur | 120 |
| Google Ads | Kullanıcı +5/tur (süreli) | 60/tur |
| Referral Sistemi | Viral katsayı +0.3 | 60 |

### AI/Teknoloji Kartları
| Kart | Etki | Maliyet |
|---|---|---|
| Yapay Zeka Entegre Et | Değer önerisi +, geliştirme 100 | 100 |
| AI ile Maliyet Optimize | Backend maliyet -%20 | 60 |
| Otomatik Müşteri Desteği | Destek maliyeti -%80 | 80 |

### Risk/İllegal Kartlar
| Kart | Kazanç | Risk |
|---|---|---|
| Apple Guideline İhlali | Hızlı özellik | App Store'dan kaldırılma |
| Kullanıcı Verisi Sat | Gelir +50 bu tur | KVKK: 50K–2M TL + ciro oranı |
| Rakip API Scraping | Veri avantajı | Dava + API block |
| Sahte İndirme/Review | Ranking boost | App Store ban |
| GPL Lisans İhlali | Hızlı geliştirme | Hukuki dava |

---

## 10. Özel Mekanikler

**Delay mekaniği:** Tech App başlangıçta 2–3 tur gelir yok. Gerçek MVP geliştirme süresini modelliyor.

**Ödeme gecikmesi sistemi:**
- iOS seçtiysen: her tur gelirin 45 gün sonra gelir → nakit kriz riski
- Android seçtiysen: ayın 15'inde ödeme → daha az bekleme
- Her ikisi: daha geniş kitle ama 2x maliyet

**Platform komisyonu:** Her tur gelirin otomatik %30'u (Apple) veya %15–30'u (Google) kesilir.

**Scaling cost mekaniği:** Kullanıcı artınca backend maliyeti de artar. Firebase kullanıyorsan 10K kullanıcıda maliyet spike yaşarsın.

**Churn rate:** Her tur kullanıcıların %5–15'i churn olur. Retention kartları bu oranı düşürür.

**App Store rating döngüsü:** Bug → 1 yıldız → rating düşer → indirme azalır → gelir düşer → geliştirme yavaşlar → daha fazla bug.

---

## 11. Event Listesi

| Event | Tetikleyici | Etki | Süre |
|---|---|---|---|
| Veri Sızıntısı | Güvenlik yatırımı düşükse | Tech kullanıcı -5, rating -0.5 | 1 tur |
| App Store Red | Apple guideline ihlali | 1–2 tur gelir yok | 2 tur |
| Sunucu Çökmesi | Firebase limiti aşıldıysa | 1 tur üretim yapamaz | 1 tur |
| Product Hunt #1 | İyi rating + güçlü uygulama | Kullanıcı x3 bu tur | 1 tur |
| AI Devrimi | Random | AI entegreli app gelir +%50 | 2 tur |
| App Store Politika Değişikliği | Random | Tech gelir -%20 | 2 tur |
| Rakip Yatırım Aldı | Rakip büyük hamle | Rakip kullanıcı +15, marketing x2 | 3 tur |
| Viral Trend | Yüksek kullanıcı + iyi app | Kullanıcı x3, backend spike | 1 tur |
| KVKK Denetimi | Kullanıcı verisi toplanıyorsa | 100K+ TL ceza | 1 tur |

---

## 12. Zincir Reaksiyon Örnekleri

**Domino 1 — "Sunucu Çöküşü":**
```
Ucuz $5 VPS ile başla
↓ 10K kullanıcıya geçince yavaşlar
↓ App 5+ sn yüklenir
↓ Kullanıcılar terk eder
↓ Rating 4.5 → 3.8
↓ App Store sıralaması düşer
↓ Yeni kullanıcı gelmez
↓ Gelir durur
↓ Upgrade edecek para yok
↓ ÖLÜM SPİRALİ
```

**Domino 2 — "Feature Creep":**
```
Her özellik isteğini ekle
↓ Uygulama karmaşıklaşır
↓ Bug artar
↓ 1-star review birikir
↓ Developer burnout
↓ Güncelleme azalır
↓ Apple "stale" işaretler
↓ İndirme düşer
↓ TERK
```

**Domino 3 — "Parasız Büyüme":**
```
Ücretsiz kullanıcılara odaklan
↓ 50K kullanıcı, $0 gelir
↓ Sunucu $500/ay
↓ Cebinden ödüyorsun
↓ Para biter
↓ Sunucu kapanır
↓ 50K kullanıcı anında kayıp
↓ KAPANIS
```

---

## 13. Oyun Balans Notları

| Metrik | Oyun Değeri | Gerçek Dünya | Uyum |
|---|---|---|---|
| Tech Startup delay | 2–3 tur | MVP geliştirme süresi | Doğru |
| Platform komisyonu | Gelirin %30 kesilir | Apple %30, Google %15–30 | Doğru |
| Ödeme gecikmesi | Nakit kriz riski | Apple 45 gün, Android 30 gün | Doğru |
| Firebase → Cloudflare | Maliyet -%40 | Gerçek %60–70 tasarruf | Doğru |
| WebP tasarrufu | CDN -%60 | Gerçek %60–70 | Doğru |
