# Empire of Cards Event System

> Amaç: popup tabanlı rastgele olaylar değil, dünyada görünen mikro-sinematik işletme dramaları üretmek.

## 1. Philosophy

Event sistemi işletmenin canlı olduğunu kanıtlayan ana sistemdir. Oyuncu sadece sayı değişimi görmemeli; kararlarının insanlara, müşterilere, çalışanlara, rakibe ve district'e nasıl yansıdığını izlemelidir.

Her event şu yapıyı takip eder:

```text
PROBLEM
-> REACTION
-> VISIBLE CONSEQUENCE
-> NEW SYSTEMIC PRESSURE
```

Event bir cutscene değildir. Event, simülasyonun kısa süreli dramatik zoom'udur. Oyun akışı durabilir veya yavaşlayabilir, ama oyuncu her zaman bunun kendi kararlarından doğduğunu anlamalıdır.

## 2. Event Goals

- İşletme dramalarını duygusal hale getirmek.
- Simülasyon sonuçlarını sahada okunur kılmak.
- Krizleri cezadan çok yeni stratejik problem yapmak.
- Oyuncuya çalışanları ve müşterileri hatırlanabilir hissettirmek.
- Uzun anlatım veya menü yığını kullanmadan karar anı yaratmak.
- Aynı event'in her tekrarında farklı context ve sonuç üretebilmesini sağlamak.

## 3. Event Categories

| Category | Trigger Kaynağı | Örnek |
|---|---|---|
| Staff Crisis | Stress, düşük maaş, fazla mesai | Garson işi bırakmak ister |
| Customer Crisis | Delay, düşük kalite, yanlış vaat | Toplu kötü yorum patlaması |
| Capacity Collapse | Demand > capacity | Kuyruk taşar, siparişler gecikir |
| Reputation Damage | Rating düşüşü, scandal | Google puanı görünür şekilde çöker |
| Supplier Crisis | Ucuz/kalitesiz tedarik | Hijyen sorunu, iade dalgası |
| Legal Risk | Illegal/risk tag threshold | Denetim, ceza, platform ban riski |
| Rival Move | Rakip strateji seçimi | Fiyat kırma, staff poach, kampanya |
| Opportunity | Yüksek rating, trend, festival | Viral olma, yatırımcı ilgisi |
| Growth Pain | Hızlı ölçeklenme | Müdür ihtiyacı, kültür krizi |
| Meta / Holding | Scale stage değişimi | Exit teklifi, yatırımcı baskısı |

## 4. Trigger Model

Event'ler rastgele hissettirmemelidir. Her event bir sistem baskısından doğar.

### 4.1 Hard Triggers

Kesin eşiklerle tetiklenir:

- Staff stability 30 altına düşer.
- Demand capacity'nin yüzde 140 üstüne çıkar.
- Rating bir turda 0.4 veya daha fazla düşer.
- Legal risk 70 üstüne çıkar.
- Cash iki tur üst üste negatif kalır.

### 4.2 Soft Triggers

Ağırlıklı olasılıkla tetiklenir:

- Ucuz supplier + yüksek demand + sıcak hava trendi.
- Black hat marketing + hızlı rating artışı.
- Düşük maaş + fazla mesai + staff stress.
- Rakip agresif marketing + oyuncu rating düşüşü.

### 4.3 Narrative Tags

Kartlar ve kararlar gelecekte event doğuracak tag bırakabilir:

- `underpaid_staff`
- `fake_reviews`
- `cheap_supplier`
- `over_capacity`
- `angry_regulars`
- `rival_poaching`
- `founder_burnout`
- `brand_trust`

Tag'ler event'e bağlam verir. Aynı "çalışan krizi" event'i düşük maaş, kötü müdür veya fazla müşteri yüzünden farklı diyalog ve sonuçla gelebilir.

## 5. Micro-Cinematic Structure

Her mikro-sinematik event 3-8 saniyelik okunur beat'lerden oluşur.

