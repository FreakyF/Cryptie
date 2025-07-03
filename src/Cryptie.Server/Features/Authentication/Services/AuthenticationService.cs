using Cryptie.Common.Entities;
using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Server.Persistence.DatabaseContext;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using Totp = Cryptie.Common.Entities.Totp;

namespace Cryptie.Server.Features.Authentication.Services;

public class AuthenticationService(
    IAppDbContext appDbContext,
    ILockoutService lockoutService,
    IDatabaseService databaseService)
    : ControllerBase, IAuthenticationService
{
    /// <summary>
    /// Handles user login by validating credentials and issuing a TOTP token
    /// when successful.
    /// </summary>
    /// <param name="loginRequest">Login credentials.</param>
    /// <returns>An <see cref="IActionResult"/> describing the outcome.</returns>
    public IActionResult LoginHandler(LoginRequestDto loginRequest)
    {
        var user = appDbContext.Users
            .Include(user => user.Password)
            .SingleOrDefault(u => u.Login == loginRequest.Login);

        if (lockoutService.IsUserLockedOut(user, loginRequest.Login)) return NotFound();

        if (user != null && !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password.Secret))
        {
            databaseService.LogLoginAttempt(user);
            return BadRequest();
        }

        if (user == null)
        {
            databaseService.LogLoginAttempt(loginRequest.Login);
            return BadRequest();
        }

        var totpToken = databaseService.CreateTotpToken(user);

        return Ok(new LoginResponseDto { TotpToken = totpToken });
    }

    /// <summary>
    /// Validates a TOTP code and returns a session token if it is correct.
    /// </summary>
    /// <param name="totpRequest">User TOTP request.</param>
    /// <returns>An <see cref="IActionResult"/> describing the outcome.</returns>
    public IActionResult TotpHandler(TotpRequestDto totpRequest)
    {
        var now = DateTime.UtcNow;

        var totpToken = appDbContext.TotpTokens
            .Include(totpTokens => totpTokens.User)
            .ThenInclude(user => user.Totp)
            .SingleOrDefault(t => t.Id == totpRequest.TotpToken
            );

        if (totpToken == null) return BadRequest();

        if (totpToken.Until < now)
        {
            appDbContext.TotpTokens.Remove(totpToken);
            return BadRequest();
        }

        var totp = new OtpNet.Totp(totpToken.User.Totp.Secret);
        var isValid = totp.VerifyTotp(totpRequest.Secret, out _);

        if (!isValid) return BadRequest();

        var token = databaseService.GenerateUserToken(totpToken.User);
        appDbContext.TotpTokens.Remove(totpToken);

        return Ok(new TotpResponseDto
        {
            Token = token
        });
    }

    /// <summary>
    /// Registers a new user and returns provisioning information for TOTP.
    /// </summary>
    /// <param name="registerRequest">Registration details.</param>
    /// <returns>An <see cref="IActionResult"/> with the registration result.</returns>
    public IActionResult RegisterHandler(RegisterRequestDto registerRequest)
    {
        if (databaseService.FindUserByLogin(registerRequest.Login) != null) return BadRequest();

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

        var password = appDbContext.Passwords.Add(new Password
        {
            Id = Guid.Empty,
            Secret = hashedPassword
        });

        var totp = appDbContext.Totps.Add(new Totp
        {
            Id = Guid.Empty,
            Secret = KeyGeneration.GenerateRandomKey(20)
        });

        var user = appDbContext.Users.Add(new User
        {
            Id = Guid.Empty,
            Email = registerRequest.Email,
            Login = registerRequest.Login,
            DisplayName = registerRequest.DisplayName,
            Password = password.Entity,
            Totp = totp.Entity,
            PublicKey = registerRequest.PublicKey,
            PrivateKey = registerRequest.PrivateKey,
            ControlValue = registerRequest.ControlValue
        });

        var totpToken = databaseService.CreateTotpToken(user.Entity);

        appDbContext.SaveChanges();

        return Ok(new RegisterResponseDto
        {
            Secret =
                $"otpauth://totp/Cryptie:{user.Entity.Login}?secret={Base32Encoding.ToString(totp.Entity.Secret)}&issuer=Cryptie",
            TotpToken = totpToken
        });
    }
}