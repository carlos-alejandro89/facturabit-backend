using System.Text.Json.Serialization;

namespace FacturaBit.Api.Contracts.Responses;

public sealed class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Mensaje { get; set; }

    [JsonPropertyName("httpCode")]
    public int HttpCode { get; set; }

    public T? Data { get; set; }
}
