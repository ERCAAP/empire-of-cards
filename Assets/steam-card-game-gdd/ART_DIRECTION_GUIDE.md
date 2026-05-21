# ART DIRECTION & ASSET GUIDE — Empire of Cards

> Versiyon: 1.0 | Tarih: 2026-05-18
> GDD v2.0 baz alınarak hazırlanmıştır.

---

# BÖLÜM 1: GEREKLİ 3D OBJELER VE ASSET'LER

## 1.1 Neden 3D?

GDD "Flat 2D + depth" diyor. Ama masa üstü hissini vermek için **2.5D yaklaşım** (3D objeler + orthographic kamera) en doğru yol. Kartlar 2D sprite, ama masa, tokenlar ve bazı UI elementleri 3D modellenerek derinlik ve fiziksellik hissi kazanır.

## 1.2 Masa & Ortam (Environment)

| Obje | Açıklama | Polygon Hedef | Öncelik |
|---|---|---|---|
| **Ana Masa** | Ahşap masa, kenarları yuvarlak, toon shader ile. Masanın üstünde hafif kumaş/keçe doku. Poker masası hissi ama business temalı. | 500-800 poly | P1 |
| **Masa Kenarları** | Masanın etrafında hafif yükseltilmiş kenar (boardgame çerçevesi hissi) | Masaya dahil | P1 |
| **Arka Plan Oda** | Bulanık/basit bir ofis ortamı. Pencere, raf, bitki. Masanın arkasında derinlik vermek için. Focus masada. | 1000-1500 poly (toplamda) | P3 |
| **Masa Lambası** | Sıcak ışık kaynağı. Masanın sol üstünden aydınlatma. Toon shading ile. | 200-300 poly | P3 |

## 1.3 Kart Objeleri

| Obje | Açıklama | Polygon Hedef | Öncelik |
|---|---|---|---|
| **Kart Modeli (Generic)** | Tek bir 3D kart modeli. Düz dikdörtgen, hafif kalınlık (0.5mm hissi), yuvarlatılmış kenarlar. Material/texture swap ile tüm kartlar bu modelden üretilir. | 100-200 poly | P1 |
| **Kart Destesi** | 3-4 kart üst üste yığılmış görsel. Draw pile ve discard pile için. | 300-500 poly | P1 |
| **Kart Holder/Fan** | Elde tutulan kartların fan şeklinde dizilimi için guide obje (opsiyonel, kod ile de yapılabilir) | - | P3 |

## 1.4 Token & Jeton Objeleri

| Obje | Açıklama | Polygon Hedef | Öncelik |
|---|---|---|---|
| **Müşteri Token** | Küçük yuvarlak jeton, üstünde insan silueti. 3 renk varyantı: mavi (senin), kırmızı (rakip), gri (serbest). Fiziksel jeton hissi, hafif tıklama sesi ile masaya düşer. | 50-100 poly | P1 |
| **Para Token (Coin)** | Altın madeni para. Kenarında çizgiler. Bir yüzünde "$" veya "E" (Empire). Coin rain efekti için çok sayıda spawn edilecek. | 80-120 poly | P1 |
| **FBI Rozeti** | Kırmızı-mavi rozet. FBI baskını animasyonunda masanın ortasına düşer. | 100-150 poly | P2 |
| **Yıldız Token** | Combo tetiklendiğinde beliren altın yıldız. Parıltı efekti ile. | 30-50 poly | P2 |

## 1.5 Bölge Haritası (Territory Map)

| Obje | Açıklama | Polygon Hedef | Öncelik |
|---|---|---|---|
| **Bölge Kutusu** | 10 adet küçük kare/dikdörtgen blok. Her biri renk değiştirebilir (mavi/kırmızı/gri). Fiziksel hisli, domino taşı gibi. | 20-40 poly (x10 = 200-400) | P1 |
| **Bölge Çerçevesi** | 10 kutuyu saran çerçeve. "Market Share" yazısı. | 100-200 poly | P1 |
| **Bayrak/İşaret** | Bölge el değiştirdiğinde küçük bir bayrak animasyonu (opsiyonel, 2D sprite da olabilir) | 50 poly | P3 |

## 1.6 Slot Objeleri

