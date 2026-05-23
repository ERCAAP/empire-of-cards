# GAME DESIGN DOCUMENT
# "Empire of Cards" v5

> Versiyon: 5.0 | Tarih: 2026-05-23
> Tür: Question-driven business card strategy
> Platform: PC / Steam

---

# 1. Yüksek Konsept ve Oyuncu Fantezisi

## 1.1 Oyuncu Vaadi

Empire of Cards, oyuncuya mahallede iş kurup rakibini geride bırakma hissi veren, karar baskısı yüksek, kart tabanlı bir işletme simülasyonudur.

Oyuncu bir "slot dolduran masa oyunu" oynamaz. Oyuncu:

- işletmesini kurar,
- tur başına gelen gerçek problemleri çözer,
- müşteri akışını gözle görünür şekilde etkiler,
- kısa vadeli kurtarış ile uzun vadeli istikrar arasında karar verir,
- rakibin aynı müşteri havuzu için baskısını hisseder.

## 1.2 Oyunun Cümlesi

> "Mahalledeki müşterileri rakibinden önce kendine çekmek için işletmeni kur, sorulara kartlarla cevap ver ve her turun sonunda mahallenin sana mı rakibine mi aktığını izle."

## 1.3 Çekirdek Fantezi

- Senin işletmen altta, rakibin işletmesi üstte yaşar.
- Ortada mahalle ve müşteri havuzu görünür.
- Her tur 2 baskı sorusu açılır.
- Oyuncu 1 kalıcı gelişim kartı ve soru başına uygun cevap kartları oynar.
- Tur sonunda müşteriler fiziksel olarak taraf değiştirir.

---

# 2. Çekirdek Tasarım İlkeleri

## 2.1 Tasarım Sütunları

1. Kartlar büyü değil, işletme kararıdır.
2. Board dekor değil, yaşayan durum ekranıdır.
3. Sorular oyunun ana oynanış motorudur.
4. Ekonomi derin olmalı ama okunamaz olmamalıdır.
5. Her iyi karar bir bedel ya da fırsat maliyeti taşımalıdır.
6. Müşteri akışı oyunun ana geri bildirimi olmalıdır.

## 2.2 Neden Board Hala Var

Bu tasarım question-first'tür ama boardless değildir.

Board'un görevleri:

- oyuncunun kurduğu kalıcı işletmeyi göstermek,
- müşteri savaşını görselleştirmek,
- rakibin baskısını okunur tutmak,
- aktif etkileri ve kalıcı yatırımları sahnede yaşatmak,
- tur sonunda kararların sonucu olarak neyin değiştiğini göstermektir.

## 2.3 Oyuncunun Her Tur Yaptığı Şey

1. Turn brief'i okur.
2. Ortaya çıkan 2 aktif soruyu görür.
3. Elindeki kartlardan bu sorulara uygun cevapları seçer.
4. Aynı tur 1 kalıcı build kartını işletmesine yerleştirir.
5. Turu bitirir ve müşteri akışı, gelir, risk ve rakip cevabını izler.

---

# 3. Board ve Level Design

## 3.1 Üç Bölge Yapısı

### Üst Bölge: Rival Zone

- kırmızı tonlu rakip işletme
- rakibin aktif personeli, kampanyaları ve baskıları görünür
- tüm iç detaylar açık edilmez
- rakip güçlü ama tamamen şeffaf olmayan bir tehdit olarak hissedilir

### Orta Bölge: District / Market Zone

- gri, nötr müşteri havuzu burada yaşar
- tur içi soru panelleri burada açılır
- resolve anında müşteriler bu bölgeden sana veya rakibe akar
- market share sayısal olmadan da burada hissedilir

### Alt Bölge: Player Business Zone

- oyuncunun kalıcı işletmesi burada büyür
- personel, ekipman, supplier bağlantıları ve aktif altyapı burada görünür
- mavi ve sıcak tonlarla sahiplik hissi verir

## 3.2 Alt Şerit: Hand Rail

- eldeki kartlar alt şeritte tutulur
- kartlar fiziksel ve taktil görünür
- turu bitir butonu burada yer alır
- oyuncu elindeki kartları hem soru paneline hem işletme anchor'larına sürükleyebilir

## 3.3 Yan Paneller

