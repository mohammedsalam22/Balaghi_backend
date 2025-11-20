using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Services;
using Application.Validators;
using FluentValidation;
using Application.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.OpenApi.Models;
using Infrastructure.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on any IP
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // HTTP endpoint on port 5000 (or any port you prefer)
    serverOptions.ListenAnyIP(5000);
    
    // HTTPS endpoint on port 5001
    serverOptions.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
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
            Array.Empty<string>()
        }
    });
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("Infrastructure")));
builder.Services.AddScoped<IUserDao, UserDao>();
builder.Services.AddScoped<IRoleDao, RoleDao>();
builder.Services.AddScoped<IRefreshTokenDao, RefreshTokenDao>();
builder.Services.AddScoped<IPasswordResetDao, PasswordResetDao>();
builder.Services.AddScoped<IPermissionService, AuthorizationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<RegisterUserService>();
builder.Services.AddScoped<VerifyOtpService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddScoped<LogoutService>();
builder.Services.AddScoped<ForgotPasswordService>();
builder.Services.AddScoped<ResetPasswordService>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
builder.Services.AddScoped<IValidator<VerifyOtpRequest>, VerifyOtpRequestValidator>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));


builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
    options.AddPolicy("LoginPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.OnRejected = (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        return new ValueTask(context.HttpContext.Response.WriteAsync("Too many requests try again later", cancellationToken));
    };
});
var app = builder.Build();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.SeedAsync(context, scope.ServiceProvider.GetRequiredService<IPasswordHasher>());
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
