# PREMORTEM ANALİZİ — Empire of Cards

> **Çerçeve:** Kasım 2027. Empire of Cards 6 ay önce çıktı. Sadece 847 kopya sattı. Geriye bakıyoruz — ne oldu?
>
> *Gary Klein'ın premortem metodolojisi uygulanmıştır. 5 farklı başarısızlık perspektifinden analiz yapılmıştır.*

---

## SENTEZ: EN KRİTİK 7 BULGU

Aşağıda 5 analizin tamamından çıkan, **hemen harekete geçilmesi gereken** en kritik bulgular:

### 1. SCOPE ÖLÜMCÜL BÜYÜKLÜKTE
155 kart, 60 combo, 6 rakip, 10 ascension — bu 3-4 kişilik ekip işi. Solo dev için **40 kart, 10 combo, 1 rakip** ile başlanmalı. Geri kalanı Early Access sonrası eklenir.

### 2. "BİR DECKBUILDER DAHA" ALGISI ÖLDÜRÜR
1,385+ deckbuilder var. Marketing'de **asla** "deckbuilder" kelimesi öne çıkmamalı. Pitch: "Şirket imparatorluğu kur" — kartlar araç, fantazi ürün.

### 3. İŞLETME TEMASI SIKILMA RİSKİ TAŞIYOR
"Muhasebeci işe al, vergi öde" = ödev hissi. Tema **grotesk absürditeye** itilmeli. Kart isimleri tek başına komik olmalı: "Torpil İşe Alımı (bedava, bir şey yapmaz, kovamazsın)".

### 4. JUICE OLMADAN OYUN ÖLÜ
Combo tetiklendiğinde sayı değişmesi yetmez. Para yağmuru, ekran sarsılması, rakip kartları masadan düşmesi — bunlar oyunun KENDİSİ, süs değil. Faz 2'de yapılmalı, Faz 3'te değil.

### 5. WİSHLİST OLMADAN LANSMAN YOK
Steam sayfası **18 ay önce** açılmalı. 7,000+ wishlist olmadan çıkma. Her hafta devlog, her Next Fest'e katıl.

### 6. 25 TUR ÇOK UZUN OLABİLİR
Tur 11-20 arası "ölüm vadisi" — motor kuruldu ama oyun bitmiyor. **15 tura düşür** veya her 3 turda kriz event'i koy.

### 7. VALİDASYON YAPMADAN KOD YAZMA
Market gap = talep kanıtı değil. İlk 4 hafta: prototip yap, 100 kişiye göster, itch.io'ya koy, ölç.

---

## ANALİZ 1: OYUN TASARIMI BAŞARISIZLIĞI

*"Oyun eğlenceli değildi. İşte bu yüzden."*

### 1.1 Excel Problemi
İş teması bir paradoks yarattı: spreadsheet seven insanlar zaten Capitalism Lab oynuyor. Deckbuilder seven insanlar "çalışan işe al" görünce "bu iş gibi" dedi. Satirik ton bunu kurtaramadı çünkü satir, oyuncu zaten bağlanmadan işe yaramaz.

**Başarısızlık senaryosu:** Streamer oyunu açtı. İki tur oynadı. "Tamam yani muhasebeci işe alıyorum ve ofise koyuyorum... harika." Chat: "bu işe benziyor lol." 8 dakikada kapattı.

**Düzeltme:** Tema DAKİKA BİRDEN absürd olmalı. Çalışan işe alındığında ofise fiziksel olarak çarpmalı. İşletme battığında bina karikatürsel şekilde çökmeli. Kart isimleri iş bilgisi gerektirmeden tek başına komik olmalı.

### 1.2 Ortadaki Ölüm Vadisi
Tur 1-5: hızlı. Tur 6-10: eğlenceli. **Tur 11-20: ölüm vadisi** — motor kuruldu, kararlar artımlı optimizasyona dönüştü, ama oyun bitmiyor. Tur 21-25: sonuç zaten belli olan formalite.

**Düzeltme:** 15 tura düşür. Her 3 turda zorunlu kriz event'i. Geç oyun erken oyundan daha tehlikeli olmalı. Market share %60'a ulaşınca run erken bitebilsin.

### 1.3 Combo Hissettirilmiyor
Combo mekanik olarak çalıştı. Ama sonucu "gelir %340 arttı" olan bir sayı değişimi. Slay the Spire'da düşmanın canı eriyor — görsel, anlık, visceral. Empire of Cards'da dashboard'da sayı değişti.

