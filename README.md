# Drone Fleet Management API

![.NET 10.0](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white)

Gelişmiş İHA (İnsansız Hava Aracı) filolarını ve bu araçlara ait uçuş kayıtlarını yönetmek için tasarlanmış, veri bütünlüğünü ve güvenliğini ön planda tutan sunucu taraflı (server-side) bir uygulamadır.

---

## İçindekiler

- [Projenin Amacı ve Kapsamı](#projenin-amacı-ve-kapsamı)
- [Teknolojiler ve Mimari Standartlar](#teknolojiler-ve-mimari-standartlar)
- [Kurulum ve Çalıştırma](#kurulum-ve-çalıştırma)
- [API Dökümantasyonu](#api-dökümantasyonu)
  - [İHA Yönetimi](#1-i̇ha-yönetimi)
  - [Uçuş Kayıt Yönetimi](#2-uçuş-kayıt-flight-log-yönetimi)

---

## Projenin Amacı ve Kapsamı

Bu proje, modern backend mimarilerini, yazılım mühendisliği prensiplerini ve güncel endüstri standartlarını öğrenmek ve uygulamak amacıyla geliştirilmiştir.

Uygulama aktif olarak herhangi bir kullanıcı arayüzüne (Frontend) bağlı değildir. Temel odak noktası; güvenli veri transferi, katmanlı mimari yaklaşımı, asenkron programlama ve ilişkisel veritabanı yönetiminin API Controller standartları üzerinden kurgulanmasıdır.

---

## Teknolojiler ve Mimari Standartlar

| Kategori | Teknoloji / Yaklaşım |
|---|---|
| **Framework & Dil** | .NET 10.0 SDK, C# |
| **Veritabanı & ORM** | PostgreSQL, Entity Framework Core (Code-First) |
| **Konteynerleştirme** | Docker, Docker Compose |

**Uygulanan Mimari Prensipler:**

**RESTful Tasarım** — API uç noktaları evrensel isimlendirme ve HTTP metot standartlarına uygun olarak tasarlanmıştır.

**DTO (Data Transfer Object) Katmanı** — Veritabanı varlıklarının (Entity) dış dünyaya sızması engellenmiş; istemci ve sunucu arasında yalnızca gerekli verilerin taşınması sağlanmıştır.

**Global Hata Yönetimi** — Uygulama genelinde oluşabilecek istisnai durumlar merkezi bir ara katmanda (middleware) yakalanarak istemciye standartlaştırılmış JSON formatında iletilmektedir.

**Loglama** — Kritik sistem hataları izole edilerek sunucu tarafında fiziksel bir log dosyasına (`.txt`) anlık olarak kaydedilmektedir.

**Defense in Depth** — Veri doğrulama işlemleri hem Controller/DTO seviyesinde (Data Annotations) hem de veritabanı seviyesinde katmanlı olarak uygulanmıştır.

---

## Kurulum ve Çalıştırma

Proje tamamen Dockerize edilmiştir. Uygulamayı çalıştırmak için sisteminizde .NET SDK veya PostgreSQL kurulu olmasına gerek yoktur; yalnızca Docker'ın çalışır durumda olması yeterlidir.

**1.** Depoyu yerel makinenize klonlayın.

**2.** Terminal üzerinden projenin ana dizinine (`compose.yaml` dosyasının bulunduğu konuma) geçiş yapın.

**3.** Aşağıdaki komutu çalıştırarak uygulamayı ve veritabanını izole bir ortamda başlatın:
```bash
docker compose up -d --build
```

İşlem tamamlandığında API, `http://localhost:8080` adresinde istekleri kabul etmeye hazır olacaktır.

---

## API Dökümantasyonu

**Base URL:** `http://localhost:8080`

---

### 1. İHA Yönetimi

#### Tüm İHA'ları Getir

Sistemde kayıtlı olan tüm İHA'ları ve bunlara ait uçuş loglarını listeler.
```
GET /drones
```

**Başarılı Yanıt — `200 OK`**
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

#### Tek Bir İHA Getir

Belirtilen `id`'ye sahip İHA'nın detaylı verisini döner.
```
GET /drones/{id}
```

| Durum | Kod | Açıklama |
|---|---|---|
| Başarılı | `200 OK` | `DroneResponseDTO` formatında tek bir nesne döner. |
| Bulunamadı | `404 Not Found` | `{"mesaj": "1 numaralı İHA bulunamadı."}` |

---

#### Aktif İHA Özet Listesi

Yalnızca sistemde aktif olarak işaretlenmiş İHA'ların özet bilgisini döner.
```
GET /drones/active-summary
```

**Başarılı Yanıt — `200 OK`**
```json
[
  {
    "modelName": "Gözcü-X1",
    "ipAddress": "192.168.1.15"
  }
]
```

---

#### Yeni İHA Ekle

Sisteme yeni bir İHA kaydeder. Yeni kayıtlar varsayılan olarak `isActive: true` ve `isDeleted: false` olarak işaretlenir.
```
POST /drones
```

**İstek Gövdesi**
```json
{
  "modelName": "Atak-V2",
  "ipAddress": "192.168.1.50",
  "maxFlightTimeMinutes": 240
}
```

| Durum | Kod | Açıklama |
|---|---|---|
| Başarılı | `201 Created` | Eklenen veri `DroneResponseDTO` formatında döner. |
| Doğrulama Hatası | `400 Bad Request` | Zorunlu alanlar eksik veya hatalıysa döner. |

---

#### İHA Bilgilerini Güncelle

Belirtilen İHA'nın mevcut verilerini tam kapsamlı olarak günceller.
```
PUT /drones/{id}
```

**İstek Gövdesi**
```json
{
  "modelName": "Atak-V2 Güncel",
  "ipAddress": "192.168.1.55",
  "maxFlightTimeMinutes": 260,
  "isActive": false
}
```

**Başarılı Yanıt — `200 OK`:** Güncellenmiş veri döner.

---

#### İHA Sil (Soft Delete)

Belirtilen İHA'yı sistemden pasif duruma çeker. Kayıt kalıcı olarak silinmez; `isDeleted` bayrağı işaretlenir.
```
DELETE /drones/{id}
```

**Başarılı Yanıt — `204 No Content`:** Gövdesiz yanıt döner.

---

### 2. Uçuş Kayıt (Flight Log) Yönetimi

#### Uçuş Kaydı Ekle

Belirtilen `id`'ye sahip İHA için yeni bir uçuş kayıt raporu oluşturur. Kayıt tarihi sunucu tarafından otomatik olarak atanır.
```
POST /drones/{id}/flightlogs
```

**İstek Gövdesi**
```json
{
  "description": "Batarya %20 seviyesine düştüğü için otonom dönüş başlatıldı."
}
```

**Başarılı Yanıt — `200 OK`**
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