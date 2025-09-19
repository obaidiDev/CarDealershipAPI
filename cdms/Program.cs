using cdms.Data;
using cdms.Middleware;
using cdms.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------
// Configuration & DbContext
// ------------------------------
builder.Services.AddDbContext<CarDealershipDbContext>(options =>
    options.UseInMemoryDatabase("CarDealershipDB")); // Use SQL Server or Postgres for production

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ------------------------------
// Services
// ------------------------------
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CarService>();
builder.Services.AddScoped<PurchaseService>();
builder.Services.AddSingleton<OtpService>();

// ------------------------------
// JWT Authentication
// ------------------------------
var key = builder.Configuration["Jwt:Key"] ?? "THIS_IS_A_DEMO_SECRET";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

builder.Services.AddAuthorization();

// ------------------------------
// Logging
// ------------------------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// ------------------------------
// Middleware
// ------------------------------
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ------------------------------
// Seed initial data
// ------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CarDealershipDbContext>();
    var userService = scope.ServiceProvider.GetRequiredService<UserService>();
    var carService = scope.ServiceProvider.GetRequiredService<CarService>();

    // Add admin user
    if (!db.Users.Any(u => u.Email == "admin@dealer.com"))
    {
        var admin = new cdms.Models.User
        {
            Email = "admin@dealer.com",
            PasswordHash = UserService.HashPassword("Admin123!"),
            Role = "Admin",
            FullName = "System Admin",
            IsActive = true
        };
        db.Users.Add(admin);
        db.SaveChanges();
    }

    // Seed 10 sample cars
    if (!db.Cars.Any())
    {
        for (int i = 1; i <= 10; i++)
        {
            var car = new cdms.Models.Car
            {
                Make = "Make" + i,
                Model = "Model" + i,
                Year = 2015 + (i % 8),
                TrimLevel = "Trim" + i,
                MarketRegion = "Region" + i,
                PricePerDay = 50 + i * 10,
                Mileage = 10000 + i * 2000,
                TransmissionType = i % 2 == 0 ? "Automatic" : "Manual",
                FuelType = i % 2 == 0 ? "Gasoline" : "Diesel",
                NumberOfSeats = 5,
                Color = i % 2 == 0 ? "Red" : "Blue",
                IsAvailable = true,
                Description = $"Car {i} description",
                Features = new[] { "GPS", "Bluetooth" },
                ImageUrl = $"https://picsum.photos/200/300?random={i}"
            };
            db.Cars.Add(car);
        }
        db.SaveChanges();
    }
}

app.Run();
