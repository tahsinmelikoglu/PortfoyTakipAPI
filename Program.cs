using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using PortfoyTakipAPI.Middlewares;
using PortfoyTakipAPI.Models;
using PortfoyTakipAPI.Repositories;
using PortfoyTakipAPI.Services;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- REDIS CACHE AYARI ---
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; // Docker'daki Redis'in adresi
    options.InstanceName = "PortfoyAPI_"; // Önbellekteki verilerin başına eklenecek etiket
});
// --------------------------------------
// MediatR kütüphanesini projeye dahil ediyoruz
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddSignalR();
builder.Services.AddScoped<IVarlikRepository, VarlikRepository>();
builder.Services.AddScoped<IVarlikService, VarlikService>();
builder.Services.AddHttpClient<IYapayZekaService, YapayZekaService>(client =>
{
    // Llama 3'ün derin düşünmesi için süreyi 180 saniyeye çıkarıyoruz
    client.Timeout = TimeSpan.FromSeconds(180);
});
builder.Services.AddHttpClient<ISemanticSearchService, SemanticSearchService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Portföy Takip API",
        Version = "v1",
        Description = "Kişisel Varlık ve Bütçe Yönetimi Sistemi"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Lütfen oluşturulan JWT Token değerini buraya girin."
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddControllers();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("SabitSinir", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 5;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "text/plain; charset=utf-8";
        await context.HttpContext.Response.WriteAsync("Sakin ol şampiyon! Çok hızlı istek atıyorsun, lütfen 10 saniye bekle.", cancellationToken: token);
    };
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- YENİ EKLENEN ARAYÜZ (FRONTEND) DESTEĞİ ---
app.UseDefaultFiles(); // Varsayılan olarak index.html arar
app.UseStaticFiles();  // wwwroot klasöründeki HTML, CSS, JS dosyalarını dışarıya açar
// ----------------------------------------------

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers().RequireRateLimiting("SabitSinir");
app.MapHub<PortfoyTakipAPI.Hubs.PortfoyHub>("/portfoyhub");
app.Run();