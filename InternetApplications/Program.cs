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
using Infrastructure.Data;
using Application.UseCases.Auth;
using InternetApplications.Abstractions;
using InternetApplications.Services;
using System.Text.Json.Serialization;   
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000);
    serverOptions.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps());
});
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My App", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter Token: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Infrastructure")));
builder.Services.AddScoped<IUserDao, UserDao>();
builder.Services.AddScoped<IRoleDao, RoleDao>();
builder.Services.AddScoped<IRefreshTokenDao, RefreshTokenDao>();
builder.Services.AddScoped<IPasswordResetDao, PasswordResetDao>();
builder.Services.AddScoped<IGovernmentAgencyDao, GovernmentAgencyDao>();
builder.Services.AddScoped<IComplaintDao, ComplaintDao>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<RegisterUserService>();
builder.Services.AddScoped<VerifyOtpService>();
builder.Services.AddScoped<GovernorateAgencyService>();
builder.Services.AddScoped<LogoutService>();
builder.Services.AddScoped<ForgotPasswordService>();
builder.Services.AddScoped<ResetPasswordService>();
builder.Services.AddScoped<IPermissionService, AuthorizationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
builder.Services.AddScoped<IValidator<VerifyOtpRequest>, VerifyOtpRequestValidator>();
builder.Services.AddScoped<InviteEmployeeService>();
builder.Services.AddScoped<CompleteEmployeeSetupService>();
builder.Services.AddScoped<DeleteEmployeeService>();
builder.Services.AddScoped<SubmitComplaintService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddAuthentication("Bearer")
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
        context.HttpContext.Response.ContentType = "application/json";
        return new ValueTask(context.HttpContext.Response.WriteAsync(
            @"{""success"":false,""message"":""The limit has been exceeded Please try again in one minute""}", cancellationToken));
    };
});
var app = builder.Build();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    await context.Database.MigrateAsync();
    await DbInitializer.SeedAsync(context, hasher, configuration);
}
app.Run();