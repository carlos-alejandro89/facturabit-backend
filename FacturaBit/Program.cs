using System.Reflection;
using System.Text;
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
builder.Services.AddAuthorization();

builder.Services.AddScoped<FacturaBit.Infrastructure.Security.Cryptography.OpenSSL>();
builder.Services.AddScoped<IEntidadFiscalService, EntidadFiscalService>();
builder.Services.AddScoped<IEntidadFiscalRepository, EntidadFiscalRepository>();
builder.Services.AddScoped<ISatRegimenFiscalService, SatRegimenFiscalService>();
builder.Services.AddScoped<ISatRegimenFiscalRepository, SatRegimenFiscalRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IAesEncryptionService, AesGcmEncryptionService>();
builder.Services.AddScoped<ICertificadoSelloDigitalRepository, CertificadoSelloDigitalRepository>();
builder.Services.AddScoped<ICertificadoDigitalService, CertificadoDigitalService>();
builder.Services.AddScoped<IComprobanteFiscalRepository, ComprobanteFiscalRepository>();
builder.Services.AddScoped<IComprobanteFiscalService, ComprobanteFiscalService>();
builder.Services.AddScoped<IPasswordHasher<Accounts>, PasswordHasher<Accounts>>();
builder.Services.AddScoped<ICertificateStorage, CertificateStorage>();
builder.Services.AddScoped<ICfdiXmlBuilder, CfdiXmlBuilder>();
builder.Services.AddScoped<ICfdiSigner, CfdiSigner>();
builder.Services.AddScoped<ITimbradoService, TimbradoSWSapiens>();
builder.Services.AddScoped<IFacturacionService, FacturacionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();



app.Run();
