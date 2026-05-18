# ASSET LİSTESİ — Empire of Cards (Unity)

> Prototip + MVP için gereken tüm asset'ler.
> Öncelik: P1 = prototip şart, P2 = MVP şart, P3 = polish/EA sonrası

---

## 1. UNITY PAKETLERİ / PLUGIN'LER

| Paket | Ne için | Ücretsiz? | Öncelik |
|---|---|---|---|
| **DOTween Pro** | Kart animasyonları, juice, tween | $15 (free versiyonu da var) | P1 |
| **TextMeshPro** | UI text, kart yazıları | Dahili (ücretsiz) | P1 |
| **Steamworks.NET** | Steam entegrasyonu | Ücretsiz | P2 |
| **NaughtyAttributes** | Inspector düzenleme, debug | Ücretsiz | P1 |
| **Newtonsoft JSON** | Save sistemi, kart data | Ücretsiz | P1 |
| **UniTask** | Async operasyonlar | Ücretsiz | P2 |

---

## 2. GÖRSEL ASSET'LER

### 2a. Kart Görselleri

| Asset | Detay | Adet | Öncelik |
|---|---|---|---|
| Kart çerçevesi — İşletme | Mavi tema, slot göstergesi | 1 template | P1 |
| Kart çerçevesi — Çalışan | Yeşil tema, maaş göstergesi | 1 template | P1 |
| Kart çerçevesi — Action | Kırmızı tema, tek kullanım işareti | 1 template | P1 |
| Kart çerçevesi — Upgrade | Mor tema, kalıcı işareti | 1 template | P1 |
| Kart çerçevesi — Event | Sarı/turuncu tema, gazete stili | 1 template | P1 |
| Kart arka yüzü | Logo + desen | 1 | P1 |
| Nadirlik göstergeleri | ★ gri, ★★ yeşil, ★★★ mavi, ★★★★ mor, ★★★★★ altın | 5 varyant | P1 |
| **Kart ikonları** | Her kart için benzersiz ikon | 40 adet | P2 |

**Prototip için:** 5 template çerçeve + basit ikonlar yeterli. Unique art P2.

**İkon stili önerisi:** Flat/line art, tek renk, 128x128px. Kenney.nl veya game-icons.net'ten ücretsiz alınabilir.

### 2b. Masa / Board Görselleri

| Asset | Detay | Adet | Öncelik |
|---|---|---|---|
| Masa arka planı | Ahşap/kumaş doku, koyu ton | 1 | P1 |
| İşletme slot çerçevesi | Boş slot göstergesi, "buraya koy" hissi | 1 template | P1 |
| Çalışan alt-slot | İşletmenin altında küçük yuvalar | 1 template | P1 |
| Market alanı | Ortadaki paylaşılan alan, müşteri havuzu | 1 | P1 |
| Rakip alanı | Üstteki düşman bölgesi | 1 | P1 |
| El alanı (hand zone) | Alttaki kart dizilim alanı | 1 | P1 |
| Deste görseli | Kart yığını | 1 | P1 |
| Çöp/discard görseli | Atılan kart yığını | 1 | P1 |
| Dükkan paneli | 3 kart gösterim alanı, overlay | 1 | P2 |

### 2c. Token / Jeton Görselleri

| Asset | Detay | Adet | Öncelik |
|---|---|---|---|
| Müşteri token'ı | Küçük insan ikonu, renkli | 3 varyant (küçük/orta/büyük grup) | P1 |
| Para ikonu | Madeni para, altın | 1 (+ animasyon sprite) | P1 |
| FBI rozeti | Kırmızı-mavi rozet | 1 | P2 |
| Combo yıldızı | Combo tetiklenince beliren | 1 | P1 |
| Market share barı | İlerleme çubuğu | 1 | P1 |

### 2d. UI Elementleri