### Beat 1: System Signal

Oyuncu önce dünyada problemi sezer:

- Kuyruk uzar.
- Çalışan üzerinde stress ikonları artar.
- Review balonları kırmızılaşır.
- Müşteriler kapıdan dönmeye başlar.

### Beat 2: Camera Focus

Kamera ilgili noktaya kısa zoom/pan yapar. Amaç dramatik göstermek, kontrolü uzun süre almamak.

Kamera kuralları:

- 0.3-0.6 sn ease-in.
- 2-4 sn olay odağı.
- UI kararı sırasında hafif yavaş zaman.
- 0.3-0.6 sn gameplay kamerasına dönüş.
- Aynı turda maksimum 2 güçlü kamera kesmesi.

### Beat 3: Character Action

Bir NPC veya fiziksel olay problemi görünür yapar:

- Garson tepsiyi masaya geç bırakır, sonra patrona yürür.
- Müşteri beklemekten vazgeçip kırmızı yöne döner.
- Kurye paketleri üst üste yığar.
- Rakip işletme önünde kırmızı müşteri kalabalığı oluşur.

### Beat 4: Choice Overlay

Kısa karar paneli çıkar. Panel dünyayı kapatmamalı; olayın yanında veya alt bandında görünmelidir.

Kural:

- 1-3 seçenek.
- Her seçenek kısa isim + net tradeoff.
- Uzun açıklama yok.
- Stat etkisinin tamamı değil, kararın niyeti gösterilir.

Örnek:

```text
Çalışanlar iş bırakmak üzere.

[Maaşları yükselt] Cash gideri artar, stability toparlanır.
[Kriz çıkaranları kov] Kısa vadede düzen, capacity kaybı.
[Söz ver, geçiştir] Cash korunur, risk tag'i büyür.
```

### Beat 5: Visible Consequence

Seçimden hemen sonra dünya tepki verir:

- Maaş artışı: çalışanların stress ikonları azalır, cash pulse düşer.
- Kovma: iki çalışan sahadan çıkar, kuyruk uzar.
- Geçiştirme: çalışanlar işe döner ama üstlerinde riskli kırmızı stress halkası kalır.

### Beat 6: New Pressure

Event yeni bir systemic pressure bırakır:

- `wage_cost_up`
- `capacity_shortage`
- `staff_resentment`
- `public_apology_active`
- `audit_pending`
- `rival_advantage_window`

Bu pressure sonraki kart havuzunu, event riskini ve simülasyon çözümünü etkiler.

## 6. Camera Behavior

Kamera oyuncunun dikkatini yönetir, ama oyunu kesmez.

Ana kamera dili sabit izometrik Toy Diorama'dır. Serbest orbit yoktur. Event kameraları `CAMERA_LIGHTING_TURN_FLOW.md` içindeki virtual camera kurallarına uyar.

### 6.1 Camera Modes

- `Board Overview`: normal strateji kamerası.
- `Card Placement Focus`: kartın sahaya indiği kısa kamera.
- `Incident Focus`: event'in kaynağına zoom.
- `Market Flow View`: müşteri renk akışını göstermek için district üst görünümü.
- `Rival Reaction View`: rakibin hamlesini kısa göstermek.
- `Scale View`: işletme büyüdüğünde daha geniş açı.

Geçiş süreleri:

- Inspect: 0.25-0.45 sn.
- Slot placement: 0.6-1.0 sn.
- Event focus: 0.4-0.8 sn.
- Market flow: 0.8-1.2 sn.

Event sırasında simulation pause veya 0.1 hız kullanılır. Dünya tamamen ölmez; ambient animasyonlar çok yavaş kalabilir.

### 6.2 Camera Priority

1. Player critical crisis
2. Player card placement
3. Major market share shift
4. Rival major action
5. Ambient district events

Kamera her küçük stat değişimine zıplamamalıdır. Küçük olaylar world UI ve animasyonla anlatılır.

