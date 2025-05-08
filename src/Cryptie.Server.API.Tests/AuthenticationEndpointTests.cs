using System.Net;
using System.Net.Http.Json;
using Cryptie.Common.Features.Authentication.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Identity.Data;
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

    private RegisterRequestDto CreateRegisterRequest()
    {
        var request = new RegisterRequestDto()
        {
            Login = "Username123",
            Password = "Password1234!",
            DisplayName = "user 123",
            Email = "test@test.com",
        };
        return request;
    } 
    
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
        var response = await _httpClient.PostAsJsonAsync("auth/register", request);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

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
        var response = await _httpClient.PostAsJsonAsync("auth/register", request);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("abcdefghijklmnopqrstuvwxyz")]
    public async Task InvalidRegisterDisplayNameRequest(string displayName)
    {
        var request = new RegisterRequestDto()
        {
            Login = "Username123",
            Password = "Password1234!",
            DisplayName = displayName,
            Email = "test@test.com",
        };
        var response = await _httpClient.PostAsJsonAsync("auth/register", request);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task InvalidRegisterEmailRequest(string email)
    {
        var request = new RegisterRequestDto()
        {
            Login = "Username123",
            Password = "Password1234!",
            DisplayName = "user 123",
            Email = email,
        };
        var response = await _httpClient.PostAsJsonAsync("auth/register", request);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
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
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    //[InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("user*&()")]
    [InlineData("abcd")]
    [InlineData("abcdefghijklmnopqrstuvwxyz")]
    public async Task InvalidLoginUsernameRequest(string username)
    {
        var dto = CreateRegisterRequest();
        await _httpClient.PostAsJsonAsync("auth/register",dto);
        var RequestDto = new LoginRequestDto()
        {
            Login = username,
            Password = "Password1234!",
        };
        var response = await _httpClient.PostAsJsonAsync("auth/login", RequestDto);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Theory]
    //[InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\u200b")]
    [InlineData("Te!st2")]
    [InlineData("Test!Passwordabcd12345")]
    [InlineData("password!123")]
    [InlineData("PASSWORD!123")]
    [InlineData("PASSWORD!abc")]
    [InlineData("PASSWORD1234abc")]
    public async Task InvalidLoginPasswordRequest(string password)
    {
        var dto = CreateRegisterRequest();
        await _httpClient.PostAsJsonAsync("auth/register",dto);
        var RequestDto = new LoginRequestDto()
        {
            Login = "Username123",
            Password = password,
        };
        var response = await _httpClient.PostAsJsonAsync("auth/login", RequestDto);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ValidLoginRequest()
    {
        var dto = CreateRegisterRequest();
        await _httpClient.PostAsJsonAsync("auth/register", dto);
        var RequestDto = new LoginRequestDto()
        {
            Login = "Username123",
            Password = "Password1234!",
        };
        var response = await _httpClient.PostAsJsonAsync("auth/login", RequestDto);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}