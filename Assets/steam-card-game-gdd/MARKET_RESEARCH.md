# Pazar Araştırması - Tabletop Business Deckbuilder

> Araştırma Tarihi: 2026-05-18
> Kaynaklar: Steam, SteamSpy, Gamalytic, SullyGnome, GameDiscover, Ars Technica
> Durum: Product strategy reference. Gameplay source of truth degildir.

---

## Yönetici Özeti

**En kritik bulgu:** Steam'de "deckbuilding + business/tycoon management" kombinasyonu **henüz yok**. Bu, üç kanıtlanmış kârlı türün kesişiminde boş bir alan.

Deckbuilder türü 2024-2026'da zirve yaşıyor (Balatro 5M+, StS2 ilk haftada 3M+). Non-combat deckbuilder trendi büyüyor. Business/tycoon oyunları her zaman satıyor. Ama ikisini birleştiren **tabletop hisli** bir oyun yok.

---

## 1. Rakip Analizi

### Doğrudan Rakipler (Deckbuilder + Management)

| Oyun | Tür | Neden tam rakip değil |
|---|---|---|
| Stacklands | Kart + survival/köy | Business değil, survival. Kart mekaniği farklı |
| Griftlands | Deckbuilder + müzakere | Combat odaklı, business ikincil |
| Luck be a Landlord | Slot machine roguelite | Deckbuilder değil, pasif sistem |

**Sonuç:** Doğrudan rakip yok. Bu hem fırsat hem risk.

### Dolaylı Rakipler - Deckbuilder Tarafı

| Oyun | Kopya | Gelir (Tahmini) | Metacritic | Dev | Süre |
|---|---|---|---|---|---|
| **Balatro** | 5M+ | $60-75M | 90-95 | Solo dev | 2.5 yıl |
| **Slay the Spire** | 10M+ | $90-200M | 89 | 3 kişi | 3 yıl |
| **Slay the Spire 2** | 4.6M (ilk hafta) | $92-108M | - | Mega Crit | - |
| **Inscryption** | 2-5M | ~$119M brüt | 85-87 | Küçük ekip | - |
| **Monster Train** | 500K-1M | ~$8-15M | 82 | Küçük stüdyo | - |
| **Stacklands** | 450K-1M | ~$10.4M brüt | - | Sokpop | - |
| **Luck be a Landlord** | ~807K | ~$4.8M brüt | 94% pozitif | Küçük indie | Godot |

### Dolaylı Rakipler - Management/Tycoon Tarafı

| Oyun | Kopya | Gelir | Önemli Not |
|---|---|---|---|
| **TCG Card Shop Sim** | 2.8M | ~$32M | Solo dev, ilk oyunu! Kart + tycoon = patlama |
| **Game Dev Tycoon** | 1M+ | ~$10M+ | Basit management, uzun ömürlü, 860+ mod |

### Önemli Referans: TCG Card Shop Simulator

Bu oyun **en yakın başarı referansımız**:
- Solo dev (OPNeon, Malezya), ilk oyunu
- 2.8M kopya, ~$32M brüt gelir
- İlk 10 günde 400K, ilk ayda 1M kopya
- Fiyat: $12.99
- Fark: O "kart dükkanı işletme sim", bizimki "kartlarla şirket empire kurma"

### Boş Alan Haritası

```
         DECKBUILDING
             |
      Balatro | Slay the Spire
             |
   ----------+----------→ TABLETOP HİSSİ
             |              Inscryption
      Stacklands
             |
   BUSINESS / MANAGEMENT SIM
    Game Dev Tycoon | TCG Card Shop Sim

   ★ BİZİM OYUN = Üçgenin ortası. Kimse yok.
```

---

## 2. Steam Market Verileri

### Tür Büyüklükleri

| Tag | Oyun Sayısı | Durum |
|---|---|---|
| Deckbuilding | ~3,000 | Kalabalık ama büyüyen |
| Roguelike Deckbuilder | ~1,385 | 2019'da 99'du → patlama |
| Management | ~5,000+ | Evergreen tür |
| Tycoon | ~2,000+ | Sürekli talep |

### Indie Gelir İstatistikleri (2024-2025)