**Düzeltme:** Her combo'nun fiziksel/görsel/işitsel sonucu olmalı. Market share patlaması → rakip kartları masadan itilir. Gelir spike'ı → karttan slot makinesi gibi madeni para akar. Çalışan sinerjisi → çalışan kartları birbirine beşlik çakar.

### 1.4 Karar Yorgunluğu (Trait Overload)
Çalışan trait sistemi simülasyon mekaniği. Deckbuilder el yönetimi mekaniği. İkisi birleşince her kart oynanması 5 katman değerlendirme gerektirdi. Oyuncular akıllı değil yorgun hissetti.

**Düzeltme:** Her çalışanın TEK görünür keyword'ü olsun: "Açgözlü — 2x kazanır, 2x maliyetli." Bir okuma, bir anlama, bir karar. Tooltip dalışı yok.

### 1.5 Rakip AI: Sinir Bozucu Ama İlginç Değil
Kolay mod'da AI aptal. Zor mod'da gizli avantajlarla (ekstra para, ekstra aksiyon) hile yapıyor. Hiçbir zorlukta gerçek rakip gibi hissettirmiyor.

**Düzeltme:** AI asla oyuncunun elinden/masasından kart almamalı. Ortak kaynaklar (müşteri havuzu, market pozisyonları) için rekabet etmeli. Oyuncu rakibin masasına bakıp "ne yapıyor anlıyorum, nasıl yeneceğimi biliyorum" demeli. Rekabet, soygun değil.

### 1.6 Tutorial Mekanik Öğretti, Eğlence Öğretmedi
12 tooltip kutusu okuyan oyuncu kuralları öğrendi ama neden umursaması gerektiğini öğrenmedi.

**Düzeltme:** İlk run, komboyu, rakip anını ve kazanma hissini gösteren kurgulanmış senaryo olsun. Inscryption yaklaşımı: açıklama, göster.

### 1.7 Tekrar Oynanabilirlik İllüzyonu
150+ kart. 5 run sonra %80'ı görüldü. Her run aynı yay: gelir kur → çalışan al → market share kazan. Kart bileşimi değişti ama **run şekli** değişmedi.

**Düzeltme:** Her run'da yapısal farklılık: farklı kazanma koşulları (düşman devralma vs IPO vs monopol vs viral olma), farklı masa layout'ları, farklı ekonomi kuralları.

---

## ANALİZ 2: PAZAR & KİTLE BAŞARISIZLIĞI

*"Kimse oyunu bulamadı veya istemedi."*

### 2.1 Kimlik Krizi
"Deckbuilder" etiketi savaş bekleyen oyuncuları çekti → "işletme kur" görüp kaçtılar. İşletme seven oyuncular "deckbuilder" etiketine hiç bakmadı. İki kitle arasında kimseye ulaşılamadı.

**Düzeltme:** Birincil etiketler: Strategy, Management, Economy. "Deckbuilder" ikincil. Store page: "Şirket imparatorluğunu kur. Rakiplerini ez. Kartlar sadece aracın."

### 2.2 Wishlist Faciası
Lansmandan 4 ay önce açılan Steam sayfası ile 1,100 wishlist. İlk hafta 180 satış. Algoritma "düşük ilgi" okudu, görünürlük bastırıldı, 5 günde kayboldu.

**Düzeltme:** Oyun hazır olmasa bile 18 ay önce store page aç. Her Next Fest'e katıl (2 hakkın var). 12 ay sonra 3,000 wishlist'e ulaşamıyorsan ya pozisyonlama yanlış ya da ürün viable değil.

### 2.3 Capsule Art Felaketi
Dolar işaretli generic bir kart görseli. 230x107 piksel thumbnail'de görünmez. Mobil F2P oyundan ayırt edilemiyor.

**Düzeltme:** Capsule'de KARAKter göster — abartılı çizgi tarzında küstah bir işadamı, kartlardan yapılmış masada, sinirli rakipler arkada. Kişilik, mizah, anlatı.

### 2.4 Streamer Reddi
200 key gönderildi, 3'ü oynadı. Neden: "Çeyrek gelir tablomu geçti mi?" Twitch clip'i üretmiyor. Boss fight yok, sadece spreadsheet artıyor.

**Düzeltme:** Rakip battığında ofis binası karikatürsel patlama ile çökmeli. Borsa çöktüğünde alarmlar çalmalı, çalışanlar kaçışmalı. Combo'da para absürd miktarda yağmalı. Run başına minimum 3 "clip-worthy" an tasarla.

