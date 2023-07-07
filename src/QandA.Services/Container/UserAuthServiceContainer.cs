using Microsoft.Extensions.Configuration;
using QandA.DTOs.Payload.Response;
using System.Text.Json;

namespace QandA.Services.Container;

public interface IUserAuthServiceContainer
{
    Task<string> GetUserName(string reqeustHeaderProperty);
}

public class UserAuthServiceContainer : IUserAuthServiceContainer
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _auth0UserInfo;

    public UserAuthServiceContainer(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _auth0UserInfo = $"{configuration["Auth0:Authority"]}userinfo";
    }

    public async Task<string> GetUserName(string requestHeaderProperty)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _auth0UserInfo);
        request.Headers.Add("Authorization", requestHeaderProperty);
        var client = _httpClientFactory.CreateClient();
        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var jsonContent = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<User>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return user.Name;
        }
        else
        {
            return "";
        }
    }
}