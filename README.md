# 🚁 Drone Fleet Management API

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white)

Gelişmiş İHA (Drone) filolarını ve uçuş loglarını yönetmek için geliştirilmiş, yüksek performanslı ve tam teşekküllü **RESTful API** projesi. 

Sistem; otonom uçuş verilerini, donanım konfigürasyonlarını (5.1 GHz - 5.9 GHz haberleşme modülleri vb.) ve anlık uçuş kayıtlarını güvenli bir şekilde merkezi veritabanında saklamak üzere tasarlanmıştır.

## ✨ Temel Özellikler (Features)

- **RESTful Mimari:** Evrensel standartlara uygun Controller yapısı ve endpoint tasarımı.
- **DTO (Data Transfer Object) Katmanı:** İstemci ile veritabanı arasında güvenli veri transferi. Veri sızıntılarını (Entity leakage) engeller.
- **Global Error Handling & Logging:** Beklenmeyen sistem hatalarını havada yakalar, API'nin çökmesini engeller ve tüm kriz anlarını fiziksel `.txt` formatında bir "Kara Kutu" (Log) dosyasına kaydeder.
- **Eager Loading (Include):** 1-N (Bire-Çok) ilişkili tabloları (İHA ve Uçuş Kayıtları) yüksek performanslı LINQ sorgularıyla tek seferde getirir.
- **Defense in Depth:** Hem DataAnnotations (Filtreleme) hem de Veritabanı (EF Core) seviyesinde katmanlı veri doğrulama kilitleri.

## 🛠️ Kullanılan Teknolojiler

- **Backend:** C# & ASP.NET Core Web API (Controllers)
- **ORM:** Entity Framework Core (Code-First Approach)
- **Veritabanı:** PostgreSQL
- **Konteynerleştirme:** Docker & Docker Compose

## 🚀 Kurulum & Çalıştırma (Getting Started)

Proje tamamen Dockerize edilmiştir. Bilgisayarınızda .NET SDK veya PostgreSQL kurulu olmasına gerek yoktur.

1. Projeyi bilgisayarınıza klonlayın.
2. Terminali açarak projenin ana dizinine ( `compose.yaml` dosyasının olduğu yere) gidin.
3. Aşağıdaki sihirli komutu çalıştırın:

```bash
docker compose up -d --build