Sağ veya sol yan paneller şu işlevleri taşır:

- anlık ana metrikler
- turn report
- decision history
- aktif temp effect listesi
- son rakip hamlesi özeti

## 3.4 Board Üzerinde Neler Hareket Eder

- müşteriler
- soru panelleri
- aktif kampanya ve baskı ikonları
- temp effect işaretleri
- resolve anındaki para, puan ve risk pop-up'ları

## 3.5 Kalıcı Olanlar

- personel varlığı
- ekipman
- supplier anlaşmaları
- kalıcı marketing altyapısı
- işletme kapasite yapısı

## 3.6 Görsel Okunabilirlik Kuralları

- oyuncu gözü önce ortaya, sonra kendi işletmesine, sonra rakibe kaymalıdır
- oyuncunun işletmesi detaylı, rakip işletme özetlenmiş görünmelidir
- soru paneli sahneye entegre olmalı, menü gibi kopuk durmamalıdır
- resolve anında müşteri akışı en baskın animasyon olmalıdır

---

# 4. Ana Oyun Döngüsü ve Tur Yapısı

## 4.1 Tur Yapısı

Her standart tur şu sırayla akar:

1. Turn Start Brief
2. Question Spawn
3. Decision Placement
4. Persistent Build Placement
5. Resolve
6. Customer Movement
7. Rival Response
8. State Carryover

## 4.2 Turn Start Brief

Tur başında oyuncuya kısa ama operasyonel anlam taşıyan bir brief verilir.

Brief şunları açıklar:

- günün baskısı
- hangi metriğin kırılgan olduğu
- rakipten gelen görünür tehdit
- önerilen oyun yönü

## 4.3 Question Spawn

Varsayılan tempo:

- her tur 2 aktif soru
- risk eşiği aşılırsa 1 zorunlu bonus reaksiyon sorusu
- milestone turlarında daha güçlü soru pencereleri

Sorular şu kaynaklardan gelir:

- venture script beat'leri
- board state tetikleri
- ekonomi ve risk eşikleri
- rakip hamleleri
- milestone turn kırılmaları

## 4.4 Decision Placement

Oyuncu response kartlarını orta bölgedeki soru paneline bırakır.

Bir soru:

- 1 ana ihtiyaç etiketi ister
- isteğe bağlı 1 destek etiketi isteyebilir
- risk uyarısı taşıyabilir
- çözülünce yeni temp effect ya da follow-up build ihtiyacı doğurabilir

## 4.5 Persistent Build Placement

Oyuncu her tur 1 kalıcı build kartını işletme anchor'larından birine yerleştirir.

Kalıcı build:

- kapasite kurar,
- yeni insan gücü ekler,
- supplier zinciri tanımlar,
- marketing altyapısı oluşturur,
- sonraki turların soru çözme potansiyelini değiştirir.

## 4.6 Resolve

Resolve şu sırayla hesaplanır:

1. aktif soru çözümleri
2. operasyon ve staff etkisi
3. supplier ve quality etkisi
4. demand üretimi ve service kapasitesi
5. reputation güncellemesi
6. gelir/gider ve nakit sonucu
7. risk ve gizli baskı birikimi
8. market share kayması

## 4.7 Customer Movement

Resolve sonrası müşteriler fiziksel olarak hareket eder.

- gri müşteriler nötr havuzdur
- maviye dönenler o tur sana gelen müşteridir
- kırmızıya dönenler rakibe giden müşteridir
- sadık müşteriler gelecekte geri dönme eğilimi taşır

## 4.8 Rival Response

Rakip her tur görünür bir niyet ya da hamle üretir:

- fiyat baskısı
- personel transferi
- agresif kampanya
- daha hızlı servis
- trust/reputation oyunu

Oyuncu rakibin tam formülünü görmez, ama board'da baskısını hisseder.

## 4.9 State Carryover

Tur sonunda:

- kalıcı build kartları board'da kalır
- response kartları discard olur veya temp effect'e dönüşür
- risk ve güven birikimleri taşınır
- decision history'ye tur özeti düşer

---

# 5. Soru Sistemi

## 5.1 Soru Nedir

Soru, oyuncuya o tur "hangi problemi nasıl çözeceksin" diye soran ana oynanış birimidir.

