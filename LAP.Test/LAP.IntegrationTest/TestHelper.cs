using System.Net.Http.Json;
using System.Text.Json;

namespace LAP.Test.IntegrationTest;

public static class TestHelper
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
    };

    public static readonly JsonSerializerOptions SnakeCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    public static JsonContent CreateJsonContent<T>(T value)
    {
        return JsonContent.Create(value, options: JsonOptions);
    }

    public static JsonContent ToJsonContent<T>(T value)
        => JsonContent.Create(value, options: SnakeCaseOptions);

    public static Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient client, string url, T value)
        => client.PostAsync(url, JsonContent.Create(value, options: SnakeCaseOptions));

    public static Task<HttpResponseMessage> PutJsonAsync<T>(this HttpClient client, string url, T value)
        => client.PutAsync(url, JsonContent.Create(value, options: SnakeCaseOptions));
}
