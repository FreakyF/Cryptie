using Cryptie.Common.Entities;
using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Common.Features.Authentication.Services;
using Cryptie.Server.Features.Authentication.Services;
using Cryptie.Server.Persistence.DatabaseContext;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

public class AuthenticationServiceTests
{
    private readonly Mock<IAppDbContext> _dbContextMock = new();
    private readonly Mock<ILockoutService> _lockoutServiceMock = new();
    private readonly Mock<IDatabaseService> _databaseServiceMock = new();
    private readonly AuthenticationService _service;

    public AuthenticationServiceTests()
    {
        _service = new AuthenticationService(_dbContextMock.Object, _lockoutServiceMock.Object, _databaseServiceMock.Object);
    }

    [Fact]
    public void LoginHandler_UserLockedOut_ReturnsNotFound()
    {
        var loginRequest = new LoginRequestDto { Login = "user", Password = "pass" };
        var user = new User { Login = "user", Password = new Password { Secret = "hash" } };
        _dbContextMock.Setup(x => x.Users).Returns(MockDbSet(new List<User> { user }));
        _lockoutServiceMock.Setup(x => x.IsUserLockedOut(user, loginRequest.Login)).Returns(true);

        var result = _service.LoginHandler(loginRequest);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void LoginHandler_WrongPassword_ReturnsBadRequest()
    {
        var loginRequest = new LoginRequestDto { Login = "user", Password = "wrong" };
        var user = new User { Login = "user", Password = new Password { Secret = BCrypt.Net.BCrypt.HashPassword("pass") } };
        _dbContextMock.Setup(x => x.Users).Returns(MockDbSet(new List<User> { user }));
        _lockoutServiceMock.Setup(x => x.IsUserLockedOut(user, loginRequest.Login)).Returns(false);
        _databaseServiceMock.Setup(x => x.LogLoginAttempt(user));

        var result = _service.LoginHandler(loginRequest);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void LoginHandler_UserNotFound_ReturnsBadRequest()
    {
        var loginRequest = new LoginRequestDto { Login = "user", Password = "pass" };
        _dbContextMock.Setup(x => x.Users).Returns(MockDbSet(new List<User>()));
        _lockoutServiceMock.Setup(x => x.IsUserLockedOut(null, loginRequest.Login)).Returns(false);
        _databaseServiceMock.Setup(x => x.LogLoginAttempt(loginRequest.Login));

        var result = _service.LoginHandler(loginRequest);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void LoginHandler_Success_ReturnsOkWithTotpToken()
    {
        var loginRequest = new LoginRequestDto { Login = "user", Password = "pass" };
        var user = new User { Login = "user", Password = new Password { Secret = BCrypt.Net.BCrypt.HashPassword("pass") } };
        var totpGuid = Guid.NewGuid();
        _dbContextMock.Setup(x => x.Users).Returns(MockDbSet(new List<User> { user }));
        _lockoutServiceMock.Setup(x => x.IsUserLockedOut(user, loginRequest.Login)).Returns(false);
        _databaseServiceMock.Setup(x => x.CreateTotpToken(user)).Returns(totpGuid);

        var result = _service.LoginHandler(loginRequest) as OkObjectResult;
        Assert.NotNull(result);
        var dto = Assert.IsType<LoginResponseDto>(result.Value);
        Assert.Equal(totpGuid, dto.TotpToken);
    }

    [Fact]
    public void TotpHandler_TokenNotFound_ReturnsBadRequest()
    {
        var totpRequest = new TotpRequestDto { TotpToken = Guid.NewGuid(), Secret = "123" };
        _dbContextMock.Setup(x => x.TotpTokens).Returns(MockDbSet(new List<TotpToken>()));

        var result = _service.TotpHandler(totpRequest);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void TotpHandler_TokenExpired_ReturnsBadRequest()
    {
        var user = new User { Totp = new Totp { Secret = OtpNet.KeyGeneration.GenerateRandomKey(20) } };
        var totpToken = new TotpToken { Id = Guid.NewGuid(), Until = DateTime.UtcNow.AddMinutes(-1), User = user };
        _dbContextMock.Setup(x => x.TotpTokens).Returns(MockDbSet(new List<TotpToken> { totpToken }));

        var totpRequest = new TotpRequestDto { TotpToken = totpToken.Id, Secret = "123" };
        var result = _service.TotpHandler(totpRequest);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void TotpHandler_InvalidSecret_ReturnsBadRequest()
    {
        var secret = OtpNet.KeyGeneration.GenerateRandomKey(20);
        var user = new User { Totp = new Totp { Secret = secret } };
        var totpToken = new TotpToken { Id = Guid.NewGuid(), Until = DateTime.UtcNow.AddMinutes(5), User = user };
        _dbContextMock.Setup(x => x.TotpTokens).Returns(MockDbSet(new List<TotpToken> { totpToken }));

        var totpRequest = new TotpRequestDto { TotpToken = totpToken.Id, Secret = "invalid" };
        var result = _service.TotpHandler(totpRequest);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void TotpHandler_ValidSecret_ReturnsOkWithToken()
    {
        var secret = OtpNet.KeyGeneration.GenerateRandomKey(20);
        var user = new User { Totp = new Totp { Secret = secret } };
        var totpToken = new TotpToken { Id = Guid.NewGuid(), Until = DateTime.UtcNow.AddMinutes(5), User = user };
        _dbContextMock.Setup(x => x.TotpTokens).Returns(MockDbSet(new List<TotpToken> { totpToken }));
        var userTokenGuid = Guid.NewGuid();
        _databaseServiceMock.Setup(x => x.GenerateUserToken(user)).Returns(userTokenGuid);

        var totp = new OtpNet.Totp(secret);
        var validCode = totp.ComputeTotp();
        var totpRequest = new TotpRequestDto { TotpToken = totpToken.Id, Secret = validCode };
        var result = _service.TotpHandler(totpRequest) as OkObjectResult;
        Assert.NotNull(result);
        var dto = Assert.IsType<TotpResponseDto>(result.Value);
        Assert.Equal(userTokenGuid, dto.Token);
    }

    [Fact]
    public void LogoutHandler_TokenNotFound_ReturnsBadRequest()
    {
        var logoutRequest = new LogoutRequestDto { Token = Guid.NewGuid() };
        _dbContextMock.Setup(x => x.UserTokens).Returns(MockDbSet(new List<UserToken>()));

        var result = _service.LogoutHandler(logoutRequest);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void LogoutHandler_Success_ReturnsOk()
    {
        var token = new UserToken { Id = Guid.NewGuid() };
        _dbContextMock.Setup(x => x.UserTokens).Returns(MockDbSet(new List<UserToken> { token }));
        _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

        var logoutRequest = new LogoutRequestDto { Token = token.Id };
        var result = _service.LogoutHandler(logoutRequest);
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public void RegisterHandler_UserExists_ReturnsBadRequest()
    {
        var registerRequest = new RegisterRequestDto {
            Login = "user",
            Password = "pass",
            DisplayName = "User",
            Email = "mail@mail.com",
            PrivateKey = "priv",
            PublicKey = "pub"
        };
        _databaseServiceMock.Setup(x => x.FindUserByLogin(registerRequest.Login)).Returns(new User());

        var result = _service.RegisterHandler(registerRequest);
        Assert.IsType<BadRequestResult>(result);
    }

    
    private static DbSet<T> MockDbSet<T>(IEnumerable<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var dbSet = new Mock<DbSet<T>>();
        dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        return dbSet.Object;
    }

    
    private class EntityEntryStub<T> where T : class
    {
        public T Entity { get; }
        public EntityEntryStub(T entity) => Entity = entity;
    }
}