Soru bir kart metni değil, sahne içinde açılan karar panelidir.

## 5.2 Soru Yapısı

Her soru şu alanları taşır:

- başlık
- bağlam metni
- ana ihtiyaç etiketi
- opsiyonel destek etiketi
- risk uyarısı
- önerilen kart tipleri
- follow-up ihtimali

## 5.3 Standart Etiketler

Tüm venture'larda ortak kullanılan etiketler:

- Cash
- Demand
- Capacity
- Quality
- Staff
- Speed
- Risk
- Reputation
- Loyalty
- Supply

## 5.4 Soru Sonuç Tipleri

Bir soru şu biçimlerde sonuçlanabilir:

- temiz çözüldü
- pahalı çözüldü
- riskli çözüldü
- kısmen çözüldü
- görmezden gelindi

## 5.5 Soru Örnekleri

- Öğle kalabalığı geliyor, yetişecek misin?
- Tedarik gecikti, kaliteyi mi maliyeti mi koruyacaksın?
- İlk yorumlar düşüyor, güveni nasıl toparlayacaksın?
- Rakip iyi elemanını ayartıyor, maaşı mı sadakati mi artıracaksın?

## 5.6 2 Questions + 1 Build Tempo Kuralı

Temel tempo kilidi:

- 2 aktif soru
- 1 kalıcı build hakkı
- risk yükselirse ek reaksiyon sorusu

Bu yapı oyuna hem yönetilebilirlik hem baskı sağlar.

---

# 6. Kart Sistemi ve Yerleştirme Kuralları

## 6.1 Kart Aileleri

### Persistent Build Cards

- staff alımı
- ekipman yatırımı
- supplier anlaşması
- kalıcı marketing altyapısı

Yerleşim:

- player business zone içindeki business anchor'larına gider
- board'da kalır

### Decision Response Cards

- mesai
- indirim
- özür kampanyası
- acil alım
- fiyat ayarı

Yerleşim:

- soru panelindeki drop alanına gider
- bu tur çözülür
- sonra discard olur veya temp effect'e dönüşür

### Risk / Opportunity Cards

- denetim
- influencer ziyareti
- grev tehdidi
- sahte yorum fırsatı
- tedarik şoku

Yerleşim:

- bazıları soru olarak açılır
- bazıları temp effect strip'e düşer
- bazıları kriz zinciri başlatır

## 6.2 Kart Bilgi Yapısı

Her kart şu tasarım alanlarıyla tanımlanır:

- primary tags
- secondary tags
- cost
- immediate effect
- end-turn effect
- persistence
- delayed risk
- venture affinity

## 6.3 Business Anchor Mantığı

Görselde klasik slot kutuları yoktur, ama sistemsel anchor mantığı vardır.

Ana anchor aileleri:

- Operation
- Staff
- Marketing
- Supplier
- Temp Effect

Bunlar oyuncuya "slot" diye sunulmaz. İşletmenin gerçek parçaları olarak görünürler.

## 6.4 Kart Seçim Felsefesi

Kartlar iyi/kötü olarak değil, bağlama uygunluk üzerinden değerlenir.

Bir kart:

- ideal cevap olabilir,
- yasal ama zayıf cevap olabilir,
- çok güçlü ama tehlikeli cevap olabilir.

Oyuncu bugünü kurtarıp yarını bozabilmelidir.

## 6.5 Persistent vs Temporary State

Kalıcı durum:

- personel
- ekipman
- supplier
- uzun ömürlü marketing altyapısı

Geçici durum:

- mesai baskısı
- denetim hazırlığı
- kampanya etkisi
- kısa süreli şikayet baskısı

---

# 7. Ekonomi ve Pazar Simülasyonu

## 7.1 Görünür Ana Metrikler

- Cash
- Customer Pull
- Service Capacity
- Product / Service Quality
- Reputation
- Staff Stability
- Legal / Operational Risk
- Market Share

## 7.2 Venture-Specific Destek Metrikleri

### Fast Food / Cafe

- ingredient quality
- hygiene
- service speed

### Tech App

- product stability
- churn
- server load

### Giyim

- stock freshness
- seasonal fit
- return pressure

### Market / Bakkal

- spoilage pressure
- shelf health
- local credit load

