# Online Muhasebe Server - ORM Kullanımı Detaylı Rehberi

Bu dokümantasyon, Online Muhasebe Server projesinde **Entity Framework Core** ORM'inin nasıl kullanıldığını detaylı bir şekilde açıklamaktadır.

## İçindekiler

1. [ORM Mimarisi Genel Bakış](#orm-mimarisi-genel-bakış)
2. [DbContext Yapıları](#dbcontext-yapıları)
3. [Entity Tanımlamaları](#entity-tanımlamaları)
4. [Entity Configurations](#entity-configurations)
5. [Migration Yönetimi](#migration-yönetimi)
6. [Multi-Tenant Yapısı](#multi-tenant-yapısı)
7. [Kod Örnekleri](#kod-örnekleri)
8. [En İyi Uygulamalar](#en-iyi-uygulamalar)

## ORM Mimarisi Genel Bakış

Bu proje **Entity Framework Core** kullanarak **Code-First** yaklaşımını benimser. Proje, **Clean Architecture** prensiplerine uygun olarak katmanlı bir yapıya sahiptir:

### Katman Yapısı
```
OnlineMuhasebeServer.Domain/          # Domain katmanı (Entities, Constants)
OnlineMuhasebeserver.Persistance/     # Persistance katmanı (DbContext, Configurations)
OnlineMuhasebeServer.Application/     # Application katmanı (Business Logic)
OnlineMuhasebeServer.Infrastructure/  # Infrastructure katmanı
OnlineMuhasebeServer.Presentation/    # Presentation katmanı (Controllers)
OnlineMuhasebeServer.WebApi/          # Web API katmanı
```

### Kullanılan Teknolojiler
- **.NET 8.0**
- **Entity Framework Core 9.0.5**
- **SQL Server**
- **ASP.NET Core Identity** (kimlik doğrulama için)

## DbContext Yapıları

Projede iki farklı DbContext kullanılmaktadır:

### 1. AppDbContext (Ana Uygulama Veritabanı)

```csharp
public sealed class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Company> Companies { get; set; }
    public DbSet<UserAndComanyRelationship> UserAndComanyRelationships { get; set; }
}
```

**Amaç:** 
- Kullanıcı kimlik bilgileri
- Şirket bilgileri
- Kullanıcı-şirket ilişkileri

**Özellikler:**
- ASP.NET Core Identity entegrasyonu
- Merkezi kullanıcı yönetimi
- Şirket kayıtları

### 2. CompanyDbContext (Şirket Bazlı Veritabanları)

```csharp
public sealed class CompanyDbContext : DbContext
{
    private string ConnectionString = "";
    
    public CompanyDbContext(string companyId, Company company = null)
    {
        // Şirkete özel connection string oluşturma
        if(company != null)
        {
            if (company.UserId == "")
                ConnectionString = $"Data Source={company.ServerName};" +
                    $"Initial Catalog={company.DatabaseName};" +
                    $"Integrated Security=True;...";
            else
                ConnectionString = $"Data Source={company.ServerName};" +
                    $"Initial Catalog={company.DatabaseName};" +
                    $"User Id={company.UserId};" +
                    $"Password={company.Password}...";
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);
}
```

**Amaç:**
- Her şirket için ayrı veritabanı
- Şirkete özel muhasebe kayıtları
- İzolasyon ve veri güvenliği

## Entity Tanımlamaları

### Base Entity (Temel Varlık)

```csharp
public abstract class Entities
{
    public string Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }   
}
```

**Özellikler:**
- Tüm entity'ler bu sınıftan türer
- Ortak kimlik ve zaman damgası alanları
- Soft delete desteği için hazır yapı

### Ana Uygulama Entity'leri (App Entities)

#### 1. Company (Şirket)
```csharp
public sealed class Company : Entities
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string IdentityNumber { get; set; }
    public string TaxDepartment { get; set; }
    public string Tel { get; set; }
    public string Email { get; set; }
    public string ServerName { get; set; }      // Veritabanı sunucu adı
    public string DatabaseName { get; set; }    // Şirkete özel DB adı
    public string UserId { get; set; }          // DB kullanıcı adı
    public string Password { get; set; }        // DB şifresi
}
```

#### 2. AppUser (Uygulama Kullanıcısı)
```csharp
public sealed class AppUser : IdentityUser<string>
{
    // ASP.NET Core Identity'den miras alır
    // Ek özellikler buraya eklenebilir
}
```

#### 3. UserAndCompanyRelationship (Kullanıcı-Şirket İlişkisi)
```csharp
public class UserAndComanyRelationship : Entities
{
    [ForeignKey("AppUser")]
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }

    [ForeignKey("Company")]
    public string CompanyId { get; set; }
    public Company Company { get; set; }
}
```

### Şirket Entity'leri (Company Entities)

#### UniformChartOfAccount (Tekdüzen Hesap Planı)
```csharp
public sealed class UniformChartOfAccount : Entities
{
    public string Code { get; set; }            // Hesap kodu
    public string Name { get; set; }            // Hesap adı
    public char Type { get; set; }              // A: Asset, L: Liability, E: Equity, R: Revenue, C: Cost, I: Income
    public string CompanyId { get; set; }       // Şirket kimliği
}
```

## Entity Configurations

Entity'lerin veritabanı tablolarına nasıl mapping edileceği **Fluent API** kullanılarak yapılandırılır:

### UCAFConfiguration (Tekdüzen Hesap Planı Konfigürasyonu)
```csharp
public sealed class UCAFConfiguration : IEntityTypeConfiguration<UniformChartOfAccount>
{
    public void Configure(EntityTypeBuilder<UniformChartOfAccount> builder)
    {
        builder.ToTable(TableNames.UniformChartOfAccounts);
        builder.HasKey(p => p.Id);
        
        // Ek konfigürasyonlar buraya eklenebilir:
        // builder.Property(p => p.Code).HasMaxLength(50).IsRequired();
        // builder.Property(p => p.Name).HasMaxLength(255).IsRequired();
        // builder.HasIndex(p => p.Code).IsUniqueek();
    }
}
```

### Constants (Sabitler)
```csharp
public static class TableNames
{
    public static string UniformChartOfAccounts = nameof(UniformChartOfAccount);
    // Yeni tablo adları buraya eklenebilir
}
```

## Migration Yönetimi

### Migration Oluşturma
```bash
# Ana uygulama veritabanı için
dotnet ef migrations add MigrationName --context AppDbContext

# Şirket veritabanı için  
dotnet ef migrations add MigrationName --context CompanyDbContext --output-dir Migrations/CompanyDb
```

### Database Update
```bash
# Ana uygulama veritabanı
dotnet ef database update --context AppDbContext

# Şirket veritabanı
dotnet ef database update --context CompanyDbContext
```

### Mevcut Migration'lar
1. **20250530164551_database_oluşturma**: Ana uygulama veritabanı oluşturma
2. **20250530191055_company_database_oluşturma**: Şirket veritabanı oluşturma

## Multi-Tenant Yapısı

Bu proje **Database-per-Tenant** yaklaşımını kullanır:

### Avantajları
1. **Veri İzolasyonu**: Her şirketin verileri tamamen ayrı
2. **Performans**: Şirkete özel optimizasyonlar
3. **Güvenlik**: Bir şirketin verisi diğerine karışmaz
4. **Ölçeklenebilirlik**: Büyük şirketler için ayrı sunucularda barındırma

### Nasıl Çalışır?
1. Kullanıcı giriş yapar (AppDbContext)
2. Kullanıcının erişebileceği şirketler listelenir
3. Şirket seçimi yapılır
4. Seçilen şirkete özel CompanyDbContext oluşturulur
5. Tüm muhasebe işlemleri şirkete özel veritabanında yapılır

## Kod Örnekleri

### AppDbContext Dependency Injection Kaydı
```csharp
// Program.cs
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
```

### CompanyDbContext Kullanımı
```csharp
// Şirkete özel context oluşturma
var company = await appDbContext.Companies.FindAsync(companyId);
using var companyContext = new CompanyDbContext(companyId, company);

// Tekdüzen hesap planı işlemleri
var accounts = await companyContext.Set<UniformChartOfAccount>()
    .Where(x => x.CompanyId == companyId)
    .ToListAsync();
```

### Entity Ekleme Örneği
```csharp
// Yeni şirket ekleme
var company = new Company
{
    Id = Guid.NewGuid().ToString(),
    Name = "Örnek Şirket A.Ş.",
    Address = "İstanbul",
    IdentityNumber = "1234567890",
    TaxDepartment = "Kadıköy",
    Tel = "0216 123 45 67",
    Email = "info@ornek.com",
    ServerName = "(localdb)\\MSSQLLocalDB",
    DatabaseName = "OrnekSirket_DB",
    UserId = "",
    Password = "",
    CreatedDate = DateTime.Now
};

appDbContext.Companies.Add(company);
await appDbContext.SaveChangesAsync();
```

### Entity Güncelleme Örneği
```csharp
// Hesap planı güncelleme
var account = await companyContext.Set<UniformChartOfAccount>()
    .FirstOrDefaultAsync(x => x.Id == accountId);

if (account != null)
{
    account.Name = "Güncellenmiş Hesap Adı";
    account.UpdatedDate = DateTime.Now;
    
    await companyContext.SaveChangesAsync();
}
```

## En İyi Uygulamalar

### 1. Connection String Yönetimi
- Hassas bilgileri (şifreler) güvenli yerlerde saklayın
- Connection string'leri configuration dosyalarından okuyun
- Production ortamında güvenli connection yöntemleri kullanın

### 2. Migration Yönetimi
- Her değişiklik için anlamlı migration adları kullanın
- Production'da migration'ları dikkatli çalıştırın
- Migration'ları version control'de takip edin

### 3. Performance Optimizasyonu
- Lazy loading'i dikkatli kullanın
- Include() ile ilişkili verileri önceden yükleyin
- Büyük veri setleri için pagination kullanın
- Index'leri doğru şekilde tanımlayın

### 4. Güvenlik
- SQL Injection'a karşı parametreli sorgular kullanın
- Connection string'lerde minimum gerekli yetkileri verin
- Şirket verilerini izole edin

### 5. Kod Organizasyonu
- Entity'leri ilgili klasörlerde gruplandırın
- Configuration'ları ayrı dosyalarda tutun
- Constants'ları merkezi bir yerde toplayın

## Sonuç

Bu ORM implementasyonu, muhasebe yazılımı için gerekli olan:
- **Multi-tenancy** desteği
- **Veri izolasyonu**  
- **Ölçeklenebilirlik**
- **Güvenlik**

özelliklerını sağlamaktadır. Entity Framework Core'un sunduğu güçlü özellikler ile birlikte, profesyonel bir muhasebe uygulaması için solid bir temel oluşturmaktadır.