# Drone Fleet Management API

[![.NET 10.0](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=flat-square&logo=postgresql&logoColor=white)](https://www.postgresql.org)
[![Docker](https://img.shields.io/badge/Docker-2CA5E0?style=flat-square&logo=docker&logoColor=white)](https://www.docker.com)

İHA (İnsansız Hava Aracı) filolarının ve bu araçlara ait uçuş kayıtlarının merkezi bir sistem üzerinden yönetilmesi için geliştirilmiş bir RESTful API uygulamasıdır. Veri bütünlüğü, katmanlı mimari ve asenkron programlama prensipleri temel alınarak kurgulanmıştır.

---

## Mimari ve Teknolojiler

Uygulama, modern backend standartlarına uygun olarak aşağıdaki bileşenler ve yaklaşımlar üzerine inşa edilmiştir:

| Kategori | Teknoloji / Yaklaşım |
|---|---|
| **Framework** | .NET 10.0 SDK, C# |
| **Veritabanı** | PostgreSQL, Entity Framework Core (Code-First) |
| **Konteyner** | Docker, Docker Compose |
| **Mimari Standart** | RESTful, DTO (Data Transfer Object) Katmanı |
| **Hata Yönetimi** | Merkezi Global Exception Handling Middleware |
| **Güvenlik** | Defense in Depth (Data Annotations & DB Constraints) |
| **Silme Politikası** | Soft Delete (isDeleted bayrağı ile kayıt koruma) |

---

## Kurulum ve Çalıştırma

Proje tamamen Dockerize edilmiştir. Yerel makinenizde .NET veya PostgreSQL kurulu olmasına gerek yoktur.

1. Depoyu yerel makinenize klonlayın.
2. Terminal üzerinden `compose.yaml` dosyasının bulunduğu ana dizine geçin.
3. Aşağıdaki komutu çalıştırarak servisleri başlatın:

```bash
docker compose up -d --build
```

---

## API Erişimi ve Dökümantasyon

Servisler ayağa kalktıktan sonra aşağıdaki adreslerden erişim sağlanabilir:

- **API Base URL:** `http://localhost:8080`
- **Swagger UI (Dökümantasyon):** `http://localhost:8080/swagger`

> **Not:** API uç noktaları, parametreler, şemalar ve test arayüzü için Swagger UI dökümantasyonunu kullanınız. Manuel bir dökümantasyon tutulmamaktadır; tüm güncel tanımlamalar Swagger üzerinden otomatik olarak sunulur.

---

## Teknik Kısıtlamalar ve Validasyonlar

Veri tutarlılığını sağlamak amacıyla uygulanan temel kurallar:

- **İHA Model Adı:** Zorunludur ve maksimum 50 karakter sınırına sahiptir.
- **IP Adresi:** Zorunludur ve IPv4 formatına uygun maksimum 15 karakter olmalıdır.
- **Uçuş Süresi:** Operasyonel limitler gereği 10 ile 3000 dakika arasında tanımlanmalıdır.
- **Uçuş Kaydı:** Açıklama alanı detaylı raporlama için maksimum 500 karakter ile sınırlandırılmıştır.
- **Loglama:** Kritik sistem hataları sunucu tarafında `/Logs` dizini altındaki `.txt` dosyalarına otomatik olarak kaydedilir.

---

## Lisans ve Kullanım

Bu proje eğitim ve standart geliştirme pratiklerini uygulama amacıyla hazırlanmıştır. Ticari kullanım öncesi operasyonel güvenlik gereksinimlerinin gözden geçirilmesi önerilir.