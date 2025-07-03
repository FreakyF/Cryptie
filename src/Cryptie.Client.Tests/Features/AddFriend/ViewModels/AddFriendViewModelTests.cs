using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.AddFriend.Services;
using Cryptie.Client.Features.AddFriend.ViewModels;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.KeysManagement.DTOs;
using Cryptie.Common.Features.UserManagement.DTOs;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using ReactiveUI;

namespace Cryptie.Client.Tests.Features.AddFriend
{
    public class AddFriendViewModelTests
    {
        private readonly Mock<IFriendsService> _friendsService = new();
        private readonly Mock<IKeyService> _keyService = new();
        private readonly Mock<IUserDetailsService> _userDetailsService = new();
        private readonly Mock<IValidator<AddFriendRequestDto>> _validator = new();
        private readonly Mock<IUserState> _userState = new();
        private readonly Mock<IScreen> _screen = new();

        private AddFriendViewModel CreateVm()
        {
            return new AddFriendViewModel(
                _screen.Object,
                _friendsService.Object,
                _userDetailsService.Object,
                _keyService.Object,
                _validator.Object,
                _userState.Object
            );
        }

        [Fact]
        public void Constructor_InitializesPropertiesAndCommands()
        {
            var vm = CreateVm();
            Assert.NotNull(vm.SendFriendRequest);
            Assert.Equal(string.Empty, vm.FriendInput);
        }

        [Fact]
        public void FriendInput_Property_Works()
        {
            var vm = CreateVm();
            vm.FriendInput = "test";
            Assert.Equal("test", vm.FriendInput);
        }

        [Fact]
        public void ConfirmationMessage_Property_Works()
        {
            var vm = CreateVm();
            var prop = vm.GetType().GetProperty("ConfirmationMessage");
            prop.SetValue(vm, "ok");
            Assert.Equal("ok", prop.GetValue(vm));
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("not-a-guid", false)]
        [InlineData("b8a7e6e2-2b2e-4e2a-8e2e-2b2e4e2a8e2e", true)]
        public void IsSessionTokenValid_Works(string token, bool expected)
        {
            var method = typeof(AddFriendViewModel).GetMethod("IsSessionTokenValid", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var args = new object[] { token, null };
            var result = (bool)method.Invoke(null, args);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("user", "user", true)]
        [InlineData("user", "USER", true)]
        [InlineData("user", "other", false)]
        public void IsAddingSelf_Works(string user, string friend, bool expected)
        {
            var method = typeof(AddFriendViewModel).GetMethod("IsAddingSelf", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var result = (bool)method.Invoke(null, new object[] { user, friend });
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GenerateAesKey_ReturnsKey()
        {
            var method = typeof(AddFriendViewModel).GetMethod("GenerateAesKey", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var key = (byte[])method.Invoke(null, null);
            Assert.NotNull(key);
            Assert.True(key.Length > 0);
        }

        [Fact]
        public void SetGenericError_SetsErrorMessage()
        {
            var vm = CreateVm();
            var method = typeof(AddFriendViewModel).GetMethod("SetGenericError", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            method.Invoke(vm, null);
            Assert.Equal("An error occurred. Please try again.", vm.ErrorMessage);
        }

        [Fact]
        public async Task AddFriendAsync_InvalidSessionToken_SetsGenericError()
        {
            var vm = CreateVm();
            _userState.SetupGet(x => x.SessionToken).Returns((string)null);
            vm.FriendInput = "friend";
            await vm.SendFriendRequest.Execute().Catch(Observable.Return(Unit.Default));
            Assert.Equal("An error occurred. Please try again.", vm.ErrorMessage);
        }

        [Fact]
        public async Task AddFriendAsync_AddingSelf_SetsError()
        {
            var vm = CreateVm();
            _userState.SetupGet(x => x.SessionToken).Returns(Guid.NewGuid().ToString());
            _userState.SetupGet(x => x.Username).Returns("user");
            vm.FriendInput = "user";
            await vm.SendFriendRequest.Execute().Catch(Observable.Return(Unit.Default));
            Assert.Equal("You cannot add yourself!", vm.ErrorMessage);
        }

        [Fact]
        public async Task AddFriendAsync_InvalidFriendRequest_SetsError()
        {
            var vm = CreateVm();
            _userState.SetupGet(x => x.SessionToken).Returns(Guid.NewGuid().ToString());
            _userState.SetupGet(x => x.Username).Returns("user");
            vm.FriendInput = "friend";
            _validator.Setup(x => x.ValidateAsync(It.IsAny<AddFriendRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new("Friend", "err") }));
            await vm.SendFriendRequest.Execute().Catch(Observable.Return(Unit.Default));
            Assert.Equal("User not found!", vm.ErrorMessage);
        }

        [Fact]
        public async Task AddFriendAsync_UserNotFound_SetsError()
        {
            var vm = CreateVm();
            _userState.SetupGet(x => x.SessionToken).Returns(Guid.NewGuid().ToString());
            _userState.SetupGet(x => x.Username).Returns("user");
            vm.FriendInput = "friend";
            _validator.Setup(x => x.ValidateAsync(It.IsAny<AddFriendRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            _userDetailsService.Setup(x => x.GetGuidFromLoginAsync(It.IsAny<GuidFromLoginRequestDto>(), It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException(null, null, HttpStatusCode.NotFound));
            await vm.SendFriendRequest.Execute().Catch(Observable.Return(Unit.Default));
            Assert.Equal("User not found!", vm.ErrorMessage);
        }

        [Fact]
        public async Task AddFriendAsync_CertificateNull_SetsGenericError()
        {
            var vm = CreateVm();
            _userState.SetupGet(x => x.SessionToken).Returns(Guid.NewGuid().ToString());
            _userState.SetupGet(x => x.Username).Returns("user");
            vm.FriendInput = "friend";
            _validator.Setup(x => x.ValidateAsync(It.IsAny<AddFriendRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            _userDetailsService.Setup(x => x.GetGuidFromLoginAsync(It.IsAny<GuidFromLoginRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GuidFromLoginResponseDto { UserId = Guid.NewGuid() });
            _keyService.Setup(x => x.GetUserKeyAsync(It.IsAny<GetUserKeyRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync((GetUserKeyResponseDto)null);
            await vm.SendFriendRequest.Execute().Catch(Observable.Return(Unit.Default));
            Assert.Equal("An error occurred. Please try again.", vm.ErrorMessage);
        }

        

        [Fact]
        public async Task TryAddFriend_UserNotFound_SetsError()
        {
            var vm = CreateVm();
            var method = typeof(AddFriendViewModel).GetMethod("TryAddFriend", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            _friendsService.Setup(x => x.AddFriendAsync(It.IsAny<AddFriendRequestDto>(), It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException(null, null, HttpStatusCode.NotFound));
            var result = await (Task<bool>)method.Invoke(vm, new object[] { new AddFriendRequestDto(), CancellationToken.None });
            Assert.False(result);
            Assert.Equal("User not found!", vm.ErrorMessage);
        }

        [Fact]
        public async Task TryAddFriend_Throws_SetsGenericError()
        {
            var vm = CreateVm();
            var method = typeof(AddFriendViewModel).GetMethod("TryAddFriend", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            _friendsService.Setup(x => x.AddFriendAsync(It.IsAny<AddFriendRequestDto>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            var result = await (Task<bool>)method.Invoke(vm, new object[] { new AddFriendRequestDto(), CancellationToken.None });
            Assert.False(result);
            Assert.Equal("An error occurred. Please try again.", vm.ErrorMessage);
        }
    }
}
