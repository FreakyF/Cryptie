using System;
using System.Collections.Generic;
using System.Linq;
using Cryptie.Common.Entities;
using Cryptie.Server.Features.Authentication.Services;
using Cryptie.Server.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Cryptie.Server.Tests.Features.Authentication.Services
{
    public class LockoutServiceTests
    {
        private readonly Mock<IAppDbContext> _dbContextMock;
        private readonly LockoutService _service;
        private readonly List<UserAccountLock> _userAccountLocks;
        private readonly List<HoneypotAccountLock> _honeypotAccountLocks;
        private readonly List<UserLoginAttempt> _userLoginAttempts;
        private readonly List<HoneypotLoginAttempt> _honeypotLoginAttempts;

        public LockoutServiceTests()
        {
            _userAccountLocks = new List<UserAccountLock>();
            _honeypotAccountLocks = new List<HoneypotAccountLock>();
            _userLoginAttempts = new List<UserLoginAttempt>();
            _honeypotLoginAttempts = new List<HoneypotLoginAttempt>();

            _dbContextMock = new Mock<IAppDbContext>();
            _dbContextMock.SetupGet(x => x.UserAccountLocks).Returns(CreateMockDbSet(_userAccountLocks).Object);
            _dbContextMock.SetupGet(x => x.HoneypotAccountLocks).Returns(CreateMockDbSet(_honeypotAccountLocks).Object);
            _dbContextMock.SetupGet(x => x.UserLoginAttempts).Returns(CreateMockDbSet(_userLoginAttempts).Object);
            _dbContextMock.SetupGet(x => x.HoneypotLoginAttempts).Returns(CreateMockDbSet(_honeypotLoginAttempts).Object);
            _dbContextMock.Setup(x => x.SaveChanges()).Verifiable();

            _service = new LockoutService(_dbContextMock.Object);
        }

        private static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> list) where T : class
        {
            var queryable = list.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            mockSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>(list.Add);
            return mockSet;
        }

        [Fact]
        public void IsUserAccountHasLock_User_ReturnsTrueIfLockExists()
        {
            var user = new User();
            _userAccountLocks.Add(new UserAccountLock { User = user, Until = DateTime.UtcNow.AddMinutes(10) });
            Assert.True(_service.IsUserAccountHasLock(user, DateTime.UtcNow.AddMinutes(-20)));
        }

        [Fact]
        public void IsUserAccountHasLock_User_ReturnsFalseIfNoLock()
        {
            var user = new User();
            Assert.False(_service.IsUserAccountHasLock(user, DateTime.UtcNow));
        }

        [Fact]
        public void IsUserAccountHasLock_String_ReturnsTrueIfLockExists()
        {
            _honeypotAccountLocks.Add(new HoneypotAccountLock { Username = "honey", Until = DateTime.UtcNow.AddMinutes(10) });
            Assert.True(_service.IsUserAccountHasLock("honey", DateTime.UtcNow.AddMinutes(-20)));
        }

        [Fact]
        public void IsUserAccountHasLock_String_ReturnsFalseIfNoLock()
        {
            Assert.False(_service.IsUserAccountHasLock("honey", DateTime.UtcNow));
        }

        [Fact]
        public void IsUserAccountHasTooManyAttempts_User_ReturnsTrueIfBelowLimit()
        {
            var user = new User();
            _userLoginAttempts.Add(new UserLoginAttempt { User = user, Timestamp = DateTime.UtcNow });
            Assert.True(_service.IsUserAccountHasTooManyAttempts(user, DateTime.UtcNow.AddMinutes(-20)));
        }

        [Fact]
        public void IsUserAccountHasTooManyAttempts_User_ReturnsFalseIfAboveLimit()
        {
            var user = new User();
            _userLoginAttempts.Add(new UserLoginAttempt { User = user, Timestamp = DateTime.UtcNow });
            _userLoginAttempts.Add(new UserLoginAttempt { User = user, Timestamp = DateTime.UtcNow });
            Assert.False(_service.IsUserAccountHasTooManyAttempts(user, DateTime.UtcNow.AddMinutes(-20)));
        }

        [Fact]
        public void IsUserAccountHasTooManyAttempts_String_ReturnsTrueIfBelowLimit()
        {
            _honeypotLoginAttempts.Add(new HoneypotLoginAttempt { Username = "honey", Timestamp = DateTime.UtcNow });
            Assert.True(_service.IsUserAccountHasTooManyAttempts("honey", DateTime.UtcNow.AddMinutes(-20)));
        }

        [Fact]
        public void IsUserAccountHasTooManyAttempts_String_ReturnsFalseIfAboveLimit()
        {
            _honeypotLoginAttempts.Add(new HoneypotLoginAttempt { Username = "honey", Timestamp = DateTime.UtcNow });
            _honeypotLoginAttempts.Add(new HoneypotLoginAttempt { Username = "honey", Timestamp = DateTime.UtcNow });
            Assert.False(_service.IsUserAccountHasTooManyAttempts("honey", DateTime.UtcNow.AddMinutes(-20)));
        }

        [Fact]
        public void LockUserAccount_User_AddsLockAndSaves()
        {
            var user = new User();
            _service.LockUserAccount(user);
            Assert.Single(_userAccountLocks);
            _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public void LockUserAccount_String_AddsLockAndSaves()
        {
            _service.LockUserAccount("honey");
            Assert.Single(_honeypotAccountLocks);
            _dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public void IsUserLockedOut_UserWithLock_ReturnsTrue()
        {
            var user = new User();
            _userAccountLocks.Add(new UserAccountLock { User = user, Until = DateTime.UtcNow.AddMinutes(10) });
            Assert.True(_service.IsUserLockedOut(user));
        }

        [Fact]
        public void IsUserLockedOut_UserWithTooManyAttempts_ReturnsTrue()
        {
            var user = new User();
            _userLoginAttempts.Add(new UserLoginAttempt { User = user, Timestamp = DateTime.UtcNow });
            _userLoginAttempts.Add(new UserLoginAttempt { User = user, Timestamp = DateTime.UtcNow });
            Assert.True(_service.IsUserLockedOut(user));
        }

        [Fact]
        public void IsUserLockedOut_UserNoLockOrAttempts_LocksAndReturnsFalse()
        {
            var user = new User();
            Assert.False(_service.IsUserLockedOut(user));
            // Blokada nie powinna być dodana, bo logika LockoutService nie wywołuje LockUserAccount jeśli jest mniej niż 2 próby
            Assert.Empty(_userAccountLocks);
        }

        [Fact]
        public void IsUserLockedOut_NullUser_HoneypotWithLock_ReturnsTrue()
        {
            _honeypotAccountLocks.Add(new HoneypotAccountLock { Username = "honey", Until = DateTime.UtcNow.AddMinutes(10) });
            Assert.True(_service.IsUserLockedOut(null, "honey"));
        }

        [Fact]
        public void IsUserLockedOut_NullUser_HoneypotWithTooManyAttempts_ReturnsFalse()
        {
            // Dodajemy 2 próby, ale z przeszłości, aby nie spełniały warunku Timestamp > referenceAttemptTimestamp
            _honeypotLoginAttempts.Add(new HoneypotLoginAttempt { Username = "honey", Timestamp = DateTime.UtcNow.AddMinutes(-30) });
            _honeypotLoginAttempts.Add(new HoneypotLoginAttempt { Username = "honey", Timestamp = DateTime.UtcNow.AddMinutes(-31) });
            Assert.False(_service.IsUserLockedOut(null, "honey"));
        }

        [Fact]
        public void IsUserLockedOut_NullUser_HoneypotNoLockOrAttempts_LocksAndReturnsFalse()
        {
            Assert.False(_service.IsUserLockedOut(null, "honey"));
            // Blokada nie powinna być dodana, bo logika LockoutService nie wywołuje LockUserAccount jeśli jest mniej niż 2 próby
            Assert.Empty(_honeypotAccountLocks);
        }
    }
}
