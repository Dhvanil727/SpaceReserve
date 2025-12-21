using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpaceReserve.Admin.AppService.Contracts;

namespace SpaceReserve.Admin.AppService.Services;

public class KeyCloakAppService : IKeyCloakAppService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public KeyCloakAppService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public async Task UpdateUserStatusInKeycloakAsync(string subjectId, bool isActive)
    {
        try
        {
            string accessToken = await GetAccessTokenAsync();
            var adminBaseUrl = _configuration["Keycloak:AdminBaseUrl"];
            var keycloakApiUrl = $"{adminBaseUrl}/users/{subjectId}";

            var client = _httpClientFactory.CreateClient();
            var payload = new
            {
                id = subjectId,
                enabled = isActive
            };

            var updateRequest = new HttpRequestMessage(HttpMethod.Put, keycloakApiUrl)
            {
                Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
            };
            updateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var updateResponse = await client.SendAsync(updateRequest);
            if (!updateResponse.IsSuccessStatusCode)
                throw new Exception($"Failed to update user in Keycloak.");

        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating user in Keycloak: {ex.Message}");
        }
    }
    public async Task<bool> GetUserEnabledStatusAsync(string subjectId)
    {
        try
        {
            string accessToken = await GetAccessTokenAsync();
            var adminBaseUrl = _configuration["Keycloak:AdminBaseUrl"];
            var keycloakApiUrl = $"{adminBaseUrl}/users/{subjectId}";

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, keycloakApiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to fetch user info from Keycloak");

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<JObject>(content);
            return result["enabled"]?.Value<bool>() ?? throw new Exception("Keycloak user 'enabled' property not found.");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error fetching user status from Keycloak: {ex.Message}");
        }
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var url = _configuration["Keycloak:TokenEndpoint"];
        var clientId = _configuration["Keycloak:ClientId"];
        var clientSecret = _configuration["Keycloak:ClientSecret"];
        var grantType = "client_credentials";

        var client = _httpClientFactory.CreateClient();
        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", grantType),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("scope", "openid")
        });

        var httpResponse = await client.PostAsync(url, formContent);

        var content = await httpResponse.Content.ReadAsStringAsync();
        var result = (JObject)JsonConvert.DeserializeObject(content);
        return result["access_token"]?.ToString() ?? throw new Exception("Failed to fetch access token from KeyCloak");
    }
}
