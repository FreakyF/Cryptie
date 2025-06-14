using Cryptie.Common.Entities.User;
using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Common.Features.Authentication.Services;
using Cryptie.Server.Persistence.DatabaseContext;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using Totp = Cryptie.Common.Entities.User.Totp;

namespace Cryptie.Server.Features.Authentication.Services;

public class AuthenticationService(
    AppDbContext appDbContext,
    ILockoutService lockoutService,
    IDatabaseService databaseService)
    : ControllerBase, IAuthenticationService
{
    public IActionResult LoginHandler(LoginRequestDto loginRequest)
    {
        var user = appDbContext.Users
            .Include(user => user.Password)
            .SingleOrDefault(u => u.Login == loginRequest.Login);

        if (lockoutService.IsUserLockedOut(user, loginRequest.Login))
        {
            return NotFound();
        }

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

    public IActionResult TotpHandler(TotpRequestDto totpRequest)
    {
        var now = DateTime.UtcNow;

        var totpToken = appDbContext.TotpTokens
            .Include(totpTokens => totpTokens.User)
            .ThenInclude(user => user.Totp)
            .SingleOrDefault(t => t.Id == totpRequest.TotpToken
            );

        if (totpToken == null)
        {
            return BadRequest();
        }

        if (totpToken.Until < now)
        {
            appDbContext.TotpTokens.Remove(totpToken);
            return BadRequest();
        }

        var totp = new OtpNet.Totp(totpToken.User.Totp.Secret);
        var isValid = totp.VerifyTotp(totpRequest.Secret, out _);

        if (!isValid)
        {
            return BadRequest();
        }

        var token = databaseService.GenerateUserToken(totpToken.User);
        appDbContext.TotpTokens.Remove(totpToken);

        return Ok(new TotpResponseDto
        {
            Token = token
        });
    }

    public IActionResult LogoutHandler(LogoutRequestDto logoutRequest)
    {
        var userToken = appDbContext.UserTokens.SingleOrDefault(t => t.Id == logoutRequest.Token);

        if (userToken == null)
        {
            return BadRequest();
        }

        appDbContext.UserTokens.Remove(userToken);
        appDbContext.SaveChangesAsync();

        return Ok();
    }

    public IActionResult RegisterHandler(RegisterRequestDto registerRequest)
    {
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
            Totp = totp.Entity
        });

        var totpToken = databaseService.CreateTotpToken(user.Entity);

        appDbContext.SaveChanges();

        return Ok(new RegisterResponseDto
        {
            Secret =
                $"otpauth://totp/Panel:{user.Entity.Login}?secret={Base32Encoding.ToString(totp.Entity.Secret)}&issuer=Panel",
            TotpToken = totpToken
        });
    }
}