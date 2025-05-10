using Cryptie.Client.Desktop.Features.Authentication.Models;
using Cryptie.Common.Features.Authentication.DTOs;
using Mapster;

namespace Cryptie.Client.Desktop.Features.Authentication.Mapping;

// ReSharper disable once UnusedType.Global
public class AuthenticationMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<LoginModel, LoginRequestDto>()
            .Map(dest => dest.Login, src => src.Username)
            .Map(dest => dest.Password, src => src.Password);

        config.NewConfig<RegisterModel, RegisterRequestDto>()
            .Map(dest => dest.Login, src => src.Username)
            .Map(dest => dest.DisplayName, src => src.DisplayName)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.Password, src => src.Password);
    }
}