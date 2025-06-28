// using System.Net;
// using System.Net.Http.Json;
// using Cryptie.Common.Features.Authentication.DTOs;
// using FluentAssertions;
//
// namespace Cryptie.Server.Tests;
//
// public class AuthenticationEndpointTests(AuthenticationApiFactory factory) : IClassFixture<AuthenticationApiFactory>
// {
//     private readonly HttpClient _httpClient = factory.CreateClient();
//
//     private static RegisterRequestDto CreateRegisterRequest()
//     {
//         var request = new RegisterRequestDto
//         {
//             Login = "Username123",
//             Password = "Password1234!",
//             DisplayName = "user 123",
//             Email = "test@test.com"
//         };
//         return request;
//     }
//
//     [Trait("TestCategory", "Integration")]
//     [Theory]
//     [InlineData(null, HttpStatusCode.BadRequest)]
//     [InlineData("", HttpStatusCode.OK)]
//     [InlineData(" ", HttpStatusCode.OK)]
//     [InlineData(";;;abba", HttpStatusCode.OK)]
//     [InlineData("ada", HttpStatusCode.OK)]
//     [InlineData("abcdefghijklmnopqrstuvwxyz", HttpStatusCode.OK)]
//     public async Task InvalidRegisterUsernameRequest(string username, HttpStatusCode statusCode)
//     {
//         var request = new RegisterRequestDto
//         {
//             Login = username,
//             Password = "Password1234!",
//             DisplayName = "user 123",
//             Email = "test@test.com"
//         };
//         var response = await _httpClient.PostAsJsonAsync("auth/register", request);
//         await response.Content.ReadAsStringAsync();
//         response.StatusCode.Should().Be(statusCode);
//     }
//
//     [Trait("TestCategory", "Integration")]
//     [Theory]
//     [InlineData(null, HttpStatusCode.BadRequest)]
//     [InlineData("", HttpStatusCode.OK)]
//     [InlineData(" ", HttpStatusCode.OK)]
//     [InlineData("\u200b", HttpStatusCode.OK)]
//     [InlineData("Te!st2", HttpStatusCode.OK)]
//     [InlineData("Test!Passwordabcd12345", HttpStatusCode.OK)]
//     [InlineData("password!123", HttpStatusCode.OK)]
//     [InlineData("PASSWORD!123", HttpStatusCode.OK)]
//     [InlineData("PASSWORD!abc", HttpStatusCode.OK)]
//     [InlineData("PASSWORD1234abc", HttpStatusCode.OK)]
//     public async Task InvalidRegisterPasswordRequest(string password, HttpStatusCode statusCode)
//     {
//         var request = new RegisterRequestDto
//         {
//             Login = "Username123",
//             Password = password,
//             DisplayName = "user 123",
//             Email = "test@test.com"
//         };
//         var response = await _httpClient.PostAsJsonAsync("auth/register", request);
//         await response.Content.ReadAsStringAsync();
//         response.StatusCode.Should().Be(statusCode);
//     }
//
//     [Trait("TestCategory", "Integration")]
//     [Theory]
//     [InlineData(null, HttpStatusCode.BadRequest)]
//     [InlineData("", HttpStatusCode.OK)]
//     [InlineData(" ", HttpStatusCode.OK)]
//     [InlineData("abcdefghijklmnopqrstuvwxyz", HttpStatusCode.OK)]
//     public async Task InvalidRegisterDisplayNameRequest(string displayName, HttpStatusCode statusCode)
//     {
//         var request = new RegisterRequestDto
//         {
//             Login = "Username123",
//             Password = "Password1234!",
//             DisplayName = displayName,
//             Email = "test@test.com"
//         };
//         var response = await _httpClient.PostAsJsonAsync("auth/register", request);
//         await response.Content.ReadAsStringAsync();
//         response.StatusCode.Should().Be(statusCode);
//     }
//
//     [Trait("TestCategory", "Integration")]
//     [Theory]
//     [InlineData(null, HttpStatusCode.BadRequest)]
//     [InlineData("", HttpStatusCode.OK)]
//     [InlineData(" ", HttpStatusCode.OK)]
//     public async Task InvalidRegisterEmailRequest(string email, HttpStatusCode statusCode)
//     {
//         var request = new RegisterRequestDto
//         {
//             Login = "Username123",
//             Password = "Password1234!",
//             DisplayName = "user 123",
//             Email = email
//         };
//         var response = await _httpClient.PostAsJsonAsync("auth/register", request);
//         await response.Content.ReadAsStringAsync();
//         response.StatusCode.Should().Be(statusCode);
//     }
//
//     [Trait("TestCategory", "Integration")]
//     [Fact]
//     public async Task ValidRegisterRequest()
//     {
//         var request = new RegisterRequestDto
//         {
//             Login = "User1234",
//             Password = "Password1234!",
//             DisplayName = "user 123",
//             Email = "test@test.com"
//         };
//         var response = await _httpClient.PostAsJsonAsync("auth/register", request);
//         await response.Content.ReadAsStringAsync();
//         response.StatusCode.Should().Be(HttpStatusCode.OK);
//     }
//
//     [Trait("TestCategory", "Integration")]
//     [Theory]
//     [InlineData(null, HttpStatusCode.BadRequest)]
//     [InlineData("", HttpStatusCode.BadRequest)]
//     [InlineData(" ", HttpStatusCode.BadRequest)]
//     [InlineData("user*&()", HttpStatusCode.BadRequest)]
//     [InlineData("abcd", HttpStatusCode.BadRequest)]
//     [InlineData("abcdefghijklmnopqrstuvwxyz", HttpStatusCode.BadRequest)]
//     public async Task InvalidLoginUsernameRequest(string username, HttpStatusCode statusCode)
//     {
//         var dto = CreateRegisterRequest();
//         await _httpClient.PostAsJsonAsync("auth/register", dto);
//         var requestDto = new LoginRequestDto
//         {
//             Login = username,
//             Password = "Password1234!"
//         };
//         var response = await _httpClient.PostAsJsonAsync("auth/login", requestDto);
//         await response.Content.ReadAsStringAsync();
//         response.StatusCode.Should().Be(statusCode);
//     }
//
//     [Trait("TestCategory", "Integration")]
//     [Theory]
//     [InlineData(null, HttpStatusCode.BadRequest)]
//     [InlineData("", HttpStatusCode.InternalServerError)]
//     [InlineData(" ", HttpStatusCode.InternalServerError)]
//     [InlineData("\u200b", HttpStatusCode.InternalServerError)]
//     [InlineData("Te!st2", HttpStatusCode.InternalServerError)]
//     [InlineData("Test!Passwordabcd12345", HttpStatusCode.InternalServerError)]
//     [InlineData("password!123", HttpStatusCode.InternalServerError)]
//     [InlineData("PASSWORD!123", HttpStatusCode.InternalServerError)]
//     [InlineData("PASSWORD!abc", HttpStatusCode.InternalServerError)]
//     [InlineData("PASSWORD1234abc", HttpStatusCode.InternalServerError)]
//     public async Task InvalidLoginPasswordRequest(string password, HttpStatusCode status)
//     {
//         var dto = CreateRegisterRequest();
//         await _httpClient.PostAsJsonAsync("auth/register", dto);
//         var requestDto = new LoginRequestDto
//         {
//             Login = "Username123",
//             Password = password
//         };
//         var response = await _httpClient.PostAsJsonAsync("auth/login", requestDto);
//         await response.Content.ReadAsStringAsync();
//         response.StatusCode.Should().Be(status);
//     }
//
//     // [Trait("TestCategory", "Integration")]
//     // [Fact]
//     public async Task ValidLoginRequest()
//     {
//         // var dto = new RegisterRequestDto
//         // {
//         //     Login = "Username1234",
//         //     Password = "Password1234!",
//         //     DisplayName = "user 1234",
//         //     Email = "test1@test.com"
//         // };
//         //await _httpClient.PostAsJsonAsync("auth/register", dto);
//         var requestDto = new LoginRequestDto
//         {
//             Login = "Username1234",
//             Password = "Password1234!"
//         };
//         var response = await _httpClient.PostAsJsonAsync("auth/login", requestDto);
//         await response.Content.ReadAsStringAsync();
//         response.StatusCode.Should().Be(HttpStatusCode.OK); //poprawić, nie loguje
//     }
// }