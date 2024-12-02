using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using MultiShop.Images.Services;
using Microsoft.Extensions.Options;
using System;

var builder = WebApplication.CreateBuilder(args);

// Configuration'ý doðru þekilde al
var configuration = builder.Configuration;

// Add authentication and configure JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = configuration["IdentityServerUrl"]; // JWT Authority URL
        options.Audience = "ResourceImage"; // JWT Audience
        options.RequireHttpsMetadata = false; // Güvenli baðlantýyý opsiyonel yap
    });

// Servisler ve yapýlandýrmalar
builder.Services.AddControllers();

// GCSConfigOptions yapýlandýrmasýný ekle
builder.Services.Configure<GCSConfigOptions>(builder.Configuration);
builder.Services.AddSingleton<ICloudStorageService, CloudStorageService>();

// Swagger yapýlandýrmasý
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Uygulama oluþturuluyor
var app = builder.Build();

// Geliþtirme ortamýnda Swagger'ý etkinleþtir
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS yönlendirmesi ve yetkilendirme
app.UseHttpsRedirection();
app.UseAuthentication(); // JWT Authentication ekliyoruz
app.UseAuthorization(); // Yetkilendirme middleware'ý

// Controller'larý haritalama
app.MapControllers();

// Uygulama çalýþtýrýlýyor
app.Run();