| Obje | Açıklama | Polygon Hedef | Öncelik |
|---|---|---|---|
| **İşletme Slot Çerçevesi** | Kart konacak alan. Hafif çukur/platform. Boşken "+" ikonu görünür. Doluyken kart oturur. Glow efekti (valid drop target). | 50-100 poly | P1 |
| **Çalışan Alt-Slot** | İşletme slotunun altında küçük yuvalar. İşletmeye bağlı 1-3 arası. | 30-50 poly | P1 |
| **Upgrade Slot** | Masanın alt kısmında upgrade kartları için alan. | 50-100 poly | P2 |

## 1.7 UI 3D Elementler

| Obje | Açıklama | Polygon Hedef | Öncelik |
|---|---|---|---|
| **Aksiyon Noktası (Dot)** | 3D küçük top/jeton. Dolu = parlak, boş = karartılmış. 3-4 adet. | 20-30 poly | P2 |
| **Tur Çarkı** | "Tur 8/15" göstergesi. Mekanik çark hissi (opsiyonel, UI ile de yapılabilir). | 200-300 poly | P3 |
| **Dükkan Tabelası** | "SHOP" yazılı küçük tabela. Dükkan butonunun üstünde. | 50-100 poly | P3 |

## 1.8 Efekt Objeleri

| Obje | Açıklama | Polygon Hedef | Öncelik |
|---|---|---|---|
| **Combo Patlaması** | Yıldız + halka şeklinde patlama. Particle system + basit mesh. | Particle based | P1 |
| **Screen Flash Quad** | Tam ekran renkli flash için basit quad. | 4 poly | P1 |
| **Duman/Sis** | Sabotaj, kapanma efektleri için. | Particle based | P2 |

## 1.9 Toplam Polygon Bütçesi

| Kategori | Tahmini Polygon |
|---|---|
| Masa + Ortam | 2000-3000 |
| Kartlar (aktif 15-20 kart) | 2000-4000 |
| Tokenlar (50-100 aktif) | 5000-10000 |
| Slotlar + Harita | 1000-2000 |
| UI 3D | 500-1000 |
| **Toplam** | **~10,000-20,000 poly** |

Bu son derece düşük. Toon shader ile güzel görünecek, performans sorunu olmayacak.

---

# BÖLÜM 2: TOON YAPI — ART STYLE GUIDE

## 2.1 Referans Tarz

**"Satirik Business Karikatür + Board Game Fizikselliği"**

Referans oyunlar/stiller:
- **Balatro** — Kart sunumu, juice, parlak renkler
- **Monopoly Go** — Abartılı iş dünyası karikatürü
- **Slay the Spire** — Temiz kart layout, okunabilirlik
- **Cultist Simulator** — Masa üstü atmosfer
- **Overcooked** — Toon 3D obje stili

## 2.2 Toon Shader Yapısı (Unity)

### Temel Toon Shader Özellikleri:

```
1. CEL SHADING (2-3 kademeli gölge)
   - Aydınlık bölge: Ana renk (parlak, doygun)
   - Gölge bölge: Ana rengin koyu tonu (%30-40 karartma)
   - Geçiş: Sert kenar (smooth değil, step function)
   
2. OUTLINE (Dış çizgi)
   - Kalınlık: 1-2 pixel
   - Renk: Objenin koyu tonu (siyah DEĞİL)
   - Yöntem: Inverted hull veya post-process
   
3. SPECULAR HIGHLIGHT
   - Tek büyük parlak nokta (anime tarzı)
   - Sert kenar
   - Beyaz veya çok açık renk
   
4. RIM LIGHT (Kenar ışık)
   - Objelerin kenarında ince parlak çizgi
   - Hover/seçim durumunda güçlenir
   - Renk: Beyaz veya mavi
```

### Unity'de Toon Shader Yaklaşımları:

| Yöntem | Avantaj | Dezavantaj | Öneri |
|---|---|---|---|
| **Shader Graph (URP)** | Kolay, görsel | Performans okay | P1 — Prototipte bunu kullan |
| **Custom HLSL** | Tam kontrol | Kod gerektirir | P2 — MVP'de geçiş yap |
| **Toony Colors Pro 2** (Asset Store) | Hazır, güzel | $40 | Alternatif — bütçe varsa |

### Önerilen Shader Graph Yapısı:

