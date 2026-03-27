# Drone Fleet Management API

![.NET 10.0](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white)

> Gelişmiş İHA (Drone) filolarını ve bu araçlara ait uçuş kayıtlarını yönetmek için tasarlanmış, veri bütünlüğünü ve güvenliğini ön planda tutan sunucu taraflı (server-side) bir uygulamadır.

## Projenin Amacı ve Kapsamı

Bu proje, modern backend mimarilerini, yazılım mühendisliği prensiplerini ve güncel endüstri standartlarını öğrenmek/uygulamak amacıyla geliştirilmiştir. 

Uygulama aktif olarak herhangi bir kullanıcı arayüzüne (Frontend) bağlı değildir. Temel odak noktası; güvenli veri transferi, katmanlı mimari yaklaşımı, asenkron programlama ve ilişkisel veritabanı yönetiminin API Controller standartları üzerinden kusursuz bir şekilde kurgulanmasıdır.

## Kullanılan Teknolojiler ve Mimari Standartlar

Proje geliştirilirken aşağıdaki teknolojiler ve endüstriyel standartlar temel alınmıştır:

* **Framework & Dil:** .NET 10.0 SDK, C#
* **Veritabanı & ORM:** PostgreSQL, Entity Framework Core (Code-First)
* **Konteynerleştirme:** Docker, Docker Compose
* **Mimari Yaklaşımlar:**
  * **RESTful Tasarım:** API uç noktaları evrensel isimlendirme ve HTTP metot standartlarına uygun olarak tasarlanmıştır.
  * **DTO (Data Transfer Object) Katmanı:** Veritabanı varlıklarının (Entity) dış dünyaya sızması engellenmiş, istemci ve sunucu arasında sadece gerekli verilerin taşınması sağlanmıştır.
  * **Global Hata Yönetimi (Error Handling):** Uygulama genelinde oluşabilecek istisnai durumlar (exceptions) merkezi bir ara katmanda (middleware) yakalanarak istemciye standartlaştırılmış JSON formatında döndürülmektedir.
  * **Loglama:** Kritik sistem hataları izole edilerek sunucu tarafında fiziksel bir log dosyasına (`.txt` formatında) anlık olarak kaydedilmektedir.
  * **Defense in Depth:** Veri doğrulama işlemleri hem Controller/DTO seviyesinde (Data Annotations) hem de veritabanı seviyesinde katmanlı olarak uygulanmıştır.

## Kurulum ve Çalıştırma

Proje tamamen Dockerize edilmiştir. Uygulamayı ayağa kaldırmak için bilgisayarınızda .NET SDK veya PostgreSQL kurulu olmasına gerek yoktur; yalnızca Docker'ın çalışır durumda olması yeterlidir.

1. Proje dizinini bilgisayarınıza klonlayın.
2. Terminal üzerinden projenin ana dizinine (`compose.yaml` dosyasının bulunduğu konuma) geçiş yapın.
3. Uygulamayı ve veritabanını izole bir ortamda başlatmak için aşağıdaki komutu çalıştırın:

```bash
docker compose up -d --build