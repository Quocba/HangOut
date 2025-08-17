using System.Text.Json;
using System.Text.Json.Serialization;

namespace HangOut.Domain.Payload.Base;

public class ApiResponse<T>
{
    [JsonPropertyName("status")]
    public int Status { get; set; }
    [JsonPropertyName("message")]
    public string? Message { get; set; } = string.Empty;
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

public class ApiResponse : ApiResponse<object>
{
    public ApiResponse() { }
}