### 2.5 %100 Dev, %0 Marketing
26 ay boyunca 11 tweet, 1 Reddit postu (14 upvote), Discord sunucusu lansmandan 3 hafta önce.

**Düzeltme:** Çalışma süresinin %20'si marketing'e. Her hafta devlog GIF'i. Aylık YouTube videosu. r/gamedev Screenshot Saturday her hafta. Bu opsiyonel değil.

### 2.6 Hook Etmeyen Demo
Demo 3 dakika tutorial ile başladı. 90. saniyede %68 oyuncu çıktı.

**Düzeltme:** Demo oyunun ORTASINDA başlasın. Oyuncunun önünde çalışan küçük bir imparatorluk. İlk hamle: kart koy → para yağmuru → rakip tepki versin. 60 saniyede dopamine.

### 2.7 Fiyat Sinyali Sorunu
$12.99: impulse buy için çok pahalı, derinlik sinyali için çok ucuz. Bilinmeyen indie için dead zone.

**Düzeltme:** İki seçenek: (A) $9.99 — küçük ama cilalanmış, hacim hedefi. (B) $17.99 — premium, sesli anlatım, derin içerik. Ortası kimseye yaramıyor. Solo dev için A daha güvenli.

---

## ANALİZ 3: SOLO DEV UYGULAMA BAŞARISIZLIĞI

*"Proje %60'da terk edildi."*

