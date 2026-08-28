# 🚀 PortfoyTakipAPI - Akıllı Portföy Yönetim ve Asistan Sistemi

Bu proje, Yıldız Teknik Üniversitesi staj programı kapsamında geliştirilmiş; modern yazılım mimarilerini, gerçek zamanlı veri akışını ve yapay zeka entegrasyonunu barındıran kurumsal düzeyde bir .NET Web API uygulamasıdır.

Staj programı final kriteri olan *"Token-Based Auth, kullanıcı yetkilendirmesi ve LLM motoruyla iletişim"*[cite: 1] şartını, endüstri standardı mimari desenlerle birleştirerek hayata geçirilmiştir.

---

## 🛠️ Kullanılan Teknolojiler ve Mimari

* **Framework:** .NET Web API
* **Veritabanı & ORM:** MSSQL, Entity Framework Core (Global Conventions & Migrations)
* **Mimari Desenler:** Katmanlı Mimari (Repository Pattern), **CQRS (Command/Query Responsibility Segregation)** & **MediatR**
* **Gerçek Zamanlı İletişim:** **SignalR** (WebSockets tabanlı anlık bildirimler)
* **Güvenlik:** JWT (JSON Web Token) tabanlı Kimlik Doğrulama ve Rol Bazlı Yetkilendirme (`[Authorize]`)
* **Yapay Zeka / LLM:** **Ollama & Llama3** (Yerel sunucuda çalışan LLM motoru ile portföy analizi ve akıllı asistan)

---

## ✨ Öne Çıkan Özellikler

1. **CQRS & MediatR Entegrasyonu:** 
   * Okuma (Query) ve yazma (Command) operasyonları birbirinden ayrılarak Controller katmanı şişmekten kurtarılmış, yüksek performanslı ve modüler bir akış sağlanmıştır.
2. **Gerçek Zamanlı Canlı Bildirimler (SignalR):** 
   * Sisteme yeni bir varlık/hisse eklendiği milisaniye, tarayıcılar sayfayı hiç yenilemeden (F5 yapmadan) `PortfoyHub` üzerinden anlık roket bildirimleri alır.
3. **Akıllı Portföy Asistanı (Local LLM - Llama3):** 
   * Ollama entegrasyonu sayesinde veritabanındaki varlıklar yerel LLM motoruna beslenir; kullanıcılar portföyleri hakkında yapay zekadan anlık risk ve büyüme analizleri alabilir.
4. **Jilet Gibi Konsol ve Hata Yönetimi:** 
   * Global konfigürasyonlarla ondalıklı sayı hassasiyetleri (`precision`) sabitlenmiş, log kirlilikleri ve uyarılar sıfırlanmıştır.

---

## 📂 Proje Mimarisi (Katmanlar)

```text
PortfoyTakipAPI/
│
├── Controllers/         # API uç noktaları (Varliklar, Auth vb.)
├── CQRS/                # Command, Query ve bunlara ait Handler sınıfları
│   ├── Commands/        # Yazma operasyonları (Create, Update, Delete)
│   └── Queries/         # Okuma operasyonları ve sayfalama mantığı
├── Hubs/                # SignalR gerçek zamanlı iletişim merkezi (PortfoyHub)
├── Models/              # Veritabanı Entity sınıfları ve AppDbContext
├── Repositories/        # Veri erişim katmanı (Repository Pattern & Interface)
├── DTOs/                # Veri transfer nesneleri ve istek parametreleri
└── wwwroot/             # Statik test arayüzleri (canli-borsa.html vb.)