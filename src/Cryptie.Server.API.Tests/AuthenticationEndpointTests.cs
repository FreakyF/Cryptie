using System.Net;
using System.Net.Http.Json;
using Cryptie.Common.Features.Authentication.DTOs;
using FluentAssertions;
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

    [Trait("TestCategory", "Integration")]
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(";;;abba")]
    [InlineData("ada")]
    [InlineData("abcdefghijklmnopqrstuvwxyz")]
    public async Task InvalidRegisterUsernameRequest(string username)
    {
        var request = new RegisterRequestDto
        {
            Login = username,
            Password = "Password1234!",
            DisplayName = "user 123",
            Email = "test@test.com",
        };
        var response = await _httpClient.PostAsJsonAsync("api/register", request);
        var body = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine($"Status: {(int)response.StatusCode}\n{body}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Trait("TestCategory", "Integration")]
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\u200b12345AA!")]
    [InlineData("Te!st2")]
    [InlineData("Test!Passwordabcd12345")]
    [InlineData("te!st12356")]
    [InlineData("TEST!123456")]
    [InlineData("TEST!test")]
    [InlineData("Test123456")]
    public async Task InvalidRegisterPasswordRequest(string password)
    {
        var request = new RegisterRequestDto
        {
            Login = "Username123",
            Password = password,
            DisplayName = "user 123",
            Email = "test@test.com",
        };
        var response = await _httpClient.PostAsJsonAsync("api/register", request);
        var body = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine($"Status: {(int)response.StatusCode}\n{body}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Trait("TestCategory", "Integration")]
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