```
[Main Texture] → [Cel Shade Node (2 step)] → [+ Outline Pass] → [+ Rim Light] → Output

Cel Shade: 
  dot(Normal, LightDir) → Step(0.3) → Lerp(ShadowColor, BaseColor)

Outline:
  Vertex extrusion yöntemi (ikinci pass, ters yüz)
  VEYA
  Post-process edge detection (Sobel filter)
```

## 2.3 Renk Paleti

### Ana Renkler (Kart Tipleri):

| Tip | Ana Renk | Hex | Gölge | Hex |
|---|---|---|---|---|
| İşletme (Mavi) | Parlak Mavi | `#4A90D9` | Koyu Mavi | `#2C5A8A` |
| Çalışan (Yeşil) | Canlı Yeşil | `#4CAF50` | Koyu Yeşil | `#2E7D32` |
| Action (Kırmızı) | Ateş Kırmızı | `#E53935` | Koyu Kırmızı | `#B71C1C` |
| Upgrade (Mor) | Parlak Mor | `#9C27B0` | Koyu Mor | `#6A1B9A` |
| Event (Sarı) | Altın Sarı | `#FFB300` | Koyu Sarı | `#E65100` |

### Ortam Renkleri:

| Element | Renk | Hex | Not |
|---|---|---|---|
| Masa yüzeyi | Sıcak kahverengi | `#5D4037` | Ahşap hissi |
| Masa keçe | Koyu yeşil | `#2E5035` | Poker masası hissi |
| Arka plan | Koyu gri-kahve | `#3E2723` | Karanlık ofis |
| UI Panel | Yarı saydam koyu | `#1A1A1AE0` | %88 opacity |
| Para/Altın | Altın | `#FFD700` | Tüm para elementleri |

### Nötral/UI Renkleri:

| Element | Renk | Hex |
|---|---|---|
| Beyaz text | Saf beyaz | `#FFFFFF` |
| Açık gri | Açık gri | `#B0BEC5` |
| Devre dışı | Koyu gri | `#616161` |
| Tehlike | Turuncu-kırmızı | `#FF5722` |
| Başarı | Yeşil | `#66BB6A` |
| Uyarı | Turuncu | `#FFA726` |

## 2.4 Kart Tasarım Anatomisi

```
┌─────────────────────────┐
│ ▓▓▓ RENK BANDI ▓▓▓▓▓▓▓ │  ← Tip rengi (mavi/yeşil/kırmızı/mor/sarı)
│                         │
│  [KART İKONU/İLLÜSTR.]  │  ← 60% alan — ana görsel
│                         │
│  ═══════════════════════ │  ← Ayırıcı çizgi
│  Kart İsmi         ★★  │  ← Sol: isim, Sağ: nadirlik yıldızları
│  ─────────────────────  │
│  💰80/tur   👤5/tur    │  ← Sol: gelir, Sağ: müşteri
│  👷 2 slot             │  ← Çalışan slotu (sadece işletme)
│  ─────────────────────  │
│  "Trend aktifken        │  ← Özel yetenek açıklaması
│   gelir x1.5"          │     (kısa, net, 2 satır max)
│  ─────────────────────  │
│  Maliyet: 💰150         │  ← En altta maliyet
└─────────────────────────┘
```

### Kart Boyutları:
- Fiziksel oran: **2.5:3.5** (standart oyun kartı oranı)
- Unity'de: 250px x 350px (veya 500x700 HD)
- Kenar yuvarlaklığı: 12px radius

## 2.5 Toon İllüstrasyon Stili (Kart İkonları)

### Genel Kurallar:
1. **Kalın dış çizgi** (2-3px siyah outline)
2. **Flat renk** — gradient minimum, cel-shade tarzı 2 ton
3. **Abartılı oranlar** — Büyük kafa, küçük gövde (chibi değil ama karikatür)
4. **Doygun renkler** — Pastel DEĞİL, canlı ve parlak
5. **Basit arka plan** — İkon arkasında tek renk veya basit şekil
6. **Gülümseme/Enerji** — Karakterler enerjik, iş dünyası karikatürü

### Karakter İllüstrasyon Örnekleri:

