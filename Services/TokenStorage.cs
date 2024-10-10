using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class TokenStorage
{
    private const string TokenUrl = "https://id.sandbox.splitit.com/connect/token";
    private const string ClientId = "SplititJobAssignment";
    private const string ClientSecret = "kH1kyMSQ3ZB21qkUS1TtsHU24XnzPVQdN9kC0BinCCIRpaZCJW";

    public string GeneratedToken { get; private set; }

    public async Task InitializeTokenAsync()
    {
        GeneratedToken = await GetTokenAsync();
    }

    private async Task<string> GetTokenAsync()
    {
        using var httpClient = new HttpClient();
        var parameters = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("scope", "job.assignment.api"),
            new KeyValuePair<string, string>("client_secret", ClientSecret),
            new KeyValuePair<string, string>("client_id", ClientId),
        });

        var response = await httpClient.PostAsync(TokenUrl, parameters);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return ExtractTokenFromJson(content);
        }

        throw new HttpRequestException("Failed to retrieve token from API");
    }

    private string ExtractTokenFromJson(string jsonResponse)
    {
        var jsonObject = JObject.Parse(jsonResponse);
        return jsonObject["access_token"]?.ToString();
    }
}
