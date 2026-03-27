# Drone Fleet Management API

Bu proje, İHA (İnsansız Hava Aracı) filolarının ve uçuş kayıtlarının merkezi bir sistem üzerinden yönetilmesi amacıyla geliştirilmiş bir RESTful API uygulamasıdır. Veri bütünlüğü, güvenli veri transferi ve katmanlı mimari prensipleri temel alınarak kurgulanmıştır.

## Teknolojiler ve Standartlar

- **Framework:** .NET 10.0
- **Veritabanı & ORM:** PostgreSQL, Entity Framework Core (Code-First)
- **Konteynerleştirme:** Docker, Docker Compose
- **Mimari Yaklaşım:** DTO (Data Transfer Object), Global Error Handling, Soft Delete

## Kurulum ve Çalıştırma

Projenin çalıştırılması için sistemde yalnızca Docker'ın kurulu olması yeterlidir.

1. Depoyu klonlayın.
2. Ana dizinde aşağıdaki komutu çalıştırın:
```bash
docker compose up -d --build
```
API, `http://localhost:8080` adresinde erişime açılacaktır.

## Teknik Kısıtlamalar ve Validasyonlar

Sistemde veri tutarlılığını sağlamak için aşağıdaki kurallar uygulanmaktadır:

- **Model Adı:** Zorunludur ve maksimum 50 karakter olabilir.
- **IP Adresi:** Zorunludur ve maksimum 15 karakter (IPv4 formatı) olabilir.
- **Uçuş Süresi:** Minimum 10, maksimum 3000 dakika arasında olmalıdır.
- **Silme İşlemi:** Veriler veritabanından kalıcı olarak silinmez, `isDeleted` bayrağı ile işaretlenerek sistemden gizlenir (Soft Delete).
- **Loglama:** Uçuş kayıtlarında açıklama alanı maksimum 500 karakter ile sınırlandırılmıştır.

## API Uç Noktaları

### 1. İHA Yönetimi

#### Tüm İHA'ları Listele
`GET /drones`
- Kayıtlı tüm İHA'ları ve ilişkili uçuş kayıtlarını listeler.

#### İHA Detayı Getir
`GET /drones/{id}`
- Belirtilen ID'ye sahip İHA'nın detaylı verisini döner.
- Bulunamazsa `404 Not Found` yanıtı döner.

#### Aktif İHA Özet Listesi
`GET /drones/active-summary`
- Sadece `isActive: true` olan İHA'ların model adı ve IP adresini listeler.

#### Yeni İHA Kaydı
`POST /drones`
- Gövde Parametreleri: `modelName` (string), `ipAddress` (string), `maxFlightTimeMinutes` (int), `isActive` (bool).
- Başarılı işlem sonrası `201 Created` kodu ile eklenen veri döner.

#### İHA Bilgilerini Güncelle
`PUT /drones/{id}`
- Belirtilen ID'ye sahip İHA'nın tüm bilgilerini günceller.

#### İHA Sil
`DELETE /drones/{id}`
- Belirtilen İHA'yı pasif duruma getirir. Yanıt olarak `204 No Content` döner.

### 2. Uçuş Kaydı Yönetimi

#### Uçuş Kaydı Ekle
`POST /drones/{id}/flightlogs`
- Gövde Parametresi: `description` (string, max 500 karakter).
- Kayıt tarihi sunucu tarafından otomatik olarak atanır.
- Başarılı işlem sonrası `200 OK` kodu ile işlem özeti ve veri döner.