| Kart | İkon Açıklaması |
|---|---|
| **Büfe** | Küçük tezgah, üstünde sandviç, duman çıkıyor. Sıcak renkler. |
| **Kahveci** | Kahve fincanı, kalp şeklinde latte art, buhar. Kahverengi-krem. |
| **Burger Zinciri** | Devasa burger, salata yaprakları uçuyor, neon tabela. Kırmızı-sarı. |
| **Tech Startup** | Laptop + roket fırlatma ikonu, mavi-mor neon. |
| **Gece Kulübü** | Disko topu + neon ışıklar + müzik notaları. Mor-pembe. |
| **Stajyer** | Genç karakter, büyük gözlük, elinde kahve, koşuyor. |
| **Barista** | Hipster, sakallı, latte art yapıyor, kendinden emin gülümseme. |
| **Hacker** | Hoodie, laptop, yeşil matrix kodu arka planda, şeytani gülümseme. |
| **Dolandırıcı** | Takım elbise, güneş gözlüğü, para saçıyor, çarpık gülümseme. |
| **FBI Rozeti** | Parlak rozet, kırmızı-mavi, ciddi. |

## 2.6 Animasyon Prensipleri (Toon)

| Hareket | Tarz | Süre | Easing |
|---|---|---|---|
| Kart koyma | Yukarıdan düşme + bounce | 0.3s | EaseOutBounce |
| Kart çekme | Desteden fırlatma + fan açılma | 0.4s | EaseOutBack |
| Token hareketi | Kayma + hafif sıçrama | 0.5s | EaseInOutQuad |
| Combo patlaması | Scale 0→1.5→1 + shake | 0.6s | EaseOutElastic |
| Para sayacı | Sayı hızla artma | 0.8s | EaseOutExpo |
| Bölge renk değişimi | Renk fade + pulse | 0.5s | EaseInOutSine |
| Hover (kart) | Scale 1→1.15 + glow | 0.15s | EaseOutQuad |
| FBI baskını | Kırmızı flash + screen shake | 1.0s | - |

**Genel kural:** Her şey biraz **abartılı**. Juice > gerçekçilik. Kart koymak "thud" hissi vermeli, para kazanmak "ka-ching" festivali olmalı.

---

# BÖLÜM 3: UI/UX FONT SEÇİMİ

## 3.1 Font Stratejisi

4 farklı font kullanımı öneriliyor. Hepsi **ücretsiz, ticari lisanslı** (Google Fonts / OFL).

## 3.2 Ana UI Font — Genel Metin

### Birincil Öneri: **Nunito**
- **Neden:** Yuvarlatılmış uçlar, sıcak ve samimi. Toon stille mükemmel uyum. Okunabilir.
- Kullanım: Menüler, buton yazıları, açıklamalar, diyalog, tooltip
- Weight: Regular (400) + Bold (700) + ExtraBold (800)
- Kaynak: Google Fonts (OFL lisans)

### Alternatifler:
| Font | Karakter | Neden tercih edilebilir |
|---|---|---|
| **Poppins** | Geometrik, modern | Daha "kurumsal" his istersen |
| **Inter** | Temiz, nötr | Çok text-heavy UI varsa okunabilirlik max |
| **Quicksand** | Yumuşak, yuvarlak | Nunito'dan daha casual |
| **Comfortaa** | Çok yuvarlak | Çok toon/çocuksu his istersen |

## 3.3 Kart Başlık Font — Kart İsimleri

### Birincil Öneri: **Oswald**
- **Neden:** Condensed, güçlü, otoriter. "Business empire" hissi. Kart başlığında az yerde çok bilgi.
- Kullanım: Kart isimleri ("KAHVECI", "BURGER ZİNCİRİ"), kategori başlıkları
- Weight: Medium (500) + Bold (700)
- Stil: UPPERCASE kullanılacak
- Kaynak: Google Fonts

### Alternatifler:
| Font | Karakter | Neden tercih edilebilir |
|---|---|---|
| **Bebas Neue** | Ultra condensed | Daha dar alanda sığdırmak için |
| **Barlow Condensed** | Modern condensed | Biraz daha yumuşak versiyon |
| **Archivo Black** | Kalın, güçlü | Daha fazla ağırlık istersen |
| **Russo One** | Futuristik | Tech temalı kartlar için özel font |

