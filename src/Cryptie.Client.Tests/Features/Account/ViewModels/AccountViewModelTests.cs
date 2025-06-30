// using System;
// using System.Linq;
// using System.Reactive;
// using System.Reactive.Linq;
// using System.Threading.Tasks;
// using Cryptie.Client.Core.Navigation;
// using Cryptie.Client.Features.Account.Dependencies;
// using Cryptie.Client.Features.Account.ViewModels;
// using Cryptie.Client.Features.Account.services;
// using Cryptie.Client.Features.Authentication.Services;
// using Cryptie.Client.Features.Authentication.State;
// using Cryptie.Client.Features.Groups.State;
// using Cryptie.Client.Features.Menu.State;
// using Cryptie.Common.Features.UserManagement.DTOs;
// using FluentValidation;
// using FluentValidation.Results;
// using Moq;
// using ReactiveUI;
// using Xunit;
//
// namespace Cryptie.Client.Tests.Features.Account.ViewModels
// {
//     public class AccountViewModelTests
//     {
//         private readonly Mock<IKeychainManagerService> _keychain = new();
//         private readonly Mock<IShellCoordinator> _shell = new();
//         private readonly Mock<IAccountService> _account = new();
//         private readonly Mock<IValidator<UserDisplayNameRequestDto>> _validator = new();
//         private readonly Mock<IUserState> _userState = new();
//         private readonly Mock<IGroupSelectionState> _groupSelectionState = new();
//         private readonly Mock<ILoginState> _loginState = new();
//         private readonly Mock<IRegistrationState> _registrationState = new();
//         private readonly AccountDependencies _dependencies;
//         private readonly TestScreen _screen = new();
//
//         public AccountViewModelTests()
//         {
//             _dependencies = new AccountDependencies(
//                 _userState.Object,
//                 _groupSelectionState.Object,
//                 _loginState.Object,
//                 _registrationState.Object
//             );
//         }
//
//         [Fact]
//         public void Constructor_InitializesCommandsAndProperties()
//         {
//             // Arrange
//             _userState.SetupGet(x => x.Username).Returns("user");
//             _validator.Setup(x => x.Validate(It.IsAny<UserDisplayNameRequestDto>())).Returns(new ValidationResult());
//
//             // Act
//             var vm = CreateViewModel();
//
//             // Assert
//             Assert.NotNull(vm.ChangeNameCommand);
//             Assert.NotNull(vm.SignOutCommand);
//             Assert.Equal("user", vm.Username);
//         }
//
//         [Fact]
//         public void Username_Property_RaisesChangeNotification()
//         {
//             // Arrange
//             _userState.SetupGet(x => x.Username).Returns("user");
//             var vm = CreateViewModel();
//             bool raised = false;
//             vm.WhenAnyValue(x => x.Username).Skip(1).Subscribe(_ => raised = true);
//
//             // Act
//             vm.Username = "newuser";
//
//             // Assert
//             Assert.True(raised);
//             Assert.Equal("newuser", vm.Username);
//         }
//
//         [Fact]
//         public void ValidateDto_ReturnsValidationResult()
//         {
//             // Arrange
//             _userState.SetupGet(x => x.Username).Returns("user");
//             var expected = new ValidationResult();
//             _validator.Setup(x => x.Validate(It.IsAny<UserDisplayNameRequestDto>())).Returns(expected);
//             var vm = CreateViewModel();
//             vm.Username = "test";
//
//             // Act
//             var result = vm.GetType().GetMethod("ValidateDto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(vm, new object[] { });
//
//             // Assert
//             Assert.Same(expected, result);
//         }
//
//         [Fact]
//         public async Task ExecuteChangeNameAsync_ValidToken_UpdatesUserStateAndClearsError()
//         {
//             // Arrange
//             var token = Guid.NewGuid();
//             _userState.SetupGet(x => x.SessionToken).Returns(token.ToString());
//             _userState.SetupProperty(x => x.Username);
//             var vm = CreateViewModel();
//             vm.Username = "newname";
//             _validator.Setup(x => x.ValidateAndThrowAsync(It.IsAny<UserDisplayNameRequestDto>(), System.Threading.CancellationToken.None)).Returns(Task.CompletedTask);
//             _account.Setup(x => x.ChangeUserDisplayNameAsync(It.IsAny<UserDisplayNameRequestDto>())).Returns(Task.CompletedTask);
//
//             // Act
//             var method = vm.GetType().GetMethod("ExecuteChangeNameAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
//             var resultObj = method.Invoke(vm, null);
//             if (resultObj is Task resultTask)
//                 await resultTask;
//             else
//                 throw new InvalidOperationException("Method did not return Task");
//
//             // Assert
//             Assert.Equal("newname", _userState.Object.Username);
//             Assert.Equal(string.Empty, vm.ErrorMessage);
//         }
//
//         [Fact]
//         public async Task ExecuteChangeNameAsync_InvalidToken_DoesNothing()
//         {
//             // Arrange
//             _userState.SetupGet(x => x.SessionToken).Returns((string?)null);
//             var vm = CreateViewModel();
//             vm.Username = "newname";
//
//             // Act
//             var method = vm.GetType().GetMethod("ExecuteChangeNameAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
//             var resultObj = method.Invoke(vm, null);
//             if (resultObj is Task resultTask)
//                 await resultTask;
//             else
//                 throw new InvalidOperationException("Method did not return Task");
//
//             // Assert
//             _account.Verify(x => x.ChangeUserDisplayNameAsync(It.IsAny<UserDisplayNameRequestDto>()), Times.Never);
//         }
//
//         [Fact]
//         public async Task ExecuteChangeNameAsync_Throws_SetsErrorMessage()
//         {
//             // Arrange
//             var token = Guid.NewGuid();
//             _userState.SetupGet(x => x.SessionToken).Returns(token.ToString());
//             var vm = CreateViewModel();
//             vm.Username = "newname";
//             _validator.Setup(x => x.ValidateAndThrowAsync(It.IsAny<UserDisplayNameRequestDto>(), System.Threading.CancellationToken.None)).ThrowsAsync(new Exception());
//
//             // Act
//             var method = vm.GetType().GetMethod("ExecuteChangeNameAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
//             var resultObj = method.Invoke(vm, null);
//             if (resultObj is Task resultTask)
//                 await resultTask;
//             else
//                 throw new InvalidOperationException("Method did not return Task");
//
//             // Assert
//             Assert.Equal("An error occurred. Please try again.", vm.ErrorMessage);
//         }
//
//         [Fact]
//         public void ExecuteLogout_ClearsAllStateAndCallsShell()
//         {
//             // Arrange
//             _userState.SetupProperty(x => x.Username);
//             _userState.SetupProperty(x => x.SessionToken);
//             _userState.SetupProperty(x => x.UserId);
//             _groupSelectionState.SetupProperty(x => x.SelectedGroupId);
//             _groupSelectionState.SetupProperty(x => x.SelectedGroupName);
//             _groupSelectionState.SetupProperty(x => x.IsGroupPrivate);
//             _loginState.SetupProperty(x => x.LastResponse);
//             _registrationState.SetupProperty(x => x.LastResponse);
//             var vm = CreateViewModel();
//
//             // Act
//             vm.GetType().GetMethod("ExecuteLogout", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(vm, null);
//
//             // Assert
//             Assert.Null(_userState.Object.Username);
//             Assert.Null(_userState.Object.SessionToken);
//             Assert.Null(_userState.Object.UserId);
//             Assert.Equal(Guid.Empty, _groupSelectionState.Object.SelectedGroupId);
//             Assert.Null(_groupSelectionState.Object.SelectedGroupName);
//             Assert.False(_groupSelectionState.Object.IsGroupPrivate);
//             Assert.Null(_loginState.Object.LastResponse);
//             Assert.Null(_registrationState.Object.LastResponse);
//             _shell.Verify(x => x.ResetAndShowLogin(), Times.Once);
//             _keychain.Verify(x => x.TryClearSessionToken(out It.Ref<string?>.IsAny), Times.Once);
//         }
//
//         private AccountViewModel CreateViewModel() => new(
//             _screen,
//             _keychain.Object,
//             _shell.Object,
//             _account.Object,
//             _validator.Object,
//             _dependencies);
//
//         private class TestScreen : IScreen
//         {
//             public RoutingState Router { get; } = new();
//         }
//     }
// }