### 3.1 Scope Sanrısı
GDD'nin kendisi sorunu gösteriyor. Tek kişinin 52 haftada yapması gereken:
- 155 kart (minimum 2 saat/kart = 310 saat sadece kart)
- 60 combo (3 saat/combo = 180 saat)
- 6 AI tipi (40 saat/tip = 240 saat)
- **Sadece içerik = 730 saat** (toplam çalışma süresinin %35'i)

Geriye engine, UI, ses, marketing, balance, test için 1,350 saat kalıyor. Yetmez.

### 3.2 Burnout Zaman Çizelgesi
- **Hafta 1-6:** Yüksek enerji. Kart sürükle-bırak çalışıyor. Heyecan.
- **Hafta 7-14:** Yavaşlama. 5 faz tur sistemi edge case cehennemi.
- **Hafta 15-20:** 80 kart üretimi. Art trap: öğren (6-8 hafta), satın al ($3-8K), veya placeholder (kötü görünür).
- **Hafta 22-28:** DUVAR. Yarım oyun, placeholder art, ses yok, balance bozuk. Demo deadline kaçırılıyor.
- **Hafta 30+:** Timeline erteleniyor, Steam sayfası hiç açılmıyor, proje sessizce ölüyor.

### 3.3 Balance Kabusu
60 combo'yu tek başına dengelemek kombinasyonel patlama. 25 combo x 3 rakip x 5 ascension = 375 test permütasyonu. 40 dk/run = 250 saat playtest. Yalnız başına.

### 3.4 Yalnızlık Spirali
- Hafta 8: Reddit'te devlog. 3 upvote.
- Hafta 20: Discord'da 12 üye, 9'u bot.
- Hafta 26: Kendi oyunundan bıkmış 200+ test run.
- Hafta 30: Daha iyi görünen başka bir deckbuilder çıkıyor. Karşılaştırma umutsuzluğu.

### REVİZE EDİLMİŞ MVP

| Kategori | Orijinal MVP | Revize MVP |
|---|---|---|
| Toplam kart | 80 | **40** |
| Combo | 25 | **10** |
| Rakip tipi | 3 | **1** (3 zorluk ayarı ile) |
| Ascension | 5 | **3** |
| Çalışan trait | Tam sistem | **Yok. Çalışanlar sadece stat.** |
| Çalışan draması | 8 event | **0. Tamamen kes.** |
| Çalışan evrimi | 3 kademe | **0. Tamamen kes.** |
| Müşteri tipi | 5 | **2** (Budget + Standard) |
| Etik/FBI | Tam | **Basit: legal/illegal, sabit ceza** |
| Müzik | 5 parça | **2** (sakin + yoğun, crossfade) |
| Kart art | Her karta unique | **5 template + renk/ikon varyasyonları** |

### REVİZE EDİLMİŞ ZAMAN ÇİZELGESİ (32 Hafta)

| Hafta | İş | Milestone |
|---|---|---|
| 1-4 | Prototip. Sürükle-bırak, 5 kart, para artar. | "Eğlenceli mi?" testi |
| 5-10 | Core loop. 20 kart, 5 combo, 1 AI, temel ekonomi | 5 harici playtester bul |
| 11-16 | İçerik + juice. 40 kart, 10 combo, ses, animasyon | **Steam sayfası hafta 14'te açılır** |
| 17-22 | Polish + demo. Balance, Next Fest demo, trailer | Next Fest katılımı |
| 23-28 | **Early Access lansmanı** (40 kart) | Topluluk geri bildirimi |
| 29-32 | İlk güncelleme (+20 kart, +5 combo, 2. rakip) | |

### Hesap Verebilirlik Yapıları
1. Her Pazar halka açık devlog (kaçırılmaz)
2. 2 haftada 1 playtest oturumu (3-5 kişi)
3. Gamedev accountability grubuna katıl
4. Aylık milestone değerlendirmesi yazılı olarak

---

## ANALİZ 4: TEKNİK & KALİTE BAŞARISIZLIĞI

*"Steam yorumları 'Mixed' (%58 pozitif). Oyuncular sürekli şikayet ediyor."*

### 4.1 Combo Sistemi Performans Bombası
5 işletme slotu, 12 çalışan, 3 upgrade, 2 event = 22 aktif entity. C(22,2) + C(22,3) = 1,771 subset kontrolü x 60+ combo tanımı = **106,260 tag-array karşılaştırması** her tur. GDScript'te 200-400ms frame stutter. Steam Deck'te 800ms+.

**Düzeltme:**
- Tag-indexed combo lookup dictionary. Kart oynanınca sadece o kartın tag'lerine uyan combo'ları kontrol et (60+ yerine 3-5).
- Aktif tag set'lerini cache'le. `Dictionary<String, Array[CardRef]>` tut.
- Performans bütçesi: resolution phase < 50ms. Otomatik benchmark her build'de.

### 4.2 Sonsuz Para Spirali
Coffee Shop 80/tur + combo x3 = tur 15'te 800+/tur gelir, 200/tur gider. Para anlamsızlaşıyor. Gerilim buharlaşıyor. İkinci yarı: "bitmesini bekleyerek tur atlıyorum."

**Düzeltme:**
- Azalan getiri: 2. aktif combo %80 bonus, 3. %50.
- Dinamik gider: gelir arttıkça vergi dilimi yükselir, maaş talepleri artar.
- %60 market share'de run erken bitebilsin.
- Economy simulator: 10,000 rastgele oyun çalıştır, hiçbir strateji tur 15'te medyanın 4x'ini geçmemeli.

### 4.3 Save Corruption
80 kart EA'dan 155 kart v1.0'a geçişte kart ID'leri değişti. Save migration kodu aceleye geldi. Oyuncuların %30'u meta progression kaybetti. "200 saatim gitti."

**Düzeltme:**
- Gün 1'de save formatına `save_version` integer'ı koy.
- Migration fonksiyonları zincirle: v1→v2→v3.
- Kart/combo'ları **sabit string ID** ile referansla (`"card_coffee_shop"`). Asla ID tekrar kullanma.
- Migration öncesi otomatik backup.
- JSON kullan, Godot Resource serialization KULLANMA.

### 4.4 AI Kalitesi
Kural tabanlı AI 6-8 koşulla 25 tur boyunca zeka illüzyonu yaratamadı. MegaCorp "agresif büyüme" olması gerekirken bazen Coffee Shop açtı.

**Düzeltme:**
- Her AI kişiliğine ağırlıklı karar tablosu: MegaCorp %70 genişleme / %5 sabotaj. Shadow Inc. %60 illegal / %10 genişleme.
- AI'a asla gizli kaynak verme. Zor AI = daha iyi karar vermeli.
- AI karar log sistemi. Kararlar kişiliğe uyuyorsa → iyi. Uymuyorsa → ağırlıkları düzelt.

### 4.5 Sürükle-Bırak UX Kabusu
İşletme slotu 200x150px. Çalışan alt-slotu 60x40px. Trackpad ile imkansız. Redraw (aşağı sürükle) vs satma (sağa sürükle) karışıyor.

**Düzeltme:**
- Birincil kontrol: **tıkla-seç, tıkla-yerleştir**. Sürükle-bırak ikincil.
- Çalışan ataması: işletmeye tıkla → panel açılır → çalışanı seç.
- Redraw: karttaki küçük buton, sürükleme jesti değil.
- Satma: Uncommon+ kartlarda onay popup'ı.
- Gün 1'den 1280x800 (Steam Deck) çözünürlüğünde test et.

### 4.6 Güncelleme İstikrarsızlığı
Yeni kartlar mevcut kartlarla test edilmemiş combo tag etkileşimleri oluşturdu. "Franchise License" + "Automation" = sonsuz para döngüsü. Hotfix mevcut save'leri kırdı.

**Düzeltme:**
- Run ortasında kart stat'larını değiştirme. Run başladığındaki kart tanımlarını run save'ine kaydet.
- Combo etkileşim test matrisi: her yeni kart x her mevcut kart. Otomatik. Patch'ten önce.
- Semantic versioning.

### KALİTE KAPILARI (Vazgeçilmez)

1. **Performans bütçesi:** Combo çözümleme < 50ms
2. **Ekonomi simülatörü:** 10,000 oyun, hiçbir strateji 4x medyan geçemez
3. **Save round-trip testi:** Kaydet → yükle → kaydet → karşılaştır
4. **Combo etkileşim matrisi:** Her çift/üçlü otomatik test
5. **5 dakika retention testi:** Herhangi bir turda %15+ kayıp = blocker
6. **1280x800 çözünürlük:** Her UI ekranı Steam Deck'te çalışmalı

---

## ANALİZ 5: GİZLİ VARSAYIMLAR & KÖR NOKTALAR

*"Temel varsayımlar yanlıştı."*

### Varsayım 1: "Market gap = talep"
**Yanılma:** Boşluk = kimse istemiyor da olabilir. İş sim oyuncuları derinlik istiyor, deckbuilder oyuncuları savaş istiyor. İkisini %60 yapan hibrit ikisini de tatmin etmiyor.

**Test:** r/deckbuilding, r/tycoon'a konsepti paylaş. "Bunu satın alır mıydım?" yanıtlarını say. Landing page yap, 500 ziyaretçi ile email dönüşüm <%3 ise → boşluk boşluk.

### Varsayım 2: "Satirik tema oyuncu çeker"
**Yanılma:** Satir subjektif, kültüre bağlı, çabuk eskir. Kart isimlerinde yaşayan satir 20 dakika sonra okunmayı bırakır.

**Test:** 20 kart açıklaması yaz, arkadaş çevren dışında 50 kişiye göster. Medyan komiklik <6/10 ise → satir çalışmıyor.

### Varsayım 3: "Non-combat deckbuilder trendi"
**Yanılma:** Balatro ve Inscryption non-combat oldukları İÇİN değil, başka devasa farklılaştırıcıları olduğu RAĞMEN başarılı. Her başarılı non-combat deckbuilder için Steam'de 40+ başarısız non-combat deckbuilder var. Survivorship bias.

**Test:** SteamDB'de 2025-2026'da çıkan non-combat deckbuilder'ları say. Kaçı 500+ review'a ulaşmış? 5'ten azsa → "trend" outlier'ların yarattığı illüzyon.

### Varsayım 4: "TCG Card Shop Sim başarısı bizi doğrular"
**Yanılma:** TCG Card Shop Sim deckbuilder DEĞİL. Kart açma + dükkan yönetimi sim. Kartlar envanter öğesi, gameplay mekaniği değil. Bu bağlantı yüzeysel — "Cooking Mama food-themed roguelite doğrular" demek gibi.

**Test:** TCG Card Shop Sim Steam topluluğunda "deckbuilder mekaniği istiyorum" diyen oyuncu sayısını say. <%5 ise → crossover kitle yok.

### Varsayım 5: "$12.99-14.99 doğru fiyat"
**Yanılma:** Balatro $14.99 = yüzlerce saat. StS indirimde $8. Bilinmeyen solo dev ilk oyunu için bu fiyatta Balatro ile doğrudan rekabet. Dead zone.

**Test:** 200 deckbuilder oyuncusuna trailer'ı göster, "ne kadar ödersiniz?" sor. Medyan <$10 ise → ya değer algısını artır ya da fiyatı düşür.

### Varsayım 6: "Streamer'lar karar anlarını sever"
**Yanılma:** Streamer'lar KARAR anlarını değil, TEPKİ anlarını sever — görünür, abartılı, paylaşılabilir. "A mı B mi?" 5 saniyelik sessiz düşünme üretir. Clip değil.

**Test:** Prototipi 30 dk oyna, kaydet. İzleyici olarak izle. 30 dk'da 3+ clip-worthy an bulamıyorsan → oyun streamable değil.

### Varsayım 7: "Tabletop estetiği farklılaştırıcı"
**Yanılma:** Tabletop estetiği farklılaştırıcı değil — düşük bütçe sinyali. Inscryption masayı narrative cihaz olarak kullandı. Onsuz, tabletop görünüm = "daha iyi art afford edemedik".

**Test:** Mock capsule art yap, 10 deckbuilder capsule ile yan yana koy, 30 kişiye 5 sn göster, "hangisine tıklarsınız?" sor. <%10 seçilirse → estetik görünmez.

### GİZLİ VARSAYIM A: "Kendi marketing'imi yapabilirim"
Kimse bunu risk olarak listelemedi. Solo dev "Reddit + Twitter'a atarım" diye düşünüyor. Bu plan değil, dilek. **Test:** Steam sayfası aç, 90 gün marketing yap. 90 günde 5,000 wishlist'e ulaşamıyorsan → oyunun ticari viability'si tehlikede.

### GİZLİ VARSAYIM B: "Oyuncular store page'den oyunu anlayacak"
Satirik non-combat iş temalı tabletop deckbuilder → karmaşık pitch. 3-7 saniye. Hangi kitleye optimize edeceksin? **Test:** Draft store page'i 30 kişiye 10 sn göster. "Bu oyun ne?" sor. <%60 doğru cevaplayamıyorsa → iletişim başarısız.

### GİZLİ VARSAYIM C: "8. ayda hâlâ bunu yapmak isteyeceğim"
52 haftalık solo proje maraton. 8. ayda heyecan bitmiş olacak, geriye disiplin kalacak. Hesap verebilirlik yapısı yoksa proje sessizce ölür. **Test:** 4 haftalık prototip sprint'i yap. Sonunda enerji/heyecan 1-10 skalasında <7 ise → 52 haftaya dayanamayacaksın.

---

## REVİZE EDİLMİŞ PLAN: PREMORTEM SONRASI

### İLK 4 HAFTA: DOĞRULAMA (KOD YAZMADAN ÖNCE)
1. Kağıt prototip yap (30 kart). 10 kişiyle oyna.
2. "Bunu satın alır mıydınız?" anketi (100+ kişi)
3. itch.io'ya dijital prototip koy, retention ölç
4. Mock Steam sayfası oluştur, capsule art testi yap
5. 5 küçük streamer'a (500-2000 takipçi) gönder, VOD izle

→ **%15'ten az "satın alırım" → konsepti pivot et**
→ **%15+ → devam et ama revize scope ile**

### SCOPE: KESTİKLERİN TAKVİMİ

| Ne zaman eklenir | İçerik |
|---|---|
| **EA Lansmanı** | 40 kart, 10 combo, 1 rakip, 3 ascension, basit çalışanlar |
| **Güncelleme 1** | +20 kart, +5 combo, 2. rakip tipi |
| **Güncelleme 2** | Çalışan trait sistemi, drama event'leri |
| **Güncelleme 3** | 3. rakip, ascension 4-7 |
| **Güncelleme 4** | Etik/FBI tam sistem, müşteri tipleri genişleme |
| **v1.0** | 6. rakip, ascension 8-10, tam içerik |

### ÖLÜM KALIM KURALLARI

1. **Juice, Faz 2'de yapılır.** Resolution phase animasyonu ilk yapılan şeylerden biri olmalı.
2. **Her hafta Pazar devlog.** Kaçırılmaz. "3 bug düzelttim, nefret ettim" bile olsa.
3. **Steam sayfası hafta 14'te açılır.** Oyun hazır olmasa bile.
4. **Next Fest'e katılır.** Demo ilk 8 turu kapsar, oyunun ortasında başlar.
5. **10,000 wishlist olmadan çıkılmaz.**
6. **Auto-play bot çalıştırılır.** 1,000 simüle oyun, kırık combo'lar tespit edilir.
7. **40 kart ile çıkılır.** 155 kart hiçbir zaman birinci gün hedefi olmaz.

---

> *"40 kart ve 10 combo ile çıkan ve sıkı dengelenmiş oyun, %60'da terk edilen 90 kartlı oyundan her zaman daha fazla satar."*

---

*Premortem analizi tamamlandı: 2026-05-18*
*Metodoloji: Gary Klein premortem — 5 paralel başarısızlık perspektifi*
