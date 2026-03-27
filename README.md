# Drone Fleet Management API

![.NET 10.0](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white)

İHA (İnsansız Hava Aracı) filolarını ve uçuş kayıtlarını yönetmek amacıyla geliştirilmiş, katmanlı mimari ve güvenlik öncelikleri üzerine inşa edilmiş bir REST API uygulamasıdır.

<br>

## İçindekiler

- [Genel Bakış](#genel-bakış)
- [Mimari ve Tasarım Kararları](#mimari-ve-tasarım-kararları)
- [Kurulum](#kurulum)
- [API Referansı](#api-referansı)
  - [İHA Yönetimi](#iha-yönetimi)
  - [Uçuş Kaydı Yönetimi](#uçuş-kaydı-yönetimi)
- [Hata Yanıtları](#hata-yanıtları)

<br>

---

## Genel Bakış

Bu proje, modern .NET backend mimarisini, yazılım mühendisliği prensiplerini ve endüstri standartlarını uygulamalı olarak ele almak amacıyla geliştirilmiştir. Herhangi bir frontend uygulamasına bağlı değildir; odak noktası güvenli veri transferi, temiz katmanlı yapı ve ilişkisel veritabanı yönetimidir.

| Alan | Teknoloji |
|---|---|
| Framework & Dil | .NET 10.0 SDK, C# |
| Veritabanı & ORM | PostgreSQL, Entity Framework Core (Code-First) |
| Konteynerleştirme | Docker, Docker Compose |

<br>

---

## Mimari ve Tasarım Kararları

**RESTful Tasarım**
HTTP metodları ve kaynak isimlendirmesi evrensel REST standartlarına uygun şekilde tasarlanmıştır.

**DTO Katmanı**
Veritabanı varlıkları (Entity) doğrudan dışa açılmaz. İstemci ile sunucu arasında yalnızca gerekli veriler `DTO` nesneleri aracılığıyla taşınır.

**Global Hata Yönetimi**
Tüm istisnai durumlar merkezi bir middleware katmanında yakalanır ve istemciye standartlaştırılmış JSON formatında iletilir.

**Loglama**
Kritik sistem hataları sunucu tarafında fiziksel bir `.txt` log dosyasına anlık olarak kaydedilir.

**Defense in Depth**
Veri doğrulama; Controller/DTO seviyesinde (Data Annotations) ve veritabanı kısıtlamaları olmak üzere iki katmanda uygulanmaktadır.

<br>

---

## Kurulum

Proje tamamen Dockerize edilmiştir. Sisteminizde .NET SDK veya PostgreSQL kurulu olmasına gerek yoktur.

**Gereksinim:** Docker
```bash
# Depoyu klonlayın
git clone https://github.com/kullanici-adi/drone-fleet-api.git
cd drone-fleet-api

# Uygulamayı ve veritabanını başlatın
docker compose up -d --build
```

Başarılı kurulumun ardından API aşağıdaki adreste çalışmaya başlayacaktır:
```
http://localhost:8080
```

<br>

---

## API Referansı

**Base URL:** `http://localhost:8080`

<br>

### İHA Yönetimi

---

#### Tüm İHA'ları Listele
```
GET /drones
```

Kayıtlı tüm İHA'ları ve ilişkili uçuş kayıtlarını döner.

**Yanıt — `200 OK`**
```json
[
  {
    "id": 1,
    "modelName": "Gözcü-X1",
    "ipAddress": "192.168.1.15",
    "maxFlightTimeMinutes": 120,
    "isActive": true,
    "flightLogs": [
      {
        "id": 1,
        "logDate": "2026-03-28T09:00:00Z",
        "description": "Rutin sınır devriyesi tamamlandı.",
        "droneId": 1
      }
    ]
  }
]
```

---

#### İHA Detayı Getir
```
GET /drones/{id}
```

Belirtilen `id`'ye sahip İHA'nın detaylı verisini döner.

| Parametre | Tür | Konum | Açıklama |
|---|---|---|---|
| `id` | `int` | path | İHA'nın benzersiz kimlik numarası |

**Yanıtlar**

| Kod | Açıklama |
|---|---|
| `200 OK` | `DroneResponseDTO` formatında tek bir nesne |
| `404 Not Found` | Belirtilen ID'ye sahip kayıt bulunamadı |

**Hata Yanıtı — `404 Not Found`**
```json
{
  "mesaj": "1 numaralı İHA bulunamadı."
}
```

---

#### Aktif İHA Özet Listesi
```
GET /drones/active-summary
```

Yalnızca `isActive: true` olan İHA'ların model adı ve IP adresini listeler.

**Yanıt — `200 OK`**
```json
[
  {
    "modelName": "Gözcü-X1",
    "ipAddress": "192.168.1.15"
  }
]
```

---

#### Yeni İHA Kaydı
```
POST /drones
```

Sisteme yeni bir İHA kaydeder. Yeni kayıtlar `isActive: true` ve `isDeleted: false` varsayılan değerleriyle oluşturulur.

**İstek Gövdesi**

| Alan | Tür | Zorunlu | Açıklama |
|---|---|---|---|
| `modelName` | `string` | Evet | İHA'nın model adı |
| `ipAddress` | `string` | Evet | İHA'ya atanan IP adresi |
| `maxFlightTimeMinutes` | `int` | Evet | Maksimum uçuş süresi (dakika) |
| `isActive` | `bool` | Hayır | Varsayılan: `true` |
```json
{
  "modelName": "Atak-V2",
  "ipAddress": "192.168.1.50",
  "maxFlightTimeMinutes": 240
}
```

**Yanıtlar**

| Kod | Açıklama |
|---|---|
| `201 Created` | Eklenen kayıt `DroneResponseDTO` formatında döner |
| `400 Bad Request` | Doğrulama hatası; zorunlu alan eksik veya geçersiz |

---

#### İHA Bilgilerini Güncelle
```
PUT /drones/{id}
```

Belirtilen İHA kaydını tam kapsamlı olarak günceller.

| Parametre | Tür | Konum | Açıklama |
|---|---|---|---|
| `id` | `int` | path | Güncellenecek İHA'nın kimlik numarası |

**İstek Gövdesi**

| Alan | Tür | Zorunlu | Açıklama |
|---|---|---|---|
| `modelName` | `string` | Evet | İHA'nın model adı |
| `ipAddress` | `string` | Evet | İHA'ya atanan IP adresi |
| `maxFlightTimeMinutes` | `int` | Evet | Maksimum uçuş süresi (dakika) |
| `isActive` | `bool` | Evet | İHA'nın aktiflik durumu |
```json
{
  "modelName": "Atak-V2 Güncel",
  "ipAddress": "192.168.1.55",
  "maxFlightTimeMinutes": 260,
  "isActive": false
}
```

**Yanıtlar**

| Kod | Açıklama |
|---|---|
| `200 OK` | Güncellenmiş kayıt döner |
| `404 Not Found` | Belirtilen ID'ye sahip kayıt bulunamadı |

---

#### İHA Sil
```
DELETE /drones/{id}
```

Belirtilen İHA'yı pasif duruma getirir. Kayıt veritabanından kalıcı olarak silinmez; `isDeleted` bayrağı işaretlenir (Soft Delete).

| Parametre | Tür | Konum | Açıklama |
|---|---|---|---|
| `id` | `int` | path | Silinecek İHA'nın kimlik numarası |

**Yanıtlar**

| Kod | Açıklama |
|---|---|
| `204 No Content` | İşlem başarılı; yanıt gövdesi döndürülmez |
| `404 Not Found` | Belirtilen ID'ye sahip kayıt bulunamadı |

<br>

### Uçuş Kaydı Yönetimi

---

#### Uçuş Kaydı Ekle
```
POST /drones/{id}/flightlogs
```

Belirtilen İHA'ya yeni bir uçuş kaydı ekler. Kayıt tarihi sunucu tarafından otomatik olarak atanır.

| Parametre | Tür | Konum | Açıklama |
|---|---|---|---|
| `id` | `int` | path | Kaydın ekleneceği İHA'nın kimlik numarası |

**İstek Gövdesi**

| Alan | Tür | Zorunlu | Kısıtlama | Açıklama |
|---|---|---|---|---|
| `description` | `string` | Evet | maks. 500 karakter | Uçuş kaydının açıklaması |
```json
{
  "description": "Batarya %20 seviyesine düştüğü için otonom dönüş başlatıldı."
}
```

**Yanıt — `200 OK`**
```json
{
  "mesaj": "Flight Log Added to Drone: 1",
  "data": {
    "id": 2,
    "logDate": "2026-03-28T14:30:00Z",
    "description": "Batarya %20 seviyesine düştüğü için otonom dönüş başlatıldı.",
    "droneId": 1
  }
}
```

**Yanıtlar**

| Kod | Açıklama |
|---|---|
| `200 OK` | İşlem özeti ve eklenen kayıt döner |
| `400 Bad Request` | Doğrulama hatası; açıklama zorunludur veya karakter sınırı aşıldı |
| `404 Not Found` | Belirtilen ID'ye sahip İHA bulunamadı |

<br>

---

## Hata Yanıtları

Tüm hata durumları merkezi middleware tarafından yakalanır ve aşağıdaki standart formatta döndürülür.
```json
{
  "mesaj": "Hatanın açıklaması."
}
```

| Kod | Durum | Açıklama |
|---|---|---|
| `400` | Bad Request | İstek gövdesi doğrulama kurallarını karşılamıyor |
| `404` | Not Found | İstenen kaynak bulunamadı |
| `500` | Internal Server Error | Sunucu taraflı beklenmeyen hata |