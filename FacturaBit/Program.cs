using FacturaBit.Api.Contracts.Responses;
using FacturaBit.Application.Abstractions;
using FacturaBit.Application.Features;
using FacturaBit.Domain.Entities;
using FacturaBit.Infrastructure.Identity;
using FacturaBit.Infrastructure.Persistence;
using FacturaBit.Infrastructure.Persistence.Repositories;
using FacturaBit.Infrastructure.Services;
using FacturaBit.Infrastructure.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDBContext>(options => options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgreSQL")
    ).UseSnakeCaseNamingConvention()
);

var DapperConnectionString = builder.Configuration.GetConnectionString("PostgreSQL");
builder.Services.AddSingleton(new DapperDBContext(DapperConnectionString));

// Add services to the container.
builder.Services.AddControllers();
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
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<FacturaBit.Infrastructure.Security.Cryptography.OpenSSL>();
builder.Services.AddScoped<IEntidadFiscalService, EntidadFiscalService>();
builder.Services.AddScoped<IEntidadFiscalRepository, EntidadFiscalRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddSingleton<IAesEncryptionService, AesGcmEncryptionService>();
builder.Services.AddScoped<ICertificadoSelloDigitalRepository, CertificadoSelloDigitalRepository>();
builder.Services.AddScoped<IComprobanteFiscalRepository, ComprobanteFiscalRepository>();
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

app.MapControllers();



app.Run();