Işık kuralı: Eventler global sahneyi karartmaz. Staff stress, legal risk, rating damage veya recovery lokal Toy Studio vurgu ışığı/VFX ile gösterilir.

## 7. UI Overlay Rules

UI overlay destekleyici olmalı, olayın kendisi olmamalıdır.

### 7.1 Event Panel

- Olayın yanında veya ekran alt bandında çıkar.
- Başlık problem cümlesidir.
- 1 kısa bağlam cümlesi.
- 1-3 karar butonu.
- Her butonun tradeoff etiketi vardır.

### 7.2 World Labels

- Kuyruk üzerinde `+Delay`.
- Çalışan üzerinde `Stress`.
- Review balonu üzerinde `-0.2 Rating`.
- District üstünde `Rival Pull`.

World labels 1-2 saniye kalır, sonra kaybolur. UI spam olmamalıdır.

### 7.3 Stat Feedback

Stat değişimleri panelde uzun liste olarak değil, dünyada ve küçük pulse ile gösterilir:

- Rating yıldızı çatlar veya parlar.
- Cash sayaç kısa düşüş/yükseliş pulse'ı yapar.
- Market share barı ile birlikte müşteri renk oranı değişir.

## 8. NPC Physical Reactions

### 8.1 Customers

Müşteriler konuşmadan anlaşılmalıdır:

- Bekleyen müşteri: ayak vurma, saate bakma.
- Memnun müşteri: hızlı çıkış, pozitif review, gold sparkle.
- Kızgın müşteri: baş sallama, kırmızı review, rakip yönüne dönme.
- Sadık müşteri: daha yavaş renk kaybeder, krizlerde hemen kaçmaz.
- Viral müşteri: sosyal paylaşım animasyonu, çevresindeki gri müşterileri etkiler.

### 8.2 Employees

Çalışanlar stress state'e göre değişir:

- Low stress: temiz çalışma loop'u.
- Medium stress: hızlanma, ter, ufak hata, kısa şikayet balonu.
- High stress: duraksama, tartışma, servis hatası, patrona yürüme.
- Breaking point: işi bırakma, grev, hata zinciri veya müşteriyle kavga event'i.

Çalışanlar mümkünse isimli archetype taşımalıdır: `Aylin - Barista`, `Mert - Kurye`, `Deniz - Developer`. İsimler oyuncunun bağ kurmasını sağlar.

### 8.3 Rival NPCs

Rakip tarafında da fiziksel işaretler olmalıdır:

- Rakip kampanya tabelası.
- Rakip önünde kırmızı kuyruk.
- Oyuncu çalışanına rakipten teklif balonu.
- Rakibin kötü krizinde kırmızı müşterilerin grileşmesi.

## 9. Long-Term Consequences

Event sadece anlık stat değişimi değildir. Her ciddi event bir iz bırakır.

İz tipleri:

- `Temporary Modifier`: 1-5 tur süren etki.
- `Persistent Tag`: gelecek event'lerin bağlamını değiştirir.
- `Relationship Change`: çalışan/müşteri/rakip algısı değişir.
- `Deck Bias`: sonraki kart tekliflerini etkiler.
- `Visual Scar`: işletmede bir süre görünen iz.

Örnek:

- Çalışanları geçiştirmek: `staff_resentment` tag'i bırakır; ileride daha sert grev event'i tetiklenir.
- Sahte yorum almak: rating artar; `fake_review_suspicion` tag'i denetim veya platform ban event'i açar.
- Özür kampanyası yapmak: kısa vadede cash gider; `brand_honesty` tag'i sadık müşteri üretir.

## 10. Anti-Repetition Rules

Event sistemi tekrar hissini azaltmak için şu kuralları kullanır:

- Aynı event family cooldown'a girer.
- Aynı trigger farklı actor ile gelirse diyalog ve consequence değişir.
- Event text'i venture, tag ve scale stage'e göre varyant seçer.
- Oyuncunun son seçimi tekrar teklif edilmez veya farklı maliyetle gelir.
- Event küçükse ambient reaction olarak çözülür; her şey decision event olmaz.