## 7.3 Gizli Birikim Baskıları

- burnout buildup
- review instability
- inspection readiness
- supplier reliability drift
- rival pressure bank

## 7.4 Temel Ekonomi Akışı

Ekonomi şuna göre çözülür:

`Pull x Trust x Capacity Fit x Quality`

Yüksek talep tek başına kazandırmaz.

Kazanç için:

- müşteriyi çekmek,
- gelen müşteriye yetişmek,
- kaliteyi korumak,
- güveni sürdürülebilir tutmak gerekir.

## 7.5 Oyunun Gerçek Gerginliği

Ana baskılar:

- oyuncu tüm soruları aynı anda iyi çözemez
- kısa vadeli çözümler uzun vadeli borç veya risk yaratır
- kalıcı büyüme nakit ve maaş baskısı doğurur
- rakip zayıf alanına saldırır
- müşteri kazanımı geri çevrilebilir

---

# 8. Müşteri Akışı ve Market Share Görselleştirmesi

## 8.1 Müşteri Akışı Ana Feedback'tir

Oyunun en önemli sahne geri bildirimi müşteri hareketidir.

Oyuncu kararının sonucu:

- sayıyla,
- panel raporuyla,
- sahne hareketiyle

aynı anda okunmalıdır.

## 8.2 Müşteri Grupları

- nötr gri müşteri
- oyuncuya kayan mavi müşteri
- rakibe kayan kırmızı müşteri
- sadık müşteri
- kararsız müşteri

## 8.3 Market Share Kuralı

Market share her tur yeniden tartılır.

Milestone turlar büyük kırılma üretir ama günlük çekişmeyi iptal etmez.

## 8.4 Neden Bu Sistem Eğlencelidir

- oyuncu sayısal üstünlüğü gözle görür
- rakibin baskısı sahnede okunur
- kart kararları hemen sonuç üretir
- board statik kalmaz

---

# 9. Rakip Tasarımı

## 9.1 Sabit Kural

Rakip aynı venture ile oynar ve aynı müşteri havuzu için savaşır.

## 9.2 Rakibin Rolü

Rakip yalnızca sayı rakibi değildir. Rakip:

- oyuncunun zayıf tarafını cezalandırır,
- müşteriyi çalar,
- personel transfer etmeye çalışır,
- riskten faydalanır,
- baskı yaratır ama tüm iç planını açmaz.

## 9.3 Rakip Davranış Aileleri

- aggressive
- conservative
- balanced
- opportunist

## 9.4 Rakip Nasıl Gösterilir

- üst bölgedeki işletme durumu
- görünür kampanya ve baskı ikonları
- kısa hamle bildirimi
- müşteri akışındaki kırmızı hareket

---

# 10. Oyun Aşamaları

## 10.1 Opening

Amaç:

- minimum operasyonu kurmak
- ilk güveni kazanmak
- ilk müşteri akışını başlatmak

Soru tipi:

- fark edilme
- yetişme
- ilk ekip seçimi

## 10.2 Stabilization

Amaç:

- staff, supply ve review dengesini kurmak
- ilk darboğazları çözmek

Soru tipi:

- kalite mi maliyet mi
- hız mı güven mi
- maaş mı büyüme mi

## 10.3 Escalation

Amaç:

- rakibin baskısını yönetmek
- birden çok sistem çarpışmasını taşımak

Soru tipi:

- poaching
- kötü yorum dalgası
- tedarik arızası
- kapasite kırılması

## 10.4 Expansion / Domination

Amaç:

- makineleşmiş bir işletme yönetmek
- karmaşıklık artarken güveni korumak
- şubeleşme veya baskın pazar hakimiyeti kurmak

Soru tipi:

- ikinci şube baskısı
- marka itibarı krizi
- yasal denetim zinciri
- aşırı büyümenin yan etkileri

---

# 11. Venture Adaptation Framework

## 11.1 Tüm Venture'lar İçin Ortak Çatı

Her venture aynı üst döngüyü kullanır:

- 3-zone board
- 2 questions + 1 build
- customer movement
- persistent anchors
- readable deep sim

## 11.2 Venture'ların Değiştirdiği Alanlar

- soru aileleri
- support metric'ler
- pressure map
- kart havuzu
- rival davranış önceliği
- müşteri kazanım mantığının tonu

