using System.Net;
using System.Net.Http.Json;
using Cryptie.Common.Features.Authentication.DTOs;
using FluentAssertions;
using Org.BouncyCastle.Asn1.Ocsp;
using Xunit.Abstractions;

namespace Cryptie.Server.API.Tests;

public class AuthenticationEndpointTests : IClassFixture<AuthenticationApiFactory>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _httpClient;

    public AuthenticationEndpointTests(AuthenticationApiFactory factory, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task ValidRegisterRequest()
    {
        var request = new RegisterRequestDto
        {
            Login = "User1234",
            Password = "Password1234!",
            DisplayName = "user 123",
            Email = "test@test.com",
        };
        var response = await _httpClient.PostAsJsonAsync("auth/register", request);
        var body = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine($"Status: {(int)response.StatusCode}\n{body}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}