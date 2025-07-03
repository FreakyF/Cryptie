using System;
using System.Reactive;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Encryption;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Client.Features.PinCode.ViewModels;
using Cryptie.Common.Features.UserManagement.DTOs;
using Moq;
using ReactiveUI;
using Xunit;

namespace Cryptie.Client.Tests.Features.PinCode;

public class PinCodeViewModelTests
{
    private readonly Mock<IShellCoordinator> _shellCoordinator = new();
    private readonly Mock<IUserState> _userState = new();
    private readonly Mock<IKeychainManagerService> _keychain = new();
    private readonly Mock<IUserDetailsService> _userDetails = new();
    private readonly IScreen _screen = Mock.Of<IScreen>();

    private PinCodeViewModel CreateVm(UserPrivateKeyResponseDto? backendResponse = null)
    {
        var vm = new PinCodeViewModel(_screen, _shellCoordinator.Object, _userState.Object, _keychain.Object, _userDetails.Object);
        if (backendResponse != null)
        {
            typeof(PinCodeViewModel)
                .GetField("_backendResponse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(vm, backendResponse);
        }
        return vm;
    }

    [Fact]
    public void PinCode_Property_Works()
    {
        var vm = CreateVm();
        vm.PinCode = "123456";
        Assert.Equal("123456", vm.PinCode);
    }

    [Fact]
    public void Activator_Is_NotNull()
    {
        var vm = CreateVm();
        Assert.NotNull(vm.Activator);
    }

    [Fact]
    public void VerifyCommand_Rejects_Invalid_Pin_Length()
    {
        var vm = CreateVm(new UserPrivateKeyResponseDto());
        vm.PinCode = "123";
        var task = vm.VerifyCommand.Execute().ToTask();
        task.Wait();
        Assert.Equal("PIN must be exactly 6 digits.", vm.ErrorMessage);
    }

    [Fact]
    public void VerifyCommand_Rejects_Empty_Pin()
    {
        var vm = CreateVm(new UserPrivateKeyResponseDto());
        vm.PinCode = "";
        var task = vm.VerifyCommand.Execute().ToTask();
        task.Wait();
        Assert.Equal("PIN must be exactly 6 digits.", vm.ErrorMessage);
    }

    [Fact]
    public void VerifyCommand_Rejects_Null_BackendResponse()
    {
        var vm = CreateVm();
        vm.PinCode = "123456";
        var task = vm.VerifyCommand.Execute().ToTask();
        task.Wait();
        Assert.Equal("An error occurred. Please try again.", vm.ErrorMessage);
    }

    [Fact]
    public void VerifyCommand_Rejects_When_DecryptedControl_IsNull()
    {
        var pin = "123456";
        var login = "login";
        var aesKey = (string)typeof(PinCodeViewModel)
            .GetMethod("DeriveAesKeyFromPin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .Invoke(null, new object[] { pin });
        // Szyfrujemy inną wartość niż login, by deszyfrowanie nie pasowało
        var encryptedControl = AesDataEncryption.Encrypt("notlogin", aesKey);
        var backend = new UserPrivateKeyResponseDto { ControlValue = encryptedControl, PrivateKey = "irrelevant" };
        var vm = CreateVm(backend);
        vm.PinCode = pin;
        _userState.SetupGet(x => x.Login).Returns(login);
        var task = vm.VerifyCommand.Execute().ToTask();
        task.Wait();
        Assert.Equal("PIN is incorrect. Please try again", vm.ErrorMessage);
    }

    [Fact]
    public void VerifyCommand_Rejects_When_DecryptedControl_NotMatch_Login()
    {
        var pin = "123456";
        var login = "notmatch";
        var aesKey = (string)typeof(PinCodeViewModel)
            .GetMethod("DeriveAesKeyFromPin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .Invoke(null, new object[] { pin });
        // Szyfrujemy inną wartość niż login, by deszyfrowanie nie pasowało
        var encryptedControl = AesDataEncryption.Encrypt("somethingelse", aesKey);
        var backend = new UserPrivateKeyResponseDto { ControlValue = encryptedControl, PrivateKey = "irrelevant" };
        var vm = CreateVm(backend);
        vm.PinCode = pin;
        _userState.SetupGet(x => x.Login).Returns(login);
        var task = vm.VerifyCommand.Execute().ToTask();
        task.Wait();
        Assert.Equal("PIN is incorrect. Please try again", vm.ErrorMessage);
    }

    [Fact]
    public void VerifyCommand_Rejects_When_PrivateKey_Decryption_Fails()
    {
        var backend = new UserPrivateKeyResponseDto { ControlValue = Convert.ToBase64String(new byte[16]), PrivateKey = "badkey" };
        var vm = CreateVm(backend);
        vm.PinCode = "123456";
        _userState.SetupGet(x => x.Login).Returns((string)null);
        // Decrypt throws for private key
        var task = vm.VerifyCommand.Execute().ToTask();
        task.Wait();
        Assert.Equal("PIN is incorrect. Please try again", vm.ErrorMessage);
    }

    [Fact]
    public void VerifyCommand_Rejects_When_TrySavePrivateKey_Fails()
    {
        var pin = "123456";
        var login = "testuser";
        var aesKey = (string)typeof(PinCodeViewModel)
            .GetMethod("DeriveAesKeyFromPin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .Invoke(null, new object[] { pin });
        var encryptedControl = AesDataEncryption.Encrypt(login, aesKey);
        var privateKey = "PRIVATE_KEY";
        var encryptedPrivateKey = AesDataEncryption.Encrypt(privateKey, aesKey);
        var backend = new UserPrivateKeyResponseDto { ControlValue = encryptedControl, PrivateKey = encryptedPrivateKey };
        var vm = CreateVm(backend);
        vm.PinCode = pin;
        _userState.SetupGet(x => x.Login).Returns(login);
        _keychain.Setup(x => x.TrySavePrivateKey(privateKey, out It.Ref<string>.IsAny)).Returns(false);
        var task = vm.VerifyCommand.Execute().ToTask();
        task.Wait();
        Assert.Equal("An error occurred. Please try again.", vm.ErrorMessage);
    }

    [Fact]
    public void VerifyCommand_Success_CallsShowDashboard()
    {
        var pin = "123456";
        var login = "testuser";
        var aesKey = (string)typeof(PinCodeViewModel)
            .GetMethod("DeriveAesKeyFromPin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .Invoke(null, new object[] { pin });
        var encryptedControl = AesDataEncryption.Encrypt(login, aesKey);
        var privateKey = "PRIVATE_KEY";
        var encryptedPrivateKey = AesDataEncryption.Encrypt(privateKey, aesKey);
        var backend = new UserPrivateKeyResponseDto { ControlValue = encryptedControl, PrivateKey = encryptedPrivateKey };
        var vm = CreateVm(backend);
        vm.PinCode = pin;
        _userState.SetupGet(x => x.Login).Returns(login);
        _keychain.Setup(x => x.TrySavePrivateKey(privateKey, out It.Ref<string>.IsAny)).Returns(true);
        var called = false;
        _shellCoordinator.Setup(x => x.ShowDashboard()).Callback(() => called = true);
        var task = vm.VerifyCommand.Execute().ToTask();
        task.Wait();
        Assert.True(called);
    }

    [Fact]
    public void DeriveAesKeyFromPin_Works()
    {
        var method = typeof(PinCodeViewModel).GetMethod("DeriveAesKeyFromPin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        var result = (string)method.Invoke(null, new object[] { "123456" });
        Assert.False(string.IsNullOrWhiteSpace(result));
    }
}
