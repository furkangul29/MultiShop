using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MultiShop.Catalog.BackgroundServices;
using MultiShop.Catalog.Entites;
using MultiShop.Catalog.Services.AboutServices;
using MultiShop.Catalog.Services.BrandServices;
using MultiShop.Catalog.Services.CategoryServices;
using MultiShop.Catalog.Services.ContactServices;
using MultiShop.Catalog.Services.DealsOfDayServices;
using MultiShop.Catalog.Services.FavoriteProductServices;
using MultiShop.Catalog.Services.FeatureServices;
using MultiShop.Catalog.Services.FeatureSliderServices;
using MultiShop.Catalog.Services.HourlyDealServices;
using MultiShop.Catalog.Services.OfferDiscountServices;
using MultiShop.Catalog.Services.ProductDetailDetailServices;
using MultiShop.Catalog.Services.ProductImageServices;
using MultiShop.Catalog.Services.ProductServices;
using MultiShop.Catalog.Services.SpecialOfferServices;
using MultiShop.Catalog.Services.StatisticServices;
using MultiShop.Catalog.Settings;
using System.Reflection;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.Authority = builder.Configuration["IdentityServerUrl"];
    opt.Audience = "ResourceCatalog";
    opt.RequireHttpsMetadata = false;
});
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    var database = client.GetDatabase(settings.DatabaseName);
    return database.GetCollection<Product>("Products");
});
builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    var database = client.GetDatabase(settings.DatabaseName);
    return database.GetCollection<Category>("Categories");
});
builder.Services.AddHttpClient("CommentAPI", c =>
{
    c.BaseAddress = new Uri("https://localhost:7005");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
});


builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    var database = client.GetDatabase(settings.DatabaseName);
    return database.GetCollection<HourlyDeal>("HourlyDeals");
});

builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    return client.GetDatabase(settings.DatabaseName);
});
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    var database = client.GetDatabase(settings.DatabaseName);
    return database.GetCollection<DealsOfDay>("DealsOfDay");
});

builder.Services.AddHostedService<DealsExpirationService>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CatalogFullPermission", policy =>
        policy.RequireClaim("scope", "CatalogFullPermission"));
});
builder.Services.AddScoped<IStatisticService, StatisticService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductDetailService, ProductDetailService>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<IFeatureSliderService, FeatureSliderService>();
builder.Services.AddScoped<ISpecialOfferService, SpecialOfferService>();
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<IOfferDiscountService, OfferDiscountService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IAboutService, AboutService>();
builder.Services.AddScoped<IDealsOfDayService, DealsOfDayService>();

builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IFavoriteProductService, FavoriteProductService>();
builder.Services.AddScoped<IHourlyDealService, HourlyDealService>();

builder.Services.AddHostedService<HourlyDealBackgroundService>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddScoped<IDatabaseSettings>(sp =>
{
    return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
});
builder.Services.AddHttpClient();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
