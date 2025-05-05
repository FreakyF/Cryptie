using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Cryptie.Client.Desktop.Composition.Factories;
using Cryptie.Client.Desktop.Models;
using Cryptie.Client.Domain.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.DTOs;
using FluentValidation;
using FluentValidation.Results;
using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels
{
    public class RegisterViewModel : RoutableViewModelBase
    {
        private readonly IAuthenticationService _authentication;
        private readonly IAppCoordinator _coordinator;
        private readonly IValidator<RegisterRequestDto> _validator;

        public RegisterModel Model { get; } = new();
        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToLoginCommand { get; }

        public RegisterViewModel(
            IAuthenticationService authentication,
            IAppCoordinator coordinator,
            IValidator<RegisterRequestDto> validator
        ) : base(coordinator)
        {
            _authentication = authentication;
            _coordinator = coordinator;
            _validator = validator;

            RegisterCommand = ReactiveCommand.CreateFromTask(
                RegisterAsync,
                SetupValidation()
            );

            GoToLoginCommand = ReactiveCommand.Create(() =>
                _coordinator.ShowLogin()
            );
        }

        private IObservable<bool> SetupValidation()
        {
            var usernameTouched = SetupTouched(x => x.Model.Username);
            var displayNameTouched = SetupTouched(x => x.Model.DisplayName);
            var emailTouched = SetupTouched(x => x.Model.Email);
            var passwordTouched = SetupTouched(x => x.Model.Password);

            var validationChanged = this.WhenAnyValue(
                    x => x.Model.Username,
                    x => x.Model.DisplayName,
                    x => x.Model.Email,
                    x => x.Model.Password
                )
                .Select(_ => ValidateDto())
                .Publish()
                .RefCount();

            var errorStream = validationChanged
                .CombineLatest(
                    usernameTouched,
                    displayNameTouched,
                    emailTouched,
                    passwordTouched,
                    (result, uT, dT, eT, pT) =>
                    {
                        var propertyOrder = new[]
                        {
                            nameof(RegisterRequestDto.Login),
                            nameof(RegisterRequestDto.DisplayName),
                            nameof(RegisterRequestDto.Email),
                            nameof(RegisterRequestDto.Password)
                        };

                        var touchedMap = new Dictionary<string, bool>
                        {
                            { propertyOrder[0], uT },
                            { propertyOrder[1], dT },
                            { propertyOrder[2], eT },
                            { propertyOrder[3], pT }
                        };

                        var first = result.Errors
                            .OrderBy(e => Array.IndexOf(propertyOrder, e.PropertyName))
                            .FirstOrDefault(e =>
                                touchedMap.TryGetValue(e.PropertyName, out var touched)
                                && touched
                            );

                        return first?.ErrorMessage ?? string.Empty;
                    }
                )
                .ObserveOn(RxApp.MainThreadScheduler);

            errorStream.Subscribe(msg => ErrorMessage = msg);

            return validationChanged.Select(r => r.IsValid);
        }

        private IObservable<bool> SetupTouched<T>(Expression<Func<RegisterViewModel, T>> property)
        {
            return this.WhenAnyValue(property)
                .Skip(1)
                .Select(_ => true)
                .StartWith(false);
        }

        private ValidationResult ValidateDto()
        {
            var dto = new RegisterRequestDto
            {
                Login = Model.Username,
                DisplayName = Model.DisplayName,
                Email = Model.Email,
                Password = Model.Password
            };
            return _validator.Validate(dto);
        }

        private async Task RegisterAsync()
        {
            var dto = new RegisterRequestDto
            {
                Login = Model.Username,
                DisplayName = Model.DisplayName,
                Email = Model.Email,
                Password = Model.Password
            };

            try
            {
                await _authentication.RegisterAsync(dto);
                _coordinator.ShowLogin();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}