## 11.3 Venture Identity Kuralları

Venture farkı kozmetik değildir.

Her venture:

- farklı bir baskı eğrisi,
- farklı soru dili,
- farklı kalıcı anchor önceliği,
- farklı risk yapısı üretmelidir.

---

# 12. UI/UX Okunabilirliği ve Geçmiş Panelleri

## 12.1 Decision History and Turn Memory

Oyuncu şu soruların cevabını her an alabilmelidir:

- bu tur ne seçtim?
- hangi etkiler hala aktif?
- geçen tur neden müşteri kaybettim?
- rakip son ne yaptı?

## 12.2 Zorunlu Geçmiş Katmanları

### Current Turn Queue

Bu tur commit edilen kartlar görünür.

### Last Turn Report

Bir önceki turun:

- müşteri farkı
- nakit sonucu
- reputation etkisi
- risk artışı

özetlenir.

### Decision Ledger

Son 3-5 turun karar hafızası tutulur.

## 12.3 Okunabilirlik Kuralı

Oyuncu battığında "neden battığını" anlayabilmelidir.

Bu oyun cezayı gizleyerek değil, sebep-sonuç zincirini görünür kılarak zor olmalıdır.

---

# 13. Kazanma ve Kaybetme Koşulları

## 13.1 Ana Kazanma Hissi

- market share üstünlüğü
- mahallede açık baskınlık
- daha sağlıklı işletme makinesi
- rakibi stratejik olarak sıkıştırma

## 13.2 Kaybetme Hissi

- nakit çöküşü
- trust / reputation çöküşü
- kapasite yetişememe zinciri
- staff dağılması
- yasal riskin cezaya dönüşmesi
- rakibin müşteriyi kalıcı biçimde ele geçirmesi

## 13.3 Kaybetme Şartları

- iflas
- kritik güven çöküşü
- belirli süre toparlanamayan operasyon iflası
- milestone hedeflerinin sürekli kaçırılması

---

# 14. Playtest Senaryoları

## 14.1 Fast Food Turn 1

- oyuncu 2 basit soru görür
- 1 build ve 1 response oynar
- tur sonunda müşteriler bölünür

## 14.2 Fast Food Turn 2

- kısa vadeli çözüm burnout ya da hijyen riski üretir
- oyuncu bunun nedenini raporda anlar

## 14.3 Cafe Reputation Loop

- yavaş ama güvenli büyüme müşteri sadakati yaratır
- hızlı büyüme kadar patlayıcı olmayan ama daha dirençli bir akış verir

## 14.4 Tech App Service Failure

- fiziksel olmayan venture bile aynı board gramerine oturur
- orta bölgedeki kullanıcı akışı ile pazar baskısı okunur

## 14.5 Rival Counterplay

- rakip oyuncunun açık verdiği metriğe yüklenir
- bu baskı sahnede görünür

## 14.6 History Readability

- oyuncu son 3 turda ne yaptığını okuyabilir
- aktif durum ile geçmiş karar ayrımı nettir

---

# 15. MVP ve Genişleme Önceliği

## 15.1 MVP Önceliği

- 3-zone live board
- 2 questions + 1 build tempo
- müşteri hareketi
- decision history
- visible rival pressure
- Fast Food referans denge

## 15.2 Faz 1 Venture'ları

- Fast Food
- Cafe
- Market / Bakkal

## 15.3 Faz 2 Venture'ları

- Tech App
- Giyim Magazası

## 15.4 Genişleme Mantığı

Yeni içerik yeni kart eklemekten önce:

- yeni soru aileleri,
- yeni pressure map'ler,
- yeni müşteri davranışları,
- yeni rival baskıları

üretmelidir.

---

# 16. Kabul Kriterleri

## 16.1 Bu GDD'nin Doğru Çalıştığını Nasıl Anlarız

- oyun visible slot filling hissi vermiyorsa
- board yaşayan bir sistem gibi davranıyorsa
- kartların nereye gittiği netse
- ekonomi baskısı sade ama ciddi hissediliyorsa
- venture'lar aynı loop içinde farklı kimlik taşıyorsa
- müşteri akışı kararların ana sonucu olarak okunuyorsa

Bu doküman amacına ulaşmıştır.
