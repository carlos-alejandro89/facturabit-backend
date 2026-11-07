using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using FacturaBit.Api.Contracts.Responses;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.Features;
using FacturaBit.Domain.Entities;
using FacturaBit.Infrastructure.Identity;
using FacturaBit.Infrastructure.Persistence;
using FacturaBit.Infrastructure.Persistence.Repositories;
using FacturaBit.Infrastructure.Services;
using FacturaBit.Infrastructure.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDBContext>(options => options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgreSQL")
    ).UseSnakeCaseNamingConvention()
);

var DapperConnectionString = builder.Configuration.GetConnectionString("PostgreSQL");
builder.Services.AddSingleton(new DapperDBContext(DapperConnectionString));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        string[] allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        string[] errors = context.ModelState.Values
            .SelectMany(value => value.Errors)
            .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                ? "El valor proporcionado no es válido."
                : error.ErrorMessage)
            .ToArray();

        return new BadRequestObjectResult(new ApiResponse<object>
        {
            Success = false,
            Mensaje = "Uno o más datos del payload no son válidos.",
            HttpCode = StatusCodes.Status400BadRequest,
            Data = new { Errores = errors }
        });
    };
});
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "FacturaBit API", Version = "v1" });

    // ── Soporte para Bearer JWT en Swagger UI ──────────────────────────────
    var securityScheme = new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Ingresa el token JWT. Ejemplo: eyJhbGci..."
    };
    
    //Permite personalizar la documentacion Swagger de los endpoints
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    
    c.IncludeXmlComments(xmlPath);
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// Configuración de JWT Local
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Key"] ?? "ClaveSuperSecretaMinimo32Caracteres!!";
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmitirCfdi", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context =>
        {
            string? authType = context.User.FindFirst("auth_type")?.Value;
            if (!string.Equals(authType, "client_credentials", StringComparison.Ordinal))
                return context.User.IsInRole("SUPERADMIN") || context.User.IsInRole("ADMIN") || context.User.IsInRole("FACTURISTA");

            string scope = context.User.FindFirst("scope")?.Value ?? string.Empty;
            return scope.Split(' ', StringSplitOptions.RemoveEmptyEntries).Contains("cfdi.emit", StringComparer.Ordinal);
        });
    });
    options.AddPolicy("ConsultarEstatusCfdi", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context =>
        {
            string? authType = context.User.FindFirst("auth_type")?.Value;
            if (!string.Equals(authType, "client_credentials", StringComparison.Ordinal))
                return context.User.IsInRole("SUPERADMIN") || context.User.IsInRole("ADMIN") || context.User.IsInRole("FACTURISTA");

            string scope = context.User.FindFirst("scope")?.Value ?? string.Empty;
            return scope.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Contains("cfdi.status", StringComparer.Ordinal);
        });
    });
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("ApiClients", context =>
    {
        string partitionKey = context.User.FindFirst("client_id")?.Value
            ?? context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? context.Connection.RemoteIpAddress?.ToString()
            ?? "anonymous";

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 60,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        });
    });
    options.AddPolicy("PasswordRecovery", context =>
    {
        string partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(15),
            QueueLimit = 0,
            AutoReplenishment = true
        });
    });
});

builder.Services.AddScoped<FacturaBit.Infrastructure.Security.Cryptography.OpenSSL>();
builder.Services.AddScoped<IEntidadFiscalService, EntidadFiscalService>();
builder.Services.AddScoped<IEntidadFiscalRepository, EntidadFiscalRepository>();
builder.Services.AddScoped<ISatRegimenFiscalService, SatRegimenFiscalService>();
builder.Services.AddScoped<ISatRegimenFiscalRepository, SatRegimenFiscalRepository>();
builder.Services.AddScoped<ISatMotivosCancelacionService, SatMotivosCancelacionService>();
builder.Services.AddScoped<ISatMotivosCancelacionRepository, SatMotivosCancelacionRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<IPasswordRecoveryRepository, PasswordRecoveryRepository>();
builder.Services.AddScoped<IPasswordRecoveryService, PasswordRecoveryService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IApiClientService, ApiClientService>();
builder.Services.AddScoped<IApiClientRepository, ApiClientRepository>();
builder.Services.AddScoped<IApiClientSecretService, ApiClientSecretService>();
builder.Services.AddSingleton<IApplicationTokenGenerator, ApplicationTokenGenerator>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IAesEncryptionService, AesGcmEncryptionService>();
builder.Services.AddScoped<ICertificadoSelloDigitalRepository, CertificadoSelloDigitalRepository>();
builder.Services.AddScoped<ICertificadoDigitalService, CertificadoDigitalService>();
builder.Services.AddScoped<IComprobanteFiscalRepository, ComprobanteFiscalRepository>();
builder.Services.AddScoped<IComprobanteFiscalService, ComprobanteFiscalService>();
builder.Services.AddScoped<IPasswordHasher<Accounts>, PasswordHasher<Accounts>>();
builder.Services.AddScoped<IPasswordHasher<ApiClient>, PasswordHasher<ApiClient>>();
builder.Services.AddScoped<ICertificateStorage, CertificateStorage>();
builder.Services.AddScoped<IDocumentPdfService, DocumentPdfService>();
builder.Services.AddScoped<ICfdiXmlBuilder, CfdiXmlBuilder>();
builder.Services.AddScoped<ICfdiSigner, CfdiSigner>();
builder.Services.AddScoped<ITimbradoService, TimbradoSWSapiens>();
builder.Services.AddScoped<IFacturacionService, FacturacionService>();
builder.Services.AddScoped<ICancelacionCfdiService, CancelacionCfdiService>();
builder.Services.AddScoped<IConsultaEstatusCfdiService, ConsultaEstatusCfdiService>();
builder.Services.Configure<SatConsultaEstatusOptions>(
    builder.Configuration.GetSection(SatConsultaEstatusOptions.SectionName));
builder.Services.AddHttpClient<ISatConsultaEstatusClient, SatConsultaEstatusClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.Configure<PasswordRecoveryOptions>(
    builder.Configuration.GetSection(PasswordRecoveryOptions.SectionName));
builder.Services.Configure<EmailOptions>(
    builder.Configuration.GetSection(EmailOptions.SectionName));

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    AppDBContext db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();



app.Run();