## 3.4 Combo/Efekt Font — Büyük Animasyonlu Text

### Birincil Öneri: **Bangers**
- **Neden:** Comic book tarzı, enerjik, BAĞIRIYOR. "COMBO!", "LATTE SANATI!", "LEVEL UP!" gibi anlık patlamalar için.
- Kullanım: Combo isimleri, büyük event duyuruları, kazanma/kaybetme ekranı, skor
- Weight: Regular (tek weight, zaten kalın)
- Stil: UPPERCASE, büyük punto (48-72pt), gölge + outline
- Kaynak: Google Fonts

### Alternatifler:
| Font | Karakter | Neden tercih edilebilir |
|---|---|---|
| **Bungee** | Kalın, modern | Daha geometrik bir patlama istersen |
| **Permanent Marker** | El yazısı, çizik | Daha organik/çizim hissi |
| **Rubik Mono One** | Kalın mono | Sayısal combo bonusları için |
| **Luckiest Guy** | Cartoon | Daha çocuksu/çizgi film hissi |
| **Black Ops One** | Askeri/ciddi | FBI baskını gibi ciddi anlar için özel |

## 3.5 Sayı/İstatistik Font — Para, Müşteri Sayıları

### Birincil Öneri: **Space Mono**
- **Neden:** Monospace = sayılar hizalı kalır. Retro-modern hissi. Para sayacı animasyonunda rakamlar kayarken düzgün görünür.
- Kullanım: 💰 sayacı, müşteri sayıları, tur sayacı, istatistik ekranları, skor breakdown
- Weight: Regular (400) + Bold (700)
- Kaynak: Google Fonts

### Alternatifler:
| Font | Karakter | Neden tercih edilebilir |
|---|---|---|
| **JetBrains Mono** | Developer tarzı | Daha modern/tech hissi |
| **Fira Code** | Ligature destekli | Özel semboller istersen |
| **IBM Plex Mono** | Kurumsal mono | "Büyük şirket" hissi |
| **Azeret Mono** | Geometrik | Daha temiz görünüm |

## 3.6 Font Kullanım Tablosu (Özet)

| Bağlam | Font | Weight | Boyut | Renk | Örnek |
|---|---|---|---|---|---|
| Menü butonları | Nunito Bold | 700 | 24-28pt | Beyaz | "Yeni Oyun", "Ayarlar" |
| Kart açıklaması | Nunito Regular | 400 | 14-16pt | Açık gri | "Trend aktifken gelir x1.5" |
| Kart başlığı | Oswald Bold | 700 | 18-22pt | Beyaz/Krem | "KAHVECI" |
| Para sayacı | Space Mono Bold | 700 | 28-32pt | Altın `#FFD700` | "💰 1,250" |
| Tur sayacı | Space Mono Regular | 400 | 20pt | Beyaz | "TUR 8/15" |
| Combo patlaması | Bangers | 400 | 48-72pt | Altın + outline | "LATTE SANATI!" |
| Event duyurusu | Bangers | 400 | 36-48pt | Sarı `#FFB300` | "KAHVE ÇILGINLIĞI!" |
| FBI uyarısı | Bangers | 400 | 48pt | Kırmızı `#E53935` | "FBI BASKINI!" |
| Kazanma | Bangers | 400 | 72pt | Altın + glow | "KAZANDIN!" |
| Tooltip | Nunito Regular | 400 | 12-14pt | Açık gri | "Bu işletme her tur..." |
| Rakip konuşması | Nunito Italic | 400i | 16pt | Gri | "İlginç bir hamle..." |

## 3.7 TextMeshPro Ayarları

```
Font Asset oluştururken:
- Atlas Resolution: 2048x2048 (4 font için ayrı ayrı)
- Character Set: Extended ASCII + Turkish (ÇçĞğİıÖöŞşÜü)
- Render Mode: SDF (Signed Distance Field) — ölçekleme kalitesi için şart
- Padding: 5-7 (outline ve glow efektleri için alan)

Material Preset'leri:
1. "Default" — Normal text
2. "Outline" — 2px siyah outline (kart başlıkları)
3. "Glow" — Altın glow efekti (combo, kazanma)
4. "Shadow" — Drop shadow (menü butonları)
```

## 3.8 Türkçe Karakter Desteği

