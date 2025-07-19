using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Talabeyah.OrderManagement.Infrastructure.Persistence;
using Talabeyah.OrderManagement.Domain.Entities;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Configure the application to listen on port 80 when running in a container
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
{
    builder.WebHost.UseUrls("http://+:80");
}

// Add services to the container.

builder.Services.AddControllers();

// Register FluentValidation validators
builder.Services.AddValidatorsFromAssembly(typeof(Talabeyah.OrderManagement.Application.Orders.Commands.PlaceOrderCommandValidator).Assembly);

// Add SignalR services
builder.Services.AddSignalR();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Talabeyah.OrderManagement.Application.Products.Queries.GetProductsQuery).Assembly));

// Register services
builder.Services.AddScoped<Talabeyah.OrderManagement.Domain.Interfaces.INotificationService, Talabeyah.OrderManagement.Infrastructure.Notifications.OrderNotificationService>();
builder.Services.AddScoped<Talabeyah.OrderManagement.Domain.Interfaces.ISignalRService, Talabeyah.OrderManagement.Infrastructure.Notifications.SignalRService>();

// Register IHubContext for SignalR
builder.Services.AddScoped<Talabeyah.OrderManagement.Domain.Interfaces.IHubContext>(provider =>
{
    var hubContext = provider.GetRequiredService<Microsoft.AspNetCore.SignalR.IHubContext<Talabeyah.OrderManagement.Contracts.OrderHub>>();
    return new Talabeyah.OrderManagement.Contracts.SignalRHubContextAdapter(hubContext);
});

// Register domain services
builder.Services.AddScoped<Talabeyah.OrderManagement.Domain.Services.OrderDomainService>();
builder.Services.AddScoped<Talabeyah.OrderManagement.Domain.Services.ProductDomainService>();

// Register Kafka topic initializer
builder.Services.AddScoped<Talabeyah.OrderManagement.Infrastructure.Services.KafkaTopicInitializer>();

// Add DbContext and Identity
builder.Services.AddDbContext<OrderManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<OrderManagementDbContext>()
    .AddDefaultTokenProviders();

// Register UnitOfWork
builder.Services.AddScoped<Talabeyah.OrderManagement.Domain.Interfaces.IUnitOfWork, Talabeyah.OrderManagement.Infrastructure.Persistence.UnitOfWork>();

// Add SignInManager
builder.Services.AddScoped<SignInManager<ApplicationUser>>();

// Register UserAuthenticator and JwtTokenGenerator
builder.Services.AddScoped<Talabeyah.OrderManagement.Domain.Interfaces.IUserAuthenticator, Talabeyah.OrderManagement.Infrastructure.Services.UserAuthenticator>();
builder.Services.AddScoped<Talabeyah.OrderManagement.Application.Users.Commands.IJwtTokenGenerator, Talabeyah.OrderManagement.Infrastructure.Services.JwtTokenGenerator>();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Talabeyah.OrderManagement.Application.Contracts.IUserContextAccessor, Talabeyah.OrderManagement.Infrastructure.Services.UserContextAccessor>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
    
    // Configure JWT for SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/orderHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Management API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Initialize Kafka topics and seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // Initialize Kafka topics first
        var kafkaInitializer = services.GetRequiredService<Talabeyah.OrderManagement.Infrastructure.Services.KafkaTopicInitializer>();
        await kafkaInitializer.InitializeTopicsAsync();
        logger.LogInformation("Kafka topics initialized successfully");
        
        // Wait a bit for Kafka to be fully ready
        await Task.Delay(2000);
        
        // Seed database
        var db = services.GetRequiredService<OrderManagementDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        await DbInitializer.SeedAsync(db, userManager, roleManager);
        logger.LogInformation("Database seeded successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during startup initialization");
        throw;
    }
}

// Configure the HTTP request pipeline.
// Enable Swagger for all environments for testing
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Place this BEFORE UseAuthentication and UseAuthorization
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseMiddleware<Talabeyah.OrderManagement.API.Middleware.UserContextMiddleware>();
app.UseAuthorization();

// Register IdempotencyMiddleware before controllers
app.UseMiddleware<Talabeyah.OrderManagement.API.IdempotencyMiddleware>();

app.MapControllers();
app.MapHub<Talabeyah.OrderManagement.API.Hubs.OrderHub>("/orderHub");

app.Run();
