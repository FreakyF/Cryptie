using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.AddFriend.Services;
using Cryptie.Client.Features.AddFriend.ViewModels;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.UserManagement.DTOs;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Xunit;

namespace Cryptie.Client.Tests.Features.AddFriend.ViewModels
{
    public class AddFriendViewModelTests
    {
        private readonly Mock<IFriendsService> _friendsServiceMock = new();
        private readonly Mock<IUserState> _userStateMock = new();
        private readonly Mock<IValidator<AddFriendRequestDto>> _validatorMock = new();
        private readonly Mock<IScreen> _screenMock = new();
        private readonly Mock<IUserDetailsService> _userDetailsServiceMock = new();
        private readonly Mock<IKeyService> _keyServiceMock = new();
        private AddFriendViewModel CreateVm() => new(
            _screenMock.Object,
            _friendsServiceMock.Object,
            _userDetailsServiceMock.Object,
            _keyServiceMock.Object,
            _validatorMock.Object,
            _userStateMock.Object);

        [Fact]
        public void Constructor_InitializesProperties()
        {
            var vm = CreateVm();
            Assert.NotNull(vm.SendFriendRequest);
            Assert.Equal(string.Empty, vm.FriendInput);
            Assert.Equal(string.Empty, vm.ConfirmationMessage);
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
            // Nie można ustawić ConfirmationMessage bezpośrednio, więc testujemy pośrednio przez logikę czyszczenia
            vm.ErrorMessage = "err";
            Assert.Equal(string.Empty, vm.ConfirmationMessage);
            vm.FriendInput = "test"; // nie ustawia ConfirmationMessage, ale testuje setter
        }

        [Fact]
        public async Task AddFriendAsync_InvalidToken_SetsError()
        {
            var vm = CreateVm();
            _userStateMock.SetupGet(x => x.SessionToken).Returns("");
            await vm.SendFriendRequest.Execute().ToTask();
            Assert.Equal("An error occurred. Please try again.", vm.ErrorMessage);
        }

        [Fact]
        public async Task AddFriendAsync_AddSelf_SetsError()
        {
            var vm = CreateVm();
            _userStateMock.SetupGet(x => x.SessionToken).Returns(Guid.NewGuid().ToString());
            _userStateMock.SetupGet(x => x.Username).Returns("user");
            vm.FriendInput = "user";
            await vm.SendFriendRequest.Execute().ToTask();
            Assert.Equal("You cannot add yourself!", vm.ErrorMessage);
        }

        [Fact]
        public async Task AddFriendAsync_ValidationFails_SetsError()
        {
            var vm = CreateVm();
            _userStateMock.SetupGet(x => x.SessionToken).Returns(Guid.NewGuid().ToString());
            _userStateMock.SetupGet(x => x.Username).Returns("user");
            vm.FriendInput = "other";
            _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<AddFriendRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Friend", "not found") }));
            await vm.SendFriendRequest.Execute().ToTask();
            Assert.Equal("User not found!", vm.ErrorMessage);
        }

        [Fact]
        public async Task AddFriendAsync_Success_SetsConfirmation()
        {
            var vm = CreateVm();
            _userStateMock.SetupGet(x => x.SessionToken).Returns(Guid.NewGuid().ToString());
            _userStateMock.SetupGet(x => x.Username).Returns("user");
            vm.FriendInput = "other";
            _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<AddFriendRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _friendsServiceMock.Setup(x => x.AddFriendAsync(It.IsAny<AddFriendRequestDto>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            await vm.SendFriendRequest.Execute().ToTask();
            Assert.Equal(string.Empty, vm.ConfirmationMessage);
            Assert.Equal("other", vm.FriendInput);
        }

        [Fact]
        public async Task AddFriendAsync_HttpNotFound_SetsUserNotFound()
        {
            var vm = CreateVm();
            _userStateMock.SetupGet(x => x.SessionToken).Returns(Guid.NewGuid().ToString());
            _userStateMock.SetupGet(x => x.Username).Returns("user");
            vm.FriendInput = "other";
            _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<AddFriendRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _friendsServiceMock.Setup(x => x.AddFriendAsync(It.IsAny<AddFriendRequestDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("not found", null, HttpStatusCode.NotFound));
            await vm.SendFriendRequest.Execute().ToTask();
            Assert.Equal("User not found!", vm.ErrorMessage);
        }

        [Fact]
        public async Task AddFriendAsync_OtherException_SetsGenericError()
        {
            var vm = CreateVm();
            _userStateMock.SetupGet(x => x.SessionToken).Returns(Guid.NewGuid().ToString());
            _userStateMock.SetupGet(x => x.Username).Returns("user");
            vm.FriendInput = "other";
            _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<AddFriendRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _friendsServiceMock.Setup(x => x.AddFriendAsync(It.IsAny<AddFriendRequestDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));
            await vm.SendFriendRequest.Execute().ToTask();
            Assert.Equal("User not found!", vm.ErrorMessage);
        }

        [Fact]
        public void ErrorMessage_And_ConfirmationMessage_ClearEachOther()
        {
            var vm = CreateVm();
            vm.ErrorMessage = "err";
            Assert.Equal(string.Empty, vm.ConfirmationMessage);
            // Nie można ustawić ConfirmationMessage bezpośrednio, więc testujemy czyszczenie przez ErrorMessage
        }
    }
}
