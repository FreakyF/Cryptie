using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Common.Entities.User;
using Cryptie.Common.Features.UserManagement.DTOs;

namespace Cryptie.Client;

public interface IUserManagementService
{
    Task<User> GetUserAsync(string toekn, CancellationToken ct = default);
}

public sealed class UserManagementService : IUserManagementService
{
    private readonly HttpClient _http;

    public UserManagementService(HttpClient http) => _http = http;

    public async Task<User> GetUserAsync(string token, CancellationToken ct = default)
    {
        // Przygotuj DTO
        var requestDto = new UserRequestDto { Toekn = Guid.Parse(token) };
        // Serializacja do JSON
        var json = JsonSerializer.Serialize(requestDto);
        // Zbuduj zapytanie GET z body
        using var request = new HttpRequestMessage(HttpMethod.Get, "user/user")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        // Wyślij i obsłuż odpowiedź
        using var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content
                          .ReadFromJsonAsync<UserResponseDto>(cancellationToken: ct)
                      ?? throw new HttpRequestException("Brak treści odpowiedzi /user/user.");

        return payload.User
               ?? throw new HttpRequestException("Pole User w odpowiedzi jest puste.");
    }
}