| Metrik | Değer |
|---|---|
| Medyan indie oyun brüt geliri | **$249** (evet, iki yüz kırk dokuz dolar) |
| Ortalama ilk oyun geliri | **$120,000** (outlier'lar yüzünden yüksek) |
| Ortalama 2. oyun geliri | **$168,000** |
| Ortalama 3. oyun geliri | **$209,000** |
| $100K+ yıllık gelir yapan oyun | 5,863 / ~19,000 |
| Indie'nin Steam gelir payı | **%25** ($4.4B / $17.7B) |
| Yılda çıkan indie oyun | **~19,000** |

### Wishlist → Satış Dönüşüm

| Hedef | Gereken Wishlist |
|---|---|
| Minimum görünürlük | **7,000 - 10,000** |
| Altın seviye ($250K+) | **30,000 - 50,000** |
| İlk ay dönüşüm oranı | **~%27** |

---

## 3. Balatro Vaka Analizi (Solo Dev Referans)

Balatro bizim için en önemli referans. Solo dev'in ne yapabileceğinin kanıtı:

| Detay | Bilgi |
|---|---|
| Geliştirici | LocalThunk (1 kişi, Kanada) |
| Engine | Love2D (Lua) |
| Geliştirme süresi | ~2.5 yıl (Aralık 2021 → Şubat 2024) |
| Fiyat | $14.99 |
| İlk 8 saat | $1M brüt gelir |
| İlk ay | 1M kopya |
| Toplam | 5M+ kopya, tahmini $60-75M |
| Mobil | $9M+ IAP geliri |
| Ödüller | GDC GOTY, The Game Awards (3 ödül), BAFTA |
| Twitch 2026 | 1.6M saat izlenme |

**Başarı nedenleri:**
1. Poker = herkesin bildiği sistem (sıfır öğrenme eğrisi)
2. 150+ Joker kartla derin combo sistemi
3. "One more run" bağımlılığı
4. Geliştirici **hiç başka deckbuilder oynamamış** → taze bakış açısı
5. Güçlü mod desteği

---

## 4. Streamer / İçerik Üretici Potansiyeli

### Twitch Verileri (2026)

| Oyun | Yıllık İzlenme | Ort. İzleyici | Peak | Streamer Sayısı |
|---|---|---|---|---|
| Slay the Spire 2 | 21.6M saat | 11,830 | 130,977 | 15,200 |
| Balatro | 1.6M saat | 490 | 12,824 | 4,200 |
| Inscryption | 221.8K saat | 67 | 12,695 | 1,100 |
| Stacklands | 7.3K saat | 6 | 214 | 56 |

### Bizim Oyunun Stream Potansiyeli

**Güçlü yönler:**
- Business kararları = doğal yorum malzemesi ("Çalışanı kovmalı mıyım?")
- Rakip AI = izleyicilerin sevdiği düşmanlık narratifi
- Combo patlamaları = clip-worthy anlar
- Satirik eventler (crypto crash, cancel culture) = reaction content

**Riskler:**
- Management oyunları yavaş tempolu → combat deckbuilder'lardan daha az heyecanlı
- "Spreadsheet simulator" algısından kaçınılmalı
- Görsel feedback çok güçlü olmalı

---

## 5. Trendler (2024-2026)

### Yükselen Trendler

1. **Post-Balatro altın hücumu** → 100+ yeni deckbuilder Steam'e akıyor, farklılaşma ŞART
2. **StS2 türü yeniden doğruladı** → Kitle küçülmüyor, büyüyor
3. **Non-combat deckbuilder yükselişi** → Balatro, Stacklands, Luck be a Landlord
4. **Tabletop/cozy estetik trendi** → Inscryption'ın kabin hissi, Tiny Glade
5. **Mobil kritik hale geldi** → Balatro mobilde $4.4M (2 ayda)
6. **Mod desteği = uzun ömür** → Balatro, StS, Game Dev Tycoon
7. **Solo dev viability kanıtlandı** → Balatro, TCG Card Shop Sim, Vampire Survivors

### Yükselen Alt Türler
- Otomasyon deckbuilder'lar (Stacklands 2000 DLC)
- Narrative deckbuilder'lar (Inscryption)
- Score-chaser modeli (Balatro: düşman yok, skoru maximize et)
- Hibrit tür mashup'ları

---

## 6. Gelir Projeksiyonları

| Senaryo | Kopya (Yıl 1) | Brüt Gelir | Olasılık |
|---|---|---|---|
| Mega hit (Balatro seviyesi) | 1M+ | $15M+ | <%1 |
| Güçlü indie hit | 100K-500K | $1.5M-$7.5M | %5-10 |
| Sağlam indie | 20K-100K | $300K-$1.5M | %15-20 |
| Ortalama indie | 5K-20K | $75K-$300K | %30-40 |
| Ortalamanın altı | <5K | <$75K | %30-40 |

**"Sağlam indie" için gerekenler:**
- 10,000+ wishlist (lansman öncesi)
- %80+ pozitif Steam review
- En az 1-2 content creator coverage
- Net unique selling point
- Profesyonel store page + capsule art

---

## 7. Başarı ve Başarısızlık Faktörleri

### Başarılı Deckbuilder'ların Ortak Özellikleri
1. **Benzersiz hook:** "Deckbuilder + X" — X beklenmedik olmalı ✅ (business)
2. **Streamer dostu:** Paylaşılabilir anlar, kolay anlaşılır görsel ✅
3. **Lansman öncesi momentum:** Demo, streamer outreach, sosyal medya
4. **Topluluk inşası:** Geliştirme sırasında build in public
5. **Doğru fiyatlama:** $12.99-$14.99 sweet spot

### Başarısızlık Nedenleri
1. **"Bir deckbuilder daha" algısı** → Marketing'de BUSINESS öne çıkmalı
2. **Pazarlanabilir hook yok** → Farkı tek cümlede anlatamıyorsan bitti
3. **Karmaşık UI** → Streamer'lar ve yeni oyuncular kaçar
4. **Lansman öncesi kitle yok** → 0 wishlist = 0 algoritma desteği
5. **Tür yorgunluğu** → 1,385+ roguelike deckbuilder var

---

## 8. Stratejik Öneriler

### Pozisyonlama
**Pitch:** "Steam'in ilk deckbuilder'ı — zindan yerine şirket imparatorluğu kuruyorsun."

### Steam Etiketleri
- **Birincil:** Deckbuilder, Card Game, Management, Strategy
- **İkincil:** Roguelite, Tabletop, Economy, 2D, Singleplayer, Indie
- **Hedef:** Tycoon, Business Sim, Turn-Based Strategy

### Fiyat: $12.99 - $14.99

### Hedef Kitle
1. Balatro / StS oynamış, yeni deneyim arayan deckbuilder fanları
2. Game Dev Tycoon / TCG Card Shop Sim seven management oyuncuları
3. Satirik ton seven genç kitle (18-35)
4. İçerik üreticiler (karar anları = clip üretir)

### Risk Matrisi

| Risk | Olasılık | Etki | Azaltma |
|---|---|---|---|
| "Bir deckbuilder daha" algısı | Orta | Yüksek | Business hook öne çıkar |
| Kitle bölünmesi (deckbuilder vs tycoon) | Orta | Orta | Her iki kitleye hitap eden demo |
| Tempo problemi | Yüksek | Yüksek | Turları kısa tut, erken test et |
| Solo dev burnout | Yüksek | Yüksek | Scope küçük tut, MVP odaklı |
| Benzer oyun çıkması | Düşük | Yüksek | Hızlı prototip, erken Steam sayfası |

---

## 9. Zamanlama Değerlendirmesi

| Faktör | Durum |
|---|---|
| StS2 deckbuilder kitlesini aktive etti | ✅ Olumlu |
| Non-combat deckbuilder trendi | ✅ Büyüyor |
| TCG Card Shop Sim business+kart ilgisini kanıtladı | ✅ Olumlu |
| Nişimiz boş | ✅ Kimse yok |
| Tür kalabalıklığı | ⚠️ 1,385+ deckbuilder |
| Post-Balatro deckbuilder seli | ⚠️ Farklılaşma şart |

**Sonuç:** 2026 iyi zamanlama. Ama hook net olmalı, hızlı hareket edilmeli.
