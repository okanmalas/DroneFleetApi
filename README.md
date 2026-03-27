# Drone Fleet Management API

[![.NET 10.0](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=flat-square&logo=postgresql&logoColor=white)](https://www.postgresql.org)
[![Docker](https://img.shields.io/badge/Docker-2CA5E0?style=flat-square&logo=docker&logoColor=white)](https://www.docker.com)

İHA filolarının ve uçuş kayıtlarının merkezi olarak yönetilmesi için geliştirilmiş RESTful API. Veri bütünlüğü, katmanlı mimari ve asenkron programlama prensipleri temel alınmıştır.

---

## Mimari

| Kategori | Teknoloji / Yaklaşım |
|---|---|
| **Framework** | .NET 10.0, C# |
| **Veritabanı** | PostgreSQL · Entity Framework Core (Code-First) |
| **Konteyner** | Docker · Docker Compose |
| **Mimari** | RESTful · DTO Katmanı |
| **Hata Yönetimi** | Global Exception Handling Middleware |
| **Güvenlik** | Data Annotations + Veritabanı Kısıtlamaları |
| **Silme Politikası** | Soft Delete (`isDeleted` bayrağı) |

---

## Kurulum

Proje tamamen Dockerize edilmiştir; .NET SDK veya PostgreSQL kurulumu gerekmez.

**Docker kurulu değilse:** [docs.docker.com/get-docker](https://docs.docker.com/get-docker/) adresinden işletim sisteminize uygun paketi indirin ve kurulumu tamamlayın.

Ardından depoyu klonlayıp `compose.yaml` dosyasının bulunduğu dizinde aşağıdaki komutu çalıştırın:

```bash
docker compose up -d --build
```

---

## Erişim

Servisler ayağa kalktıktan sonra:

| Kaynak | Adres |
|---|---|
| API Base URL | `http://localhost:8080` |
| Swagger UI | `http://localhost:8080/swagger` |

Tüm uç nokta tanımları, parametre şemaları ve etkileşimli test arayüzü Swagger UI üzerinden sunulmaktadır.

---

## Validasyon Kuralları

| Alan | Kural |
|---|---|
| İHA Model Adı | Zorunlu · maks. 50 karakter |
| IP Adresi | Zorunlu · IPv4 formatı · maks. 15 karakter |
| Uçuş Süresi | 10 – 3000 dakika aralığı |
| Uçuş Kaydı Açıklaması | maks. 500 karakter |
| Sistem Logları | Kritik hatalar `/Logs` dizinine `.txt` olarak yazılır |

---

## Lisans

Eğitim ve standart geliştirme pratiklerini uygulamak amacıyla hazırlanmıştır. Ticari kullanım öncesinde operasyonel güvenlik gereksinimlerinin gözden geçirilmesi önerilir.
