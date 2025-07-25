# Online Muhasebe Server

Modern muhasebe yazılımı için geliştirilmiş .NET 8.0 tabanlı RESTful API uygulaması.

## Proje Özeti

Bu proje, **Clean Architecture** prensiplerine uygun olarak geliştirilmiş, **Entity Framework Core** kullanarak **multi-tenant** yapıyı destekleyen bir online muhasebe sistemidir.

## Teknolojiler

- **.NET 8.0**
- **Entity Framework Core 9.0.5**
- **ASP.NET Core Identity**
- **SQL Server**
- **Swagger/OpenAPI**

## Proje Yapısı

```
OnlineMuhasebeServer.Domain/          # Domain katmanı (Entities, Constants)
OnlineMuhasebeserver.Persistance/     # Persistance katmanı (DbContext, Configurations)  
OnlineMuhasebeServer.Application/     # Application katmanı (Business Logic)
OnlineMuhasebeServer.Infrastructure/  # Infrastructure katmanı
OnlineMuhasebeServer.Presentation/    # Presentation katmanı (Controllers)
OnlineMuhasebeServer.WebApi/          # Web API katmanı
```

## Özellikler

### Multi-Tenant Mimari
- Her şirket için ayrı veritabanı
- Güvenli veri izolasyonu
- Ölçeklenebilir yapı

### Kimlik Doğrulama
- ASP.NET Core Identity entegrasyonu
- JWT Token tabanlı authentication
- Kullanıcı-şirket ilişki yönetimi

### ORM ve Veritabanı
- Entity Framework Core Code-First yaklaşımı
- Migration yönetimi
- İki ayrı DbContext (App ve Company)

## ORM Kullanımı

Bu projede ORM'in nasıl kullanıldığına dair detaylı bilgi için:

📖 **[ORM Kullanımı Detaylı Rehberi](./ORM-KULLANIMI.md)**

### ORM Özellikleri
- **Dual DbContext Yapısı**: Ana uygulama ve şirket bazlı veritabanları
- **Code-First Migrations**: Otomatik veritabanı şema yönetimi  
- **Entity Configurations**: Fluent API ile esnek yapılandırma
- **Base Entity Pattern**: Ortak özellikler için inheritance
- **Multi-Database Support**: Her şirket için ayrı veritabanı

## Kurulum

### Gereksinimler
- .NET 8.0 SDK
- SQL Server (LocalDB veya tam sürüm)
- Visual Studio 2022 veya VS Code

### Adımlar

1. **Repository'yi klonlayın:**
```bash
git clone https://github.com/Gorkem5/Online-Accounting-Server.git
cd Online-Accounting-Server
```

2. **Bağımlılıkları yükleyin:**
```bash
dotnet restore
```

3. **Veritabanı connection string'ini yapılandırın:**
`OnlineMuhasebeServer.WebApi/appsettings.json` dosyasındaki connection string'i güncelleyin.

4. **Migration'ları çalıştırın:**
```bash
# Ana uygulama veritabanı
dotnet ef database update --context AppDbContext --project OnlineMuhasebeserver.Persistance --startup-project OnlineMuhasebeServer.WebApi

# Şirket veritabanı template'i  
dotnet ef database update --context CompanyDbContext --project OnlineMuhasebeserver.Persistance --startup-project OnlineMuhasebeServer.WebApi
```

5. **Uygulamayı çalıştırın:**
```bash
dotnet run --project OnlineMuhasebeServer.WebApi
```

6. **Swagger UI'ya erişin:**
Tarayıcınızda `https://localhost:7xxx/swagger` adresine gidin.

## API Endpoints

### Ana Endpoints
- `GET /api/demo` - Demo endpoint (test için)

### Planlanan Endpoints
- `/api/auth` - Kimlik doğrulama
- `/api/companies` - Şirket yönetimi  
- `/api/accounts` - Hesap planı yönetimi
- `/api/transactions` - Muhasebe işlemleri

## Geliştirme

### Yeni Entity Ekleme
1. `Domain/CompanyEntities` veya `Domain/AppEntities` klasörüne entity sınıfını ekleyin
2. Gerekirse `Domain/Constans/TableNames.cs` dosyasını güncelleyin
3. `Persistance/Configurations` klasörüne configuration sınıfını ekleyin
4. Migration oluşturun ve çalıştırın

### Migration Oluşturma
```bash
# Ana uygulama için
dotnet ef migrations add MigrationName --context AppDbContext --project OnlineMuhasebeserver.Persistance --startup-project OnlineMuhasebeServer.WebApi

# Şirket veritabanı için
dotnet ef migrations add MigrationName --context CompanyDbContext --project OnlineMuhasebeserver.Persistance --startup-project OnlineMuhasebeServer.WebApi --output-dir Migrations/CompanyDb
```

## Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## İletişim

Proje sahibi: Görkem
- GitHub: [@Gorkem5](https://github.com/Gorkem5)

---

## Dökümantasyon

- 📖 [ORM Kullanımı Detaylı Rehberi](./ORM-KULLANIMI.md) - Entity Framework Core implementasyonu
- 🏗️ [Mimari Rehberi](./ARCHITECTURE.md) - Clean Architecture implementation (yakında)
- 🔐 [Security Rehberi](./SECURITY.md) - Güvenlik best practices (yakında)