# Drone Fleet Management API

[![.NET 10.0](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=flat-square&logo=postgresql&logoColor=white)](https://www.postgresql.org)
[![Docker](https://img.shields.io/badge/Docker-2CA5E0?style=flat-square&logo=docker&logoColor=white)](https://www.docker.com)

İHA filolarını ve uçuş kayıtlarını yönetmek için geliştirilmiş, katmanlı mimari üzerine inşa edilmiş bir REST API.

---

## İçindekiler

- [Mimari](#mimari)
- [Kurulum](#kurulum)
- [API Referansı](#api-referansı)
  - [İHA Yönetimi](#iha-yönetimi)
  - [Uçuş Kaydı Yönetimi](#uçuş-kaydı-yönetimi)
- [Hata Yanıtları](#hata-yanıtları)

---

## Mimari

| Karar | Yaklaşım |
|---|---|
| **Katman Ayrımı** | Entity'ler doğrudan dışa açılmaz; DTO katmanı üzerinden iletişim kurulur |
| **Hata Yönetimi** | Tüm istisnalar merkezi bir middleware'de yakalanır, standart JSON formatında döner |
| **Veri Doğrulama** | DTO/Controller seviyesinde (Data Annotations) + veritabanı kısıtlamaları |
| **Soft Delete** | Silme işlemleri `isDeleted` bayrağıyla yapılır; kayıtlar fiziksel olarak silinmez |
| **Loglama** | Kritik hatalar sunucu tarafında `.txt` dosyasına yazılır |

**Stack:** .NET 10.0 · C# · PostgreSQL · Entity Framework Core (Code-First) · Docker

---

## Kurulum

Projeyi çalıştırmak için yalnızca **Docker** gereklidir — .NET SDK veya PostgreSQL kurulumuna gerek yoktur.
> Docker kurulu değilse → [docs.docker.com/get-docker](https://docs.docker.com/get-docker/)

```bash
git clone https://github.com/kullanici-adi/drone-fleet-api.git
cd drone-fleet-api
docker compose up -d --build
```

API başarıyla ayağa kalktıktan sonra şu adresten erişilebilir:
- **API Base URL:** `http://localhost:8080`
- **Swagger UI:** `http://localhost:8080/swagger` (API dökümantasyonu ve test arayüzü)

---

## API Referansı

**Base URL:** `http://localhost:8080`

---

### İHA Yönetimi

#### `GET /drones` — Tüm İHA'ları Listele

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

#### `GET /drones/{id}` — İHA Detayı

| Parametre | Tür | Açıklama |
|---|---|---|
| `id` | `int` | İHA'nın benzersiz kimlik numarası |

| Kod | Açıklama |
|---|---|
| `200 OK` | `DroneResponseDTO` formatında tek nesne |
| `404 Not Found` | Kayıt bulunamadı |

---

#### `GET /drones/active-summary` — Aktif İHA Özeti

Yalnızca `isActive: true` olan İHA'ların model adı ve IP adresini listeler.

**Yanıt — `200 OK`**
```json
[
  { "modelName": "Gözcü-X1", "ipAddress": "192.168.1.15" }
]
```

---

#### `POST /drones` — Yeni İHA Kaydı

Yeni kayıtlar `isActive: true` ve `isDeleted: false` varsayılan değerleriyle oluşturulur.

**İstek Gövdesi**

| Alan | Tür | Zorunlu | Açıklama |
|---|---|---|---|
| `modelName` | `string` | ✓ | İHA'nın model adı |
| `ipAddress` | `string` | ✓ | Atanan IP adresi |
| `maxFlightTimeMinutes` | `int` | ✓ | Maksimum uçuş süresi (dakika) |
| `isActive` | `bool` | — | Varsayılan: `true` |

```json
{
  "modelName": "Atak-V2",
  "ipAddress": "192.168.1.50",
  "maxFlightTimeMinutes": 240
}
```

| Kod | Açıklama |
|---|---|
| `201 Created` | Eklenen kayıt döner |
| `400 Bad Request` | Zorunlu alan eksik veya geçersiz |

---

#### `PUT /drones/{id}` — İHA Güncelle

| Parametre | Tür | Açıklama |
|---|---|---|
| `id` | `int` | Güncellenecek İHA'nın kimliği |

Tüm alanlar zorunludur (`isActive` dahil).

```json
{
  "modelName": "Atak-V2 Güncel",
  "ipAddress": "192.168.1.55",
  "maxFlightTimeMinutes": 260,
  "isActive": false
}
```

| Kod | Açıklama |
|---|---|
| `200 OK` | Güncellenmiş kayıt döner |
| `404 Not Found` | Kayıt bulunamadı |

---

#### `DELETE /drones/{id}` — İHA Sil (Soft Delete)

Kayıt fiziksel olarak silinmez; `isDeleted` bayrağı işaretlenir.

| Kod | Açıklama |
|---|---|
| `204 No Content` | İşlem başarılı |
| `404 Not Found` | Kayıt bulunamadı |

---

### Uçuş Kaydı Yönetimi

#### `POST /drones/{id}/flightlogs` — Uçuş Kaydı Ekle

Belirtilen İHA'ya yeni bir uçuş kaydı ekler. Kayıt tarihi sunucu tarafından atanır.

| Alan | Tür | Zorunlu | Kısıtlama |
|---|---|---|---|
| `description` | `string` | ✓ | maks. 500 karakter |

```json
{ "description": "Batarya %20 seviyesine düştüğü için otonom dönüş başlatıldı." }
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

| Kod | Açıklama |
|---|---|
| `200 OK` | İşlem özeti ve eklenen kayıt |
| `400 Bad Request` | Açıklama eksik veya karakter sınırı aşıldı |
| `404 Not Found` | İHA bulunamadı |

---

## Hata Yanıtları

Tüm hata durumları merkezi middleware tarafından aşağıdaki standart formatta döndürülür:

```json
{ "mesaj": "Hatanın açıklaması." }
```

| Kod | Durum | Açıklama |
|---|---|---|
| `400` | Bad Request | Doğrulama kuralları karşılanmıyor |
| `404` | Not Found | İstenen kaynak bulunamadı |
| `500` | Internal Server Error | Beklenmeyen sunucu hatası |