| Asset | Detay | Adet | Öncelik |
|---|---|---|---|
| Buton — ana | "Tur Bitir", "Satın Al", vs. | 1 template (normal/hover/pressed) | P1 |
| Panel arka planı | Popup, dükkan, deste görünümü | 2-3 varyant | P1 |
| Para göstergesi | Sol üst, 💰 + sayı | 1 | P1 |
| Market share göstergesi | Bar + yüzde | 1 | P1 |
| Aksiyon göstergesi | ●●● dolup boşalan | 1 | P1 |
| FBI risk göstergesi | Tehlike barı, kırmızıya döner | 1 | P2 |
| Etik göstergesi | (MVP'de basit, bar yeterli) | 1 | P3 |
| Tur sayacı | "Tur 8/20" | 1 | P1 |
| Skor ekranı | Run sonu, skor dökümü | 1 panel | P2 |
| Ana menü arka planı | Masa üstü atmosfer | 1 | P2 |

### 2e. Efekt / Particle

| Asset | Detay | Öncelik |
|---|---|---|
| Para yağmuru | Madeni paralar düşer | P1 |
| Screen shake | Kamera titremesi (kod ile yapılır) | P1 |
| Combo text pop | "COMBO!" büyüyerek belirir | P1 |
| Kart parıltısı | Nadir kart açılınca | P2 |
| Kırmızı flash | FBI baskını, kötü event | P2 |
| Altın glow | İyi event, kazanma | P2 |
| Yaprak efekti | Organik combo | P3 |
| Karanlık gölge | İllegal combo | P3 |

---

## 3. SES ASSET'LERİ

### 3a. Ses Efektleri (SFX)

| Ses | Kullanım | Öncelik | Ücretsiz Kaynak |
|---|---|---|---|
| card_place.wav | Kart slota konunca | P1 | Kenney.nl, freesound.org |
| card_draw.wav | Kart çekince | P1 | " |
| card_flip.wav | Event açılınca | P1 | " |
| coin_single.wav | Küçük para kazanma | P1 | " |
| coin_cascade.wav | Büyük para / combo | P1 | " |
| combo_trigger.wav | Combo tetiklenince | P1 | " |
| combo_mega.wav | 3'lü combo | P2 | " |
| customer_arrive.wav | Müşteri gelince (pop) | P2 | " |
| rival_move.wav | Rakip hamle yapınca | P2 | " |
| fbi_siren.wav | Baskın! | P2 | " |
| upgrade_ding.wav | Upgrade yerleşince | P2 | " |
| negative_buzz.wav | Kötü olay, para kaybı | P1 | " |
| button_click.wav | UI tıklama | P1 | " |
| turn_start.wav | Yeni tur sesi | P2 | " |
| win_fanfare.wav | Kazanma | P2 | " |
| lose_sad.wav | Kaybetme | P2 | " |

**Minimum P1 ses sayısı: 8**

### 3b. Müzik

| Parça | Durum | Süre | Öncelik | Kaynak Önerisi |
|---|---|---|---|---|
| main_calm.ogg | Erken turlar, dükkan | 2-3 dk loop | P2 | Ücretsiz: incompetech.com, opengameart.org |
| main_intense.ogg | Geç turlar, rakip baskısı | 2-3 dk loop | P2 | " |

**Prototipte müzik gerekmez.** MVP'de 2 parça yeter (crossfade).

---

## 4. FONT'LAR

| Font | Kullanım | Öncelik | Kaynak |
|---|---|---|---|
| Ana UI font | Menüler, butonlar, para | P1 | Google Fonts: Inter, Poppins, veya Nunito |
| Kart başlık font | Kart isimleri | P1 | Google Fonts: Oswald, Bebas Neue |
| Combo/efekt font | "COMBO!", "VIRAL!", skor | P1 | Google Fonts: Bangers, Bungee |
| Mono font | Sayılar, istatistikler | P2 | Google Fonts: JetBrains Mono, Space Mono |

**Tümü ücretsiz, ticari kullanıma uygun olmalı.**

---

## 5. UNITY PROJE YAPISI

```
Assets/
├── _Project/
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GameManager.cs
│   │   │   ├── TurnManager.cs (state machine)
│   │   │   └── Constants.cs
│   │   ├── Cards/
│   │   │   ├── CardData.cs (ScriptableObject)
│   │   │   ├── CardUI.cs (MonoBehaviour - görsel)
│   │   │   ├── CardDrag.cs (sürükle-bırak)
│   │   │   └── CardFactory.cs
│   │   ├── Board/
│   │   │   ├── BoardManager.cs
│   │   │   ├── BusinessSlot.cs
│   │   │   ├── EmployeeSlot.cs
│   │   │   ├── HandManager.cs
│   │   │   └── MarketArea.cs
│   │   ├── Economy/
│   │   │   ├── EconomyManager.cs
│   │   │   ├── CustomerPool.cs
│   │   │   └── MarketShare.cs
│   │   ├── Combo/
│   │   │   ├── ComboData.cs (ScriptableObject)
│   │   │   ├── ComboSystem.cs
│   │   │   └── ComboVFX.cs
│   │   ├── AI/
│   │   │   ├── RivalAI.cs
│   │   │   └── RivalData.cs (ScriptableObject)
│   │   ├── Events/
│   │   │   ├── EventData.cs (ScriptableObject)
│   │   │   └── EventSystem.cs
│   │   ├── UI/
│   │   │   ├── UIManager.cs
│   │   │   ├── MoneyDisplay.cs
│   │   │   ├── MarketShareBar.cs
│   │   │   ├── TurnCounter.cs
│   │   │   ├── ShopPanel.cs
│   │   │   └── ScoreScreen.cs
│   │   ├── Save/
│   │   │   ├── SaveManager.cs
│   │   │   └── SaveData.cs
│   │   └── Juice/
│   │       ├── ScreenShake.cs
│   │       ├── CoinRain.cs
│   │       ├── ComboText.cs
│   │       └── CardTween.cs
│   ├── Data/
│   │   ├── Cards/ (40 adet ScriptableObject)
│   │   │   ├── Business/
│   │   │   ├── Employee/
│   │   │   ├── Action/
│   │   │   ├── Upgrade/
│   │   │   └── Event/
│   │   ├── Combos/ (10 adet ScriptableObject)
│   │   └── Rivals/ (1 adet başlangıç)
│   ├── Art/
│   │   ├── Cards/ (template + ikon)
│   │   ├── Board/ (masa, slot)
│   │   ├── UI/ (buton, panel, bar)
│   │   ├── Tokens/ (müşteri, para)
│   │   └── Effects/ (particle, sprite)
│   ├── Audio/
│   │   ├── SFX/
│   │   └── Music/
│   ├── Fonts/
│   ├── Prefabs/
│   │   ├── Card.prefab
│   │   ├── BusinessSlot.prefab
│   │   ├── CustomerToken.prefab
│   │   └── CoinEffect.prefab
│   └── Scenes/
│       ├── MainMenu.unity
│       ├── Game.unity
│       └── Score.unity
├── Plugins/
│   └── DOTween/
└── StreamingAssets/ (save dosyaları)
```

---

## 6. ÖNCELİK ÖZETİ

### P1 — Prototip (Hafta 1-4)
Bunlar olmadan prototip çalışmaz:
- [ ] 5 kart çerçeve template'i
- [ ] Masa arka planı (basit)
- [ ] Slot görselleri (placeholder)
- [ ] Para + müşteri ikonları
- [ ] 8 temel ses efekti
- [ ] 3 font
- [ ] DOTween + TextMeshPro
- [ ] Screen shake + coin rain + combo text (basit)
- [ ] Unity proje yapısı

### P2 — MVP / Demo (Hafta 5-16)
Demo kalitesi için:
- [ ] 40 kart ikonu (unique)
- [ ] Dükkan paneli
- [ ] Skor ekranı
- [ ] FBI göstergesi
- [ ] Tüm ses efektleri (16)
- [ ] 2 müzik parçası
- [ ] Particle efektleri
- [ ] Kart parıltısı / glow
- [ ] Steam entegrasyonu

### P3 — Polish / EA (Hafta 17+)
Lansman kalitesi:
- [ ] Kart unique art (illüstrasyon)
- [ ] Combo'ya özel animasyonlar
- [ ] Ekstra particle (yaprak, gölge, hologram)
- [ ] Sesli anlatıcı / rakip seslendirme
- [ ] Müzik varyasyonları (5+ parça)
- [ ] Ana menü animasyonu

---

## 7. ÜCRETSİZ ASSET KAYNAKLARI

| Kaynak | Ne için | Link |
|---|---|---|
| Kenney.nl | İkon, UI, ses efekti | kenney.nl |
| game-icons.net | 4000+ oyun ikonu (CC) | game-icons.net |
| OpenGameArt.org | Müzik, ses, sprite | opengameart.org |
| Freesound.org | Ses efektleri | freesound.org |
| Incompetech.com | Royalty-free müzik | incompetech.com |
| Google Fonts | Ticari fontlar | fonts.google.com |
| itch.io asset packs | Kart, UI paketleri | itch.io/game-assets |

---

## 8. TAHMİNİ MALİYET

| Kalem | Ücretsiz yol | Ücretli yol |
|---|---|---|
| Kart ikonları | game-icons.net (CC) | Fiverr: $200-500 (40 ikon) |
| Kart çerçeve | Kendte yap (Figma/Canva) | Fiverr: $50-100 (5 template) |
| Ses efektleri | Kenney + Freesound | Ücretsiz yeterli |
| Müzik | Incompetech / OGA | Fiverr: $100-300 (2 parça) |
| Font | Google Fonts | Ücretsiz yeterli |
| DOTween Pro | Free sürüm | $15 |
| **TOPLAM** | **$0-15** | **$365-915** |

**Prototip maliyeti: $0.** Her şey ücretsiz kaynaklarla yapılabilir.
