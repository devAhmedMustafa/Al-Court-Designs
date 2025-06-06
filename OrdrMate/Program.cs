using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using OrdrMate.Data;
using OrdrMate.Middlewares;
using OrdrMate.Repositories;
using OrdrMate.Services;
using OrdrMate.Managers;
using OrdrMate.Sockets;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrdrMateDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS
builder.Services.AddCors(options =>
{
    // Allow all origins, methods, and headers
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        if (env.IsDevelopment())
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
        else if (env.IsProduction())
        {
            builder.WithOrigins("https://ordrmate-manager.vercel.app", "https://ordrmate-manager.vercel.app")
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    });
});

// Repositories and Services

builder.Services.AddScoped<IUserRepo, ManagerRepo>();
builder.Services.AddScoped<ManagerService, ManagerService>();

builder.Services.AddScoped<IRestaurantRepo, RestaurantRepo>();
builder.Services.AddScoped<RestaurantService, RestaurantService>();

builder.Services.AddScoped<IItemRepo, ItemRepo>();
builder.Services.AddScoped<ItemService, ItemService>();

builder.Services.AddScoped<IBranchRepo, BranchRepo>();
builder.Services.AddScoped<BranchService, BranchService>();

builder.Services.AddScoped<IBranchRequestRepo, BranchRequestRepo>();

builder.Services.AddScoped<ITableRepo, TableRepo>();
builder.Services.AddScoped<TableService, TableService>();

builder.Services.AddScoped<IKitchenRepo, KitchenRepo>();
builder.Services.AddScoped<KitchenService, KitchenService>();

builder.Services.AddScoped<CustomerService, CustomerService>();

builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<OrderService, OrderService>();

builder.Services.AddScoped<IPaymentRepo, PaymentRepo>();
builder.Services.AddScoped<PaymentService, PaymentService>();

builder.Services.AddScoped<CloudMessaging>();

// AWS S3 Configuration
builder.Services.AddScoped<S3Service>();

// Sockets
builder.Services.AddScoped<BranchOrdersSocketHandler>();
builder.Services.AddScoped<CustomerOrdersSocketHandler>();

// Managers
builder.Services.AddScoped<OrderManager>();

builder.Services.AddControllers();

// JWT Authentication

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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),
            RoleClaimType = ClaimTypes.Role,
        };
    });

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageRestaurant", policy =>
        policy.Requirements.Add(new ManageRestaurantRequirement()));

    options.AddPolicy("Admin", policy =>
        policy.Requirements.Add(new AdminRequirement()));

    options.AddPolicy("BranchManager", policy =>
        policy.Requirements.Add(new BranchManagerRequirement()));
});

// Authorization Handlers
builder.Services.AddScoped<IAuthorizationHandler, ManageRestaurantHandler>();
builder.Services.AddScoped<IAuthorizationHandler, AdminHandler>();
builder.Services.AddScoped<IAuthorizationHandler, BranchManagerHandler>();

FirebaseAdmin.FirebaseApp.Create(new FirebaseAdmin.AppOptions()
{
    Credential = GoogleCredential.FromFile("Keys/firebase-adminsdk.json"),
});

var app = builder.Build();
app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();

// Find the uploads folder in the current directory and serve static files from it

var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads",
});

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