## 11. Readability Rules

Oyuncu her event sonrası 5 saniyede üç şeyi anlamalıdır:

1. Ne oldu?
2. Neden oldu?
3. Şimdi hangi baskı kaldı?

Bunun için:

- Event başlığı problem cümlesi olmalıdır.
- Kamera olayın fiziksel kaynağına gitmelidir.
- Seçim sonucu anında görünmelidir.
- Yeni pressure küçük ama net bir world label ile gösterilmelidir.

## 12. Business Drama Formula

Business drama kişisel ve sistemik baskının birleşimidir.

Güçlü event formülü:

```text
Bir insan veya işletme parçası zorlanır
çünkü oyuncu daha önce bir tradeoff seçmiştir.
Oyuncu şimdi yeni bir tradeoff seçer.
Dünya bu seçimi hemen gösterir.
Yeni problem daha ilginç hale gelir.
```

Örnek:

```text
Ucuz maaş + Black Hat campaign
-> Trafik patlar
-> Garsonlar yetişemez
-> Aylin patrona gelir: "Bu tempoyla çalışamam."
-> Oyuncu maaş artırır, kovar veya söz verir
-> Müşteri akışı, capacity ve staff stability sahada değişir
```

## 13. Failure Should Be Entertaining

Başarısızlık sadece kayıp ekranı değildir. Oyuncu işletmenin nasıl çöktüğünü anlamalı ve bir sonraki run için hikaye çıkarmalıdır.

Failure ilkeleri:

- Çöküş zinciri görünür olmalı.
- Oyuncuya en az bir pahalı toparlanma yolu sunulmalı.
- Ağır kriz bile yeni stratejik fırsat açabilir.
- Komik/stylized animasyon olabilir, ama kararların ağırlığı hafife alınmamalıdır.

Örnek toparlanma:

- Rating 3.2'ye düşer.
- Müşteriler kırmızıya kayar.
- Oyuncuya `Public Apology`, `Quality Overhaul`, `Fake Review Push` seçenekleri gelir.
- Doğru toparlanma uzun vadede daha sadık müşteri üretebilir.

## 14. Reputation Damage Visuals

Rating hasarı şu şekillerde görünür:

- Yıldız panelinde çatlama veya solma.
- District'te negatif review balonları.
- Mavi müşterilerin pale blue olup yavaşlaması.
- Gri müşterilerin oyuncu girişinde kararsız kalması.
- Rakip çekim alanının kırmızı parlaması.
- İşletme önünde daha az organik foot traffic.

Reputation recovery de görünür olmalıdır:

- Özür kampanyası sonrası küçük ikram animasyonu.
- Gold loyal customer oluşumu.
- Review balonlarının nötrden pozitife dönmesi.
- Mavi akışın tekrar hızlanması.

## 15. Market Share as a Living System

Market share sadece yüzde değildir. Üç katmanda gösterilir:

1. Physical crowd distribution: district'teki gri/mavi/kırmızı yoğunluk.
2. Flow direction: müşterilerin hangi işletmeye yürüdüğü.
3. Strategic UI: yüzde barı, trend oku ve pressure etiketi.

Market share değişimi her resolve sonunda küçük ama okunur bir beat alır. Büyük değişimlerde camera `Market Flow View` moduna geçer ve müşteri renkleri fiziksel olarak kayar.

## 16. Event Acceptance Criteria

Bir event oyuna eklenmeye hazır sayılmak için şunlara sahip olmalıdır:

- Net trigger condition.
- Problem cümlesi.
- Kamera odağı.
- En az bir NPC veya world animation.
- 1-3 oyuncu seçeneği.
- Her seçeneğin görünür sonucu.
- En az bir stat veya tag sonucu.
- Cooldown veya tekrar varyantı.
- Venture'a göre text/visual uyarlaması.