**KRİTİK:** Tüm fontların Türkçe özel karakterleri desteklediğinden emin ol:

```
Gerekli karakterler: Ç ç Ğ ğ İ ı Ö ö Ş ş Ü ü
Kontrol: Google Fonts > font seç > "Type something" kutusuna Türkçe yaz > gözle kontrol et.

Yukarıdaki 4 font (Nunito, Oswald, Bangers, Space Mono) 
hepsi Latin Extended destekler = Türkçe sorunsuz.
```

---

# BÖLÜM 4: ÖNCELİK SIRASI VE İŞ PLANI

## 4.1 P1 — Prototip (Hafta 1-4)

### 3D Modeller:
- [ ] Ana masa (basit box + ahşap material)
- [ ] Generic kart modeli (rounded rectangle)
- [ ] Kart destesi (3-4 kart stack)
- [ ] Müşteri token (disk/jeton)
- [ ] Para coin (basit coin)
- [ ] Bölge kutusu (x10 küçük box)
- [ ] İşletme slot çerçevesi (x5)
- [ ] Çalışan alt-slot (x10)

### Shader:
- [ ] Basit toon shader (2-step cel shade + outline)
- [ ] Kart material (texture swap destekli)
- [ ] Glow material (valid drop target)

### Font:
- [ ] Nunito (Regular + Bold + ExtraBold) — TMP asset
- [ ] Oswald (Medium + Bold) — TMP asset
- [ ] Bangers — TMP asset
- [ ] Space Mono (Regular + Bold) — TMP asset

### 2D Sprite:
- [ ] 5 kart çerçeve template (mavi, yeşil, kırmızı, mor, sarı)
- [ ] Kart arka yüzü
- [ ] Placeholder ikonlar (basit shape/silhouette)

## 4.2 P2 — MVP (Hafta 5-16)

### 3D Ek:
- [ ] FBI rozeti
- [ ] Yıldız token (combo)
- [ ] Upgrade slot
- [ ] Aksiyon noktaları (3D dot)

### İllüstrasyon:
- [ ] 40 unique kart ikonu (toon stil, 256x256 veya 512x512)
- [ ] 6 event kartı özel görseli
- [ ] 1 rakip portresi (MegaCorp logo/yüz)

### Shader Geliştirme:
- [ ] Rim light ekleme
- [ ] Specular highlight
- [ ] Daha iyi outline (post-process)

## 4.3 P3 — Polish (Hafta 17+)

### 3D Ek:
- [ ] Arka plan oda
- [ ] Masa lambası
- [ ] Tur çarkı
- [ ] Dükkan tabelası

### İllüstrasyon:
- [ ] Full kart illüstrasyonları (portrait boyutu)
- [ ] Ana menü arka plan görseli
- [ ] Loading screen görseli

---

# BÖLÜM 5: ÜCRETSİZ ARAÇ VE KAYNAK ÖNERİLERİ

## 3D Modelleme
| Araç | Ne İçin |
|---|---|
| **Blender** | Tüm 3D modeller (masa, token, coin) |
| **Blender Toon Shader** | Blender'da toon material geliştir, Unity'ye export et |
| **ProBuilder (Unity)** | Basit geometriler direkt Unity içinde |

## 2D / İllüstrasyon
| Araç | Ne İçin |
|---|---|
| **Krita** | Kart ikonları, dijital çizim (ücretsiz Photoshop alternatifi) |
| **Figma** | Kart layout template, UI tasarım |
| **Inkscape** | Vektörel ikonlar, logo |
| **game-icons.net** | Placeholder ikonlar (CC BY 3.0) |

## Shader
| Kaynak | Ne İçin |
|---|---|
| **Unity Shader Graph** | Toon shader oluşturma (URP) |
| **Brackeys Toon Shader Tutorial** | Öğrenme kaynağı |
| **MinionsArt** | Stylized shader referansları |

## Font
| Kaynak | Ne İçin |
|---|---|
| **Google Fonts** | Tüm 4 font (fonts.google.com) |
| **dafont.com** | Alternatif fontlar (lisansa dikkat!) |

---

> Bu doküman GDD v2.0 ile tam uyumludur.
> Tüm öneriler solo developer, düşük bütçe, toon stiline göre optimize edilmiştir.
