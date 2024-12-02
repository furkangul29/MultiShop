using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using MultiShop.Images.Services;
using Microsoft.Extensions.Options;
using System;

var builder = WebApplication.CreateBuilder(args);

// Configuration'� do�ru �ekilde al
var configuration = builder.Configuration;

// Add authentication and configure JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = configuration["IdentityServerUrl"]; // JWT Authority URL
        options.Audience = "ResourceImage"; // JWT Audience
        options.RequireHttpsMetadata = false; // G�venli ba�lant�y� opsiyonel yap
    });

// Servisler ve yap�land�rmalar
builder.Services.AddControllers();

// GCSConfigOptions yap�land�rmas�n� ekle
builder.Services.Configure<GCSConfigOptions>(builder.Configuration);
builder.Services.AddSingleton<ICloudStorageService, CloudStorageService>();

// Swagger yap�land�rmas�
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Uygulama olu�turuluyor
var app = builder.Build();

// Geli�tirme ortam�nda Swagger'� etkinle�tir
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS y�nlendirmesi ve yetkilendirme
app.UseHttpsRedirection();
app.UseAuthentication(); // JWT Authentication ekliyoruz
app.UseAuthorization(); // Yetkilendirme middleware'�

// Controller'lar� haritalama
app.MapControllers();

// Uygulama �al��t�r�l�yor
app.Run();
